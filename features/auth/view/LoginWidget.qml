import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    required property AuthViewModel viewModel

    readonly property string authStep: "Email"
    readonly property int stepCount: 1
    readonly property int gap: 15
    readonly property string buttonText: qsTr("Continue")

    implicitHeight: loginColumn.implicitHeight

    function submit() {
        emailInput.field.focus = false

        let email = emailInput.field.text
        let emailCheck = Validate.validateEmail(email)

        if (emailCheck)
        {
            emailInput.error(false, "")
            AuthContext.email = email
            viewModel.login(email)
        }
        else
        {
            emailInput.error(true, "Wrong email format")
        }
    }

    ColumnLayout {
        id: loginColumn

        width: parent.width
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 30

        ColumnLayout {
            spacing: 15

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("Welcome back")
                color: "#fff"

                font.bold: true
                font.pointSize: 20
            }

            ColumnLayout {
                spacing: 0

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTr("Log in or create an account")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTr("via email.")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }
            }
        }

        InputField {
            id: emailInput

            Layout.fillWidth: true
            Layout.preferredHeight: 50

            field.placeholderText: qsTr("Email")

            normalColor: "#242527"
            hoverColor: "#292a2c"
        }
    }
}
