package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.DoiMatKhauRequest;
import com.uit.passbook_management_api.dto.request.NhanVienRequest;
import com.uit.passbook_management_api.entity.NhanVien;
import com.uit.passbook_management_api.service.NhanVienService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/nhan-vien")
@PreAuthorize("hasAnyRole('ADMIN', 'NHAN_VIEN')")
public class NhanVienController {

    private final NhanVienService nhanVienService;

    public NhanVienController(NhanVienService nhanVienService) {
        this.nhanVienService = nhanVienService;
    }

    @GetMapping
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> xemDanhSachNhanVien() {
        return ResponseEntity.ok(nhanVienService.layDanhSachNhanVien());
    }

    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> addNhanVien(@RequestBody NhanVienRequest request) {
        try {
            return ResponseEntity.ok(nhanVienService.themNhanVienMoi(request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> capNhatNhanVien(@PathVariable String id, @RequestBody com.uit.passbook_management_api.dto.request.CapNhatNhanVienRequest request) {
        try {
            com.uit.passbook_management_api.entity.NhanVien nv = nhanVienService.capNhatThongTin(id, request);
            return ResponseEntity.ok("Cập nhật thông tin nhân viên " + id + " thành công!");

        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    @PutMapping("/doi-mat-khau")
    @PreAuthorize("isAuthenticated()")
    public ResponseEntity<?> doiMatKhauTaiKhoan(@RequestBody DoiMatKhauRequest request) {
        try {
            return ResponseEntity.ok(nhanVienService.doiMatKhau(request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}