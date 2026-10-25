#include "auth_source.h"

#include "core/network/http/http_client.h"

AuthSource::AuthSource(QObject *parent)
    : BaseSource(parent)
{}

void AuthSource::createProfile(const RequestData &reqData)
{
    request(RequestTypes::POST, reqData);
}

void AuthSource::login(const RequestData &reqData)
{
    request(RequestTypes::POST, reqData);
}

void AuthSource::verifyCode(const RequestData &reqData)
{
    request(RequestTypes::POST, reqData);
}
