using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Label")]
    [Description("A label control with support for images and multi-line text.")]
    public class BeepLabel : BeepControl
    {
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
        #endregion "Fields"

        #region "Properties"
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
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
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
                Font = value;
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
        #endregion "Properties"

        #region "Constructors"
        public BeepLabel() : base()
        {
            DoubleBuffered = true;
            InitializeComponents();
            beepImage.ImageEmbededin = ImageEmbededin.Label;
            BoundProperty = "Text";
            BorderRadius = 3;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            UpdateDrawingRect();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            if (AutoSize)
            {
                this.Size = GetPreferredSize(new Size(this.Width, 0));
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
                    return new Size(200, measured.Height + 6);
                }
            }
        }
        #endregion "Constructors"

        #region "Painting"
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (AutoSize)
            {
                Size newPreferred = GetPreferredSize(new Size(this.Width, 0));
                this.Size = newPreferred;
            }
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateDrawingRect();
            contentRect = DrawingRect;
            DrawImageAndText(e.Graphics);
        }

        private void DrawImageAndText(Graphics g)
        {
            if (!SetFont() && UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }

            Font scaledFont = _useScaledfont ? GetScaledFont(g, Text, contentRect.Size, _textFont) : _textFont;
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize;
            if (_multiline)
            {
                textSize = TextRenderer.MeasureText(g, Text, scaledFont, new Size(contentRect.Width, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
            }
            else
            {
                textSize = TextRenderer.MeasureText(g, Text, scaledFont, new Size(int.MaxValue, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
            }

            Rectangle imageRect, textRect;
            CalculateLayout(contentRect, imageSize, textSize, out imageRect, out textRect);

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

            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                if (_multiline)
                {
                    flags |= TextFormatFlags.WordBreak;
                }
                else
                {
                    flags |= TextFormatFlags.SingleLine;
                }
                TextRenderer.DrawText(g, Text, scaledFont, textRect, ForeColor, flags);
            }

            if (BadgeText != null)
            {
                DrawBadge(g);
            }
        }
        #endregion "Painting"

        #region "Theme"
        public override void ApplyTheme()
        {
            if (_currentTheme != null)
            {
                BackColor = _currentTheme.LabelBackColor;
                ForeColor = _currentTheme.LabelForeColor;
                HoverBackColor = _currentTheme.ButtonHoverBackColor;
                HoverForeColor = _currentTheme.ButtonHoverForeColor;

                if (UseThemeFont)
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }
                Font = _textFont;

                if (IsChild && Parent != null)
                {
                    parentbackcolor = Parent.BackColor;
                }

                ApplyThemeToSvg();
                Invalidate();
                Refresh();
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

        #region "Text and Alignment"
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
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (!SetFont() && UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }

            // Use the control's current width as the constraint for multi-line text
            int maxWidth = proposedSize.Width > 0 ? proposedSize.Width : (DrawingRect.Width > 0 ? DrawingRect.Width : 200);
            Size textSize;
            if (_multiline)
            {
                textSize = TextRenderer.MeasureText(Text, _textFont, new Size(maxWidth, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.WordBreak);
            }
            else
            {
                textSize = TextRenderer.MeasureText(Text, _textFont, new Size(int.MaxValue, int.MaxValue), GetTextFormatFlags(TextAlign) | TextFormatFlags.SingleLine);
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
            Rectangle contentRect = new Rectangle(0, 0, maxWidth, textSize.Height);
            Rectangle textRect, imageRect;
            CalculateLayout(contentRect, imageSize, textSize, out imageRect, out textRect);

            Rectangle bounds = Rectangle.Union(imageRect, textRect);
            int width = bounds.Width + Padding.Left + Padding.Right;
            int height = bounds.Height + Padding.Top + Padding.Bottom;

            return new Size(width, height);
        }

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
                int totalWidth = textSize.Width + offset + imageSize.Width;
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
                        textRect = AlignRectangle(new Rectangle(contentRect.Left + imageSize.Width + offset, contentRect.Top, contentRect.Width - imageSize.Width - offset, contentRect.Height), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextBeforeImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, textSize.Width, contentRect.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left + textSize.Width + offset, contentRect.Top, contentRect.Width - textSize.Width - offset, contentRect.Height), imageSize, ImageAlign);
                        break;

                    case TextImageRelation.ImageAboveText:
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, imageSize.Height), imageSize, ImageAlign);
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + imageSize.Height + offset, contentRect.Width, contentRect.Height - imageSize.Height - offset), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextAboveImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, textSize.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + textSize.Height + offset, contentRect.Width, contentRect.Height - textSize.Height - offset), imageSize, ImageAlign);
                        break;
                }
                // Adjust positions based on TextAlign and ImageAlign within the contentRect
                if (TextImageRelation == TextImageRelation.TextBeforeImage)
                {
                    // Recalculate the total content width and adjust positions
                    int contentWidth = textRect.Width + offset + imageRect.Width;
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
                    imageRect = new Rectangle(contentX + textRect.Width + offset, contentY + (contentHeight - imageRect.Height) / 2, imageRect.Width, imageRect.Height);
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

        #region "Mouse Events"
        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = true;
        }
        #endregion "Mouse Events"

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
    }
}