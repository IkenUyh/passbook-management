package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.NhanVienRequest;
import com.uit.passbook_management_api.entity.AppUser;
import com.uit.passbook_management_api.entity.NhanVien;
import com.uit.passbook_management_api.repository.AppUserRepository;
import com.uit.passbook_management_api.repository.NhanVienRepository;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class NhanVienService {

    private final NhanVienRepository nhanVienRepository;
    private final AppUserRepository appUserRepository;
    private final PasswordEncoder passwordEncoder;

    public NhanVienService(NhanVienRepository nhanVienRepository,
                           AppUserRepository appUserRepository,
                           PasswordEncoder passwordEncoder) {
        this.nhanVienRepository = nhanVienRepository;
        this.appUserRepository = appUserRepository;
        this.passwordEncoder = passwordEncoder;
    }

    public List<NhanVien> layDanhSachNhanVien() {
        return nhanVienRepository.findAll();
    }

    @Transactional
    public NhanVien themNhanVienMoi(NhanVienRequest request) {
        // 1. Kiểm tra trùng tài khoản
        if (appUserRepository.findByUsername(request.getUsername()).isPresent()) {
            throw new RuntimeException("Tên tài khoản đăng nhập này đã tồn tại!");
        }

        // 2. Tạo tài khoản hệ thống (Mã hóa mật khẩu)
        AppUser user = new AppUser();
        user.setUsername(request.getUsername());
        user.setPassword(passwordEncoder.encode(request.getPassword()));
        user.setRole(request.getRole());
        user = appUserRepository.save(user);

        // 3. Tạo hồ sơ nhân viên rút gọn
        NhanVien nv = new NhanVien();
        nv.setHoTen(request.getHoTen());
        nv.setSoDienThoai(request.getSoDienThoai());
        nv.setCccd(request.getCccd());
        nv.setAppUser(user);

        return nhanVienRepository.save(nv);
    }
}