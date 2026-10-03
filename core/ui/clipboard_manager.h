#ifndef CLIPBOARD_MANAGER_H
#define CLIPBOARD_MANAGER_H

#include <QClipboard>
#include <QObject>
#include <QtQml/qqmlregistration.h>

class QClipboard;

class ClipboardManager : public QObject
{
    Q_OBJECT
    //QML_ELEMENT
public:
    explicit ClipboardManager(QObject *parent = nullptr);

    Q_INVOKABLE QString getClipboardData();

private:
    QClipboard* _Clipboard;
};

#endif // CLIPBOARD_MANAGER_H
