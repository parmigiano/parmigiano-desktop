import QtQuick
import QtQuick.Controls

Item {
    id: root

    property alias mouseArea: buttonMouseArea

    property string buttonTextDefault: ""
    property string buttonTextLoading: ""

    function startLoadingAnim() {
        button.buttonState = "loading";
    }

    function stopLoadingAnim() {
        clickButtonDelay.start();
    }

    Timer {
        id: clickButtonDelay
        interval: 250
        running: false
        repeat: false

        onTriggered: {
            button.buttonState = "default";
        }
    }

    Rectangle {
        id: button

        property string buttonState: "default"
        state: buttonState

        radius: 12
        anchors.fill: parent

        Behavior on color {
            ColorAnimation {
                duration: 100
            }
        }

        states: [
            State {
                name: "default"
                PropertyChanges {
                    target: buttonMouseArea

                    hoverEnabled: true
                    cursorShape: Qt.PointingHandCursor
                    enabled: true
                }

                PropertyChanges {
                    target: button

                    color: buttonMouseArea.containsMouse ? "#fff" : "#ececeb"
                }

                PropertyChanges {
                    target: buttonText

                    text: buttonTextDefault
                }
            },

            State {
                name: "loading"
                PropertyChanges {
                    target: buttonMouseArea

                    hoverEnabled: false
                    cursorShape: Qt.ArrowCursor
                    enabled: false
                }

                PropertyChanges {
                    target: button

                    color: "#2b6fb3"
                }

                PropertyChanges {
                    target: buttonText

                    text: buttonTextLoading
                }
            }
        ]

        Text {
            id: buttonText

            font.pointSize: 11
            font.bold: true
            color: "#191a1c"
            anchors.centerIn: parent
        }

        MouseArea {
            id: buttonMouseArea

            anchors.fill: parent
            hoverEnabled: true
            cursorShape: Qt.PointingHandCursor
        }
    }
}
