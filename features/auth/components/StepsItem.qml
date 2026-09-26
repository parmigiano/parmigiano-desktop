import QtQuick
import QtQuick.Layouts

Item {
    id: root

    property alias line: stepLine
    property alias text: stepText

    implicitHeight: stepColumn.implicitHeight

    ColumnLayout {
        id: stepColumn

        width: parent.width

        Rectangle {
            id: stepLine

            Layout.fillWidth: true
            Layout.preferredHeight: 3

            radius: 1

            color: "#1c75a9"
        }

        Text {
            id: stepText

            text: qsTr("Email")
            font.pointSize: 10
            color: "#fff"
        }
    }
}
