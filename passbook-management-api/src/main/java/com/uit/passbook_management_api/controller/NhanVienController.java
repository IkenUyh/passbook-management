package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.NhanVienRequest;
import com.uit.passbook_management_api.entity.NhanVien;
import com.uit.passbook_management_api.service.NhanVienService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/nhan-vien")
@PreAuthorize("hasRole('ADMIN')")
public class NhanVienController {

    private final NhanVienService nhanVienService;

    public NhanVienController(NhanVienService nhanVienService) {
        this.nhanVienService = nhanVienService;
    }

    @GetMapping
    public ResponseEntity<?> xemDanhSachNhanVien() {
        return ResponseEntity.ok(nhanVienService.layDanhSachNhanVien());
    }

    @PostMapping
    public ResponseEntity<?> addNhanVien(@RequestBody NhanVienRequest request) {
        try {
            // Service bây giờ sẽ trả về NhanVienResponse chứa username mới sinh và password "123456"
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
}