package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.MoSoRequest;
import com.uit.passbook_management_api.service.SoTietKiemService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/so-tiet-kiem")
public class SoTietKiemController {

    private final SoTietKiemService soTietKiemService;

    public SoTietKiemController(SoTietKiemService soTietKiemService) {
        this.soTietKiemService = soTietKiemService;
    }

    @PostMapping("/mo-so")
    public ResponseEntity<?> moSoTietKiem(@RequestBody MoSoRequest request) {
        try {
            String maSo = soTietKiemService.moSoTietKiem(request);
            return ResponseEntity.ok("Mở sổ thành công! Mã sổ: " + maSo);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}