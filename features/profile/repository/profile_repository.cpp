#include "profile_repository.h"

#include "features/profile/service/remote/profile_source.h"

#include "shared/model/user_profile.h"
#include "shared/data/user_store.h"

#include "core/config/app_config.h"

ProfileRepository::ProfileRepository(QObject *parent)
    : _GetProfileSource(new ProfileSource(this))
    , _UserStore(new UserStore(this))
{}

void ProfileRepository::fillProfile()
{
    RequestData reqData = buildRequest(
        AppConfig::getProfileEndPoint(),
        std::optional<QString>{},
        [this] (const ResponseData& result, int code,
               std::pair<QString, QString> message)
        {
            const QJsonObject response = result.jsonData.value("message").toObject();

            UserProfile user
            {
                response.value("id").toInt(),
                response.value("created_at").toInt(),
                response.value("user_uid").toInt(),

                response.value("avatar").toString(),
                response.value("name").toString(),
                response.value("username").toString(),
                response.value("username_visible").toBool(),

                response.value("email").toString(),
                response.value("email_visible").toBool(),
                response.value("email_confirm").toBool(),

                response.value("phone").toString(),
                response.value("phone_visible").toBool(),

                response.value("overview").toString()
            };

            _UserStore->setProfile(user);

            emit fillProfileFinished(message.first, message.second, code);
        });

    _GetProfileSource->getProfileData(reqData);
}
