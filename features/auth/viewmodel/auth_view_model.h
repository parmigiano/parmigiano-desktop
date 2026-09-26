#ifndef AUTH_VIEW_MODEL_H
#define AUTH_VIEW_MODEL_H

#include <QObject>
#include <QtQml/qqmlregistration.h>

class AuthRepository;

class AuthViewModel : public QObject
{
    Q_OBJECT
    QML_ELEMENT
public:
    explicit AuthViewModel(QObject *parent = nullptr);
    ~AuthViewModel() = default;

private:
    AuthRepository* _AuthRepository;

    enum Status {
        Unknown = 0,
        Ok = 200,
        Accepted = 202,
        BadRequest = 400,
        NotFound = 404,
        InternalError = 500
    };

    void processLoginFinished(const QString& messageHeader,
                              const QString& messageBody,
                              int code) const;

    void processVerifyCodeFinished(const QString& messageHeader,
                                   const QString& messageBody,
                                   int code) const;

    void processCreateProfileFinished(const QString& messageHeader,
                                      const QString& messageBody,
                                      int code) const;

signals:
    void navigateToVerifyCode();
    void navigateToCreateProfile();
    void displayMessage(QString header, const QString& body);

public slots:
    void login(QString email);

    void verifyCode(QString email,
                    int code);

    void createProfile(QString name,
                       QString username,
                       QString email);
};

#endif // AUTH_VIEW_MODEL_H
