#include "localization_manager.h"

#include <qDebug>
LocalizationManager LocalizationManager::_instancePtr;
QQmlEngine* LocalizationManager::_engine = nullptr;

LocalizationManager::LocalizationManager(QObject *parent)
    : QObject{parent}
{}

LocalizationManager *LocalizationManager::getInstance()
{
    return &_instancePtr;
}

void LocalizationManager::setEngine(QQmlEngine *engine)
{
    if(_engine == engine) return;
    _engine = engine;
}

void LocalizationManager::changeLocalizationTo(const QString& localizationPATH)
{
    qApp->removeTranslator(&_Translator);

    if(_Translator.load(localizationPATH))
    {
        qApp->installTranslator(&_Translator);
    }

    if(_engine)
    {
        _engine->retranslate();
    }
}

void LocalizationManager::changeLanguage(Pmg::Language lang)
{
    switch (lang)
    {
    case Pmg::Language::RU:
        changeLocalizationTo(":/translations/release/ParmigianoDesktop_ru_RU.qm");
        break;
    case Pmg::Language::EN:
        changeLocalizationTo(":/translations/release/ParmigianoDesktop_en_EN.qm");
        break;
    default:
        break;
    }
}
