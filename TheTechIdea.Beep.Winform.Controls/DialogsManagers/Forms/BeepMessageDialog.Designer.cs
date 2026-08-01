using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepMessageDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _autoCloseTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates and configures the controls; the shell decides where they sit.
        /// </summary>
        /// <remarks>
        /// Every control here used to carry an explicit <c>Location</c> and <c>Size</c> — the message
        /// label pinned to 396×80 whatever the message said, the title to 352×36 whatever the caption
        /// was, the button to a hardcoded (155, 152). Those numbers are gone. The shell's AutoSize
        /// header and percentage body derive them from content, which is what lets this dialog
        /// survive a long message, a high-DPI display and a caption localised into a longer language.
        /// <para>
        /// <c>AutoScaleMode</c> also changes from <c>Font</c> to <c>Dpi</c>: font-based scaling moves
        /// controls that were placed by coordinate, which is meaningless once a layout manager owns
        /// placement, and Dpi is what the rest of this codebase scales by.
        /// </para>
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new BeepDialogShell();
            _headerPanel = new TableLayoutPanel();
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _okButton = new BeepButton();
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
            _dialogIcon.Theme = "ModernTheme";
            _dialogIcon.ImagePath = Svgs.Information;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.Theme = "ModernTheme";
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;
            _titleLabel.Text = "Title";

            // ── body: the message, wrapping to whatever width the shell gives it ──
            _messageLabel.UseThemeColors = true;
            _messageLabel.Theme = "ModernTheme";
            _messageLabel.IsFrameless = true;
            _messageLabel.WordWrap = true;
            _messageLabel.Text = "Message";

            // ── footer action ────────────────────────────────────────────────────
            _okButton.UseThemeColors = true;
            _okButton.Theme = "ModernTheme";
            _okButton.Text = "OK";
            _okButton.AutoSize = true;
            _okButton.MinimumSize = new System.Drawing.Size(110, 34);
            _okButton.Click += OkButton_Click;

            // ── form ─────────────────────────────────────────────────────────────
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(420, 206);
            MinimumSize = new System.Drawing.Size(320, 160);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            // This dialog draws its own header row, so the skinned caption bar would be a
            // second, empty title band: it reserved 72px of top chrome and painted
            // owner.Text, which was blank. Text is now the accessible window name instead.
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepMessageDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal BeepButton _okButton;
    }
}
