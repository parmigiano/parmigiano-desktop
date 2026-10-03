#include "auth_repository.h"

#include "core/config/app_config.h"
#include "core/network/http/model/request_data.h"

#include "features/auth/service/dto/create_profile_model.h"
#include "features/auth/service/dto/login_model.h"
#include "features/auth/service/dto/verify_code_model.h"

#include "features/auth/service/remote/login_source.h"
#include "features/auth/service/remote/create_profile_source.h"
#include "features/auth/service/remote/verify_code_source.h"

#include "domain/pow_process.h"
#include "domain/token_manager.h"

#include <QNetworkInformation>
#include <QMetaEnum>
#include <qjsonobject.h>
#include <qtimer.h>

AuthRepository::AuthRepository(QObject *parent)
    : QObject{parent}
    , _LoginSource(new LoginSource(nullptr, AppConfig::loginEndPoint()))
    , _CreateProfileSource(new CreateProfileSource(nullptr, AppConfig::createProfileEndPoint()))
    , _VerifyCodeSource(new VerifyCodeSource(nullptr, AppConfig::verifyCodeEndPoint()))
    , _PoWProcess(PoWProcess::getInstance())
    , _TokenManager(TokenManager::getInstance())
{ }

QMap<QString, QString> AuthRepository::defineHeaders()
{
    QMap<QString, QString> headers;
    auto nonce = _PoWProcess->getNonce();
    QString challenge = _PoWProcess->getChallenge();

    if (!challenge.isEmpty())
    {
        headers["pow-challenge"] = challenge;
    }

    if (nonce.has_value())
    {
        QString value = QString::number(*nonce);
        headers["pow-nonce"] = value;
    }

    headers["Accept-Language"] = "en";

    return headers;
}

std::pair<QString, QString> AuthRepository::defineMessage(int code)
{
    QString messageHeader = "";
    QString messageBody = "";

    if (code >= 400 && code < 500)
    {
        messageHeader = tr("Client side error");
        messageBody = tr("Check your internet connection and try again later.");
    }
    else if (code >= 500 || code == 0)
    {
        messageHeader = tr("Server is unavailable");
        messageBody = tr("Server connection error. Please try again later.");
    }

    return std::make_pair(messageHeader, messageBody);
}

void AuthRepository::login(const QString &email)
{
    LoginRequest model{email};
    QJsonObject jsonObj = model.toJson();
    QMap<QString, QString> headers = defineHeaders();

    auto onResult = [this] (const ResponseData& result)
    {
        int code = result.code;
        std::pair<QString, QString> message = defineMessage(code);

        emit loginFinished(message.first, message.second, code);
    };

    RequestData reqData{onResult, jsonObj, headers};

    _LoginSource->login(reqData);
}

void AuthRepository::createProfile(const QString &name,
                                   const QString &username,
                                   const QString &email)
{
    CreateProfileRequest model{name, username, email};
    QJsonObject jsonObj = model.toJson();
    QMap<QString, QString> headers = defineHeaders();

    auto onResult = [this] (const ResponseData& result)
    {
        int code = result.code;
        std::pair<QString, QString> message = defineMessage(code);

        if (code == 200)
        {
            _TokenManager->save(result.jsonData.value("message").toString());
        }

        emit createProfileFinished(message.first, message.second, code);
    };

    RequestData reqData{onResult, jsonObj, headers};

    _CreateProfileSource->createProfile(reqData);
}

void AuthRepository::verifyCode(const QString &email, int code)
{
    VerifyCodeRequest model{email, code};
    QJsonObject jsonObj = model.toJson();
    QMap<QString, QString> headers = defineHeaders();

    auto onResult = [this] (const ResponseData& result)
    {
        int code = result.code;
        std::pair<QString, QString> message = defineMessage(code);

        if (code == 200)
        {
            _TokenManager->save(result.jsonData.value("message").toString());
        }

        emit verifyCodeFinished(message.first, message.second, code);
    };

    RequestData reqData{onResult, jsonObj, headers};

    _VerifyCodeSource->verifyCode(reqData);
}
