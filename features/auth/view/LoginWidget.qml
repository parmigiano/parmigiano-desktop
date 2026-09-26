import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    required property AuthViewModel viewModel

    readonly property string authStep: "Email"
    readonly property int stepCount: 1
    readonly property string buttonText: qsTr("CONTINUE")

    implicitHeight: loginColumn.implicitHeight

    function submit() {
        emailInput.inputField.focus = false

        let email = emailInput.inputField.text
        let emailCheck = Validate.validateEmail(email)

        if (emailCheck)
        {
            emailInput.error(false, "")
            AuthContext.email = email
            viewModel.login(email)
        }
        else
        {
            emailInput.error(true, "Wrong email format")
        }
    }

    ColumnLayout {
        id: loginColumn

        width: parent.width
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 35

        ColumnLayout {

            spacing: 5

            Text {
                Layout.fillWidth: true

                text: qsTr("Email address")
                color: "#fff"

                font.bold: true
                font.pointSize: 18
            }

            Text {
                Layout.fillWidth: true

                text: qsTr("The confirmation code will be sent to the specified email address.")
                color: "#7d7d7d"

                font.pointSize: 10

                wrapMode: Text.WordWrap
            }
        }

        FloatingLabelInput {
            id: emailInput

            Layout.fillWidth: true
            Layout.preferredHeight: inputContainer.height //+ errorText.height
            //fieldContainer.height: 50

            //Layout.preferredHeight: 50
            //Layout.alignment: Qt.AlignBottom

            inputLabel.text: qsTr("Email")
        }
    }
}
