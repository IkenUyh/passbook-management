-- 1. Tao bang loai_tiet_kiem thay the cho logic gop chung cu
CREATE TABLE loai_tiet_kiem (
    ma_loai_tk VARCHAR(50) PRIMARY KEY,
    ten_loai_tk VARCHAR(100) NOT NULL,
    ky_han INT NOT NULL, -- Don vi tinh: thang
    lai_suat DECIMAL(5, 2) NOT NULL
);

-- 2. Tao bang tham_so moi de quan ly tinh tien hoa linh hoat
CREATE TABLE tham_so (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tien_gui_ban_dau_toi_thieu DECIMAL(15, 2) NOT NULL DEFAULT 1000000,
    tien_gui_them_toi_thieu DECIMAL(15, 2) NOT NULL DEFAULT 100000,
    so_ngay_gui_toi_thieu INT NOT NULL DEFAULT 15
);

-- 3. Chen du lieu mac dinh ban dau cho he thong
INSERT INTO tham_so (id, tien_gui_ban_dau_toi_thieu, tien_gui_them_toi_thieu, so_ngay_gui_toi_thieu)
VALUES (1, 1000000, 100000, 15);

INSERT INTO loai_tiet_kiem (ma_loai_tk, ten_loai_tk, ky_han, lai_suat) VALUES
('KKH', 'Khong ky han', 0, 0.5),
('3T', '3 thang', 3, 5.0),
('6T', '6 thang', 6, 5.5);

-- 4. Thay doi cau truc bang so_tiet_kiem de lien ket voi bang loai_tiet_kiem
ALTER TABLE so_tiet_kiem DROP COLUMN loai_tiet_kiem;
ALTER TABLE so_tiet_kiem ADD COLUMN ma_loai_tk VARCHAR(50);
ALTER TABLE so_tiet_kiem ADD CONSTRAINT fk_stk_loai_tk FOREIGN KEY (ma_loai_tk) REFERENCES loai_tiet_kiem(ma_loai_tk);

-- 5. Tao bang phieu_gui_tien phuc vu cho FR2
CREATE TABLE phieu_gui_tien (
    ma_phieu_gui_tien BIGINT AUTO_INCREMENT PRIMARY KEY,
    so_tien_gui DECIMAL(15, 2) NOT NULL,
    ngay_gui DATE NOT NULL,
    ma_so_tiet_kiem VARCHAR(50) NOT NULL,
    khach_hang_id BIGINT NOT NULL,
    FOREIGN KEY (ma_so_tiet_kiem) REFERENCES so_tiet_kiem(id),
    FOREIGN KEY (khach_hang_id) REFERENCES khach_hang(id)
);

-- 6. Tao bang phieu_rut_tien phuc vu cho FR3
CREATE TABLE phieu_rut_tien (
    ma_phieu_rut BIGINT AUTO_INCREMENT PRIMARY KEY,
    so_tien_rut DECIMAL(15, 2) NOT NULL,
    ngay_rut DATE NOT NULL,
    ma_so_tiet_kiem VARCHAR(50) NOT NULL,
    khach_hang_id BIGINT NOT NULL,
    FOREIGN KEY (ma_so_tiet_kiem) REFERENCES so_tiet_kiem(id),
    FOREIGN KEY (khach_hang_id) REFERENCES khach_hang(id)
);

-- 7. Tao bang bao_cao_ngay phuc vu cho FR5
CREATE TABLE bao_cao_ngay (
    ngay DATE NOT NULL,
    ma_loai_tk VARCHAR(50) NOT NULL,
    tong_thu DECIMAL(15, 2) NOT NULL DEFAULT 0,
    tong_chi DECIMAL(15, 2) NOT NULL DEFAULT 0,
    chenh_lech DECIMAL(15, 2) NOT NULL DEFAULT 0,
    PRIMARY KEY (ngay, ma_loai_tk),
    FOREIGN KEY (ma_loai_tk) REFERENCES loai_tiet_kiem(ma_loai_tk)
);

-- 8. Tao bang bao_cao_thang phuc vu cho FR5
CREATE TABLE bao_cao_thang (
    thang_nam VARCHAR(7) NOT NULL, -- Dinh dang chuoi: YYYY-MM
    ma_loai_tk VARCHAR(50) NOT NULL,
    so_so_mo INT NOT NULL DEFAULT 0,
    so_so_dong INT NOT NULL DEFAULT 0,
    chenh_lech INT NOT NULL DEFAULT 0,
    PRIMARY KEY (thang_nam, ma_loai_tk),
    FOREIGN KEY (ma_loai_tk) REFERENCES loai_tiet_kiem(ma_loai_tk)
);