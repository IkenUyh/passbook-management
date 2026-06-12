package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDate;

@Data
@Entity
@Table(name = "phieu_gui_tien")
public class PhieuGuiTien {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ma_phieu_gui_tien")
    private Long maPhieuGuiTien;

    @Column(name = "so_tien_gui", nullable = false)
    private BigDecimal soTienGui;

    @Column(name = "ngay_gui", nullable = false)
    private LocalDate ngayGui;

    // Khoa ngoai tro den SoTietKiem
    @ManyToOne
    @JoinColumn(name = "ma_so_tiet_kiem", nullable = false)
    private SoTietKiem soTietKiem;

    // Khoa ngoai tro den KhachHang
    @ManyToOne
    @JoinColumn(name = "khach_hang_id", nullable = false)
    private KhachHang khachHang;
}