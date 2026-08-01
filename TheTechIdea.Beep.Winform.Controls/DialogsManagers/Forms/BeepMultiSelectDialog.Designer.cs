using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepMultiSelectDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates and configures the controls; <see cref="BeepDialogShell"/> decides where they sit.
        /// </summary>
        /// <remarks>
        /// This dialog was the last one still laying itself out by hand, and it did not survive being
        /// shown: <c>OnResize</c> called <c>PositionButtons</c>, which dereferenced <c>_buttonPanel</c>
        /// while the constructor was still assigning it, so <c>Show()</c> threw a
        /// <see cref="System.NullReferenceException"/>. It also stacked three docked panels, positioned
        /// its buttons with <c>SetBounds</c> against a panel width, and re-measured every checkbox on
        /// each resize. The shell replaces all of it, and the crash goes with the code that caused it.
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new BeepDialogShell();
            _headerPanel = new TableLayoutPanel();
            _bodyPanel = new TableLayoutPanel();
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _listPanel = new FlowLayoutPanel();
            _selectAllButton = new BeepButton();
            _okButton = new BeepButton();
            _cancelButton = new BeepButton();
            SuspendLayout();

            // ── header: icon + title ─────────────────────────────────────────────
            _headerPanel.Dock = DockStyle.Fill;
            _headerPanel.AutoSize = true;
            _headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _headerPanel.ColumnCount = 2;
            _headerPanel.RowCount = 1;
            _headerPanel.Margin = new Padding(0);
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _headerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _dialogIcon.UseThemeColors = true;
            _dialogIcon.ImagePath = Svgs.CheckSquare;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;

            // ── body: prompt above the scrolling checkbox list ───────────────────
            _bodyPanel.Dock = DockStyle.Fill;
            _bodyPanel.ColumnCount = 1;
            _bodyPanel.RowCount = 2;
            _bodyPanel.Margin = new Padding(0);
            _bodyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // prompt
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // the list itself

            _messageLabel.UseThemeColors = true;
            _messageLabel.IsFrameless = true;
            _messageLabel.AutoEllipsis = true;
            _messageLabel.AutoSize = true;
            _messageLabel.Dock = DockStyle.Fill;
            _messageLabel.Margin = new Padding(0, 0, 0, 8);

            // A single-column flow panel replaces the hand-computed y-cursor and the per-resize
            // width loop: each checkbox is added in order and the panel handles scrolling.
            _listPanel.Dock = DockStyle.Fill;
            _listPanel.FlowDirection = FlowDirection.TopDown;
            _listPanel.WrapContents = false;
            _listPanel.AutoScroll = true;
            _listPanel.Margin = new Padding(0);
            _listPanel.Padding = new Padding(0, 0, 0, 4);

            // ── footer actions ───────────────────────────────────────────────────
            _selectAllButton.UseThemeColors = true;
            _selectAllButton.Text = "Select All";
            _selectAllButton.AutoSize = true;
            _selectAllButton.MinimumSize = new System.Drawing.Size(110, 34);
            _selectAllButton.Click += SelectAllButton_Click;

            _okButton.UseThemeColors = true;
            _okButton.Text = "OK";
            _okButton.AutoSize = true;
            _okButton.MinimumSize = new System.Drawing.Size(110, 34);
            _okButton.Click += OkButton_Click;

            _cancelButton.UseThemeColors = true;
            _cancelButton.Text = "Cancel";
            _cancelButton.AutoSize = true;
            _cancelButton.MinimumSize = new System.Drawing.Size(110, 34);
            _cancelButton.Click += CancelButton_Click;

            // ── form ─────────────────────────────────────────────────────────────
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(440, 380);
            MinimumSize = new System.Drawing.Size(360, 260);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepMultiSelectDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        private TableLayoutPanel _bodyPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal FlowLayoutPanel _listPanel;
        internal BeepButton _selectAllButton;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
