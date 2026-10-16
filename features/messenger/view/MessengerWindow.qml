import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Effects
import QtQuick.Window

import ParmigianoDesktop.FeatureSidebar

Item {
    id: root

    Rectangle {
        anchors.fill: parent

        RowLayout {
            anchors.fill: parent
            spacing: 0

            Rectangle {
                Layout.preferredWidth: Math.min(Window.width * 0.28, 400)
                Layout.fillHeight: true

                Sidebar {
                    anchors.fill: parent
                }
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
