import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

Item {
    id: root

    property alias errorText: errorText
    property alias inputLabel: label
    property alias inputField: field
    property alias inputContainer: inputPane
    property Item blurSource

    function error(isError, message) {
        if (isError)
        {
            label.color = "#f05a5a"
            background.border.color = "#f05a5a"
        }
        else if (field.activeFocus)
        {
            label.color = "#1c75a9"
            background.border.color = "#1c75a9"
        }

        errorText.newMessage = message
        changeTextAnimation.start()
    }

    implicitHeight: inputPane.implicitHeight

    Pane {
        id: inputPane

        width: parent.width
        height: 50

        background: Rectangle {
            id: background

            color: "#181818"
            radius: 12
            border.color: "#505050"

            Behavior on border.color {
                ColorAnimation {
                    duration: 120
                }
            }
        }

        leftPadding: 20
        rightPadding: 20
        topPadding: 0
        bottomPadding: 0

        // state: inputState

        states: [
            State {
                name: "floating"
                when: field.activeFocus /*&& field.text.length > 0*/

                PropertyChanges {
                    target: label

                    y: -label.height / 2
                    font.pointSize: 10
                    color: "#1c75a9"
                }

                PropertyChanges {
                    target: background

                    color: "#181818"
                    radius: 12
                    border.color: "#1c75a9"
                }
            },

            State {
                name: "normal with text"
                when: !field.activeFocus && field.text.length > 0

                PropertyChanges {
                    target: label

                    y: -label.height / 2
                    font.pointSize: 10
                    color: "#575757"
                }

                PropertyChanges {
                    target: background

                    color: "#181818"
                    radius: 12
                    border.color: "#505050"
                }
            },

            State {
                name: "normal with out text"

                PropertyChanges {
                    target: label

                    y: 0
                    font.pointSize: 12
                    color: "#575757"
                }

                PropertyChanges {
                    target: background

                    color: "#181818"
                    radius: 12
                    border.color: "#505050"
                }
            }
        ]

        Text {
            id: label

            // anchors.fill: parent
            // verticalAlignment: Qt.AlignVCenter
            z: 10
            y: (inputPane.height - label.height) / 2

            text: qsTr("")
            font.pointSize: 12
            color: "#575757"

            Behavior on color {
                ColorAnimation {
                    duration: 120
                }
            }

            // Rectangle {
            //     anchors.fill: parent
            //     color: "#181818"
            //     z: -1
            // }
        }

        TextField {
            id: field

            anchors.fill: parent
            verticalAlignment: Qt.AlignVCenter

            background: null

            leftPadding: 0
            rightPadding: 0
            topPadding: 0
            bottomPadding: 0

            color: "#fff"
            selectionColor: "#053ba7"
            font.pointSize: 12

            TapHandler {
                acceptedButtons: Qt.RightButton
                onTapped: contextMenu.menu.popup()
            }

            TextContextMenu {
                id: contextMenu

                target: field
            }

            onTextChanged: {
                error(false, "");
            }

            onFocusChanged: {
                error(false, "");
            }
        }

        Row {
            x: -4
            z: 3

            Rectangle {
                width: label.width / 2 + 4
                height: 1

                transformOrigin: Item.Right
                scale: (field.activeFocus || field.text.length > 0) ? 1 : 0

                color: "#181818"

                Behavior on scale {
                    NumberAnimation {
                        duration: 150
                    }
                }
            }

            Rectangle {
                width: label.width / 2 + 4
                height: 1

                transformOrigin: Item.Left
                scale: (field.activeFocus || field.text.length > 0) ? 1 : 0

                color: "#181818"

                Behavior on scale {
                    NumberAnimation {
                        duration: 150
                    }
                }
            }
        }

        transitions: Transition {
            NumberAnimation {
                properties: "y,font.pointSize"
                duration: 100
            }
        }
    }

    Text {
        property string newMessage: ""

        id: errorText

        // text: "Email"
        // x: label.mapToItem(parent, 0, 0).x
        y: inputPane.height
        x: inputPane.x + inputPane.leftPadding + background.border.width

        // anchors {
        //     top: container.bottom
        //     left: container.left
        //     topMargin: 6
        // }

        font.pointSize: 10
        color: "#f05a5a"

        SequentialAnimation {
            id: changeTextAnimation

            NumberAnimation {
                target: errorText
                property: "opacity"
                to: 0
                duration: 60
            }

            ScriptAction {
                script: errorText.text = errorText.newMessage
            }

            NumberAnimation {
                target: errorText
                property: "opacity"
                to: 1
                duration: 60
            }
        }
    }
}
