using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Containers")]
    [Description("A panel with a title and optional line below the title.")]
    [DisplayName("Beep Panel")]
    public class BeepPanel : BeepControl
    {
        // We'll keep everything the same, only adjusting the logic in DrawTitle and DrawTitleLine
        const int startyoffset = 0;
        private string _titleText = "Panel Title";
        private bool _showTitle = true;
        private bool _showTitleLine = true;
        private bool _titleLineFullWidth = true;
        private Color _titleLineColor = Color.Gray;
        private int _titleLineThickness = 2;

        private int _titleBottomY = startyoffset;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;

        int padding = 2; // Adjusted padding for top, left, etc.

        #region "Scrolling"
        private bool _enableScrolling = false;
        private int _scrollOffset = 0;
        private int _scrollSpeed = 1;
        private Timer _scrollTimer;
        private int _scrollInterval = 10;
        private int _scrollDirection = 1;
        private VScrollBar _verticalScrollBar;
        private HScrollBar _horizontalScrollBar;
        #endregion

        #region "Public Properties"
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
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
               // SafeApplyFont( _textFont);
                Invalidate();
            }
        }
      
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Title Bottom Location Y")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleBottomY
        {
            get => _titleBottomY;
            set
            {
                _titleBottomY = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed as the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config a line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set
            {
                _showTitleLine = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TitleLineColor
        {
            get => _titleLineColor;
            set
            {
                _titleLineColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleLineThickness
        {
            get => _titleLineThickness;
            set
            {
                _titleLineThickness = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                _titleAlignment = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw the title line with full width or just below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set
            {
                _titleLineFullWidth = value;
                Invalidate();
            }
        }

        #endregion

        #region "Constructor"
        public BeepPanel()
        {
            ApplyTheme();
            this.Size = new Size(400, 300);
        }
        #endregion
        
      
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            //   // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
            //    this.Value = new Value(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        
        public override void ApplyTheme()
        {
            // CRITICAL FIX: Suspend/Resume layout to prevent control movement during theme changes
            this.SuspendLayout();
            
            try
            {
                base.ApplyTheme();

                if (_currentTheme == null)
                    return;

                // Apply panel-specific theme properties
                BackColor = _currentTheme.PanelBackColor;
                ForeColor = _currentTheme.PrimaryTextColor;
                BorderColor = _currentTheme.BorderColor;

                // Apply gradient properties if enabled
                if (_useGradientBackground)
                {
                    _gradientStartColor = _currentTheme.PanelGradiantStartColor != Color.Empty ?
                        _currentTheme.PanelGradiantStartColor : _currentTheme.GradientStartColor;

                    _gradientEndColor = _currentTheme.PanelGradiantEndColor != Color.Empty ?
                        _currentTheme.PanelGradiantEndColor : _currentTheme.GradientEndColor;

                    _gradientDirection = _currentTheme.GradientDirection;
                }

                // Apply title-specific properties
                if (!string.IsNullOrEmpty(_titleText) && _showTitle)
                {
                    // Title line color
                    _titleLineColor = _currentTheme.CardTitleForeColor != Color.Empty ?
                        _currentTheme.CardTitleForeColor : _currentTheme.PrimaryTextColor;
                }

                // Apply font settings based on theme - SIMPLIFIED!
                if (UseThemeFont)
                {
                    try
                    {
                        if (_currentTheme.TitleSmall != null)
                        {
                            _textFont = BeepThemesManager.ToFont(_currentTheme.TitleSmall);
                        }
                        else if (_currentTheme.CardHeaderStyle != null)
                        {
                            _textFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
                        }
                        else
                        {
                            _textFont = new Font(_currentTheme.FontFamily, _currentTheme.FontSizeBlockHeader, FontStyle.Regular);
                        }
                  //  SafeApplyFont( _textFont);
                    }
                    catch (Exception)
                    {
                        // Fallback to default font
                        _textFont = new Font("Arial", 10);
                    //SafeApplyFont (_textFont);
                    }
                }

                // Child controls will manage their own themes - don't interfere!
                // ApplyThemeToChilds is set to false to prevent automatic theme application

                // Apply rounded corners from theme if available
                if (_currentTheme.BorderRadius > 0)
                {
                    IsRounded = true;
                    BorderRadius = _currentTheme.BorderRadius;
                }

                // Set border thickness from theme if available
                if (_currentTheme.BorderSize > 0)
                {
                    BorderThickness = _currentTheme.BorderSize;
                }

                // Apply shadow properties if enabled
                if (_showShadow)
                {
                    _shadowColor = _currentTheme.ShadowColor;
                    _shadowOpacity = _currentTheme.ShadowOpacity;
                }
            }
            finally
            {
                // CRITICAL: Always resume layout in finally block
                this.ResumeLayout(false);
            }

            // Invalidate to ensure the panel is redrawn with the new theme
            Invalidate();
        }
        
      
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            // Reset TitleBottomY each time we paint. We'll recalc it if we draw a title.
            _titleBottomY = startyoffset;
            UpdateDrawingRect();
            // If ShowTitle is true and TitleText is not empty, draw the title
            if (_showTitle && !string.IsNullOrEmpty(_titleText))
            {
                DrawTitle(g, DrawingRect);
            }
        }
       
        private void DrawTitle(Graphics g,Rectangle rectangle)
        {
            // Use a title font from your theme, or fallback
         
            // Update DrawingRect before measuring or positioning
            

            // measure how big the text is
            SizeF titleSize = g.MeasureString(_titleText, _textFont);

            // We'll define a "textTop" for vertical. It's typically DrawingRect.Top + some padding
            float textTop = DrawingRect.Top + padding;
            // We'll define textLeft based on TitleAlignment
            float textLeft = 0;

            switch (_titleAlignment)
            {
                case ContentAlignment.TopLeft:
                    textLeft = DrawingRect.Left + padding;
                    break;
                case ContentAlignment.TopCenter:
                    textLeft = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2f;
                    break;
                case ContentAlignment.TopRight:
                    textLeft = DrawingRect.Right - titleSize.Width - padding;
                    break;
                    // If you plan to support MiddleLeft, MiddleCenter, etc. you'd handle them similarly,
                    // but your code suggests you only do top alignments
            }

            // Draw the title text
            using (Brush brush = new SolidBrush(_currentTheme.TextBoxForeColor))
            {
                g.DrawString(_titleText, _textFont, brush, textLeft, textTop);
            }

            // The bottom of the drawn text
            float textBottomY = textTop + titleSize.Height;

            if (_showTitleLine)
            {
                // We'll draw a line below the text
                // line Y is a bit below the text
                // e.g. lineY = (int)(textBottomY + 2) if you want a small gap
                int lineY = (int)(textBottomY + 2);

                // If ShowTitleLineinFullWidth => from DrawingRect.Left + _borderThickness
                // else from textLeft => textLeft + titleSize.Width
                int lineStartX = ShowTitleLineinFullWidth
                    ? (DrawingRect.Left + BorderThickness)
                    : (int)textLeft;
                int lineEndX = ShowTitleLineinFullWidth
                    ? (DrawingRect.Right - BorderThickness)
                    : (int)(textLeft + titleSize.Width);

                using (var pen = new Pen(_titleLineColor, _titleLineThickness))
                {
                    g.DrawLine(pen, lineStartX, lineY, lineEndX, lineY);
                }
                
                // TitleBottomY is the line's bottom => lineY + lineThickness + some extra
                _titleBottomY = lineY + _titleLineThickness + padding;
            }
            else
            {
                // no line => just set TitleBottomY to textBottom plus some padding
                _titleBottomY = (int)(textBottomY + padding);
            }
        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            DrawTitle(graphics, rectangle);
            foreach (Control ctrl in Controls)
            {
                if (ctrl is IBeepUIComponent)
                    ((IBeepUIComponent)ctrl).Draw(graphics, rectangle);
               
            }
        }
    }
}
