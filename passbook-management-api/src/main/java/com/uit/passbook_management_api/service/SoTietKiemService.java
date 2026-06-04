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

    // BỔ SUNG 2 REPO NÀY ĐỂ KHỚP VỚI BÁO CÁO TRANG 1
    private final PhieuGuiTienRepository phieuGuiTienRepository;
    private final AuditLogService auditLogService;

    public SoTietKiemService(SoTietKiemRepository soTietKiemRepository,
                             KhachHangRepository khachHangRepository,
                             LoaiTietKiemRepository loaiTietKiemRepository,
                             ThamSoRepository thamSoRepository,
                             PhieuGuiTienRepository phieuGuiTienRepository,
                             AuditLogService auditLogService) {
        this.soTietKiemRepository = soTietKiemRepository;
        this.khachHangRepository = khachHangRepository;
        this.loaiTietKiemRepository = loaiTietKiemRepository;
        this.thamSoRepository = thamSoRepository;
        this.phieuGuiTienRepository = phieuGuiTienRepository;
        this.auditLogService = auditLogService;
    }

    // Lấy danh sách sổ sắp đáo hạn cho FR8
    public List<SoTietKiem> layDanhSachSapDaoHan(int soNgayBaoTruoc) {
        LocalDate homNay = LocalDate.now();
        LocalDate ngayMucTieu = homNay.plusDays(soNgayBaoTruoc);
        return soTietKiemRepository.timSoSapDaoHan(homNay, ngayMucTieu);
    }

    // FR4: Tra cứu danh sách sổ
    public List<SoTietKiem> layDanhSachSoTietKiem() {
        return soTietKiemRepository.findAll();
    }

    // FR1: Mở sổ tiết kiệm
    @Transactional
    public String moSoTietKiem(MoSoRequest request) {
        // 1. Lấy tham số cấu hình
        ThamSo thamSo = thamSoRepository.findById(1)
                .orElseThrow(() -> new RuntimeException("Chưa thiết lập tham số hệ thống!"));

        // 2. Validate số tiền tối thiểu
        if (request.getSoTienGuiBanDau().compareTo(thamSo.getTienGuiBanDauToiThieu()) < 0) {
            throw new RuntimeException("Số tiền gửi ban đầu không đạt mức tối thiểu: "
                    + thamSo.getTienGuiBanDauToiThieu() + "đ");
        }

        // 3. Kiểm tra loại tiết kiệm
        LoaiTietKiem loaiTk = loaiTietKiemRepository.findById(request.getLoaiTietKiem())
                .orElseThrow(() -> new RuntimeException("Loại tiết kiệm không hợp lệ!"));

        // 4. Tìm Khách hàng bằng CMND, nếu chưa có thì tạo mới
        Optional<KhachHang> khOptional = khachHangRepository.findByCmnd(request.getCmnd());
        KhachHang khachHang;

        if (khOptional.isPresent()) {
            khachHang = khOptional.get();
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

        // 7. FIX: TẠO PHIẾU GỬI TIỀN BAN ĐẦU (Khớp báo cáo bước 7)
        PhieuGuiTien phieu = new PhieuGuiTien();
        phieu.setSoTienGui(request.getSoTienGuiBanDau());
        phieu.setNgayGui(LocalDate.now());
        phieu.setSoTietKiem(stk);
        phieu.setKhachHang(khachHang);
        phieuGuiTienRepository.save(phieu);

        // 8. FIX: GHI LOG HỆ THỐNG (Khớp báo cáo bước 9)
        auditLogService.ghiLog("MỞ SỔ", "Mở sổ mới: " + maSo + " | Khách hàng: " + khachHang.getTen() + " | Gửi ban đầu: " + request.getSoTienGuiBanDau() + "đ");

        return maSo;
    }
}