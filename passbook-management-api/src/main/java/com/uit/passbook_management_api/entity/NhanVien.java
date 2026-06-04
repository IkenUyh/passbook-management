package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;

@Data
@Entity
@Table(name = "nhan_vien")
public class NhanVien {
    @Id // CHỈ GIỮ LẠI @Id
    @Column(name = "id", length = 20)
    private String id;

    @Column(name = "ho_ten", nullable = false)
    private String hoTen;

    @Column(name = "so_dien_thoai")
    private String soDienThoai;

    @Column(name = "cccd", unique = true)
    private String cccd;

    @OneToOne
    @JoinColumn(name = "app_user_id", referencedColumnName = "id")
    private AppUser appUser;
}