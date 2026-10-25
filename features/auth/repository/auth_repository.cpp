#include "auth_repository.h"

#include "core/config/app_config.h"

#include "features/auth/service/remote/auth_source.h"

#include "features/auth/service/dto/create_profile_model.h"
#include "features/auth/service/dto/login_model.h"
#include "features/auth/service/dto/verify_code_model.h"

#include "domain/token_manager.h"

#include <QNetworkInformation>
#include <QMetaEnum>
#include <qjsonobject.h>
#include <qtimer.h>

AuthRepository::AuthRepository(QObject *parent)
    : _AuthSource(new AuthSource(this))
{ }

void AuthRepository::login(const QString &email)
{
    LoginRequest model{email};
    RequestData reqData = buildRequest(
        AppConfig::loginEndPoint(),
        std::optional<LoginRequest>(model),
        [this] (const ResponseData& result, int code,
               std::pair<QString, QString> message)
        {
            emit loginFinished(RequestResult{message.first, message.second, code});
        });

    _AuthSource->login(reqData);
}

void AuthRepository::createProfile(const QString &name,
                                   const QString &username,
                                   const QString &email)
{
    CreateProfileRequest model{name, username, email, _TokenManager->token("registration_token")};
    RequestData reqData = buildRequest(
        AppConfig::createProfileEndPoint(),
        std::optional<CreateProfileRequest>(model),
        [this] (const ResponseData& result, int code,
               std::pair<QString, QString> message)
        {
            if (code == 201)
            {
                _TokenManager->save("session_token", result.jsonData.value("message").toString());
            }

            emit createProfileFinished(RequestResult{message.first, message.second, code});
        });

    _AuthSource->createProfile(reqData);
}

void AuthRepository::verifyCode(const QString &email, int code)
{
    VerifyCodeRequest model{email, code};
    RequestData reqData = buildRequest(
        AppConfig::verifyCodeEndPoint(),
        std::optional<VerifyCodeRequest>(model),
        [this] (const ResponseData& result, int code,
                std::pair<QString, QString> message)
        {
            if (code == 200)
            {
                _TokenManager->save("registration_token", result.jsonData.value("registration_token").toString());
            }

            emit verifyCodeFinished(RequestResult{message.first, message.second, code}, result.jsonData.value("next").toString());
            // emit verifyCodeFinished(, );
        });

    _AuthSource->verifyCode(reqData);
}
