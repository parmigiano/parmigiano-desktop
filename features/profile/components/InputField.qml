import QtQuick
import QtQuick.Controls

import ParmigianoDesktop.CoreUI

Item {
    id: root

    TextField {
        id: field

        verticalAlignment: Text.AlignVCenter
        leftPadding: 15
        placeholderTextColor: "#909195"
        color: "white"
        font.pointSize: 11
        selectionColor: "#053ba7"

        background: FieldFrame {
            hovered: field.hovered
            focused: field.activeFocus
            error: field.hasError
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
}
