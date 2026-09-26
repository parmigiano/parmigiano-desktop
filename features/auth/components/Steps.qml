import QtQuick
import QtQuick.Layouts

Item {
    id: root

    property string currentStep: "Email" // Email/VerifyCode

    implicitHeight: stepsRow.implicitHeight

    RowLayout {
        id: stepsRow
        width: parent.width

        state: currentStep

        states: [
            State {
                name: "Email"

                PropertyChanges {
                    target: emailStep

                    line.color: "#1c75a9"
                    text.color: "#fff"
                }

                PropertyChanges {
                    target: codeStep

                    line.color: "#353535"
                    text.color: "#575757"
                }
            },

            State {
                name: "VerifyCode"

                PropertyChanges {
                    target: emailStep

                    line.color: "#1c75a9"
                    text.color: "#fff"
                }

                PropertyChanges {
                    target: codeStep

                    line.color: "#1c75a9"
                    text.color: "#fff"
                }
            }
        ]

        StepsItem {
            id: emailStep

            Layout.fillWidth: true
            Layout.preferredHeight: implicitHeight

            line.color: "#1c75a9"
            text.text: qsTr("Email")
            text.color: "#fff"
        }

        StepsItem {
            id: codeStep

            Layout.fillWidth: true
            Layout.preferredHeight: implicitHeight

            line.color: "#1c75a9"
            text.text: qsTr("Code")
            text.color: "#fff"
        }

        StepsItem {
            Layout.fillWidth: true
            Layout.preferredHeight: implicitHeight

            line.color: "#353535"
            text.text: qsTr("Ready")
            text.color: "#575757"
        }
    }
}
