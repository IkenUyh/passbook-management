package com.uit.passbook_management_api.security;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import org.springframework.stereotype.Component;

import java.security.Key;
import java.util.Date;

@Component
public class JwtUtil {
    // Trong thực tế, secret key này nên được đọc từ application.yaml
    private static final String SECRET = "my-secret-key-my-secret-key-my-secret-key";
    private static final Key key = Keys.hmacShaKeyFor(SECRET.getBytes());
    private static final long EXPIRATION_TIME = 86400000; // 1 ngày

    // 1. Tạo Token - SỬA ĐỔI: Nhận thêm tham số role và đẩy vào Claim
    public String generateToken(String username, String role) {
        return Jwts.builder()
                .claim("role", role) // <-- BỔ SUNG: Lưu role (ADMIN/NHAN_VIEN) vào payload của token
                .setSubject(username)
                .setIssuedAt(new Date())
                .setExpiration(new Date(System.currentTimeMillis() + EXPIRATION_TIME))
                .signWith(key)
                .compact();
    }

    // 2. Lấy Username từ Token (Giữ nguyên)
    public String extractUsername(String token) {
        return Jwts.parserBuilder()
                .setSigningKey(key)
                .build()
                .parseClaimsJws(token)
                .getBody()
                .getSubject();
    }

    // 3. Lấy Role từ Token - BỔ SUNG MỚI
    public String extractRole(String token) {
        return Jwts.parserBuilder()
                .setSigningKey(key)
                .build()
                .parseClaimsJws(token)
                .getBody()
                .get("role", String.class); // <-- Trích xuất claim có key là "role" dưới dạng String
    }

    // 4. Kiểm tra Token còn hợp lệ không (Giữ nguyên)
    public boolean validateToken(String token) {
        try {
            Jwts.parserBuilder()
                    .setSigningKey(key)
                    .build()
                    .parseClaimsJws(token);
            return true;
        } catch (Exception e) {
            System.out.println("Lỗi Token: " + e.getMessage());
            return false;
        }
    }
}