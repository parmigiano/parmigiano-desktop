#include "token_manager.h"

#include <QEventLoop>
#include <qt6keychain/keychain.h>
#include <QCoreApplication>
#include <QDebug>

TokenManager::TokenManager(QObject *parent)
    : QObject{parent}
{}

TokenManager *TokenManager::getInstance()
{
    static TokenManager tokenManager;
    return &tokenManager;
}

void TokenManager::save(const QString& name, const QString &token)
{
    QKeychain::WritePasswordJob job(QCoreApplication::applicationName());
    job.setAutoDelete(false);
    job.setKey(name);
    job.setTextData(token);

    QEventLoop loop;
    QObject::connect(&job, &QKeychain::Job::finished, &loop, &QEventLoop::quit);
    job.start();
    loop.exec();
}

QString TokenManager::token(const QString& name)
{
    QKeychain::ReadPasswordJob job(QCoreApplication::applicationName());
    job.setAutoDelete(false);
    job.setKey(name);

    QEventLoop loop;
    QObject::connect(&job, &QKeychain::Job::finished, &loop, &QEventLoop::quit);
    job.start();
    loop.exec();

    return job.textData();
}
