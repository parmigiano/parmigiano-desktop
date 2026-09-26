import QtQuick
import QtQuick.Controls
import QtQuick.Effects

// import core.UIStateManager 1.0
// import core.types 1.0
import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import Types

Item {
    id: root

    Connections {
        target: UIStateManager

        function onLocalizationChanged(lang) {
            if(lang === Pmg.RU
                && switcherText.text !== "RU")
            {
                switcherText.text = "RU";
            }
            else if(lang === Pmg.EN
                    && switcherText.text !== "EN")
            {
                switcherText.text = "EN";
            }
        }
    }

    Rectangle {
        id: rectangle

        width: 45
        height: 30

        color: rectangleMouseArea.containsMouse ? Qt.rgba(0.2, 0.56, 0.92, 0.15) : "transparent"
        radius: 8

        Behavior on color {
            ColorAnimation {
                duration: 150
                easing.type: Easing.OutCubic
            }
        }

        Text {
            id: switcherText

            anchors.verticalCenter: parent.verticalCenter
            anchors.horizontalCenter: parent.horizontalCenter

            font.pointSize: 11
            text: qsTr("RU") // RU/EN
            color: "#3390ec"
        }

        MouseArea {
            id: rectangleMouseArea

            anchors.fill: parent
            hoverEnabled: true
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                contextMenu.popup(-(contextMenu.width - rectangle.width), rectangle.height + 10);
            }
        }
    }

    Menu {
        id: contextMenu

        implicitWidth: 125

        topPadding: 5
        bottomPadding: 5
        leftPadding: 5
        rightPadding: 5

        enter: Transition {
            NumberAnimation {
                property: "opacity"
                from: 0.0
                to: 1.0
                duration: 60
            }
        }

        delegate: MenuItem {
            id: menuItem

            implicitWidth: 125
            implicitHeight: 35

            contentItem: Text {
                text: menuItem.text
                font.pointSize: 10
                color: "#fff"

                leftPadding: 10
                anchors.left: parent.left
                verticalAlignment: Qt.AlignVCenter
            }

            background: Rectangle {
                anchors.fill: parent
                color: menuItem.hovered ? "#323232" : "#202020"
                radius: 5

                Behavior on color {
                    ColorAnimation {
                        duration: 150
                        easing.type: Easing.OutCubic
                    }
                }
            }
        }

        background: Rectangle {
            id: menuBackground

            anchors.fill: parent

            color: "#1e1f21"
            border.color: '#373737'
            radius: 10
        }

        Action {
            text: qsTr("Russian")

            onTriggered: {
                LocalizationManager.changeLanguage(Pmg.RU);
                UIStateManager.setLocalization(Pmg.RU);
            }
        }

        MenuSeparator {
            topPadding: 2
            bottomPadding: 2

            contentItem: Rectangle {
                implicitWidth: 200
                implicitHeight: 1
                color: "#373737"
            }
        }

        Action {
            text: qsTr("English")

            onTriggered: {
                LocalizationManager.changeLanguage(Pmg.EN);
                UIStateManager.setLocalization(Pmg.EN);
            }
        }
    }
}
