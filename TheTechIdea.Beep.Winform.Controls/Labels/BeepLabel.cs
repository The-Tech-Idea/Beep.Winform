using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
  
  
using TheTechIdea.Beep.Winform.Controls.Labels.Helpers;
using TheTechIdea.Beep.Winform.Controls.Labels.Models;
using TheTechIdea.Beep.Winform.Controls.Labels.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Label")]
    [Description("A label control with support for images and multi-line text.")]
  //  [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepLabelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public class BeepLabel : BaseControl
    {
        #region "Fields"
        private string _imagePath = string.Empty;
        private Size _resolvedImageSize = Size.Empty;
        private bool _hasResolvedImage;
        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16);
        private bool _hideText = false;
        private int offset = 6;
        private Color _backcolor;
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
        private bool _useScaledfont = false;
        private bool _multiline = false;
        private bool _autoEllipsis = false;
        private bool _wordWrap = false;
        private Rectangle contentRect;
        private Font _textFont;
        private readonly BeepLabelState _state = new BeepLabelState();
        private BeepLabelLayoutContext _layoutContext = new BeepLabelLayoutContext();
        private string _lastAccessibilitySnapshot = string.Empty;
        private LabelStyleConfig _styleProfile = new LabelStyleConfig();
        private LabelColorConfig _colorProfile = new LabelColorConfig();
        private ToolTip _truncationToolTip;
        private bool _isTextTruncated = false;
        // Add subheader field
        private string _subHeaderText = string.Empty;
        // Add spacing between header and subheader
        private int _headerSubheaderSpacing = 2;
        // Constants - framework handles DPI scaling

        #endregion "Fields"

        #region "Properties"
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The subheader text displayed below the main text.")]
        public string SubHeaderText
        {
            get => _subHeaderText;
            set
            {
                _subHeaderText = value;
                UpdateAccessibilitySnapshot();
                Invalidate();
                UpdateMinimumSize();
                if (AutoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between header and subheader text.")]
        public int HeaderSubheaderSpacing
        {
            get => _headerSubheaderSpacing;
            set
            {
                _headerSubheaderSpacing = value;
                Invalidate();
                if (AutoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
            }
        }

        // Add a property for subheader font - defaults to a smaller version of the main font
        private Font _subHeaderFont;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the subheader text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font SubHeaderFont
        {
            get
            {
                if (_subHeaderFont == null && _textFont != null)
                {
                    _subHeaderFont = _currentTheme?.SmallText != null
                        ? BeepThemesManager.ToFont(_currentTheme.SmallText)
                        : _textFont;
                }
                return _subHeaderFont;
            }
            set
            {
                _subHeaderFont = value;
                Invalidate();
                if (AutoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
            }
        }

        // Add a property for subheader text color
        private Color _subHeaderForeColor;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text color for the subheader.")]
        public Color SubHeaderForeColor
        {
            get => _subHeaderForeColor == Color.Empty ? (_currentTheme?.LabelForeColor ?? ForeColor) : _subHeaderForeColor;
            set
            {
                _subHeaderForeColor = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Color LabelBackColor
        {
            get => _backcolor;
            set
            {
                base.BackColor = value;
                _backcolor = value;
                BackColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _useScaledfont;
            set
            {
                _useScaledfont = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        public bool HideText
        {
            get => _hideText;
            set
            {
                _hideText = value;
                UpdateAccessibilitySnapshot();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Sets the alignment of the text within the control bounds.")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Image alignment relative to text (Left or Right).")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                Invalidate();
            }
        }

        private bool _applyThemeOnImage = false;

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Align image within the BeepLabel (Left, Center, Right).")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set
            {
                imageAlign = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepImagePathEditor, TheTechIdea.Beep.Winform.Controls.Design.Server", typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value ?? string.Empty;
                _hasResolvedImage = false;
                _resolvedImageSize = Size.Empty;
                UpdateAccessibilitySnapshot();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size for the image.")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                _resolvedImageSize = Size.Empty;
                _hasResolvedImage = false;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelStyleConfig StyleProfile
        {
            get => _styleProfile;
            set
            {
                _styleProfile = value ?? new LabelStyleConfig();
                ApplyStyleProfile();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                _colorProfile = value ?? new LabelColorConfig();
                ApplyColorProfile();
                Invalidate();
            }
        }
        private bool _autoSize = false;
        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically resize the control based on the text and image size.")]
        public override bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                if (_autoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines whether the text can span multiple lines.")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                Invalidate();
                if (AutoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("When true, text that does not fit is truncated with an ellipsis (...).")]
        public bool AutoEllipsis
        {
            get => _autoEllipsis;
            set
            {
                _autoEllipsis = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("When true, text wraps to multiple lines within the control bounds.")]
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                Invalidate();
                if (AutoSize)
                {
                    this.Size = GetPreferredSize(new Size(this.Width, 0));
                }
            }
        }



    

     
        // Compute and enforce a DatePicker-like minimum size
        private void UpdateMinimumSize()
        {
            try
            {
                var headerFont = _textFont ?? Font;
                string headerSample = string.IsNullOrEmpty(Text) ? "A" : Text;

                Size headerSize = TextRenderer.MeasureText(
                    headerSample,
                    headerFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine);

                bool hasSub = !string.IsNullOrEmpty(SubHeaderText);
                var subFont = SubHeaderFont ?? headerFont;
                Size subSize = Size.Empty;
                if (hasSub)
                {
                    subSize = TextRenderer.MeasureText(
                        SubHeaderText,
                        subFont,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine);
                }

                int textWidth = Math.Max(headerSize.Width, hasSub ? subSize.Width : 0);
                int textHeight = headerSize.Height + (hasSub ? DpiScalingHelper.ScaleValue(_headerSubheaderSpacing, this) + subSize.Height : 0);

                Size imgSize = Size.Empty;
                if (HasImage)
                {
                    imgSize = GetResolvedImageSize();
                    var maxSz = MaxImageSize;
                    if (imgSize.Width > maxSz.Width || imgSize.Height > maxSz.Height)
                    {
                        float scale = Math.Min(
                            (float)maxSz.Width / Math.Max(1, imgSize.Width),
                            (float)maxSz.Height / Math.Max(1, imgSize.Height));
                        imgSize = new Size(
                            Math.Max(1, (int)(imgSize.Width * scale)),
                            Math.Max(1, (int)(imgSize.Height * scale)));
                    }
                }

                int contentW = textWidth + (imgSize != Size.Empty ? imgSize.Width : 0);
                int contentH = Math.Max(textHeight, imgSize.Height);

                contentW += Padding.Left + Padding.Right;
                contentH += Padding.Top + Padding.Bottom;

                Size baseContentMin = new Size(Math.Max(80, contentW), Math.Max(20, contentH));

                Size effectiveMin = new Size(
                        baseContentMin.Width + (BorderThickness + 2) * 2,
                        baseContentMin.Height + (BorderThickness + 2) * 2);

                effectiveMin.Width = Math.Max(effectiveMin.Width, 60);
                effectiveMin.Height = Math.Max(effectiveMin.Height, 24);

                MinimumSize = effectiveMin;

                if (Height < effectiveMin.Height)
                {
                    Height = effectiveMin.Height;
                }
            }
            catch
            {
                MinimumSize = new Size(120, 28);
            }
        }
        /// <summary>
        /// Override to provide label specific minimum width
        /// </summary>
        /// <returns>Minimum width for Material Design label</returns>
        protected override int GetMaterialMinimumWidth()
        {
            // Base minimum width for label (scaled for DPI)
            int baseMinWidth = DpiScalingHelper.ScaleValue(80, this);

            // Add space for icons if present
            var iconSpace = GetMaterialIconSpace();
            baseMinWidth += iconSpace.Width;

            return baseMinWidth;
        }
        #endregion "Properties"

        #region "Constructors"
        public BeepLabel() : base()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.Selectable, false);
            InitializeComponents();
            BoundProperty = "Text";
            ApplyStyleProfile();
            UpdateAccessibilitySnapshot();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _maxImageSize = DpiScalingHelper.ScaleSize(new Size(16, 16), this);
            _hasResolvedImage = false;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            UpdateDrawingRect();
        }

        // Ensure min updates on font changes
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;

            UpdateMinimumSize();

            if (AutoSize)
            {
                Size = GetPreferredSize(new Size(Width, 0));
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                Size measured = TextRenderer.MeasureText(
                    string.IsNullOrEmpty(Text) ? "A" : Text,
                    _textFont ?? Font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine
                );
                return new Size(
                    DpiScalingHelper.ScaleValue(200, this),
                    measured.Height + DpiScalingHelper.ScaleValue(offset, this));
            }
        }
        #endregion "Constructors"

        #region "Painting"
        // Ensure min updates on text changes
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            UpdateMinimumSize();
            UpdateAccessibilitySnapshot();
            Invalidate();
        }
     
        private void InitializeComponents()
        {
            Padding = new Padding(1);
            Margin = new Padding(0);
            _truncationToolTip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100, ShowAlways = false };
        }

      

        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            UpdateDrawingRect();
            int inset = Math.Max(1, DpiScalingHelper.ScaleValue(1, this));
            var rect = bounds;
            rect.Inflate(-inset, -inset);
            contentRect = rect;
            DrawImageAndText(g, rect);
        }
        private void DrawImageAndText(Graphics g, Rectangle bounds)
        {
            _state.HeaderText = Text ?? string.Empty;
            _state.SubHeaderText = SubHeaderText ?? string.Empty;
            _state.HasImage = HasImage;
            _state.ImagePath = ImagePath;
            _state.HideText = HideText;
            _state.IsEnabled = Enabled;
            _state.Multiline = Multiline;
            _state.WordWrap = WordWrap;
            _state.AutoEllipsis = AutoEllipsis;
            _state.TextImageRelation = TextImageRelation;
            _state.TextAlign = TextAlign;
            _state.ImageAlign = ImageAlign;
            _state.MaxImageSize = MaxImageSize;

            Font headerFont = BeepLabelFontHelpers.GetHeaderFont(this, _currentTheme);
            Font subHeaderFont = BeepLabelFontHelpers.GetSubHeaderFont(this, _currentTheme, headerFont);
            Size imageSize = HasImage ? GetResolvedImageSize() : Size.Empty;
            bool hasSubHeader = !string.IsNullOrEmpty(_state.SubHeaderText);
            bool wrapText = _state.Multiline || _state.WordWrap;
            Size headerTextSize = wrapText
                ? TextRenderer.MeasureText(g, _state.HeaderText, headerFont, new Size(contentRect.Width, int.MaxValue), BeepLabelLayoutHelper.GetTextFormatFlags(_state))
                : TextRenderer.MeasureText(g, _state.HeaderText, headerFont, new Size(int.MaxValue, int.MaxValue), BeepLabelLayoutHelper.GetTextFormatFlags(_state));

            Size subHeaderTextSize = Size.Empty;
            if (hasSubHeader)
            {
                subHeaderTextSize = wrapText
                    ? TextRenderer.MeasureText(g, _state.SubHeaderText, subHeaderFont, new Size(contentRect.Width, int.MaxValue), BeepLabelLayoutHelper.GetTextFormatFlags(_state))
                    : TextRenderer.MeasureText(g, _state.SubHeaderText, subHeaderFont, new Size(int.MaxValue, int.MaxValue), BeepLabelLayoutHelper.GetTextFormatFlags(_state));
            }

            int spacing = DpiScalingHelper.ScaleValue(_headerSubheaderSpacing, this);
            int combinedTextHeight = headerTextSize.Height + (hasSubHeader ? spacing + subHeaderTextSize.Height : 0);
            Size combinedTextSize = new Size(System.Math.Max(headerTextSize.Width, subHeaderTextSize.Width), combinedTextHeight);

            Rectangle imageRect;
            Rectangle textAreaRect;
            BeepLabelLayoutHelper.CalculateLayout(this, _state, bounds, imageSize, combinedTextSize, out imageRect, out textAreaRect);

            _layoutContext = new BeepLabelLayoutContext
            {
                Bounds = bounds,
                ContentBounds = bounds,
                ImageBounds = imageRect,
                TextBounds = textAreaRect,
                HeaderSize = headerTextSize,
                SubHeaderSize = subHeaderTextSize,
                ImageSize = imageSize,
                TextSize = combinedTextSize,
                HasSubHeader = hasSubHeader
            };
            _layoutContext.HeaderBounds = new Rectangle(textAreaRect.X, textAreaRect.Y, textAreaRect.Width, headerTextSize.Height);
            _layoutContext.SubHeaderBounds = hasSubHeader
                ? new Rectangle(textAreaRect.X, _layoutContext.HeaderBounds.Bottom + spacing, textAreaRect.Width, subHeaderTextSize.Height)
                : Rectangle.Empty;

            Color headerColor = Enabled ? ForeColor : DisabledForeColor;
            Color subHeaderColor = Enabled ? SubHeaderForeColor : DisabledForeColor;
            headerColor = BeepLabelAccessibilityHelpers.EnsureContrast(headerColor, BackColor);
            subHeaderColor = BeepLabelAccessibilityHelpers.EnsureContrast(subHeaderColor, BackColor);

            BeepLabelPainter.Paint(g, this, _state, _layoutContext, headerFont, subHeaderFont, headerColor, subHeaderColor, ApplyThemeOnImage);
        }

        private bool HasImage => !string.IsNullOrWhiteSpace(_imagePath);

        private Size GetResolvedImageSize()
        {
            if (!HasImage)
            {
                return Size.Empty;
            }

            if (_hasResolvedImage && _resolvedImageSize != Size.Empty)
            {
                return _resolvedImageSize;
            }

            Size resolved = _maxImageSize;
            try
            {
                using var painter = new ImagePainter(_imagePath);
                if (painter.HasImage)
                {
                    resolved = painter.ImageSize;
                }
            }
            catch
            {
                resolved = _maxImageSize;
            }

            if (resolved.Width > _maxImageSize.Width || resolved.Height > _maxImageSize.Height)
            {
                float scaleFactor = System.Math.Min(
                    (float)_maxImageSize.Width / System.Math.Max(1, resolved.Width),
                    (float)_maxImageSize.Height / System.Math.Max(1, resolved.Height));
                resolved = new Size(
                    System.Math.Max(1, (int)(resolved.Width * scaleFactor)),
                    System.Math.Max(1, (int)(resolved.Height * scaleFactor)));
            }

            _resolvedImageSize = resolved;
            _hasResolvedImage = true;
            return _resolvedImageSize;
        }



        #endregion "Painting"

        #region "Theme"
        public override void ApplyTheme()
        {
            if (_currentTheme != null)
            {
                if (IsChild && Parent != null)
                {
                    ParentBackColor = Parent.BackColor;
                    BackColor = ParentBackColor;
                }
                else
                {
                    BackColor = _currentTheme.LabelBackColor;

                }

                ForeColor = _currentTheme.LabelForeColor;
                _subHeaderForeColor = ColorUtils.GetLighterColor(ForeColor, 50);
                HoverBackColor = _currentTheme.LabelHoverBackColor;
                HoverForeColor = _currentTheme.LabelHoverForeColor;
                SelectedBackColor = _currentTheme.LabelSelectedBackColor;
                SelectedForeColor = _currentTheme.LabelSelectedForeColor;
                BorderColor = _currentTheme.LabelBorderColor;
                DisabledBackColor = _currentTheme.LabelDisabledBackColor;
                DisabledForeColor = _currentTheme.LabelDisabledForeColor;
                if (UseThemeFont)
                {
                    _textFont = BeepFontManager.ToFont(_currentTheme.LabelFont);
                    _subHeaderFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
                }
                // SafeApplyFont(TextFont ?? _textFont);
                ApplyThemeToSvg();
                ApplyStyleProfile();
                ApplyColorProfile();
                UpdateAccessibilitySnapshot();
                //Invalidate();
                //Refresh();
            }
        }


        public void ApplyThemeToSvg()
        {
            Invalidate();
        }

        private void ApplyStyleProfile()
        {
            if (_styleProfile == null)
            {
                return;
            }

            textImageRelation = _styleProfile.TextImageRelation;
            _textAlign = _styleProfile.TextAlign;
            imageAlign = _styleProfile.ImageAlign;
            _headerSubheaderSpacing = _styleProfile.HeaderSubheaderSpacing;
            _multiline = _styleProfile.Multiline;
            _wordWrap = _styleProfile.WordWrap;
            _autoEllipsis = _styleProfile.AutoEllipsis;
        }

        private void ApplyColorProfile()
        {
            if (_colorProfile == null)
            {
                return;
            }

            if (_colorProfile.BackColor != Color.Empty)
            {
                BackColor = _colorProfile.BackColor;
            }

            if (_colorProfile.ForeColor != Color.Empty)
            {
                ForeColor = _colorProfile.ForeColor;
            }

            if (_colorProfile.SubHeaderForeColor != Color.Empty)
            {
                _subHeaderForeColor = _colorProfile.SubHeaderForeColor;
            }

            if (_colorProfile.DisabledBackColor != Color.Empty)
            {
                DisabledBackColor = _colorProfile.DisabledBackColor;
            }

            if (_colorProfile.DisabledForeColor != Color.Empty)
            {
                DisabledForeColor = _colorProfile.DisabledForeColor;
            }

            if (_colorProfile.BorderColor != Color.Empty)
            {
                BorderColor = _colorProfile.BorderColor;
            }
        }

        private void UpdateAccessibilitySnapshot()
        {
            string snapshot = $"{Text}|{_subHeaderText}|{_imagePath}|{Enabled}";
            if (snapshot == _lastAccessibilitySnapshot)
            {
                return;
            }

            BeepLabelAccessibilityHelpers.ApplyAccessibility(this, _subHeaderText, HasImage);
            _lastAccessibilitySnapshot = snapshot;
        }
        #endregion "Theme"

        #region "Mouse Events"
        private bool _isHovered = false;
        private bool _isPressed = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!_isHovered)
            {
                _isHovered = true;
                _state.IsHovered = true;
                UpdateTruncationTooltip();
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_isHovered || _isPressed)
            {
                _isHovered = false;
                _isPressed = false;
                _state.IsHovered = false;
                _state.IsPressed = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                _state.IsPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isPressed)
            {
                _isPressed = false;
                _state.IsPressed = false;
                Invalidate();
            }
        }

        private void UpdateTruncationTooltip()
        {
            if (!_autoEllipsis || string.IsNullOrEmpty(Text))
            {
                _truncationToolTip.Hide(this);
                _isTextTruncated = false;
                return;
            }

            var fontToUse = _textFont ?? Font;
            var bounds = _layoutContext.HeaderBounds;
            if (bounds.IsEmpty) bounds = ClientRectangle;

            Size fullSize = TextRenderer.MeasureText(Text, fontToUse, new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            Size constrainedSize = TextRenderer.MeasureText(Text, fontToUse, new Size(bounds.Width, int.MaxValue),
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);

            _isTextTruncated = fullSize.Width > constrainedSize.Width;

            if (_isTextTruncated)
            {
                _truncationToolTip.SetToolTip(this, Text);
            }
            else
            {
                _truncationToolTip.Hide(this);
            }
        }
        #endregion "Mouse Events"
        /// <summary>
        /// Measures the preferred size of the label based on its current text, font, and constraints.
        /// </summary>
        /// <param name="proposedSize">The proposed size for measuring, typically the available width.</param>
        /// <returns>The measured size of the label.</returns>
        public Size Measure(Size proposedSize)
        {
            // Use GetPreferredSize to calculate the size, passing the proposed constraints
            return GetPreferredSize(proposedSize);
        }


        public override Size GetPreferredSize(Size proposedSize)
        {
            var fontToUse = _textFont ?? Font;

            _state.HeaderText = Text ?? string.Empty;
            _state.SubHeaderText = SubHeaderText ?? string.Empty;
            _state.HideText = HideText;
            _state.HasImage = HasImage;
            _state.Multiline = _multiline;
            _state.WordWrap = _wordWrap;
            _state.AutoEllipsis = _autoEllipsis;
            _state.TextAlign = TextAlign;
            _state.ImageAlign = ImageAlign;
            _state.TextImageRelation = TextImageRelation;

            // Use the control's current width as the constraint when wrapping
            bool wrapText = _multiline || _wordWrap;
            int defaultMaxW = DpiScalingHelper.ScaleValue(200, this);
            int maxWidth = proposedSize.Width > 0 ? proposedSize.Width : (DrawingRect.Width > 0 ? DrawingRect.Width : defaultMaxW);

            // Measure header text size
            Size headerTextSize;
            if (wrapText)
            {
                headerTextSize = TextRenderer.MeasureText(Text, fontToUse, new Size(maxWidth, int.MaxValue),
                    BeepLabelLayoutHelper.GetTextFormatFlags(_state));
            }
            else
            {
                headerTextSize = TextRenderer.MeasureText(Text, fontToUse, new Size(int.MaxValue, int.MaxValue),
                    BeepLabelLayoutHelper.GetTextFormatFlags(_state));
            }

            // Measure subheader text size
            Size subHeaderTextSize = Size.Empty;
            bool hasSubHeader = !string.IsNullOrEmpty(SubHeaderText);

            if (hasSubHeader)
            {
                if (wrapText)
                {
                    subHeaderTextSize = TextRenderer.MeasureText(SubHeaderText, SubHeaderFont,
                        new Size(maxWidth, int.MaxValue),
                        BeepLabelLayoutHelper.GetTextFormatFlags(_state));
                }
                else
                {
                    subHeaderTextSize = TextRenderer.MeasureText(SubHeaderText, SubHeaderFont,
                        new Size(int.MaxValue, int.MaxValue),
                        BeepLabelLayoutHelper.GetTextFormatFlags(_state));
                }
            }

            // Calculate combined text size (include scaled spacing between header and subheader)
            int textWidth = Math.Max(headerTextSize.Width, hasSubHeader ? subHeaderTextSize.Width : 0);
            int textHeight = headerTextSize.Height;

            if (hasSubHeader)
            {
                textHeight += DpiScalingHelper.ScaleValue(_headerSubheaderSpacing, this) + subHeaderTextSize.Height;
            }

            Size imageSize = HasImage ? GetResolvedImageSize() : Size.Empty;

            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Calculate layout without depending on DrawingRect
            Rectangle contentRect = new Rectangle(0, 0, maxWidth, textHeight);
            Rectangle textRect, imageRect;
            BeepLabelLayoutHelper.CalculateLayout(this, _state, contentRect, imageSize, new Size(textWidth, textHeight), out imageRect, out textRect);

            Rectangle bounds = Rectangle.Union(imageRect, textRect);
            int width = bounds.Width + Padding.Left + Padding.Right;
            int height = bounds.Height + Padding.Top + Padding.Bottom;

            return new Size(width, height);
        }

        // Recompute min on resize
        private bool _isResizing = false;
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isResizing) return;
            _isResizing = true;
            try
            {
                UpdateMinimumSize();
                if (Height < MinimumSize.Height)
                    Height = MinimumSize.Height;
            }
            finally
            {
                _isResizing = false;
            }
            Invalidate();
        }

        #region"Text and Alignment"
        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, out Rectangle imageRect, out Rectangle textRect)
        {
            _state.HeaderText = Text ?? string.Empty;
            _state.HideText = HideText;
            _state.HasImage = HasImage;
            _state.TextImageRelation = TextImageRelation;
            _state.TextAlign = TextAlign;
            _state.ImageAlign = ImageAlign;
            BeepLabelLayoutHelper.CalculateLayout(this, _state, contentRect, imageSize, textSize, out imageRect, out textRect);
        }

        private Rectangle AlignRectangle(Rectangle container, Size size, ContentAlignment alignment)
        {
            return BeepLabelLayoutHelper.AlignRectangle(container, size, alignment);
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            _state.Multiline = _multiline;
            _state.WordWrap = _wordWrap;
            _state.AutoEllipsis = _autoEllipsis;
            _state.TextAlign = alignment;
            return BeepLabelLayoutHelper.GetTextFormatFlags(_state);
        }
        #endregion "Text and Alignment"



        #region "IBeep UI Component Implementation"
        public override void SetValue(object value)
        {
            Text = value?.ToString();
        }

        public override object GetValue()
        {
            return Text;
        }

        public override void ClearValue()
        {
            Text = "";
        }

        public override bool ValidateData(out string message)
        {
            message = "";
            return true;
        }

        // NOTE: Draw method now above (calls Paint function)
        #endregion "IBeep UI Component Implementation"
    }
}
