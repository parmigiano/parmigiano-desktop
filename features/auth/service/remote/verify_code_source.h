#ifndef VERIFY_CODE_SOURCE_H
#define VERIFY_CODE_SOURCE_H

#include "core/network/http/remote/base_source.h"

class VerifyCodeSource : public BaseSource
{
    Q_OBJECT
public:
    explicit VerifyCodeSource(QObject *parent = nullptr, const QString& link = "");

    void verifyCode(const RequestData& reqData);

};

#endif // VERIFY_CODE_SOURCE_H
