import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Window

import ParmigianoDesktop.FeatureProfile
import ParmigianoDesktop.CoreUI

Item {
    id: root

    property alias dialog: profileDialog

    Component {
        id: profilePage

        ProfilePage {

        }
    }

    Component {
        id: editPage

        EditPage {

        }
    }

    readonly property var pagesURI: {
        "ProfilePage": profilePage,
        "EditPage": editPage
    }

    Connections {
        target: NavigationManager

        function onNavigateTo(pageName) {
            if (!pagesURI.hasOwnProperty(pageName))
            {
                return
            }

            stackView.replace(pagesURI[pageName])
        }

        function onNavigateBack() {
            if (stackView.depth > 1)
            {
                stackView.pop()
            }
            else
            {
                // exit.openManual()
            }
        }
    }

    Popup {
        id: profileDialog

        parent: Overlay.overlay

        width: 350
        // height: implicitHeight

        x: (parent.width - width) / 2
        y: (parent.height - height) / 2

        padding: 15

        background: Rectangle {
            radius: 14
            color: "#191a1c"
            border.color: "#292a2c"
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
                // NumberAnimation {
                //     property: "y"
                //     from: (parent.height - parent.height * 0.6) / 2
                //     to: (parent.height - parent.height * 0.6) / 2
                //     duration: 200

                //     easing.type: Easing.BezierSpline
                //     // easing.bezierCurve: [0.34, 1.56, 0.64, 1, 1, 1]
                // }
            }
        }

        exit: Transition {
            ParallelAnimation {
                NumberAnimation {
                    property: "opacity"
                    from: 1.0
                    to: 0.0
                    duration: 100
                    easing.type: Easing.InQuad
                }

                // NumberAnimation {
                //     property: "y"
                //     from: (parent.height - profileDialog.height) / 2
                //     to: (parent.height - profileDialog.height) / 2 - 20
                //     duration: 200

                //     easing.type: Easing.BezierSpline
                //     // easing.bezierCurve: [0.34, 1.56, 0.64, 1, 1, 1]
                // }
            }
        }

        onAboutToShow: {
            stackView.replace(profilePage)
        }

        ColumnLayout {
            anchors.fill: parent

            spacing: 0

            Rectangle {
                Layout.preferredWidth: 36
                Layout.preferredHeight: 36
                Layout.alignment: Qt.AlignRight

                radius: 10
                color: closeDialogArea.containsMouse ? "#272729" : "#191a1c"

                Image {
                    anchors.centerIn: parent

                    sourceSize: Qt.size(12, 12)
                    source: "qrc:/assets/titlebar_close.svg"
                }

                MouseArea {
                    id: closeDialogArea

                    anchors.fill: parent
                    hoverEnabled: true
                    cursorShape: Qt.PointingHandCursor

                    onClicked: profileDialog.close()
                }

                Behavior on color {
                    ColorAnimation {
                        duration: 50
                    }
                }
            }

            StackView {
                id: stackView

                Layout.fillWidth: true
                Layout.preferredHeight: currentItem ? currentItem.implicitHeight : 0

                initialItem: profilePage

                pushEnter: Transition {
                    NumberAnimation {
                        property: "opacity";
                        from: 0;
                        to: 1;
                        duration: 150;
                        easing.type: Easing.OutQuad
                    }
                }

                pushExit: Transition {
                    PropertyAction {
                        property: "opacity";
                        value: 0
                    }
                }

                popEnter: Transition {
                    NumberAnimation {
                        property: "opacity";
                        from: 0;
                        to: 1;
                        duration: 150;
                        easing.type: Easing.OutQuad
                    }
                }

                popExit: Transition {
                    PropertyAction {
                        property: "opacity";
                        value: 0
                    }
                }

                replaceEnter: Transition {
                    NumberAnimation {
                        property: "opacity";
                        from: 0;
                        to: 1;
                        duration: 150;
                        easing.type: Easing.OutQuad
                    }
                }

                replaceExit: Transition {
                    PropertyAction {
                        property: "opacity";
                        value: 0
                    }
                }
            }
        }
    }

    Rectangle {
        parent: Overlay.overlay
        anchors.fill: parent
        color: "black"

        opacity: profileDialog.visible ? 0.3 : 0.0
        visible: opacity > 0

        Behavior on opacity {
            NumberAnimation {
                duration: 200
            }
        }
    }
}
