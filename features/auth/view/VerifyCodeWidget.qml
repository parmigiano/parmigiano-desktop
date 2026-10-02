import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

import ParmigianoDesktop.FeatureAuth
import ParmigianoDesktop.CoreUI
import "js/validate.js" as Validate

Item {
    id: root

    required property AuthViewModel viewModel

    readonly property string authStep: "VerifyCode"
    readonly property int stepCount: 2
    readonly property int gap: 30
    readonly property string buttonText: qsTr("Confirm")

    implicitHeight: verifyCodeColumn.implicitHeight

    StackView.onActivated: {
        let field = repeaterCodeFields.itemAt(0);

        if (field) {
            field.setFocusAtEnd();
        }
    }

    function submit() {
        let code = "";

        for (let i = 0; i < repeaterCodeFields.count; ++i)
        {
            let target = repeaterCodeFields.itemAt(i);

            if (target)
            {
                code += target.text;
            }
        }

        if (code.length == 6)
        {
            viewModel.verifyCode(AuthContext.email, code);
        }
        else
        {
            console.log("no: " + code);
        }
    }

    ColumnLayout {
        id: verifyCodeColumn

        width: parent.width
        height: implicitHeight

        anchors.centerIn: parent

        spacing: 30

        ColumnLayout {
            spacing: 15

            Text {
                Layout.fillWidth: true
                horizontalAlignment: Qt.AlignHCenter

                text: qsTr("Check your email")
                color: "#fff"

                font.bold: true
                font.pointSize: 20
            }

            ColumnLayout {
                spacing: 0

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: qsTr("We have sent the confirmation code to")
                    color: "#a1a2a5"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }

                Text {
                    Layout.fillWidth: true
                    horizontalAlignment: Qt.AlignHCenter

                    text: AuthContext.email
                    color: "#eeeeef"

                    font.pointSize: 10

                    wrapMode: Text.WordWrap
                }
            }
        }

        RowLayout {
            id: rowCodeFields

            Layout.fillWidth: true
            spacing: 10

            Repeater {
                id: repeaterCodeFields
                model: 6

                Shake {
                    id: fieldShake

                    shakeTarget: repeaterCodeFields
                }

                delegate: TextField {
                    required property int index

                    property alias animationPulse: fieldPulse

                    id: codeField
                    Layout.fillWidth: true
                    Layout.preferredWidth: 0
                    Layout.preferredHeight: implicitHeight * 1.45
                    selectionColor: "#053ba7"
                    font.pointSize: 20
                    color: "white"
                    maximumLength: 1
                    horizontalAlignment: Text.AlignHCenter
                    verticalAlignment: Text.AlignVCenter
                    cursorVisible: false

                    BorderPulse {
                        id: fieldPulse

                        borderTarget: codeFieldBackground
                    }

                    background: Rectangle {
                        id: codeFieldBackground

                        // color: "#181818"
                        // border.color: codeField.focus ? "#1c75a9" : "#505050"
                        // border.width: 1
                        // radius: 10

                        color: codeField.focus ? "#fff" : "#4b4d50"
                        height: codeField.focus ? 2 : 1
                        anchors.bottom: parent.bottom

                        Behavior on color {
                            ColorAnimation {
                                duration: 100
                            }
                        }
                    }

                    function getDigitalsFromString(string) {
                        let digitals = [];

                        for (let i = 0; i < string.length; ++i)
                        {
                            let ch = string[i];

                            if (ch > '0' && ch < '9')
                            {
                                digitals.push(ch);
                            }
                        }

                        return digitals;
                    }

                    // Returning num which referred to the end of the code
                    function focusAtEnd() {
                        for (let i = 0; i < repeaterCodeFields.count; ++i)
                        {
                            let target = repeaterCodeFields.itemAt(i);

                            if (!target.text)
                            {
                                return i;
                            }
                        }

                        return repeaterCodeFields.count - 1;
                    }

                    function setFocusAtEnd() {
                        let focus = focusAtEnd();
                        let target = repeaterCodeFields.itemAt(focus);

                        if (target)
                        {
                            target.forceActiveFocus()
                        }
                    }

                    // Set necessary focus on clicking to any field
                    MouseArea {
                        id: codeFieldArea
                        anchors.fill: parent
                        hoverEnabled: true

                        onClicked: {
                            setFocusAtEnd()
                        }
                    }

                    // Parsing pressed button 1-9 and BACKSPACE (just input)
                    Keys.onPressed: (event) => {
                        // 1-9
                        if (event.key >= Qt.Key_0 && event.key <= Qt.Key_9)
                        {
                            if (index !== repeaterCodeFields.count - 1)
                            {
                                nextItemInFocusChain(true).forceActiveFocus();
                            }
                            else if (index === repeaterCodeFields.count - 1)
                            {
                                Qt.callLater(submit);
                            }

                            return;
                        }

                        // BACKSPACE
                        if (event.key === Qt.Key_Backspace)
                        {
                            if ((index !== (repeaterCodeFields.count - 1) && index !== 0)
                                || (index === (repeaterCodeFields.count - 1) && text.length === 0))
                            {
                                let target = repeaterCodeFields.itemAt(index - 1);

                                target.text = "";
                                target.forceActiveFocus();
                            }

                            return;
                        }

                        // if (event.key === Qt.Key_Backspace &&
                        //     (event.modifiers & Qt.ControlModifier))
                        // {
                        //     console.log("Ctrl+Backspace");
                        // }

                        // CTRL+V / Paste
                        if (event.matches(StandardKey.Paste))
                        {
                            if (index === repeaterCodeFields.count - 1) return;

                            let clipboardText = ClipboardManager.getClipboardData();
                            let digitals = getDigitalsFromString(clipboardText);

                            digitals = digitals.slice(0, repeaterCodeFields.count - index);

                            for (let i = 0; i < digitals.length; ++i)
                            {
                                let target = repeaterCodeFields.itemAt(i + index);

                                if (target)
                                {
                                    target.text = String(digitals[i]);
                                }
                            }

                            setFocusAtEnd();
                        }
                    }

                    onFocusChanged: {
                        if (focus && index == 0)
                        {
                            setFocusAtEnd();
                        }
                    }

                    // Disable tab/backTab inside code fields
                    Keys.onBacktabPressed: (event) => {}
                    Keys.onTabPressed: (event) => {}

                    // Only digitals
                    validator: IntValidator {}

                    // Disable cursor on inputing
                    // cursorDelegate: Item {
                    //     visible: false
                    // }
                }
            }
        }
    }
}
