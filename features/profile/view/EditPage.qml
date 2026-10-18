import QtQuick
import QtQuick.Layouts

import ParmigianoDesktop.SharedUI
import ParmigianoDesktop.FeatureProfile

ColumnLayout {
    id: root

    // anchors.fill: parent

    spacing: 22

    ColumnLayout {
        Layout.alignment: Qt.AlignHCenter

        Rectangle {
            Layout.preferredWidth: 90
            Layout.preferredHeight: 90
            Layout.alignment: Qt.AlignHCenter

            radius: 100
            color: "red"
        }

        Text {
            Layout.alignment: Qt.AlignHCenter

            text: qsTrId("Add photo")
            color: "#909195"

            font.pointSize: 10
        }
    }

    ColumnLayout {
        // Rectangle {
        //     Layout.fillWidth: true
        //     Layout.preferredHeight: 10

        //     color: "red"
        // }

        spacing: 15

        InputField {
            Layout.fillWidth: true
            Layout.preferredHeight: 60

            field.maximumLength: 24
            labelText: qsTrId("Name")
        }

        InputField {
            Layout.fillWidth: true
            Layout.preferredHeight: 60

            field.maximumLength: 24
            labelText: qsTrId("Username")
        }

        InputField {
            Layout.fillWidth: true
            Layout.preferredHeight: 60

            field.maximumLength: 24
            labelText: qsTrId("About")
        }
    }
}
