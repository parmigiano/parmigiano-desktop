#include "ui_state_manager.h"

//UIStateManager UIStateManager::instancePtr_;

UIStateManager::UIStateManager(QObject *parent)
    : QObject{parent}
{}

// UIStateManager *UIStateManager::getInstance()
// {
//     return &instancePtr_;
// }

bool UIStateManager::getNetworkStatus()
{
    return _networkStatus;
}

Pmg::Language UIStateManager::getLocalization()
{
    return _localization;
}

void UIStateManager::setNetworkStatus(bool status)
{
    if (_networkStatus != status)
    {
        _networkStatus = status;
        emit networkStatusChanged(_networkStatus);
    }
}

void UIStateManager::setLocalization(Pmg::Language lang)
{
    if (_localization != lang)
    {
        _networkStatus = lang;
        _localization = lang;
        emit localizationChanged(_localization);
    }
}
