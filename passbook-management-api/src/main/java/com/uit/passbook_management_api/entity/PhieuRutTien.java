package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDate;

@Data
@Entity
@Table(name = "phieu_rut_tien")
public class PhieuRutTien {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ma_phieu_rut")
    private Long maPhieuRut;

    @Column(name = "so_tien_rut", nullable = false)
    private BigDecimal soTienRut;

    @Column(name = "ngay_rut", nullable = false)
    private LocalDate ngayRut;

    @ManyToOne
    @JoinColumn(name = "ma_so_tiet_kiem", nullable = false)
    private SoTietKiem soTietKiem;

    @ManyToOne
    @JoinColumn(name = "khach_hang_id", nullable = false)
    private KhachHang khachHang;
}