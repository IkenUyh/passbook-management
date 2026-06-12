package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.SoTietKiem;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.util.List;

@Repository
public interface SoTietKiemRepository extends JpaRepository<SoTietKiem, String> {

    @Query("SELECT s FROM SoTietKiem s WHERE s.trangThai = 'DANG_HOAT_DONG' " +
            "AND s.loaiTietKiem.kyHan > 0 " +
            "AND s.ngayDaoHan BETWEEN :homNay AND :ngayMucTieu")
    List<SoTietKiem> timSoSapDaoHan(@Param("homNay") LocalDate homNay, @Param("ngayMucTieu") LocalDate ngayMucTieu);

    List<SoTietKiem> findByNgayDaoHanAndTrangThai(LocalDate ngayDaoHan, String trangThai);
}