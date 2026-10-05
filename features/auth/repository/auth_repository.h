#ifndef AUTH_REPOSITORY_H
#define AUTH_REPOSITORY_H

#include <QObject>
#include <QString>

#include "features/auth/service/dto/create_profile_model.h"
#include "features/auth/service/dto/login_model.h"
#include "features/auth/service/dto/verify_code_model.h"

#include "core/network/http/model/request_data.h"

class LoginSource;
class CreateProfileSource;
class VerifyCodeSource;
class PoWProcess;
class TokenManager;
struct RequestData;

class AuthRepository : public QObject
{
    Q_OBJECT
public:

    explicit AuthRepository(QObject *parent = nullptr);
    virtual ~AuthRepository() = default;

private:
    using RequestModel = std::variant<CreateProfileRequest,
                                      LoginRequest,
                                      VerifyCodeRequest>;

    LoginSource* _LoginSource;
    CreateProfileSource* _CreateProfileSource;
    VerifyCodeSource* _VerifyCodeSource;
    PoWProcess* _PoWProcess;
    TokenManager* _TokenManager;

    QMap<QString, QString> defineHeaders();
    std::pair<QString, QString> defineMessage(int code);
    RequestData buildRequest(const RequestModel& model,
                             std::function<void(const ResponseData&, int, std::pair<QString, QString>)> func);

public:
    void login(const QString& email);

    void createProfile(const QString& name,
                       const QString& username,
                       const QString& email);

    void verifyCode(const QString& email,
                    int code);

signals:
    void loginFinished(const QString& messageHeader,
                       const QString& messageBody,
                       int code);

    void createProfileFinished(const QString& messageHeader,
                               const QString& messageBody,
                               int code);

    void verifyCodeFinished(const QString& messageHeader,
                            const QString& messageBody,
                            int code);

};

#endif // AUTH_REPOSITORY_H
