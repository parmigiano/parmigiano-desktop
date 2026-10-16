import QtQuick

Rectangle {
    id: root

    property bool hovered: false
    property bool focused: false
    property bool error: false
    property color focusColor: "#77797c"

    radius: 12
    color: root.hovered && !root.focused ? "#292a2c" : "#242527"
    border.width: 1
    border.color: root.error ? "#f05a5a" : root.focused ? root.focusColor : root.hovered && !root.focused ? "#292a2c" : "#242527"

    Behavior on color {
        ColorAnimation {
            duration: 180
        }
    }

    Behavior on border.color {
        ColorAnimation {
            duration: 180
        }
    }
}