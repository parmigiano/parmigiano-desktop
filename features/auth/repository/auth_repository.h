#ifndef AUTH_REPOSITORY_H
#define AUTH_REPOSITORY_H

#include <QObject>
#include <QString>

class LoginSource;
class CreateProfileSource;
class VerifyCodeSource;
class PoWProcess;

class AuthRepository : public QObject
{
    Q_OBJECT
public:

    explicit AuthRepository(QObject *parent = nullptr);
    virtual ~AuthRepository() = default;

private:
    LoginSource* _LoginSource;
    CreateProfileSource* _CreateProfileSource;
    VerifyCodeSource* _VerifyCodeSource;
    PoWProcess* _PoWProcess;

    QMap<QString, QString> defineHeaders();
    std::pair<QString, QString> defineMessage(int code);

public:
    void login(const QString& email);

    void createProfile(const QString& name,
                       const QString& username,
                       const QString& email);

    void verifyCode(const QString& email,
                    int code);

signals:
    void loginFinished(const QString& messageHeader,
                       const QString& messageBody,
                       int code);

    void createProfileFinished(const QString& messageHeader,
                               const QString& messageBody,
                               int code);

    void verifyCodeFinished(const QString& messageHeader,
                            const QString& messageBody,
                            int code);

};

#endif // AUTH_REPOSITORY_H
