#ifndef APP_CONFIG_H
#define APP_CONFIG_H

#include <QString>

namespace AppConfig {
    inline constexpr char baseURL[] = "http://localhost:8080/api/v2/";     // http-server
    inline constexpr char authBaseURL[] = "http://localhost:8081/api/v2/"; // auth-server

    inline QString loginEndPoint() { return QString(authBaseURL) + "auth/confirm/email"; }
    inline QString createProfileEndPoint() { return QString(authBaseURL) + "auth/create"; }
    inline QString verifyCodeEndPoint() { return QString(authBaseURL) + "auth/verify"; }
    inline QString passwordLoginEndPoint() { return QString(authBaseURL) + "auth/login"; }
    inline QString logoutEndPoint() { return QString(authBaseURL) + "auth/logout"; }
    inline QString deleteAccountEndPoint() { return QString(authBaseURL) + "auth/delete"; }
}

#endif // CONFIG_H
