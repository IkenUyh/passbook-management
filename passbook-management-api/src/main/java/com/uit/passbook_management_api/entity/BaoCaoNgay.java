package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;

@Data
@Entity
@Table(name = "bao_cao_ngay")
public class BaoCaoNgay {
    @EmbeddedId
    private BaoCaoNgayId id;

    @Column(name = "tong_thu")
    private BigDecimal tongThu;

    @Column(name = "tong_chi")
    private BigDecimal tongChi;

    @Column(name = "chenh_lech")
    private BigDecimal chenhLech;
}