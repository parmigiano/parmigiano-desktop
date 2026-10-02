#include "clipboard_manager.h"

ClipboardManager::ClipboardManager(QObject *parent)
    : QObject{parent}
{}

QString ClipboardManager::getClipboardData()
{
    return _Clipboard->text();
}
