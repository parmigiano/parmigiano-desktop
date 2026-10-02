import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

Item {
    id: root

    property alias background: fieldBackground
    property color normalColor: "#ffffff"
    property color hoverColor: "#eeeeee"
    // property alias wrongAnimation: wrongInputAnimation
    property alias field: field

    implicitHeight: field.implicitHeight

    // property bool hasError: false

    function error(isError, message) {
        if (isError)
        {
            field.inputState = "error"
        }
        else if (field.activeFocus)
        {
            field.inputState = "active"
        }
    }

    // ParallelAnimation {
    //     id: wrongInputAnimation

    //     running: false

    //     BorderPulse {
    //         id: fieldPulse

    //         borderTarget: fieldBackground
    //     }

    //     Shake {
    //         id: fieldShake

    //         shakeTarget: root
    //     }
    // }

    TextField {
        property string inputState: "normal"

        id: field

        anchors.fill: parent
        verticalAlignment: Text.AlignVCenter
        leftPadding: 15
        placeholderTextColor: "#909195"
        color: "white"
        font.pointSize: 11
        selectionColor: "#053ba7"

        TapHandler {
            acceptedButtons: Qt.RightButton
            onTapped: contextMenu.menu.popup()
        }

        TextContextMenu {
            id: contextMenu

            target: field
        }

        background:  Rectangle {
            id: fieldBackground

            color: field.hovered ? hoverColor : normalColor
            border.width: 1.4
            radius: 10

            Behavior on color {
                ColorAnimation {
                    duration: 100
                }
            }

            Behavior on border.color {
                ColorAnimation {
                    duration: 100
                }
            }
        }

        state: inputState

        states: [
            State {
                name: "error"

                PropertyChanges {
                    target: fieldBackground

                    border.color: "#f05a5a"
                }
            },

            State {
                name: "active"
                when: field.activeFocus

                PropertyChanges {
                    target: fieldBackground

                    border.color: "#77797c"
                }
            },

            State {
                name: "normal"
                when: !field.activeFocus

                PropertyChanges {
                    target: fieldBackground

                    border.color: normalColor
                }
            }
        ]

        onTextChanged: {
            error(false, "");
        }

        onFocusChanged: {
            error(false, "");
        }
    }

}
