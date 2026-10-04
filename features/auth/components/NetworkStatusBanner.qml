import QtQuick

import ParmigianoDesktop.CoreUI

Item {
    id: root

    property real expandedHeight: 35

    height: networkBanner.height

    Timer {
        id: hideBanner
        interval: 2000
        running: false
        repeat: false

        onTriggered: {
            statusSwap.stop();
            statusHide.restart();
        }
    }

    Connections {
        target: UIStateManager

        function onNetworkStatusChanged(status) {
            hideBanner.stop();
            statusHide.stop();
            statusSwap.stop();

            networkBanner.bannerState = status ? "success" : "waiting";
            statusSwap.restart();

            if (status) {
                hideBanner.restart();
            }
        }
    }

    Rectangle {
        property string bannerState: definitionState() // success/waiting/hidden

        id: networkBanner

        width: parent.width
        height: 0
        clip: true
        color: "transparent"
        state: bannerState

        states: [
            State {
                name: "hidden"
                PropertyChanges {
                    target: networkBanner
                    height: 0
                }

                PropertyChanges {
                    target: statusText
                    text: qsTr("")
                }
            },

            State {
                name: "waiting"
                PropertyChanges {
                    target: networkBanner
                    color: "transparent"
                    height: root.expandedHeight
                }

                PropertyChanges {
                    target: statusImage
                    source: "qrc:/assets/no_internet.svg"
                }

                PropertyChanges {
                    target: statusText
                    text: qsTrId("network.offline")
                }
            },

            State {
                name: "success"
                PropertyChanges {
                    target: networkBanner
                    color: "transparent"
                    height: root.expandedHeight
                }

                PropertyChanges {
                    target: statusImage
                    source: "qrc:/assets/internet_restored.svg"
                }

                PropertyChanges {
                    target: statusText
                    text: qsTrId("network.restored")
                }
            }
        ]

        function definitionState() {
            let status = UIStateManager.getNetworkStatus();

            if(!status)
            {
                return "waiting";
            }
            else
            {
                return "hidden";
            }
        }

        Behavior on height {
            NumberAnimation {
                duration: 250
                easing.type: Easing.InOutQuad
            }
        }

        Behavior on color {
            ColorAnimation {
                duration: 300
            }
        }

        Row {
            id: statusRow

            anchors.centerIn: parent
            spacing: 8

            transform: Translate {
                id: statusOffset

                y: 0
            }

            Image {
                id: statusImage

                sourceSize: Qt.size(15, 15)
            }

            Text {
                id: statusText

                text: ""
                color: "white"
                font.pixelSize: 12
                font.bold: true
            }
        }

        ParallelAnimation {
            id: statusSwap

            NumberAnimation {
                target: statusRow
                property: "opacity"
                from: 0
                to: 1
                duration: 260
                easing.type: Easing.BezierSpline
                easing.bezierCurve: [0, 0, 0.58, 1, 1, 1]
            }

            NumberAnimation {
                target: statusOffset
                property: "y"
                from: 6
                to: 0
                duration: 260
                easing.type: Easing.BezierSpline
                easing.bezierCurve: [0, 0, 0.58, 1, 1, 1]
            }
        }

        ParallelAnimation {
            id: statusHide

            NumberAnimation {
                target: statusRow
                property: "opacity"
                to: 0
                duration: 260
                easing.type: Easing.InCubic
            }

            NumberAnimation {
                target: statusOffset
                property: "y"
                to: -6
                duration: 260
                easing.type: Easing.InCubic
            }

            onFinished: {
                networkBanner.bannerState = "hidden";
            }
        }
    }
}
