package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.CapNhatThamSoRequest;
import com.uit.passbook_management_api.dto.request.LoaiTietKiemRequest;
import com.uit.passbook_management_api.entity.LoaiTietKiem;
import com.uit.passbook_management_api.entity.ThamSo;
import com.uit.passbook_management_api.repository.LoaiTietKiemRepository;
import com.uit.passbook_management_api.repository.ThamSoRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class QuyDinhService {

    private final ThamSoRepository thamSoRepository;
    private final LoaiTietKiemRepository loaiTietKiemRepository;

    public QuyDinhService(ThamSoRepository thamSoRepository, LoaiTietKiemRepository loaiTietKiemRepository) {
        this.thamSoRepository = thamSoRepository;
        this.loaiTietKiemRepository = loaiTietKiemRepository;
    }

    // 1. Xem và Cập nhật Tham Số Chung
    public ThamSo xemThamSo() {
        return thamSoRepository.findById(1)
                .orElseThrow(() -> new RuntimeException("Chưa có tham số hệ thống!"));
    }

    @Transactional
    public ThamSo capNhatThamSo(CapNhatThamSoRequest request) {
        ThamSo thamSo = xemThamSo();
        thamSo.setTienGuiBanDauToiThieu(request.getTienGuiBanDauToiThieu());
        thamSo.setTienGuiThemToiThieu(request.getTienGuiThemToiThieu());
        thamSo.setSoNgayGuiToiThieu(request.getSoNgayGuiToiThieu());
        return thamSoRepository.save(thamSo);
    }

    // 2. Xem và Quản lý Loại Tiết Kiệm (QĐ1: Thêm/Sửa số lượng kỳ hạn)
    public List<LoaiTietKiem> xemDanhSachLoaiTietKiem() {
        return loaiTietKiemRepository.findAll();
    }

    @Transactional
    public LoaiTietKiem luuLoaiTietKiem(LoaiTietKiemRequest request) {
        LoaiTietKiem loaiTk = loaiTietKiemRepository.findById(request.getMaLoaiTk())
                .orElse(new LoaiTietKiem()); // Nếu không tìm thấy thì tạo mới

        loaiTk.setMaLoaiTk(request.getMaLoaiTk());
        loaiTk.setTenLoaiTk(request.getTenLoaiTk());
        loaiTk.setKyHan(request.getKyHan());
        loaiTk.setLaiSuat(request.getLaiSuat());

        return loaiTietKiemRepository.save(loaiTk);
    }
}