using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepListDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates and configures the controls; <see cref="BeepDialogShell"/> decides where they sit.
        /// </summary>
        /// <remarks>
        /// The combo box was pinned to 396×28 at (12, 36) inside a body panel pinned to 420×90. A
        /// dialog whose entire purpose is to present a list could not grow to show more of it, and
        /// its two buttons at (102, 8) and (220, 8) would overlap on any longer caption. The shell
        /// gives the body the space the form actually has.
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new BeepDialogShell();
            _headerPanel = new TableLayoutPanel();
            _bodyPanel = new TableLayoutPanel();
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _comboBox = new BeepComboBox();
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
            _dialogIcon.ImagePath = Svgs.Input;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;

            // ── body: prompt above the list ──────────────────────────────────────
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

            // Fills the row rather than a fixed 396×28, so the list grows with the dialog.
            _comboBox.UseThemeColors = true;
            _comboBox.Dock = DockStyle.Top;
            _comboBox.Margin = new Padding(0);

            // ── footer actions ───────────────────────────────────────────────────
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
            ClientSize = new System.Drawing.Size(420, 200);
            MinimumSize = new System.Drawing.Size(340, 180);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            // This dialog draws its own header row, so the skinned caption bar would be a
            // second, empty title band: it reserved 72px of top chrome and painted
            // owner.Text, which was blank. Text is now the accessible window name instead.
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepListDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        private TableLayoutPanel _bodyPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal BeepComboBox _comboBox;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
