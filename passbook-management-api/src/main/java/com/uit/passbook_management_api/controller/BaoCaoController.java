package com.uit.passbook_management_api.controller;

import com.uit.passbook_management_api.dto.response.BaoCaoNgayDTO;
import com.uit.passbook_management_api.dto.response.BaoCaoThangDTO;
import com.uit.passbook_management_api.service.BaoCaoService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/v1/bao-cao")
@PreAuthorize("hasRole('ADMIN')")
public class BaoCaoController {

    private final BaoCaoService baoCaoService;

    public BaoCaoController(BaoCaoService baoCaoService) {
        this.baoCaoService = baoCaoService;
    }

    @GetMapping("/ngay")
    public ResponseEntity<List<BaoCaoNgayDTO>> xemBaoCaoNgay(@RequestParam("ngay") LocalDate ngay) {
        // Vừa tính toán trả về, vừa lưu âm thầm vào Database
        return ResponseEntity.ok(baoCaoService.chotBaoCaoNgay(ngay));
    }

    @GetMapping("/thang")
    public ResponseEntity<List<BaoCaoThangDTO>> xemBaoCaoThang(@RequestParam("thang") int thang, @RequestParam("nam") int nam) {
        return ResponseEntity.ok(baoCaoService.chotBaoCaoThang(thang, nam));
    }
}