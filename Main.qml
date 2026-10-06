import QtQuick
import QtQuick.Layouts
import QtQuick.Controls

import ParmigianoDesktop as Logic
import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.FeatureMessenger
import ParmigianoDesktop.CoreUI

Window {
    id: mainWindow

    width: 1000
    height: 640
    minimumWidth: 1000
    minimumHeight: 640
    visible: true

    flags: Qt.FramelessWindowHint | Qt.Window

    Component {
        id: authWindow

        AuthWindow {}
    }

    Component {
        id: messengerWindow

        MessengerWindow {}
    }

    readonly property var pagesURI: {
        "AuthWindow": authWindow,
        "MessengerWindow": messengerWindow
    }

    Connections {
        target: NavigationManager

        function onNavigateTo(pageName) {
            if (!pagesURI.hasOwnProperty(pageName))
            {
                return
            }

            stackView.replace(pagesURI[pageName], StackView.Immediate)
        }

        // function onNavigateBack(windowName) {
        //     if (!pagesURI.hasOwnProperty(pageName))
        //     {
        //         return
        //     }

        //     if (stackView.depth > 1) {
        //         stackView.pop()
        //     } else {
        //         exit.openManual()
        //     }
        // }
    }

    ColumnLayout {
        anchors.fill: parent

        spacing: 0

        Rectangle {
            Layout.fillWidth: true
            Layout.preferredHeight: 30

            Layout.margins: 0

            color: "#191a1c"

            DragHandler {
                onActiveChanged: if (active) mainWindow.startSystemMove();
                target: null
            }

            NetworkStatusBanner {
                height: parent.height

                anchors.top: parent.top
                anchors.left: parent.left
                anchors.right: parent.right

                expandedHeight: parent.height
            }

            // Text {
            //     anchors.verticalCenter: parent.verticalCenter
            //     leftPadding: 10

            //     text: qsTrId("app.title")
            //     color: "#a1a2a5"

            //     font.pointSize: 10
            // }

            Row {
                anchors.right: parent.right
                anchors.top: parent.top
                anchors.bottom: parent.bottom

                spacing: 0

                Rectangle {
                    width: 45
                    height: parent.height

                    color: minimizeMouseArea.containsMouse ? "#272729" : "#191a1c"

                    Image {
                        anchors.centerIn: parent

                        sourceSize: Qt.size(10, 10)
                        source: "qrc:/assets/titlebar_minimize.svg"

                        visible: true
                    }

                    MouseArea {
                        id: minimizeMouseArea

                        anchors.fill: parent
                        hoverEnabled: true
                        cursorShape: Qt.PointingHandCursor

                        onClicked: mainWindow.showMinimized()
                    }

                    Behavior on color {
                        ColorAnimation {
                            duration: 50
                        }
                    }
                }

                Rectangle {
                    width: 45
                    height: parent.height

                    color: maximizedMouseArea.containsMouse ? "#272729" : "#191a1c"

                    Image {
                        id: maximizedImage

                        anchors.centerIn: parent

                        sourceSize: Qt.size(10, 10)
                        source: mainWindow.visibility === Window.Maximized ? "qrc:/assets/titlebar_restore.svg" : "qrc:/assets/titlebar_maximize.svg"

                        visible: true
                    }

                    MouseArea {
                        id: maximizedMouseArea

                        anchors.fill: parent
                        hoverEnabled: true
                        cursorShape: Qt.PointingHandCursor

                        onClicked: mainWindow.visibility === Window.Maximized ? mainWindow.showNormal() : mainWindow.showMaximized()
                    }

                    Behavior on color {
                        ColorAnimation {
                            duration: 50
                        }
                    }
                }

                Rectangle {
                    width: 45
                    height: parent.height

                    color: closeMouseArea.containsMouse ? "#c42b1c" : "#191a1c"

                    Image {
                        anchors.centerIn: parent

                        sourceSize: Qt.size(10, 10)
                        source: "qrc:/assets/titlebar_close.svg"

                        visible: true
                    }

                    MouseArea {
                        id: closeMouseArea

                        anchors.fill: parent
                        hoverEnabled: true
                        cursorShape: Qt.PointingHandCursor

                        onClicked: mainWindow.close()
                    }

                    Behavior on color {
                        ColorAnimation {
                            duration: 50
                        }
                    }
                }
            }
        }

        Rectangle {
            Layout.fillWidth: true
            Layout.fillHeight: true

            StackView {
                id: stackView
                anchors.fill: parent
                initialItem: authWindow
                // initialItem: messengerWindow
            }
        }
    }
}
