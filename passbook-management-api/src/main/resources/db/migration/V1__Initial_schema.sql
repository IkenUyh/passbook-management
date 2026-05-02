CREATE TABLE quy_dinh (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tien_gui_toi_thieu DECIMAL(15, 2) NOT NULL DEFAULT 1000000,
    tien_gui_them_toi_thieu DECIMAL(15, 2) NOT NULL DEFAULT 100000,
    thoi_gian_gui_toi_thieu_ngay INT NOT NULL DEFAULT 15,
    lai_suat_kkh DECIMAL(5, 2) NOT NULL DEFAULT 0.5,
    lai_suat_3t DECIMAL(5, 2) NOT NULL DEFAULT 5.0,
    lai_suat_6t DECIMAL(5, 2) NOT NULL DEFAULT 5.5
);

CREATE TABLE khach_hang (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    ten VARCHAR(255) NOT NULL,
    cmnd VARCHAR(20) UNIQUE NOT NULL,
    dia_chi VARCHAR(500),
    sdt VARCHAR(20)
);

CREATE TABLE so_tiet_kiem (
    id VARCHAR(50) PRIMARY KEY,
    khach_hang_id BIGINT NOT NULL,
    loai_tiet_kiem VARCHAR(50) NOT NULL,
    so_du DECIMAL(15, 2) NOT NULL,
    ngay_mo DATE NOT NULL,
    ngay_dao_han DATE,
    trang_thai VARCHAR(20) NOT NULL,
    FOREIGN KEY (khach_hang_id) REFERENCES khach_hang(id)
);

INSERT INTO quy_dinh (id) VALUES (1);