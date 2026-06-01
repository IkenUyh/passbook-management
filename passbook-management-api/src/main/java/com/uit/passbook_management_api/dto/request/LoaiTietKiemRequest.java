package com.uit.passbook_management_api.dto.request;
import lombok.Data;
import java.math.BigDecimal;

@Data
public class LoaiTietKiemRequest {
    private String maLoaiTk;
    private String tenLoaiTk;
    private Integer kyHan;
    private BigDecimal laiSuat;
}