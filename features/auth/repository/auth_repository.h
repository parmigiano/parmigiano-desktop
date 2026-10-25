#ifndef AUTH_REPOSITORY_H
#define AUTH_REPOSITORY_H

#include <QObject>
#include <QString>

#include "shared/repository/base_repository.h"
#include "shared/model/request_result.h"

class AuthSource;
class PoWProcess;

class AuthRepository : public BaseRepository
{
    Q_OBJECT
public:
    explicit AuthRepository(QObject *parent = nullptr);
    virtual ~AuthRepository() = default;

private:
    AuthSource* _AuthSource;

public:
    void login(const QString& email);

    void createProfile(const QString& name,
                       const QString& username,
                       const QString& email);

    void verifyCode(const QString& email,
                    int code);

signals:
    void loginFinished(RequestResult result);
    void createProfileFinished(RequestResult result);
    void verifyCodeFinished(RequestResult result, const QString& step);

};

#endif // AUTH_REPOSITORY_H
