#include "http_client.h"

#include <qjsondocument.h>
#include <qjsonobject.h>
#include <qnetworkreply.h>

HTTPClient::HTTPClient(QObject *parent)
    : _networkManager(new QNetworkAccessManager(this))
{}

void HTTPClient::sendRequest(RequestTypes type,
                             const QJsonObject &jsonData,
                             const QString &link,
                             const QMap<QString, QString> &headers,
                             const handleFunc &onResult)
{
    QJsonDocument jsonDoc(jsonData);
    QByteArray jsonArray = jsonDoc.toJson(QJsonDocument::Compact);

    QNetworkRequest request(QUrl(link.toUtf8()));
    QNetworkReply* reply;

    addHeaders(&request, headers);

#ifndef NDEBUG
    auto typeName = [type] () {
        switch (type)
        {
        case RequestTypes::POST: return "POST";
        case RequestTypes::GET: return "GET";
        case RequestTypes::PATCH: return "PATCH";
        case RequestTypes::DELET: return "DELETE";
        default: return "unknw";
        }
    };

    qDebug().nospace() << "Link: " << link << "\n"
                       << "Request type: " << typeName() << "\n"
                       << "JSON: " << jsonArray.data() << "\n"
                       << "Headers ->";

    QList<QByteArray> headerList = request.rawHeaderList();
    foreach (const QByteArray &head, headerList)
    {
        qDebug() << head << ":" << request.rawHeader(head);
    }

    qDebug() << "";
#endif

    switch (type)
    {
    case RequestTypes::POST:
        reply = _networkManager->post(request, jsonArray);
        break;
    case RequestTypes::GET:
        break;
    case RequestTypes::PATCH:
        break;
    case RequestTypes::DELET:
        break;
    default:
        break;
    }

    connect(reply, &QNetworkReply::finished, this, [this, onResult, reply] {
        processResponse(reply, onResult);
    });
}

void HTTPClient::addHeaders(QNetworkRequest *request,
                            const QMap<QString, QString> &headers)
{
    request->setHeader(QNetworkRequest::ContentTypeHeader, "application/json");

    if (!headers.isEmpty())
    {
        for(auto [name, value] : headers.asKeyValueRange())
        {
            request->setRawHeader(name.toUtf8(), value.toUtf8());
        }
    }
}

void HTTPClient::processResponse(QNetworkReply *reply,
                                 const handleFunc &callbackFunc)
{
    if (!reply || !callbackFunc) return;

    QByteArray jsonArray = reply->readAll();
    QJsonDocument jsonDoc = QJsonDocument::fromJson(jsonArray);
    QJsonObject jsonData = jsonDoc.object();

    int code = reply->attribute(QNetworkRequest::HttpStatusCodeAttribute).toInt();

#ifndef NDEBUG
    qDebug().nospace() << "JSON: " << jsonArray.data() << "\n"
                       << "Code: " << code << "\n"
                       << "";
#endif

    callbackFunc(jsonData, code);

    reply->deleteLater();
    reply = nullptr;
}
