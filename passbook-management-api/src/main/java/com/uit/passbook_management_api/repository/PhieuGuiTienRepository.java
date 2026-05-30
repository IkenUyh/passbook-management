package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.PhieuGuiTien;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface PhieuGuiTienRepository extends JpaRepository<PhieuGuiTien, Long> {
}
