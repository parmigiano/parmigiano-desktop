#ifndef USER_STORE_H
#define USER_STORE_H

#include <QObject>

#include "shared/data/user_profile.h"

class UserStore : public QObject
{
    Q_OBJECT
public:
    explicit UserStore(QObject *parent = nullptr);

    static UserStore* getInstance();
    UserProfile& profile();
    void setProfile(const UserProfile& profile);

private:
    UserProfile _profile;

signals:
    void profileChanged();
};

#endif // USER_STORE_H
