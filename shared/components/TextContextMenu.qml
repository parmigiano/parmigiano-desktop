import QtQuick
import QtQuick.Controls
import QtQuick.Effects

Item {
    id: root

    property alias menu: contextMenu
    property Item target

    Menu {
        id: contextMenu

        implicitWidth: 200

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

            NumberAnimation {
                property: "scale"
                from: 0.9
                to: 1.0
                duration: 60
            }
        }

        delegate: MenuItem {
            id: menuItem

            implicitWidth: 200
            implicitHeight: 35

            contentItem: Item {
                Text {
                    text: menuItem.text
                    font.pointSize: 10
                    color: "#fff"

                    anchors.left: parent.left
                    anchors.verticalCenter: parent.verticalCenter
                }

                Text {
                    text:  menuItem.action.shortcut || ""
                    font.pointSize: 10
                    color: "#7d7d7d"

                    anchors.right: parent.right
                    anchors.verticalCenter: parent.verticalCenter
                }
            }

            background: Rectangle {
                anchors.fill: parent
                color: menuItem.hovered ? "#fff" : "#232323"
                radius: 5
                opacity: 0.1

                Behavior on color {
                    ColorAnimation {
                        duration: 150
                        easing.type: Easing.OutCubic
                    }
                }
            }
        }

        background: Rectangle {
            anchors.fill: parent

            color: "#232323"
            border.color: "#373737"
            radius: 10
            opacity: 0.95

            MultiEffect {
                source: parent

                blurEnabled: true
                blur: 0.5
            }
        }

        Action {
            text: qsTrId("edit.copy")
            shortcut: "Ctrl+C"

            onTriggered: {
                if (target && target.copy)
                {
                    target.copy()
                };
            }
        }

        Action {
            text: qsTrId("edit.paste")
            shortcut: "Ctrl+V"

            onTriggered: {
                if (target && target.paste)
                {
                    target.paste()
                }
            }
        }

        Action {
            text: qsTrId("edit.cut")
            shortcut: "Ctrl+X"

            onTriggered: {
                if (target && target.cut)
                {
                    target.cut()
                }
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
            text: qsTrId("edit.select_all")
            shortcut: "Ctrl+A"

            onTriggered: {
                if (target && target.selectAll)
                {
                    target.selectAll()
                }
            }
        }
    }
}
