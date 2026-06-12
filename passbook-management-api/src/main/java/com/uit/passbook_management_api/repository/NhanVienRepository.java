package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.NhanVien;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.Optional;

@Repository
public interface NhanVienRepository extends JpaRepository<NhanVien, String> {

    boolean existsByCccd(String cccd);

    //Kiểm tra xem số CCCD mới nhập có bị trùng với nhân viên KHÁC không
    boolean existsByCccdAndIdNot(String cccd, String id);

    @Query(value = "SELECT id FROM nhan_vien ORDER BY CAST(SUBSTRING(id, 3) AS UNSIGNED) DESC LIMIT 1", nativeQuery = true)
    String findLastId();

    Optional<NhanVien> findByAppUserUsername(String username);
}