package com.uit.passbook_management_api.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.Data;
import java.io.Serializable;

@Data
@Embeddable
public class BaoCaoThangId implements Serializable {
    @Column(name = "thang_nam")
    private String thangNam;

    @Column(name = "ma_loai_tk")
    private String maLoaiTk;
}