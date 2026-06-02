INSERT INTO app_user (username, password, role) VALUES
('tranthib', '$2a$10$E2upv7arXmp3q0gHxGgYduQUkSS94gUzgK9S0FRZFiS84eaVJmY6.', 'NHAN_VIEN'),
('lequangc', '$2a$10$E2upv7arXmp3q0gHxGgYduQUkSS94gUzgK9S0FRZFiS84eaVJmY6.', 'NHAN_VIEN'),
('phamminhd', '$2a$10$E2upv7arXmp3q0gHxGgYduQUkSS94gUzgK9S0FRZFiS84eaVJmY6.', 'NHAN_VIEN'),
('hoangthie', '$2a$10$E2upv7arXmp3q0gHxGgYduQUkSS94gUzgK9S0FRZFiS84eaVJmY6.', 'NHAN_VIEN');


INSERT INTO nhan_vien (ho_ten, so_dien_thoai, cccd, app_user_id) VALUES
('Trần Thị B', '0912345678', '012345678902', (SELECT id FROM app_user WHERE username = 'tranthib')),
('Lê Quang C', '0923456789', '012345678903', (SELECT id FROM app_user WHERE username = 'lequangc')),
('Phạm Minh D', '0934567890', '012345678904', (SELECT id FROM app_user WHERE username = 'phamminhd')),
('Hoàng Thị E', '0945678901', '012345678905', (SELECT id FROM app_user WHERE username = 'hoangthie'));