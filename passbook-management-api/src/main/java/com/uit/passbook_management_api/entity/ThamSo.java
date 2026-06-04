
package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;

@Data
@Entity
@Table(name = "tham_so")
public class    ThamSo {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "tien_gui_ban_dau_toi_thieu")
    private BigDecimal tienGuiBanDauToiThieu;

    @Column(name = "tien_gui_them_toi_thieu")
    private BigDecimal tienGuiThemToiThieu;

    @Column(name = "so_ngay_gui_toi_thieu")
    private Integer soNgayGuiToiThieu;
}