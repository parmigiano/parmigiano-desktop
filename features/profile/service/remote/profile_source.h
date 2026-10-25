#ifndef PROFILE_SOURCE_H
#define PROFILE_SOURCE_H

#include "shared/service/remote/base_source.h"

class ProfileSource : public BaseSource
{
    Q_OBJECT
public:
    explicit ProfileSource(QObject *parent = nullptr);

    void getProfileData(const RequestData& reqData);

signals:
};

#endif // PROFILE_SOURCE_H
