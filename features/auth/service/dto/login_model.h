#ifndef LOGIN_MODEL_H
#define LOGIN_MODEL_H

#include "QString"
#include <qjsonobject.h>

struct LoginRequest {
    QString email;

    QJsonObject toJson() const {
        QJsonObject obj;

        obj["email"] = email;

        return obj;
    }
};

#endif // LOGIN_MODEL_H
