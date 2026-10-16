#ifndef AUTH_VIEW_MODEL_H
#define AUTH_VIEW_MODEL_H

#include <QObject>
#include <QtQml/qqmlregistration.h>

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

    enum Status {
        Unknown = 0,
        Ok = 200,
        Accepted = 202,
        BadRequest = 400,
        NotFound = 404,
        InternalError = 500
    };

signals:
    void emailChanged();

    void navigateToVerifyCode();
    void navigateToCreateProfile();
    void displayMessage(QString header, const QString& body);

public slots:
    void processLoginFinished(const QString& messageHeader,
                              const QString& messageBody,
                              int code) const;

    void processVerifyCodeFinished(const QString& messageHeader,
                                   const QString& messageBody,
                                   const QString& step,
                                   int code) const;

    void processCreateProfileFinished(const QString& messageHeader,
                                      const QString& messageBody,
                                      int code) const;
};

#endif // AUTH_VIEW_MODEL_H
