import QtQuick

import ParmigianoDesktop.CoreUI

Item {
    id: root

    height: networkBanner.height

    Timer {
        id: hideBanner
        interval: 2000
        running: false
        repeat: false

        onTriggered: {
            networkBanner.bannerState = "hidden"
        }
    }

    Connections {
        target: UIStateManager

        function onNetworkStatusChanged(status) {
            hideBanner.stop();

            let nextState = status ? "success" : "waiting";

            if (networkBanner.bannerState !== nextState)
            {
                networkBanner.bannerState = nextState;
                statusSwap.restart();
            }

            if (status)
            {
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
        color: "#242424"
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
                    color: "#222325"
                    height: 35
                }

                PropertyChanges {
                    target: statusImage
                    source: "qrc:/assets/no_internet.svg"
                }

                PropertyChanges {
                    target: statusText
                    text: qsTr("No internet")
                }
            },

            State {
                name: "success"
                PropertyChanges {
                    target: networkBanner
                    color: "#222325"
                    height: 35
                }

                PropertyChanges {
                    target: statusImage
                    source: "qrc:/assets/internet_restored.svg"
                }

                PropertyChanges {
                    target: statusText
                    text: qsTr("Internet restored")
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
    }
}
