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

private:
    bool _networkStatus = true;
    Pmg::Language _localization = Pmg::Language::RU;

signals:
    void networkStatusChanged(bool status);
    void localizationChanged(Pmg::Language lang);

public slots:
    void setNetworkStatus(bool status);
    void setLocalization(Pmg::Language lang);

    bool getNetworkStatus();
    Pmg::Language getLocalization();
};

#endif // UI_STATE_MANAGER_H
