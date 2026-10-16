#include "user_store.h"

UserStore::UserStore(QObject *parent)
    : QObject{parent}
{}

UserStore *UserStore::getInstance()
{
    static UserStore userStore;
    return &userStore;
}

UserProfile &UserStore::profile()
{
    return _profile;
}

void UserStore::setProfile(const UserProfile &profile)
{
    if (_profile != profile)
    {
        _profile = profile;
        emit profileChanged();
    }
}
