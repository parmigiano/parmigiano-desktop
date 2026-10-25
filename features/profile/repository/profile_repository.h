#ifndef PROFILE_REPOSITORY_H
#define PROFILE_REPOSITORY_H

#include <QObject>

#include "shared/repository/base_repository.h"

class ProfileSource;
class UserStore;

class ProfileRepository : private BaseRepository
{
    Q_OBJECT
public:
    explicit ProfileRepository(QObject *parent = nullptr);

    void fillProfile();

private:
    ProfileSource* _GetProfileSource;
    UserStore* _UserStore;

signals:
    void fillProfileFinished(const QString& messageHeader,
                             const QString& messageBody,
                             int code);
};

#endif // PROFILE_REPOSITORY_H
