#include "profile_source.h"

#include "core/network/http/http_client.h"

ProfileSource::ProfileSource(QObject *parent)
    : BaseSource(parent)
{}

void ProfileSource::getProfileData(const RequestData &reqData)
{
    request(RequestTypes::GET, reqData);
}
