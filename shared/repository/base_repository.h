#ifndef BASE_REPOSITORY_H
#define BASE_REPOSITORY_H

#include <QObject>

class BaseRepository : public QObject
{
    Q_OBJECT
public:
    explicit BaseRepository(QObject *parent = nullptr);

signals:
};

#endif // BASE_REPOSITORY_H
