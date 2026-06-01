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

    // Truy vấn Báo Cáo Doanh Số Ngày
    @Query(value = """
        SELECT 
            ltk.ma_loai_tk AS maLoaiTk,
            ltk.ten_loai_tk AS loaiTietKiem,
            COALESCE(SUM(pg.so_tien_gui), 0) AS tongThu,
            COALESCE(SUM(pr.so_tien_rut), 0) AS tongChi,
            (COALESCE(SUM(pg.so_tien_gui), 0) - COALESCE(SUM(pr.so_tien_rut), 0)) AS chenhLech
        FROM loai_tiet_kiem ltk
        LEFT JOIN so_tiet_kiem stk ON ltk.ma_loai_tk = stk.ma_loai_tk
        LEFT JOIN phieu_gui_tien pg ON stk.id = pg.ma_so_tiet_kiem AND pg.ngay_gui = :ngay
        LEFT JOIN phieu_rut_tien pr ON stk.id = pr.ma_so_tiet_kiem AND pr.ngay_rut = :ngay
        GROUP BY ltk.ten_loai_tk
    """, nativeQuery = true)
    List<BaoCaoNgayDTO> lapBaoCaoNgay(@Param("ngay") LocalDate ngay);

    // Truy vấn Báo Cáo Đóng/Mở Sổ Tháng
    @Query(value = """
        SELECT 
            ltk.ma_loai_tk AS maLoaiTk,
            ltk.ten_loai_tk AS loaiTietKiem,
            COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_mo) = :thang AND YEAR(stk.ngay_mo) = :nam THEN stk.id END) AS soSoMo,
            COUNT(DISTINCT CASE WHEN stk.trang_thai = 'DA_DONG' AND MONTH(pr.ngay_rut) = :thang AND YEAR(pr.ngay_rut) = :nam THEN stk.id END) AS soSoDong,
            (COUNT(DISTINCT CASE WHEN MONTH(stk.ngay_mo) = :thang AND YEAR(stk.ngay_mo) = :nam THEN stk.id END) - 
             COUNT(DISTINCT CASE WHEN stk.trang_thai = 'DA_DONG' AND MONTH(pr.ngay_rut) = :thang AND YEAR(pr.ngay_rut) = :nam THEN stk.id END)) AS chenhLech
        FROM loai_tiet_kiem ltk
        LEFT JOIN so_tiet_kiem stk ON ltk.ma_loai_tk = stk.ma_loai_tk
        LEFT JOIN phieu_rut_tien pr ON stk.id = pr.ma_so_tiet_kiem
        GROUP BY ltk.ten_loai_tk
    """, nativeQuery = true)
    List<BaoCaoThangDTO> lapBaoCaoThang(@Param("thang") int thang, @Param("nam") int nam);
}