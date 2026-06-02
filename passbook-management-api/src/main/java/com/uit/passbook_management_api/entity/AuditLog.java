package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "audit_log")
public class AuditLog {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "hanh_dong", nullable = false, length = 100)
    private String hanhDong;

    @Column(name = "chi_tiet", columnDefinition = "TEXT")
    private String chiTiet;

    @Column(name = "nguoi_thuc_hien", nullable = false, length = 50)
    private String nguoiThucHien;

    @Column(name = "thoi_gian", nullable = false)
    private LocalDateTime thoiGian;
}