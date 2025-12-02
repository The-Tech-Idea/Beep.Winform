using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
 
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Label")]
    [Description("A label control with support for images and multi-line text.")]
  //  [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepLabelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public class BeepLabel : BaseControl
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        #region "Fields"
        private BeepImage beepImage;
        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16);
        private bool _hideText = false;
        private int offset = 6;
        private Color _backcolor;
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
        private bool _useScaledfont = false;
        private bool _multiline = false;
        private Rectangle contentRect;
        private Font _textFont;
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
                    // Default to a slightly smaller font than header
                    _subHeaderFont = new Font(_textFont.FontFamily,
                                            _textFont.Size - 2,
                                            FontStyle.Regular);
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
                beepImage.ApplyThemeOnImage = value;
                if (value)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyThemeToSvg();
                }
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
            get => beepImage?.ImagePath;
            set
            {
                if (beepImage == null)
                {
                    beepImage = new BeepImage();
                }
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        beepImage.Theme = Theme;
                        beepImage.ApplyThemeOnImage = true;
                        beepImage.ApplyThemeToSvg();
                        beepImage.ApplyTheme();
                    }
                    Invalidate();
                }
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
                // Font = value;
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



    

     
        // Compute and enforce a DatePicker-like minimum size
        private void UpdateMinimumSize()
        {
            try
            {
                using var g = CreateGraphics();

                // 1) Measure header text
                var headerFont = _textFont ?? Font;
                string headerSample = string.IsNullOrEmpty(Text) ? "A" : Text;

                // Use single-line for intrinsic minimum; multiline will expand at layout time
                Size headerSize = TextRenderer.MeasureText(
                    g,
                    headerSample,
                    headerFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine);

                // 2) Measure subheader (if any)
                bool hasSub = !string.IsNullOrEmpty(SubHeaderText);
                var subFont = SubHeaderFont ?? headerFont;
                Size subSize = Size.Empty;
                if (hasSub)
                {
                    subSize = TextRenderer.MeasureText(
                        g,
                        SubHeaderText,
                        subFont,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine);
                }

                int textWidth = Math.Max(headerSize.Width, hasSub ? subSize.Width : 0);
                int textHeight = headerSize.Height + (hasSub ? subSize.Height : 0);

                // 3) Image size (if any) limited by MaxImageSize
                Size imgSize = Size.Empty;
                if (beepImage?.HasImage == true)
                {
                    imgSize = beepImage.GetImageSize();
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

                // 4) Combine content sizes; account for image+text spacing if both exist
                int contentW = textWidth + (imgSize != Size.Empty ? (imgSize.Width) : 0);
                int contentH = Math.Max(textHeight, imgSize.Height);

                // Include control padding
                contentW += Padding.Left + Padding.Right;
                contentH += Padding.Top + Padding.Bottom;

                // Base content minimums
                Size baseContentMin = new Size(Math.Max(80, contentW), Math.Max(20, contentH));

                // 5) Effective minimum
                Size effectiveMin = new Size(
                        baseContentMin.Width + (BorderThickness + 2) * 2,
                        baseContentMin.Height + (BorderThickness + 2) * 2);

                // Safety clamps
                effectiveMin.Width = Math.Max(effectiveMin.Width, 60);
                effectiveMin.Height = Math.Max(effectiveMin.Height, 24);

                MinimumSize = effectiveMin;

                // 6) Enforce height like other controls
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
            // Base minimum width for label
            int baseMinWidth = 80;

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
            beepImage.ImageEmbededin = ImageEmbededin.Label;
            BoundProperty = "Text";

         
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
                using (Graphics g = CreateGraphics())
                {
                    Size measured = TextRenderer.MeasureText(
                        g,
                        string.IsNullOrEmpty(Text) ? "A" : Text,
                        _textFont ?? Font,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine
                    );
                    // Framework handles DPI scaling
                    return new Size(200, measured.Height + 6);
                }
            }
        }
        #endregion "Constructors"

        #region "Painting"
        // Ensure min updates on text changes
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            UpdateMinimumSize();
            Invalidate();
        }
     
        private void InitializeComponents()
        {
            beepImage = new BeepImage
            {
                IsChild = true,
                Visible = false,
                Dock = DockStyle.None,
                Margin = new Padding(0),
                Location = new Point(0, 0),
                Size = _maxImageSize
            };
            Padding = new Padding(1);
            Margin = new Padding(0);
        }

      

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            // Apply a small inner inset to prevent clipping - framework handles DPI scaling
            var inset = Math.Max(1, 1);
            var rect = DrawingRect;
            rect.Inflate(-inset, -inset);
            contentRect = rect;
            DrawImageAndText(g);
        }
        private void DrawImageAndText(Graphics g)
        {



            bool hasSubHeader = !string.IsNullOrEmpty(SubHeaderText);

            // Framework handles DPI scaling automatically - use fonts directly
            Font scaledFont = _textFont;
            Font scaledSubHeaderFont = hasSubHeader ? SubHeaderFont : null;

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                var maxSz = MaxImageSize;
                float scaleFactor = Math.Min(
                    (float)maxSz.Width / Math.Max(1, imageSize.Width),
                    (float)maxSz.Height / Math.Max(1, imageSize.Height));
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Measure header text
            Size headerTextSize;
            if (_multiline)
            {
                headerTextSize = TextRenderer.MeasureText(g, Text, scaledFont, new Size(contentRect.Width, int.MaxValue),
                    GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
            }
            else
            {
                headerTextSize = TextRenderer.MeasureText(g, Text, scaledFont, new Size(int.MaxValue, int.MaxValue),
                    GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
            }

            // Measure subheader text if present
            Size subHeaderTextSize = Size.Empty;
            if (hasSubHeader)
            {
                if (_multiline)
                {
                    subHeaderTextSize = TextRenderer.MeasureText(g, SubHeaderText, scaledSubHeaderFont,
                        new Size(contentRect.Width, int.MaxValue),
                        GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
                }
                else
                {
                    subHeaderTextSize = TextRenderer.MeasureText(g, SubHeaderText, scaledSubHeaderFont,
                        new Size(int.MaxValue, int.MaxValue),
                        GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
                }
            }

            // Calculate combined text height
            int combinedTextHeight = headerTextSize.Height;
            if (hasSubHeader)
            {
                combinedTextHeight += subHeaderTextSize.Height;
            }

            // Define text area rect (total area needed for all text)
            Size combinedTextSize = new Size(
                Math.Max(headerTextSize.Width, hasSubHeader ? subHeaderTextSize.Width : 0),
                combinedTextHeight);

            Rectangle imageRect, textAreaRect;
            CalculateLayout(contentRect, imageSize, combinedTextSize, out imageRect, out textAreaRect);

            if (beepImage != null && beepImage.HasImage)
            {
                if (beepImage.Size.Width > this.Size.Width || beepImage.Size.Height > this.Size.Height)
                {
                    imageSize = this.Size;
                }
                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageRect.Size;
                beepImage.DrawImage(g, imageRect);
            }

            // Calculate individual text rectangles within the text area
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                Rectangle headerTextRect = new Rectangle(
                    textAreaRect.X,
                    textAreaRect.Y,
                    textAreaRect.Width,
                    headerTextSize.Height);

                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                if (_multiline)
                {
                    flags |= TextFormatFlags.WordBreak;
                }
                else
                {
                    flags |= TextFormatFlags.SingleLine;
                }

                // Draw header text
                TextRenderer.DrawText(g, Text, scaledFont, headerTextRect, ForeColor, flags);

                // Draw subheader text if present
                if (hasSubHeader)
                {
                    Rectangle subHeaderTextRect = new Rectangle(
                        textAreaRect.X,
                        headerTextRect.Bottom,
                        textAreaRect.Width,
                        subHeaderTextSize.Height);

                    TextRenderer.DrawText(g, SubHeaderText, scaledSubHeaderFont,
                        subHeaderTextRect, SubHeaderForeColor, flags);
                }
            }

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
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelFont);

                    // Create a smaller font for subheader if not explicitly set
                    if (_subHeaderFont == null)
                    {
                        _subHeaderFont = new Font(_textFont.FontFamily,
                                               _textFont.Size - 2,
                                               FontStyle.Regular);
                    }
                }
                // SafeApplyFont(TextFont ?? _textFont);
                ApplyThemeToSvg();
                //Invalidate();
                //Refresh();
            }
        }


        public void ApplyThemeToSvg()
        {
            if (beepImage != null)
            {
                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.BackColor = BackColor;
                    if (IsChild)
                    {
                        beepImage.ForeColor = _currentTheme.LabelForeColor;
                    }
                    else
                    {
                        beepImage.ForeColor = _currentTheme.LabelForeColor;
                    }
                    beepImage.ApplyThemeToSvg();
                }
            }
        }
        #endregion "Theme"

        #region "Mouse Events"
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Suppress repaint on click to prevent flicker
            SendMessage(Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            try
            {
                base.OnMouseDown(e);
            }
            finally
            {
                SendMessage(Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Suppress repaint on click to prevent flicker
            SendMessage(Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            try
            {
                base.OnMouseUp(e);
            }
            finally
            {
                SendMessage(Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
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
            if (!SetFont() && UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }

            // Use the control's current width as the constraint for multi-line text
            int maxWidth = proposedSize.Width > 0 ? proposedSize.Width : (DrawingRect.Width > 0 ? DrawingRect.Width : 200);

            // Measure header text size
            Size headerTextSize;
            if (_multiline)
            {
                headerTextSize = TextRenderer.MeasureText(Text, _textFont, new Size(maxWidth, int.MaxValue),
                    GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
            }
            else
            {
                headerTextSize = TextRenderer.MeasureText(Text, _textFont, new Size(int.MaxValue, int.MaxValue),
                    GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
            }

            // Measure subheader text size
            Size subHeaderTextSize = Size.Empty;
            bool hasSubHeader = !string.IsNullOrEmpty(SubHeaderText);

            if (hasSubHeader)
            {
                if (_multiline)
                {
                    subHeaderTextSize = TextRenderer.MeasureText(SubHeaderText, SubHeaderFont,
                        new Size(maxWidth, int.MaxValue),
                        GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
                }
                else
                {
                    subHeaderTextSize = TextRenderer.MeasureText(SubHeaderText, SubHeaderFont,
                        new Size(int.MaxValue, int.MaxValue),
                        GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
                }
            }

            // Calculate combined text size
            int textWidth = Math.Max(headerTextSize.Width, hasSubHeader ? subHeaderTextSize.Width : 0);
            int textHeight = headerTextSize.Height;

            if (hasSubHeader)
            {
                textHeight += subHeaderTextSize.Height;
            }

            Size imageSize = beepImage?.HasImage == true ? beepImage.GetImageSize() : Size.Empty;

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
            CalculateLayout(contentRect, imageSize, new Size(textWidth, textHeight), out imageRect, out textRect);

            Rectangle bounds = Rectangle.Union(imageRect, textRect);
            int width = bounds.Width + Padding.Left + Padding.Right;
            int height = bounds.Height + Padding.Top + Padding.Bottom;

            return new Size(width, height);
        }

        // Recompute min on resize
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateMinimumSize();
            if (Height < MinimumSize.Height)
                Height = MinimumSize.Height;
            Invalidate();
        }

        #region"Text and Alignment"
        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            bool hasImage = imageSize != Size.Empty;
            bool hasText = !string.IsNullOrEmpty(Text) && !HideText;

            contentRect.Inflate(-2, -2);

            if (hasImage && !hasText)
            {
                imageRect = AlignRectangle(contentRect, imageSize, ContentAlignment.MiddleCenter);
            }
            else if (hasText && !hasImage)
            {
                if (_multiline)
                {
                    textSize = TextRenderer.MeasureText(Text, _textFont, new Size(contentRect.Width, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
                }
                textRect = AlignRectangle(contentRect, textSize, TextAlign);
            }
            else if (hasImage && hasText)
            {
                if (_multiline)
                {
                    textSize = TextRenderer.MeasureText(Text, _textFont, new Size(contentRect.Width, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
                }
                // Calculate the total width required for text, image, and spacing
                int totalWidth = textSize.Width + imageSize.Width;
                int totalHeight = Math.Max(textSize.Height, imageSize.Height);

                // Adjust contentRect to fit the total content
                contentRect.Width = Math.Min(contentRect.Width, totalWidth);
                contentRect.Height = Math.Min(contentRect.Height, totalHeight);
                switch (this.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        imageRect = AlignRectangle(contentRect, imageSize, ImageAlign);
                        textRect = AlignRectangle(contentRect, textSize, TextAlign);
                        break;

                    case TextImageRelation.ImageBeforeText:
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, imageSize.Width, contentRect.Height), imageSize, ImageAlign);
                        textRect = AlignRectangle(new Rectangle(contentRect.Left + imageSize.Width, contentRect.Top, contentRect.Width - imageSize.Width, contentRect.Height), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextBeforeImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, textSize.Width, contentRect.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left + textSize.Width, contentRect.Top, contentRect.Width - textSize.Width, contentRect.Height), imageSize, ImageAlign);
                        break;

                    case TextImageRelation.ImageAboveText:
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, imageSize.Height), imageSize, ImageAlign);
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + imageSize.Height, contentRect.Width, contentRect.Height - imageSize.Height), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextAboveImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, textSize.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + textSize.Height, contentRect.Width, contentRect.Height - textSize.Height), imageSize, ImageAlign);
                        break;
                }
                // Adjust positions based on TextAlign and ImageAlign within the contentRect
                if (TextImageRelation == TextImageRelation.TextBeforeImage)
                {
                    // Recalculate the total content width and adjust positions
                    int contentWidth = textRect.Width + imageRect.Width;
                    int contentHeight = Math.Max(textRect.Height, imageRect.Height);

                    // Center the entire content (text + image) within the contentRect based on TextAlign
                    int contentX = contentRect.Left;
                    int contentY = contentRect.Top;

                    switch (TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.BottomLeft:
                            contentX = contentRect.Left;
                            break;
                        case ContentAlignment.TopCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.BottomCenter:
                            contentX = contentRect.Left + (contentRect.Width - contentWidth) / 2;
                            break;
                        case ContentAlignment.TopRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.BottomRight:
                            contentX = contentRect.Right - contentWidth;
                            break;
                    }

                    switch (TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.TopCenter:
                        case ContentAlignment.TopRight:
                            contentY = contentRect.Top;
                            break;
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.MiddleRight:
                            contentY = contentRect.Top + (contentRect.Height - contentHeight) / 2;
                            break;
                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.BottomRight:
                            contentY = contentRect.Bottom - contentHeight;
                            break;
                    }

                    // Adjust text and image positions within the content area
                    textRect = new Rectangle(contentX, contentY + (contentHeight - textRect.Height) / 2, textRect.Width, textRect.Height);
                    imageRect = new Rectangle(contentX + textRect.Width, contentY + (contentHeight - imageRect.Height) / 2, imageRect.Width, imageRect.Height);
                }
            }
        }

        private Rectangle AlignRectangle(Rectangle container, Size size, ContentAlignment alignment)
        {
            int x = 0;
            int y = 0;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = container.X;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    x = container.X + (container.Width - size.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    x = container.Right - size.Width;
                    break;
            }

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = container.Y;
                    break;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = container.Y + (container.Height - size.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = container.Bottom - size.Height;
                    break;
            }

            return new Rectangle(new Point(x, y), size);
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags = TextFormatFlags.PreserveGraphicsClipping;

            if (_multiline)
            {
                flags |= TextFormatFlags.WordBreak;
            }
            else
            {
                flags |= TextFormatFlags.SingleLine;
            }

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Top;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Bottom;
                    break;
            }

            return flags;
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

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            contentRect = rectangle;
            DrawImageAndText(graphics);
        }
        #endregion "IBeep UI Component Implementation"
        #region "Badge"
        private BeepControl _lastBeepParent;



        #endregion "Badge"

        #region "Material Design Support"

       


        #endregion "Material Design Support"
    }
}