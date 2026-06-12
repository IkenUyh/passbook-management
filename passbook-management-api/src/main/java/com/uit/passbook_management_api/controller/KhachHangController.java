package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.KhachHangRequest;
import com.uit.passbook_management_api.service.KhachHangService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/khach-hang")
@PreAuthorize("hasAnyRole('ADMIN', 'NHAN_VIEN')")
public class KhachHangController {

    private final KhachHangService khachHangService;

    public KhachHangController(KhachHangService khachHangService) {
        this.khachHangService = khachHangService;
    }

    @GetMapping
    public ResponseEntity<?> layDanhSachKhachHang() {
        return ResponseEntity.ok(khachHangService.layDanhSachKhachHang());
    }

    @PostMapping
    public ResponseEntity<?> themKhachHang(@RequestBody KhachHangRequest request) {
        try {
            return ResponseEntity.ok(khachHangService.themKhachHang(request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    @PutMapping("/{id}")
    public ResponseEntity<?> capNhatKhachHang(@PathVariable Long id, @RequestBody KhachHangRequest request) {
        try {
            return ResponseEntity.ok(khachHangService.capNhatKhachHang(id, request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<?> xoaKhachHang(@PathVariable Long id) {
        try {
            khachHangService.xoaKhachHang(id);
            return ResponseEntity.ok("Xóa khách hàng thành công!");
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}