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

            // authPanePulse.restart()
            stackView.push(pagesURI[pageName])
        }

        function onNavigateBack() {
            if (stackView.depth > 1)
            {
                // authPanePulse.restart()
                stackView.pop()
            }
            else
            {
                exit.openManual()
            }
        }
    }

    // SequentialAnimation {
    //     id: authPanePulse

    //     NumberAnimation {
    //         target: authPane
    //         property: "scale"
    //         from: 1
    //         to: 0.980
    //         duration: 150

    //         easing.type: Easing.BezierSpline
    //         easing.bezierCurve: [0.25, 0.1, 0.25, 1.0, 1.0, 1.0]
    //     }

    //     NumberAnimation {
    //         target: authPane
    //         property: "scale"
    //         from: 0.980
    //         to: 1
    //         duration: 200

    //         easing.type: Easing.BezierSpline
    //         easing.bezierCurve: [0.25, 0.1, 0.25, 1.0, 1.0, 1.0]
    //     }
    // }

    Pane {
        anchors.fill: parent

        background: Rectangle {
            color: "#191a1c"
        }

        padding: 0

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

                NetworkStatusBanner {
                    // id: networkBanner

                    Layout.alignment: Qt.AlignTop
                    Layout.fillWidth: true
                    Layout.preferredHeight: height
                    Layout.maximumHeight: 35

                    visible: height > 0
                }

                RowLayout {
                    Layout.fillWidth: true

                    Layout.topMargin: 15
                    Layout.leftMargin: 25
                    Layout.rightMargin: 25

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

                        // StepCounter {
                        //     id: stepCount

                        //     width: implicitWidth
                        //     height: implicitHeight

                        //     anchors.verticalCenter: parent.verticalCenter

                        //     current: stackView.currentItem ? stackView.currentItem.stepCount : 1
                        //     total: "2"
                        // }

                        SwitchLanguageMenu {
                            id: langSwitcher

                            width: 45
                            height: 30

                            anchors.verticalCenter: parent.verticalCenter
                        }
                    }
                }
            }

            ColumnLayout {
                width: 360

                anchors.centerIn: parent

                spacing: stackView.currentItem ? stackView.currentItem.gap : 15

                StackView {
                    id: stackView

                    Layout.fillWidth: true
                    Layout.preferredHeight: currentItem ? currentItem.implicitHeight : 0

                    initialItem: login

                    pushEnter: Transition {
                        NumberAnimation {
                            property: "opacity"
                            from: 0
                            to: 1
                            duration: 240
                            easing.type: Easing.BezierSpline
                            easing.bezierCurve: [0.25, 0.1, 0.25, 1, 1, 1]
                        }

                        NumberAnimation {
                            property: "y"
                            from: 8
                            to: 0
                            duration: 240
                            easing.type: Easing.BezierSpline
                            easing.bezierCurve: [0.25, 0.1, 0.25, 1, 1, 1]
                        }
                    }

                    pushExit: Transition {
                        PropertyAction {
                            property: "opacity"
                            value: 0
                        }
                    }

                    popEnter: stackView.pushEnter
                    popExit: stackView.pushExit

                    replaceEnter: stackView.pushEnter
                    replaceExit: stackView.pushExit
                }

                Action {
                    id: clickButtonAction

                    onTriggered: {
                        const page = stackView.currentItem

                        if (page)
                        {
                            page.submit()
                        }
                    }
                }

                Shortcut {
                    sequence: "Return" // normal human enter
                    onActivated: {
                        clickButtonAction.trigger();
                    }
                }

                Shortcut {
                    sequence: "Enter" // NumPad enter
                    onActivated: {
                        clickButtonAction.trigger();
                    }
                }

                ButtonConfirm {
                    Layout.fillWidth: true
                    Layout.preferredHeight: 55

                    buttonTextDefault: stackView.currentItem ? stackView.currentItem.buttonText : ""
                    buttonTextLoading: qsTr("LOADING")

                    mouseArea.onClicked: {
                        clickButtonAction.trigger();
                    }
                }
            }
        }

        ExitDialog {
            id: exit

            anchors.fill: parent

            content: content
        }

        ErrorBanner {
            id: errorBanner

            width: implicitWidth
            height: implicitHeight

            anchors.bottom: parent.bottom
            anchors.horizontalCenter: parent.horizontalCenter
        }
    }
}
