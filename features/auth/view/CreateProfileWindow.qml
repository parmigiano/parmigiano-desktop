import QtQuick
import QtQuick.Layouts
import QtQuick.Controls

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    AuthViewModel {
        id: viewModel
    }

    ColumnLayout {
        width: parent.width
        height: 60
        spacing: 0

        Layout.margins: 0

        NetworkStatusBanner {
            // id: networkBanner

            Layout.alignment: Qt.AlignTop
            Layout.fillWidth: true
            Layout.preferredHeight: height
            Layout.maximumHeight: 35

            visible: height > 0
        }
    }

    ColumnLayout {
        width: 360
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 35

        ColumnLayout {
            spacing: 15

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("What is your name?")
                color: "#fff"

                font.bold: true
                font.pointSize: 18
            }

            ColumnLayout {
                spacing: 0

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTr("This is how you will be seen in chats.")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTr("The photo can be added later.")
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

                field.placeholderText: qsTr("Name")

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

            ColumnLayout {
                spacing: 10

                Action {
                    id: clickButtonAction

                    onTriggered: {
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
                }

                Shortcut {
                    sequence: "Return" // normal human enter
                    onActivated: {
                        clickButtonAction.trigger();
                    }
                }

                Shortcut {
                    sequence: "Enter" // NumPad enter
                    onActivated: {
                        clickButtonAction.trigger();
                    }
                }

                ButtonConfirm {
                    Layout.fillWidth: true
                    Layout.preferredHeight: 50

                    buttonTextDefault: qsTr("Confirm")
                    buttonTextLoading: qsTr("Loading")

                    mouseArea.onClicked: {
                        clickButtonAction.trigger();
                    }
                }

                Row {
                    Layout.alignment: Qt.AlignHCenter

                    spacing: 0

                    Text {
                        Layout.fillWidth: true
                        horizontalAlignment: Qt.AlignHCenter

                        text: qsTr("The entrance is designed as - ")
                        color: "#a1a2a5"

                        font.pointSize: 10

                        wrapMode: Text.WordWrap
                    }

                    Text {
                        Layout.fillWidth: true
                        horizontalAlignment: Qt.AlignHCenter

                        text: AuthContext.email
                        color: "#eeeeef"

                        font.pointSize: 10

                        wrapMode: Text.WordWrap
                    }
                }
            }
        }
    }
}
