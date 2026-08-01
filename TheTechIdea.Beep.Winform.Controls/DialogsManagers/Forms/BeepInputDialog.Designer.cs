using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepInputDialog
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Creates and configures the controls; <see cref="BeepDialogShell"/> decides where they sit.
        /// </summary>
        /// <remarks>
        /// This file was 2,060 lines for ten controls - roughly 180 serialized properties each.
        /// Almost none of it was configuration: the designer had written out runtime state
        /// (<c>IsFocused = false</c>, <c>IsHovered = false</c>, <c>IsPressed = false</c>),
        /// null-valued references (<c>LinkedProperty = null</c>), and 30 <c>GraphicsPath</c>
        /// allocations for computed geometry - none of which a form should persist, and the paths
        /// leaked a GDI+ handle apiece on every construction.
        /// <para>
        /// What remains is the settings that genuinely differ from default and that the code-behind
        /// depends on: the icon, the frameless labels, the multiline-capable input, and the
        /// validation label that starts hidden.
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
            _inputBox = new BeepTextBox();
            _validationLabel = new BeepLabel();
            _okButton = new BeepButton();
            _cancelButton = new BeepButton();
            SuspendLayout();

            // header: icon + title
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
            _dialogIcon.Size = new Size(32, 32);
            _dialogIcon.Margin = new Padding(0, 0, 12, 0);

            _titleLabel.UseThemeColors = true;
            _titleLabel.IsFrameless = true;
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Dock = DockStyle.Fill;

            // body: prompt, input, validation
            _bodyPanel.Dock = DockStyle.Fill;
            _bodyPanel.ColumnCount = 1;
            _bodyPanel.RowCount = 3;
            _bodyPanel.Margin = new Padding(0);
            _bodyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // prompt
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // input
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // validation

            _messageLabel.UseThemeColors = true;
            _messageLabel.IsFrameless = true;
            _messageLabel.WordWrap = true;
            _messageLabel.AutoSize = true;
            _messageLabel.Dock = DockStyle.Fill;
            _messageLabel.Margin = new Padding(0, 0, 0, 6);

            // Docks to the top of its row so a single-line box keeps its natural height, while the
            // multiline case (see InputBoxMultiline) grows into the row's remaining space.
            _inputBox.UseThemeColors = true;
            _inputBox.Dock = DockStyle.Top;
            _inputBox.Margin = new Padding(0);

            // Hidden until a validator reports an error. The row is AutoSize, so it costs no height
            // while hidden - previously it sat at a fixed (9, 101) whether showing or not.
            _validationLabel.UseThemeColors = true;
            _validationLabel.IsFrameless = true;
            _validationLabel.WordWrap = true;
            _validationLabel.AutoSize = true;
            _validationLabel.Visible = false;
            _validationLabel.Dock = DockStyle.Fill;
            _validationLabel.Margin = new Padding(0, 6, 0, 0);

            // footer actions
            _okButton.UseThemeColors = true;
            _okButton.Text = "OK";
            _okButton.AutoSize = true;
            _okButton.MinimumSize = new Size(110, 34);
            _okButton.Click += OkButton_Click;

            _cancelButton.UseThemeColors = true;
            _cancelButton.Text = "Cancel";
            _cancelButton.AutoSize = true;
            _cancelButton.MinimumSize = new Size(110, 34);
            _cancelButton.Click += CancelButton_Click;

            // form
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(420, 260);
            MinimumSize = new Size(340, 200);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            // This dialog draws its own header row, so the skinned caption bar would be a
            // second, empty title band: it reserved 72px of top chrome and painted
            // owner.Text, which was blank. Text is now the accessible window name instead.
            ShowCaptionBar = false;
            Text = string.Empty;
            Name = "BeepInputDialog";

            Controls.Add(_shell);

            ResumeLayout(false);
        }

        private BeepDialogShell _shell;
        private TableLayoutPanel _headerPanel;
        private TableLayoutPanel _bodyPanel;
        internal BeepImage _dialogIcon;
        internal BeepLabel _titleLabel;
        internal BeepLabel _messageLabel;
        internal BeepTextBox _inputBox;
        internal BeepLabel _validationLabel;
        internal BeepButton _okButton;
        internal BeepButton _cancelButton;
    }
}
