package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.request.GuiTienRequest;
import com.uit.passbook_management_api.dto.request.RutTienRequest;
import com.uit.passbook_management_api.service.GiaoDichService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/giao-dich")
@PreAuthorize("hasRole('NHAN_VIEN')")
public class GiaoDichController {

    private final GiaoDichService giaoDichService;

    public GiaoDichController(GiaoDichService giaoDichService) {
        this.giaoDichService = giaoDichService;
    }

    @PostMapping("/gui-tien")
    public ResponseEntity<?> guiTien(@RequestBody GuiTienRequest request) {
        try {
            String message = giaoDichService.lapPhieuGuiTien(request);
            return ResponseEntity.ok(message);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }

    @PostMapping("/rut-tien")
    public ResponseEntity<?> rutTien(@RequestBody RutTienRequest request) {
        try {
            String message = giaoDichService.lapPhieuRutTien(request);
            return ResponseEntity.ok(message);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(e.getMessage());
        }
    }
}

