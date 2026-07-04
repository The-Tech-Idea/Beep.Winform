using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    partial class BeepListDialog
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this._headerPanel  = new BeepPanel();
            this._dialogIcon   = new BeepImage();
            this._titleLabel   = new BeepLabel();
            this._bodyPanel    = new BeepPanel();
            this._messageLabel = new BeepLabel();
            this._comboBox     = new BeepComboBox();
            this._buttonPanel  = new BeepPanel();
            this._okButton     = new BeepButton();
            this._cancelButton = new BeepButton();
            this.SuspendLayout();

            this._headerPanel.IsFrameless = true;
            this._headerPanel.ShowTitle = false;
            this._headerPanel.ShowTitleLine = false;
            this._headerPanel.UseThemeColors = true;
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Size = new System.Drawing.Size(420, 56);

            this._dialogIcon.UseThemeColors = true;
            this._dialogIcon.ImagePath = Svgs.Input;
            this._dialogIcon.ScaleMode = ImageScaleMode.Stretch;
            this._dialogIcon.Location = new System.Drawing.Point(8, 10);
            this._dialogIcon.Size = new System.Drawing.Size(36, 36);

            this._titleLabel.UseThemeColors = true;
            this._titleLabel.IsFrameless = true;
            this._titleLabel.AutoEllipsis = true;
            this._titleLabel.Location = new System.Drawing.Point(52, 12);
            this._titleLabel.Size = new System.Drawing.Size(360, 36);

            this._bodyPanel.IsFrameless = true;
            this._bodyPanel.ShowTitle = false;
            this._bodyPanel.ShowTitleLine = false;
            this._bodyPanel.UseThemeColors = true;
            this._bodyPanel.Location = new System.Drawing.Point(0, 56);
            this._bodyPanel.Size = new System.Drawing.Size(420, 90);

            this._messageLabel.UseThemeColors = true;
            this._messageLabel.IsFrameless = true;
            this._messageLabel.AutoEllipsis = true;
            this._messageLabel.Location = new System.Drawing.Point(12, 8);
            this._messageLabel.Size = new System.Drawing.Size(396, 20);

            this._comboBox.UseThemeColors = true;
            this._comboBox.Location = new System.Drawing.Point(12, 36);
            this._comboBox.Size = new System.Drawing.Size(396, 28);

            this._buttonPanel.IsFrameless = true;
            this._buttonPanel.ShowTitle = false;
            this._buttonPanel.ShowTitleLine = false;
            this._buttonPanel.UseThemeColors = true;
            this._buttonPanel.Location = new System.Drawing.Point(0, 146);
            this._buttonPanel.Size = new System.Drawing.Size(420, 54);

            this._okButton.UseThemeColors = true;
            this._okButton.Text = "OK";
            this._okButton.AutoSize = true;
            this._okButton.MinimumSize = new System.Drawing.Size(110, 34);
            this._okButton.Location = new System.Drawing.Point(102, 8);

            this._cancelButton.UseThemeColors = true;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.AutoSize = true;
            this._cancelButton.MinimumSize = new System.Drawing.Size(110, 34);
            this._cancelButton.Location = new System.Drawing.Point(220, 8);

            this._headerPanel.Controls.Add(this._dialogIcon);
            this._headerPanel.Controls.Add(this._titleLabel);
            this._bodyPanel.Controls.Add(this._messageLabel);
            this._bodyPanel.Controls.Add(this._comboBox);
            this._buttonPanel.Controls.Add(this._okButton);
            this._buttonPanel.Controls.Add(this._cancelButton);
            this.Controls.Add(this._bodyPanel);
            this.Controls.Add(this._headerPanel);
            this.Controls.Add(this._buttonPanel);

            this._okButton.Click += this.OkButton_Click;
            this._cancelButton.Click += this.CancelButton_Click;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 200);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.Text = string.Empty;
            this.Name = "BeepListDialog";

            this.ResumeLayout(false);
        }

        internal BeepPanel    _headerPanel;
        internal BeepImage    _dialogIcon;
        internal BeepLabel    _titleLabel;
        internal BeepPanel    _bodyPanel;
        internal BeepLabel    _messageLabel;
        internal BeepComboBox _comboBox;
        internal BeepPanel    _buttonPanel;
        internal BeepButton   _okButton;
        internal BeepButton   _cancelButton;
    }
}
