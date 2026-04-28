using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
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
            // 2 — Fresh layout from current DrawingRect via the new Layout Engine
            var renderState = ComboBoxStateFactory.Build(this);
            var layout = ComboBoxLayoutEngine.Compute(DrawingRect, renderState, this);

            _textAreaRect = layout.TextAreaRect;
            _dropdownButtonRect = layout.DropdownButtonRect;
            _clearButtonRect = layout.ClearButtonRect;
            _imageRect = layout.ImageRect;

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

            // 3 — Sync inline editor (or kill it if the type no longer supports it)
            if (_inlineEditor != null && _inlineEditor.Visible)
            {
                if (!IsInlineEditorAllowed())
                {
                    HideInlineEditor(false);
                }
                else if (_inlineEditor.Bounds != _textAreaRect)
                {
                    _inlineEditor.Bounds = _textAreaRect;
                }
            }

            // 4 — Painter
            if (_comboBoxPainter == null)
            {
                _comboBoxPainter = CreatePainter(_comboBoxType);
                _comboBoxPainter.Initialize(this, _currentTheme);
            }
            _comboBoxPainter.Paint(g, this, renderState, layout);

            // 5 — Hit areas
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

            var renderState = ComboBoxStateFactory.Build(this);
            var layout = ComboBoxLayoutEngine.Compute(rectangle, renderState, this);
            if (_comboBoxPainter == null)
            {
                _comboBoxPainter = CreatePainter(_comboBoxType);
                _comboBoxPainter.Initialize(this, _currentTheme);
            }

            // Keep Draw(g, rect) visually consistent with DrawContent(g).
            _comboBoxPainter.Paint(graphics, this, renderState, layout);
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
            return ComboBoxTypeRegistry.CreatePainter(type);
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

            // Register clear button for hover cursor
            if (!_clearButtonRect.IsEmpty && ShowClearButton)
            {
                AddHitArea("ClearButton", _clearButtonRect, null, null);
            }

            // Register chip close buttons for hover cursor
            if (ChipCloseRects.Count > 0)
            {
                foreach (var kvp in ChipCloseRects)
                {
                    AddHitArea($"ChipClose_{kvp.Key}", kvp.Value, null, null);
                }
            }
        }
        
        #endregion
    }
}
