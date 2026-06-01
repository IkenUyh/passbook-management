package com.uit.passbook_management_api.dto.request;

import lombok.Data;

@Data
public class KhachHangRequest {
    private String ten;
    private String cmnd;
    private String diaChi;
    private String sdt;
}