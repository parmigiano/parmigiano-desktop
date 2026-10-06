import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Effects
import QtQuick.Window

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI

Item {
    id: root

    Rectangle {
        anchors.fill: parent

        RowLayout {
            anchors.fill: parent
            spacing: 0

            // Sidebar {
            //     Layout.preferredWidth: Math.min(Window.window ? Window.window.width * 0.3 : 300, 270)
            //     Layout.fillHeight: true
            // }

            Rectangle {
                Layout.preferredWidth: Window.width * 0.28
                Layout.fillHeight: true

                color: "#191a1c"
            }

            Rectangle {
                Layout.preferredWidth: 1
                Layout.fillHeight: true

                color: "#262728"
            }

            Rectangle {
                Layout.fillWidth: true
                Layout.fillHeight: true

                color: "#191a1c"

                StackView {
                    id: stackView

                    anchors.fill: parent

                    // initialItem: AboutPage {}
                }
            }
        }
    }
}
