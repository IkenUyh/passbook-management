package com.uit.passbook_management_api.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.Data;
import java.io.Serializable;
import java.time.LocalDate;

@Data
@Embeddable
public class BaoCaoNgayId implements Serializable {
    private LocalDate ngay;
    @Column(name = "ma_loai_tk")
    private String maLoaiTk;
}