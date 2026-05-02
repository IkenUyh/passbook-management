package com.uit.passbook_management_api.repository;

import com.uit.passbook_management_api.entity.AppUser;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface AppUserRepository extends JpaRepository<AppUser, Long> {
    // Hàm này sẽ tự động query vào CSDL tìm user theo username
    Optional<AppUser> findByUsername(String username);
}