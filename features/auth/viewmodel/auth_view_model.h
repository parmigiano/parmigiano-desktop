#ifndef AUTH_VIEW_MODEL_H
#define AUTH_VIEW_MODEL_H

#include <QObject>
#include <QtQml/qqmlregistration.h>

#include "shared/model/request_result.h"

class AuthRepository;

class AuthViewModel : public QObject
{
    Q_OBJECT
    QML_ELEMENT
    Q_PROPERTY(QString email READ email WRITE setEmail NOTIFY emailChanged)
public:
    explicit AuthViewModel(QObject *parent = nullptr);
    ~AuthViewModel() = default;

    Q_INVOKABLE QString email();
    Q_INVOKABLE void setEmail(QString email);

    Q_INVOKABLE void login(QString email);
    Q_INVOKABLE void verifyCode(QString email, int code);
    Q_INVOKABLE void createProfile(QString name, QString username, QString email);

private:
    AuthRepository* _AuthRepository;
    QString _email;

signals:
    void emailChanged();

    void navigateToVerifyCode();
    void navigateToCreateProfile();
    void displayMessage(QString header, const QString& body);

public slots:
    void processLoginFinished(RequestResult result);
    void processCreateProfileFinished(RequestResult result);
    void processVerifyCodeFinished(RequestResult result, const QString& step);
};

#endif // AUTH_VIEW_MODEL_H
