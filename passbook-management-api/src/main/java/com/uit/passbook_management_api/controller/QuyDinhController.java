package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.CapNhatThamSoRequest;
import com.uit.passbook_management_api.dto.request.LoaiTietKiemRequest;
import com.uit.passbook_management_api.service.QuyDinhService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/quy-dinh")
public class QuyDinhController {

    private final QuyDinhService quyDinhService;

    public QuyDinhController(QuyDinhService quyDinhService) {
        this.quyDinhService = quyDinhService;
    }

    // --- API Tham số chung ---
    @GetMapping("/tham-so")
    public ResponseEntity<?> layThamSo() {
        return ResponseEntity.ok(quyDinhService.xemThamSo());
    }

    @PutMapping("/tham-so")
    public ResponseEntity<?> capNhatThamSo(@RequestBody CapNhatThamSoRequest request) {
        try {
            return ResponseEntity.ok(quyDinhService.capNhatThamSo(request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    // --- API Loại Tiết Kiệm ---
    @GetMapping("/loai-tiet-kiem")
    public ResponseEntity<?> layDanhSachLoaiTietKiem() {
        return ResponseEntity.ok(quyDinhService.xemDanhSachLoaiTietKiem());
    }

    @PostMapping("/loai-tiet-kiem")
    public ResponseEntity<?> themHoacSuaLoaiTietKiem(@RequestBody LoaiTietKiemRequest request) {
        try {
            return ResponseEntity.ok(quyDinhService.luuLoaiTietKiem(request));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}