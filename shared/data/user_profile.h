#ifndef USER_PROFILE_H
#define USER_PROFILE_H

#include "QString"

struct UserProfile
{
    QString id;
    QString name;
    QString username;
    QString email;

    bool operator==(const UserProfile&) const = default;
};

#endif // USER_PROFILE_H
