namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class MainFrm_MDI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Controls.DocumentHost.DocumentDescriptor documentDescriptor1 = new Controls.DocumentHost.DocumentDescriptor();
            Controls.DocumentHost.DocumentDescriptor documentDescriptor2 = new Controls.DocumentHost.DocumentDescriptor();
            GraphicsPath graphicsPath1 = new GraphicsPath();
            GraphicsPath graphicsPath2 = new GraphicsPath();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm_MDI));
            GraphicsPath graphicsPath3 = new GraphicsPath();
            beepDocumentManager1 = new TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDocumentManager(components);
            beepTabbedView1 = new TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepTabbedView(components);
            beepDocumentHost1 = new TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDocumentHost();
            (beepDocumentManager1).BeginInit();
            SuspendLayout();
            // 
            // beepFormuiManager1
            // 
            beepFormuiManager1.ApplyBeepFormStyle = true;
            // 
            // beepDocumentManager1
            // 
            beepDocumentManager1.ContainerType = ContainerTypeEnum.TabbedPanel;
            documentDescriptor1.AccentColor = Color.Empty;
            documentDescriptor1.BadgeColor = Color.Empty;
            documentDescriptor1.BadgeText = null;
            documentDescriptor1.CanClose = true;
            documentDescriptor1.Category = null;
            documentDescriptor1.IconPath = null;
            documentDescriptor1.Id = "2ffe28fb3afc4b599b014d8a08d7d7e3";
            documentDescriptor1.InitialContent = Winform.Controls.DocumentHost.DocumentInitialContent.Empty;
            documentDescriptor1.IsModified = false;
            documentDescriptor1.IsPinned = false;
            documentDescriptor1.PersistenceKey = "0b959cf7-7146-4c8b-8370-41f73b0bffae";
            documentDescriptor1.PreviousGroupId = null;
            documentDescriptor1.TabColor = Color.Empty;
            documentDescriptor1.Tag = null;
            documentDescriptor1.Title = "Document 12277";
            documentDescriptor1.TooltipText = null;
            documentDescriptor2.AccentColor = Color.Empty;
            documentDescriptor2.BadgeColor = Color.Empty;
            documentDescriptor2.BadgeText = null;
            documentDescriptor2.CanClose = true;
            documentDescriptor2.Category = null;
            documentDescriptor2.IconPath = null;
            documentDescriptor2.Id = "143ecbe9984e401a95a4144e93277908";
            documentDescriptor2.InitialContent = Winform.Controls.DocumentHost.DocumentInitialContent.Empty;
            documentDescriptor2.IsModified = false;
            documentDescriptor2.IsPinned = false;
            documentDescriptor2.PersistenceKey = "98ef353a-b28a-4c99-8ee7-f86d3878c80e";
            documentDescriptor2.PreviousGroupId = null;
            documentDescriptor2.TabColor = Color.Empty;
            documentDescriptor2.Tag = null;
            documentDescriptor2.Title = "Document 13527";
            documentDescriptor2.TooltipText = null;
            beepDocumentManager1.DesignTimeDocuments.Add(documentDescriptor1);
            beepDocumentManager1.DesignTimeDocuments.Add(documentDescriptor2);
            beepDocumentManager1.View = beepTabbedView1;
            // 
            // beepTabbedView1
            // 
            beepTabbedView1.Host = beepDocumentHost1;
            // 
            // beepDocumentHost1
            // 
            beepDocumentHost1.AccessibleDescription = "Active document Document 13527";
            beepDocumentHost1.AccessibleName = "Document 13527";
            beepDocumentHost1.AnimationDuration = 500;
            beepDocumentHost1.AnimationType = DisplayAnimationType.None;
            beepDocumentHost1.ApplyThemeToChilds = true;
            beepDocumentHost1.AutoDrawHitListComponents = true;
            beepDocumentHost1.BackColor = Color.FromArgb(255, 255, 255);
            beepDocumentHost1.BadgeBackColor = Color.FromArgb(92, 139, 255);
            beepDocumentHost1.BadgeFont = new Font("Segoe UI", 7F, FontStyle.Bold);
            beepDocumentHost1.BadgeForeColor = Color.FromArgb(58, 58, 58);
            beepDocumentHost1.BadgeShape = BadgeShape.Circle;
            beepDocumentHost1.BadgeText = "";
            beepDocumentHost1.BlockID = null;
            beepDocumentHost1.BorderColor = Color.FromArgb(220, 227, 240);
            beepDocumentHost1.BorderDashStyle = DashStyle.Solid;
            beepDocumentHost1.BorderPainter = BeepControlStyle.Modern;
            graphicsPath1.FillMode = FillMode.Alternate;
            beepDocumentHost1.BorderPath = graphicsPath1;
            beepDocumentHost1.BorderRadius = 8;
            beepDocumentHost1.BorderRect = new Rectangle(1, 1, 1268, 482);
            beepDocumentHost1.BorderStyle = BorderStyle.FixedSingle;
            beepDocumentHost1.BorderThickness = 2;
            beepDocumentHost1.BottomoffsetForDrawingRect = 0;
            beepDocumentHost1.BoundProperty = null;
            beepDocumentHost1.CanBeFocused = false;
            beepDocumentHost1.CanBeHovered = true;
            beepDocumentHost1.CanBePressed = true;
            beepDocumentHost1.CanBeSelected = true;
            beepDocumentHost1.Category = DbFieldCategory.String;
            beepDocumentHost1.ComponentName = "BaseControl";
            beepDocumentHost1.ContentRect = new Rectangle(0, 0, 0, 0);
            graphicsPath2.FillMode = FillMode.Alternate;
            beepDocumentHost1.ContentShape = graphicsPath2;
            beepDocumentHost1.CustomPadding = new Padding(0);
            beepDocumentHost1.DataContext = null;
            beepDocumentHost1.DataSourceProperty = null;
            beepDocumentHost1.DisabledBackColor = Color.FromArgb(244, 246, 252);
            beepDocumentHost1.DisabledBorderColor = Color.FromArgb(220, 227, 240);
            beepDocumentHost1.DisabledForeColor = Color.FromArgb(222, 223, 229);
            beepDocumentHost1.Dock = DockStyle.Fill;
            beepDocumentHost1.DocumentContentTemplate = null;
            beepDocumentHost1.DocumentIdSelector = null;
            beepDocumentHost1.DocumentTemplate = null;
            beepDocumentHost1.DocumentTitleSelector = null;
            beepDocumentHost1.DrawingRect = new Rectangle(4, 4, 1262, 476);
            beepDocumentHost1.Easing = EasingType.Linear;
            beepDocumentHost1.EnableHighQualityRendering = true;
            beepDocumentHost1.EnableRippleEffect = false;
            beepDocumentHost1.EnableSplashEffect = false;
            beepDocumentHost1.ErrorColor = Color.Empty;
            beepDocumentHost1.ErrorText = "";
            beepDocumentHost1.ExternalDrawingLayer = DrawingLayer.AfterAll;
            beepDocumentHost1.FieldID = null;
            beepDocumentHost1.FilledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            beepDocumentHost1.FloatingLabel = true;
            beepDocumentHost1.FocusBackColor = Color.LightYellow;
            beepDocumentHost1.FocusBorderColor = Color.RoyalBlue;
            beepDocumentHost1.FocusForeColor = Color.Black;
            beepDocumentHost1.FocusIndicatorColor = Color.RoyalBlue;
            beepDocumentHost1.ForeColor = Color.FromArgb(33, 33, 33);
            beepDocumentHost1.Form = null;
            beepDocumentHost1.GlassmorphismBlur = 10F;
            beepDocumentHost1.GlassmorphismOpacity = 0.1F;
            beepDocumentHost1.GradientAngle = 0F;
            beepDocumentHost1.GradientDirection = LinearGradientMode.Horizontal;
            beepDocumentHost1.GradientEndColor = Color.FromArgb(242, 245, 252);
            beepDocumentHost1.GradientStartColor = Color.FromArgb(251, 252, 255);
            beepDocumentHost1.GridMode = false;
            beepDocumentHost1.GuidID = "57892bda-da25-43f8-ab1d-f17b7d6e4150";
            beepDocumentHost1.HasError = false;
            beepDocumentHost1.HelperText = "";
            beepDocumentHost1.HelperTextOn = false;
            beepDocumentHost1.HitAreaEventOn = false;
            beepDocumentHost1.HitTestControl = null;
            beepDocumentHost1.HoverBackColor = Color.LightBlue;
            beepDocumentHost1.HoverBorderColor = Color.Blue;
            beepDocumentHost1.HoveredBackcolor = Color.LightBlue;
            beepDocumentHost1.HoverForeColor = Color.Black;
            beepDocumentHost1.IconKey = "";
            beepDocumentHost1.IconSize = 20;
            beepDocumentHost1.Id = -1;
            beepDocumentHost1.InactiveBorderColor = Color.Gray;
            graphicsPath3.FillMode = FillMode.Alternate;
            beepDocumentHost1.InnerShape = graphicsPath3;
            beepDocumentHost1.IsAcceptButton = false;
            beepDocumentHost1.IsBorderAffectedByTheme = true;
            beepDocumentHost1.IsCancelButton = false;
            beepDocumentHost1.IsChild = true;
            beepDocumentHost1.IsCustomeBorder = false;
            beepDocumentHost1.IsDefault = false;
            beepDocumentHost1.IsDeleted = false;
            beepDocumentHost1.IsDirty = false;
            beepDocumentHost1.IsEditable = true;
            beepDocumentHost1.IsFocused = false;
            beepDocumentHost1.IsFrameless = false;
            beepDocumentHost1.IsHovered = false;
            beepDocumentHost1.IsNew = false;
            beepDocumentHost1.IsPressed = false;
            beepDocumentHost1.IsReadOnly = false;
            beepDocumentHost1.IsRequired = false;
            beepDocumentHost1.IsRounded = true;
            beepDocumentHost1.IsRoundedAffectedByTheme = true;
            beepDocumentHost1.IsSelected = false;
            beepDocumentHost1.IsSelectedOptionOn = false;
            beepDocumentHost1.IsShadowAffectedByTheme = true;
            beepDocumentHost1.IsTransparentBackground = false;
            beepDocumentHost1.IsValid = true;
            beepDocumentHost1.IsVisible = true;
            beepDocumentHost1.LabelPosition = LabelPosition.Left;
            beepDocumentHost1.LabelText = "";
            beepDocumentHost1.LabelTextOn = false;
            beepDocumentHost1.LeadingIconPath = "";
            beepDocumentHost1.LeadingImagePath = "";
            beepDocumentHost1.LeftoffsetForDrawingRect = 0;
            beepDocumentHost1.LinkedProperty = null;
            beepDocumentHost1.Location = new Point(4, 48);
            beepDocumentHost1.MaxHitListDrawPerFrame = 0;
            beepDocumentHost1.ModernGradientType = ModernGradientType.None;
            beepDocumentHost1.Name = "beepDocumentHost1";
            beepDocumentHost1.OuterShape = null;
            beepDocumentHost1.OverrideFontSize = TypeStyleFontSize.None;
            beepDocumentHost1.PainterKind = BaseControlPainterKind.Classic;
            beepDocumentHost1.ParentBackColor = Color.FromArgb(255, 255, 255);
            beepDocumentHost1.ParentControl = null;
            beepDocumentHost1.PressedBackColor = Color.Gray;
            beepDocumentHost1.PressedBorderColor = Color.DarkGray;
            beepDocumentHost1.PressedForeColor = Color.White;
            beepDocumentHost1.PreviewCaptureSize = new Size(200, 120);
            beepDocumentHost1.RadialCenter = (PointF)resources.GetObject("beepDocumentHost1.RadialCenter");
            beepDocumentHost1.RestoreDocumentFactory = null;
            beepDocumentHost1.RightoffsetForDrawingRect = 0;
            beepDocumentHost1.SavedGuidID = null;
            beepDocumentHost1.SavedID = null;
            beepDocumentHost1.ScaleMode = ImageScaleMode.KeepAspectRatio;
            beepDocumentHost1.SelectedBackColor = Color.LightGreen;
            beepDocumentHost1.SelectedBorderColor = Color.Green;
            beepDocumentHost1.SelectedForeColor = Color.Black;
            beepDocumentHost1.SelectedValue = null;
            beepDocumentHost1.ShadowColor = Color.FromArgb(40, 38, 44, 57);
            beepDocumentHost1.ShadowOffset = 3;
            beepDocumentHost1.ShadowOpacity = 0.25F;
            beepDocumentHost1.ShowAllBorders = false;
            beepDocumentHost1.ShowBottomBorder = false;
            beepDocumentHost1.ShowFocusIndicator = false;
            beepDocumentHost1.ShowLabelAboveBorder = false;
            beepDocumentHost1.ShowLeftBorder = false;
            beepDocumentHost1.ShowRightBorder = false;
            beepDocumentHost1.ShowShadow = false;
            beepDocumentHost1.ShowTopBorder = false;
            beepDocumentHost1.Size = new Size(1271, 485);
            beepDocumentHost1.SlideFrom = SlideDirection.Left;
            beepDocumentHost1.StaticNotMoving = false;
            beepDocumentHost1.TabIndex = 1;
            beepDocumentHost1.Tag = this;
            beepDocumentHost1.TempBackColor = Color.LightGray;
            beepDocumentHost1.TextFont = new Font("Consolas", 12F);
            beepDocumentHost1.Theme = "ModernTheme";
            beepDocumentHost1.ThemeName = "";
            beepDocumentHost1.TooltipFont = null;
            beepDocumentHost1.TooltipMaxSize = null;
            beepDocumentHost1.ToolTipText = null;
            beepDocumentHost1.TopoffsetForDrawingRect = 0;
            beepDocumentHost1.TrailingIconPath = "";
            beepDocumentHost1.TrailingImagePath = "";
            beepDocumentHost1.UseExternalBufferedGraphics = true;
            beepDocumentHost1.UseFormStylePaint = true;
            beepDocumentHost1.UseGlassmorphism = false;
            beepDocumentHost1.UseGradientBackground = false;
            beepDocumentHost1.UseRichToolTip = true;
            beepDocumentHost1.UseThemeFont = true;
            beepDocumentHost1.ViewModelTemplate = null;
            // 
            // MainFrm_MDI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 537);
            Controls.Add(beepDocumentHost1);
            IsMdiContainer = true;
            KeyPreview = true;
            Name = "MainFrm_MDI";
            Text = "MainFrm_MDI";
            (beepDocumentManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.DocumentHost.BeepDocumentManager beepDocumentManager1;
        private Controls.DocumentHost.BeepTabbedView beepTabbedView1;
        private Controls.DocumentHost.BeepDocumentHost beepDocumentHost1;
    }
}