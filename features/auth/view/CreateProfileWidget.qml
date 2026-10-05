import QtQuick
import QtQuick.Layouts
import QtQuick.Controls

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    required property AuthViewModel viewModel

    readonly property int gap: 15
    readonly property string buttonText: qsTrId("auth.profile.confirm")

    implicitHeight: createProfileColumn.implicitHeight

    function submit() {
        nameInput.field.focus = false
        usernameInput.field.focus = false

        let name = nameInput.field.text
        let username = usernameInput.field.text

        let nameCheck = Validate.validateName(name)
        let usernameCheck = Validate.validateUsername(username)

        if (nameCheck && usernameCheck)
        {
            nameInput.error(false, "")
            usernameInput.error(false, "")

            viewModel.createProfile(name, username, AuthContext.email)
        }
        else if (nameCheck && !usernameCheck)
        {
            usernameInput.error(true, "")
        }
        else if (!nameCheck && usernameCheck)
        {
            nameInput.error(true, "")
        }
        else
        {
            nameInput.error(true, "")
            usernameInput.error(true, "")
        }
    }

    ColumnLayout {
        id: createProfileColumn

        width: 360
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 35

        ColumnLayout {
            spacing: 10

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTrId("auth.profile.name.title")
                color: "#fff"

                font.bold: true
                font.pointSize: 18
            }

            ColumnLayout {
                spacing: 0

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTrId("auth.profile.name.description")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTrId("auth.profile.photo.hint")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }
            }
        }

        ColumnLayout {
            spacing: 15

            InputField {
                id: nameInput

                Layout.fillWidth: true
                Layout.preferredHeight: 50

                field.placeholderText: qsTrId("auth.profile.name.placeholder")

                normalColor: "#242527"
                hoverColor: "#292a2c"
            }

            InputField {
                id: usernameInput

                Layout.fillWidth: true
                Layout.preferredHeight: 50

                field.placeholderText: "@Username"

                normalColor: "#242527"
                hoverColor: "#292a2c"
            }
        }
    }
}
