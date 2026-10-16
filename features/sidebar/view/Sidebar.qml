import QtQuick
import QtQuick.Layouts
import QtQuick.Controls

import ParmigianoDesktop.FeatureProfile

Item {
    id: root

    Pane {
        anchors.fill: parent

        background: Rectangle {
            color: "#191a1c"
        }

        padding: 8
        // padding: 40

        // Rectangle {
        //     width: parent.width
        //     height: 10

        //     anchors.bottom: parent.bottom

        //     color: "#262728"
        // }

        ColumnLayout {
            // width: parent.width
            // height: parent.height
            anchors.fill: parent

            spacing: 8

            Rectangle {
                Layout.fillWidth: true
                Layout.fillHeight: true

                color: "transparent"
            }

            Rectangle {
                Layout.fillWidth: true
                Layout.preferredHeight: 1

                color: "#262728"
            }

            ProfileWidget {
                Layout.fillWidth: true
                Layout.preferredHeight: implicitHeight
            }
        }
    }
}
