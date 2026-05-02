package com.uit.passbook_management_api.security;

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

    // 1. Tạo Token (Hàm gốc của bạn)
    public String generateToken(String username) {
        return Jwts.builder()
                .setSubject(username)
                .setIssuedAt(new Date())
                .setExpiration(new Date(System.currentTimeMillis() + EXPIRATION_TIME))
                .signWith(key)
                .compact();
    }

    // 2. Lấy Username từ Token (BỔ SUNG)
    public String extractUsername(String token) {
        return Jwts.parserBuilder()
                .setSigningKey(key)
                .build()
                .parseClaimsJws(token) // Đọc token, sẽ ném lỗi nếu token bị fake
                .getBody()
                .getSubject();         // Lấy ra chuỗi username đã lưu lúc tạo
    }

    // 3. Kiểm tra Token còn hợp lệ không (BỔ SUNG)
    public boolean validateToken(String token) {
        try {
            // Nếu parse thành công mà không có lỗi (Exception) thì token hợp lệ
            Jwts.parserBuilder()
                    .setSigningKey(key)
                    .build()
                    .parseClaimsJws(token);
            return true;
        } catch (Exception e) {
            // Các lỗi có thể xảy ra: Token hết hạn, chữ ký không khớp, chuỗi token bị sửa đổi
            System.out.println("Lỗi Token: " + e.getMessage());
            return false;
        }
    }
}