package com.uit.passbook_management_api.dto.request;

import lombok.Data;

@Data
public class NhanVienRequest {
    // Thông tin cá nhân
    private String hoTen;
    private String soDienThoai;
    private String cccd;

    // Thông tin cấp tài khoản đăng nhập
    private String username;
    private String password;
    private String role;
}