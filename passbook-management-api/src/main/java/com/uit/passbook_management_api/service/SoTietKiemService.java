package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.MoSoRequest;
import com.uit.passbook_management_api.entity.*;
import com.uit.passbook_management_api.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class SoTietKiemService {

    private final SoTietKiemRepository soTietKiemRepository;
    private final KhachHangRepository khachHangRepository;
    private final LoaiTietKiemRepository loaiTietKiemRepository;
    private final ThamSoRepository thamSoRepository;

    public SoTietKiemService(SoTietKiemRepository soTietKiemRepository,
                             KhachHangRepository khachHangRepository,
                             LoaiTietKiemRepository loaiTietKiemRepository,
                             ThamSoRepository thamSoRepository) {
        this.soTietKiemRepository = soTietKiemRepository;
        this.khachHangRepository = khachHangRepository;
        this.loaiTietKiemRepository = loaiTietKiemRepository;
        this.thamSoRepository = thamSoRepository;
    }

    // FR4: Tra cứu danh sách sổ
    public List<SoTietKiem> layDanhSachSoTietKiem() {
        return soTietKiemRepository.findAll();
    }

    // FR1: Mở sổ tiết kiệm
    @Transactional
    public String moSoTietKiem(MoSoRequest request) {
        // 1. Lấy tham số cấu hình (Giả sử id = 1 luôn tồn tại như file SQL)
        ThamSo thamSo = thamSoRepository.findById(1)
                .orElseThrow(() -> new RuntimeException("Chưa thiết lập tham số hệ thống!"));

        // 2. Validate số tiền tối thiểu
        if (request.getSoTienGuiBanDau().compareTo(thamSo.getTienGuiBanDauToiThieu()) < 0) {
            throw new RuntimeException("Số tiền gửi ban đầu không đạt mức tối thiểu: "
                    + thamSo.getTienGuiBanDauToiThieu() + "đ");
        }

        // 3. Kiểm tra loại tiết kiệm (Client sẽ gửi 'KKH', '3T', hoặc '6T')
        LoaiTietKiem loaiTk = loaiTietKiemRepository.findById(request.getLoaiTietKiem())
                .orElseThrow(() -> new RuntimeException("Loại tiết kiệm không hợp lệ!"));

        // 4. Tìm Khách hàng bằng CMND, nếu chưa có thì tạo mới
        Optional<KhachHang> khOptional = khachHangRepository.findByCmnd(request.getCmnd());
        KhachHang khachHang;

        if (khOptional.isPresent()) {
            khachHang = khOptional.get();
            // Cập nhật địa chỉ nếu có sự thay đổi
            khachHang.setDiaChi(request.getDiaChi());
            khachHangRepository.save(khachHang);
        } else {
            khachHang = new KhachHang();
            khachHang.setTen(request.getTenKhachHang());
            khachHang.setCmnd(request.getCmnd());
            khachHang.setDiaChi(request.getDiaChi());
            khachHang = khachHangRepository.save(khachHang);
        }

        // 5. Khởi tạo Sổ tiết kiệm mới
        SoTietKiem stk = new SoTietKiem();
        // Generate mã sổ ngẫu nhiên, ví dụ: STK-2a9b...
        String maSo = "STK-" + UUID.randomUUID().toString().substring(0, 8).toUpperCase();

        stk.setId(maSo);
        stk.setKhachHang(khachHang);
        stk.setLoaiTietKiem(loaiTk);
        stk.setSoDu(request.getSoTienGuiBanDau());
        stk.setNgayMo(LocalDate.now());

        // 6. Tính ngày đáo hạn nếu là loại CÓ kỳ hạn
        if (loaiTk.getKyHan() > 0) {
            stk.setNgayDaoHan(LocalDate.now().plusMonths(loaiTk.getKyHan()));
        }

        stk.setTrangThai("DANG_HOAT_DONG");

        soTietKiemRepository.save(stk);

        return maSo;
    }
}