using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class DocumentHostTextPromptDialog : Form
    {
        private readonly TextBox _textBox;

        public string Value => _textBox.Text.Trim();

        public DocumentHostTextPromptDialog(string title, string prompt, string initialValue)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 138);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9f);

            var promptLabel = new Label
            {
                Left = 18,
                Top = 18,
                Width = 384,
                Text = prompt
            };

            _textBox = new TextBox
            {
                Left = 18,
                Top = 44,
                Width = 384,
                Text = initialValue ?? string.Empty
            };
            _textBox.SelectAll();

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 212,
                Top = 90,
                Width = 90
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 312,
                Top = 90,
                Width = 90
            };

            AcceptButton = okButton;
            CancelButton = cancelButton;

            Controls.Add(promptLabel);
            Controls.Add(_textBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
        }
    }
}