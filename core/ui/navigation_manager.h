#ifndef NAVIGATION_MANAGER_H
#define NAVIGATION_MANAGER_H

#include <QObject>
#include <QtQml/qqmlregistration.h>

class NavigationManager : public QObject
{
    Q_OBJECT
    QML_ELEMENT
    QML_SINGLETON
public:
    explicit NavigationManager(QObject *parent = nullptr);
    virtual ~NavigationManager() = default;

signals:
    void navigateTo(QString pageName);
    void navigateBack(/*QString windowName*/);

public slots:
    void goTo(QString pageName);
    void goBack(/*QString windowName*/);
};

#endif // NAVIGATION_MANAGER_H
