using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepMultiSelectDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates the controls and declares the grid they sit in.
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
            _shell = new TableLayoutPanel();
            _shell.Dock = DockStyle.Fill;
            _shell.Padding = new Padding(16);
            _dialogIcon = new BeepImage();
            _titleLabel = new BeepLabel();
            _messageLabel = new BeepLabel();
            _listPanel = new TableLayoutPanel();
            _selectAllButton = new BeepButton();
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
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // actions
            SuspendLayout();

            // ── header: icon + title ─────────────────────────────────────────────

            _dialogIcon.IsChild = true;
            _dialogIcon.UseThemeColors = true;
            _dialogIcon.ImagePath = Svgs.CheckSquare;
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

            // ── body: prompt above the scrolling checkbox list ───────────────────

            _messageLabel.UseThemeColors = true;
            _messageLabel.IsFrameless = true;
            _messageLabel.AutoEllipsis = true;
            _messageLabel.AutoSize = true;
            _messageLabel.Dock = DockStyle.Fill;
            _messageLabel.Margin = new Padding(0, 0, 0, 8);

            // One column, one row per item, each checkbox filling its cell. A FlowLayoutPanel stacked
            // them instead, which left every checkbox at its own preferred width - so the rows did not
            // share a left edge for their content and nothing could be aligned across them. Rows are
            // added by SetItems as the items arrive.
            _listPanel.Dock = DockStyle.Fill;
            _listPanel.ColumnCount = 1;
            _listPanel.RowCount = 0;
            _listPanel.AutoScroll = true;
            _listPanel.Margin = new Padding(0);
            // The right padding reserves the vertical scrollbar's gutter.
            //
            // An AutoScroll TableLayoutPanel sizes its percent column from the full client width, and
            // only then discovers it needs a vertical scrollbar - which takes that width away. The
            // rows are already laid out to the wider figure, so a *horizontal* scrollbar appears
            // under a list that only ever needed to scroll vertically: measured at 372px of content
            // in a 355px client. Reserving the gutter up front means the column is sized to a width
            // that survives the scrollbar arriving.
            _listPanel.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 4);
            _listPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ── footer actions ───────────────────────────────────────────────────
            _selectAllButton.UseThemeColors = true;
            _selectAllButton.Text = "Select All";
            _selectAllButton.MinimumSize = new System.Drawing.Size(110, 34);
            // Sized from MinimumSize rather than AutoSize: a BeepButton's preferred size
            // comes from its text alone, so an AutoSize cell sizes to the caption and the
            // button then overflows the cell it was given.
            _selectAllButton.AutoSize = false;
            _selectAllButton.Size = new System.Drawing.Size(110, 34);
            _selectAllButton.Click += SelectAllButton_Click;

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
            ClientSize = new System.Drawing.Size(440, 380);
            MinimumSize = new System.Drawing.Size(360, 260);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepMultiSelectDialog";


            // ── the action row ───────────────────────────────────────────────────
            //
            // Right-anchored as a whole rather than pushed over by a spacer column: a percent
            // spacer starves the AutoSize button columns and lays the actions past the edge.
            _footerPanel.Anchor = AnchorStyles.Right;
            _footerPanel.AutoSize = true;
            _footerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _footerPanel.Margin = new Padding(0, 12, 0, 0);
            _footerPanel.RowCount = 1;
            _footerPanel.ColumnCount = 3;
            _footerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _selectAllButton.Anchor = AnchorStyles.None;
            _selectAllButton.Margin = new Padding(8, 0, 0, 0);
            _footerPanel.Controls.Add(_selectAllButton, 0, 0);
            _cancelButton.Anchor = AnchorStyles.None;
            _cancelButton.Margin = new Padding(8, 0, 0, 0);
            _footerPanel.Controls.Add(_cancelButton, 1, 0);
            _okButton.Anchor = AnchorStyles.None;
            _okButton.Margin = new Padding(8, 0, 0, 0);
            _footerPanel.Controls.Add(_okButton, 2, 0);

            _shell.Controls.Add(_dialogIcon, 0, 0);
            _shell.SetRowSpan(_dialogIcon, 3);
            _shell.Controls.Add(_titleLabel, 1, 0);
            _shell.Controls.Add(_messageLabel, 1, 1);
            _shell.Controls.Add(_listPanel, 1, 2);
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
        internal TableLayoutPanel _listPanel;
        internal BeepButton _selectAllButton;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
