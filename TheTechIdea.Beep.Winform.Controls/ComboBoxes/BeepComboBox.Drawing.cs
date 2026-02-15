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
        
        /// <summary>
        /// DrawContent override - called by BaseControl.
        /// First calls base to let ClassicBaseControlPainter draw border/background/shadow,
        /// then paints the ComboBox-specific content inside DrawingRect.
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            // Let BaseControl's ClassicBaseControlPainter draw border, background, shadow
            base.DrawContent(g);
            
            // Now paint ComboBox content (text, dropdown button, etc.) inside the DrawingRect
            Paint(g, DrawingRect);
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
                    ?? new Font("Segoe UI", 9f, FontStyle.Regular);

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
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Ensure painter exists for current type
            if (_comboBoxPainter == null)
            {
                _comboBoxPainter = CreatePainter(_comboBoxType);
                _comboBoxPainter.Initialize(this, _currentTheme);
            }
            
            // Update layout ONLY if needed (not on every paint)
            if (_needsLayoutUpdate || !_layoutCacheValid)
            {
                UpdateLayout();
                _needsLayoutUpdate = false;
            }
            
            // Let the combo box painter draw everything
            _comboBoxPainter.Paint(g, this, bounds);
            
            // Draw loading indicator if loading
            if (_isLoading)
            {
                DrawLoadingIndicator(g, bounds);
            }
            
            // Register hit areas for interaction (disabled when loading)
            if (!_isLoading)
            {
                RegisterHitAreas();
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
        /// Registers interactive hit areas using BaseControl's hit test system
        /// </summary>
        private void RegisterHitAreas()
        {
            // Clear previous hit areas
            ClearHitList();
            
            // Register dropdown button hit area
            if (!_dropdownButtonRect.IsEmpty)
            {
                AddHitArea("DropdownButton", _dropdownButtonRect, null, TogglePopup);
            }
            
            // Register text area hit area (for editable mode or focus)
            if (!_textAreaRect.IsEmpty)
            {
                AddHitArea("TextArea", _textAreaRect, null, () =>
                {
                    if (IsEditable)
                    {
                        StartEditing();
                    }
                    else
                    {
                        Focus();
                    }
                });
            }
        }
        
        #endregion
    }
}
