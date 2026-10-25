#ifndef REQUEST_RESULT_H
#define REQUEST_RESULT_H

#include "QString"

struct RequestResult
{
    QString messageHeader;
    QString messageBody;
    int code;
};

#endif // REQUEST_RESULT_H
