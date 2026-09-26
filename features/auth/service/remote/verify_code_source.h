#ifndef VERIFY_CODE_SOURCE_H
#define VERIFY_CODE_SOURCE_H

#include "features/auth/service/dto/verify_code_model.h"
#include "base_source.h"

class VerifyCodeSource : public BaseSource
{
    Q_OBJECT
public:
    explicit VerifyCodeSource(QObject *parent = nullptr, const QString& link = "");

    void verifyCode(const RequestData& reqData);

};

#endif // VERIFY_CODE_SOURCE_H
