import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Effects

Item {
    id: root

    property alias dialog: dialog
    property Item content
    // property alias blur: effectSource

    function openManual() {
        dialog.open()

        // content.layer.enabled = true
        content.blurAmount = 1
        blackoutRect.opacity = 0.45
        blackoutArea.visible = true
    }

    function closeManual() {
        dialog.close()

        // content.layerTimer.start()
        content.blurAmount = 0
        blackoutRect.opacity = 0
        blackoutArea.visible = false
    }

    Popup {
        id: dialog

        x: (parent.width - dialog.width) / 2
        y: (parent.height - dialog.height) / 2

        // anchors.centerIn: parent
        width: 350
        // height: implicitHeight
        // height: 500

        padding: 25

        // closePolicy: Popup.NoAutoClose

        background: Rectangle {
            color: "#242424"
            border.color: "#373737"
            border.width: 1
            radius: 20
        }

        enter: Transition {
            ParallelAnimation {
                NumberAnimation {
                    property: "opacity"
                    from: 0.0
                    to: 1.0
                    duration: 200
                    easing.type: Easing.OutCubic
                }
                NumberAnimation {
                    property: "y"
                    from: (parent.height - dialog.height) / 2 + 10
                    to: (parent.height - dialog.height) / 2
                    duration: 200

                    easing.type: Easing.BezierSpline
                    // easing.bezierCurve: [0.34, 1.56, 0.64, 1, 1, 1]
                }
            }
        }

        exit: Transition {
            NumberAnimation {
                property: "opacity"
                from: 1.0
                to: 0.0
                duration: 100
                easing.type: Easing.InQuad
            }

            NumberAnimation {
                property: "y"
                from: (parent.height - dialog.height) / 2
                to: (parent.height - dialog.height) / 2 - 20
                duration: 300

                easing.type: Easing.BezierSpline
                // easing.bezierCurve: [0.34, 1.56, 0.64, 1, 1, 1]
            }
        }

        ColumnLayout {
            anchors.fill: parent

            Text {
                Layout.fillWidth: true

                text: qsTr("Close the app?")
                color: "#fff"
                font.bold: true
                font.pointSize: 16

                wrapMode: Text.WordWrap
            }

            Text {
                Layout.fillWidth: true

                text: qsTr("The login will be interrupted, and you will return to the system")
                color: "#575757"
                font.pointSize: 10

                wrapMode: Text.WordWrap
            }

            Row {
                Layout.alignment: Qt.AlignRight
                Layout.topMargin: 10

                spacing: 10

                Rectangle {
                    id: cancelButton

                    height: 35
                    width: 70

                    color: cancelMouseArea.containsMouse ? "#3c3c3c" : "#323232"
                    radius: 8

                    Behavior on color {
                        ColorAnimation {
                            duration: 50
                        }
                    }

                    Text {
                        id: cancelText

                        anchors.centerIn: parent

                        text: qsTr("Cancel")

                        font.pointSize: 11
                        font.bold: true
                        color: "white"
                    }

                    MouseArea {
                        id: cancelMouseArea

                        anchors.fill: parent
                        hoverEnabled: true
                        cursorShape: Qt.PointingHandCursor

                        onClicked: closeManual()
                    }
                }

                Rectangle {
                    id: exitButton

                    height: 35
                    width: 70

                    color: exitMouseArea.containsMouse ? "#ef5f5f" : "#d84d4d"
                    radius: 8

                    Behavior on color {
                        ColorAnimation {
                            duration: 50
                        }
                    }

                    Text {
                        id: exitText

                        anchors.centerIn: parent

                        text: qsTr("Exit")

                        font.pointSize: 11
                        font.bold: true
                        color: "white"
                    }

                    MouseArea {
                        id: exitMouseArea

                        anchors.fill: parent
                        hoverEnabled: true
                        cursorShape: Qt.PointingHandCursor

                        onClicked: Qt.quit()
                    }
                }
            }
        }
    }

    Rectangle {
        id: blackoutRect

        anchors.fill: parent

        color: "#000"
        // opacity: dialog.visible ? 0.35 : 0
        opacity: 0

        Behavior on opacity {
            NumberAnimation {
                duration: 200
                easing.type: Easing.OutCubic
            }
        }

        MouseArea {
            id: blackoutArea

            anchors.fill: parent
            hoverEnabled: true
            visible: false

            onClicked: closeManual()
        }
    }
}
