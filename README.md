# ParmigianoDesktop

Desktop client of the **Parmigiano Chat** messenger, written in C++ / Qt 6 (QML).
The project is at an early stage: the authentication flow is implemented, the messenger itself is not started yet.

Repository: https://github.com/Flugih/Parmigiano-Desktop-QT

## Features

- Login by email with a one-time confirmation code, profile creation for new accounts
- Transparent Proof-of-Work (PoW) handshake with the server (SHA-256 challenge, automatic retry)
- Network status banner (offline / restored) driven by `QNetworkInformation`
- Interface language switching RU / EN (translation sources are in `translations/`)
- Dark UI with animated step navigation, floating-label inputs, error banners and an exit dialog

## Tech stack

- C++17, Qt 6.8+ (developed with Qt 6.9.1, MSVC 2022 64-bit)
- Qt modules: Core, Gui, Quick, Qml, Network, LinguistTools
- CMake 3.16+ with `qt_add_qml_module` (every layer is a separate QML module / static library)
- Inno Setup for the Windows installer

## Project structure

```
ParmigianoDesktop/
├─ main.cpp, Main.qml            application entry point, root window and root StackView
├─ CMakeLists.txt                targets: appParmigianoDesktop, core, domain, coreUI, featureAuth
├─ Resource.qrc                  assets and translation sources
├─ assets/                       svg icons, logo
├─ core/
│  ├─ config/app_config.h        server base URL and endpoints
│  ├─ network/http_client.*      REST client on top of QNetworkAccessManager
│  └─ ui/                        QML module ParmigianoDesktop.CoreUI:
│                                NavigationManager, UIStateManager, LocalizationManager,
│                                ClipboardManager, Pmg types
├─ domain/pow_process.*          Proof-of-Work solver
├─ features/
│  ├─ auth/                      QML module ParmigianoDesktop.FeatureAuth (see features/auth/README.md)
│  └─ messenger/                 planned
├─ translations/                 .ts sources (en_150, ru_RU)
├─ setup.iss                     Inno Setup installer script
└─ deploy/                       windeployqt output, not tracked by git
```

## Architecture

The app is split into features. Each feature follows the same layering:

view (QML) → viewmodel → repository → service (remote sources) → core/HTTPClient

- **view** — QML screens and components, only UI logic and client-side validation
- **viewmodel** — `QML_ELEMENT` objects: receive calls from QML, map server responses to navigation signals and user messages
- **repository** — builds request DTOs and headers, converts response codes into user messages
- **service/remote** — one source per endpoint on top of `BaseSource`, which owns the HTTP client and the PoW retry logic
- **core** — shared infrastructure: HTTP client, config, navigation, UI state, localization, clipboard
- **domain** — logic without UI (PoW)

Navigation goes through the `NavigationManager` singleton: screens are registered by name in a `StackView` and listen to `navigateTo(pageName)`.
The auth flow is described in detail in [features/auth/README.md](features/auth/README.md).

## Build

Requirements:

- Qt 6.8 or newer with the modules listed above (Qt 6.9.1 MSVC 2022 64-bit is used for development)
- CMake 3.16+, Ninja or another generator
- MSVC 2022 on Windows (other platforms are not tested)

Qt Creator: open `CMakeLists.txt`, choose a Qt 6.8+ kit and build the `appParmigianoDesktop` target.

Command line:

```bash
cmake -S . -B build -DCMAKE_PREFIX_PATH="C:/Qt/6.9.1/msvc2022_64"
cmake --build build --config Debug
```

The executable is `appParmigianoDesktop` in the build directory.

## Configuration

The server address is set at compile time in `core/config/app_config.h`:

- `AppConfig::baseURL` — `http://localhost:8080/api/v2/` by default (the production URL `https://parmigianochat.ru/api/v2/` is commented out)
- endpoints: `auth/confirm/email`, `auth/verify`, `auth/create`

All requests are `POST` with a JSON body; see the auth README for the payloads and the PoW headers.

## Translations

- Sources: `translations/ParmigianoDesktop_en_150.ts`, `translations/ParmigianoDesktop_ru_RU.ts` (currently empty skeletons)
- `LocalizationManager` loads compiled `.qm` files from the resource path `:/ui/translations/release/`; the `.qm` files are not generated or registered in `Resource.qrc` yet, so switching the language has no effect at the moment

## Deployment (Windows)

1. Build the app (Release is recommended)
2. Run `windeployqt` on `appParmigianoDesktop.exe` with the output in `deploy/` (the folder is ignored by git)
3. Put `VC_redist.x64.exe` into `deploy/`
4. Compile `setup.iss` with Inno Setup: it produces `ParmigianoChat_Setup.exe`, installs the app into Program Files (x64 only), installs the Visual C++ redistributable quietly and offers English / Russian installer languages

## Status

- Auth: email login, code verification, profile creation — done; navigation to the messenger after a successful auth, cloud password and password recovery — not implemented
- Messenger: planned
- Known gaps of the auth feature are listed in [features/auth/README.md](features/auth/README.md#known-gaps)
