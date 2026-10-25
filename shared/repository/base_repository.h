#ifndef BASE_REPOSITORY_H
#define BASE_REPOSITORY_H

#include <QObject>

#include "shared/model/request_data.h"

class PoWProcess;
class TokenManager;

class BaseRepository : public QObject
{
    Q_OBJECT
public:
    explicit BaseRepository(QObject *parent = nullptr);
    virtual ~BaseRepository() = default;

protected:
    PoWProcess* _PoWProcess;
    TokenManager* _TokenManager;

    QMap<QString, QString> defineHeaders();
    std::pair<QString, QString> defineMessage(int code);

    template<typename T>
    RequestData buildRequest(const QString& url,
                             const std::optional<T>& model,
                             std::function<void(const ResponseData&, int, std::pair<QString, QString>)> func)
    {
        RequestData reqData;
        QMap<QString, QString> headers = defineHeaders();

        auto onResult = [this, func] (const ResponseData& result)
        {
            int code = result.code;
            std::pair<QString, QString> message = defineMessage(code);

            if (func) func(result, code, message);
        };

        reqData.headers = headers;
        reqData.callback = onResult;
        reqData.url = url;

        if constexpr (requires { model.value().toJson(); })
        {
            if (model)
            {
                reqData.jsonObj = model->toJson();
            }
        }

        return reqData;
    }

signals:
};

#endif // BASE_REPOSITORY_H
