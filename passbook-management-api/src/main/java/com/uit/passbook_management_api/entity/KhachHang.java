package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;

@Data
@Entity
@Table(name = "khach_hang")
public class KhachHang {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id; // SQL là BIGINT

    private String ten;

    @Column(unique = true)
    private String cmnd;

    @Column(name = "dia_chi")
    private String diaChi;

    private String sdt;

    // TODO: Thêm Getters / Setters
}