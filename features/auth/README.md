# Auth feature

## Overview

The feature is responsible for the user authentication cycle:
- login by email (a confirmation code is sent to the email)
- verification of the confirmation code
- profile creation (name + @username) for a new account

The feature also:
- keeps the auth state between steps (`AuthContext.email`)
- talks to the HTTP server through the REST client (`core/network/http_client`)
- solves Proof-of-Work (PoW) challenges from the server transparently and retries the request
- shows the network status (offline / restored)
- notifies the user about server and client-side errors (error banner)
- supports interface language switching (RU / EN)

Not implemented yet: cloud password, password recovery, navigation to the messenger after a successful auth
(see "Known gaps" at the end).

## Structure

- view/ — QML screens
  - `AuthWindow.qml` — container of the auth flow: steps header, inner `StackView`, shared confirm button, exit dialog, error banner, network banner
  - `LoginWidget.qml` — step 1, email input
  - `VerifyCodeWidget.qml` — step 2, 6-digit code input
  - `CreateProfileWindow.qml` — separate top-level screen: name + @username
  - `js/validate.js` — client-side validation (regexps)
  - `LoginPage.qml`, `VerifyCodePage.qml`, `CreateProfilePage.qml`, `CloudPasswordPage.qml` — legacy pages, excluded from the build (commented out in `CMakeLists.txt`)
- viewmodel/ — `AuthViewModel` (`QML_ELEMENT`): UI logic, maps HTTP codes to navigation signals / user messages
- data/ — `AuthContext` (QML singleton): auth state shared between screens (`email`)
- model/ — `RequestData` (json + headers + callback), `ResponseData` (json + code)
- repository/ — `AuthRepository`: builds DTOs and headers, converts response codes into user messages
- service/dto/ — request DTOs with `toJson()`: `LoginRequest`, `VerifyCodeRequest`, `CreateProfileRequest`
- service/remote/ — `BaseSource` (generic request + PoW retry) and thin sources: `LoginSource`, `VerifyCodeSource`, `CreateProfileSource`
- components/ — UI components inside auth
  - inputs: `FloatingLabelInput` (floating label + inline error text), `CustomInput` (legacy, used only by legacy pages)
  - feedback: `ErrorBanner` (popup with header/body), `NetworkStatusBanner`, `ExitDialog`
  - progress: `Steps`, `StepsItem`, `StepCounter`
  - controls: `ButtonConfirm` (default / loading states), `ArrowBack`, `SwitchLanguageMenu`, `TextContextMenu`
  - animations/ — `Shake`, `BorderPulse`

## Data Flow

view → viewmodel → repository → service (remote source) → core/HTTPClient

The answer comes back the same way:

HTTPClient callback → `BaseSource::request` (PoW check / retry) → `RequestData.callback` (repository)
→ repository signal `*Finished(messageHeader, messageBody, code)` → viewmodel slot `process*Finished`
→ viewmodel signal (`navigateTo*` / `displayMessage`) → view

## Endpoints

Configured in `core/config/app_config.h` (`AppConfig::baseURL` = `http://localhost:8080/api/v2/`).
All requests are `POST` with a JSON body (`Content-Type: application/json`).

| Action        | Endpoint             | Body                        |
|---------------|----------------------|-----------------------------|
| login         | `auth/confirm/email` | `{ email }`                 |
| verifyCode    | `auth/verify`        | `{ email, code:int }`       |
| createProfile | `auth/create`        | `{ name, username, email }` |

Headers added by `AuthRepository::defineHeaders()`: `Accept-Language: en`, plus `pow-challenge` / `pow-nonce`
when a PoW nonce has already been solved.

## Dependencies

- core/config/app_config — endpoints
- core/network/http_client — REST client (`RequestTypes`, `sendRequest`)
- core/ui/navigation_manager — `NavigationManager` singleton (`goTo`, `goBack`)
- core/ui/ui_state_manager — network status, current localization
- core/ui/localization_manager — language switching
- core/ui/clipboard_manager — paste of the confirmation code
- core/ui/pmg_types — `Pmg::Language`
- domain/pow_process — Proof-of-Work solver (SHA-256)

## Rules & Constraints

- Client-side validation (`view/js/validate.js`) runs before any request:
  - email: 5–254 chars, `local@domain.tld` format
  - name: 2–24 chars
  - username: 4–24 chars
  - code: exactly 6 digits (only digits are accepted; pasted text is filtered to digits)
- `AuthContext.email` is set by `LoginWidget` after a successful email validation and is reused by the
  verify code and create profile steps.
- Every widget pushed into the inner `StackView` of `AuthWindow` must provide:
  - `authStep` (`"Email"` / `"VerifyCode"`) — highlights the current step in `Steps`
  - `stepCount` (int) — shown by `StepCounter` ("N of 2")
  - `buttonText` — caption of the shared `ButtonConfirm`
  - `submit()` — called when the confirm button is clicked
- PoW: if a response contains `message.challenge`, `BaseSource` solves it (`PoWProcess::solve`, difficulty from
  `message.difficulty`), adds `pow-challenge` / `pow-nonce` headers and repeats the same request.
  At most 2 attempts per request (`_retryAttempts`).
- Response codes are interpreted only in `AuthViewModel`; `AuthRepository` only converts a code into a
  `(messageHeader, messageBody)` pair.
- Page names passed to `NavigationManager.goTo()` must be unique across navigation levels (see Navigation).

