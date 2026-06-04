package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.NhanVien;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface NhanVienRepository extends JpaRepository<NhanVien, Long> {
    boolean existsByCccd(String cccd);
    @Query(value = "SELECT id FROM nhan_vien ORDER BY CAST(SUBSTRING(id, 3) AS UNSIGNED) DESC LIMIT 1", nativeQuery = true)
    String findLastId();
}