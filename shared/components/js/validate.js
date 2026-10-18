.pragma library

function validateString(string, regExp) {
    if (string.length === 0) return false;
    return regExp.test(string);
}

function validateEmail(email) {
    return validateString(email, /^(?=.{5,254}$)\w+([.-]?\w+)*@\w+([.-]?\w+)*\.\w+$/);
}

function validateName(name) {
    return validateString(name, /^.{2,24}$/); // /^[a-zA-Zа-яёА-ЯЁ0-9_]{2,24}$/
}

function validateUsername(username) {
    return validateString(username, /^.{4,24}$/); // /^[0-9A-Za-z_]{4,24}$/
}

function validatePassword(password) {
    return validateString(password, /^.{8,16}$/);
}

function playWrongAnimation(obj) {
    obj.wrongAnimation.start();
    obj.field.focus = false;
}

function verifyCodeValid(repeaterCodeFields) {
    let code = [];

    for (let i = 0; i < repeaterCodeFields.count; ++i)
    {
        let target = repeaterCodeFields.itemAt(i);

        if (target && target.text)
        {
            code.push(target.text);
        }
    }

    if (code.length === repeaterCodeFields.count)
    {
        return true;
    }
    else
    {
        return false;
    }
}
