package com.uit.passbook_management_api.security;

import com.uit.passbook_management_api.entity.AppUser;
import com.uit.passbook_management_api.repository.AppUserRepository;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

@Component
public class JwtAuthenticationFilter extends OncePerRequestFilter {

    @Autowired
    private JwtUtil jwtUtil;

    // Tiêm thêm Repository để lấy thông tin chức vụ của User
    @Autowired
    private AppUserRepository appUserRepository;

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response, FilterChain filterChain)
            throws ServletException, IOException {

        final String authHeader = request.getHeader("Authorization");
        String username = null;
        String jwtToken = null;

        // Kiểm tra xem Header có chứa Bearer Token không
        if (authHeader != null && authHeader.startsWith("Bearer ")) {
            jwtToken = authHeader.substring(7);
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

                // Lấy thông tin user từ Database
                AppUser user = appUserRepository.findByUsername(username).orElse(null);
                List<GrantedAuthority> authorities = new ArrayList<>();

                // Gắn chức vụ (Role) vào thẻ xanh nếu user tồn tại
                if (user != null && user.getRole() != null) {
                    authorities.add(new SimpleGrantedAuthority(user.getRole())); // VD: "ADMIN"
                }

                // Xác thực thành công -> Cấp thẻ xanh kèm danh sách quyền (authorities)
                UsernamePasswordAuthenticationToken authToken = new UsernamePasswordAuthenticationToken(
                        username, null, authorities);

                authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                // Lưu vào SecurityContext
                SecurityContextHolder.getContext().setAuthentication(authToken);
            }
        }

        // Chuyển request đi tiếp tới Controller
        filterChain.doFilter(request, response);
    }
}