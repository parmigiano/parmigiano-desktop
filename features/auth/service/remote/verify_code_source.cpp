#include "verify_code_source.h"

#include <QJsonObject>
#include <QUrlQuery>

#include "core/network/http_client.h"

VerifyCodeSource::VerifyCodeSource(QObject *parent, const QString& link)
    : BaseSource(parent, link)
{}

void VerifyCodeSource::verifyCode(const RequestData& reqData)
{
    request(RequestTypes::POST, reqData);
}
