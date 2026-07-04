using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepQuestionDialog
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this._headerPanel  = new BeepPanel();
            this._dialogIcon   = new BeepImage();
            this._titleLabel   = new BeepLabel();
            this._bodyPanel    = new BeepPanel();
            this._messageLabel = new BeepLabel();
            this._detailsToggleButton = new BeepButton();
            this._detailsLabel = new BeepLabel();
            this._buttonPanel  = new BeepPanel();
            this._yesButton    = new BeepButton();
            this._noButton     = new BeepButton();
            this.SuspendLayout();

            this._headerPanel.IsFrameless = true;
            this._headerPanel.ShowTitle = false;
            this._headerPanel.ShowTitleLine = false;
            this._headerPanel.UseThemeColors = true;
            this._headerPanel.Theme = "ModernTheme";
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Size = new System.Drawing.Size(420, 56);

            this._dialogIcon.UseThemeColors = true;
            this._dialogIcon.Theme = "ModernTheme";
            this._dialogIcon.ImagePath = Svgs.Question;
            this._dialogIcon.ScaleMode = ImageScaleMode.Stretch;
            this._dialogIcon.Location = new System.Drawing.Point(8, 10);
            this._dialogIcon.Size = new System.Drawing.Size(36, 36);

            this._titleLabel.UseThemeColors = true;
            this._titleLabel.Theme = "ModernTheme";
            this._titleLabel.IsFrameless = true;
            this._titleLabel.AutoEllipsis = true;
            this._titleLabel.Location = new System.Drawing.Point(52, 12);
            this._titleLabel.Size = new System.Drawing.Size(360, 36);

            this._bodyPanel.IsFrameless = true;
            this._bodyPanel.ShowTitle = false;
            this._bodyPanel.ShowTitleLine = false;
            this._bodyPanel.UseThemeColors = true;
            this._bodyPanel.Theme = "ModernTheme";
            this._bodyPanel.Location = new System.Drawing.Point(0, 56);
            this._bodyPanel.Size = new System.Drawing.Size(420, 90);

            this._messageLabel.UseThemeColors = true;
            this._messageLabel.Theme = "ModernTheme";
            this._messageLabel.IsFrameless = true;
            this._messageLabel.AutoEllipsis = true;
            this._messageLabel.WordWrap = true;
            this._messageLabel.Location = new System.Drawing.Point(12, 8);
            this._messageLabel.Size = new System.Drawing.Size(396, 40);

            this._detailsToggleButton.UseThemeColors = true;
            this._detailsToggleButton.Theme = "ModernTheme";
            this._detailsToggleButton.Visible = false;
            this._detailsToggleButton.Text = "Show details";
            this._detailsToggleButton.Location = new System.Drawing.Point(12, 52);
            this._detailsToggleButton.Size = new System.Drawing.Size(396, 26);

            this._detailsLabel.UseThemeColors = true;
            this._detailsLabel.Theme = "ModernTheme";
            this._detailsLabel.IsFrameless = true;
            this._detailsLabel.WordWrap = true;
            this._detailsLabel.Visible = false;
            this._detailsLabel.Location = new System.Drawing.Point(12, 78);
            this._detailsLabel.Size = new System.Drawing.Size(396, 0);

            this._buttonPanel.IsFrameless = true;
            this._buttonPanel.ShowTitle = false;
            this._buttonPanel.ShowTitleLine = false;
            this._buttonPanel.UseThemeColors = true;
            this._buttonPanel.Theme = "ModernTheme";
            this._buttonPanel.Location = new System.Drawing.Point(0, 146);
            this._buttonPanel.Size = new System.Drawing.Size(420, 54);

            this._yesButton.UseThemeColors = true;
            this._yesButton.Theme = "ModernTheme";
            this._yesButton.Text = "Yes";
            this._yesButton.AutoSize = true;
            this._yesButton.MinimumSize = new System.Drawing.Size(110, 34);
            this._yesButton.Location = new System.Drawing.Point(102, 8);

            this._noButton.UseThemeColors = true;
            this._noButton.Theme = "ModernTheme";
            this._noButton.Text = "No";
            this._noButton.AutoSize = true;
            this._noButton.MinimumSize = new System.Drawing.Size(110, 34);
            this._noButton.Location = new System.Drawing.Point(220, 8);

            this._headerPanel.Controls.Add(this._dialogIcon);
            this._headerPanel.Controls.Add(this._titleLabel);
            this._bodyPanel.Controls.Add(this._messageLabel);
            this._bodyPanel.Controls.Add(this._detailsToggleButton);
            this._bodyPanel.Controls.Add(this._detailsLabel);
            this._buttonPanel.Controls.Add(this._yesButton);
            this._buttonPanel.Controls.Add(this._noButton);
            this.Controls.Add(this._bodyPanel);
            this.Controls.Add(this._headerPanel);
            this.Controls.Add(this._buttonPanel);

            this._yesButton.Click += this.YesButton_Click;
            this._noButton.Click += this.NoButton_Click;
            this._detailsToggleButton.Click += this.DetailsToggleButton_Click;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 200);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.Text = string.Empty;
            this.Name = "BeepQuestionDialog";

            this.ResumeLayout(false);
        }

        internal BeepPanel  _headerPanel;
        internal BeepImage  _dialogIcon;
        internal BeepLabel  _titleLabel;
        internal BeepPanel  _bodyPanel;
        internal BeepLabel  _messageLabel;
        internal BeepButton _detailsToggleButton;
        internal BeepLabel  _detailsLabel;
        internal BeepPanel  _buttonPanel;
        internal BeepButton _yesButton;
        internal BeepButton _noButton;
    }
}
