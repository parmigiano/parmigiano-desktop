#ifndef CREATEA_PROFILE_SOURCE_H
#define CREATEA_PROFILE_SOURCE_H

#include "features/auth/service/dto/create_profile_model.h"
#include "base_source.h"

class CreateProfileSource : public BaseSource
{
    Q_OBJECT
public:
    explicit CreateProfileSource(QObject *parent = nullptr, const QString& link = "");

    void createProfile(const RequestData& reqData);

};

#endif // CREATEA_PROFILE_SOURCE_H
