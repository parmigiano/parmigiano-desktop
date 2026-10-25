#include "auth_view_model.h"

#include <QMetaEnum>

#include "features/auth/repository/auth_repository.h"
#include "core/network/http/http_response_status.h"

AuthViewModel::AuthViewModel(QObject *parent)
    : _AuthRepository(new AuthRepository(this))
{
    connect(_AuthRepository, &AuthRepository::loginFinished,
           this, &AuthViewModel::processLoginFinished);

    connect(_AuthRepository, &AuthRepository::createProfileFinished,
            this, &AuthViewModel::processCreateProfileFinished);

    connect(_AuthRepository, &AuthRepository::verifyCodeFinished,
            this, &AuthViewModel::processVerifyCodeFinished);
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

void AuthViewModel::processLoginFinished(RequestResult result)
{
    switch (result.code)
    {
    case HTTPStatus::Ok:
        emit const_cast<AuthViewModel*>(this)->navigateToVerifyCode();
        break;
    default:
        emit const_cast<AuthViewModel*>(this)->displayMessage(result.messageHeader, result.messageBody);
        break;
    }
}

void AuthViewModel::processCreateProfileFinished(RequestResult result)
{
    switch (result.code)
    {
    case HTTPStatus::Ok:
        qDebug() << "navigate to messanger";
        break;
    default:
        emit const_cast<AuthViewModel*>(this)->displayMessage(result.messageHeader, result.messageBody);
        break;
    }
}

void AuthViewModel::processVerifyCodeFinished(RequestResult result, const QString& step)
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
