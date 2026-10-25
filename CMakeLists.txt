cmake_minimum_required(VERSION 3.16)

project(ParmigianoDesktop VERSION 0.1 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTOUIC ON)

find_package(Qt6 REQUIRED COMPONENTS Quick Gui Core Qml Network LinguistTools)
find_package(Qt6Keychain CONFIG REQUIRED)

qt_standard_project_setup(REQUIRES 6.8)

qt_add_executable(appParmigianoDesktop)

qt_add_qml_module(appParmigianoDesktop
    URI ParmigianoDesktop
    VERSION 1.0
    QML_FILES
        Main.qml
    SOURCES
        main.cpp
    RESOURCES
        Resource.qrc
)
target_link_libraries(appParmigianoDesktop
    PRIVATE
        Qt6::Core
        Qt6::Quick
        Qt6::Gui
        #core
        coreUI
        #domain
        #featureAuth
)

add_library(core STATIC
        # Core/Network/http
        # -->
        core/network/http/http_client.h
        core/network/http/http_client.cpp
        core/network/http/http_response_status.h

        # Core/Config
        # -->
        core/config/app_config.h
        core/config/app_config.cpp
)
target_include_directories(core PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(core
    PRIVATE
        Qt6::Core
        Qt6::Gui
        Qt6::Qml
)

qt_add_library(coreUI STATIC)
qt_add_qml_module(coreUI
    URI ParmigianoDesktop.CoreUI
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/CoreUI"
    SOURCES
        # Core/ui
        # -->
        core/ui/navigation_manager.h
        core/ui/navigation_manager.cpp
        core/ui/ui_state_manager.h
        core/ui/ui_state_manager.cpp
        core/ui/clipboard_manager.h
        core/ui/clipboard_manager.cpp
        core/ui/localization_manager.h
        core/ui/localization_manager.cpp
        core/ui/pmg_types.h
)
target_include_directories(coreUI PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_include_directories(coreUI # for MOC padla takaya
    PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/core/ui
)
target_link_libraries(coreUI
    PRIVATE
        Qt6::Core
        Qt6::Gui
        Qt6::Qml
)

add_library(domain STATIC
        domain/pow_process.h
        domain/pow_process.cpp
        domain/token_manager.h domain/token_manager.cpp
)
target_include_directories(domain PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(domain
    PRIVATE
        Qt6::Core
        Qt6::Gui
        Qt6Keychain::Qt6Keychain
)
target_link_libraries(domain PRIVATE Qt6::Core)

add_library(shared STATIC
        # shared/model
        # -->
        shared/model/request_data.h
        shared/model/response_data.h
        shared/model/user_profile.h

        # shared/data
        # -->
        shared/data/user_store.h
        shared/data/user_store.cpp

        # shared/repository
        # -->
        shared/repository/base_repository.h
        shared/repository/base_repository.cpp

        # shared/service/remote
        # -->
        shared/service/remote/base_source.cpp
        shared/service/remote/base_source.h
        shared/model/request_result.h
)
target_include_directories(shared PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(shared
    PRIVATE
        Qt6::Core
        Qt6::Network
)

qt_add_library(sharedUI STATIC)
set_source_files_properties(shared/components/js/validate.js PROPERTIES
    QT_QML_SOURCE_TYPENAME Validate
)
qt_add_qml_module(sharedUI
    URI ParmigianoDesktop.SharedUI
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/SharedUI"
    QML_FILES
        # shared/components
        # -->
        shared/components/FieldFrame.qml
        shared/components/TextContextMenu.qml
        shared/components/ButtonConfirm.qml

        # shared/components/js
        # -->
        shared/components/js/validate.js
)

qt_add_library(featureAuth STATIC)
qt_add_qml_module(featureAuth
    URI ParmigianoDesktop.FeatureAuth
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/FeatureAuth"
    #BASE_DIR ${CMAKE_CURRENT_SOURCE_DIR}
    SOURCES
        # Features/Auth/data
        # -->

        # Features/Auth/model
        # -->
        # features/auth/model/request_data.h
        # features/auth/model/response_data.h

        # Features/Auth/repository
        # -->
        features/auth/repository/auth_repository.h
        features/auth/repository/auth_repository.cpp

        # Features/Auth/sevrice/dto
        # -->
        features/auth/service/dto/create_profile_model.h
        features/auth/service/dto/login_model.h
        features/auth/service/dto/verify_code_model.h

        # Features/Auth/sevrice/remote
        # -->
        # features/auth/service/remote/base_source.h
        # features/auth/service/remote/base_source.cpp

        # Features/Auth/viewmodel
        # -->
        features/auth/viewmodel/auth_view_model.h
        features/auth/viewmodel/auth_view_model.cpp

    QML_FILES
        # Features/Auth/components
        # -->
        features/auth/components/NetworkStatusBanner.qml
        features/auth/components/SwitchLanguageMenu.qml
        features/auth/components/InputField.qml

        # Features/Auth/components/animations
        # -->
        features/auth/components/animations/BorderPulse.qml
        features/auth/components/animations/Shake.qml

        # Features/Auth/view
        # -->
        features/auth/view/AuthWindow.qml
        features/auth/view/LoginWidget.qml
        features/auth/view/CreateProfileWidget.qml
        features/auth/view/VerifyCodeWidget.qml
        SOURCES features/auth/service/remote/auth_source.h features/auth/service/remote/auth_source.cpp
)
target_include_directories(featureAuth PUBLIC ${CMAKE_CURRENT_SOURCE_DIR} )
target_include_directories(featureAuth # for MOC padla takaya
    PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/features/auth/data
        ${CMAKE_CURRENT_SOURCE_DIR}/features/auth/viewmodel
)
target_link_libraries(featureAuth
    PRIVATE
        Qt6::Core
        Qt6::Gui
        Qt6::Quick
        core
        coreUI
        domain
        shared
        sharedUI
)
#target_link_libraries(featureAuth PRIVATE Qt6::Core)

qt_add_library(featureMessenger STATIC)
qt_add_qml_module(featureMessenger
    URI ParmigianoDesktop.FeatureMessenger
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/FeatureMessenger"
    SOURCES
        # Features/messenger/data
        # -->

        # Features/messenger/model
        # -->

        # Features/messenger/repository
        # -->

        # Features/messenger/sevrice/dto
        # -->

        # Features/messenger/sevrice/remote
        # -->

        # Features/messenger/viewmodel
        # -->

    QML_FILES
        # Features/messenger/components
        # -->

        # Features/messenger/view
        # -->
        features/messenger/view/MessengerWindow.qml
)
# target_include_directories(featureMessenger PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
# target_include_directories(featureMessenger # for MOC padla takaya
#     PUBLIC
#         ${CMAKE_CURRENT_SOURCE_DIR}/core/ui
# )
target_link_libraries(featureMessenger
    PRIVATE
        # Qt6::Core
        # Qt6::Gui
        # Qt6::Quick
        featureSidebar
)

qt_add_library(featureSidebar STATIC)
qt_add_qml_module(featureSidebar
    URI ParmigianoDesktop.FeatureSidebar
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/FeatureSidebar"
    QML_FILES
        features/sidebar/view/Sidebar.qml
)
target_link_libraries(featureSidebar
    PRIVATE
        featureProfile
)

qt_add_library(featureProfile STATIC)
qt_add_qml_module(featureProfile
    URI ParmigianoDesktop.FeatureProfile
    VERSION 1.0
    OUTPUT_DIRECTORY "ParmigianoDesktop/FeatureProfile"
    QML_FILES
        features/profile/view/ProfileWidget.qml
        features/profile/view/ProfilePage.qml
        features/profile/view/EditPage.qml

        features/profile/components/ProfileDialog.qml
        features/profile/components/ProfileInput.qml
    SOURCES
        features/profile/viewmodel/profile_view_model.h
        features/profile/viewmodel/profile_view_model.cpp
        SOURCES features/profile/repository/profile_repository.h features/profile/repository/profile_repository.cpp
        SOURCES features/profile/service/remote/profile_source.h features/profile/service/remote/profile_source.cpp
)
target_include_directories(featureProfile PUBLIC ${CMAKE_CURRENT_SOURCE_DIR} )
target_include_directories(featureProfile # for MOC padla takaya
    PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/features/profile/viewmodel
)

# Qt for iOS sets MACOSX_BUNDLE_GUI_IDENTIFIER automatically since Qt 6.1.
# If you are developing for iOS or macOS you should consider setting an
# explicit, fixed bundle identifier manually though.
set_target_properties(appParmigianoDesktop PROPERTIES
#    MACOSX_BUNDLE_GUI_IDENTIFIER com.example.appParmigianoDesktop
    MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
    MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
    MACOSX_BUNDLE TRUE
    WIN32_EXECUTABLE TRUE
)

include(GNUInstallDirs)
install(TARGETS appParmigianoDesktop
    BUNDLE DESTINATION .
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
