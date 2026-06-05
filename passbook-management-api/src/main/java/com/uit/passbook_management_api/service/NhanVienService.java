package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.DoiMatKhauRequest;
import com.uit.passbook_management_api.dto.request.CapNhatNhanVienRequest;
import com.uit.passbook_management_api.dto.request.NhanVienRequest;
import com.uit.passbook_management_api.dto.response.NhanVienResponse;
import com.uit.passbook_management_api.entity.AppUser;
import com.uit.passbook_management_api.entity.NhanVien;
import com.uit.passbook_management_api.repository.AppUserRepository;
import com.uit.passbook_management_api.repository.NhanVienRepository;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.text.Normalizer;
import java.util.List;
import java.util.regex.Pattern;

@Service
public class NhanVienService {

    private final NhanVienRepository nhanVienRepository;
    private final AppUserRepository appUserRepository;
    private final PasswordEncoder passwordEncoder;
    private final AuditLogService auditLogService;

    public NhanVienService(NhanVienRepository nhanVienRepository,
                           AppUserRepository appUserRepository,
                           PasswordEncoder passwordEncoder, AuditLogService auditLogService) {
        this.nhanVienRepository = nhanVienRepository;
        this.appUserRepository = appUserRepository;
        this.passwordEncoder = passwordEncoder;
        this.auditLogService = auditLogService;
    }

    public List<NhanVien> layDanhSachNhanVien() {
        return nhanVienRepository.findAll();
    }

    @Transactional
    public NhanVienResponse themNhanVienMoi(NhanVienRequest request) {
        if (nhanVienRepository.existsByCccd(request.getCccd())) {
            throw new RuntimeException("Số CCCD này đã tồn tại trong hệ thống!");
        }

        String lastId = nhanVienRepository.findLastId();
        String nextId = "NV1"; // Mặc định nếu DB chưa có nhân viên nào
        if (lastId != null) {
            // Cắt bỏ 2 ký tự "NV", lấy phần số đổi sang kiểu Int và cộng thêm 1
            int currentNumber = Integer.parseInt(lastId.substring(2));
            nextId = "NV" + (currentNumber + 1);
        }
        // 1. Tự động sinh Username từ Họ và Tên
        String baseUsername = chuyenDoiHotenThanhUsername(request.getHoTen());
        String finalUsername = baseUsername;
        int count = 1;

        // Vòng lặp kiểm tra trùng tài khoản: nếu trùng sẽ tự động thêm số (ví dụ: nguyenan, nguyenan1, nguyenan2...)
        while (appUserRepository.findByUsername(finalUsername).isPresent()) {
            finalUsername = baseUsername + count;
            count++;
        }

        // 2. Định nghĩa mật khẩu mặc định
        String macDinhPassword = "123456";

        // 3. Tạo tài khoản hệ thống (Mã hóa mật khẩu)
        AppUser user = new AppUser();
        user.setUsername(finalUsername);
        user.setPassword(passwordEncoder.encode(macDinhPassword));
        user.setRole(request.getRole());
        user = appUserRepository.save(user);

        // 4. Tạo hồ sơ nhân viên
        NhanVien nv = new NhanVien();
        nv.setId(nextId);
        nv.setHoTen(request.getHoTen());
        nv.setSoDienThoai(request.getSoDienThoai());
        nv.setCccd(request.getCccd());
        nv.setAppUser(user);
        nhanVienRepository.save(nv);

        // 5. Trả về thông tin kèm theo tài khoản mật khẩu dạng tường minh (Plain text)
        return NhanVienResponse.builder()
                .hoTen(nv.getHoTen())
                .soDienThoai(nv.getSoDienThoai())
                .cccd(nv.getCccd())
                .username(finalUsername)
                .password(macDinhPassword)
                .role(user.getRole())
                .build();
    }

    @Transactional
    public NhanVien capNhatThongTin(String id, CapNhatNhanVienRequest request) {
        // 1. Kiểm tra nhân viên có tồn tại không
        NhanVien nv = nhanVienRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Không tìm thấy nhân viên có mã: " + id));

        // 2. Kiểm tra trùng CCCD với các tài khoản khác trong hệ thống
        if (nhanVienRepository.existsByCccdAndIdNot(request.getCccd(), id)) {
            throw new RuntimeException("Số CCCD này đã tồn tại ở một hồ sơ nhân viên khác!");
        }

        // 3. Cập nhật các thông tin cá nhân được phép
        nv.setHoTen(request.getHoTen());
        nv.setSoDienThoai(request.getSoDienThoai());
        nv.setCccd(request.getCccd());

        // 4. Lưu lại vào DB và trả về đối tượng hồ sơ sau khi sửa
        return nhanVienRepository.save(nv);
    }



    private String chuyenDoiHotenThanhUsername(String hoTen) {
        if (hoTen == null || hoTen.trim().isEmpty()) {
            throw new RuntimeException("Họ tên nhân viên không được để trống!");
        }

        // Loại bỏ dấu tiếng Việt
        String temp = Normalizer.normalize(hoTen, Normalizer.Form.NFD);
        Pattern pattern = Pattern.compile("\\p{InCombiningDiacriticalMarks}+");
        String cleanString = pattern.matcher(temp).replaceAll("")
                .replace('đ', 'd')
                .replace('Đ', 'd');

        // Tách các từ dựa trên khoảng trắng
        String[] words = cleanString.trim().toLowerCase().split("\\s+");

        if (words.length == 1) {
            return words[0];
        }

        return words[words.length - 1] + words[0];
    }

    @Transactional
    public String doiMatKhau(DoiMatKhauRequest request) {
        // Lấy username của người đang gọi API từ JWT Token trong Context
        String username = SecurityContextHolder.getContext().getAuthentication().getName();

        // Tìm tài khoản trong Database
        AppUser user = appUserRepository.findByUsername(username)
                .orElseThrow(() -> new RuntimeException("Không tìm thấy tài khoản trong hệ thống!"));

        // Kiểm tra mật khẩu cũ có khớp không
        if (!passwordEncoder.matches(request.getMatKhauCu(), user.getPassword())) {
            throw new RuntimeException("Mật khẩu cũ không chính xác!");
        }

        // 2. BỔ SUNG: Kiểm tra mật khẩu mới có bị trùng mật khẩu cũ không
        if (passwordEncoder.matches(request.getMatKhauMoi(), user.getPassword())) {
            throw new RuntimeException("Mật khẩu mới không được trùng với mật khẩu hiện tại!");
        }

        // Mã hóa mật khẩu mới và lưu lại
        user.setPassword(passwordEncoder.encode(request.getMatKhauMoi()));
        appUserRepository.save(user);

        // 3. BỔ SUNG: Ghi nhận vết vào Audit Log phục vụ kiểm toán bảo mật
        auditLogService.ghiLog(
                "ĐỔI MẬT KHẨU",
                "Tài khoản " + username + " đã tự thay đổi mật khẩu thành công."
        );

        return "Đổi mật khẩu thành công!";
    }
}