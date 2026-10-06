#include "auth_repository.h"

#include "core/config/app_config.h"

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
        messageHeader = qtTrId("error.client.title");
        messageBody = qtTrId("error.client.body");
    }
    else if (code >= 500 || code == 0)
    {
        messageHeader = qtTrId("error.server.title");
        messageBody = qtTrId("error.server.body");
    }

    return std::make_pair(messageHeader, messageBody);
}

RequestData AuthRepository::buildRequest(const RequestModel &model,
                                         std::function<void(const ResponseData&, int, std::pair<QString, QString>)> func)
{
    RequestData reqData;
    QMap<QString, QString> headers = defineHeaders();

    auto onResult = [this, func] (const ResponseData& result)
    {
        int code = result.code;
        std::pair<QString, QString> message = defineMessage(code);

        if (func) func(result, code, message);
    };

    reqData.headers = headers;
    reqData.callback = onResult;

    std::visit([&](const auto& model) {
        QJsonObject jsonObj = model.toJson();
        reqData.jsonObj = jsonObj;
    }, model);

    return reqData;
}

void AuthRepository::login(const QString &email)
{
    LoginRequest model{email};
    RequestData reqData = buildRequest(
        model,
        [this] (const ResponseData& result, int code,
               std::pair<QString, QString> message)
        {
            emit loginFinished(message.first, message.second, code);
        });

    _LoginSource->login(reqData);
}

void AuthRepository::createProfile(const QString &name,
                                   const QString &username,
                                   const QString &email)
{
    CreateProfileRequest model{name, username, email};
    RequestData reqData = buildRequest(
        model,
        [this] (const ResponseData& result, int code,
               std::pair<QString, QString> message)
        {
            if (code == 201)
            {
                _TokenManager->save(result.jsonData.value("message").toString());
            }

            emit createProfileFinished(message.first, message.second, code);
        });

    _CreateProfileSource->createProfile(reqData);
}

void AuthRepository::verifyCode(const QString &email, int code)
{
    VerifyCodeRequest model{email, code};
    RequestData reqData = buildRequest(
        model,
        [this] (const ResponseData& result, int code,
                std::pair<QString, QString> message)
        {
            if (code == 201)
            {
               _TokenManager->save(result.jsonData.value("message").toString());
            }

            emit verifyCodeFinished(message.first, message.second, code);
        });

    _VerifyCodeSource->verifyCode(reqData);
}
