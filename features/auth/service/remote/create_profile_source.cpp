#include "create_profile_source.h"

#include <qjsonobject.h>

#include "core/network/http_client.h"

CreateProfileSource::CreateProfileSource(QObject *parent, const QString &link)
    : BaseSource(parent, link)
{}

void CreateProfileSource::createProfile(const RequestData& reqData)
{
    request(RequestTypes::POST, reqData);
}
