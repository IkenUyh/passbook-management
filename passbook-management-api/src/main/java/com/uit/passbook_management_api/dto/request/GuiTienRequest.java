package com.uit.passbook_management_api.dto.request;

import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDate;

@Data
public class GuiTienRequest {
    private String maSoTietKiem;
    private BigDecimal soTienGui;
    private LocalDate ngayGui; // Thường sẽ lấy ngày hiện tại từ client gửi lên
}