import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

import ParmigianoDesktop.SharedUI

FieldFrame {
    id: root

    property alias field: field
    property string labelText: ""
    property bool hasError: false

    hovered: field.hovered
    focused: field.activeFocus
    error: hasError

    function error(isError, message) {
        root.hasError = isError
        // root.errorMessage = isError ? message : ""
    }

    ColumnLayout {
        anchors.fill: parent
        // Layout.fillWidth: true
        // Layout.fillHeight: true

        Item {
            Layout.fillHeight: true
        }

        Text {
            id: label

            // Layout.fillHeight: true
            // Layout.alignment: Qt.AlignVCenter

            leftPadding: 15
            // anchors.fill: parent
            // verticalAlignment: Text.AlignVCenter
            // z: 1
            // y: (root.height - label.height) / 3

            text: labelText
            font.pointSize: 10
            color: "#909195"

            Behavior on color {
                ColorAnimation {
                    duration: 120
                }
            }

            // Rectangle {
            //     anchors.fill: parent
            //     color: "#181818"
            //     z: -1
            // }
        }

        TextField {
            id: field

            Layout.fillWidth: true
            // Layout.fillHeight: true
            // Layout.alignment: Qt.AlignVCenter

            verticalAlignment: Qt.AlignVCenter
            leftPadding: 15
            placeholderTextColor: "#909195"
            color: "white"
            font.pointSize: 11
            selectionColor: "#053ba7"

            background: null

            // background: FieldFrame {
            //     hovered: root.hovered
            //     focused: root.activeFocus
            //     // error: field.hasError
            // }

            TapHandler {
                acceptedButtons: Qt.RightButton
                onTapped: contextMenu.menu.popup()
            }

            TextContextMenu {
                id: contextMenu

                target: root
            }

            onTextEdited: root.error(false, "")
        }

        Item {
            Layout.fillHeight: true
        }
    }
}


