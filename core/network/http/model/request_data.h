#ifndef REQUEST_DATA_H
#define REQUEST_DATA_H

#include <qjsonobject.h>
#include "core/network/http/model/response_data.h"

struct RequestData
{
    //std::function<void(const QJsonObject&, uint16_t)> callback;
    std::function<void(const ResponseData&)> callback;
    QJsonObject jsonObj;
    QMap<QString, QString> headers;
};

#endif // REQUEST_DATA_H
