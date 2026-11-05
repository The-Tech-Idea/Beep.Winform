using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Labels;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Small modeless progress window for an export. It is deliberately
    /// modeless: the export runs asynchronously while the form stays
    /// responsive, and the Cancel button cancels the token the engine owns.
    /// </summary>
    public sealed class GridExportProgressForm : Form
    {
        private readonly BeepLabel _message;
        private readonly BeepProgressBar _progress;
        private readonly BeepButton _cancel;
        private readonly CancellationTokenSource _cancellation;
        private bool _completed;

        public GridExportProgressForm(string title, CancellationTokenSource cancellation)
        {
            _cancellation = cancellation ?? throw new ArgumentNullException(nameof(cancellation));

            Text = string.IsNullOrWhiteSpace(title) ? "Exporting" : title;
            Width = 420;
            Height = 150;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            _message = new BeepLabel
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                WordWrap = true,
                Text = "Preparing export..."
            };

            _progress = new BeepProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            _cancel = new BeepButton
            {
                Text = "Cancel",
                Dock = DockStyle.Right,
                Width = 96
            };
            _cancel.Click += (_, __) =>
            {
                _cancel.Enabled = false;
                _message.Text = "Cancelling...";
                _cancellation.Cancel();
            };

            var buttonRow = new Panel { Dock = DockStyle.Fill };
            buttonRow.Controls.Add(_cancel);

            layout.Controls.Add(_message, 0, 0);
            layout.Controls.Add(_progress, 0, 1);
            layout.Controls.Add(buttonRow, 0, 2);
            Controls.Add(layout);

            FormClosing += (_, e) =>
            {
                if (!_completed && !_cancellation.IsCancellationRequested && _cancel.Enabled)
                {
                    // The user closed the window; treat it as a cancel request.
                    _cancellation.Cancel();
                    e.Cancel = false;
                }
            };
        }

        public void MarkComplete()
        {
            _completed = true;
            _cancel.Enabled = false;
        }

        public void Report(GridExportProgress progress)
        {
            if (IsDisposed || progress == null) return;

            int percent = (int)Math.Round(progress.Percent);
            if (percent >= 0 && percent <= 100)
                _progress.Value = percent;

            if (!string.IsNullOrWhiteSpace(progress.Message))
                _message.Text = progress.Message;
        }
    }
}
