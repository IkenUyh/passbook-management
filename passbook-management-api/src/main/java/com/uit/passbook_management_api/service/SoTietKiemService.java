package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.MoSoRequest;
import com.uit.passbook_management_api.entity.KhachHang;
import com.uit.passbook_management_api.entity.SoTietKiem;
import com.uit.passbook_management_api.repository.KhachHangRepository;
import com.uit.passbook_management_api.repository.SoTietKiemRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

@Service
public class SoTietKiemService {

    @Autowired
    private SoTietKiemRepository soTietKiemRepository;

    // Phải gọi cả Khách Hàng Repo để lưu thông tin khách hàng trước
    @Autowired
    private KhachHangRepository khachHangRepository;

    // Dùng @Transactional để đảm bảo nếu lưu sổ lỗi thì nó tự rollback khách hàng
    @Transactional
    public String moSoTietKiem(MoSoRequest request) {
        BigDecimal tienToiThieu = new BigDecimal("1000000");
        if (request.getSoTienGuiBanDau().compareTo(tienToiThieu) < 0) {
            throw new RuntimeException("Số tiền gửi ban đầu không đạt mức tối thiểu: 1.000.000đ");
        }

        // BƯỚC 1: Xử lý thông tin Khách Hàng
        // Tạm thời tạo mới luôn, nếu mốt nâng cấp thì check xem CMND có trong DB chưa
        KhachHang kh = new KhachHang();
        kh.setTen(request.getTenKhachHang());
        kh.setCmnd(request.getCmnd());
        kh.setDiaChi(request.getDiaChi());
        // kh.setSdt(...); // Thêm sdt nếu Request của ông có

        // Cực kỳ quan trọng: Phải save Khách Hàng trước để lấy ID (khóa ngoại)
        kh = khachHangRepository.save(kh);

        // BƯỚC 2: Tạo Sổ Tiết Kiệm
        SoTietKiem stk = new SoTietKiem();
        String maSoMoi = "STK" + System.currentTimeMillis();

        stk.setId(maSoMoi);
        stk.setKhachHang(kh); // Gắn ông khách hàng vừa tạo vào đây
        stk.setLoaiTietKiem(request.getLoaiTietKiem());
        stk.setSoDu(request.getSoTienGuiBanDau());
        stk.setNgayMo(LocalDate.now());
        stk.setTrangThai("DANG_HOAT_DONG"); // Trạng thái mặc định khi mới tạo

        // BƯỚC 3: LƯU XUỐNG DATABASE
        soTietKiemRepository.save(stk);

        return "Mở sổ thành công! Mã sổ: " + maSoMoi;
    }

    public List<SoTietKiem> layDanhSachSoTietKiem() {
        return soTietKiemRepository.findAll();
    }
}