## Common Scenarios

### Login Flow

1. User enters the email in `LoginWidget` and presses CONTINUE (`submit()`)
2. The email is validated; on failure the inline error "Wrong email format" is shown under the field
3. `AuthContext.email` is set, `AuthViewModel.login(email)` is called
4. `AuthRepository.login` → `LoginSource` → `POST auth/confirm/email`
5. Depending on the response code:
   - 200 → `navigateToVerifyCode` → inner stack pushes `VerifyCode`
   - other → `displayMessage` → `ErrorBanner`

### Verify Code Flow

1. User enters the 6-digit code in `VerifyCodeWidget` (typing or Ctrl+V) and presses CONFIRM
2. `AuthViewModel.verifyCode(AuthContext.email, code)` (submit is skipped if fewer than 6 digits are entered)
3. `AuthRepository.verifyCode` → `VerifyCodeSource` → `POST auth/verify`
4. Depending on the response code:
   - 200 → messenger (not implemented yet, `qDebug` only)
   - 202 → `navigateToCreateProfile` → root stack pushes `CreateProfileWindow`
   - 400 → invalid code (no UI feedback yet, `qDebug` only)
   - other → `displayMessage` → `ErrorBanner`

### Create Profile Flow

1. User fills the name and @username in `CreateProfileWindow` and presses CONFIRM
2. Both fields are validated; invalid fields are highlighted in red
3. `AuthViewModel.createProfile(name, username, AuthContext.email)`
4. `AuthRepository.createProfile` → `CreateProfileSource` → `POST auth/create`
5. Depending on the response code:
   - 200 → messenger (not implemented yet, `qDebug` only)
   - other → `displayMessage` (not shown on this screen yet, see "Known gaps")

### Exit Flow

1. `ArrowBack` on the first step (`NavigationManager.goBack()` while the inner stack has depth 1) opens `ExitDialog`
2. The content behind the dialog is blurred and dimmed
3. "Exit" calls `Qt.quit()`; "Cancel" or a click outside the dialog closes it

---

### Error Handling

#### Server / request errors

`AuthRepository::defineMessage(code)` produces the banner text:

| Code    | Header                | Body                                                |
|---------|-----------------------|-----------------------------------------------------|
| 400–499 | Client side error     | Check your internet connection and try again later. |
| 500+    | Server is unavailable | Server connection error. Please try again later.    |

The message is shown by `ErrorBanner` at the bottom of `AuthWindow`: the popup slides up from the bottom edge,
does not auto-close and is dismissed with the "Ok" button.

#### Internet Connection State

`NetworkStatusBanner` (top of `AuthWindow`) listens to `UIStateManager.networkStatusChanged`
(fed by `QNetworkInformation` in `main.cpp`):

- no internet → the banner slides down with **"Waiting..."** and a pulsing dot
- connection restored → the banner turns green with **"Internet restored"** and hides after 2 s

#### Input errors

- `FloatingLabelInput.error(true, message)` — red border and label, inline message under the field;
  the error is reset on the next text or focus change
- code fields — `Shake` / `BorderPulse` animations are available but are not triggered by `VerifyCodeWidget` yet

## Navigation

Navigation is two-level. Both levels listen to `NavigationManager.navigateTo(pageName)` and ignore names
they do not know.

Root `StackView` (`Main.qml`, pages are `Component`s registered in `pagesURI`):

- `AuthWindow` (initial)
- `CreateProfileWindow`

Inner `StackView` (`AuthWindow.qml`, inside the auth card, slide + fade transitions, card "pulse" on every push/pop):

- `Login` (initial) → `VerifyCode`

`NavigationManager.goBack()` is handled only by `AuthWindow`: it pops the inner stack, or opens `ExitDialog`
on the first step. `Main.qml` does not handle `navigateBack`, so there is no way back from `CreateProfileWindow`.

Flow:

- Login → VerifyCode (200 on login)
- VerifyCode → CreateProfileWindow (202 on verify)
- VerifyCode → messenger (200 on verify) — TODO
- CreateProfileWindow → messenger (200 on create) — TODO

## Localization

- `SwitchLanguageMenu` calls `LocalizationManager.changeLanguage(Pmg.RU | Pmg.EN)` and
  `UIStateManager.setLocalization(...)`
- Translation sources: `translations/ParmigianoDesktop_en_150.ts`, `translations/ParmigianoDesktop_ru_RU.ts`
  (`LinguistTools` is enabled in CMake); the files are currently empty skeletons

## Known gaps

- Navigation to the messenger after 200 on verify code / create profile is not implemented (`qDebug` only)
- 400 on verify code (invalid code) has no UI feedback (`qDebug` only)
- Transport-level failures (no connection, timeout) arrive with HTTP code `0`; `defineMessage` returns empty
  strings, so `ErrorBanner` is shown empty
- `CreateProfileWindow` creates its own `AuthViewModel` and does not connect to its signals: `displayMessage`
  is lost on this screen; it also has no error banner, network banner, back button or language switcher
- `ButtonConfirm` loading state (`startLoadingAnim` / `stopLoadingAnim`) is not used by `AuthWindow`
- `LocalizationManager` still loads `.qm` files from `:/ui/translations/release/`, which are no longer listed
  in `Resource.qrc`, so switching the language has no visible effect until the `.qm` files are generated
  and registered; `Accept-Language` is hardcoded to `en`
