using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using static TheTechIdea.Beep.Winform.Controls.Base.BaseControl;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Classic mode painter: computes inner/border/content rects, draws background/shadow/borders/labels, and icons.
    /// </summary>
    internal sealed class ClassicBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _drawingRect;
        private Rectangle _borderRect;
        private Rectangle _contentRect;
        private int _reserveTop;
        private int _reserveBottom;
        
        // Cached label properties for shared use between drawing and border functions
        private int _labelLeft;
        private int _labelWidth;
        private int _labelHeight;
        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _borderRect;
        public Rectangle ContentRect => _contentRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null)
            {
                _drawingRect = _borderRect = _contentRect = Rectangle.Empty;
                return;
            }

            // Compute border thickness and padding
            // When UseFormStylePaint is true, use BeepStyling values, otherwise use owner's properties
            int border = 0;
            int padding = 0;
            int shadow = 0;
            
            if (owner.UseFormStylePaint && owner.ControlStyle != BeepControlStyle.None)
            {
                // Use BeepStyling values for BeepStyling-based painting
                float borderWidth = BeepStyling.GetBorderThickness(owner.ControlStyle);
                border = (int)Math.Ceiling(borderWidth);
                padding = BeepStyling.GetPadding(owner.ControlStyle);
                
                // Check if this style has shadows
                if (StyleShadows.HasShadow(owner.ControlStyle))
                {
                    // Use a reasonable shadow offset based on style
                    //shadow = Math.Max(2, StyleShadows.GetShadowBlur(owner.ControlStyle) / 2);
                    shadow = 0;
                }
            }
            else
            {
                // Use owner's properties for classic painting
                if (owner.ShowAllBorders || owner.BorderThickness > 0 && (owner.ShowTopBorder || owner.ShowBottomBorder || owner.ShowLeftBorder || owner.ShowRightBorder))
                {
                    border = owner.BorderThickness;
                }
                
                // For classic mode, use the average of padding
                var pad = owner.Padding;
                padding = (pad.Left + pad.Top + pad.Right + pad.Bottom) / 4;
                
                // Shadow offset
                shadow = owner.ShowShadow ? owner.ShadowOffset : 0;
            }

            // Base paddings + optional offsets (always use owner's specific offsets)
            var ownerPadding = owner.Padding;
            int leftPad = ownerPadding.Left + owner.LeftoffsetForDrawingRect;
            int topPad = ownerPadding.Top + owner.TopoffsetForDrawingRect;
            int rightPad = ownerPadding.Right + owner.RightoffsetForDrawingRect;
            int bottomPad = ownerPadding.Bottom + owner.BottomoffsetForDrawingRect;
            
            // When using BeepStyling, USE style padding instead of adding to owner's padding
            // This prevents double-padding which causes layout issues
            if (owner.UseFormStylePaint && owner.ControlStyle != BeepControlStyle.None)
            {
                // Use the MAXIMUM of owner padding and style padding, not both
                leftPad = Math.Max(leftPad, padding + owner.LeftoffsetForDrawingRect);
                topPad = Math.Max(topPad, padding + owner.TopoffsetForDrawingRect);
                rightPad = Math.Max(rightPad, padding + owner.RightoffsetForDrawingRect);
                bottomPad = Math.Max(bottomPad, padding + owner.BottomoffsetForDrawingRect);
            }

            // Account for rounded corners - reduce drawable area to prevent corners from being cut off
            int cornerAdjustment = 0;
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                // When corners are rounded, reduce the drawing rect to account for the corner curve
                // This prevents content from extending into rounded corner areas
                cornerAdjustment = Math.Max(2, owner.BorderRadius / 2);
            }

            // Calculate inner drawing rect
            int calculatedWidth = owner.Width - (shadow * 2 + border * 2 + leftPad + rightPad + (cornerAdjustment * 2));
            int calculatedHeight = owner.Height - (shadow * 2 + border * 2 + topPad + bottomPad + (cornerAdjustment * 2));

            var inner = new Rectangle(
                shadow + border + leftPad + cornerAdjustment,
                shadow + border + topPad + cornerAdjustment,
                Math.Max(0, calculatedWidth),
                Math.Max(0, calculatedHeight)
            );

            // Reserve space for label and helper when not material
            // IMPORTANT: Also ensure content height can accommodate the control's text font
            int reserveTop = 0;
            int reserveBottom = 0;
            try
            {
                using var g = owner.CreateGraphics();

                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = 8f;
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveTop = h + 2;
                }

                string support = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(support))
                {
                    float supSize = 8f;
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveBottom = h + 4;
                }

                // Calculate minimum height needed for main control text
                int mainTextHeight = 0;
              
                    var textSize = TextRenderer.MeasureText(g, "Ag", owner.TextFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    mainTextHeight = textSize.Height;
                

                if (reserveTop > 0 || reserveBottom > 0)
                {
                    int availableHeight = inner.Height - reserveTop - reserveBottom;
                    
                    // Ensure available height is at least enough for the main text
                    if (availableHeight < mainTextHeight)
                    {
                        // Not enough space - reduce reserved space proportionally or don't reserve
                        // Priority: main text > label/helper
                        if (inner.Height < mainTextHeight + reserveTop + reserveBottom)
                        {
                            // Not enough space for everything - prioritize main text
                            reserveTop = 0;
                            reserveBottom = 0;
                        }
                        else
                        {
                            // Scale down reserves to fit
                            int totalReserves = reserveTop + reserveBottom;
                            int neededSpace = mainTextHeight;
                            int availableForReserves = inner.Height - neededSpace;
                            
                            if (totalReserves > 0 && availableForReserves > 0)
                            {
                                // Proportionally reduce reserves
                                reserveTop = (int)(reserveTop * (availableForReserves / (float)totalReserves));
                                reserveBottom = (int)(reserveBottom * (availableForReserves / (float)totalReserves));
                            }
                        }
                    }
                    
                    inner = new Rectangle(
                        inner.X,
                        inner.Y + reserveTop,
                        inner.Width,
                        Math.Max(0, inner.Height - reserveTop - reserveBottom)
                    );
                }
            }
            catch { /* best-effort */ }

            // Store reserves for use in Paint method
            _reserveTop = reserveTop;
            _reserveBottom = reserveBottom;

            // Border rectangle with rounded corner consideration
            // Border Y should be positioned to cut through the middle of the label text
            // So the label appears to sit ON the border
            int borderY = shadow + (reserveTop > 0 ? reserveTop / 2 : 0);
            int borderHeight = Math.Max(0, owner.Height - (shadow) * 2 - (borderY - shadow) - reserveBottom);
            
            var borderRect = new Rectangle(
                shadow,
                borderY,
                Math.Max(0, owner.Width - (shadow) * 2),
                borderHeight
            );
            
            // Adjust border rect for rounded corners
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                int cornerOffset = Math.Max(1, owner.BorderRadius / 3);
                borderRect = new Rectangle(
                    borderRect.X + cornerOffset,
                    borderRect.Y + cornerOffset,
                    Math.Max(0, borderRect.Width - (cornerOffset * 2)),
                    Math.Max(0, borderRect.Height - (cornerOffset * 2))
                );
            }

            // Border rectangle like BeepControl
            if (border > 0)
            {
                int halfPen = (int)Math.Ceiling(owner.BorderThickness / 2f);
                borderRect = new Rectangle(
                    borderRect.X + halfPen,
                    borderRect.Y + halfPen,
                    Math.Max(0, borderRect.Width - (halfPen * 2)),
                    Math.Max(0, borderRect.Height - (halfPen * 2))
                );
            }

            // Compute icon-adjusted content
            Rectangle contentRect = inner;
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(inner);
                contentRect = icons.AdjustedContentRect;
            }

            // The drawing rect should be aligned with the border rect for consistent background fill
            // It should NOT include the label/helper reserved space
            Rectangle drawingRect = new Rectangle(
                borderRect.X,
                borderRect.Y,
                borderRect.Width,
                borderRect.Height
            );

            _drawingRect = drawingRect;   // Use borderRect-aligned rect for background
            _borderRect = borderRect;
            _contentRect = contentRect;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            UpdateLayout(owner);
            
            // Calculate label properties once for shared use across drawing functions
            CalculateLabelProperties(g, owner);
            
            // Background (classic)
            if (owner.UseFormStylePaint && owner.ControlStyle != BeepControlStyle.None)
            {
                // Use the already-calculated border rect which accounts for label/helper space
                GraphicsPath path = BeepStyling.CreateStylePath(_borderRect);
                
                // Paint the styled control only in the border area
                BeepStyling.PaintControl(g, path, owner.ControlStyle, owner._currentTheme, false, GetEffectiveState(owner), owner.IsTransparentBackground);
                
                // Clean up path
                path?.Dispose();
            }
            else
            {
                var backColor = GetEffectiveBackColor(owner);
                using (var brush = new SolidBrush(backColor))
                {
                    if (owner.IsRounded && owner.BorderRadius > 0)
                    {
                        using var path = GraphicsExtensions.GetRoundedRectPath(_drawingRect, owner.BorderRadius);
                        g.FillPath(brush, path);
                    }
                    else
                    {
                        g.FillRectangle(brush, _drawingRect);
                    }
                }
                // Borders (classic)
                // Draw borders only when control is not frameless AND some border is requested
                bool shouldDrawBorders = !owner.IsFrameless && (owner.ShowAllBorders || (owner.BorderThickness > 0 && (owner.ShowTopBorder || owner.ShowBottomBorder || owner.ShowLeftBorder || owner.ShowRightBorder)));
                if (shouldDrawBorders)
                {
                    DrawBorders(g, owner);
                    // Shadow
                    if (owner.ShowShadow && owner.ShadowOpacity > 0)
                    {
                        DrawShadow(g, owner);
                    }
                }
            }

            // Draw label and helper text (always, regardless of styling mode)
            DrawLabelAndHelperNonMaterial(g, owner);

            // Draw icons if any
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(_drawingRect);
                icons.Draw(g);
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (!(hasLeading || hasTrailing)) return;

            var icons = new BaseControlIconsHelper(owner);
            icons.UpdateLayout(_drawingRect);
            var lead = icons.LeadingRect;
            var trail = icons.TrailingRect;
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("ClassicLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("ClassicTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            int shadow = owner.ShowShadow ? owner.ShadowOffset * 2 : 0;
            int border = 0;
            if (owner.ShowAllBorders && !owner.IsFrameless)
            {
                border = owner.BorderThickness * 2;

            }
             if(!owner.ShowAllBorders && !owner.IsFrameless)
            {
                if (owner.BorderThickness > 0 && (owner.ShowTopBorder || owner.ShowBottomBorder || owner.ShowLeftBorder || owner.ShowRightBorder))
                    border = owner.BorderThickness * 2;
            }

            var pad = owner.Padding;
            int padW = pad.Horizontal;
            int padH = pad.Vertical;

            // label/supporting reserves
            int extraTop = 0;
            int extraBottom = 0;
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraTop = h + 2;
                }
                string support = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(support))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraBottom = h + 4;
                }
            }
            catch { }

            // measure text (fallback content sizing)
            int textW = 0;
            int textH = owner.Font.Height;
            try
            {
                using var g = owner.CreateGraphics();
                var sz = TextRenderer.MeasureText(g, string.IsNullOrEmpty(owner.Text) ? "" : owner.Text, owner.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                textW = sz.Width;
                textH = sz.Height;
            }
            catch { }

            // icon width contribution
            int iconW = 0;
            if (!string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath))
                iconW += owner.IconSize + (owner.IconPadding * 2) + 8;
            if (!string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton)
                iconW += owner.IconSize + (owner.IconPadding * 2) + 8;

            // If using styled painting, treat developer-set size as CONTENT size and add chrome (border+shadow) around it
            if (owner.UseFormStylePaint)
            {
                int contentW = Math.Max(owner.Width, textW + iconW + padW);
                int contentH = Math.Max(owner.Height, textH + padH);
                int totalW = contentW + border + shadow;
                int totalH = contentH + border + shadow + extraTop + extraBottom;
                // Ensure minimums when extremely small
                totalW = Math.Max(30, totalW);
                totalH = Math.Max(18 + extraTop + extraBottom, totalH);
                return new Size(totalW, totalH);
            }

            int minWidth = 60;
            int width = Math.Max(minWidth, textW + iconW + padW + border + shadow);
            int height = Math.Max(textH + padH + border + shadow + extraTop + extraBottom, owner.Height);

            return new Size(width, height);
        }

        private static Color GetEffectiveBackColor(Base.BaseControl owner)
        {
            if (!owner.Enabled) return owner.DisabledBackColor;
            if (owner.IsPressed && owner.CanBePressed) return owner.PressedBackColor;
            if (owner.IsHovered && owner.CanBeHovered) return owner.HoverBackColor;
            if (owner.Focused && owner.CanBeFocused) return owner.FocusBackColor;
            if (owner.IsSelected && owner.CanBeSelected) return owner.SelectedBackColor;
            return owner.BackColor;
        }
        private static ControlState GetEffectiveState(Base.BaseControl owner)
        {
            if (!owner.Enabled) return ControlState.Disabled;
            if (owner.IsPressed && owner.CanBePressed) return ControlState.Pressed;
            if (owner.IsHovered && owner.CanBeHovered) return ControlState.Hover;
            if (owner.Focused && owner.CanBeFocused) return ControlState.Focused;
            if (owner.IsSelected && owner.CanBeSelected) return ControlState.Selected;
            return ControlState.Normal;
        }

        /// <summary>
        /// Calculates and caches label properties (position, width, height) for shared use.
        /// Call this before DrawBorders and DrawLabelAndHelperNonMaterial to ensure consistent calculations.
        /// </summary>
        private void CalculateLabelProperties(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null || string.IsNullOrEmpty(owner.LabelText))
            {
                _labelWidth = 0;
                _labelHeight = 0;
                _labelLeft = 0;
                return;
            }
            int startoffset = 8; // 
            // Measure label text
            float labelSize = Math.Max(8f, owner.Font.Size - 1f);
            using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
            var actualTextSize = TextRenderer.MeasureText(g, owner.LabelText, lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            
            int textWidth = actualTextSize.Width;
            int padding = 4; // padding on both sides
            _labelHeight = actualTextSize.Height;
            _labelWidth = textWidth + (padding * 2);

            // Calculate label position based on alignment
            switch (owner.LabelPosition)
            {
                case LabelPosition.Center:
                    _labelLeft = _borderRect.Left + (_borderRect.Width - _labelWidth) / 2;
                    break;
                case LabelPosition.Right:
                    _labelLeft = _borderRect.Right - _labelWidth- startoffset;
                    break;
                case LabelPosition.Left:
                default:
                    _labelLeft = _borderRect.Left+ startoffset;
                    break;
            }
        }

        private void DrawBorders(Graphics g, Base.BaseControl owner)
        {
            Color borderColor = owner.BorderColor;
            if (!owner.Enabled) borderColor = owner.DisabledBorderColor;
            else if (owner.Focused) borderColor = owner.FocusBorderColor;
            else if (owner.IsHovered) borderColor = owner.HoverBorderColor;
            else if (owner.IsPressed) borderColor = owner.PressedBorderColor;
            else if (owner.IsSelected) borderColor = owner.SelectedBorderColor;

            // Determine if we should skip the top border line for the label area
            bool skipTopBorderForLabel = owner.ShowLabelAboveBorder && !string.IsNullOrEmpty(owner.LabelText) && _labelWidth > 0;
            
            if (owner.ShowAllBorders && owner.BorderThickness > 0)
            {
                using var pen = new Pen(borderColor, owner.BorderThickness) { DashStyle = owner.BorderDashStyle, Alignment = PenAlignment.Inset };
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    using var path = GraphicsExtensions.GetRoundedRectPath(_borderRect, owner.BorderRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    if (skipTopBorderForLabel)
                    {
                        // Draw borders (skip top part where label is)
                        // Left border
                        g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _borderRect.Left, _borderRect.Bottom);
                        // Right border
                        g.DrawLine(pen, _borderRect.Right, _borderRect.Top, _borderRect.Right, _borderRect.Bottom);
                        // Bottom border
                        g.DrawLine(pen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                        // Top border - left part (before label)
                        if (_labelLeft > _borderRect.Left)
                            g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _labelLeft, _borderRect.Top);
                        // Top border - right part (after label)
                        if (_labelLeft + _labelWidth < _borderRect.Right)
                            g.DrawLine(pen, _labelLeft + _labelWidth, _borderRect.Top, _borderRect.Right, _borderRect.Top);
                    }
                    else
                    {
                        g.DrawRectangle(pen, _borderRect);
                    }
                }
            }
            else if (owner.BorderThickness > 0)
            {
                using var pen = new Pen(borderColor, owner.BorderThickness) { DashStyle = owner.BorderDashStyle };
                
                if (owner.ShowBottomBorder)
                    g.DrawLine(pen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                if (owner.ShowLeftBorder)
                    g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _borderRect.Left, _borderRect.Bottom);
                if (owner.ShowRightBorder)
                    g.DrawLine(pen, _borderRect.Right, _borderRect.Top, _borderRect.Right, _borderRect.Bottom);
                
                if (owner.ShowTopBorder)
                {
                    if (skipTopBorderForLabel)
                    {
                        // Draw top border segments (before and after label)
                        if (_labelLeft > _borderRect.Left)
                            g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _labelLeft, _borderRect.Top);
                        if (_labelLeft + _labelWidth < _borderRect.Right)
                            g.DrawLine(pen, _labelLeft + _labelWidth, _borderRect.Top, _borderRect.Right, _borderRect.Top);
                    }
                    else
                    {
                        g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _borderRect.Right, _borderRect.Top);
                    }
                }
            }
        }

        private void DrawShadow(Graphics g, Base.BaseControl owner)
        {
            int shadowDepth = Math.Max(1, owner.ShadowOffset / 2);
            int maxLayers = Math.Min(shadowDepth, 6);

            Rectangle shadowRect = new Rectangle(
                _drawingRect.X + owner.ShadowOffset,
                _drawingRect.Y + owner.ShadowOffset,
                _drawingRect.Width,
                _drawingRect.Height);

            for (int i = 1; i <= maxLayers; i++)
            {
                float layerOpacityFactor = (float)(maxLayers - i + 1) / maxLayers;
                float finalOpacity = owner.ShadowOpacity * layerOpacityFactor * 0.6f;
                int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                Color layerShadowColor = Color.FromArgb(layerAlpha, owner.ShadowColor);
                int spread = i - 1;
                Rectangle layerRect = new Rectangle(
                    shadowRect.X - spread,
                    shadowRect.Y - spread,
                    shadowRect.Width + (spread * 2),
                    shadowRect.Height + (spread * 2));

                using var shadowBrush = new SolidBrush(layerShadowColor);
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    int shadowRadius = Math.Max(0, owner.BorderRadius + spread);
                    using var shadowPath = ControlPaintHelper.GetRoundedRectPath(layerRect, shadowRadius);
                    g.FillPath(shadowBrush, shadowPath);
                }
                else
                {
                    g.FillRectangle(shadowBrush, layerRect);
                }
            }
        }

        private void DrawLabelAndHelperNonMaterial(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;

            // Draw label text in the reserved space above the border
            if (!string.IsNullOrEmpty(owner.LabelText) && _labelWidth > 0)
            {
                float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                
                int labelTop;
                TextFormatFlags textFormat;
                
                if (owner.ShowLabelAboveBorder)
                {
                    // Label floats on the border line
                    labelTop = 2; // shadowOffset
                    
                    // Determine text format based on position
                    switch (owner.LabelPosition)
                    {
                        case LabelPosition.Center:
                            textFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;
                            break;
                        case LabelPosition.Right:
                            textFormat = TextFormatFlags.Right | TextFormatFlags.EndEllipsis;
                            break;
                        case LabelPosition.Left:
                        default:
                            textFormat = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                            break;
                    }
                    
                    // Draw line under the text with background color
                    int lineY = _borderRect.Top;
                    using (var linePen = new Pen(owner.BackColor, 2))
                    {
                        g.DrawLine(linePen, _labelLeft, lineY, _labelLeft + _labelWidth, lineY);
                    }
                }
                else
                {
                    // Label above border (traditional position)
                    labelTop = Math.Max(0, _borderRect.Top - _reserveTop);
                    
                    // Determine text format based on position
                    switch (owner.LabelPosition)
                    {
                        case LabelPosition.Center:
                            textFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;
                            break;
                        case LabelPosition.Right:
                            textFormat = TextFormatFlags.Right | TextFormatFlags.EndEllipsis;
                            break;
                        case LabelPosition.Left:
                        default:
                            textFormat = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                            break;
                    }
                }
                
                var labelRect = new Rectangle(
                    _labelLeft, 
                    labelTop, 
                    _labelWidth, 
                    _reserveTop > 0 ? _reserveTop : _labelHeight
                );

                Color labelColor = owner._currentTheme.AppBarTitleForeColor;
                
                TextRenderer.DrawText(g, owner.LabelText, lf, labelRect, labelColor, textFormat);
            }

            // Draw helper or error text just below the bottom border
            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, owner.Font.Size - 1f);
                using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                var supportHeight = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                
                int supportTop = _borderRect.Bottom + 2;
                
                var supportRect = new Rectangle(
                    _borderRect.Left + 6, 
                    supportTop, 
                    Math.Max(10, _borderRect.Width - 12), 
                    supportHeight
                );
                
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : owner.ForeColor;
                
                TextRenderer.DrawText(g, supporting, sf, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        // Private fields to store calculated rectangles and reserves
       
    }
}
