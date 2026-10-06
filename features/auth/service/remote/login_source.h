#ifndef LOGIN_SOURCE_H
#define LOGIN_SOURCE_H

#include "core/network/http/remote/base_source.h"

class LoginSource : public BaseSource
{
    Q_OBJECT
public:
    explicit LoginSource(QObject *parent = nullptr, const QString& link = "");

    void login(const RequestData& reqData);

};

#endif // LOGIN_SOURCE_H
