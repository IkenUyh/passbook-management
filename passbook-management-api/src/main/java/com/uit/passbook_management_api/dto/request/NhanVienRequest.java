package com.uit.passbook_management_api.dto.request;

import lombok.Data;

@Data
public class NhanVienRequest {
    // Thông tin cá nhân
    private String hoTen;
    private String soDienThoai;
    private String cccd;
    private String role;
}