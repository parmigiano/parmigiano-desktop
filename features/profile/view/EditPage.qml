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

        spacing: 12

        ProfileInput {
            Layout.fillWidth: true
            Layout.preferredHeight: 56

            maximumLength: 24
            labelText: qsTrId("Name")
        }

        ProfileInput {
            Layout.fillWidth: true
            Layout.preferredHeight: 56

            maximumLength: 24
            labelText: qsTrId("Username")
        }

        ProfileInput {
            id: about
            Layout.fillWidth: true
            Layout.preferredHeight: 90
            // Layout.preferredHeight: field.height + 56

            multiline: true
            maximumLength: 70
            labelText: qsTrId("About")

            Text {
                anchors.right: parent.right
                anchors.rightMargin: 14
                anchors.bottom: parent.bottom
                anchors.bottomMargin: 9

                text: about.field.length + "/" + about.maximumLength
                color: "#909195"
                font.pointSize: 8
            }
        }
    }

    ButtonConfirm {
        Layout.fillWidth: true
        Layout.preferredHeight: 50

        buttonTextDefault: qsTrId("Save")
        buttonTextLoading: qsTrId("auth.loading")

        mouseArea.onClicked: {
            // clickButtonAction.trigger();
        }
    }
}
