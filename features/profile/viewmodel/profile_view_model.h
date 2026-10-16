#ifndef PROFILE_VIEW_MODEL_H
#define PROFILE_VIEW_MODEL_H

#include <QObject>

class ProfileViewModel : public QObject
{
    Q_OBJECT
public:
    explicit ProfileViewModel(QObject *parent = nullptr);

signals:
};

#endif // PROFILE_VIEW_MODEL_H
