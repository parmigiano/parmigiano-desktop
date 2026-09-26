#include "base_source.h"

#include <QJsonDocument>
#include <QNetworkInformation>
#include <qjsonobject.h>
#include <QTimer>
#include <qthread.h>

#include "core/network/http_client.h"
#include "features/auth/model/response_data.h"
#include "features/auth/model/request_data.h"
#include "domain/pow_process.h"

BaseSource::BaseSource(QObject *parent, const QString &link)
    : QObject{parent}, _link(link)
    , _HTTPClient(new HTTPClient())
    , _PoWProcess(PoWProcess::getInstance())
{}

void BaseSource::request(RequestTypes type, const RequestData& reqData)
{
    if (_currentAttempt >= _retryAttempts)
    {
        return;
    }

    _currentAttempt++;

    _HTTPClient->sendRequest(
        type,
        reqData.jsonObj,
        _link,
        reqData.headers,
        [this, type, reqData] (const QJsonObject& jsonObj, int code) {
            // Checking for PoW, solve PoW and retry request if it need
            // If all right call up callback
            if (jsonObj["message"].toObject().contains("challenge"))
            {
                QString challenge = jsonObj["message"].toObject()["challenge"].toString();
                uint8_t difficulty = jsonObj["message"].toObject()["difficulty"].toInt();

                _PoWProcess->solve(challenge, difficulty);

                auto nonce = _PoWProcess->getNonce();
                QString value = QString::number(*nonce);

                QMap<QString, QString> headers = reqData.headers;
                headers["pow-challenge"] = challenge;
                headers["pow-nonce"] = value;

                RequestData RetryReqData;
                RetryReqData.jsonObj = reqData.jsonObj;
                RetryReqData.callback = reqData.callback;
                RetryReqData.headers = headers;

                request(type, RetryReqData);
            }
            else
            {
                ResponseData result;
                result.jsonData = jsonObj;
                result.code = code;

                _currentAttempt = 0;
                reqData.callback(result);
            }
        });
}
