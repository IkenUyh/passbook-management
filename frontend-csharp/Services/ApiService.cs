using frontend_csharp.Models.AuditLogModel;
using frontend_csharp.Models.Auth;
using frontend_csharp.Models.BaoCaoModel;
using frontend_csharp.Models.GiaoDichModel;
using frontend_csharp.Models.KhachHangModel;
using frontend_csharp.Models.NhanVienModel;
using frontend_csharp.Models.QuyDinhModel;
using frontend_csharp.Models.SoTietKiem.SoTietKiemModel;
using frontend_csharp.Models.SoTietKiemModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace frontend_csharp.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        //private const string BaseUrl = "http://localhost:8083/api/";
        private const string BaseUrl = "https://passbook.kienhuy-dev.name.vn/api/";

        public ApiService()
        {
            _client = new HttpClient();

            // Cấu hình đọc JSON không phân biệt hoa thường để tránh lỗi lệch kiểu ký tự với Java
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Hàm helper tự động đính kèm JWT Token vào Header của request
        /// </summary>
        private void AddAuthHeader()
        {
            if (!string.IsNullOrEmpty(AppSession.CurrentToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppSession.CurrentToken);
            }
        }

        // ==========================================
        // --- 1. (AUTH) ---
        // ==========================================

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var requestData = new LoginRequest
            {
                Username = username,
                Password = password
            };

            string json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"{BaseUrl}auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    var authResult = JsonSerializer.Deserialize<AuthResponse>(responseString, _jsonOptions);

                    if (authResult != null)
                    {
                        AppSession.CurrentToken = authResult.Token;
                        AppSession.LoggedInUsername = authResult.Username;
                        AppSession.CurrentRole = authResult.Role;
                    }

                    return authResult;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối API Đăng nhập: {ex.Message}");
                return null;
            }
        }

        // ==========================================
        // --- 2. QUẢN LÝ SỔ TIẾT KIỆM ---
        // ==========================================

        public async Task<List<SoTietKiem>> GetDanhSachSoTietKiemAsync()
        {
            try
            {
                AddAuthHeader();

                var response = await _client.GetAsync($"{BaseUrl}v1/so-tiet-kiem");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SoTietKiem>>(json, _jsonOptions) ?? new List<SoTietKiem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách sổ: {ex.Message}");
                return new List<SoTietKiem>();
            }
        }

        /// Gửi yêu cầu mở sổ tiết kiệm mới (Nhập trực tiếp thông tin khách hàng)
        public async Task<bool> MoSoTietKiemAsync(Models.SoTietKiem.SoTietKiemModel.MoSoRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Route gọi đến SoTietKiemController ở backend để xử lý mở sổ
                var response = await _client.PostAsync($"{BaseUrl}v1/so-tiet-kiem/mo-so", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API mở sổ tiết kiệm: {ex.Message}");
                return false;
            }
        }


        ///
        /// Lấy danh sách sổ sắp đáo hạn
        /// 
        public async Task<List<SoTietKiem>> GetDanhSachSoSapDaoHanAsync(int soNgayBaoTruoc = 3)
        {
            try
            {
                AddAuthHeader();
                var response = await _client.GetAsync($"{BaseUrl}v1/so-tiet-kiem/sap-dao-han?soNgayBaoTruoc={soNgayBaoTruoc}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SoTietKiem>>(json, _jsonOptions) ?? new List<SoTietKiem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách nhắc đáo hạn: {ex.Message}");
                return new List<SoTietKiem>();
            }
        }

        // ==========================================
        // --- 3. QUẢN LÝ KHÁCH HÀNG ---
        // ==========================================

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng 
        /// </summary>
        public async Task<List<KhachHang>> GetDanhSachKhachHangAsync()
        {
            try
            {
                AddAuthHeader();

                // Lưu ý: Route "v1/khach-hang" đặt theo mẫu "v1/so-tiet-kiem" của bạn.
                // Hãy thay đổi lại nếu Backend quy định khác.
                var response = await _client.GetAsync($"{BaseUrl}v1/khach-hang");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<KhachHang>>(json, _jsonOptions) ?? new List<KhachHang>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách khách hàng: {ex.Message}");
                return new List<KhachHang>();
            }
        }

        /// <summary>
        /// Thêm mới khách hàng 
        /// </summary>
        /// <summary>
        /// Thêm mới khách hàng (Truyền vào KhachHangRequest không có ID)
        /// </summary>
        public async Task<bool> CreateKhachHangAsync(KhachHangRequest request)
        {
            try
            {
                AddAuthHeader();
                string json = JsonSerializer.Serialize(request); // JSON sinh ra sẽ sạch đẹp, không có "id"
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{BaseUrl}v1/khach-hang", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm mới: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng 
        /// </summary>
        public async Task<bool> UpdateKhachHangAsync(long id, KhachHangRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"{BaseUrl}v1/khach-hang/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật thông tin khách hàng ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa hồ sơ khách hàng 
        /// </summary>
        public async Task<bool> DeleteKhachHangAsync(long id)
        {
            try
            {
                AddAuthHeader();

                var response = await _client.DeleteAsync($"{BaseUrl}v1/khach-hang/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa khách hàng ID {id}: {ex.Message}");
                return false;
            }
        }

        // ==========================================
        // --- 4. QUẢN LÝ GIAO DỊCH ---
        // ==========================================

        /// Gửi thêm tiền vào một sổ tiết kiệm đã tồn tại
        public async Task<bool> GuiTienAsync(GuiTienRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Route gọi đến GiaoDichController ở backend để xử lý gửi tiền vào sổ
                var response = await _client.PostAsync($"{BaseUrl}v1/giao-dich/gui-tien", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API gửi tiền vào sổ: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Rút tiền từ sổ tiết kiệm
        /// </summary>
        public async Task<bool> RutTienAsync(RutTienRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{BaseUrl}v1/giao-dich/rut-tien", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API rút tiền: {ex.Message}");
                return false;
            }
        }

        // ==========================================
        // --- 5. QUẢN LÝ BÁO CÁO  ---
        // ==========================================

        /// 
        /// Lấy báo cáo doanh số hoạt động theo ngày
        /// 
        public async Task<List<BaoCaoNgayDTO>> GetBaoCaoNgayAsync(DateTime ngay)
        {
            try
            {
                AddAuthHeader();

                string ngayFormat = ngay.ToString("yyyy-MM-dd");

                var response = await _client.GetAsync($"{BaseUrl}v1/bao-cao/ngay?ngay={ngayFormat}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<BaoCaoNgayDTO>>(json, _jsonOptions) ?? new List<BaoCaoNgayDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy báo cáo ngày: {ex.Message}");
                return new List<BaoCaoNgayDTO>();
            }
        }

        ///
        /// Lấy báo cáo tình hình đóng/mở sổ theo tháng và năm
        /// 
        public async Task<List<BaoCaoThangDTO>> GetBaoCaoThangAsync(int thang, int nam)
        {
            try
            {
                AddAuthHeader();

                // Đẩy 2 tham số RequestParam lên URL
                var response = await _client.GetAsync($"{BaseUrl}v1/bao-cao/thang?thang={thang}&nam={nam}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<BaoCaoThangDTO>>(json, _jsonOptions) ?? new List<BaoCaoThangDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy báo cáo tháng: {ex.Message}");
                return new List<BaoCaoThangDTO>();
            }
        }


        // ==========================================
        // --- 6. QUẢN LÝ NHÂN VIÊN  ---
        // ==========================================

        /// 
        /// Lấy danh sách toàn bộ nhân viên 
        /// 
        public async Task<List<NhanVien>> GetDanhSachNhanVienAsync()
        {
            try
            {
                AddAuthHeader();

                var response = await _client.GetAsync($"{BaseUrl}v1/nhan-vien");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<NhanVien>>(json, _jsonOptions) ?? new List<NhanVien>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách nhân viên: {ex.Message}");
                return new List<NhanVien>();
            }
        }

        /// 
        /// Thêm mới một nhân viên và cấp tài khoản hệ thống (Chỉ ADMIN)
        /// 
        public async Task<NhanVienResponse> CreateNhanVienAsync(NhanVienRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{BaseUrl}v1/nhan-vien", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NhanVienResponse>(responseJson, _jsonOptions);
                }

                // Nếu backend trả về 400 Bad Request (Ví dụ: trùng CCCD)
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi nghiệp vụ từ Server: {errorMsg}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối tới API thêm nhân viên: {ex.Message}");
                return null;
            }
        }



        /// 
        /// Cập nhật thông tin cá nhân của nhân viên 
        /// 
        public async Task<bool> UpdateNhanVienAsync(string id, CapNhatNhanVienRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gọi phương thức PutAsync và truyền id lên URL Path đúng theo @PutMapping("/{id}") ở backend
                var response = await _client.PutAsync($"{BaseUrl}v1/nhan-vien/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Đọc log lỗi từ backend quăng về (ví dụ: trùng CCCD) để dễ debug
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi từ Server khi sửa nhân viên: {errorMsg}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối đến API để cập nhật nhân viên: {ex.Message}");
                return false;
            }
        }


        /// 
        /// Thay đổi mật khẩu tài khoản đang đăng nhập 
        /// 
        public async Task<string> DoiMatKhauAsync(DoiMatKhauRequest request)
        {
            try
            {
                AddAuthHeader(); 

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"{BaseUrl}v1/nhan-vien/doi-mat-khau", content);
                string resultMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return "Thành công";
                }

                // Trả về lý do thất bại từ backend (Ví dụ: "Mật khẩu cũ không chính xác!")
                return resultMessage;
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối API: {ex.Message}";
            }
        }


        // ==========================================
        // --- 7. QUẢN LÝ QUY ĐỊNH & THAM SỐ ---
        // ==========================================

        /// 
        /// Xem các tham số hiện tại 
        /// 
        public async Task<ThamSo> GetThamSoChungAsync()
        {
            try
            {
                AddAuthHeader();

                var response = await _client.GetAsync($"{BaseUrl}v1/quy-dinh/tham-so");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ThamSo>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy cấu hình tham số: {ex.Message}");
                return null;
            }
        }

        /// 
        /// Cập nhật các quy định về tiền gửi và thời gian tối thiểu 
        /// 
        public async Task<bool> UpdateThamSoChungAsync(CapNhatThamSoRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"{BaseUrl}v1/quy-dinh/tham-so", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật tham số hệ thống: {ex.Message}");
                return false;
            }
        }

        /// 
        /// Xem danh sách các loại kỳ hạn và lãi suất hiện tại
        /// 
        public async Task<List<LoaiTietKiem>> GetDanhSachLoaiTietKiemAsync()
        {
            try
            {
                AddAuthHeader();

                var response = await _client.GetAsync($"{BaseUrl}v1/quy-dinh/loai-tiet-kiem");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<LoaiTietKiem>>(json, _jsonOptions) ?? new List<LoaiTietKiem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách loại tiết kiệm: {ex.Message}");
                return new List<LoaiTietKiem>();
            }
        }

        /// 
        /// Thêm mới hoặc Cập nhật thông tin/lại suất của một loại kỳ hạn 
        /// 
        public async Task<bool> SaveLoaiTietKiemAsync(LoaiTietKiemRequest request)
        {
            try
            {
                AddAuthHeader();

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{BaseUrl}v1/quy-dinh/loai-tiet-kiem", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lưu loại tiết kiệm: {ex.Message}");
                return false;
            }
        }
    

    // ==========================================
        // --- 8. AUDIT LOG ---
        // ==========================================

        /// 
        /// Tải toàn bộ lịch sử thao tác của hệ thống 
        /// 
        public async Task<List<AuditLog>> GetDanhSachAuditLogAsync()
        {
            try
            {
                AddAuthHeader();
                var response = await _client.GetAsync($"{BaseUrl}v1/audit-log");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<AuditLog>>(json, _jsonOptions) ?? new List<AuditLog>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy nhật ký hệ thống Audit Log: {ex.Message}");
                return new List<AuditLog>();
            }
        }
    }
}