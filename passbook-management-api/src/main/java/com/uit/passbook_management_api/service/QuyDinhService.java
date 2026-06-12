package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.CapNhatThamSoRequest;
import com.uit.passbook_management_api.dto.request.LoaiTietKiemRequest;
import com.uit.passbook_management_api.entity.LoaiTietKiem;
import com.uit.passbook_management_api.entity.ThamSo;
import com.uit.passbook_management_api.repository.LoaiTietKiemRepository;
import com.uit.passbook_management_api.repository.ThamSoRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.util.List;

@Service
public class QuyDinhService {

    private final ThamSoRepository thamSoRepository;
    private final LoaiTietKiemRepository loaiTietKiemRepository;
    private final AuditLogService auditLogService;

    public QuyDinhService(ThamSoRepository thamSoRepository,
                          LoaiTietKiemRepository loaiTietKiemRepository,
                          AuditLogService auditLogService) {
        this.thamSoRepository = thamSoRepository;
        this.loaiTietKiemRepository = loaiTietKiemRepository;
        this.auditLogService = auditLogService;
    }

    // 1. Xem và Cập nhật Tham Số Chung
    public ThamSo xemThamSo() {
        return thamSoRepository.findById(1)
                .orElseThrow(() -> new RuntimeException("Chưa có tham số hệ thống!"));
    }

    @Transactional
    public ThamSo capNhatThamSo(CapNhatThamSoRequest request) {

        if (request.getTienGuiBanDauToiThieu() != null && request.getTienGuiBanDauToiThieu().compareTo(BigDecimal.ZERO) < 0) {
            throw new RuntimeException("Tiền gửi ban đầu tối thiểu không được nhỏ hơn 0!");
        }

        if (request.getTienGuiThemToiThieu() != null && request.getTienGuiThemToiThieu().compareTo(BigDecimal.ZERO) < 0) {
            throw new RuntimeException("Tiền gửi thêm tối thiểu không được nhỏ hơn 0!");
        }

        if (request.getSoNgayGuiToiThieu() != null && request.getSoNgayGuiToiThieu() < 0) {
            throw new RuntimeException("Số ngày gửi tối thiểu không được nhỏ hơn 0!");
        }

        ThamSo thamSo = xemThamSo();
        thamSo.setTienGuiBanDauToiThieu(request.getTienGuiBanDauToiThieu());
        thamSo.setTienGuiThemToiThieu(request.getTienGuiThemToiThieu());
        thamSo.setSoNgayGuiToiThieu(request.getSoNgayGuiToiThieu());

        ThamSo savedThamSo = thamSoRepository.save(thamSo);

        // --- GHI LOG HỆ THỐNG ---
        auditLogService.ghiLog("THAY ĐỔI QUY ĐỊNH", "Cập nhật tham số hệ thống: Tiền tối thiểu " + request.getTienGuiBanDauToiThieu());

        return savedThamSo;
    }

    // 2. Xem và Quản lý Loại Tiết Kiệm (QĐ1: Thêm/Sửa số lượng kỳ hạn)
    public List<LoaiTietKiem> xemDanhSachLoaiTietKiem() {
        return loaiTietKiemRepository.findAll();
    }

    @Transactional
    public LoaiTietKiem luuLoaiTietKiem(LoaiTietKiemRequest request) {

        if (request.getKyHan() != null && request.getKyHan() < 0) {
            throw new RuntimeException("Kỳ hạn (tháng) không được nhỏ hơn 0!");
        }

        if (request.getLaiSuat() != null && request.getLaiSuat().compareTo(BigDecimal.ZERO) < 0) {
            throw new RuntimeException("Lãi suất không được nhỏ hơn 0%!");
        }

        LoaiTietKiem loaiTk = loaiTietKiemRepository.findById(request.getMaLoaiTk())
                .orElse(new LoaiTietKiem()); // Nếu không tìm thấy thì tạo mới

        loaiTk.setMaLoaiTk(request.getMaLoaiTk());
        loaiTk.setTenLoaiTk(request.getTenLoaiTk());
        loaiTk.setKyHan(request.getKyHan());
        loaiTk.setLaiSuat(request.getLaiSuat());

        LoaiTietKiem savedLoaiTk = loaiTietKiemRepository.save(loaiTk);

        // --- GHI LOG HỆ THỐNG ---
        auditLogService.ghiLog("THAY ĐỔI LOẠI TIẾT KIỆM", "Lưu loại kỳ hạn: " + request.getMaLoaiTk() + " | Lãi suất: " + request.getLaiSuat() + "%");

        return savedLoaiTk;
    }
}