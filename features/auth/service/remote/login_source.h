#ifndef LOGIN_SOURCE_H
#define LOGIN_SOURCE_H

#include <qjsonobject.h>

//#include "features/auth/service/dto/login_model.h"
#include "core/network/http/remote/base_source.h"

// namespace HTTP
// {
//     class NetworkManager;
// }

// class HTTPClient;
// class PoWProcess;

class LoginSource : public BaseSource
{
    Q_OBJECT
public:
    explicit LoginSource(QObject *parent = nullptr, const QString& link = "");

    void login(const RequestData& reqData);

};

#endif // LOGIN_SOURCE_H
