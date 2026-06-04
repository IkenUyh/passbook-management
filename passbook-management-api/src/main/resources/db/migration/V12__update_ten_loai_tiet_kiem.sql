-- Cập nhật tên loại tiết kiệm thành tiếng Việt có dấu để hiển thị lên WPF
UPDATE loai_tiet_kiem SET ten_loai_tk = 'Không kỳ hạn' WHERE ma_loai_tk = 'KKH';
UPDATE loai_tiet_kiem SET ten_loai_tk = '3 tháng' WHERE ma_loai_tk = '3T';
UPDATE loai_tiet_kiem SET ten_loai_tk = '6 tháng' WHERE ma_loai_tk = '6T';

