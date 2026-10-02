#include "auth_view_model.h"

#include <QMetaEnum>

#include "features/auth/repository/auth_repository.h"

AuthViewModel::AuthViewModel(QObject *parent)
    : _AuthRepository(new AuthRepository())
{
    connect(_AuthRepository, &AuthRepository::loginFinished,
           this, &AuthViewModel::processLoginFinished);

    connect(_AuthRepository, &AuthRepository::verifyCodeFinished,
            this, &AuthViewModel::processVerifyCodeFinished);

    connect(_AuthRepository, &AuthRepository::createProfileFinished,
            this, &AuthViewModel::processCreateProfileFinished);
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
                                              int code) const
{
    switch (code)
    {
    case AuthViewModel::Status::Ok:
        qDebug() << "navigate to messanger";
        break;
    case AuthViewModel::Status::Accepted:
        emit const_cast<AuthViewModel*>(this)->navigateToCreateProfile();
        break;
    case AuthViewModel::Status::BadRequest:
        qDebug() << "server: invalid confirmation code";
        break;
    default:
        emit const_cast<AuthViewModel*>(this)->displayMessage(messageHeader, messageBody);
        break;
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
