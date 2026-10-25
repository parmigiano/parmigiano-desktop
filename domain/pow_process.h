#ifndef POW_PROCESS_H
#define POW_PROCESS_H

#include <QString>
#include <QObject>
#include <optional>

class PoWProcess : public QObject
{
    Q_OBJECT
public:
    explicit PoWProcess(QObject *parent = nullptr);
    ~PoWProcess() = default;

    PoWProcess(const PoWProcess& other) = delete;
    PoWProcess& operator=(const PoWProcess& other) = delete;

    static PoWProcess* getInstance();

    std::optional<int> solve(const QString& challenge, uint16_t difficulty);

    // Nonce
    std::optional<int> nonce();
    void setNonce(int nonce);

    // Challenge
    QString challenge();
    void setChallenge(const QString& challenge);

private:
    // static PoWProcess _instancePtr;
    std::optional<int> _nonce;
    int _attemptsToSolve = 10000;
    QString _challenge = "";

    bool isSolved(const QString& str, int difficulty);
    QByteArray toSHA256(const QString& str);
};

#endif // POW_PROCESS_H
