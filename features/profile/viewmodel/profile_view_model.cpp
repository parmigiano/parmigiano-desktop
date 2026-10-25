#include "profile_view_model.h"

#include "features/profile/repository/profile_repository.h"

ProfileViewModel::ProfileViewModel(QObject *parent)
    : QObject{parent}
    , _ProfileRepository(new ProfileRepository(this))
{}

void ProfileViewModel::fillProfile()
{
    _ProfileRepository->fillProfile();
}
