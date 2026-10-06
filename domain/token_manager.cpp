#include "token_manager.h"

#include <QEventLoop>
#include <qt6keychain/keychain.h>
#include <QCoreApplication>
#include <QDebug>

TokenManager TokenManager::_instancePtr;

TokenManager::TokenManager(QObject *parent)
    : QObject{parent}
{}

TokenManager *TokenManager::getInstance()
{
    return &_instancePtr;
}

void TokenManager::save(const QString &token)
{
    QKeychain::WritePasswordJob job(QCoreApplication::applicationName());
    job.setAutoDelete(false);
    job.setKey(_key);
    job.setTextData(token);

    QEventLoop loop;
    QObject::connect(&job, &QKeychain::Job::finished, &loop, &QEventLoop::quit);
    job.start();
    loop.exec();
}

QString TokenManager::load()
{
    QKeychain::ReadPasswordJob job(QCoreApplication::applicationName());
    job.setAutoDelete(false);
    job.setKey(_key);

    QEventLoop loop;
    QObject::connect(&job, &QKeychain::Job::finished, &loop, &QEventLoop::quit);
    job.start();
    loop.exec();

    return job.textData();
}
