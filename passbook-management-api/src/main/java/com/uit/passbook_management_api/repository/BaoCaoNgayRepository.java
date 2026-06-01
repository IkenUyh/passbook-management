package com.uit.passbook_management_api.repository;
import com.uit.passbook_management_api.entity.BaoCaoNgay;
import com.uit.passbook_management_api.entity.BaoCaoNgayId;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface BaoCaoNgayRepository extends JpaRepository<BaoCaoNgay, BaoCaoNgayId> {}