package com.uit.passbook_management_api.dto.response;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class NhanVienResponse {
    private String hoTen;
    private String soDienThoai;
    private String cccd;
    private String username;
    private String password; // Mật khẩu mặc định  (123456)
    private String role;
}