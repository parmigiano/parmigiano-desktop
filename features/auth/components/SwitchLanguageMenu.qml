import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Effects

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import Types

Item {
    id: root

    Connections {
        target: UIStateManager

        function onLocalizationChanged(lang) {
            if(lang === Pmg.RU
                && switcherText.text !== qsTrId("language.russian"))
            {
                switcherText.text = qsTrId("language.russian");
            }
            else if(lang === Pmg.EN
                    && switcherText.text !== qsTrId("language.english"))
            {
                switcherText.text = qsTrId("language.english");
            }
        }
    }

    implicitWidth: textRow.implicitWidth * 1.2
    implicitHeight: 30

    Rectangle {
        id: rectangle

        anchors.fill: parent

        color: rectangleMouseArea.containsMouse ? "#222325" : "#191a1c"
        radius: 6

        Behavior on color {
            ColorAnimation {
                duration: 150
                easing.type: Easing.OutCubic
            }
        }

        Row {
            id: textRow

            anchors.verticalCenter: parent.verticalCenter
            anchors.horizontalCenter: parent.horizontalCenter

            spacing: 8

            Image {
                anchors.verticalCenter: parent.verticalCenter

                sourceSize: Qt.size(15, 15)
                source: "qrc:/assets/language_globe.svg"
            }

            Text {
                id: switcherText

                anchors.verticalCenter: parent.verticalCenter

                font.pointSize: 10
                text: qsTrId("language.english")
                color: "#a1a2a5"
            }

            Image {
                anchors.verticalCenter: parent.verticalCenter

                sourceSize: Qt.size(14, 14)
                source: "qrc:/assets/language_chevron.svg"
            }
        }

        MouseArea {
            id: rectangleMouseArea

            anchors.fill: parent
            hoverEnabled: true
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                contextMenu.popup((rectangle.width - contextMenu.width) / 2, -contextMenu.height - 10);
            }
        }
    }

    Menu {
        id: contextMenu

        implicitWidth: 150

        topPadding: 8
        bottomPadding: 8
        leftPadding: 8
        rightPadding: 8

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

            contentItem: RowLayout {
                Text {
                    Layout.alignment: Qt.AlignVCenter

                    text: menuItem.text
                    font.pointSize: 10
                    color: "#fff"
                }

                Item {
                    Layout.fillWidth: true
                }

                Image {
                    Layout.alignment: Qt.AlignVCenter

                    sourceSize: Qt.size(14, 14)
                    source: menuItem.action.image
                }
            }

            background: Rectangle {
                anchors.fill: parent
                color: menuItem.hovered ? "#2f2f2f" : "#232323"
                radius: 10

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

            color: "#232323"
            border.color: '#373737'
            radius: 15
        }

        Action {
            text: qsTrId("language.russian")
            property url image: UIStateManager.localization === Pmg.RU ? "qrc:/assets/language_check.svg" : ""

            onTriggered: {
                LocalizationManager.changeLanguage(Pmg.RU);
                UIStateManager.setLocalization(Pmg.RU);
            }
        }

        Action {
            text: qsTrId("language.english")
            property url image: UIStateManager.localization === Pmg.EN ? "qrc:/assets/language_check.svg" : ""

            onTriggered: {
                LocalizationManager.changeLanguage(Pmg.EN);
                UIStateManager.setLocalization(Pmg.EN);
            }
        }
    }
}
