CREATE TABLE audit_log (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hanh_dong VARCHAR(100) NOT NULL,
    chi_tiet TEXT,
    nguoi_thuc_hien VARCHAR(50) NOT NULL,
    thoi_gian DATETIME NOT NULL
);