package com.uit.passbook_management_api.entity;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Collection;
import java.util.List;

@Entity
@Table(name = "app_user")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AppUser implements UserDetails { // 1. Kế thừa UserDetails ở đây

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true, nullable = false, length = 50)
    private String username;

    @Column(nullable = false)
    private String password;

    @Column(length = 20)
    private String role; // Giữ nguyên String (Giá trị trong DB sẽ là "ADMIN" hoặc "NHAN_VIEN")

    // =========================================================================
    // 2. TRIỂN KHAI CÁC HÀM BẮT BUỘC CỦA USERDETAILS
    // =========================================================================

    @Override
    public Collection<? extends GrantedAuthority> getAuthorities() {
        // Spring Security yêu cầu Quyền phải có tiền tố "ROLE_" để dùng được hàm hasRole()
        // Ví dụ: nếu trường role là "ADMIN" -> sẽ chuyển thành quyền "ROLE_ADMIN"
        return List.of(new SimpleGrantedAuthority("ROLE_" + this.role));
    }


    @Override
    public boolean isAccountNonExpired() {
        return true; // Trả về true để tài khoản không bao giờ bị hết hạn
    }

    @Override
    public boolean isAccountNonLocked() {
        return true; // Trả về true để tài khoản không bị khóa
    }

    @Override
    public boolean isCredentialsNonExpired() {
        return true; // Trả về true để mật khẩu/chứng chỉ không bị hết hạn
    }

    @Override
    public boolean isEnabled() {
        return true; // Trả về true để tài khoản luôn ở trạng thái kích hoạt/hoạt động
    }
}