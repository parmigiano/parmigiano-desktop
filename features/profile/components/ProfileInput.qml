import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

import ParmigianoDesktop.SharedUI

FieldFrame {
    id: root

    property string labelText: ""
    property string text: ""
    property string placeholderText: ""
    property bool multiline: false
    property int maximumLength: 0
    property bool hasError: false
    readonly property Item field: loader.item

    hovered: field ? field.hovered : false
    focused: field ? field.activeFocus : false
    error: hasError

    implicitHeight: multiline ? 92 : 58

    function error(isError, message) {
        root.hasError = isError
        // root.errorMessage = isError ? message : ""
    }

    ColumnLayout {
        anchors.fill: parent
        anchors.topMargin: root.multiline ? 9 : 0
        spacing: 2

        Item {
            visible: !root.multiline
            Layout.fillHeight: true
        }

        Text {
            id: label

            leftPadding: 15
            text: root.labelText
            font.pointSize: 10
            color: "#909195"

            Behavior on color {
                ColorAnimation {
                    duration: 120
                }
            }
        }

        Loader {
            id: loader

            Layout.fillWidth: true
            Layout.fillHeight: root.multiline
            sourceComponent: root.multiline ? areaComponent : lineComponent
        }

        Item {
            visible: !root.multiline
            Layout.fillHeight: true
        }
    }

    Component {
        id: lineComponent

        TextField {
            id: line

            text: root.text
            onTextChanged: root.text = text
            placeholderText: root.placeholderText
            maximumLength: root.maximumLength > 0 ? root.maximumLength : 32767

            topPadding: 0
            bottomPadding: 0
            leftPadding: 15
            placeholderTextColor: "#909195"
            color: "white"
            font.pointSize: 11
            selectionColor: "#053ba7"
            background: null

            TapHandler {
                acceptedButtons: Qt.RightButton
                onTapped: lineMenu.menu.popup()
            }

            TextContextMenu {
                id: lineMenu

                target: line
            }

            onTextEdited: root.error(false, "")
        }
    }

    Component {
        id: areaComponent

        TextArea {
            id: area

            text: root.text
            placeholderText: root.placeholderText
            wrapMode: TextArea.Wrap

            topPadding: 0
            bottomPadding: 0
            leftPadding: 15
            rightPadding: 15
            placeholderTextColor: "#909195"
            color: "white"
            font.pointSize: 11
            selectionColor: "#053ba7"
            background: null

            onTextChanged: {
                if (root.maximumLength > 0 && length > root.maximumLength) remove(root.maximumLength, length)
                if (root.text !== text) root.text = text
                if (activeFocus) root.error(false, "")
            }

            TapHandler {
                acceptedButtons: Qt.RightButton
                onTapped: areaMenu.menu.popup()
            }

            TextContextMenu {
                id: areaMenu

                target: area
            }
        }
    }
}