using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using frontend_csharp.Models;

namespace frontend_csharp.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string BaseUrl = "http://localhost:8083/api/";
        // private const string BaseUrl = "https://passbook.kienhuy-dev.name.vn/api/";

        public ApiService()
        {
            _client = new HttpClient();

            // Cấu hình đọc JSON không phân biệt hoa thường để tránh lỗi lệch kiểu ký tự với Java
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // --- 1. HÀM ĐĂNG NHẬP ---
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

        // --- 2. LẤY DANH SÁCH SỔ TIẾT KIỆM ---
        public async Task<List<SoTietKiem>> GetDanhSachSoTietKiemAsync()
        {
            try
            {
                // BỔ SUNG: Kiểm tra và đính kèm Token hợp lệ vào Header trước khi bắn request
                if (!string.IsNullOrEmpty(AppSession.CurrentToken))
                {
                         _client.DefaultRequestHeaders.Authorization =
                          new AuthenticationHeaderValue("Bearer", AppSession.CurrentToken);
                }

                var response = await _client.GetAsync($"{BaseUrl}v1/so-tiet-kiem");

                // Nếu Backend trả về 403 Forbidden hoặc 401 Unauthorized, dòng này sẽ quăng Exception ngay
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SoTietKiem>>(json, _jsonOptions) ?? new List<SoTietKiem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách sổ: {ex.Message}");
                return new List<SoTietKiem>(); // Trả về list rỗng để giao diện không bị crash dữ liệu
            }
        }
    }
}