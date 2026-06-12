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

    // [MỚI / UPDATE]: Đồng bộ hóa logic lấy danh sách sắp đáo hạn cho cả Controller (WPF) và Job
    public List<SoTietKiem> layDanhSachSapDaoHan(int soNgayBaoTruoc) {
        LocalDate homNay = LocalDate.now();
        LocalDate ngayMucTieu = homNay.plusDays(soNgayBaoTruoc);
        return soTietKiemRepository.timSoSapDaoHan(homNay, ngayMucTieu);
    }

    // [MỚI]: Hàm dành riêng cho Job xử lý tự động khi đến ĐÚNG ngày đáo hạn hôm nay
    @Transactional
    public void xuLySotietKiemDenHanHomNay() {
        LocalDate homNay = LocalDate.now();
        List<SoTietKiem> danhSachDenHan = soTietKiemRepository.findByNgayDaoHanAndTrangThai(homNay, "DANG_HOAT_DONG");

        if (!danhSachDenHan.isEmpty()) {
            for (SoTietKiem stk : danhSachDenHan) {
                // Logic thực tế: Bạn có thể cập nhật trạng thái hoặc thực hiện tự động gia hạn (quay vòng gốc lãi) tại đây
                auditLogService.ghiLog("HỆ THỐNG TỰ ĐỘNG", "Xử lý đáo hạn tự động cho sổ: " + stk.getId());

                // Ví dụ tạm thời: Ghi nhận hệ thống đã quét qua
                System.out.println("[SYSTEM PROCESS] Đang xử lý nghiệp vụ tự động cho sổ: " + stk.getId());
            }
        }
    }

    // FR4: Tra cứu danh sách sổ
    public List<SoTietKiem> layDanhSachSoTietKiem() {
        return soTietKiemRepository.findAll();
    }

    // FR1: Mở sổ tiết kiệm
    @Transactional
    public String moSoTietKiem(MoSoRequest request) {
        ThamSo thamSo = thamSoRepository.findById(1)
                .orElseThrow(() -> new RuntimeException("Chưa thiết lập tham số hệ thống!"));

        if (request.getSoTienGuiBanDau().compareTo(thamSo.getTienGuiBanDauToiThieu()) < 0) {
            throw new RuntimeException("Số tiền gửi ban đầu không đạt mức tối thiểu: "
                    + thamSo.getTienGuiBanDauToiThieu() + "đ");
        }

        LoaiTietKiem loaiTk = loaiTietKiemRepository.findById(request.getLoaiTietKiem())
                .orElseThrow(() -> new RuntimeException("Loại tiết kiệm không hợp lệ!"));

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

        SoTietKiem stk = new SoTietKiem();
        String maSo = "STK-" + UUID.randomUUID().toString().substring(0, 8).toUpperCase();

        stk.setId(maSo);
        stk.setKhachHang(khachHang);
        stk.setLoaiTietKiem(loaiTk);
        stk.setSoDu(request.getSoTienGuiBanDau());
        stk.setNgayMo(LocalDate.now());

        if (loaiTk.getKyHan() > 0) {
            stk.setNgayDaoHan(LocalDate.now().plusMonths(loaiTk.getKyHan()));
        }

        stk.setTrangThai("DANG_HOAT_DONG");
        soTietKiemRepository.save(stk);

        PhieuGuiTien phieu = new PhieuGuiTien();
        phieu.setSoTienGui(request.getSoTienGuiBanDau());
        phieu.setNgayGui(LocalDate.now());
        phieu.setSoTietKiem(stk);
        phieu.setKhachHang(khachHang);
        phieuGuiTienRepository.save(phieu);

        auditLogService.ghiLog("MỞ SỔ", "Mở sổ mới: " + maSo + " | Khách hàng: " + khachHang.getTen() + " | Gửi ban đầu: " + request.getSoTienGuiBanDau() + "đ");

        return maSo;
    }
}