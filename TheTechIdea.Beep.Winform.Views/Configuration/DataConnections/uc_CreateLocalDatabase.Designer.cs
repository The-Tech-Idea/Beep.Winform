
using TheTechIdea.Beep.ConfigUtil;

namespace Beep.Config.Winform.Functions
{
    partial class uc_CreateLocalDatabase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ReaLTaiizor.Controls.PoisonLabel passwordLabel;
            InstallFoldercomboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            EmbeddedDatabaseTypecomboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            label1 = new ReaLTaiizor.Controls.PoisonLabel();
            label2 = new ReaLTaiizor.Controls.PoisonLabel();
            label3 = new ReaLTaiizor.Controls.PoisonLabel();
            CreateDBbutton = new ReaLTaiizor.Controls.PoisonButton();
            dataConnectionsBindingSource = new BindingSource(components);
            passwordTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            databaseTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            panel2 = new ReaLTaiizor.Controls.PoisonPanel();
            poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            passwordLabel = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).BeginInit();
            poisonPanel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // passwordLabel
            // 
            passwordLabel.BorderStyle = BorderStyle.FixedSingle;
            passwordLabel.Location = new Point(32, 157);
            passwordLabel.Margin = new Padding(4, 0, 4, 0);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(200, 23);
            passwordLabel.TabIndex = 32;
            passwordLabel.Text = "Password:";
            passwordLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // InstallFoldercomboBox
            // 
            InstallFoldercomboBox.FormattingEnabled = true;
            InstallFoldercomboBox.ItemHeight = 23;
            InstallFoldercomboBox.Location = new Point(240, 87);
            InstallFoldercomboBox.Margin = new Padding(4, 3, 4, 3);
            InstallFoldercomboBox.Name = "InstallFoldercomboBox";
            InstallFoldercomboBox.Size = new Size(333, 29);
            InstallFoldercomboBox.TabIndex = 1;
            InstallFoldercomboBox.UseSelectable = true;
            // 
            // EmbeddedDatabaseTypecomboBox
            // 
            EmbeddedDatabaseTypecomboBox.FormattingEnabled = true;
            EmbeddedDatabaseTypecomboBox.ItemHeight = 23;
            EmbeddedDatabaseTypecomboBox.Location = new Point(240, 122);
            EmbeddedDatabaseTypecomboBox.Margin = new Padding(4, 3, 4, 3);
            EmbeddedDatabaseTypecomboBox.Name = "EmbeddedDatabaseTypecomboBox";
            EmbeddedDatabaseTypecomboBox.Size = new Size(333, 29);
            EmbeddedDatabaseTypecomboBox.TabIndex = 2;
            EmbeddedDatabaseTypecomboBox.UseSelectable = true;
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Location = new Point(32, 87);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 28);
            label1.TabIndex = 2;
            label1.Text = "Installation Folder";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BorderStyle = BorderStyle.FixedSingle;
            label2.Location = new Point(32, 122);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(200, 28);
            label2.TabIndex = 3;
            label2.Text = "Class  Database Type";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BorderStyle = BorderStyle.FixedSingle;
            label3.Location = new Point(32, 58);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(200, 23);
            label3.TabIndex = 5;
            label3.Text = "Database Name";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CreateDBbutton
            // 
            CreateDBbutton.Location = new Point(240, 186);
            CreateDBbutton.Margin = new Padding(4, 3, 4, 3);
            CreateDBbutton.Name = "CreateDBbutton";
            CreateDBbutton.Size = new Size(141, 27);
            CreateDBbutton.TabIndex = 4;
            CreateDBbutton.Text = "Create";
            CreateDBbutton.UseSelectable = true;
            // 
            // dataConnectionsBindingSource
            // 
            dataConnectionsBindingSource.DataSource = typeof(ConnectionProperties);
            // 
            // passwordTextBox
            // 
            // 
            // 
            // 
            passwordTextBox.CustomButton.Image = null;
            passwordTextBox.CustomButton.Location = new Point(312, 1);
            passwordTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.CustomButton.Name = "";
            passwordTextBox.CustomButton.Size = new Size(21, 21);
            passwordTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            passwordTextBox.CustomButton.TabIndex = 1;
            passwordTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            passwordTextBox.CustomButton.UseSelectable = true;
            passwordTextBox.CustomButton.Visible = false;
            passwordTextBox.Location = new Point(240, 157);
            passwordTextBox.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.MaxLength = 32767;
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '\0';
            passwordTextBox.ScrollBars = ScrollBars.None;
            passwordTextBox.SelectedText = "";
            passwordTextBox.SelectionLength = 0;
            passwordTextBox.SelectionStart = 0;
            passwordTextBox.ShortcutsEnabled = true;
            passwordTextBox.Size = new Size(334, 23);
            passwordTextBox.TabIndex = 3;
            passwordTextBox.UseSelectable = true;
            passwordTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            passwordTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // databaseTextBox
            // 
            // 
            // 
            // 
            databaseTextBox.CustomButton.Image = null;
            databaseTextBox.CustomButton.Location = new Point(312, 1);
            databaseTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            databaseTextBox.CustomButton.Name = "";
            databaseTextBox.CustomButton.Size = new Size(21, 21);
            databaseTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            databaseTextBox.CustomButton.TabIndex = 1;
            databaseTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            databaseTextBox.CustomButton.UseSelectable = true;
            databaseTextBox.CustomButton.Visible = false;
            databaseTextBox.Location = new Point(240, 58);
            databaseTextBox.Margin = new Padding(4, 3, 4, 3);
            databaseTextBox.MaxLength = 32767;
            databaseTextBox.Name = "databaseTextBox";
            databaseTextBox.PasswordChar = '\0';
            databaseTextBox.ScrollBars = ScrollBars.None;
            databaseTextBox.SelectedText = "";
            databaseTextBox.SelectionLength = 0;
            databaseTextBox.SelectionStart = 0;
            databaseTextBox.ShortcutsEnabled = true;
            databaseTextBox.Size = new Size(334, 23);
            databaseTextBox.TabIndex = 0;
            databaseTextBox.UseSelectable = true;
            databaseTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            databaseTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // poisonPanel1
            // 
            poisonPanel1.Controls.Add(panel2);
            poisonPanel1.Controls.Add(passwordTextBox);
            poisonPanel1.Controls.Add(passwordLabel);
            poisonPanel1.Controls.Add(InstallFoldercomboBox);
            poisonPanel1.Controls.Add(databaseTextBox);
            poisonPanel1.Controls.Add(EmbeddedDatabaseTypecomboBox);
            poisonPanel1.Controls.Add(CreateDBbutton);
            poisonPanel1.Controls.Add(label1);
            poisonPanel1.Controls.Add(label3);
            poisonPanel1.Controls.Add(label2);
            poisonPanel1.Dock = DockStyle.Fill;
            poisonPanel1.HorizontalScrollbarBarColor = true;
            poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel1.HorizontalScrollbarSize = 12;
            poisonPanel1.Location = new Point(0, 0);
            poisonPanel1.Margin = new Padding(4, 3, 4, 3);
            poisonPanel1.Name = "poisonPanel1";
            poisonPanel1.Size = new Size(617, 231);
            poisonPanel1.TabIndex = 33;
            poisonPanel1.VerticalScrollbarBarColor = true;
            poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel1.VerticalScrollbarSize = 12;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(poisonLabel1);
            panel2.Dock = DockStyle.Top;
            panel2.HorizontalScrollbarBarColor = true;
            panel2.HorizontalScrollbarHighlightOnWheel = false;
            panel2.HorizontalScrollbarSize = 12;
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(617, 50);
            panel2.TabIndex = 33;
            panel2.VerticalScrollbarBarColor = true;
            panel2.VerticalScrollbarHighlightOnWheel = false;
            panel2.VerticalScrollbarSize = 12;
            // 
            // poisonLabel1
            // 
            poisonLabel1.AutoSize = true;
            poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            poisonLabel1.Location = new Point(31, 7);
            poisonLabel1.Margin = new Padding(4, 0, 4, 0);
            poisonLabel1.Name = "poisonLabel1";
            poisonLabel1.Size = new Size(205, 25);
            poisonLabel1.TabIndex = 4;
            poisonLabel1.Text = "Create Local Database ";
            poisonLabel1.UseStyleColors = true;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // uc_CreateLocalDatabase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(poisonPanel1);
            Margin = new Padding(5, 3, 5, 3);
            Name = "uc_CreateLocalDatabase";
            Size = new Size(617, 231);
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).EndInit();
            poisonPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.PoisonComboBox InstallFoldercomboBox;
        private ReaLTaiizor.Controls.PoisonComboBox EmbeddedDatabaseTypecomboBox;
        private ReaLTaiizor.Controls.PoisonLabel label1;
        private ReaLTaiizor.Controls.PoisonLabel label2;
        private ReaLTaiizor.Controls.PoisonLabel label3;
        private ReaLTaiizor.Controls.PoisonButton CreateDBbutton;
        private System.Windows.Forms.BindingSource dataConnectionsBindingSource;
        private ReaLTaiizor.Controls.PoisonTextBox passwordTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox databaseTextBox;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonPanel panel2;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
    }
}
