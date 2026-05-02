package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;

import java.math.BigDecimal;
import java.time.LocalDate;
@Data
@Entity
@Table(name = "so_tiet_kiem")
public class SoTietKiem {
    @Id
    // Bỏ @GeneratedValue đi vì đây là chuỗi VARCHAR(50) bạn tự quản lý
    private String id;

    // Mapping khóa ngoại sang bảng khach_hang
    @ManyToOne
    @JoinColumn(name = "khach_hang_id", nullable = false)
    private KhachHang khachHang;

    @Column(name = "loai_tiet_kiem")
    private String loaiTietKiem;

    @Column(name = "so_du")
    private BigDecimal soDu;

    @Column(name = "ngay_mo")
    private LocalDate ngayMo;

    @Column(name = "ngay_dao_han")
    private LocalDate ngayDaoHan;

    @Column(name = "trang_thai")
    private String trangThai;

    // TODO: Thêm Getters / Setters
}