using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    partial class BeepImagePickerDialog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label _lblSource;
        private System.Windows.Forms.TextBox _txtPath;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.ComboBox _cmbSource;
        private System.Windows.Forms.ListBox _lstEmbedded;
        private System.Windows.Forms.Button _btnBrowse;
        private System.Windows.Forms.Button _btnEmbed; // explicit embed button
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.CheckBox _chkLimit;
        private System.Windows.Forms.GroupBox _grpPreview;
        private BeepImage _preview;
        private System.Windows.Forms.Label _lblPath;
        private System.Windows.Forms.Label _lblSearch;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Panel _bottomPanel;
        private System.Windows.Forms.Panel _leftPanel;
        private System.Windows.Forms.SplitContainer _split;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            _lblPath = new Label();
            _lblSource = new Label();
            _txtPath = new TextBox();
            _btnBrowse = new Button();
            _btnEmbed = new Button();
            _lblSearch = new Label();
            _txtSearch = new TextBox();
            _cmbSource = new ComboBox();
            _chkLimit = new CheckBox();
            _lstEmbedded = new ListBox();
            _grpPreview = new GroupBox();
            _preview = new BeepImage();
            _lblStatus = new Label();
            _bottomPanel = new Panel();
            _btnOK = new Button();
            _btnCancel = new Button();
            _split = new SplitContainer();
            _leftPanel = new Panel();
            _grpPreview.SuspendLayout();
            _bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_split).BeginInit();
            _split.Panel1.SuspendLayout();
            _split.Panel2.SuspendLayout();
            _split.SuspendLayout();
            _leftPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _lblPath
            // 
            _lblPath.AutoSize = true;
            _lblPath.Location = new Point(16, 12);
            _lblPath.Name = "_lblPath";
            _lblPath.Size = new Size(34, 15);
            _lblPath.TabIndex = 0;
            _lblPath.Text = "Path:";
            // 
            // _lblSource
            // 
            _lblSource.AutoSize = true;
            _lblSource.Location = new Point(12, 74);
            _lblSource.Name = "_lblSource";
            _lblSource.Size = new Size(45, 15);
            _lblSource.TabIndex = 4;
            _lblSource.Text = "Source:";
            // 
            // _txtPath
            // 
            _txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtPath.Location = new Point(64, 9);
            _txtPath.Name = "_txtPath";
            _txtPath.Size = new Size(842, 23);
            _txtPath.TabIndex = 1;
            // 
            // _btnBrowse
            // 
            _btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnBrowse.Location = new Point(912, 8);
            _btnBrowse.Name = "_btnBrowse";
            _btnBrowse.Size = new Size(90, 26);
            _btnBrowse.TabIndex = 2;
            _btnBrowse.Text = "Browse...";
            // 
            // _btnEmbed
            // 
            _btnEmbed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnEmbed.Location = new Point(1008, 8);
            _btnEmbed.Name = "_btnEmbed";
            _btnEmbed.Size = new Size(90, 26);
            _btnEmbed.TabIndex = 3;
            _btnEmbed.Text = "Embed";
            // 
            // _lblSearch
            // 
            _lblSearch.AutoSize = true;
            _lblSearch.Location = new Point(12, 106);
            _lblSearch.Name = "_lblSearch";
            _lblSearch.Size = new Size(45, 15);
            _lblSearch.TabIndex = 4;
            _lblSearch.Text = "Search:";
            // 
            // _txtSearch
            // 
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtSearch.Location = new Point(64, 103);
            _txtSearch.Name = "_txtSearch";
            _txtSearch.Size = new Size(842, 23);
            _txtSearch.TabIndex = 5;
            // 
            // _cmbSource
            // 
            _cmbSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _cmbSource.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbSource.FormattingEnabled = true;
            _cmbSource.Location = new Point(64, 71);
            _cmbSource.Name = "_cmbSource";
            _cmbSource.Size = new Size(842, 23);
            _cmbSource.TabIndex = 6;
            // 
            // _chkLimit
            // 
            _chkLimit.AutoSize = true;
            _chkLimit.Location = new Point(912, 104);
            _chkLimit.Name = "_chkLimit";
            _chkLimit.Size = new Size(75, 19);
            _chkLimit.TabIndex = 7;
            _chkLimit.Text = "SVG Only";
            // 
            // _lstEmbedded
            // 
            _lstEmbedded.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lstEmbedded.IntegralHeight = false;
            _lstEmbedded.ItemHeight = 15;
            _lstEmbedded.Location = new Point(3, 133);
            _lstEmbedded.Name = "_lstEmbedded";
            _lstEmbedded.Size = new Size(1129, 551);
            _lstEmbedded.TabIndex = 8;
            // 
            // _grpPreview
            // 
            _grpPreview.Controls.Add(_preview);
            _grpPreview.Dock = DockStyle.Fill;
            _grpPreview.Location = new Point(0, 0);
            _grpPreview.Name = "_grpPreview";
            _grpPreview.Padding = new Padding(8);
            _grpPreview.Size = new Size(270, 717);
            _grpPreview.TabIndex = 0;
            _grpPreview.TabStop = false;
            _grpPreview.Text = "Preview";
            // 
            // _preview
            // 
            _preview.BackColor = Color.White;
            _preview.BorderColor = Color.FromArgb(33, 150, 243);
            _preview.BorderRadius = 8;
            _preview.BorderThickness = 1;
            _preview.ClipShape = Vis.Modules.ImageClipShape.None;
            _preview.CornerRadius = 10F;
            _preview.Dock = DockStyle.Fill;
            _preview.ImageEmbededin = Vis.Modules.ImageEmbededin.Button;
            _preview.ImagePath = null;
            _preview.Name = "_preview";
            _preview.Opacity = 1F;
            _preview.ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio;
            _preview.Size = new Size(254, 685);
            _preview.TabIndex = 0;
            _preview.UseThemeFont = true;
            // 
            // _lblStatus
            // 
            _lblStatus.Dock = DockStyle.Fill;
            _lblStatus.Location = new Point(0, 8);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(1217, 32);
            _lblStatus.TabIndex = 2;
            _lblStatus.Text = "Status";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _bottomPanel
            // 
            _bottomPanel.Controls.Add(_lblStatus);
            _bottomPanel.Controls.Add(_btnOK);
            _bottomPanel.Controls.Add(_btnCancel);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(8, 725);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(0, 8, 0, 8);
            _bottomPanel.Size = new Size(1417, 48);
            _bottomPanel.TabIndex = 1;
            // 
            // _btnOK
            // 
            _btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnOK.Location = new Point(1023, 10);
            _btnOK.Name = "_btnOK";
            _btnOK.Size = new Size(90, 23);
            _btnOK.TabIndex = 0;
            _btnOK.Text = "OK";
            _btnOK.DialogResult = DialogResult.OK;
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnCancel.Location = new Point(1119, 10);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(90, 23);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.DialogResult = DialogResult.Cancel;
            // 
            // _split
            // 
            _split.Dock = DockStyle.Fill;
            _split.Location = new Point(8, 8);
            _split.Name = "_split";
            // 
            // _split.Panel1
            // 
            _split.Panel1.Controls.Add(_leftPanel);
            // 
            // _split.Panel2
            // 
            _split.Panel2.Controls.Add(_grpPreview);
            _split.Size = new Size(1217, 717);
            _split.SplitterDistance = 943;
            _split.TabIndex = 0;
            // 
            // _leftPanel
            // 
            _leftPanel.Controls.Add(_lblPath);
            _leftPanel.Controls.Add(_lblSource);
            _leftPanel.Controls.Add(_txtPath);
            _leftPanel.Controls.Add(_btnBrowse);
            _leftPanel.Controls.Add(_btnEmbed);
            _leftPanel.Controls.Add(_lblSearch);
            _leftPanel.Controls.Add(_txtSearch);
            _leftPanel.Controls.Add(_cmbSource);
            _leftPanel.Controls.Add(_chkLimit);
            _leftPanel.Controls.Add(_lstEmbedded);
            _leftPanel.Dock = DockStyle.Fill;
            _leftPanel.Location = new Point(0, 0);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Padding = new Padding(0, 0, 8, 0);
            _leftPanel.Size = new Size(943, 717);
            _leftPanel.TabIndex = 0;
            // 
            // BeepImagePickerDialog
            // 
            AcceptButton = _btnOK;
            CancelButton = _btnCancel;
            ClientSize = new Size(1233, 773);
            Controls.Add(_split);
            Controls.Add(_bottomPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BeepImagePickerDialog";
            Padding = new Padding(8, 8, 8, 0);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Image";
            _grpPreview.ResumeLayout(false);
            _bottomPanel.ResumeLayout(false);
            _split.Panel1.ResumeLayout(false);
            _split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_split).EndInit();
            _split.ResumeLayout(false);
            _leftPanel.ResumeLayout(false);
            _leftPanel.PerformLayout();
            ResumeLayout(false);
        }
        #endregion
    }
}