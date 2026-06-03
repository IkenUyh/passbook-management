package com.uit.passbook_management_api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

@SpringBootApplication
@EnableScheduling
@EnableMethodSecurity
public class PassbookManagementApiApplication {

	public static void main(String[] args) {

        String passwordTho = "123456";
        String chuoiMahoaChuan = new BCryptPasswordEncoder(12).encode("123456");
        System.out.println("====== CHUỖI MÃ HÓA CHUẨN TRÊN MÁY BẠN LÀ: " + chuoiMahoaChuan);
        SpringApplication.run(PassbookManagementApiApplication.class, args);

	}

}
