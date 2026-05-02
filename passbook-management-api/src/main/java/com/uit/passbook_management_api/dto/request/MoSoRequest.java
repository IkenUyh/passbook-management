package com.uit.passbook_management_api.dto.request;

import lombok.Data;

import java.math.BigDecimal;

@Data
public class MoSoRequest {
    private String tenKhachHang;
    private String cmnd;
    private String diaChi;
    private String loaiTietKiem;
    private BigDecimal soTienGuiBanDau;
}