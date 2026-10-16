import QtQuick
import QtQuick.Controls

import ParmigianoDesktop.SharedUI

TextField {
    id: root

    // property color normalColor: "#242527"
    // property color hoverColor: "#292a2c"
    // property alias frame: fieldFrame
    property bool hasError: false
    property string errorMessage: ""

    function error(isError, message) {
        root.hasError = isError
        root.errorMessage = isError ? message : ""
    }

    verticalAlignment: Text.AlignVCenter
    leftPadding: 15
    placeholderTextColor: "#909195"
    color: "white"
    font.pointSize: 11
    selectionColor: "#053ba7"

    background: FieldFrame {
        // id: fieldFrame

        // normalColor: root.normalColor
        // hoverColor: root.hoverColor
        hovered: root.hovered
        focused: root.activeFocus
        error: root.hasError
    }

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