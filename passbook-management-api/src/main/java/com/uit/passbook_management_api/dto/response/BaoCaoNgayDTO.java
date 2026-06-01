package com.uit.passbook_management_api.dto.response;

import java.math.BigDecimal;

public interface BaoCaoNgayDTO {
    String getMaLoaiTk();
    String getLoaiTietKiem();
    BigDecimal getTongThu();
    BigDecimal getTongChi();
    BigDecimal getChenhLech();
}