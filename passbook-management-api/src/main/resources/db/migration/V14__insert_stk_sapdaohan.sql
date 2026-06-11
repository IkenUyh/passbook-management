-- ====================================================================
-- MIGRATION: V14__Insert_test_data_sap_dao_han.sql
-- MỤC ĐÍCH: Tạo dữ liệu mẫu cho các sổ tiết kiệm sắp đến ngày đáo hạn
-- ====================================================================

-- 1. Tạo thêm khách hàng thử nghiệm để liên kết dữ liệu
INSERT INTO khach_hang (ten, cmnd, dia_chi, sdt) VALUES
('Nguyễn Văn Chọn', '079206001234', '92 Nguyễn Trãi, Quận 5, TP. HCM', '0901234567'),
('Trần Thị Đáo',   '079206005678', 'Khu phố 6, Linh Trung, Thủ Đức', '0912345678'),
('Lê Hoàng Hạn',   '079206009012', '111 Ngô Thì Nhậm, Dĩ An, Bình Dương', '0988888888');

-- 2. Thêm các sổ tiết kiệm mẫu sắp đáo hạn
-- Sử dụng SUBQUERY dựa trên CMND để lấy chính xác khach_hang_id vừa sinh tự động

-- Sổ 1: Loại 3 tháng (3T) - Sẽ đáo hạn sau đúng 2 ngày nữa
INSERT INTO so_tiet_kiem (id, khach_hang_id, ma_loai_tk, so_du, ngay_mo, ngay_dao_han, trang_thai, version, ngay_dong)
SELECT
    'STK-TEST-001',
    id,
    '3T',
    50000000.00,  -- Số dư: 50 triệu
    DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 2 DAY), INTERVAL 3 MONTH), -- Ngày mở = (Hôm nay + 2 ngày) - 3 tháng
    DATE_ADD(CURDATE(), INTERVAL 2 DAY),                            -- Ngày đáo hạn = Hôm nay + 2 ngày
    'HOAT_DONG',
    0,
    NULL
FROM khach_hang WHERE cmnd = '079206001234';

-- Sổ 2: Loại 6 tháng (6T) - Sẽ đáo hạn sau đúng 5 ngày nữa
INSERT INTO so_tiet_kiem (id, khach_hang_id, ma_loai_tk, so_du, ngay_mo, ngay_dao_han, trang_thai, version, ngay_dong)
SELECT
    'STK-TEST-002',
    id,
    '6T',
    150000000.00, -- Số dư: 150 triệu
    DATE_SUB(DATE_ADD(CURDATE(), INTERVAL 5 DAY), INTERVAL 6 MONTH), -- Ngày mở = (Hôm nay + 5 ngày) - 6 tháng
    DATE_ADD(CURDATE(), INTERVAL 5 DAY),                            -- Ngày đáo hạn = Hôm nay + 5 ngày
    'HOAT_DONG',
    0,
    NULL
FROM khach_hang WHERE cmnd = '079206005678';

-- Sổ 3: Loại 3 tháng (3T) - Đáo hạn ĐÚNG NGÀY HÔM NAY (Rất tốt để test logic tất toán/tự động gia hạn tự động)
INSERT INTO so_tiet_kiem (id, khach_hang_id, ma_loai_tk, so_du, ngay_mo, ngay_dao_han, trang_thai, version, ngay_dong)
SELECT
    'STK-TEST-003',
    id,
    '3T',
    20000000.00,  -- Số dư: 20 triệu
    DATE_SUB(CURDATE(), INTERVAL 3 MONTH), -- Ngày mở = Hôm nay - 3 tháng
    CURDATE(),                             -- Ngày đáo hạn = Đúng ngày hôm nay
    'HOAT_DONG',
    0,
    NULL
FROM khach_hang WHERE cmnd = '079206009012';

-- 3. [Tùy chọn] Đồng bộ thêm phiếu gửi tiền ban đầu cho đúng quy trình nghiệp vụ (FR2)
-- Giúp hệ thống không bị lỗi logic nếu ràng buộc số dư phải khớp với tổng phiếu gửi
INSERT INTO phieu_gui_tien (so_tien_gui, ngay_gui, ma_so_tiet_kiem, khach_hang_id)
SELECT 50000000.00, ngay_mo, id, khach_hang_id FROM so_tiet_kiem WHERE id = 'STK-TEST-001';

INSERT INTO phieu_gui_tien (so_tien_gui, ngay_gui, ma_so_tiet_kiem, khach_hang_id)
SELECT 150000000.00, ngay_mo, id, khach_hang_id FROM so_tiet_kiem WHERE id = 'STK-TEST-002';

INSERT INTO phieu_gui_tien (so_tien_gui, ngay_gui, ma_so_tiet_kiem, khach_hang_id)
SELECT 20000000.00, ngay_mo, id, khach_hang_id FROM so_tiet_kiem WHERE id = 'STK-TEST-003';