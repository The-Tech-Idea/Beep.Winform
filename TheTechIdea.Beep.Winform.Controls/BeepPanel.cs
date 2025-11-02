using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
 
 
 
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Containers")]
    [Description("A panel with a title and optional line below the title.")]
    [DisplayName("Beep Panel")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepPanelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public class BeepPanel : BaseControl
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

        // Track disposing to prevent paint/draw during removal
        private bool _isDisposing = false;
        private bool InDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

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
            get => _textFont ?? Font;
            set
            {
                _textFont = value;
                UseThemeFont = false;
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
            set { _titleBottomY = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed as the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config a line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set { _showTitleLine = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TitleLineColor
        {
            get => _titleLineColor;
            set { _titleLineColor = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleLineThickness
        {
            get => _titleLineThickness;
            set { _titleLineThickness = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showTitle;
            set { _showTitle = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set { _titleAlignment = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw the title line with full width or just below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set { _titleLineFullWidth = value; Invalidate(); }
        }

        /// <summary>
        /// Override to prevent BaseControl from clearing the graphics surface over child controls.
        /// As a container control, we let Windows Forms handle child control painting naturally.
        /// </summary>
        protected override bool AllowBaseControlClear => false;

        /// <summary>
        /// Override OnPaintBackground to ensure proper background painting for container controls.
        /// This ensures child controls are painted on top of the background, like a regular Panel.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Paint the background normally for container controls
            // Windows Forms will automatically paint child controls on top
         e.Graphics.Clear(BackColor);
        }

        /// <summary>
        /// Override OnPaint to paint the title. Since UserPaint is false, Windows Forms
        /// will automatically paint child controls, so we don't need to paint them explicitly.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) return;
            
            try
            {
                // Call base Control.OnPaint to ensure standard painting pipeline
                // Since UserPaint is false, Windows Forms will handle child controls automatically
                base.OnPaint(e);
                
                // Now paint our title on top (but only in the title area)
                // Only paint title if it's shown
                if (_showTitle && !string.IsNullOrEmpty(_titleText))
                {
                    var g = e.Graphics;
                    
                    // Calculate title area
                    Rectangle titleArea = GetTitleArea();
                    
                    if (!titleArea.IsEmpty)
                    {
                        // Save current clip
                        Region oldClip = g.Clip;
                        
                        // Clip to title area only - don't paint over child control area
                        using (var region = new Region(titleArea))
                        {
                            g.Clip = region;
                            
                            // Paint background for title area only (erase any child control content in title area)
                            using (var brush = new SolidBrush(BackColor))
                            {
                                g.FillRectangle(brush, titleArea);
                            }
                            
                            // Draw title and title line
                            _titleBottomY = startyoffset;
                            UpdateDrawingRect();
                            DrawTitle(g, DrawingRect);
                        }
                        
                        // Restore original clip
                        g.Clip = oldClip;
                    }
                }
                
                // Note: With UserPaint = false, Windows Forms automatically paints child controls
                // No need to manually paint them here!
            }
            catch { }
        }

        #endregion

        #region "Constructor"
        public BeepPanel() : base()
        {
            // Prevent designer issues by not subscribing to events or theme managers in design mode
            if (InDesignMode)
            {
                this.Size = new Size(400, 300);
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
                return;
            }

            // Runtime initialization
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;

            // For container controls, we need to disable UserPaint to allow Windows Forms
            // to automatically paint child controls. We can still paint the title in OnPaint.
            
            // Disable UserPaint (BaseControl sets it to true, we override it here)
            SetStyle(ControlStyles.UserPaint, false);
            
            // Keep other useful styles
            SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.ContainerControl, true);
            
            // Disable external buffered graphics for container controls - it can interfere with child control painting
            UseExternalBufferedGraphics = false;
            
            DoubleBuffered = true;
            UpdateStyles();
            this.Size = new Size(400, 300);

            try { ApplyTheme(); }
            catch
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
            }
        }
        #endregion

        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //    _isDisposing = true;
        //    base.OnHandleDestroyed(e);
        //}

        //// IMPORTANT: Avoid BaseControl's parent-change badge registration for this container
        //protected override void OnParentChanged(EventArgs e)
        //{
        //    // Intentionally do NOT call base.OnParentChanged to skip RegisterBadgeDrawer logic for BeepPanel
        //    try
        //    {
        //        if (IsChild && Parent != null)
        //        {
        //            BackColor = Parent.BackColor;
        //        }
                
        //        // Force refresh when parent changes (e.g., added to BeepiFormPro)
        //        if (Visible && !IsDisposed)
        //        {
        //            BeginInvoke(new Action(() =>
        //            {
        //                if (!IsDisposed && Visible)
        //                {
        //                    Invalidate(true);
        //                    Refresh();
        //                    // Ensure all child controls are properly refreshed
        //                    foreach (Control control in Controls)
        //                    {
        //                        if (!control.IsDisposed && control.Visible)
        //                        {
        //                            control.Invalidate(true);
        //                            control.Refresh();
        //                        }
        //                    }
        //                }
        //            }));
        //        }
        //    }
        //    catch { /* design-time safe */ }
        //}

        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    base.OnVisibleChanged(e);
        //    if (Visible && !IsDisposed)
        //    {
        //        // Force invalidation of all child controls when panel becomes visible
        //        Invalidate(true);
        //        Refresh();
        //        foreach (Control control in Controls)
        //        {
        //            if (!control.IsDisposed && control.Visible)
        //            {
        //                control.Invalidate(true);
        //                control.Refresh();
        //            }
        //        }
        //    }
        //}

        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    base.OnHandleCreated(e);
        //    // Ensure proper rendering when handle is created
        //    if (!IsDisposed && Visible)
        //    {
        //        BeginInvoke(new Action(() =>
        //        {
        //            if (!IsDisposed && Visible)
        //            {
        //                Invalidate(true);
        //                Refresh();
        //            }
        //        }));
        //    }
        //}

        // Override Dispose to properly clean up
        protected override void Dispose(bool disposing)
        {
            _isDisposing = true;
            if (disposing)
            {
                try
                {
                    if (_scrollTimer != null)
                    {
                        try { _scrollTimer.Stop(); } catch { }
                        _scrollTimer.Dispose();
                        _scrollTimer = null;
                    }

                    if (_verticalScrollBar != null) { try { _verticalScrollBar.Dispose(); } catch { } _verticalScrollBar = null; }
                    if (_horizontalScrollBar != null) { try { _horizontalScrollBar.Dispose(); } catch { } _horizontalScrollBar = null; }

                    // Do not dispose font explicitly at design-time; just release reference
                    _textFont = null;
                }
                catch { }
            }

            base.Dispose(disposing);
        }


     

        public override void ApplyTheme()
        {
            if (InDesignMode) return; // keep designer stable

            this.SuspendLayout();
            try
            {
                base.ApplyTheme();
                if (_currentTheme == null) return;

                BackColor = _currentTheme.PanelBackColor;
                ForeColor = _currentTheme.PrimaryTextColor;
                BorderColor = _currentTheme.BorderColor;

                if (UseGradientBackground)
                {
                    GradientStartColor = _currentTheme.PanelGradiantStartColor != Color.Empty ? _currentTheme.PanelGradiantStartColor : _currentTheme.GradientStartColor;
                    GradientEndColor = _currentTheme.PanelGradiantEndColor != Color.Empty ? _currentTheme.PanelGradiantEndColor : _currentTheme.GradientEndColor;
                    GradientDirection = _currentTheme.GradientDirection;
                }

                if (!string.IsNullOrEmpty(_titleText) && _showTitle)
                {
                    _titleLineColor = _currentTheme.CardTitleForeColor != Color.Empty ? _currentTheme.CardTitleForeColor : _currentTheme.PrimaryTextColor;
                }

                if (UseThemeFont)
                {
                    try
                    {
                        if (_currentTheme.TitleSmall != null)
                            _textFont = BeepThemesManager.ToFont(_currentTheme.TitleSmall);
                        else if (_currentTheme.CardHeaderStyle != null)
                            _textFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
                        else
                            _textFont = new Font(_currentTheme.FontFamily, _currentTheme.FontSizeBlockHeader, FontStyle.Regular);
                    }
                    catch { _textFont = new Font("Arial", 10); }
                }

                if (_currentTheme.BorderRadius > 0) { IsRounded = true; BorderRadius = _currentTheme.BorderRadius; }
                if (_currentTheme.BorderSize > 0) { BorderThickness = _currentTheme.BorderSize; }

                ShadowColor = _currentTheme.ShadowColor;
                ShadowOpacity = _currentTheme.ShadowOpacity;
            }
            catch
            {
                BackColor = SystemColors.Control; ForeColor = SystemColors.ControlText; _titleLineColor = SystemColors.ControlDark;
            }
            finally { this.ResumeLayout(false); }

            Invalidate();
        }


        /// <summary>
        /// Calculates the title area rectangle that needs to be painted.
        /// This excludes the area where child controls will be rendered.
        /// </summary>
        private Rectangle GetTitleArea()
        {
            if (!_showTitle || string.IsNullOrEmpty(_titleText) || _textFont == null)
                return Rectangle.Empty;

            UpdateDrawingRect();
            var titleSize = TextRenderer.MeasureText(_titleText, _textFont);
            int titleHeight = titleSize.Height + padding;
            
            if (_showTitleLine)
            {
                titleHeight += _titleLineThickness + padding;
            }
            
            // Return the title area at the top of the control
            return new Rectangle(
                DrawingRect.Left,
                DrawingRect.Top,
                DrawingRect.Width,
                titleHeight + padding * 2
            );
        }

        protected override void DrawContent(Graphics g)
        {
       
        }

        private void DrawTitle(Graphics g, Rectangle rectangle)
        {
            if (_isDisposing || IsDisposed ) return;
            if (string.IsNullOrEmpty(_titleText) || _textFont == null) return;

            try
            {
                var titleSize = System.Windows.Forms.TextRenderer.MeasureText(_titleText, _textFont);
                float textTop = DrawingRect.Top + padding;
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
                }

                Color textColor = _currentTheme?.TextBoxForeColor ?? ForeColor;
                using (Brush brush = new SolidBrush(textColor))
                {
                    //g.DrawString(_titleText, _textFont, brush, textLeft, textTop);
                    TextRenderer.DrawText(g, _titleText, _textFont, new Point((int)textLeft, (int)textTop), textColor);
                }

                float textBottomY = textTop + titleSize.Height;

                if (_showTitleLine)
                {
                    int lineY = (int)(textBottomY + 2);
                    int lineStartX = ShowTitleLineinFullWidth ? (DrawingRect.Left + BorderThickness) : (int)textLeft;
                    int lineEndX = ShowTitleLineinFullWidth ? (DrawingRect.Right - BorderThickness) : (int)(textLeft + titleSize.Width);

                    using (var pen = new Pen(_titleLineColor, _titleLineThickness))
                    {
                        g.DrawLine(pen, lineStartX, lineY, lineEndX, lineY);
                    }

                    _titleBottomY = lineY + _titleLineThickness + padding;
                }
                else
                {
                    _titleBottomY = (int)(textBottomY + padding);
                }
            }
            catch { }
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_isDisposing || IsDisposed ) return;
            try
            {
                DrawTitle(graphics, rectangle);
                // var children = Controls.Cast<Control>().ToArray();
                // foreach (Control ctrl in children)
                // {
                //     if (ctrl is IBeepUIComponent comp && !ctrl.IsDisposed)
                //     {
                //         try { comp.Draw(graphics, rectangle); } catch { }
                //     }
                // }
            }
            catch { }
        }
    }
}
