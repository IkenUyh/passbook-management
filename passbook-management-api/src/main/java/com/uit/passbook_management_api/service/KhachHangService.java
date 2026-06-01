package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.KhachHangRequest;
import com.uit.passbook_management_api.entity.KhachHang;
import com.uit.passbook_management_api.repository.KhachHangRepository;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class KhachHangService {

    private final KhachHangRepository khachHangRepository;

    public KhachHangService(KhachHangRepository khachHangRepository) {
        this.khachHangRepository = khachHangRepository;
    }

    // 1. Lấy danh sách toàn bộ khách hàng
    public List<KhachHang> layDanhSachKhachHang() {
        return khachHangRepository.findAll();
    }

    // 2. Cập nhật thông tin khách hàng
    @Transactional
    public KhachHang capNhatKhachHang(Long id, KhachHangRequest request) {
        KhachHang kh = khachHangRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Không tìm thấy khách hàng với ID: " + id));

        kh.setTen(request.getTen());
        kh.setCmnd(request.getCmnd());
        kh.setDiaChi(request.getDiaChi());
        kh.setSdt(request.getSdt());

        return khachHangRepository.save(kh);
    }

    // 3. Xóa khách hàng
    @Transactional
    public void xoaKhachHang(Long id) {
        KhachHang kh = khachHangRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Không tìm thấy khách hàng!"));
        try {
            khachHangRepository.delete(kh);
        } catch (DataIntegrityViolationException e) {
            // Lỗi này văng ra khi CSDL báo đang bị kẹt khóa ngoại (khách hàng đang có sổ tiết kiệm)
            throw new RuntimeException("Không thể xóa! Khách hàng này đang có sổ tiết kiệm trong hệ thống.");
        }
    }
}