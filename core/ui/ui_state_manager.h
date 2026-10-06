#ifndef UI_STATE_MANAGER_H
#define UI_STATE_MANAGER_H

#include <QObject>
#include <QQmlEngine>
#include <QJSEngine>
#include <QtQml/qqmlregistration.h>

#include "pmg_types.h"

class UIStateManager : public QObject
{
    Q_OBJECT
    QML_ELEMENT
    QML_SINGLETON
    Q_PROPERTY(bool networkStatus READ getNetworkStatus WRITE setNetworkStatus NOTIFY networkStatusChanged)
    Q_PROPERTY(Pmg::Language localization READ getLocalization WRITE setLocalization NOTIFY localizationChanged)
public:
    explicit UIStateManager(QObject *parent = nullptr);
    ~UIStateManager() = default;

    UIStateManager(const UIStateManager& other) = delete;
    UIStateManager& operator=(const UIStateManager& other) = delete;

    Q_INVOKABLE void setNetworkStatus(bool status);
    Q_INVOKABLE void setLocalization(Pmg::Language lang);

    Q_INVOKABLE bool getNetworkStatus();
    Q_INVOKABLE Pmg::Language getLocalization();

private:
    bool _networkStatus = true;
    Pmg::Language _localization = Pmg::Language::EN;

signals:
    void networkStatusChanged(bool status);
    void localizationChanged(Pmg::Language lang);

};

#endif // UI_STATE_MANAGER_H
