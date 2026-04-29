package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.MoSoRequest;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;

@Service
public class SoTietKiemService {

    public String moSoTietKiem(MoSoRequest request) {
        // Query bảng quy_dinh để lấy số tiền tối thiểu
        BigDecimal tienToiThieu = new BigDecimal("1000000");
        if (request.getSoTienGuiBanDau().compareTo(tienToiThieu) < 0) {
            throw new RuntimeException("Số tiền gửi ban đầu không đạt mức tối thiểu: 1.000.000đ");
        }

        return "STK" + System.currentTimeMillis();
    }
}