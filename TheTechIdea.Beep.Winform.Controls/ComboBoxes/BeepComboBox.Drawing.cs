using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

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
