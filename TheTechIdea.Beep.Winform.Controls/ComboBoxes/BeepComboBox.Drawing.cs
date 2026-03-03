using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Drawing

      
        private static void DrawChevronDesignTime(Graphics g, Rectangle btnR, Color fg)
        {
            int cx = btnR.Left + btnR.Width / 2;
            int cy = btnR.Top  + btnR.Height / 2;
            int hw = Math.Max(3, btnR.Width  / 4);
            int hh = Math.Max(2, btnR.Height / 8);
            var pts = new[]
            {
                new PointF(cx - hw, cy - hh),
                new PointF(cx,      cy + hh),
                new PointF(cx + hw, cy - hh)
            };
            using var pen = new Pen(fg, 1.5f) { LineJoin = LineJoin.Round };
            g.DrawLines(pen, pts);
        }

        /// <summary>
        /// DrawContent override — follows BeepButton's paint-only pattern:
        ///   base.DrawContent(g)  →  UpdateDrawingRect()  →  compute fresh layout  →  paint.
        /// All layout rects are recalculated every frame from the current DrawingRect,
        /// exactly like BeepButton does with contentRect = DrawingRect → CalculateLayout.
        /// This eliminates stale cached rects that caused arrow-resizing, editor-drift,
        /// and layout shifts on style/focus/theme changes.
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
         
            // 1 — BaseControl: border, background, DrawingRect
            base.DrawContent(g);
            UpdateDrawingRect();
            if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0) return;

            // 2 — Fresh layout from current DrawingRect (no caching, no stored padding)
            _helper.CalculateLayout(DrawingRect, out _textAreaRect, out _dropdownButtonRect, out _imageRect);

            // 2b — Clear-button carve-out
            if (ShowClearButton && (_selectedItem != null || !string.IsNullOrEmpty(_inputText)))
            {
                int cbw = Math.Max(16, Math.Min(ScaleLogicalX(ClearButtonWidthLogical), _textAreaRect.Width / 4));
                _clearButtonRect = new Rectangle(
                    _dropdownButtonRect.Left - cbw,
                    DrawingRect.Y, cbw, DrawingRect.Height);
                _textAreaRect = new Rectangle(
                    _textAreaRect.X, _textAreaRect.Y,
                    Math.Max(1, _textAreaRect.Width - cbw), _textAreaRect.Height);
            }
            else
            {
                _clearButtonRect = Rectangle.Empty;
            }

            // 2c — RTL mirror
            if (IsRtl && !DrawingRect.IsEmpty)
            {
                _dropdownButtonRect = MirrorRect(_dropdownButtonRect, DrawingRect);
                _textAreaRect       = MirrorRect(_textAreaRect,       DrawingRect);
                _clearButtonRect    = _clearButtonRect.IsEmpty ? Rectangle.Empty
                                     : MirrorRect(_clearButtonRect, DrawingRect);
                if (!_imageRect.IsEmpty)
                    _imageRect = MirrorRect(_imageRect, DrawingRect);
            }

            // 3 — Sync inline editor
            if (_inlineEditor != null && _inlineEditor.Visible)
            {
                if (_inlineEditor.Bounds != _textAreaRect)
                    _inlineEditor.Bounds = _textAreaRect;
            }

            // 4 — Painter
            if (_comboBoxPainter == null)
            {
                _comboBoxPainter = CreatePainter(_comboBoxType);
                _comboBoxPainter.Initialize(this, _currentTheme);
            }
            _comboBoxPainter.Paint(g, this, DrawingRect);

            // 5 — Loading overlay
            if (_isLoading) DrawLoadingIndicator(g, DrawingRect);

            // 6 — Hit areas
            if (!_isLoading) RegisterHitAreas();

           
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers.
        /// Paints a static image of the combo box into the given rectangle.
        /// Calculates layout relative to the passed rect (not the control's own bounds).
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (graphics == null || rectangle.Width <= 0 || rectangle.Height <= 0) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Calculate sub-rects from the passed rectangle
            _helper.CalculateLayout(rectangle, out var textAreaRect, out var buttonRect, out var imageRect);

            // --- Draw text ---
            string displayText = _helper.GetDisplayText();
            if (!string.IsNullOrEmpty(displayText))
            {
                Color textColor;
                if (_helper.IsShowingPlaceholder())
                {
                    Color placeholderColor = _currentTheme?.TextBoxPlaceholderColor ?? Color.Empty;
                    textColor = placeholderColor != Color.Empty
                        ? placeholderColor
                        : Color.FromArgb(128, (_currentTheme?.SecondaryColor ?? _currentTheme?.ForeColor ?? Color.Gray));
                }
                else
                {
                    textColor = _helper.GetTextColor();
                }

                Font textFont = TextFont
                    ?? BeepThemesManager.ToFont(_currentTheme?.LabelFont)
                    ?? Font;

                int hInset = ScaleLogicalX(6);
                var textBounds = new Rectangle(
                    textAreaRect.X + hInset,
                    textAreaRect.Y,
                    Math.Max(1, textAreaRect.Width - hInset * 2),
                    textAreaRect.Height);

                TextFormatFlags flags = TextFormatFlags.Left
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis
                    | TextFormatFlags.NoPrefix;

                TextRenderer.DrawText(graphics, displayText, textFont, textBounds, textColor, flags);
            }

            // --- Draw dropdown arrow ---
            if (!buttonRect.IsEmpty)
            {
                Color arrowColor = Color.FromArgb(180, (_currentTheme?.SecondaryColor ?? Color.Gray));
                int arrowSize = Math.Max(ScaleLogicalX(3), Math.Min(buttonRect.Width, buttonRect.Height) / 5);
                int arrowHalf = Math.Max(ScaleLogicalY(2), arrowSize / 2);
                int cx = buttonRect.Left + buttonRect.Width / 2;
                int cy = buttonRect.Top + buttonRect.Height / 2;

                float stroke = Math.Max(1f, ScaleLogicalX(2));
                using (var pen = new Pen(arrowColor, stroke))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;
                    graphics.DrawLines(pen, new[]
                    {
                        new Point(cx - arrowSize, cy - arrowHalf),
                        new Point(cx, cy + arrowHalf),
                        new Point(cx + arrowSize, cy - arrowHalf)
                    });
                }
            }
        }

        /// <summary>
        /// Draws the loading spinner indicator
        /// </summary>
        private void DrawLoadingIndicator(Graphics g, Rectangle bounds)
        {
            if (!_isLoading) return;

            // Calculate spinner size and position (in dropdown button area)
            var buttonRect = GetDropdownButtonRect();
            if (buttonRect.IsEmpty) return;
            
            int spinnerSize = Math.Min(16, Math.Min(buttonRect.Width, buttonRect.Height) / 2);
            if (spinnerSize < 6) return; // Too small to draw

            Point center = new Point(
                buttonRect.Left + buttonRect.Width / 2,
                buttonRect.Top + buttonRect.Height / 2);

            // Get spinner color from theme or use default
            Color spinnerColor = _currentTheme?.PrimaryColor ?? Color.Gray;
            if (spinnerColor == Color.Empty)
            {
                spinnerColor = Color.Gray;
            }

            // Draw rotating spinner using arcs
            using (Pen spinnerPen = new Pen(spinnerColor, 2f))
            {
                spinnerPen.StartCap = LineCap.Round;
                spinnerPen.EndCap = LineCap.Round;

                // Save graphics state
                var state = g.Save();
                try
                {
                    g.TranslateTransform(center.X, center.Y);
                    g.RotateTransform(_loadingRotationAngle);
                    g.TranslateTransform(-center.X, -center.Y);

                    // Draw 4 arcs to create spinner effect
                    Rectangle spinnerRect = new Rectangle(
                        center.X - spinnerSize / 2,
                        center.Y - spinnerSize / 2,
                        spinnerSize,
                        spinnerSize);

                    for (int i = 0; i < 4; i++)
                    {
                        float alpha = 255f * (1f - i * 0.25f);
                        spinnerPen.Color = Color.FromArgb((int)alpha, spinnerColor);
                        g.DrawArc(spinnerPen, spinnerRect, i * 90f, 60f);
                    }
                }
                finally
                {
                    g.Restore(state);
                }
            }
        }
        
        #endregion
        
        #region Painter Factory
        
        /// <summary>
        /// Creates the appropriate painter for the specified ComboBoxType
        /// </summary>
        private IComboBoxPainter CreatePainter(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.Standard => new StandardComboBoxPainter(),
                ComboBoxType.Minimal => new MinimalComboBoxPainter(),
                ComboBoxType.Outlined => new OutlinedComboBoxPainter(),
                ComboBoxType.Rounded => new RoundedComboBoxPainter(),
                ComboBoxType.MaterialOutlined => new MaterialOutlinedComboBoxPainter(),
                ComboBoxType.Filled => new FilledComboBoxPainter(),
                ComboBoxType.Borderless => new BorderlessComboBoxPainter(),
                ComboBoxType.BlueDropdown => new BlueDropdownPainter(),
                ComboBoxType.GreenDropdown => new GreenDropdownPainter(),
                ComboBoxType.Inverted => new InvertedComboBoxPainter(),
                ComboBoxType.Error => new ErrorComboBoxPainter(),
                ComboBoxType.MultiSelectChips => new MultiSelectChipsPainter(),
                ComboBoxType.SearchableDropdown => new SearchableDropdownPainter(),
                ComboBoxType.WithIcons => new WithIconsComboBoxPainter(),
                ComboBoxType.Menu => new MenuComboBoxPainter(),
                ComboBoxType.CountrySelector => new CountrySelectorPainter(),
                ComboBoxType.SmoothBorder => new SmoothBorderPainter(),
                ComboBoxType.DarkBorder => new DarkBorderPainter(),
                ComboBoxType.PillCorners => new PillCornersComboBoxPainter(),
                _ => new StandardComboBoxPainter()
            };
        }
        
        #endregion
        
        #region Hit Area Registration
        
        /// <summary>
        /// Registers interactive hit areas using BaseControl's hit test system.
        /// These are used for hover-detection and cursor changes.
        /// All actual click actions are handled in OnMouseDown for reliability (exact press location,
        /// fires on press not release). Hit actions here are intentionally null to prevent
        /// double-invocation when the Click event also fires after MouseDown.
        /// </summary>
        private void RegisterHitAreas()
        {
            // Clear previous hit areas
            ClearHitList();

            // Register dropdown button - action is null; click is handled in OnMouseDown
            if (!_dropdownButtonRect.IsEmpty)
            {
                AddHitArea("DropdownButton", _dropdownButtonRect, null, null);
            }

            // Register text area - editable-mode start-editing is also handled in OnMouseDown
            if (!_textAreaRect.IsEmpty)
            {
                AddHitArea("TextArea", _textAreaRect, null, null);
            }
        }
        
        #endregion
    }
}
