#include "login_source.h"

#include <QJsonObject>
#include <QUrlQuery>

#include "core/network/http_client.h"

LoginSource::LoginSource(QObject *parent, const QString &link)
    : BaseSource(parent, link)
{}

void LoginSource::login(const RequestData& reqData)
{
    request(RequestTypes::POST, reqData);
}
