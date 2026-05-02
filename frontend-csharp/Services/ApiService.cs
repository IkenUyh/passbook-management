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

        private const string BaseUrl = "http://localhost:8081/api/";

        public ApiService()
        {
            _client = new HttpClient();
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
                // Gọi tới http://localhost:8081/api/auth/login
                HttpResponseMessage response = await _client.PostAsync($"{BaseUrl}auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponse>(responseString);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối: {ex.Message}");
                return null;
            }
        }

        // --- 2. 
        public async Task<List<SoTietKiem>> GetDanhSachSoTietKiemAsync()
        {
            // BỔ SUNG: Kiểm tra xem đã có Token lưu trong Session chưa
            if (!string.IsNullOrEmpty(AppSession.CurrentToken))
            {
                // Nhét Token vào Header của request dưới dạng Bearer Token
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppSession.CurrentToken);
            }

            // Gọi GET request tới http://localhost:8081/api/v1/so-tiet-kiem
            var response = await _client.GetAsync($"{BaseUrl}v1/so-tiet-kiem");

            // Hàm này sẽ ném ra lỗi (Exception) nếu API trả về 401 hoặc 500
            response.EnsureSuccessStatusCode();

            // Đọc JSON trả về
            var json = await response.Content.ReadAsStringAsync();

            // Convert JSON thành List object C#. Nếu mảng [] thì nó tự hiểu là List rỗng (Count = 0).
            return JsonSerializer.Deserialize<List<SoTietKiem>>(json) ?? new List<SoTietKiem>();
        }
    }
}