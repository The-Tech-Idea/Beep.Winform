using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepMessageDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); _autoCloseTimer?.Dispose(); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._dialogIcon   = new BeepImage();
            this._titleLabel   = new BeepLabel();
            this._messageLabel = new BeepLabel();
            this._okButton     = new BeepButton();
            this.SuspendLayout();

            this._dialogIcon.UseThemeColors = true;
            this._dialogIcon.Theme = "ModernTheme";
            this._dialogIcon.ImagePath = Svgs.Information;
            this._dialogIcon.ScaleMode = ImageScaleMode.Stretch;
            this._dialogIcon.Location = new System.Drawing.Point(12, 12);
            this._dialogIcon.Size = new System.Drawing.Size(36, 36);

            this._titleLabel.UseThemeColors = true;
            this._titleLabel.Theme = "ModernTheme";
            this._titleLabel.IsFrameless = true;
            this._titleLabel.AutoEllipsis = true;
            this._titleLabel.Text = "Title";
            this._titleLabel.Location = new System.Drawing.Point(56, 12);
            this._titleLabel.Size = new System.Drawing.Size(352, 36);

            this._messageLabel.UseThemeColors = true;
            this._messageLabel.Theme = "ModernTheme";
            this._messageLabel.IsFrameless = true;
            this._messageLabel.WordWrap = true;
            this._messageLabel.Text = "Message";
            this._messageLabel.Location = new System.Drawing.Point(12, 60);
            this._messageLabel.Size = new System.Drawing.Size(396, 80);

            this._okButton.UseThemeColors = true;
            this._okButton.Theme = "ModernTheme";
            this._okButton.Text = "OK";
            this._okButton.AutoSize = true;
            this._okButton.MinimumSize = new System.Drawing.Size(110, 34);
            this._okButton.Location = new System.Drawing.Point(155, 152);

            this.Controls.Add(this._messageLabel);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._dialogIcon);
            this.Controls.Add(this._okButton);

            this._okButton.Click += this.OkButton_Click;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 206);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.Text = string.Empty;
            this.Name = "BeepMessageDialog";

            this.ResumeLayout(false);
        }

        internal BeepImage  _dialogIcon;
        internal BeepLabel  _titleLabel;
        internal BeepLabel  _messageLabel;
        internal BeepButton _okButton;
    }
}
