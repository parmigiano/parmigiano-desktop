import QtQuick
import QtQuick.Layouts

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    AuthViewModel {
        id: viewModel
    }

    ColumnLayout {
        width: 360
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 35

        ColumnLayout {
            spacing: 5

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("Your profile")
                color: "#fff"

                font.bold: true
                font.pointSize: 18
            }

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("This is how you will be seen in chats.")
                color: "#7d7d7d"

                font.pointSize: 10

                wrapMode: Text.WordWrap
            }

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("The photo can be added later.")
                color: "#7d7d7d"

                font.pointSize: 10

                wrapMode: Text.WordWrap
            }
        }

        ColumnLayout {
            spacing: 10

            FloatingLabelInput {
                id: nameInput

                Layout.fillWidth: true
                Layout.preferredHeight: inputContainer.height

                inputLabel.text: qsTr("Name")
            }

            FloatingLabelInput {
                id: usernameInput

                Layout.fillWidth: true
                Layout.preferredHeight: inputContainer.height

                inputLabel.text: qsTr("@Username")
            }
        }

        ColumnLayout {
            spacing: 10

            ButtonConfirm {
                Layout.fillWidth: true
                Layout.preferredHeight: 50

                buttonTextDefault: qsTr("CONFIRM")
                buttonTextLoading: qsTr("LOADING")

                mouseArea.onClicked: {
                    nameInput.inputField.focus = false
                    usernameInput.inputField.focus = false

                    let nameCheck = Validate.validateName(nameInput.inputField.text)
                    let usernameCheck = Validate.validateUsername(usernameInput.inputField.text)

                    if (nameCheck && usernameCheck)
                    {
                        nameInput.error(false, "")
                        usernameInput.error(false, "")

                        viewModel.createProfile(nameInput.inputField.text, usernameInput.inputField.text, AuthContext.email)
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
            }

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("The entrance is designed as - ") + AuthContext.email
                color: "#7d7d7d"

                font.pointSize: 10

                wrapMode: Text.WordWrap
            }
        }
    }
}
