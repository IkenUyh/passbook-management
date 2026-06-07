package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.LoaiTietKiem;
import com.uit.passbook_management_api.dto.response.BaoCaoNgayDTO;
import com.uit.passbook_management_api.dto.response.BaoCaoThangDTO;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.util.List;

@Repository
public interface LoaiTietKiemRepository extends JpaRepository<LoaiTietKiem, String> {

    // =========================================================================
    // 1. TRUY VẤN BÁO CÁO DOANH SỐ NGÀY (FR5)
    // =========================================================================
    @Query(value = """
        SELECT 
            ltk.ma_loai_tk AS maLoaiTk,
            ltk.ten_loai_tk AS loaiTietKiem,
            COALESCE(p_thu.tong_thu, 0) AS tongThu,
            COALESCE(p_chi.tong_chi, 0) AS tongChi,
            (COALESCE(p_thu.tong_thu, 0) - COALESCE(p_chi.tong_chi, 0)) AS chenhLech
        FROM loai_tiet_kiem ltk
        LEFT JOIN (
            SELECT stk.ma_loai_tk, SUM(pg.so_tien_gui) AS tong_thu 
            FROM phieu_gui_tien pg
            JOIN so_tiet_kiem stk ON pg.ma_so_tiet_kiem = stk.id
            WHERE pg.ngay_gui = :ngay 
            GROUP BY stk.ma_loai_tk
        ) p_thu ON ltk.ma_loai_tk = p_thu.ma_loai_tk
        LEFT JOIN (
            SELECT stk.ma_loai_tk, SUM(pr.so_tien_rut) AS tong_chi 
            FROM phieu_rut_tien pr
            JOIN so_tiet_kiem stk ON pr.ma_so_tiet_kiem = stk.id
            WHERE pr.ngay_rut = :ngay 
            GROUP BY stk.ma_loai_tk
        ) p_chi ON ltk.ma_loai_tk = p_chi.ma_loai_tk
        GROUP BY ltk.ma_loai_tk, ltk.ten_loai_tk
    """, nativeQuery = true)
    List<BaoCaoNgayDTO> lapBaoCaoNgay(@Param("ngay") LocalDate ngay);

    // =========================================================================
    // 2. TRUY VẤN BÁO CÁO ĐÓNG/MỞ SỔ THÁNG (FR5)
    // =========================================================================
    @Query(value = """
        SELECT 
            ltk.ma_loai_tk AS maLoaiTk,
            ltk.ten_loai_tk AS loaiTietKiem,
            COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_mo) = :thang AND YEAR(stk.ngay_mo) = :nam THEN stk.id END) AS soSoMo,
            COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_dong) = :thang AND YEAR(stk.ngay_dong) = :nam THEN stk.id END) AS soSoDong,
            (COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_mo) = :thang AND YEAR(stk.ngay_mo) = :nam THEN stk.id END) - 
             COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_dong) = :thang AND YEAR(stk.ngay_dong) = :nam THEN stk.id END)) AS chenhLech
        FROM loai_tiet_kiem ltk
        LEFT JOIN so_tiet_kiem stk ON ltk.ma_loai_tk = stk.ma_loai_tk
        GROUP BY ltk.ma_loai_tk, ltk.ten_loai_tk
    """, nativeQuery = true)
    List<BaoCaoThangDTO> lapBaoCaoThang(@Param("thang") int thang, @Param("nam") int nam);
}