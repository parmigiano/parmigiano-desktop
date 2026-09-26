import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Effects
import QtQuick.Window

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    AuthViewModel {
        id: viewModel
    }

    Connections {
        target: viewModel

        function onNavigateToVerifyCode() {

        }

        function onNavigateToCreateProfile() {

        }

        function onDisplayMessage(message) {
            confirmEmailButton.stopLoadingAnim();
            banner.show(message);
        }
    }

    Action {
        id: clickButtonAction

        onTriggered: {
            let email = confirmEmailInput.field.text;
            let emailCheck = Validate.validateEmail(email);

            if (emailCheck)
            {
                confirmEmailButton.startLoadingAnim();
                //UIStateManager.email = email;
                AuthContext.email = email;
                //uthRepository.login(email);
                viewModel.login(email);
            }
            else
            {
                Validate.playWrongAnimation(confirmEmailInput);
            }
        }
    }

    Timer {
        id: clickButtonDelay
        interval: 300
        repeat: false
    }

    Shortcut {
        sequence: "Return" // normal human enter
        onActivated: {
            clickButtonAction.trigger();
            clickButtonDelay.start();
        }
        enabled: !clickButtonDelay.running
    }

    Shortcut {
        sequence: "Enter" // NumPad enter
        onActivated: {
            clickButtonAction.trigger();
            clickButtonDelay.start();
        }
        enabled: !clickButtonDelay.running
    }

    ColumnLayout {
        width: parent.width
        height: 60
        spacing: 0

        Layout.topMargin: 0
        Layout.margins: 0

        NetworkStatusBanner {
            id: networkBanner

            Layout.alignment: Qt.AlignTop
            Layout.fillWidth: true
            Layout.preferredHeight: height
            Layout.maximumHeight: 35

            visible: height > 0
        }

        SwitchLanguageMenu {
            id: langSwitcher

            Layout.preferredWidth: 45
            Layout.preferredHeight: 30

            Layout.alignment: Qt.AlignRight | Qt.AlignTop
            Layout.margins: 15
        }
    }

    ColumnLayout {
        width: 350

        anchors.horizontalCenter: parent.horizontalCenter
        anchors.verticalCenter: parent.verticalCenter

        Label {
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            Layout.fillWidth: true
            Layout.bottomMargin: 10
            text: qsTr("Your email") // Ваша почта
            font.bold: true
            color: "white"
            font.pointSize: 18
        }

        Label {
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            Layout.fillWidth: true
            Layout.bottomMargin: 30
            text: qsTr("Please enter your email address to log in.") // Пожалуйста, введите ваш адрес электронной почты для входа.
            color: "#708499"
            font.pointSize: 11
            wrapMode: Text.WordWrap
        }

        CustomInput {
            id: confirmEmailInput

            Layout.fillWidth: true
            Layout.preferredHeight: 45
            Layout.bottomMargin: 15

            fieldPlaceholderText: qsTr("example@mail.com")
        }

        ButtonConfirm {
            id: confirmEmailButton

            Layout.fillWidth: true
            Layout.preferredHeight: 45

            buttonTextDefault: qsTr("CONTINUE")
            buttonTextLoading: qsTr("LOADING")

            //buttonText: qsTr("CONTINUE") // ПРОДОЛЖИТЬ
            mouseArea.onClicked: clickButtonAction.trigger()
        }
    }

    // InfoDisplay {
    //     id: banner

    //     width: messageWidth
    // }
}
