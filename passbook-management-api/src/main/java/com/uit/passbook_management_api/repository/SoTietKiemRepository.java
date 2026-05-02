package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.SoTietKiem;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface SoTietKiemRepository extends JpaRepository<SoTietKiem, String> { // ID là String
}