import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Effects
import QtQuick.Window

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI

Item {
    id: root

    AuthViewModel {
        id: authViewModel
    }

    Connections {
        target: authViewModel

        function onDisplayMessage(messageHeader, messageBody) {
            errorBanner.show(messageHeader, messageBody)
        }

        function onNavigateToVerifyCode() {
            NavigationManager.goTo("VerifyCode")
        }

        function onNavigateToCreateProfile() {
            NavigationManager.goTo("CreateProfileWindow")
        }
    }

    Component {
        id: login

        LoginWidget {
            viewModel: authViewModel
        }
    }

    Component {
        id: verifyCode

        VerifyCodeWidget {
            viewModel: authViewModel
        }
    }

    readonly property var pagesURI: {
        "Login": login,
        "VerifyCode": verifyCode
    }

    Connections {
        target: NavigationManager

        function onNavigateTo(pageName) {
            if (!pagesURI.hasOwnProperty(pageName))
            {
                return
            }

            authPanePulse.restart()
            stackView.push(pagesURI[pageName])
        }

        function onNavigateBack() {
            if (stackView.depth > 1)
            {
                authPanePulse.restart()
                stackView.pop()
            }
            else
            {
                exit.openManual()
            }
        }
    }

    SequentialAnimation {
        id: authPanePulse

        NumberAnimation {
            target: authPane
            property: "scale"
            from: 1
            to: 0.980
            duration: 150

            easing.type: Easing.BezierSpline
            easing.bezierCurve: [0.25, 0.1, 0.25, 1.0, 1.0, 1.0]
        }

        NumberAnimation {
            target: authPane
            property: "scale"
            from: 0.980
            to: 1
            duration: 200

            easing.type: Easing.BezierSpline
            easing.bezierCurve: [0.25, 0.1, 0.25, 1.0, 1.0, 1.0]
        }
    }

    NetworkStatusBanner {
        id: networkBanner

        // Layout.alignment: Qt.AlignTop
        // Layout.fillWidth: true
        // Layout.preferredHeight: height
        // Layout.maximumHeight: 35

        height: implicitHeight
        width: parent.width
        anchors.top: parent.top


        visible: height > 0
    }

    Pane {
        anchors.fill: parent

        background: Rectangle {
            color: "#191a1c"
        }

        leftPadding: 25
        rightPadding: 25

        Item {
            property real blurAmount: 0

            id: content

            anchors.fill: parent

            layer.enabled: true

            layer.effect: MultiEffect {
                blurEnabled: true
                blur: content.blurAmount
                blurMax: 64
            }

            Behavior on blurAmount {
                NumberAnimation {
                    duration: 1000
                    easing.type: Easing.OutCubic
                }
            }

            ColumnLayout {
                width: parent.width
                height: 60
                spacing: 0

                // Layout.topMargin: 0
                Layout.margins: 0

                // NetworkStatusBanner {
                //     // id: networkBanner

                //     Layout.alignment: Qt.AlignTop
                //     Layout.fillWidth: true
                //     Layout.preferredHeight: height
                //     Layout.maximumHeight: 35

                //     visible: height > 0
                // }

                RowLayout {
                    Layout.fillWidth: true

                    // Layout.topMargin: 15
                    // Layout.leftMargin: 25
                    // Layout.rightMargin: 25

                    ArrowBack {
                        id: arrowBack

                        Layout.preferredWidth: 35
                        Layout.preferredHeight: 35

                        Layout.alignment: Qt.AlignVCenter

                        // Layout.alignment: Qt.AlignLeft | Qt.AlignTop | Qt.AlignHCenter
                        // Layout.margins: 15
                    }

                    Item {
                        Layout.fillWidth: true
                    }

                    Row {
                        // Layout.fillWidth: true

                        Layout.alignment: Qt.AlignRight
                        // Layout.margins: 15

                        spacing: 15

                        StepCounter {
                            id: stepCount

                            width: implicitWidth
                            height: implicitHeight

                            anchors.verticalCenter: parent.verticalCenter

                            current: stackView.currentItem ? stackView.currentItem.stepCount : 1
                            total: "2"
                        }

                        SwitchLanguageMenu {
                            id: langSwitcher

                            width: 45
                            height: 30

                            anchors.verticalCenter: parent.verticalCenter
                        }
                    }
                }
            }

            Pane {
                id: authPane

                width: 440
                // height: parent.height / 1.7
                height: implicitHeight
                // height: implicitHeight

                anchors.centerIn: parent

                transformOrigin: Item.Center

                clip: true

                background: Rectangle {
                    color: "#1e1f21"
                    border.color: "#2a2b2d"
                    border.width: 1
                    radius: 25
                }

                // leftPadding: 25
                // rightPadding: 25
                // topPadding: 25
                // bottomPadding: 15

                padding: 25

                Behavior on height {
                    NumberAnimation {
                        duration: 350
                        easing.type: Easing.OutCubic
                    }
                }

                ColumnLayout {
                    anchors.fill: parent

                    spacing: 35

                    Steps {
                        id: stepsView

                        Layout.fillWidth: true

                        currentStep: stackView.currentItem ? stackView.currentItem.authStep : 1
                    }

                    StackView {
                        id: stackView

                        Layout.fillWidth: true
                        Layout.preferredHeight: currentItem ? currentItem.implicitHeight : 0

                        // clip: false
                        initialItem: login

                        onBusyChanged: {
                            if (!busy && currentItem) {
                                currentItem.x = 0
                                currentItem.y = 0
                                currentItem.opacity = 1
                            }
                        }

                        pushEnter: Transition {
                            ParallelAnimation {
                                NumberAnimation {
                                    property: "x"
                                    from: stackView.width
                                    to: 0
                                    duration: 380
                                    easing.type: Easing.BezierSpline
                                    easing.bezierCurve: [0.4, 0.0, 0.2, 1.0, 1.0, 1.0]
                                }

                                NumberAnimation {
                                    property: "y"
                                    from: 8
                                    to: 0
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }

                                NumberAnimation {
                                    property: "opacity"
                                    from: 0
                                    to: 1
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }
                            }
                        }

                        pushExit: Transition {
                            ParallelAnimation {
                                NumberAnimation {
                                    property: "x"
                                    from: 0
                                    to: -stackView.width
                                    duration: 380
                                    easing.type: Easing.BezierSpline
                                    easing.bezierCurve: [0.4, 0.0, 0.2, 1.0, 1.0, 1.0]
                                }

                                NumberAnimation {
                                    property: "y"
                                    from: 0
                                    to: 8
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }

                                NumberAnimation {
                                    property: "opacity"
                                    from: 1
                                    to: 0
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }
                            }
                        }

                        popEnter: Transition {
                            ParallelAnimation {
                                NumberAnimation {
                                    property: "x"
                                    from: -stackView.width
                                    to: 0
                                    duration: 380
                                    easing.type: Easing.BezierSpline
                                    easing.bezierCurve: [0.4, 0.0, 0.2, 1.0, 1.0, 1.0]
                                }

                                NumberAnimation {
                                    property: "y"
                                    from: 8
                                    to: 0
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }

                                NumberAnimation {
                                    property: "opacity"
                                    from: 0
                                    to: 1
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }
                            }
                        }

                        popExit: Transition {
                            ParallelAnimation {
                                NumberAnimation {
                                    property: "x"
                                    from: 0
                                    to: stackView.width
                                    duration: 380
                                    easing.type: Easing.BezierSpline
                                    easing.bezierCurve: [0.4, 0.0, 0.2, 1.0, 1.0, 1.0]
                                }

                                NumberAnimation {
                                    property: "y"
                                    from: 0
                                    to: 8
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }

                                NumberAnimation {
                                    property: "opacity"
                                    from: 1
                                    to: 0
                                    duration: 240
                                    easing.type: Easing.OutCubic
                                }
                            }
                        }
                    }

                    ButtonConfirm {
                        Layout.fillWidth: true
                        Layout.preferredHeight: 50

                        buttonTextDefault: stackView.currentItem ? stackView.currentItem.buttonText : ""
                        buttonTextLoading: qsTr("LOADING")

                        mouseArea.onClicked: {
                            const page = stackView.currentItem

                            if (page)
                            {
                                page.submit()
                            }
                        }
                    }
                }
            }
        }

        ExitDialog {
            id: exit

            anchors.fill: parent

            content: content

            // blur.sourceItem: content
            // contentSource.sourceItem: root
        }

        ErrorBanner {
            id: errorBanner

            width: implicitWidth
            height: implicitHeight

            anchors.bottom: parent.bottom
            anchors.horizontalCenter: parent.horizontalCenter

            // backgroungColor: "#fff9db"
            // borderColor: "#ffe066"
            // textColor: "#856404"
            // buttonBackgroundColor: "#ebdca5"
            // buttonTextColor: "#856404"
        }
    }
}
