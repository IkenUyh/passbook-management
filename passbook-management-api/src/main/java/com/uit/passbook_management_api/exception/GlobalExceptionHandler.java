package com.uit.passbook_management_api.exception;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import java.util.HashMap;
import java.util.Map;

@RestControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(AccessDeniedException.class)
    public ResponseEntity<?> handleAccessDeniedException(AccessDeniedException ex) {
        Map<String, Object> errors = new HashMap<>();
        errors.put("status", HttpStatus.FORBIDDEN.value());
        errors.put("error", "Forbidden");
        errors.put("message", "Bạn không có quyền thực hiện hành động này!");

        return new ResponseEntity<>(errors, HttpStatus.FORBIDDEN);
    }
}