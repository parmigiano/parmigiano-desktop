import QtQuick
import QtQuick.Controls

import ParmigianoDesktop as Logic
import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI

Window {
    id: mainWindow

    width: 1000
    height: 640
    minimumWidth: 1000
    minimumHeight: 640
    visible: true
    title: qsTr("Parmigiano Chat")
    color: "#191a1c"

    Component {
        id: authWindow

        AuthWindow {}
    }

    Component {
        id: createProfileWindow

        CreateProfileWindow {}
    }

    readonly property var pagesURI: {
        "AuthWindow": authWindow,
        "CreateProfileWindow": createProfileWindow
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

        function onNavigateBack(windowName) {
            if (!pagesURI.hasOwnProperty(pageName))
            {
                return
            }

            if (stackView.depth > 1) {
                stackView.pop()
            } else {
                exit.openManual()
            }
        }
    }

    StackView {
        id: stackView
        anchors.fill: parent
        initialItem: authWindow
    }
}
