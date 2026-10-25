#include "base_repository.h"

#include "domain/pow_process.h"
#include "domain/token_manager.h"

BaseRepository::BaseRepository(QObject *parent)
    : QObject{parent}
    , _PoWProcess(PoWProcess::getInstance())
    , _TokenManager(TokenManager::getInstance())
{}

QMap<QString, QString> BaseRepository::defineHeaders()
{
    QMap<QString, QString> headers;
    auto nonce = _PoWProcess->nonce();
    QString challenge = _PoWProcess->challenge();
    QString sessionToken = _TokenManager->token("session_token");

    if (!challenge.isEmpty())
    {
        headers["pow-challenge"] = challenge;
    }

    if (nonce)
    {
        QString value = QString::number(*nonce);
        headers["pow-nonce"] = value;
    }

    if (!sessionToken.isEmpty())
    {
        headers["Authorization"] = "Bearer " + sessionToken;
    }

    headers["Accept-Language"] = "en";

    return headers;
}

std::pair<QString, QString> BaseRepository::defineMessage(int code)
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
