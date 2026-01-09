using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    partial class BeepImagePickerDialog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox _txtPath;
        private System.Windows.Forms.TextBox _txtSearch;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepImagePickerDialog));
            _lblPath = new Label();
            _txtPath = new TextBox();
            _btnBrowse = new Button();
            _btnEmbed = new Button();
            _lblSearch = new Label();
            _txtSearch = new TextBox();
            _chkLimit = new CheckBox();
            _lstEmbedded = new ListBox();
            _grpPreview = new GroupBox();
            _preview = new BeepImage();
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
            _lblPath.Location = new Point(45, 10);
            _lblPath.Name = "_lblPath";
            _lblPath.Size = new Size(34, 15);
            _lblPath.TabIndex = 0;
            _lblPath.Text = "Path:";
            // 
            // _txtPath
            // 
            _txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtPath.Location = new Point(105, 7);
            _txtPath.Name = "_txtPath";
            _txtPath.Size = new Size(838, 23);
            _txtPath.TabIndex = 1;
            // 
            // _btnBrowse
            // 
            _btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnBrowse.Location = new Point(949, 6);
            _btnBrowse.Name = "_btnBrowse";
            _btnBrowse.Size = new Size(90, 26);
            _btnBrowse.TabIndex = 2;
            _btnBrowse.Text = "Browse...";
            // 
            // _btnEmbed
            // 
            _btnEmbed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnEmbed.Location = new Point(1045, 6);
            _btnEmbed.Name = "_btnEmbed";
            _btnEmbed.Size = new Size(90, 26);
            _btnEmbed.TabIndex = 3;
            _btnEmbed.Text = "Embed";
            // 
            // _lblSearch
            // 
            _lblSearch.AutoSize = true;
            _lblSearch.Location = new Point(34, 42);
            _lblSearch.Name = "_lblSearch";
            _lblSearch.Size = new Size(45, 15);
            _lblSearch.TabIndex = 4;
            _lblSearch.Text = "Search:";
            // 
            // _txtSearch
            // 
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtSearch.Location = new Point(105, 34);
            _txtSearch.Name = "_txtSearch";
            _txtSearch.Size = new Size(838, 23);
            _txtSearch.TabIndex = 5;
            // 
            // _chkLimit
            // 
            _chkLimit.AutoSize = true;
            _chkLimit.Location = new Point(105, 63);
            _chkLimit.Name = "_chkLimit";
            _chkLimit.Size = new Size(75, 19);
            _chkLimit.TabIndex = 6;
            _chkLimit.Text = "SVG Only";
            // 
            // _lstEmbedded
            // 
            _lstEmbedded.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lstEmbedded.IntegralHeight = false;
            _lstEmbedded.ItemHeight = 15;
            _lstEmbedded.Location = new Point(3, 90);
            _lstEmbedded.Name = "_lstEmbedded";
            _lstEmbedded.Size = new Size(1129, 619);
            _lstEmbedded.TabIndex = 7;
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
            _preview.AllowManualRotation = true;
            _preview.AnimationDuration = 500;
            _preview.AnimationType = Vis.Modules.DisplayAnimationType.None;
            _preview.ApplyThemeOnImage = false;
            _preview.ApplyThemeToChilds = false;
            _preview.AutoDrawHitListComponents = true;
            _preview.BackColor = Color.White;
            _preview.BadgeBackColor = Color.Red;
            _preview.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _preview.BadgeForeColor = Color.White;
            _preview.BadgeShape = Vis.Modules.BadgeShape.Circle;
            _preview.BadgeText = "";
            _preview.BaseSize = 50;
            _preview.BlockID = null;
            _preview.BorderColor = Color.FromArgb(33, 150, 243);
            _preview.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _preview.BorderRadius = 8;
            _preview.BorderStyle = BorderStyle.None;
            _preview.BorderThickness = 1;
            _preview.BottomoffsetForDrawingRect = 0;
            _preview.BoundProperty = "ImagePath";
            _preview.CanBeFocused = true;
            _preview.CanBeHovered = false;
            _preview.CanBePressed = true;
            _preview.Category = Utilities.DbFieldCategory.String;
            _preview.ClipShape = Vis.Modules.ImageClipShape.None;
            _preview.ComponentName = "BeepControl";
            _preview.CornerRadius = 10F;
            _preview.CustomClipPath = null;
            _preview.DataSourceProperty = null;
            _preview.DisabledBackColor = Color.White;
            _preview.DisabledBorderColor = Color.Empty;
            _preview.DisabledForeColor = Color.Black;
          
            _preview.Dock = DockStyle.Fill;
            _preview.DrawingRect = new Rectangle(0, 0, 254, 685);
            _preview.Easing = Vis.Modules.EasingType.Linear;
          
            _preview.EnableHighQualityRendering = true;
            _preview.EnableRippleEffect = false;
            _preview.EnableSplashEffect = true;
            _preview.ExternalDrawingLayer = Models.DrawingLayer.AfterAll;
            _preview.FieldID = null;
            _preview.FillColor = Color.Black;
            _preview.FilledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            _preview.FloatingLabel = true;
            _preview.FocusBackColor = Color.White;
            _preview.FocusBorderColor = Color.RoyalBlue;
            _preview.FocusForeColor = Color.Black;
            _preview.FocusIndicatorColor = Color.Blue;
            _preview.ForeColor = Color.FromArgb(33, 150, 243);
            _preview.Form = null;
            _preview.GlassmorphismBlur = 10F;
            _preview.GlassmorphismOpacity = 0.1F;
            _preview.GradientAngle = 0F;
            _preview.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _preview.GradientEndColor = Color.FromArgb(230, 230, 230);
            _preview.GradientStartColor = Color.FromArgb(255, 255, 255);
            _preview.Grayscale = false;
            _preview.GridMode = false;
            _preview.GuidID = "9a988a93-ba11-49f6-9a08-5a30d27c4107";
            _preview.HelperText = "";
            _preview.HitAreaEventOn = false;
            _preview.HitTestControl = null;
            _preview.HoverBackColor = Color.White;
            _preview.HoverBorderColor = Color.Gray;
            _preview.HoveredBackcolor = Color.Wheat;
            _preview.HoverForeColor = Color.Black;
            _preview.Id = -1;
            _preview.Image = null;
            _preview.ImageEmbededin = Vis.Modules.ImageEmbededin.Button;
            _preview.ImagePath = null;
            _preview.InactiveBorderColor = Color.Gray;
            _preview.IsAcceptButton = false;
            _preview.IsBorderAffectedByTheme = true;
            _preview.IsBouncing = false;
            _preview.IsCancelButton = false;
            _preview.IsChild = false;
            _preview.IsCustomeBorder = false;
            _preview.IsDefault = false;
            _preview.IsDeleted = false;
            _preview.IsDirty = false;
            _preview.IsEditable = false;
            _preview.IsFading = false;
            _preview.IsFocused = false;
            _preview.IsFrameless = false;
            _preview.IsHovered = false;
            _preview.IsNew = false;
            _preview.IsPressed = false;
            _preview.IsPulsing = false;
            _preview.IsReadOnly = false;
            _preview.IsRequired = false;
            _preview.IsRounded = false;
            _preview.IsRoundedAffectedByTheme = true;
            _preview.IsSelected = false;
            _preview.IsSelectedOptionOn = false;
            _preview.IsShadowAffectedByTheme = true;
            _preview.IsShaking = false;
            _preview.IsSpinning = false;
            _preview.IsStillImage = false;
            _preview.IsVisible = false;
            _preview.Items = (List<object>)resources.GetObject("_preview.Items");
            _preview.LabelText = "";
            _preview.LeftoffsetForDrawingRect = 0;
            _preview.LinkedProperty = null;
            _preview.Location = new Point(8, 24);
            _preview.ManualRotationAngle = 0F;
            _preview.MaterialBorderVariant = Vis.Modules.MaterialTextFieldVariant.Standard;
            _preview.MaxHitListDrawPerFrame = 0;
            _preview.ModernGradientType = Vis.Modules.ModernGradientType.None;
            _preview.Name = "_preview";
            _preview.Opacity = 1F;
            _preview.OverrideFontSize = Vis.Modules.TypeStyleFontSize.None;
            _preview.ParentBackColor = Color.Empty;
            _preview.ParentControl = null;
            _preview.PreserveSvgBackgrounds = false;
            _preview.PressedBackColor = Color.White;
            _preview.PressedBorderColor = Color.Gray;
            _preview.PressedForeColor = Color.Gray;
            _preview.RadialCenter = (PointF)resources.GetObject("_preview.RadialCenter");
            _preview.RightoffsetForDrawingRect = 0;
            _preview.SavedGuidID = null;
            _preview.SavedID = null;
            _preview.ScaleFactor = 1F;
            _preview.ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio;
            _preview.SelectedBackColor = Color.White;
            _preview.SelectedBorderColor = Color.Empty;
            _preview.SelectedForeColor = Color.Black;
            _preview.SelectedValue = null;
            _preview.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            _preview.ShadowOffset = 0;
            _preview.ShadowOpacity = 0.5F;
            _preview.ShowAllBorders = false;
            _preview.ShowBottomBorder = false;
            _preview.ShowFocusIndicator = false;
            _preview.ShowLeftBorder = false;
            _preview.ShowRightBorder = false;
            _preview.ShowShadow = false;
            _preview.ShowTopBorder = false;
            _preview.Size = new Size(254, 685);
            _preview.SlideFrom = Vis.Modules.SlideDirection.Left;
            _preview.SpinSpeed = 5F;
            _preview.StaticNotMoving = false;
            _preview.StrokeColor = Color.Black;
            _preview.TabIndex = 0;
            _preview.Tag = _grpPreview;
            _preview.TempBackColor = Color.Empty;
            _preview.Theme = null;
            _preview.ToolTipText = "";
            _preview.TopoffsetForDrawingRect = 0;
         
            _preview.UseExternalBufferedGraphics = false;
            _preview.UseGlassmorphism = false;
            _preview.UseGradientBackground = false;
            _preview.UseThemeFont = true;
            _preview.Velocity = 0F;
            // 
            // _bottomPanel
            // 
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
            _btnOK.Location = new Point(1223, 10);
            _btnOK.Name = "_btnOK";
            _btnOK.Size = new Size(90, 23);
            _btnOK.TabIndex = 0;
            _btnOK.Text = "OK";
            _btnOK.DialogResult = DialogResult.OK;
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnCancel.Location = new Point(1319, 10);
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
            _split.Size = new Size(1417, 717);
            _split.SplitterDistance = 1143;
            _split.TabIndex = 0;
            // 
            // _leftPanel
            // 
            _leftPanel.Controls.Add(_lblPath);
            _leftPanel.Controls.Add(_txtPath);
            _leftPanel.Controls.Add(_btnBrowse);
            _leftPanel.Controls.Add(_btnEmbed);
            _leftPanel.Controls.Add(_lblSearch);
            _leftPanel.Controls.Add(_txtSearch);
            _leftPanel.Controls.Add(_chkLimit);
            _leftPanel.Controls.Add(_lstEmbedded);
            _leftPanel.Dock = DockStyle.Fill;
            _leftPanel.Location = new Point(0, 0);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Padding = new Padding(0, 0, 8, 0);
            _leftPanel.Size = new Size(1143, 717);
            _leftPanel.TabIndex = 0;
            // 
            // BeepImagePickerDialog
            // 
            AcceptButton = _btnOK;
            CancelButton = _btnCancel;
            ClientSize = new Size(1433, 773);
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