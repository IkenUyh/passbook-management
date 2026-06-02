package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.MoSoRequest;
import com.uit.passbook_management_api.entity.SoTietKiem;
import com.uit.passbook_management_api.service.SoTietKiemService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/v1/so-tiet-kiem")
public class SoTietKiemController {

    private final SoTietKiemService soTietKiemService;

    public SoTietKiemController(SoTietKiemService soTietKiemService) {
        this.soTietKiemService = soTietKiemService;
    }

    // --- ENDPOINT 1: Lấy danh sách đổ lên giao diện WPF ---
    @GetMapping
    public ResponseEntity<List<SoTietKiem>> layDanhSachSo() {
        // Trả về HTTP 200 OK kèm theo cục JSON chứa list sổ tiết kiệm
        return ResponseEntity.ok(soTietKiemService.layDanhSachSoTietKiem());
    }

    // --- ENDPOINT 2: Logic thêm sổ mới (Giữ nguyên code chuẩn của ông) ---
    @PostMapping("/mo-so")
    public ResponseEntity<?> moSoTietKiem(@RequestBody MoSoRequest request) {
        try {
            String maSo = soTietKiemService.moSoTietKiem(request);
            return ResponseEntity.ok("Mở sổ thành công! Mã sổ: " + maSo);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    // =========================================================
    // API dành cho FR8: Lấy danh sách sổ sắp đáo hạn để WPF hiện Popup
    // Test Postman: GET http://localhost:8083/api/v1/so-tiet-kiem/sap-dao-han?soNgayBaoTruoc=3
    // =========================================================
    @GetMapping("/sap-dao-han")
    public ResponseEntity<List<SoTietKiem>> layDanhSachSapDaoHan(
            @RequestParam(defaultValue = "3") int soNgayBaoTruoc) {
        return ResponseEntity.ok(soTietKiemService.layDanhSachSapDaoHan(soNgayBaoTruoc));
    }
}