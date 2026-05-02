package com.uit.passbook_management_api.repository;
import com.uit.passbook_management_api.entity.QuyDinh;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface QuyDinhRepository extends JpaRepository<QuyDinh, Integer> {
}