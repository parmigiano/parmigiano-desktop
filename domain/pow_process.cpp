#include "pow_process.h"

#include <qjsonobject.h>
#include <QCryptographicHash>
#include <QByteArray>

PoWProcess PoWProcess::_instancePtr;

PoWProcess::PoWProcess(QObject *parent) {}

PoWProcess *PoWProcess::getInstance()
{
    return &_instancePtr;
}

std::optional<int> PoWProcess::solve(const QString &challenge, uint16_t difficulty)
{
    for(uint16_t i = 0; i < _attemptsToSolve; ++i)
    {
        QString str = challenge + QString::number(i);

        if(isSolved(str, difficulty))
        {
            setNonce(i);
            setChallenge(challenge);
            return i;
        }
    }

    return std::nullopt;
}

// bool PoWProcess::check(QJsonObject jsonObj)
// {
//     if (jsonObj["message"].toObject().contains("challenge"))
//     {
//         return true;
//     }

//     return false;
// }

std::optional<int> PoWProcess::nonce()
{
    return _nonce;
}

void PoWProcess::setNonce(int nonce)
{
    if (_nonce != nonce) _nonce = nonce;
}

QString PoWProcess::challenge()
{
    return _challenge;
}

void PoWProcess::setChallenge(const QString& challenge)
{
    if (_challenge != challenge) _challenge = challenge;
}

bool PoWProcess::isSolved(const QString& str, int difficulty)
{
    QByteArray hash = toSHA256(str);

    for (int i = 0; i < difficulty; ++i) {
        int byteIdx = i / 2;
        unsigned char val = static_cast<unsigned char>(hash[byteIdx]);

        if (i % 2 == 0)
        {
            // first half
            if ((val >> 4) != 0) return false;
        }
        else
        {
            // second half
            if ((val & 0x0F) != 0) return false;
        }
    }

    return true;
}

QByteArray PoWProcess::toSHA256(const QString &str)
{
    QByteArray data = str.toUtf8();
    QByteArray hashData = QCryptographicHash::hash(data, QCryptographicHash::Sha256);

    return hashData;
}

