package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;

@Data
@Entity
@Table(name = "bao_cao_thang")
public class BaoCaoThang {
    @EmbeddedId
    private BaoCaoThangId id;

    @Column(name = "so_so_mo")
    private Integer soSoMo;

    @Column(name = "so_so_dong")
    private Integer soSoDong;

    @Column(name = "chenh_lech")
    private Integer chenhLech;
}