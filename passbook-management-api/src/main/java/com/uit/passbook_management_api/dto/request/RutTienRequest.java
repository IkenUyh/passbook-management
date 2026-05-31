package com.uit.passbook_management_api.dto.request;

import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDate;

@Data
public class RutTienRequest {
    private String maSoTietKiem;
    private BigDecimal soTienRut;
    private LocalDate ngayRut;
}