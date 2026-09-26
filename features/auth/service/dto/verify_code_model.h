#ifndef VERIFY_CODE_MODEL_H
#define VERIFY_CODE_MODEL_H

#include "QString"
#include <qjsonobject.h>

struct VerifyCodeRequest {
    QString email;
    int code;

    QJsonObject toJson() const {
        QJsonObject obj;

        obj["email"] = email;
        obj["code"] = code;

        return obj;
    }
};

#endif // VERIFY_CODE_MODEL_H
