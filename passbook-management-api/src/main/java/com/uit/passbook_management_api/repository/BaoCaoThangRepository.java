package com.uit.passbook_management_api.repository;
import com.uit.passbook_management_api.entity.BaoCaoThang;
import com.uit.passbook_management_api.entity.BaoCaoThangId;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface BaoCaoThangRepository extends JpaRepository<BaoCaoThang, BaoCaoThangId> {}