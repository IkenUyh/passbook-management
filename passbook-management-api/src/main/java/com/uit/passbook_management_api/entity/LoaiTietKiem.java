package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;

@Data
@Entity
@Table(name = "loai_tiet_kiem")
public class LoaiTietKiem {
    @Id
    @Column(name = "ma_loai_tk", length = 50)
    private String maLoaiTk;

    @Column(name = "ten_loai_tk", nullable = false, length = 100)
    private String tenLoaiTk;

    @Column(name = "ky_han")
    private Integer kyHan;

    @Column(name = "lai_suat")
    private BigDecimal laiSuat;
}