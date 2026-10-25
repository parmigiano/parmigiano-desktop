#ifndef AUTH_SOURCE_H
#define AUTH_SOURCE_H

#include "shared/service/remote/base_source.h"

class AuthSource : public BaseSource
{
    Q_OBJECT
public:
    explicit AuthSource(QObject *parent = nullptr);

    void createProfile(const RequestData& reqData);
    void login(const RequestData& reqData);
    void verifyCode(const RequestData& reqData);

signals:
};

#endif // AUTH_SOURCE_H
