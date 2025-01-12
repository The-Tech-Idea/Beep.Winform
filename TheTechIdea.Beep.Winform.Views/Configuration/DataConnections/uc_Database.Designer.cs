using TheTechIdea.Beep.ConfigUtil;

namespace Beep.Config.Winform.DataConnections
{
    partial class uc_Database
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
            ReaLTaiizor.Controls.PoisonLabel label2;
            ReaLTaiizor.Controls.PoisonLabel label1;
            ReaLTaiizor.Controls.PoisonLabel portLabel;
            ReaLTaiizor.Controls.PoisonLabel passwordLabel;
            ReaLTaiizor.Controls.PoisonLabel userIDLabel;
            ReaLTaiizor.Controls.PoisonLabel schemaNameLabel;
            ReaLTaiizor.Controls.PoisonLabel databaseLabel;
            ReaLTaiizor.Controls.PoisonLabel hostLabel;
            ReaLTaiizor.Controls.PoisonLabel databaseTypeLabel;
            ReaLTaiizor.Controls.PoisonLabel connectionNameLabel;
            ReaLTaiizor.Controls.PoisonLabel driverVersionLabel;
            ReaLTaiizor.Controls.PoisonLabel driverNameLabel;
            ReaLTaiizor.Controls.PoisonLabel poisonLabel2;
            ReaLTaiizor.Controls.PoisonLabel poisonLabel3;
            ReaLTaiizor.Controls.PoisonLabel poisonLabel4;
            comboBox1 = new ReaLTaiizor.Controls.PoisonComboBox();
            dataConnectionsBindingSource = new BindingSource(components);
            DatasourceCategorycomboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            portTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            passwordTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            userIDTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            schemaNameTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            databaseTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            hostTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            databaseTypeComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            connectionNameTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            driverVersionComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            driverNameComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            poisonTextBox1 = new ReaLTaiizor.Controls.PoisonTextBox();
            poisonPanel2 = new ReaLTaiizor.Controls.PoisonPanel();
            poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonPanel3 = new ReaLTaiizor.Controls.PoisonPanel();
            ExitCancelpoisonButton = new ReaLTaiizor.Controls.PoisonButton();
            SaveButton = new ReaLTaiizor.Controls.PoisonButton();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            FilePathpoisonTextBox1 = new ReaLTaiizor.Controls.PoisonTextBox();
            FileNamepoisonTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            label2 = new ReaLTaiizor.Controls.PoisonLabel();
            label1 = new ReaLTaiizor.Controls.PoisonLabel();
            portLabel = new ReaLTaiizor.Controls.PoisonLabel();
            passwordLabel = new ReaLTaiizor.Controls.PoisonLabel();
            userIDLabel = new ReaLTaiizor.Controls.PoisonLabel();
            schemaNameLabel = new ReaLTaiizor.Controls.PoisonLabel();
            databaseLabel = new ReaLTaiizor.Controls.PoisonLabel();
            hostLabel = new ReaLTaiizor.Controls.PoisonLabel();
            databaseTypeLabel = new ReaLTaiizor.Controls.PoisonLabel();
            connectionNameLabel = new ReaLTaiizor.Controls.PoisonLabel();
            driverVersionLabel = new ReaLTaiizor.Controls.PoisonLabel();
            driverNameLabel = new ReaLTaiizor.Controls.PoisonLabel();
            poisonLabel2 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonLabel3 = new ReaLTaiizor.Controls.PoisonLabel();
            poisonLabel4 = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).BeginInit();
            poisonPanel1.SuspendLayout();
            poisonPanel2.SuspendLayout();
            poisonPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // label2
            // 
            label2.BorderStyle = BorderStyle.FixedSingle;
            label2.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            label2.Location = new Point(36, 247);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(200, 28);
            label2.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            label2.TabIndex = 59;
            label2.Text = "Oracle only SID or Service:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.UseStyleColors = true;
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            label1.Location = new Point(36, 148);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 28);
            label1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            label1.TabIndex = 58;
            label1.Text = "Category Type:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.UseStyleColors = true;
            // 
            // portLabel
            // 
            portLabel.BorderStyle = BorderStyle.FixedSingle;
            portLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            portLabel.Location = new Point(36, 379);
            portLabel.Margin = new Padding(4, 0, 4, 0);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(200, 28);
            portLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            portLabel.TabIndex = 57;
            portLabel.Text = "Port:";
            portLabel.TextAlign = ContentAlignment.MiddleCenter;
            portLabel.UseStyleColors = true;
            // 
            // passwordLabel
            // 
            passwordLabel.BorderStyle = BorderStyle.FixedSingle;
            passwordLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            passwordLabel.Location = new Point(36, 346);
            passwordLabel.Margin = new Padding(4, 0, 4, 0);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(200, 28);
            passwordLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            passwordLabel.TabIndex = 56;
            passwordLabel.Text = "Password:";
            passwordLabel.TextAlign = ContentAlignment.MiddleCenter;
            passwordLabel.UseStyleColors = true;
            // 
            // userIDLabel
            // 
            userIDLabel.BorderStyle = BorderStyle.FixedSingle;
            userIDLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            userIDLabel.Location = new Point(36, 313);
            userIDLabel.Margin = new Padding(4, 0, 4, 0);
            userIDLabel.Name = "userIDLabel";
            userIDLabel.Size = new Size(200, 28);
            userIDLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            userIDLabel.TabIndex = 55;
            userIDLabel.Text = "User ID:";
            userIDLabel.TextAlign = ContentAlignment.MiddleCenter;
            userIDLabel.UseStyleColors = true;
            // 
            // schemaNameLabel
            // 
            schemaNameLabel.BorderStyle = BorderStyle.FixedSingle;
            schemaNameLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            schemaNameLabel.Location = new Point(36, 280);
            schemaNameLabel.Margin = new Padding(4, 0, 4, 0);
            schemaNameLabel.Name = "schemaNameLabel";
            schemaNameLabel.Size = new Size(200, 28);
            schemaNameLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            schemaNameLabel.TabIndex = 53;
            schemaNameLabel.Text = "Schema Name:";
            schemaNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            schemaNameLabel.UseStyleColors = true;
            // 
            // databaseLabel
            // 
            databaseLabel.BorderStyle = BorderStyle.FixedSingle;
            databaseLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            databaseLabel.Location = new Point(36, 213);
            databaseLabel.Margin = new Padding(4, 0, 4, 0);
            databaseLabel.Name = "databaseLabel";
            databaseLabel.Size = new Size(200, 28);
            databaseLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            databaseLabel.TabIndex = 49;
            databaseLabel.Text = "Database:";
            databaseLabel.TextAlign = ContentAlignment.MiddleCenter;
            databaseLabel.UseStyleColors = true;
            // 
            // hostLabel
            // 
            hostLabel.BorderStyle = BorderStyle.FixedSingle;
            hostLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            hostLabel.Location = new Point(36, 181);
            hostLabel.Margin = new Padding(4, 0, 4, 0);
            hostLabel.Name = "hostLabel";
            hostLabel.Size = new Size(200, 28);
            hostLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            hostLabel.TabIndex = 46;
            hostLabel.Text = "Host:";
            hostLabel.TextAlign = ContentAlignment.MiddleCenter;
            hostLabel.UseStyleColors = true;
            // 
            // databaseTypeLabel
            // 
            databaseTypeLabel.BorderStyle = BorderStyle.FixedSingle;
            databaseTypeLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            databaseTypeLabel.Location = new Point(36, 115);
            databaseTypeLabel.Margin = new Padding(4, 0, 4, 0);
            databaseTypeLabel.Name = "databaseTypeLabel";
            databaseTypeLabel.Size = new Size(200, 28);
            databaseTypeLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            databaseTypeLabel.TabIndex = 43;
            databaseTypeLabel.Text = "Database Type:";
            databaseTypeLabel.TextAlign = ContentAlignment.MiddleCenter;
            databaseTypeLabel.UseStyleColors = true;
            // 
            // connectionNameLabel
            // 
            connectionNameLabel.BorderStyle = BorderStyle.FixedSingle;
            connectionNameLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            connectionNameLabel.Location = new Point(36, 83);
            connectionNameLabel.Margin = new Padding(4, 0, 4, 0);
            connectionNameLabel.Name = "connectionNameLabel";
            connectionNameLabel.Size = new Size(200, 28);
            connectionNameLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            connectionNameLabel.TabIndex = 41;
            connectionNameLabel.Text = "Connection Name:";
            connectionNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            connectionNameLabel.UseStyleColors = true;
            // 
            // driverVersionLabel
            // 
            driverVersionLabel.BorderStyle = BorderStyle.FixedSingle;
            driverVersionLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            driverVersionLabel.Location = new Point(36, 472);
            driverVersionLabel.Margin = new Padding(2, 0, 2, 0);
            driverVersionLabel.Name = "driverVersionLabel";
            driverVersionLabel.Size = new Size(200, 28);
            driverVersionLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            driverVersionLabel.TabIndex = 64;
            driverVersionLabel.Text = "Version:";
            driverVersionLabel.TextAlign = ContentAlignment.MiddleCenter;
            driverVersionLabel.UseStyleColors = true;
            // 
            // driverNameLabel
            // 
            driverNameLabel.BorderStyle = BorderStyle.FixedSingle;
            driverNameLabel.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            driverNameLabel.Location = new Point(36, 438);
            driverNameLabel.Margin = new Padding(2, 0, 2, 0);
            driverNameLabel.Name = "driverNameLabel";
            driverNameLabel.Size = new Size(200, 28);
            driverNameLabel.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            driverNameLabel.TabIndex = 63;
            driverNameLabel.Text = "Driver Name:";
            driverNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            driverNameLabel.UseStyleColors = true;
            // 
            // poisonLabel2
            // 
            poisonLabel2.BorderStyle = BorderStyle.FixedSingle;
            poisonLabel2.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            poisonLabel2.Location = new Point(37, 514);
            poisonLabel2.Margin = new Padding(2, 0, 2, 0);
            poisonLabel2.Name = "poisonLabel2";
            poisonLabel2.Size = new Size(200, 28);
            poisonLabel2.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonLabel2.TabIndex = 67;
            poisonLabel2.Text = "Connection String:";
            poisonLabel2.TextAlign = ContentAlignment.MiddleCenter;
            poisonLabel2.UseStyleColors = true;
            // 
            // comboBox1
            // 
            comboBox1.DataBindings.Add(new Binding("SelectedValue", dataConnectionsBindingSource, "OracleSIDorService", true));
            comboBox1.DataBindings.Add(new Binding("SelectedMenuItem", dataConnectionsBindingSource, "OracleSIDorService", true));
            comboBox1.FormattingEnabled = true;
            comboBox1.ItemHeight = 23;
            comboBox1.Items.AddRange(new object[] { "SID", "SERVICE_NAME" });
            comboBox1.Location = new Point(244, 247);
            comboBox1.Margin = new Padding(2, 3, 2, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(300, 29);
            comboBox1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            comboBox1.TabIndex = 48;
            comboBox1.UseSelectable = true;
            comboBox1.UseStyleColors = true;
            // 
            // dataConnectionsBindingSource
            // 
            dataConnectionsBindingSource.DataSource = typeof(ConnectionProperties);
            // 
            // DatasourceCategorycomboBox
            // 
            DatasourceCategorycomboBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "Category", true));
            DatasourceCategorycomboBox.FormattingEnabled = true;
            DatasourceCategorycomboBox.ItemHeight = 23;
            DatasourceCategorycomboBox.Location = new Point(244, 148);
            DatasourceCategorycomboBox.Margin = new Padding(4, 3, 4, 3);
            DatasourceCategorycomboBox.Name = "DatasourceCategorycomboBox";
            DatasourceCategorycomboBox.Size = new Size(300, 29);
            DatasourceCategorycomboBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            DatasourceCategorycomboBox.TabIndex = 44;
            DatasourceCategorycomboBox.UseSelectable = true;
            DatasourceCategorycomboBox.UseStyleColors = true;
            // 
            // portTextBox
            // 
            // 
            // 
            // 
            portTextBox.CustomButton.Image = null;
            portTextBox.CustomButton.Location = new Point(274, 2);
            portTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            portTextBox.CustomButton.Name = "";
            portTextBox.CustomButton.Size = new Size(23, 23);
            portTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            portTextBox.CustomButton.TabIndex = 1;
            portTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            portTextBox.CustomButton.UseSelectable = true;
            portTextBox.CustomButton.Visible = false;
            portTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "Port", true));
            portTextBox.Location = new Point(244, 379);
            portTextBox.Margin = new Padding(4, 3, 4, 3);
            portTextBox.MaxLength = 32767;
            portTextBox.Name = "portTextBox";
            portTextBox.PasswordChar = '\0';
            portTextBox.ScrollBars = ScrollBars.None;
            portTextBox.SelectedText = "";
            portTextBox.SelectionLength = 0;
            portTextBox.SelectionStart = 0;
            portTextBox.ShortcutsEnabled = true;
            portTextBox.Size = new Size(300, 28);
            portTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            portTextBox.TabIndex = 54;
            portTextBox.UseSelectable = true;
            portTextBox.UseStyleColors = true;
            portTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            portTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // passwordTextBox
            // 
            // 
            // 
            // 
            passwordTextBox.CustomButton.Image = null;
            passwordTextBox.CustomButton.Location = new Point(274, 2);
            passwordTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.CustomButton.Name = "";
            passwordTextBox.CustomButton.Size = new Size(23, 23);
            passwordTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            passwordTextBox.CustomButton.TabIndex = 1;
            passwordTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            passwordTextBox.CustomButton.UseSelectable = true;
            passwordTextBox.CustomButton.Visible = false;
            passwordTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "Password", true));
            passwordTextBox.Location = new Point(244, 346);
            passwordTextBox.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.MaxLength = 32767;
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.ScrollBars = ScrollBars.None;
            passwordTextBox.SelectedText = "";
            passwordTextBox.SelectionLength = 0;
            passwordTextBox.SelectionStart = 0;
            passwordTextBox.ShortcutsEnabled = true;
            passwordTextBox.Size = new Size(300, 28);
            passwordTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            passwordTextBox.TabIndex = 52;
            passwordTextBox.UseSelectable = true;
            passwordTextBox.UseStyleColors = true;
            passwordTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            passwordTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // userIDTextBox
            // 
            // 
            // 
            // 
            userIDTextBox.CustomButton.Image = null;
            userIDTextBox.CustomButton.Location = new Point(274, 2);
            userIDTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            userIDTextBox.CustomButton.Name = "";
            userIDTextBox.CustomButton.Size = new Size(23, 23);
            userIDTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            userIDTextBox.CustomButton.TabIndex = 1;
            userIDTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            userIDTextBox.CustomButton.UseSelectable = true;
            userIDTextBox.CustomButton.Visible = false;
            userIDTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "UserID", true));
            userIDTextBox.Location = new Point(244, 313);
            userIDTextBox.Margin = new Padding(4, 3, 4, 3);
            userIDTextBox.MaxLength = 32767;
            userIDTextBox.Name = "userIDTextBox";
            userIDTextBox.PasswordChar = '\0';
            userIDTextBox.ScrollBars = ScrollBars.None;
            userIDTextBox.SelectedText = "";
            userIDTextBox.SelectionLength = 0;
            userIDTextBox.SelectionStart = 0;
            userIDTextBox.ShortcutsEnabled = true;
            userIDTextBox.Size = new Size(300, 28);
            userIDTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            userIDTextBox.TabIndex = 51;
            userIDTextBox.UseSelectable = true;
            userIDTextBox.UseStyleColors = true;
            userIDTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            userIDTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // schemaNameTextBox
            // 
            // 
            // 
            // 
            schemaNameTextBox.CustomButton.Image = null;
            schemaNameTextBox.CustomButton.Location = new Point(274, 2);
            schemaNameTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            schemaNameTextBox.CustomButton.Name = "";
            schemaNameTextBox.CustomButton.Size = new Size(23, 23);
            schemaNameTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            schemaNameTextBox.CustomButton.TabIndex = 1;
            schemaNameTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            schemaNameTextBox.CustomButton.UseSelectable = true;
            schemaNameTextBox.CustomButton.Visible = false;
            schemaNameTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "SchemaName", true));
            schemaNameTextBox.Location = new Point(244, 280);
            schemaNameTextBox.Margin = new Padding(4, 3, 4, 3);
            schemaNameTextBox.MaxLength = 32767;
            schemaNameTextBox.Name = "schemaNameTextBox";
            schemaNameTextBox.PasswordChar = '\0';
            schemaNameTextBox.ScrollBars = ScrollBars.None;
            schemaNameTextBox.SelectedText = "";
            schemaNameTextBox.SelectionLength = 0;
            schemaNameTextBox.SelectionStart = 0;
            schemaNameTextBox.ShortcutsEnabled = true;
            schemaNameTextBox.Size = new Size(300, 28);
            schemaNameTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            schemaNameTextBox.TabIndex = 50;
            schemaNameTextBox.UseSelectable = true;
            schemaNameTextBox.UseStyleColors = true;
            schemaNameTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            schemaNameTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // databaseTextBox
            // 
            // 
            // 
            // 
            databaseTextBox.CustomButton.Image = null;
            databaseTextBox.CustomButton.Location = new Point(274, 2);
            databaseTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            databaseTextBox.CustomButton.Name = "";
            databaseTextBox.CustomButton.Size = new Size(23, 23);
            databaseTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            databaseTextBox.CustomButton.TabIndex = 1;
            databaseTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            databaseTextBox.CustomButton.UseSelectable = true;
            databaseTextBox.CustomButton.Visible = false;
            databaseTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "Database", true));
            databaseTextBox.Location = new Point(244, 213);
            databaseTextBox.Margin = new Padding(4, 3, 4, 3);
            databaseTextBox.MaxLength = 32767;
            databaseTextBox.Name = "databaseTextBox";
            databaseTextBox.PasswordChar = '\0';
            databaseTextBox.ScrollBars = ScrollBars.None;
            databaseTextBox.SelectedText = "";
            databaseTextBox.SelectionLength = 0;
            databaseTextBox.SelectionStart = 0;
            databaseTextBox.ShortcutsEnabled = true;
            databaseTextBox.Size = new Size(300, 28);
            databaseTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            databaseTextBox.TabIndex = 47;
            databaseTextBox.UseSelectable = true;
            databaseTextBox.UseStyleColors = true;
            databaseTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            databaseTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // hostTextBox
            // 
            // 
            // 
            // 
            hostTextBox.CustomButton.Image = null;
            hostTextBox.CustomButton.Location = new Point(274, 2);
            hostTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            hostTextBox.CustomButton.Name = "";
            hostTextBox.CustomButton.Size = new Size(23, 23);
            hostTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            hostTextBox.CustomButton.TabIndex = 1;
            hostTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            hostTextBox.CustomButton.UseSelectable = true;
            hostTextBox.CustomButton.Visible = false;
            hostTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "Host", true));
            hostTextBox.Location = new Point(244, 181);
            hostTextBox.Margin = new Padding(4, 3, 4, 3);
            hostTextBox.MaxLength = 32767;
            hostTextBox.Name = "hostTextBox";
            hostTextBox.PasswordChar = '\0';
            hostTextBox.ScrollBars = ScrollBars.None;
            hostTextBox.SelectedText = "";
            hostTextBox.SelectionLength = 0;
            hostTextBox.SelectionStart = 0;
            hostTextBox.ShortcutsEnabled = true;
            hostTextBox.Size = new Size(300, 28);
            hostTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            hostTextBox.TabIndex = 45;
            hostTextBox.UseSelectable = true;
            hostTextBox.UseStyleColors = true;
            hostTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            hostTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // databaseTypeComboBox
            // 
            databaseTypeComboBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "DatabaseType", true));
            databaseTypeComboBox.FormattingEnabled = true;
            databaseTypeComboBox.ItemHeight = 23;
            databaseTypeComboBox.Location = new Point(244, 115);
            databaseTypeComboBox.Margin = new Padding(4, 3, 4, 3);
            databaseTypeComboBox.Name = "databaseTypeComboBox";
            databaseTypeComboBox.Size = new Size(300, 29);
            databaseTypeComboBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            databaseTypeComboBox.TabIndex = 42;
            databaseTypeComboBox.UseSelectable = true;
            databaseTypeComboBox.UseStyleColors = true;
            // 
            // connectionNameTextBox
            // 
            // 
            // 
            // 
            connectionNameTextBox.CustomButton.Image = null;
            connectionNameTextBox.CustomButton.Location = new Point(274, 2);
            connectionNameTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            connectionNameTextBox.CustomButton.Name = "";
            connectionNameTextBox.CustomButton.Size = new Size(23, 23);
            connectionNameTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            connectionNameTextBox.CustomButton.TabIndex = 1;
            connectionNameTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            connectionNameTextBox.CustomButton.UseSelectable = true;
            connectionNameTextBox.CustomButton.Visible = false;
            connectionNameTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "ConnectionName", true));
            connectionNameTextBox.FontSize = ReaLTaiizor.Extension.Poison.PoisonTextBoxSize.Medium;
            connectionNameTextBox.FontWeight = ReaLTaiizor.Extension.Poison.PoisonTextBoxWeight.Bold;
            connectionNameTextBox.IconRight = true;
            connectionNameTextBox.Location = new Point(243, 83);
            connectionNameTextBox.Margin = new Padding(4, 3, 4, 3);
            connectionNameTextBox.MaxLength = 32767;
            connectionNameTextBox.Name = "connectionNameTextBox";
            connectionNameTextBox.PasswordChar = '\0';
            connectionNameTextBox.ScrollBars = ScrollBars.None;
            connectionNameTextBox.SelectedText = "";
            connectionNameTextBox.SelectionLength = 0;
            connectionNameTextBox.SelectionStart = 0;
            connectionNameTextBox.ShortcutsEnabled = true;
            connectionNameTextBox.Size = new Size(300, 28);
            connectionNameTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            connectionNameTextBox.TabIndex = 40;
            connectionNameTextBox.UseSelectable = true;
            connectionNameTextBox.UseStyleColors = true;
            connectionNameTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            connectionNameTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // driverVersionComboBox
            // 
            driverVersionComboBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "DriverVersion", true));
            driverVersionComboBox.FormattingEnabled = true;
            driverVersionComboBox.ItemHeight = 23;
            driverVersionComboBox.Location = new Point(243, 471);
            driverVersionComboBox.Margin = new Padding(2);
            driverVersionComboBox.Name = "driverVersionComboBox";
            driverVersionComboBox.Size = new Size(95, 29);
            driverVersionComboBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            driverVersionComboBox.TabIndex = 62;
            driverVersionComboBox.UseSelectable = true;
            driverVersionComboBox.UseStyleColors = true;
            // 
            // driverNameComboBox
            // 
            driverNameComboBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "DriverName", true));
            driverNameComboBox.FormattingEnabled = true;
            driverNameComboBox.ItemHeight = 23;
            driverNameComboBox.Location = new Point(244, 438);
            driverNameComboBox.Margin = new Padding(2);
            driverNameComboBox.Name = "driverNameComboBox";
            driverNameComboBox.Size = new Size(300, 29);
            driverNameComboBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            driverNameComboBox.TabIndex = 61;
            driverNameComboBox.UseSelectable = true;
            driverNameComboBox.UseStyleColors = true;
            // 
            // poisonPanel1
            // 
            poisonPanel1.Controls.Add(poisonLabel3);
            poisonPanel1.Controls.Add(poisonLabel4);
            poisonPanel1.Controls.Add(FilePathpoisonTextBox1);
            poisonPanel1.Controls.Add(FileNamepoisonTextBox);
            poisonPanel1.Controls.Add(poisonLabel2);
            poisonPanel1.Controls.Add(poisonTextBox1);
            poisonPanel1.Controls.Add(poisonPanel2);
            poisonPanel1.Controls.Add(poisonPanel3);
            poisonPanel1.Controls.Add(driverVersionLabel);
            poisonPanel1.Controls.Add(connectionNameTextBox);
            poisonPanel1.Controls.Add(driverVersionComboBox);
            poisonPanel1.Controls.Add(connectionNameLabel);
            poisonPanel1.Controls.Add(driverNameLabel);
            poisonPanel1.Controls.Add(databaseTypeComboBox);
            poisonPanel1.Controls.Add(driverNameComboBox);
            poisonPanel1.Controls.Add(databaseTypeLabel);
            poisonPanel1.Controls.Add(label2);
            poisonPanel1.Controls.Add(hostTextBox);
            poisonPanel1.Controls.Add(comboBox1);
            poisonPanel1.Controls.Add(hostLabel);
            poisonPanel1.Controls.Add(label1);
            poisonPanel1.Controls.Add(databaseTextBox);
            poisonPanel1.Controls.Add(DatasourceCategorycomboBox);
            poisonPanel1.Controls.Add(databaseLabel);
            poisonPanel1.Controls.Add(portLabel);
            poisonPanel1.Controls.Add(schemaNameTextBox);
            poisonPanel1.Controls.Add(portTextBox);
            poisonPanel1.Controls.Add(schemaNameLabel);
            poisonPanel1.Controls.Add(passwordLabel);
            poisonPanel1.Controls.Add(userIDTextBox);
            poisonPanel1.Controls.Add(passwordTextBox);
            poisonPanel1.Controls.Add(userIDLabel);
            poisonPanel1.Dock = DockStyle.Fill;
            poisonPanel1.HorizontalScrollbarBarColor = true;
            poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel1.HorizontalScrollbarSize = 12;
            poisonPanel1.Location = new Point(0, 0);
            poisonPanel1.Margin = new Padding(4, 3, 4, 3);
            poisonPanel1.Name = "poisonPanel1";
            poisonPanel1.Size = new Size(601, 814);
            poisonPanel1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonPanel1.TabIndex = 67;
            poisonPanel1.VerticalScrollbarBarColor = true;
            poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel1.VerticalScrollbarSize = 12;
            // 
            // poisonTextBox1
            // 
            // 
            // 
            // 
            poisonTextBox1.CustomButton.Image = null;
            poisonTextBox1.CustomButton.Location = new Point(190, 1);
            poisonTextBox1.CustomButton.Margin = new Padding(4, 3, 4, 3);
            poisonTextBox1.CustomButton.Name = "";
            poisonTextBox1.CustomButton.Size = new Size(109, 109);
            poisonTextBox1.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            poisonTextBox1.CustomButton.TabIndex = 1;
            poisonTextBox1.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            poisonTextBox1.CustomButton.UseSelectable = true;
            poisonTextBox1.CustomButton.Visible = false;
            poisonTextBox1.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "ConnectionString", true));
            poisonTextBox1.Location = new Point(243, 514);
            poisonTextBox1.Margin = new Padding(4, 3, 4, 3);
            poisonTextBox1.MaxLength = 32767;
            poisonTextBox1.Multiline = true;
            poisonTextBox1.Name = "poisonTextBox1";
            poisonTextBox1.PasswordChar = '\0';
            poisonTextBox1.ScrollBars = ScrollBars.None;
            poisonTextBox1.SelectedText = "";
            poisonTextBox1.SelectionLength = 0;
            poisonTextBox1.SelectionStart = 0;
            poisonTextBox1.ShortcutsEnabled = true;
            poisonTextBox1.Size = new Size(300, 111);
            poisonTextBox1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonTextBox1.TabIndex = 66;
            poisonTextBox1.UseSelectable = true;
            poisonTextBox1.UseStyleColors = true;
            poisonTextBox1.WaterMarkColor = Color.FromArgb(109, 109, 109);
            poisonTextBox1.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // poisonPanel2
            // 
            poisonPanel2.BorderStyle = BorderStyle.FixedSingle;
            poisonPanel2.Controls.Add(poisonLabel1);
            poisonPanel2.Dock = DockStyle.Top;
            poisonPanel2.HorizontalScrollbarBarColor = true;
            poisonPanel2.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel2.HorizontalScrollbarSize = 12;
            poisonPanel2.Location = new Point(0, 0);
            poisonPanel2.Margin = new Padding(4, 3, 4, 3);
            poisonPanel2.Name = "poisonPanel2";
            poisonPanel2.Size = new Size(601, 67);
            poisonPanel2.TabIndex = 4;
            poisonPanel2.UseStyleColors = true;
            poisonPanel2.VerticalScrollbarBarColor = true;
            poisonPanel2.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel2.VerticalScrollbarSize = 12;
            // 
            // poisonLabel1
            // 
            poisonLabel1.AutoSize = true;
            poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            poisonLabel1.Location = new Point(21, 20);
            poisonLabel1.Margin = new Padding(4, 0, 4, 0);
            poisonLabel1.Name = "poisonLabel1";
            poisonLabel1.Size = new Size(193, 25);
            poisonLabel1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonLabel1.TabIndex = 3;
            poisonLabel1.Text = "Database Connection";
            poisonLabel1.UseStyleColors = true;
            // 
            // poisonPanel3
            // 
            poisonPanel3.BorderStyle = BorderStyle.FixedSingle;
            poisonPanel3.Controls.Add(ExitCancelpoisonButton);
            poisonPanel3.Controls.Add(SaveButton);
            poisonPanel3.Dock = DockStyle.Bottom;
            poisonPanel3.HorizontalScrollbarBarColor = true;
            poisonPanel3.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel3.HorizontalScrollbarSize = 12;
            poisonPanel3.Location = new Point(0, 751);
            poisonPanel3.Margin = new Padding(4, 3, 4, 3);
            poisonPanel3.Name = "poisonPanel3";
            poisonPanel3.Size = new Size(601, 63);
            poisonPanel3.TabIndex = 65;
            poisonPanel3.UseStyleColors = true;
            poisonPanel3.VerticalScrollbarBarColor = true;
            poisonPanel3.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel3.VerticalScrollbarSize = 12;
            // 
            // ExitCancelpoisonButton
            // 
            ExitCancelpoisonButton.Location = new Point(21, 23);
            ExitCancelpoisonButton.Margin = new Padding(4, 3, 4, 3);
            ExitCancelpoisonButton.Name = "ExitCancelpoisonButton";
            ExitCancelpoisonButton.Size = new Size(88, 27);
            ExitCancelpoisonButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            ExitCancelpoisonButton.TabIndex = 3;
            ExitCancelpoisonButton.Text = "Exit/Cancel";
            ExitCancelpoisonButton.UseSelectable = true;
            ExitCancelpoisonButton.UseStyleColors = true;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(455, 23);
            SaveButton.Margin = new Padding(4, 3, 4, 3);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(88, 27);
            SaveButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseSelectable = true;
            SaveButton.UseStyleColors = true;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = null;
            // 
            // poisonLabel3
            // 
            poisonLabel3.BorderStyle = BorderStyle.FixedSingle;
            poisonLabel3.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            poisonLabel3.Location = new Point(89, 663);
            poisonLabel3.Margin = new Padding(4, 0, 4, 0);
            poisonLabel3.Name = "poisonLabel3";
            poisonLabel3.Size = new Size(150, 28);
            poisonLabel3.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonLabel3.TabIndex = 73;
            poisonLabel3.Text = "FilePath:";
            poisonLabel3.TextAlign = ContentAlignment.MiddleCenter;
            poisonLabel3.UseStyleColors = true;
            // 
            // poisonLabel4
            // 
            poisonLabel4.BorderStyle = BorderStyle.FixedSingle;
            poisonLabel4.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Regular;
            poisonLabel4.Location = new Point(89, 631);
            poisonLabel4.Margin = new Padding(4, 0, 4, 0);
            poisonLabel4.Name = "poisonLabel4";
            poisonLabel4.Size = new Size(150, 28);
            poisonLabel4.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonLabel4.TabIndex = 72;
            poisonLabel4.Text = "FileName:";
            poisonLabel4.TextAlign = ContentAlignment.MiddleCenter;
            poisonLabel4.UseStyleColors = true;
            // 
            // FilePathpoisonTextBox1
            // 
            // 
            // 
            // 
            FilePathpoisonTextBox1.CustomButton.Image = null;
            FilePathpoisonTextBox1.CustomButton.Location = new Point(274, 2);
            FilePathpoisonTextBox1.CustomButton.Margin = new Padding(4, 3, 4, 3);
            FilePathpoisonTextBox1.CustomButton.Name = "";
            FilePathpoisonTextBox1.CustomButton.Size = new Size(23, 23);
            FilePathpoisonTextBox1.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            FilePathpoisonTextBox1.CustomButton.TabIndex = 1;
            FilePathpoisonTextBox1.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            FilePathpoisonTextBox1.CustomButton.UseSelectable = true;
            FilePathpoisonTextBox1.CustomButton.Visible = false;
            FilePathpoisonTextBox1.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "FilePath", true));
            FilePathpoisonTextBox1.Location = new Point(243, 663);
            FilePathpoisonTextBox1.Margin = new Padding(4, 3, 4, 3);
            FilePathpoisonTextBox1.MaxLength = 32767;
            FilePathpoisonTextBox1.Name = "FilePathpoisonTextBox1";
            FilePathpoisonTextBox1.PasswordChar = '\0';
            FilePathpoisonTextBox1.ScrollBars = ScrollBars.None;
            FilePathpoisonTextBox1.SelectedText = "";
            FilePathpoisonTextBox1.SelectionLength = 0;
            FilePathpoisonTextBox1.SelectionStart = 0;
            FilePathpoisonTextBox1.ShortcutsEnabled = true;
            FilePathpoisonTextBox1.Size = new Size(300, 28);
            FilePathpoisonTextBox1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            FilePathpoisonTextBox1.TabIndex = 71;
            FilePathpoisonTextBox1.UseSelectable = true;
            FilePathpoisonTextBox1.UseStyleColors = true;
            FilePathpoisonTextBox1.WaterMarkColor = Color.FromArgb(109, 109, 109);
            FilePathpoisonTextBox1.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // FileNamepoisonTextBox
            // 
            // 
            // 
            // 
            FileNamepoisonTextBox.CustomButton.Image = null;
            FileNamepoisonTextBox.CustomButton.Location = new Point(274, 2);
            FileNamepoisonTextBox.CustomButton.Margin = new Padding(4, 3, 4, 3);
            FileNamepoisonTextBox.CustomButton.Name = "";
            FileNamepoisonTextBox.CustomButton.Size = new Size(23, 23);
            FileNamepoisonTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            FileNamepoisonTextBox.CustomButton.TabIndex = 1;
            FileNamepoisonTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            FileNamepoisonTextBox.CustomButton.UseSelectable = true;
            FileNamepoisonTextBox.CustomButton.Visible = false;
            FileNamepoisonTextBox.DataBindings.Add(new Binding("Text", dataConnectionsBindingSource, "FileName", true));
            FileNamepoisonTextBox.Location = new Point(243, 631);
            FileNamepoisonTextBox.Margin = new Padding(4, 3, 4, 3);
            FileNamepoisonTextBox.MaxLength = 32767;
            FileNamepoisonTextBox.Name = "FileNamepoisonTextBox";
            FileNamepoisonTextBox.PasswordChar = '\0';
            FileNamepoisonTextBox.ScrollBars = ScrollBars.None;
            FileNamepoisonTextBox.SelectedText = "";
            FileNamepoisonTextBox.SelectionLength = 0;
            FileNamepoisonTextBox.SelectionStart = 0;
            FileNamepoisonTextBox.ShortcutsEnabled = true;
            FileNamepoisonTextBox.Size = new Size(300, 28);
            FileNamepoisonTextBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            FileNamepoisonTextBox.TabIndex = 70;
            FileNamepoisonTextBox.UseSelectable = true;
            FileNamepoisonTextBox.UseStyleColors = true;
            FileNamepoisonTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            FileNamepoisonTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // uc_Database
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(poisonPanel1);
            Margin = new Padding(5, 3, 5, 3);
            Name = "uc_Database";
            Size = new Size(601, 814);
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).EndInit();
            poisonPanel1.ResumeLayout(false);
            poisonPanel2.ResumeLayout(false);
            poisonPanel2.PerformLayout();
            poisonPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource dataConnectionsBindingSource;
        private ReaLTaiizor.Controls.PoisonComboBox comboBox1;
        private ReaLTaiizor.Controls.PoisonComboBox DatasourceCategorycomboBox;
        private ReaLTaiizor.Controls.PoisonTextBox portTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox passwordTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox userIDTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox schemaNameTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox databaseTextBox;
        private ReaLTaiizor.Controls.PoisonTextBox hostTextBox;
        private ReaLTaiizor.Controls.PoisonComboBox databaseTypeComboBox;
        private ReaLTaiizor.Controls.PoisonTextBox connectionNameTextBox;
        private ReaLTaiizor.Controls.PoisonComboBox driverVersionComboBox;
        private ReaLTaiizor.Controls.PoisonComboBox driverNameComboBox;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel3;
        private ReaLTaiizor.Controls.PoisonButton ExitCancelpoisonButton;
        private ReaLTaiizor.Controls.PoisonButton SaveButton;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel2;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Controls.PoisonTextBox poisonTextBox1;
        private ReaLTaiizor.Controls.PoisonTextBox FilePathpoisonTextBox1;
        private ReaLTaiizor.Controls.PoisonTextBox FileNamepoisonTextBox;
    }
}
