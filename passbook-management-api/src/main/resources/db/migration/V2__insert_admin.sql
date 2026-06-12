-- Chú ý: Mật khẩu thực tế phải được băm (hash) bằng BCrypt.
-- Dưới đây là hash của chuỗi "admin123"
CREATE TABLE app_user (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE,
    password VARCHAR(255),
    role VARCHAR(20)
);

INSERT INTO app_user (username, password, role)
VALUES ('admin', '$2a$12$Kaz1LnRx0baAUD7.1XupeOIT8M9sv24ql8HwKzWTcoOPV9b/mA1x6', 'ADMIN');