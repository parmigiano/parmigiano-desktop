#ifndef HTTP_CLIENT_H
#define HTTP_CLIENT_H

#include <QNetworkAccessManager>
#include <QObject>

enum class RequestTypes
{
    POST,
    GET,
    PATCH,
    PUT,
    DELET
};

class HTTPClient : public QObject
{
    Q_OBJECT
public:
    typedef std::function<void(const QJsonObject&, uint16_t)> handleFunc;

    explicit HTTPClient(QObject *parent = nullptr);
    ~HTTPClient() = default;

    // HTTPClient(const HTTPClient& other) = delete;
    // HTTPClient& operator=(const HTTPClient& other) = delete;

    // static HTTPClient* getInstance();

    void initialize();

    void sendRequest(RequestTypes type,
                     const QJsonObject& jsonData,
                     const QString& link,
                     const QMap<QString, QString>& headers = {},
                     const handleFunc& onResult = nullptr);

private:
    //static HTTPClient _instancePtr;
    QNetworkAccessManager* _networkManager;

    void addHeaders(QNetworkRequest* request,
                    const QMap<QString, QString>& headers);

    void processResponse(QNetworkReply* reply,
                         const handleFunc& callbackFunc);
};

#endif // HTTP_CLIENT_H
