#include "base_source.h"

#include <QJsonDocument>
#include <QNetworkInformation>
#include <qjsonobject.h>
#include <QTimer>
#include <qthread.h>

#include "core/network/http/http_client.h"
#include "shared/model/response_data.h"
#include "shared/model/request_data.h"
#include "domain/pow_process.h"

BaseSource::BaseSource(QObject *parent)
    : QObject{parent}
    , _HTTPClient(new HTTPClient())
    , _PoWProcess(PoWProcess::getInstance())
{}

void BaseSource::request(RequestTypes type, const RequestData& reqData, int attempts)
{
    if (attempts == 0)
    {
        return;
    }

    _HTTPClient->sendRequest(
        type,
        reqData.jsonObj,
        reqData.url,
        reqData.headers,
        [this, type, reqData, attempts] (const QJsonObject& jsonObj, int code) {
            // Checking for PoW, solve PoW and retry request if it need
            // If all right call up callback
            QJsonObject message = jsonObj["message"].toObject();
            if (message.contains("challenge") && attempts > 1)
            {
                QString challenge = message["challenge"].toString();
                uint8_t difficulty = message["difficulty"].toInt();

                auto nonce = _PoWProcess->solve(challenge, difficulty);
                if (nonce)
                {
                    RequestData retryReqData = reqData;
                    retryReqData.headers["pow-challenge"] = challenge;
                    retryReqData.headers["pow-nonce"] = QString::number(*nonce);

                    request(type, retryReqData, attempts - 1);
                    return;
                }
            }

            ResponseData result;
            result.jsonData = jsonObj;
            result.code = code;

            reqData.callback(result);
        });
}
