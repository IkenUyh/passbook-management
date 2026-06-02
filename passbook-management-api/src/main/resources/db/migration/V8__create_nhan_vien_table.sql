CREATE TABLE nhan_vien (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    ho_ten VARCHAR(255) NOT NULL,
    so_dien_thoai VARCHAR(20),
    cccd VARCHAR(20) UNIQUE,
    app_user_id BIGINT UNIQUE, -- Vẫn giữ để liên kết với tài khoản đăng nhập
    FOREIGN KEY (app_user_id) REFERENCES app_user(id) ON DELETE SET NULL
);