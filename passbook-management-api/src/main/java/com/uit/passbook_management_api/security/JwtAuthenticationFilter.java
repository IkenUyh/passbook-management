package com.uit.passbook_management_api.security;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.util.ArrayList;

@Component
public class JwtAuthenticationFilter extends OncePerRequestFilter {

    @Autowired
    private JwtUtil jwtUtil;

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response, FilterChain filterChain)
            throws ServletException, IOException {

        // Lấy Header Authorization từ Request
        final String authHeader = request.getHeader("Authorization");
        String username = null;
        String jwtToken = null;

        // Kiểm tra xem Header có chứa Bearer Token không
        if (authHeader != null && authHeader.startsWith("Bearer ")) {
            jwtToken = authHeader.substring(7); // Cắt bỏ chữ "Bearer " để lấy đúng chuỗi mã
            try {
                username = jwtUtil.extractUsername(jwtToken);
            } catch (Exception e) {
                System.out.println("Không thể parse JWT Token hoặc Token đã hết hạn");
            }
        }

        // Nếu có Token và hệ thống chưa xác thực
        if (username != null && SecurityContextHolder.getContext().getAuthentication() == null) {

            // Kiểm tra Token có đúng không
            if (jwtUtil.validateToken(jwtToken)) {

                // Xác thực thành công -> Cấp thẻ xanh (AuthenticationToken) cho Request này
                UsernamePasswordAuthenticationToken authToken = new UsernamePasswordAuthenticationToken(
                        username, null, new ArrayList<>()); // Ở đây truyền danh sách quyền rỗng vì app chưa có phân quyền phức tạp

                authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                // Lưu vào SecurityContext
                SecurityContextHolder.getContext().setAuthentication(authToken);
            }
        }

        // Chuyển request đi tiếp tới Controller
        filterChain.doFilter(request, response);
    }
}