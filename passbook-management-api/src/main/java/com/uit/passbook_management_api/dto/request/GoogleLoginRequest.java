package com.uit.passbook_management_api.dto.request;

public class GoogleLoginRequest {
    private String idToken;

    public String getIdToken() { return idToken; }
    public void setIdToken(String idToken) { this.idToken = idToken; }
}