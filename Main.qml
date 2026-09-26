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

            stackView.push(pagesURI[pageName])
        }

        // function onNavigateBack(windowName) {
        //     if (stackView.depth > 1) {
        //         authPanePulse.restart()
        //         stackView.pop()
        //     } else {
        //         exit.openManual()
        //     }
        // }
    }

    StackView {
        id: stackView
        anchors.fill: parent
        initialItem: authWindow
    }
}
