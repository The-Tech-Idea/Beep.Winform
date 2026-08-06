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
        /// Creates the controls and declares the grid they sit in.
        /// </summary>
        /// <remarks>
        /// The combo box was pinned to 396×28 at (12, 36) inside a body panel pinned to 420×90. A
        /// dialog whose entire purpose is to present a list could not grow to show more of it, and
        /// its two buttons at (102, 8) and (220, 8) would overlap on any longer caption. The shell
        /// gives the body the space the form actually has.
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new TableLayoutPanel();
            _shell.Dock = DockStyle.Fill;
            _shell.Padding = new Padding(16);
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _comboBox = new BeepComboBox();
            _okButton = new BeepButton();
            _cancelButton = new BeepButton();
            _footerPanel = new TableLayoutPanel();

            // ── the grid ─────────────────────────────────────────────────────────
            //
            // Declared here in plain Controls.Add statements so the controls are visible and
            // editable on the design surface. Built from code at runtime instead, this form
            // rendered blank in the designer: it shows what InitializeComponent parents.
            _shell.ColumnCount = 2;
            _shell.RowCount = 4;
            _shell.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // icon gutter
            _shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // title and content
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // message: hugs its text
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // actions
            SuspendLayout();

            // ── header: icon + title ─────────────────────────────────────────────

            _dialogIcon.IsChild = true;
            _dialogIcon.UseThemeColors = true;
            _dialogIcon.ImagePath = Svgs.Input;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);
            _dialogIcon.Anchor = AnchorStyles.Top;
            _dialogIcon.IsChild = true;   // renders the shell's painted header band

            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.IsChild = true;
            _titleLabel.UseThemeColors = true;
            _titleLabel.Dock = DockStyle.Fill;
            _titleLabel.IsChild = true;   // same: the band shows through

            // ── body: prompt above the list ──────────────────────────────────────

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
            _okButton.MinimumSize = new System.Drawing.Size(110, 34);
            // Sized from MinimumSize rather than AutoSize: a BeepButton's preferred size
            // comes from its text alone, so an AutoSize cell sizes to the caption and the
            // button then overflows the cell it was given.
            _okButton.AutoSize = false;
            _okButton.Size = new System.Drawing.Size(110, 34);
            _okButton.Click += OkButton_Click;

            _cancelButton.UseThemeColors = true;
            _cancelButton.Text = "Cancel";
            _cancelButton.MinimumSize = new System.Drawing.Size(110, 34);
            // Sized from MinimumSize rather than AutoSize: a BeepButton's preferred size
            // comes from its text alone, so an AutoSize cell sizes to the caption and the
            // button then overflows the cell it was given.
            _cancelButton.AutoSize = false;
            _cancelButton.Size = new System.Drawing.Size(110, 34);
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


            // ── the action row ───────────────────────────────────────────────────
            //
            // Right-anchored as a whole rather than pushed over by a spacer column: a percent
            // spacer starves the AutoSize button columns and lays the actions past the edge.
            _footerPanel.Anchor = AnchorStyles.Right;
            _footerPanel.AutoSize = true;
            _footerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _footerPanel.Margin = new Padding(0, 12, 0, 0);
            _footerPanel.RowCount = 1;
            _footerPanel.ColumnCount = 2;
            _footerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _cancelButton.Anchor = AnchorStyles.None;
            _cancelButton.Margin = new Padding(8, 0, 0, 0);
            _footerPanel.Controls.Add(_cancelButton, 0, 0);
            _okButton.Anchor = AnchorStyles.None;
            _okButton.Margin = new Padding(8, 0, 0, 0);
            _footerPanel.Controls.Add(_okButton, 1, 0);

            _shell.Controls.Add(_dialogIcon, 0, 0);
            _shell.SetRowSpan(_dialogIcon, 3);
            _shell.Controls.Add(_titleLabel, 1, 0);
            _shell.Controls.Add(_messageLabel, 1, 1);
            _shell.Controls.Add(_comboBox, 1, 2);
            _shell.Controls.Add(_footerPanel, 0, 3);
            _shell.SetColumnSpan(_footerPanel, 2);

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        internal TableLayoutPanel _shell;
        internal TableLayoutPanel _footerPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal BeepComboBox _comboBox;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
