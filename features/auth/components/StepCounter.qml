import QtQuick
import QtQuick.Controls

Item {
    id: root

    property string current: ""
    property string total: ""

    implicitWidth: container.implicitWidth
    implicitHeight: container.implicitHeight

    Pane {
        id: container

        anchors.fill: parent

        //padding: 4

        leftPadding: 15
        rightPadding: 15

        background: Rectangle {
            color: "#191a1c"
        }

        Row {
            anchors.fill: parent

            //anchors.verticalCenter: parent.verticalCenter
            // anchors.horizontalCenter: parent.horizontalCenter

            Text {
                text: qsTr(current + " of " + total)
                font.pointSize: 10
                color: "#7d7d7d"

                // horizontalAlignment: Qt.AlignVCenter
            }
        }
    }
}
