using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepQuestionDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates and configures the controls; <see cref="BeepDialogShell"/> decides where they sit.
        /// </summary>
        /// <remarks>
        /// Every control here used to carry an explicit <c>Location</c> and <c>Size</c> — a body panel
        /// pinned to 420×90 whatever the message said, a details label at a hardcoded (12, 78) with a
        /// height of 0, and two buttons at (102, 8) and (220, 8) that would collide the moment either
        /// caption was localised. All of it is gone; the shell derives structure from content.
        /// <para>
        /// The <c>BeepPanel</c> wrappers went with it. They existed to carve the form into three
        /// bands by coordinate — the job a <c>TableLayoutPanel</c> row does — and were being told to
        /// be frameless and title-less so they stayed invisible while doing it. The body's stacked
        /// message / toggle / details is a real grouping, so that one survives as a table.
        /// </para>
        /// </remarks>
        private void InitializeComponent()
        {
            _shell = new BeepDialogShell();
            _headerPanel = new TableLayoutPanel();
            _bodyPanel = new TableLayoutPanel();
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _detailsToggleButton = new BeepButton();
            _detailsLabel = new BeepLabel();
            _yesButton = new BeepButton();
            _noButton = new BeepButton();
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
            _dialogIcon.ImagePath = Svgs.Question;
            _dialogIcon.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _dialogIcon.Size = new System.Drawing.Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.Theme = "ModernTheme";
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;

            // ── body: message, then the collapsible details ──────────────────────
            _bodyPanel.Dock = DockStyle.Fill;
            _bodyPanel.ColumnCount = 1;
            _bodyPanel.RowCount = 3;
            _bodyPanel.Margin = new Padding(0);
            _bodyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // message
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // toggle
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // details

            _messageLabel.UseThemeColors = true;
            _messageLabel.Theme = "ModernTheme";
            _messageLabel.IsFrameless = true;
            _messageLabel.WordWrap = true;
            _messageLabel.Dock = DockStyle.Fill;

            _detailsToggleButton.UseThemeColors = true;
            _detailsToggleButton.Theme = "ModernTheme";
            _detailsToggleButton.Visible = false;
            _detailsToggleButton.AutoSize = true;
            _detailsToggleButton.Text = "Show details";
            _detailsToggleButton.Anchor = AnchorStyles.Left;
            _detailsToggleButton.Margin = new Padding(0, 8, 0, 0);

            // No hardcoded height. The row is AutoSize, so details occupy exactly what the text
            // needs when shown and nothing when hidden. This label previously carried an explicit
            // Size of 396×0 — a height the form then had to fight every time it was revealed.
            _detailsLabel.UseThemeColors = true;
            _detailsLabel.Theme = "ModernTheme";
            _detailsLabel.IsFrameless = true;
            _detailsLabel.WordWrap = true;
            _detailsLabel.Visible = false;
            _detailsLabel.Dock = DockStyle.Fill;
            _detailsLabel.Margin = new Padding(0, 6, 0, 0);

            // ── footer actions ───────────────────────────────────────────────────
            _yesButton.UseThemeColors = true;
            _yesButton.Theme = "ModernTheme";
            _yesButton.Text = "Yes";
            _yesButton.AutoSize = true;
            _yesButton.MinimumSize = new System.Drawing.Size(110, 34);
            _yesButton.Click += YesButton_Click;

            _noButton.UseThemeColors = true;
            _noButton.Theme = "ModernTheme";
            _noButton.Text = "No";
            _noButton.AutoSize = true;
            _noButton.MinimumSize = new System.Drawing.Size(110, 34);
            _noButton.Click += NoButton_Click;

            _detailsToggleButton.Click += DetailsToggleButton_Click;

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
            Name = "BeepQuestionDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        private TableLayoutPanel _bodyPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal BeepButton _detailsToggleButton;
        internal BeepLabel _detailsLabel;
        internal BeepButton _yesButton;
        internal BeepButton _noButton;
    }
}
