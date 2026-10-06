import QtQuick
import QtQuick.Layouts
import QtQuick.Effects

import ParmigianoDesktop.CoreUI

Item {
    id: root

    property alias mouseArea: imageArea

    // required property string windowName

    Rectangle {
        id: imageContainer

        anchors.fill: parent

        color: imageArea.containsMouse ? "#242424" : "#191a1c"
        radius: 8

        Image {
            id: arrowBack

            anchors.centerIn: parent

            sourceSize: Qt.size(25, 25)
            source: "qrc:/assets/arrow_back.svg"

            visible: false
        }

        MultiEffect {
            source: arrowBack

            anchors.fill: arrowBack

            colorization: 1.0
            brightness: imageArea.containsMouse ? 1.0 : 0.0
            colorizationColor: imageArea.containsMouse ? "#fff" : "#7d7d7d"

            Behavior on colorizationColor {
                ColorAnimation {
                    duration: 150
                }
            }

            Behavior on brightness {
                NumberAnimation {
                    duration: 150
                    easing.type: Easing.InOutQuad
                }
            }
        }

        MouseArea {
            id: imageArea

            anchors.fill: parent

            hoverEnabled: true
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                NavigationManager.goBack();
            }
        }

        Behavior on color {
            ColorAnimation {
                duration: 150
            }
        }
    }
}
