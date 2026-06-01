package com.uit.passbook_management_api.dto.request;
import lombok.Data;
import java.math.BigDecimal;

@Data
public class CapNhatThamSoRequest {
    private BigDecimal tienGuiBanDauToiThieu;
    private BigDecimal tienGuiThemToiThieu;
    private Integer soNgayGuiToiThieu;
}