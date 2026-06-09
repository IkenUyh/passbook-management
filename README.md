<p align="center">
  <img src="docs/img/uit.png" alt="UIT Logo" width="600">
</p>

<h1 align="center">HỆ THỐNG QUẢN LÝ SỔ TIẾT KIỆM</h1>

<p align="center">
  <strong>Đồ án môn học: Nhập môn Công nghệ phần mềm</strong>
</p>

### 👥 Thành viên nhóm

| STT | MSSV     | Họ và tên          | GitHub                                                  |
|-----|----------|--------------------|----------------------------------------------------------|
| 1   | 24520697 | Tô Kiến Huy        | [@IkenUyh](https://github.com/IkenUyh)                  |
| 2   | 24520269 | Huỳnh Cao Đạt      | [@estellra](https://github.com/estellra)                 |
| 3   | 24520842 | Trần Lê Anh Khoa   | [@harutopia0](https://github.com/harutopia0)             |
| 4   | 24520693 | Phan Bùi Quang Huy | [@pbqhuy](https://github.com/pbqhuy)                    |

---

## 📖 Giới thiệu

Dự án **Quản lý Sổ tiết kiệm** là một hệ thống phần mềm hỗ trợ các ngân hàng, tổ chức tín dụng trong việc quản lý quy trình mở sổ, gửi tiền, rút tiền, tra cứu sổ tiết kiệm và báo cáo thống kê doanh số hoạt động.

Hệ thống được thiết kế theo mô hình Client-Server, với giao diện người dùng (Frontend) được phát triển trên nền tảng **C# (WPF)** và máy chủ xử lý nghiệp vụ (Backend API) được xây dựng bằng **Java Spring Boot**.

---

## 🏗 Kiến Trúc Hệ Thống

Dự án bao gồm 2 thành phần chính:

1. **`passbook-management-api` (Backend)**:
   - Framework: Java Spring Boot.
   - Database: MySQL 8.0 (Cấu hình qua JPA/Hibernate, Flyway Migration).
   - Bảo mật: JWT (JSON Web Token) + BCrypt Password Hashing.
   - Triển khai: Docker & Docker Compose kèm Caddy làm Reverse Proxy.
   - Cung cấp các RESTful APIs xử lý toàn bộ logic nghiệp vụ (quản lý người dùng, sổ tiết kiệm, giao dịch, báo cáo, quy định).
2. **`frontend-csharp` (Frontend)**:
   - Framework: C# .NET (WPF).
   - Thư viện biểu đồ: LiveChartsCore (SkiaSharp).
   - Giao tiếp với API thông qua giao thức HTTP (REST).
   - Xử lý giao diện tương tác với nhân viên ngân hàng (Teller/Admin).

---

## 🔑 Tài Khoản Mặc Định

Hệ thống được khởi tạo sẵn các tài khoản sau (tạo bởi Flyway Migration):

### Tài khoản Quản trị viên (Admin)

| Username | Mật khẩu  | Vai trò |
|----------|-----------|---------|
| `admin`  | `admin123` | ADMIN   |

### Tài khoản Nhân viên (Teller)

| Username     | Mật khẩu | Vai trò   | Họ tên       | SĐT          | CCCD           |
|--------------|----------|-----------|--------------|---------------|----------------|
| `tranthib`   | `123456` | NHAN_VIEN | Trần Thị B   | 0912345678    | 012345678902   |
| `lequangc`   | `123456` | NHAN_VIEN | Lê Quang C   | 0923456789    | 012345678903   |
| `phamminhd`  | `123456` | NHAN_VIEN | Phạm Minh D  | 0934567890    | 012345678904   |
| `hoangthie`  | `123456` | NHAN_VIEN | Hoàng Thị E  | 0945678901    | 012345678905   |

> **Lưu ý:** Khi Admin tạo nhân viên mới từ giao diện, hệ thống sẽ tự động sinh Username & Password (mật khẩu mặc định: `123456`). Vai trò mặc định là `NHAN_VIEN`.

### Phân quyền

| Vai trò     | Quyền hạn                                                                                  |
|-------------|--------------------------------------------------------------------------------------------|
| **ADMIN**   | Toàn quyền, trừ thay đổi thông tin đăng nhập. |
| **NHAN_VIEN** | Quản lý khách hàng, mở sổ tiết kiệm, gửi/rút tiền, tra cứu sổ, xem báo cáo.             |

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Ứng Dụng

### 1. Khởi chạy Backend API

Dự án backend đã được cấu hình sẵn với Docker Compose để dễ dàng triển khai.

**Yêu cầu:** Đã cài đặt Docker và Docker Compose.

```bash
# Di chuyển vào thư mục backend
cd passbook-management-api

# Khởi chạy các services (API + MySQL) qua Docker Compose
docker-compose up -d --build
```

Sau khi khởi chạy:
- **MySQL** chạy tại cổng `3307` (ánh xạ từ container port `3306`).
- **Backend API** chạy tại cổng `8082` (ánh xạ từ container port `8083`).
- Base URL API: `http://localhost:8082/api/`

### 2. Khởi chạy Frontend C#

**Yêu cầu:** Visual Studio 2022+ và .NET SDK.

1. Mở thư mục `frontend-csharp` bằng Visual Studio.
2. Khôi phục các gói NuGet (Restore NuGet Packages).
3. Biên dịch và Chạy dự án (Nhấn `F5` hoặc nút `Start`).
4. Đăng nhập bằng tài khoản `admin` / `admin123` hoặc bất kỳ tài khoản nhân viên nào ở bảng trên.

> **Lưu ý:** Mặc định Frontend trỏ tới API production (`https://passbook.kienhuy-dev.name.vn/api/`). Nếu muốn chạy local, mở file `Services/ApiService.cs` và đổi `BaseUrl` sang `http://localhost:8082/api/`.

---

## 📚 Danh Sách Chức Năng & User Flows (Luồng Người Dùng)

Dưới đây là sơ đồ chi tiết các luồng nghiệp vụ của hệ thống. Nhấn vào từng link để xem sơ đồ flowchart (Mermaid Diagram).

1. **Đăng nhập & Phân quyền**: [Xem User Flow](docs/login_flow.md)
2. **Quản lý Khách hàng** (Thêm, Sửa, Tra cứu): [Xem User Flow](docs/customer_management_flow.md)
3. **Quản lý Sổ tiết kiệm**:
   - [Mở Sổ Tiết Kiệm Mới](docs/open_passbook_flow.md)
   - [Tra Cứu Sổ Tiết Kiệm](docs/search_passbook_flow.md)
4. **Quản lý Giao dịch**:
   - [Lập Phiếu Gửi Tiền](docs/deposit_flow.md)
   - [Lập Phiếu Rút Tiền](docs/withdraw_flow.md)
5. **Báo cáo & Thống kê**:
   - [Báo Cáo Doanh Số Hoạt Động Ngày](docs/daily_report_flow.md)
   - [Báo Cáo Mở/Đóng Sổ Tháng](docs/monthly_report_flow.md)
6. **Cấu hình & Quản trị**:
   - [Quản Lý Quy Định / Tham Số](docs/regulation_management_flow.md)
   - [Quản Lý Nhân Viên (Admin)](docs/staff_management_flow.md)

---

## 🔒 Bảo mật

Hệ thống sử dụng **JWT (JSON Web Token)** để bảo mật các API endpoints. Khi đăng nhập thành công từ Frontend C#, token sẽ được lưu trữ và đính kèm vào Authorization header (`Bearer <token>`) trong các yêu cầu gửi đến Server. Mỗi API endpoint có sự phân quyền rõ ràng giữa vai trò Nhân viên (`NHAN_VIEN`) và Quản trị viên (`ADMIN`).

Mật khẩu được mã hóa bằng **BCrypt** trước khi lưu vào cơ sở dữ liệu. Hệ thống hỗ trợ chức năng **Đổi mật khẩu** cho người dùng đang đăng nhập và **Reset mật khẩu** (về `123456`) cho Admin quản lý nhân viên.
