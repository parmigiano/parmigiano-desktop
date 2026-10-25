#ifndef HTTP_RESPONSE_STATUS_H
#define HTTP_RESPONSE_STATUS_H

enum HTTPStatus {
    Unknown = 0,
    Ok = 200,
    Accepted = 202,
    BadRequest = 400,
    NotFound = 404,
    InternalError = 500
};

#endif // HTTP_RESPONSE_STATUS_H
