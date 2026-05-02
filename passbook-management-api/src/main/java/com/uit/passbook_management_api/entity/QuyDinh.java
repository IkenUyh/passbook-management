package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;

@Entity
@Table(name = "quy_dinh")
public class QuyDinh {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "tien_gui_toi_thieu")
    private BigDecimal tienGuiToiThieu;

    @Column(name = "tien_gui_them_toi_thieu")
    private BigDecimal tienGuiThemToiThieu;

    @Column(name = "thoi_gian_gui_toi_thieu_ngay")
    private Integer thoiGianGuiToiThieuNgay;

    @Column(name = "lai_suat_kkh")
    private BigDecimal laiSuatKkh;

    @Column(name = "lai_suat_3t")
    private BigDecimal laiSuat3t;

    @Column(name = "lai_suat_6t")
    private BigDecimal laiSuat6t;

    // TODO: Thêm Getters / Setters
}