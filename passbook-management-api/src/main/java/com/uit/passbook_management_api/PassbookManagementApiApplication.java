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
        SpringApplication.run(PassbookManagementApiApplication.class, args);

	}

}
