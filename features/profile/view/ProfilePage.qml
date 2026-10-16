import QtQuick
import QtQuick.Layouts
import QtQuick.Controls

import ParmigianoDesktop.CoreUI

ColumnLayout {
    id: root

    // anchors.fill: parent

    spacing: 22

    ColumnLayout {
        Layout.alignment: Qt.AlignHCenter

        Rectangle {
            Layout.preferredWidth: 90
            Layout.preferredHeight: 90
            Layout.alignment: Qt.AlignHCenter

            radius: 100
            color: "red"
        }

        Text {
            Layout.fillWidth: true
            Layout.maximumWidth: profileDialog.width * 0.8
            horizontalAlignment: Qt.AlignHCenter

            text: qsTrId("Flu")
            color: "#eeeeef"

            font.pointSize: 16

            elide: Text.ElideRight
        }

        Text {
            Layout.alignment: Qt.AlignHCenter

            text: qsTrId("online")
            color: "#a0cafd"

            font.pointSize: 10
        }
    }

    RowLayout {
        Layout.fillWidth: true

        spacing: 8

        Rectangle {
            Layout.fillWidth: true
            Layout.preferredHeight: 54

            radius: 8
            color: editArea.containsMouse ? "#292a2c" : "#242527"

            Behavior on color {
                ColorAnimation {
                    duration: 100
                    easing.type: Easing.InOutQuad
                }
            }

            ColumnLayout {
                anchors.centerIn: parent

                Image {
                    Layout.alignment: Qt.AlignCenter
                    sourceSize: Qt.size(12, 12)
                    source: "qrc:/assets/titlebar_close.svg"
                }

                Text {
                    Layout.alignment: Qt.AlignHCenter

                    text: qsTrId("Edit")
                    // color: "#a0cafd"
                    color: editArea.containsMouse ? "#fff" : "#a1a2a5"

                    Behavior on color {
                        ColorAnimation {
                            duration: 100
                            easing.type: Easing.InOutQuad
                        }
                    }

                    font.pointSize: 10
                }
            }

            MouseArea {
                id: editArea

                anchors.fill: parent
                hoverEnabled: true
                cursorShape: Qt.PointingHandCursor

                onClicked: {
                    NavigationManager.goTo("EditPage")
                }
            }
        }

        Rectangle {
            Layout.fillWidth: true
            Layout.preferredHeight: 54

            radius: 8
            color: favouritesArea.containsMouse ? "#292a2c" : "#242527"

            Behavior on color {
                ColorAnimation {
                    duration: 100
                    easing.type: Easing.InOutQuad
                }
            }

            ColumnLayout {
                anchors.centerIn: parent

                Image {
                    Layout.alignment: Qt.AlignCenter
                    sourceSize: Qt.size(12, 12)
                    source: "qrc:/assets/titlebar_close.svg"
                }

                Text {
                    Layout.alignment: Qt.AlignHCenter

                    text: qsTrId("Favourites")
                    // color: "#a0cafd"
                    color: favouritesArea.containsMouse ? "#fff" : "#a1a2a5"

                    Behavior on color {
                        ColorAnimation {
                            duration: 100
                            easing.type: Easing.InOutQuad
                        }
                    }

                    font.pointSize: 10
                }
            }

            MouseArea {
                id: favouritesArea

                anchors.fill: parent
                hoverEnabled: true
                cursorShape: Qt.PointingHandCursor
            }
        }
    }

    ColumnLayout {
        spacing: 12

        ColumnLayout {
            spacing: 0

            Text {
                Layout.alignment: Qt.AlignLeft

                text: qsTrId("Email")
                color: "#909195"
                font.pointSize: 10
            }

            Text {
                Layout.fillWidth: true
                Layout.alignment: Qt.AlignLeft

                text: "flu@gmail.com"
                color: "#eeeeef"
                font.pointSize: 11

                elide: Text.ElideRight
            }
        }

        ColumnLayout {
            spacing: 0

            Text {
                Layout.alignment: Qt.AlignLeft

                text: qsTrId("Username")
                color: "#909195"
                font.pointSize: 10
            }

            Text {
                Layout.fillWidth: true
                Layout.alignment: Qt.AlignLeft

                text: "@flu"
                color: "#eeeeef"
                font.pointSize: 11

                elide: Text.ElideRight
            }
        }

        ColumnLayout {
            spacing: 0

            Text {
                Layout.alignment: Qt.AlignLeft

                text: qsTrId("About")
                color: "#909195"
                font.pointSize: 10
            }

            Text {
                Layout.fillWidth: true
                Layout.alignment: Qt.AlignLeft

                text: "Parmigiano Chat Developer"
                color: "#eeeeef"
                font.pointSize: 11

                wrapMode: Text.Wrap
                maximumLineCount: 3
                elide: Text.ElideRight
            }
        }
    }

    // Rectangle {
    //     Layout.fillWidth: true
    //     Layout.preferredHeight: 40

    //     color: logOutArea.containsMouse ? "#2a1f21" : "#191a1c"
    //     radius: 8

    //     Row {
    //         anchors.fill: parent
    //         anchors.leftMargin: 8
    //         // anchors.verticalCenter: parent.verticalCenter

    //         spacing: 8

    //         Image {
    //             // Layout.alignment: Qt.AlignVCenter
    //             anchors.verticalCenter: parent.verticalCenter
    //             sourceSize: Qt.size(20, 20)
    //             source: "qrc:/assets/logout_icon.svg"
    //         }

    //         Text {
    //             // verticalAlignment: Qt.AlignVCenter
    //             anchors.verticalCenter: parent.verticalCenter

    //             text: qsTrId("Log out")
    //             color: "#f05a5a"
    //             font.pointSize: 12
    //         }
    //     }

    //     MouseArea {
    //         id: logOutArea

    //         anchors.fill: parent
    //         hoverEnabled: true
    //         cursorShape: Qt.PointingHandCursor
    //     }
    // }
}
