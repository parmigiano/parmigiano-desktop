import QtQuick
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Layouts

Item {
    id: root

    function show(messageHeader, messageBody) {
        header.text = messageHeader
        body.text = messageBody
        banner.open()
    }

    implicitWidth: banner.implicitWidth
    implicitHeight: banner.implicitHeight

    Popup {
        id: banner

        property alias color: background.color
        property alias borderColor: background.border.color

        // x: (parent.width - width) / 2

        width: Math.min(Window.window ? Window.window.width * 0.4 : 450, banner.implicitWidth)
        height: 65

        leftPadding: 0
        rightPadding: 15
        topPadding: 0
        bottomPadding: 0

        closePolicy: Popup.NoAutoClose

        background: Rectangle {
            id: background

            color: "#292223"
            border.color: "#582f30"
            // border.width: 2
            radius: 12
        }

        enter: Transition {
            NumberAnimation {
                property: "y";
                from: height
                to: -height - 25
                duration: 300;
                easing.type: Easing.OutBack
            }
        }

        exit: Transition {
            NumberAnimation {
                property: "y";
                from: -height - 25
                to: height
                duration: 300;
                easing.type: Easing.OutBack
            }
        }

        RowLayout {
            anchors.fill: parent

            spacing: 15

            Rectangle {
                Layout.preferredWidth: 4
                Layout.fillHeight: true
                Layout.topMargin: 14
                Layout.bottomMargin: 14
                Layout.alignment: Qt.AlignLeft

                radius: 2
                color: "#f05a5a"
            }

            Rectangle {
                // Layout.alignment: Qt.AlignTop

                Layout.preferredWidth: 30
                Layout.preferredHeight: 30

                color: "#41292a"
                radius: 30

                Image {
                    source: "qrc:/assets/circle_alert.svg"
                    sourceSize: Qt.size(20, 20)
                    anchors.centerIn: parent
                }

                // MultiEffect {
                //     source: iconImage

                //     anchors.fill: iconImage
                //     colorization: 1.0
                //     brightness: 1.0
                //     colorizationColor: "#0061ff"
                // }
            }

            ColumnLayout {
                // Layout.alignment: Qt.AlignVCenter

                spacing: 0

                Text {
                    id: header

                    Layout.fillWidth: true

                    font.pointSize: 10
                    font.bold: true

                    color: "#fff"
                    elide: Text.ElideRight
                }

                Text {
                    id: body

                    Layout.fillWidth: true
                    // Layout.fillHeight: true

                    color: "#888"
                    elide: Text.ElideRight
                }
            }

            Rectangle {
                id: okButton

                property string hoveredBackgroundColor: ""

                color: okButtonMouseArea.containsMouse ? "#41292a" : "#292223"
                radius: 8

                Layout.preferredWidth: 35
                Layout.preferredHeight: 30
                Layout.alignment: Qt.AlignVCenter

                Behavior on color {
                    ColorAnimation {
                        duration: 150
                        easing.type: Easing.OutCubic
                    }
                }

                Text {
                    id: okButtonText

                    text: qsTr("Ok")
                    font.pointSize: 10
                    font.bold: true
                    color: okButtonMouseArea.containsMouse ? "#fff" : "#ff7777"

                    anchors.verticalCenter: parent.verticalCenter
                    anchors.horizontalCenter: parent.horizontalCenter

                    Behavior on color {
                        ColorAnimation {
                            duration: 150
                            easing.type: Easing.OutCubic
                        }
                    }
                }

                MouseArea {
                    id: okButtonMouseArea

                    anchors.fill: parent
                    hoverEnabled: true
                    cursorShape: Qt.PointingHandCursor

                    onClicked: {
                        banner.close()
                    }
                }
            }
        }
    }
}
