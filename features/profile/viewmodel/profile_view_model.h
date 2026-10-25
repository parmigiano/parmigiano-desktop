#ifndef PROFILE_VIEW_MODEL_H
#define PROFILE_VIEW_MODEL_H

#include <QObject>
#include <QtQml/qqmlregistration.h>

class ProfileRepository;

class ProfileViewModel : public QObject
{
    Q_OBJECT
    QML_ELEMENT
public:
    explicit ProfileViewModel(QObject *parent = nullptr);
    virtual ~ProfileViewModel() = default;

    Q_INVOKABLE void fillProfile();

private:
    ProfileRepository* _ProfileRepository;

signals:
    // void fillProfileDataFinished();
};

#endif // PROFILE_VIEW_MODEL_H
