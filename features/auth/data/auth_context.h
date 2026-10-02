#ifndef AUTH_CONTEXT_H
#define AUTH_CONTEXT_H

#include <QObject>
#include <qqmlengine.h>
#include <QtQml/qqmlregistration.h>

class AuthContext : public QObject
{
    Q_OBJECT
    QML_ELEMENT
    QML_SINGLETON
    Q_PROPERTY(QString email READ getEmail WRITE setEmail NOTIFY emailChanged)
public:
    //explicit AuthContext(QObject *parent = nullptr);
    static QObject* singletonProvider(QQmlEngine *engine, QJSEngine *scriptEngine);

    explicit AuthContext(QObject *parent = nullptr);
    ~AuthContext() = default;

    AuthContext(const AuthContext& other) = delete;
    AuthContext& operator=(const AuthContext& other) = delete;

    static AuthContext* getInstance();

    Q_INVOKABLE QString getEmail();
    Q_INVOKABLE void setEmail(const QString& email);

private:
    static AuthContext _instancePtr;
    QString _email = "";

signals:
    void emailChanged(const QString& email);

};

#endif // AUTH_CONTEXT_H
