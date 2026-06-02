package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.entity.AuditLog;
import com.uit.passbook_management_api.repository.AuditLogRepository;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;
import java.time.LocalDateTime;

@Service
public class AuditLogService {

    private final AuditLogRepository auditLogRepository;

    public AuditLogService(AuditLogRepository auditLogRepository) {
        this.auditLogRepository = auditLogRepository;
    }

    public void ghiLog(String hanhDong, String chiTiet) {
        AuditLog log = new AuditLog();
        log.setHanhDong(hanhDong);
        log.setChiTiet(chiTiet);
        log.setThoiGian(LocalDateTime.now());

        // Lấy ID/Username của người dùng đang gọi API từ SecurityContext
        String nguoiThucHien = "SYSTEM";
        if (SecurityContextHolder.getContext().getAuthentication() != null) {
            nguoiThucHien = SecurityContextHolder.getContext().getAuthentication().getName();
        }
        log.setNguoiThucHien(nguoiThucHien);

        auditLogRepository.save(log);
    }
}