using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Partial class for BeepNumericUpDown drawing/painting logic.
    /// Uses BeepStyling for background, border, and shadow rendering.
    /// Painters only handle layout and button icons.
    /// </summary>
    public partial class BeepNumericUpDown
    {
        #region Drawing
        
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Create the control path for BeepStyling
            using (var controlPath = GetControlPath())
            {
                // Paint background (includes shadow) using BeepStyling
                BeepStyling.PaintStyleBackground(g, controlPath, Style);

                // Paint border using BeepStyling
                BeepStyling.PaintStyleBorder(g, controlPath, IsFocused, Style);

                // Now let the painter handle layout and content
                if (_currentPainter != null)
                {
                    var context = new NumericUpDownPainterContext(this);
                    var layout = _currentPainter.CalculateLayout(context, bounds);

                    // Store button rectangles for hit testing
                    _upButtonRect = layout.UpButtonRect;
                    _downButtonRect = layout.DownButtonRect;

                    // Paint value text if not editing
                    if (!_isEditing && !string.IsNullOrEmpty(layout.TextRect.ToString()))
                    {
                        string formattedText = _currentPainter.FormatValue(context);
                        _currentPainter.PaintValueText(g, context, layout.TextRect, formattedText);
                    }

                    // Paint button icons/symbols
                    if (layout.ShowButtons && _showSpinButtons)
                    {
                        // Paint button backgrounds using BeepStyling
                        PaintButtonBackgrounds(g, layout.UpButtonRect, layout.DownButtonRect);

                        // Paint button icons
                        _currentPainter.PaintButtonIcons(g, context, layout.UpButtonRect, layout.DownButtonRect);
                    }
                }
            }
        }

        /// <summary>
        /// Paint button backgrounds using BeepStyling
        /// </summary>
        private void PaintButtonBackgrounds(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            // Paint down button background
            if (downButtonRect != Rectangle.Empty)
            {
                using (var downPath = GraphicsExtensions.GetRoundedRectPath(downButtonRect, BorderRadius / 2))
                {
                    if (_downButtonPressed)
                    {
                        using (var pressBrush = new SolidBrush(PressedBackColor))
                        {
                            g.FillPath(pressBrush, downPath);
                        }
                    }
                    else if (_downButtonHovered)
                    {
                        using (var hoverBrush = new SolidBrush(HoverBackColor))
                        {
                            g.FillPath(hoverBrush, downPath);
                        }
                    }
                }
            }

            // Paint up button background
            if (upButtonRect != Rectangle.Empty)
            {
                using (var upPath = GraphicsExtensions.GetRoundedRectPath(upButtonRect, BorderRadius / 2))
                {
                    if (_upButtonPressed)
                    {
                        using (var pressBrush = new SolidBrush(PressedBackColor))
                        {
                            g.FillPath(pressBrush, upPath);
                        }
                    }
                    else if (_upButtonHovered)
                    {
                        using (var hoverBrush = new SolidBrush(HoverBackColor))
                        {
                            g.FillPath(hoverBrush, upPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the control's graphics path for rendering
        /// </summary>
        private GraphicsPath GetControlPath()
        {
            if (IsRounded && BorderRadius > 0)
            {
                return GraphicsExtensions.GetRoundedRectPath(ClientRectangle, BorderRadius);
            }
            else
            {
                var path = new GraphicsPath();
                path.AddRectangle(ClientRectangle);
                return path;
            }
        }

        /// <summary>
        /// Handles control resize events
        /// </summary>
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            CalculateButtonAreas();
            RefreshHitAreas();
        }

        private void CalculateButtonAreas()
        {
            if (!_showSpinButtons) return;

            int buttonWidth = GetButtonWidthForSize(_buttonSize);
            int buttonHeight = Height;

            // Down button (left side)
            _downButtonRect = new Rectangle(0, 0, buttonWidth, buttonHeight);

            // Up button (right side)
            _upButtonRect = new Rectangle(Width - buttonWidth, 0, buttonWidth, buttonHeight);
        }

        private int GetButtonWidthForSize(NumericSpinButtonSize size)
        {
            return size switch
            {
                NumericSpinButtonSize.Small => System.Math.Min(20, Width / 6),
                NumericSpinButtonSize.Standard => System.Math.Min(24, Width / 5),
                NumericSpinButtonSize.Large => System.Math.Min(28, Width / 4),
                NumericSpinButtonSize.ExtraLarge => System.Math.Min(32, Width / 3),
                _ => System.Math.Min(24, Width / 5)
            };
        }

        // NOTE: Draw method now implemented above (calls Paint function)
        #endregion
    }
}
