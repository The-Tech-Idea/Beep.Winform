using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepCustomDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates and configures the controls; <see cref="BeepDialogShell"/> decides where they sit.
        /// </summary>
        /// <remarks>
        /// There is no <c>_bodyPanel</c> any more. This dialog hosts a caller-supplied control, and
        /// the panel existed only to give that control a rectangle — which it did by assigning
        /// Location and Size and then re-assigning Size on every <c>SizeChanged</c>, a hand-rolled
        /// <c>Dock.Fill</c>. The shell's body row does that natively.
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new BeepDialogShell();
            _headerPanel = new TableLayoutPanel();
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _okButton = new BeepButton();
            _cancelButton = new BeepButton();
            SuspendLayout();

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
            _dialogIcon.ImagePath = Svgs.Information;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;

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

            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(420, 310);
            MinimumSize = new System.Drawing.Size(340, 220);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            // This dialog draws its own header row, so the skinned caption bar would be a
            // second, empty title band: it reserved 72px of top chrome and painted
            // owner.Text, which was blank. Text is now the accessible window name instead.
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepCustomDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
