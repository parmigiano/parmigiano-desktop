#ifndef CREATE_PROFILE_MODEL_H
#define CREATE_PROFILE_MODEL_H

#include "QString"
#include <qjsonobject.h>

struct CreateProfileRequest {
    QString name;
    QString username;
    QString email;
    QString registrationToken;

    QJsonObject toJson() const {
        QJsonObject obj;

        obj["name"] = name;
        obj["username"] = username;
        obj["email"] = email;
        obj["registration_token"] = registrationToken;

        return obj;
    }
};

#endif // CREATE_PROFILE_MODEL_H
