#ifndef BASE_SOURCE_H
#define BASE_SOURCE_H

#include <QObject>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <qjsonobject.h>

enum class RequestTypes;
struct ResponseData;
struct RequestData;
class HTTPClient;
class PoWProcess;

class BaseSource : public QObject
{
    Q_OBJECT
public:
    explicit BaseSource(QObject *parent = nullptr, const QString& link = "");

protected:
    QList<QVariantMap> _data;
    QNetworkAccessManager _networkManager;
    QNetworkReply* _reply = nullptr;
    QJsonObject currentJSON;

    QString _link;

    int _retryAttempts = 2;
    int _currentAttempt = 0;

    void request(RequestTypes type, const RequestData& reqData);

private:
    HTTPClient* _HTTPClient;
    PoWProcess* _PoWProcess;
};

#endif // BASE_SOURCE_H
