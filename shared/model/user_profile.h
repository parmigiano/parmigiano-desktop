#ifndef USER_PROFILE_H
#define USER_PROFILE_H

#include "QString"

struct UserProfile
{
    int id = 0;
    int createdAt = 0;
    int userUid = 0;

    QString avatar;
    QString name;
    QString username;
    bool usernameVisible = true;

    QString email;
    bool emailVisible = true;
    bool emailConfirm = true;

    QString phone;
    bool phoneVisible = true;

    QString overview;

    bool operator==(const UserProfile&) const = default;
};

#endif // USER_PROFILE_H
