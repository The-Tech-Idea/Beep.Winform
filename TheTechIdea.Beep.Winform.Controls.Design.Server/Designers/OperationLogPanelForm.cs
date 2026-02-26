using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class OperationLogPanelForm : Form
    {
        private readonly TextBox _logTextBox;
        private readonly Action? _clearLogsAction;

        public OperationLogPanelForm(IEnumerable<string> logEntries, Action? clearLogsAction = null)
        {
            _clearLogsAction = clearLogsAction;
            Text = "BeepDataConnection Operation Logs";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(820, 520);
            MinimumSize = new Size(640, 420);

            _logTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
                WordWrap = false,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10f)
            };

            var panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46
            };

            var closeButton = new Button
            {
                Text = "Close",
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 84,
                Location = new Point(722, 10),
                DialogResult = DialogResult.OK
            };

            var copyButton = new Button
            {
                Text = "Copy",
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 84,
                Location = new Point(632, 10)
            };
            copyButton.Click += (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(_logTextBox.Text))
                {
                    Clipboard.SetText(_logTextBox.Text);
                }
            };

            var clearButton = new Button
            {
                Text = "Clear Logs",
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 96,
                Location = new Point(530, 10)
            };
            clearButton.Click += (_, _) =>
            {
                _clearLogsAction?.Invoke();
                _logTextBox.Clear();
            };

            panel.Controls.Add(clearButton);
            panel.Controls.Add(copyButton);
            panel.Controls.Add(closeButton);
            Controls.Add(_logTextBox);
            Controls.Add(panel);

            AcceptButton = closeButton;
            _logTextBox.Text = string.Join(Environment.NewLine, logEntries ?? Enumerable.Empty<string>());
        }
    }
}
