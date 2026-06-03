package com.uit.passbook_management_api.controller;

import com.google.api.client.googleapis.auth.oauth2.GoogleIdToken;
import com.google.api.client.googleapis.auth.oauth2.GoogleIdTokenVerifier;
import com.google.api.client.http.javanet.NetHttpTransport;
import com.google.api.client.json.gson.GsonFactory;
import com.uit.passbook_management_api.dto.request.GoogleLoginRequest;
import com.uit.passbook_management_api.dto.request.LoginRequest;
import com.uit.passbook_management_api.dto.response.AuthResponse;
import com.uit.passbook_management_api.entity.AppUser;
import com.uit.passbook_management_api.repository.AppUserRepository;
import com.uit.passbook_management_api.security.JwtUtil;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;

import java.util.Collections;
import java.util.Optional;

@RestController
@PreAuthorize("hasAnyRole('ADMIN', 'NHAN_VIEN')")
@RequestMapping("/api/auth")
public class AuthController {

    @Autowired
    private AppUserRepository appUserRepository;

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Autowired
    private JwtUtil jwtUtil;

    // THAY THẾ CHUỖI NÀY BẰNG CLIENT ID CỦA BẠN TRÊN GOOGLE CLOUD CONSOLE
    private static final String GOOGLE_CLIENT_ID = "84761025681-ppm8au4223kd6knngi22upihiak21kos.apps.googleusercontent.com";

    @PostMapping("/login")
    public ResponseEntity<?> loginAdmin(@RequestBody LoginRequest request) {
        // 1. Tìm user trong Database theo username
        Optional<AppUser> userOptional = appUserRepository.findByUsername(request.getUsername());

        // 2. Nếu tìm thấy user, tiếp tục kiểm tra mật khẩu
        if (userOptional.isPresent()) {
            AppUser user = userOptional.get();

            // So sánh mật khẩu người dùng nhập vào với mật khẩu đã băm trong DB
            if (passwordEncoder.matches(request.getPassword(), user.getPassword())) {
                // 3. Đúng mật khẩu -> Sinh ra JWT Token (ĐÃ CẬP NHẬT TRUYỀN THÊM ROLE)
                String token = jwtUtil.generateToken(user.getUsername(), user.getRole());

                // Trả về AuthResponse kèm cả Role cho phía WPF nhận biết
                return ResponseEntity.ok(new AuthResponse(token, user.getUsername(), user.getRole()));
            }
        }

        // Trả về lỗi 401 nếu sai user hoặc sai pass
        return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("Tài khoản hoặc mật khẩu không chính xác");
    }

    @PostMapping("/google")
    public ResponseEntity<?> loginGoogle(@RequestBody GoogleLoginRequest request) {
        try {
            // Khởi tạo bộ xác minh token của Google
            GoogleIdTokenVerifier verifier = new GoogleIdTokenVerifier.Builder(new NetHttpTransport(), new GsonFactory())
                    .setAudience(Collections.singletonList(GOOGLE_CLIENT_ID))
                    .build();

            // Xác minh token từ phía WPF gửi lên
            GoogleIdToken idToken = verifier.verify(request.getIdToken());

            if (idToken != null) {
                GoogleIdToken.Payload payload = idToken.getPayload();

                // Lấy email từ token của Google
                String email = payload.getEmail();

                // Kiểm tra xem email này đã được Admin thêm vào bảng app_user chưa
                Optional<AppUser> userOptional = appUserRepository.findByUsername(email);

                if (userOptional.isPresent()) {
                    // Email đã được cấp phép -> Sinh token JWT của hệ thống (ĐÃ CẬP NHẬT TRUYỀN THÊM ROLE)
                    AppUser user = userOptional.get();
                    String token = jwtUtil.generateToken(user.getUsername(), user.getRole());

                    // Trả về AuthResponse kèm cả Role
                    return ResponseEntity.ok(new AuthResponse(token, user.getUsername(), user.getRole()));
                } else {
                    // Email chưa được cấp phép -> Báo lỗi 403 Forbidden
                    return ResponseEntity.status(HttpStatus.FORBIDDEN)
                            .body("Tài khoản Google này chưa được cấp phép truy cập hệ thống.");
                }
            } else {
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("Token Google không hợp lệ.");
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("Lỗi xác thực với Google: " + e.getMessage());
        }
    }
}