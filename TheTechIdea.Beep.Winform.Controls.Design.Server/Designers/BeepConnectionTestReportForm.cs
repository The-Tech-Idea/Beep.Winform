using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepConnectionTestReportForm : Form
    {
        private readonly ListView _listView;
        private readonly Label _summaryLabel;
        private readonly Button _closeButton;

        public BeepConnectionTestReportForm(IReadOnlyList<BeepConnectionTestOutcome> outcomes)
        {
            Text = "Connection Test Report";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = true;
            ShowInTaskbar = false;
            Size = new Size(720, 460);
            MinimumSize = new Size(560, 320);

            int passed = outcomes?.Count(o => o?.Success == true) ?? 0;
            int failed = (outcomes?.Count ?? 0) - passed;

            _summaryLabel = new Label
            {
                Text = outcomes == null || outcomes.Count == 0
                    ? "No connections were tested."
                    : $"Tested {outcomes.Count} connection(s): {passed} succeeded, {failed} failed.",
                Dock = DockStyle.Top,
                Height = 32,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0),
                Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Bold)
            };

            _listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                MultiSelect = false,
                HideSelection = false
            };
            _listView.Columns.Add("Status", 90);
            _listView.Columns.Add("Connection", 220);
            _listView.Columns.Add("Driver", 140);
            _listView.Columns.Add("Latency (ms)", 110);
            _listView.Columns.Add("Message", 320);

            if (outcomes != null)
            {
                foreach (var outcome in outcomes)
                {
                    if (outcome == null)
                    {
                        continue;
                    }

                    var item = new ListViewItem(outcome.Success ? "OK" : "FAILED")
                    {
                        ImageIndex = -1
                    };
                    item.SubItems.Add(outcome.ConnectionName ?? string.Empty);
                    item.SubItems.Add(outcome.DriverName ?? string.Empty);
                    item.SubItems.Add(outcome.LatencyMs?.ToString() ?? "-");
                    item.SubItems.Add(outcome.Message ?? string.Empty);
                    item.ForeColor = outcome.Success ? Color.DarkGreen : Color.Firebrick;
                    _listView.Items.Add(item);
                }
            }

            _closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Width = 90,
                Height = 28,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(0, 6, 12, 6)
            };
            _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 6);
            buttonPanel.Resize += (_, _) => _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 6);
            buttonPanel.Controls.Add(_closeButton);

            Controls.Add(_listView);
            Controls.Add(_summaryLabel);
            Controls.Add(buttonPanel);
            AcceptButton = _closeButton;
            CancelButton = _closeButton;
        }
    }

    public sealed class BeepConnectionTestOutcome
    {
        public string ConnectionName { get; init; } = string.Empty;
        public string DriverName { get; init; } = string.Empty;
        public bool Success { get; init; }
        public long? LatencyMs { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}
