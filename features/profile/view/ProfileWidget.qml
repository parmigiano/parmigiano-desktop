import QtQuick
import QtQuick.Layouts

import ParmigianoDesktop.FeatureProfile

Item {
    id: root

    implicitHeight: profileRow.implicitHeight

    ProfileViewModel {
        id: viewModel
    }

    Rectangle {
        anchors.fill: parent

        radius: 10
        color: profileArea.containsMouse ? "#1e2022" : "#191a1c"

        ProfileDialog {
            id: profileDialog
        }

        Behavior on color {
            ColorAnimation {
                duration: 100
                easing.type: Easing.InOutQuad
            }
        }

        RowLayout {
            id: profileRow

            anchors.fill: parent

            spacing: 10

            Rectangle {
                Layout.preferredWidth: 40
                Layout.preferredHeight: 40
                Layout.topMargin: 6
                Layout.bottomMargin: 6
                Layout.leftMargin: 10

                radius: 100
                color: "red"
            }

            ColumnLayout {
                Layout.fillWidth: true

                Layout.topMargin: 6
                Layout.bottomMargin: 6

                Text {
                    Layout.fillWidth: true
                    Layout.alignment: Qt.AlignLeft

                    text: qsTrId("Flu")
                    color: "#fff"

                    font.pointSize: 10

                    elide: Text.ElideRight
                }

                Text {
                    Layout.fillWidth: true
                    Layout.alignment: Qt.AlignLeft

                    text: qsTrId("flu@gmail.com")
                    color: "#909195"

                    font.pointSize: 9

                    elide: Text.ElideRight
                }
            }

            Image {
                Layout.alignment: Qt.AlignVCenter
                Layout.topMargin: 6
                Layout.bottomMargin: 6
                Layout.rightMargin: 10

                sourceSize: Qt.size(16, 16)
                source: "qrc:/assets/language_chevron.svg"

                rotation: -90
            }
        }

        MouseArea {
            id: profileArea

            anchors.fill: parent

            hoverEnabled: true
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                profileDialog.dialog.open()
            }
        }
    }
}
