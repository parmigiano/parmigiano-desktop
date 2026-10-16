#include "auth_view_model.h"

#include <QMetaEnum>

#include "features/auth/repository/auth_repository.h"

AuthViewModel::AuthViewModel(QObject *parent)
    : _AuthRepository(new AuthRepository(this))
{
    connect(_AuthRepository, &AuthRepository::loginFinished,
           this, &AuthViewModel::processLoginFinished);

    connect(_AuthRepository, &AuthRepository::verifyCodeFinished,
            this, &AuthViewModel::processVerifyCodeFinished);

    connect(_AuthRepository, &AuthRepository::createProfileFinished,
            this, &AuthViewModel::processCreateProfileFinished);
}

QString AuthViewModel::email()
{
    return _email;
}

void AuthViewModel::setEmail(QString email)
{
    if (_email != email)
    {
        _email = email;
    }
}

void AuthViewModel::processLoginFinished(const QString& messageHeader,
                                         const QString& messageBody,
                                         int code) const
{
    switch (code)
    {
    case AuthViewModel::Status::Ok:
        emit const_cast<AuthViewModel*>(this)->navigateToVerifyCode();
        break;
    default:
        emit const_cast<AuthViewModel*>(this)->displayMessage(messageHeader, messageBody);
        break;
    }
}

void AuthViewModel::processVerifyCodeFinished(const QString& messageHeader,
                                              const QString& messageBody,
                                              const QString& step,
                                              int code) const
{
    // switch (code)
    // {
    // case AuthViewModel::Status::BadRequest:
    //     qDebug() << "server: invalid confirmation code";
    //     return;
    // default:
    //     emit const_cast<AuthViewModel*>(this)->displayMessage(messageHeader, messageBody);
    //     return;
    // }

    if (step == "registration")
    {
        emit const_cast<AuthViewModel*>(this)->navigateToCreateProfile();
    }
}

void AuthViewModel::processCreateProfileFinished(const QString& messageHeader,
                                                 const QString& messageBody,
                                                 int code) const
{
    switch (code)
    {
    case AuthViewModel::Status::Ok:
        qDebug() << "navigate to messanger";
        break;
    default:
        emit const_cast<AuthViewModel*>(this)->displayMessage(messageHeader, messageBody);
        break;
    }
}

void AuthViewModel::login(QString email)
{
    _AuthRepository->login(email);
}

void AuthViewModel::verifyCode(QString email,
                               int code)
{
    _AuthRepository->verifyCode(email, code);
}

void AuthViewModel::createProfile(QString name,
                                  QString username,
                                  QString email)
{
    _AuthRepository->createProfile(name, username, email);
}
