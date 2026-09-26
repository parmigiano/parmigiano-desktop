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

    // UIStateManager {
    //     id: stateManager
    // }

    Connections {
        target: UIStateManager

        function onNetworkStatusChanged(status) {
            if (status)
            {
                networkBanner.bannerState = "success";
                hideBanner.start();
            }
            else
            {
                networkBanner.bannerState = "waiting";
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
                    target: statusDot
                    visible: true
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
                    target: statusDot
                    visible: true
                }

                PropertyChanges {
                    target: statusText
                    text: qsTr("Waiting...")
                }
            },

            State {
                name: "success"
                PropertyChanges {
                    target: networkBanner
                    color: "#469551"
                    height: 35
                }

                PropertyChanges {
                    target: statusDot
                    visible: false
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
            anchors.centerIn: parent
            spacing: 8

            Rectangle {
                id: statusDot

                width: 6
                height: 6
                radius: 3
                color: "#708499"
                anchors.verticalCenter: parent.verticalCenter
                visible: false

                SequentialAnimation on opacity {
                    loops: Animation.Infinite

                    NumberAnimation {
                        from: 0.3
                        to: 1.0
                        duration: 750
                    }

                    NumberAnimation {
                        from: 1.0
                        to: 0.3
                        duration: 750
                    }
                }
            }

            Text {
                id: statusText

                text: ""
                color: "white"
                font.pixelSize: 12
                font.bold: true
            }
        }
    }
}
