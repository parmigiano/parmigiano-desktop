#ifndef LOCALIZATION_MANAGER_H
#define LOCALIZATION_MANAGER_H

#include <QtGui/QGuiApplication>
#include <QTranslator>
#include <QObject>
#include <QQmlApplicationEngine>

#include "pmg_types.h"

class UIStateManager;

class LocalizationManager : public QObject
{
    Q_OBJECT
    QML_ELEMENT
    QML_SINGLETON
public:
    explicit LocalizationManager(QObject *parent = nullptr);
    virtual ~LocalizationManager() = default;

    LocalizationManager(const LocalizationManager& other) = delete;
    LocalizationManager& operator=(const LocalizationManager& other) = delete;

    static LocalizationManager* getInstance();
    void setEngine(QQmlEngine *engine);

    Q_INVOKABLE void changeLanguage(Pmg::Language lang);

private:
    QTranslator _Translator;

    static LocalizationManager _instancePtr;
    static QQmlEngine* _engine;

    void changeLocalizationTo(const QString& localizationPATH);
};

#endif // LOCALIZATION_MANAGER_H
