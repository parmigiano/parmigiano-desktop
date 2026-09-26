#include "navigation_manager.h"

#include <QDebug>

NavigationManager::NavigationManager(QObject *parent)
    : QObject{parent}
{}

void NavigationManager::goTo(QString pageId)
{
    qDebug() << pageId;
    emit navigateTo(pageId);
}

void NavigationManager::goBack(/*QString windowName*/)
{
    emit navigateBack(/*windowName*/);
}
