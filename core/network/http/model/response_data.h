#ifndef RESPONSE_DATA_H
#define RESPONSE_DATA_H

#include <qcontainerfwd.h>
#include <qjsonobject.h>

struct ResponseData
{
    QJsonObject jsonData;
    int code;
};

#endif // RESPONSE_DATA_H
