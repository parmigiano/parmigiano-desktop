#ifndef TOKEN_MANAGER_H
#define TOKEN_MANAGER_H

#include <QObject>

class TokenManager : public QObject
{
    Q_OBJECT
public:
    explicit TokenManager(QObject *parent = nullptr);
    ~TokenManager() = default;

    TokenManager(const TokenManager& other) = delete;
    TokenManager& operator=(const TokenManager& other) = delete;

    static TokenManager* getInstance();

    void save(const QString& name, const QString& token);
    QString token(const QString& name);

signals:
};

#endif // TOKEN_MANAGER_H
