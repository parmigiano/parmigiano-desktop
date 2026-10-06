#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <QIcon>
#include <QTranslator>
#include <QQmlContext>
#include <QNetworkInformation>
#include <QQuickWindow>

#include "core/ui/ui_state_manager.h"
#include "core/ui/localization_manager.h"
#include "core/ui/pmg_types.h"

#include <QThread>

#ifdef Q_OS_WIN
#include <windows.h>
#include <dwmapi.h>
#pragma comment(lib, "dwmapi.lib")
#endif

void checkNetwork(UIStateManager* _UIStateManager, QNetworkInformation::Reachability reachability);
void applyWindowsFrame(QWindow *window);

int main(int argc, char *argv[])
{
    QCoreApplication::setOrganizationName("ParmigianoChat co");
    QCoreApplication::setOrganizationDomain("parmigianochat.ru");
    QCoreApplication::setApplicationName("ParmigianoChat");

    QGuiApplication app(argc, argv);
    app.setWindowIcon(QIcon(":/assets/logo.png"));

    QQmlApplicationEngine engine;

    // Core
    auto _UIStateManager = engine.singletonInstance<UIStateManager*>("ParmigianoDesktop.CoreUI", "UIStateManager");

    LocalizationManager* _LocalizationManager = LocalizationManager::getInstance();
    _LocalizationManager->setEngine(&engine);
    _LocalizationManager->changeLanguage(Pmg::Language::EN);

    qmlRegisterUncreatableMetaObject(
        Pmg::staticMetaObject,
        "Types",
        1, 0,
        "Pmg",
        "Error. Pmg is namespace"
    );

    QObject::connect(
        &engine,
        &QQmlApplicationEngine::objectCreationFailed,
        &app,
        []() { QCoreApplication::exit(-1); },
        Qt::QueuedConnection);
    engine.loadFromModule("ParmigianoDesktop", "Main");

    if (auto *window = qobject_cast<QQuickWindow *>(engine.rootObjects().value(0)))
    {
        applyWindowsFrame(window);
    }

    if (QNetworkInformation::loadBackendByFeatures(QNetworkInformation::Feature::Reachability))
    {
        auto netInfo = QNetworkInformation::instance();

        checkNetwork(_UIStateManager, netInfo->reachability());
        QObject::connect(netInfo, &QNetworkInformation::reachabilityChanged,
                         &app, [=] (QNetworkInformation::Reachability reachability) {
            checkNetwork(_UIStateManager, reachability);
        });
    }

    return app.exec();
}

void checkNetwork(UIStateManager* _UIStateManager, QNetworkInformation::Reachability reachability) {
    if (reachability == QNetworkInformation::Reachability::Online)
    {
        _UIStateManager->setNetworkStatus(true);
    }
    else
    {
        _UIStateManager->setNetworkStatus(false);
    }
}

static void applyWindowsFrame(QWindow *window)
{
#ifdef Q_OS_WIN
    const HWND hwnd = reinterpret_cast<HWND>(window->winId());

    const DWORD cornerPreference = 2;
    DwmSetWindowAttribute(hwnd, 33, &cornerPreference, sizeof(cornerPreference));

    const COLORREF borderColor = RGB(0x29, 0x2a, 0x2c);
    DwmSetWindowAttribute(hwnd, 34, &borderColor, sizeof(borderColor));
#else
    Q_UNUSED(window)
#endif
}
