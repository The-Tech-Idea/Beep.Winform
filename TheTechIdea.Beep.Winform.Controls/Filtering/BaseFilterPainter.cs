using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Base implementation of IFilterPainter with common functionality
    /// Provides helper methods for painting, hit testing, and layout
    /// Derived painters implement style-specific rendering
    /// </summary>
    public abstract class BaseFilterPainter : IFilterPainter, IFilterPainterMetricsProvider
    {
        #region Cached GDI resources (Phase 9 compliance)
        // Shared, process-lifetime caches so per-paint draw paths don't allocate.
        // Fonts come from BeepThemesManager (shared cache) and MUST NOT be disposed here.
        // Brushes/pens are single-color reusable instances (UI-thread only). Callers that
        // mutate a pen (caps/dash) must allocate their own; use GetPen only for simple strokes.
        private static readonly Dictionary<(float size, FontStyle style), Font> _fontCache = new();
        private static readonly Dictionary<int, SolidBrush> _brushCache = new();
        private static readonly Dictionary<(int argb, float width), Pen> _penCache = new();

        /// <summary>Returns a cached theme-family font (shared — never dispose). Use instead of allocating fonts per paint.</summary>
        protected static Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            size = Math.Max(6f, size);
            var key = (size, style);
            if (_fontCache.TryGetValue(key, out var f) && f != null) return f;
            var weight = style.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
            f = BeepThemesManager.ToFont("Segoe UI", size, weight, style) ?? SystemFonts.DefaultFont;
            _fontCache[key] = f;
            return f;
        }

        /// <summary>Returns a cached solid brush (shared — never dispose).</summary>
        protected static SolidBrush GetBrush(Color color)
        {
            int key = color.ToArgb();
            if (_brushCache.TryGetValue(key, out var b) && b != null) return b;
            b = new SolidBrush(color);
            _brushCache[key] = b;
            return b;
        }

        /// <summary>Returns a cached simple pen (shared — never dispose or mutate).</summary>
        protected static Pen GetPen(Color color, float width = 1f)
        {
            width = Math.Max(0.5f, width);
            var key = (color.ToArgb(), width);
            if (_penCache.TryGetValue(key, out var p) && p != null) return p;
            p = new Pen(color, width);
            _penCache[key] = p;
            return p;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filter style supported by this painter implementation.
        /// </summary>
        public abstract FilterStyle FilterStyle { get; }

        /// <summary>
        /// Gets whether this painter supports animated transitions.
        /// </summary>
        public virtual bool SupportsAnimations => true;

        /// <summary>
        /// Gets whether this painter supports drag and drop interactions.
        /// </summary>
        public virtual bool SupportsDragDrop => false;

        #endregion

        #region Abstract Methods - Must be implemented by derived classes

        /// <summary>
        /// Calculate style-specific layout
        /// </summary>
        public abstract FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect);

        /// <summary>
        /// Calculate style-specific layout (generic overload for non-BeepFilter controls)
        /// </summary>
        public virtual FilterLayoutInfo CalculateLayout(Rectangle availableRect, FilterConfiguration config)
        {
            // Default implementation - derived classes should override if they need FilterConfiguration
            return new FilterLayoutInfo { ContainerRect = availableRect, ContentRect = availableRect };
        }

        /// <summary>
        /// Paint style-specific UI
        /// </summary>
        public abstract void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout);

        /// <summary>
        /// Paint style-specific UI (generic overload for non-BeepFilter controls)
        /// </summary>
        public virtual void Paint(Graphics g, Rectangle bounds, FilterConfiguration config, FilterLayoutInfo layout, IBeepTheme theme, FilterHitArea? hoveredArea, FilterHitArea? pressedArea)
        {
            // Default implementation - derived painters should override for full functionality
            if (layout == null || config == null || theme == null) return;
            
            // Basic fallback rendering
            g.FillRectangle(GetBrush(theme.BackColor), bounds);
            g.DrawRectangle(GetPen(theme.BorderColor), bounds);

            var text = $"{config.Criteria.Count} filter(s) - Override Paint method";
            TextRenderer.DrawText(g, text, GetFont(9f), new Point(bounds.X + 10, bounds.Y + 10), theme.ForeColor,
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        }

        #endregion

        #region Virtual Methods - Can be overridden

        /// <summary>
        /// Gets metrics for this filter style
        /// </summary>
        public virtual FilterPainterMetrics GetMetrics(BeepFilter owner)
        {
            return FilterPainterMetrics.DefaultFor(FilterStyle, owner._currentTheme);
        }

        /// <summary>
        /// Default hit testing implementation
        /// Checks common hit areas (tags, buttons, etc.)
        /// </summary>
        public virtual FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check action buttons
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddFilter", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            if (layout.AddGroupButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddGroup", Bounds = layout.AddGroupButtonRect, Type = FilterHitAreaType.AddGroupButton };

            if (layout.ClearAllButtonRect.Contains(point))
                return new FilterHitArea { Name = "ClearAll", Bounds = layout.ClearAllButtonRect, Type = FilterHitAreaType.ClearAllButton };

            if (layout.ApplyButtonRect.Contains(point))
                return new FilterHitArea { Name = "Apply", Bounds = layout.ApplyButtonRect, Type = FilterHitAreaType.ApplyButton };

            if (layout.SaveButtonRect.Contains(point))
                return new FilterHitArea { Name = "Save", Bounds = layout.SaveButtonRect, Type = FilterHitAreaType.SaveButton };

            if (layout.LoadButtonRect.Contains(point))
                return new FilterHitArea { Name = "Load", Bounds = layout.LoadButtonRect, Type = FilterHitAreaType.LoadButton };

            // Check search input
            if (layout.SearchInputRect.Contains(point))
                return new FilterHitArea { Name = "Search", Bounds = layout.SearchInputRect, Type = FilterHitAreaType.SearchInput };

            // Check tags and their remove buttons
            for (int i = 0; i < layout.TagRects.Length; i++)
            {
                if (i < layout.RemoveButtonRects.Length && layout.RemoveButtonRects[i].Contains(point))
                    return new FilterHitArea { Name = $"RemoveTag_{i}", Bounds = layout.RemoveButtonRects[i], Type = FilterHitAreaType.RemoveButton, Tag = i };

                if (layout.TagRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Tag_{i}", Bounds = layout.TagRects[i], Type = FilterHitAreaType.FilterTag, Tag = i };
            }

            // Check rows
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                if (layout.RowRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Row_{i}", Bounds = layout.RowRects[i], Type = FilterHitAreaType.FieldDropdown, Tag = i };
            }

            return null;
        }

        #endregion

        #region Helper Methods - Available to derived classes

        /// <summary>
        /// Paint a rounded rectangle using BeepStyling
        /// </summary>
        protected void PaintRoundedRectangle(Graphics g, Rectangle rect, int cornerRadius, Color backgroundColor, Color borderColor, int borderWidth, BeepControlStyle style)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                // Fill background
                g.FillPath(GetBrush(backgroundColor), path);

                // Draw border
                if (borderWidth > 0)
                {
                    g.DrawPath(GetPen(borderColor, borderWidth), path);
                }
            }
        }

        /// <summary>
        /// Paint a tag/chip pill. Pass <paramref name="owner"/> for DPI scaling of internal details.
        /// </summary>
        protected void PaintTagPill(Graphics g, Rectangle rect, string text, Font font, Color backgroundColor, Color textColor, Color borderColor, int cornerRadius, bool showRemove = true, Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                // Fill
                g.FillPath(GetBrush(backgroundColor), path);

                // Border
                g.DrawPath(GetPen(borderColor, 1), path);
            }

            int padH = owner != null ? DpiScalingHelper.ScaleValue(8,  owner) : 8;
            int gapW = owner != null ? DpiScalingHelper.ScaleValue(28, owner) : 28; // space for X button + right pad
            int padW = owner != null ? DpiScalingHelper.ScaleValue(16, owner) : 16; // no-X right pad

            // Text
            var textRect = new Rectangle(rect.X + padH, rect.Y, rect.Width - (showRemove ? gapW : padW), rect.Height);
            TextRenderer.DrawText(g, text, font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Remove button (X)
            if (showRemove)
            {
                int xSize = owner != null ? DpiScalingHelper.ScaleValue(12, owner) : 12;
                int xOff  = owner != null ? DpiScalingHelper.ScaleValue(18, owner) : 18;
                int xX = rect.Right - xOff;
                int xY = rect.Y + rect.Height / 2;

                var xPen = GetPen(textColor, 1.5f);
                g.DrawLine(xPen, xX - xSize / 2, xY - xSize / 2, xX + xSize / 2, xY + xSize / 2);
                g.DrawLine(xPen, xX + xSize / 2, xY - xSize / 2, xX - xSize / 2, xY + xSize / 2);
            }
        }

        /// <summary>
        /// Paint a logic connector (AND/OR)
        /// </summary>
        protected void PaintLogicConnector(Graphics g, Rectangle rect, string text, Font font, Color backgroundColor, Color textColor, Color borderColor, bool vertical = false)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background circle/pill
            var bgBrush = GetBrush(backgroundColor);
            if (vertical)
                g.FillEllipse(bgBrush, rect);
            else
                g.FillRectangle(bgBrush, rect);

            // Border
            var borderPen = GetPen(borderColor, 1);
            if (vertical)
                g.DrawEllipse(borderPen, rect);
            else
                g.DrawRectangle(borderPen, rect);

            // Text
            TextRenderer.DrawText(g, text, font, rect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// Paint a badge/counter
        /// </summary>
        protected void PaintCountBadge(Graphics g, Rectangle rect, int count, Font font, Color backgroundColor, Color textColor)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || count <= 0) return;

            // Circle background
            g.FillEllipse(GetBrush(backgroundColor), rect);

            // Text
            string countText = count > 99 ? "99+" : count.ToString();
            TextRenderer.DrawText(g, countText, font, rect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// Paint a drag handle (6 dots)
        /// </summary>
        protected void PaintDragHandle(Graphics g, Rectangle rect, Color dotColor, Control? owner = null)
        {
            int dotSize = owner != null ? DpiScalingHelper.ScaleValue(3, owner) : 3;
            int spacing = owner != null ? DpiScalingHelper.ScaleValue(4, owner) : 4;
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            var dotBrush = GetBrush(dotColor);
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    int x = centerX - spacing / 2 + col * spacing - dotSize / 2;
                    int y = centerY - spacing + row * spacing - dotSize / 2;
                    g.FillEllipse(dotBrush, x, y, dotSize, dotSize);
                }
            }
        }

        /// <summary>
        /// Create a rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            
            if (cornerRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = cornerRadius * 2;
            
            // Ensure corner radius doesn't exceed rectangle dimensions
            diameter = Math.Min(diameter, Math.Min(rect.Width, rect.Height));
            cornerRadius = diameter / 2;

            // Top left
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Top right
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Bottom right
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Bottom left
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Get colors from BeepStyling based on control style
        /// </summary>
        protected (Color background, Color border, Color text, Color accent) GetStyleColors(BeepControlStyle style)
        {
            var theme = BeepThemesManager.CurrentTheme;
            if (theme != null)
            {
                return (
                    background: theme.BackColor,
                    border: theme.BorderColor,
                    text: theme.ForeColor,
                    accent: theme.AccentColor
                );
            }

            return (
                background: Color.White,
                border: Color.FromArgb(224, 224, 224),
                text: Color.FromArgb(33, 33, 33),
                accent: Color.FromArgb(33, 150, 243)
            );
        }

        /// <summary>
        /// Gets colors preferring the owner control theme, then global theme, then defaults.
        /// </summary>
        protected (Color background, Color border, Color text, Color accent) GetStyleColors(BeepFilter owner, BeepControlStyle style)
        {
            var theme = owner?._currentTheme ?? BeepThemesManager.CurrentTheme;
            if (theme != null)
            {
                return (
                    background: theme.BackColor,
                    border: theme.BorderColor,
                    text: theme.ForeColor,
                    accent: theme.AccentColor
                );
            }

            return GetStyleColors(style);
        }

        #endregion

        #region Modern UX Helpers (Phase 1 Enhancements)

        /// <summary>
        /// Paints a modern filter count badge with DPI-aware sizing.
        /// </summary>
        protected void PaintFilterCountBadge(Graphics g, int count, Point location, Color accentColor, Control? owner = null)
        {
            if (count == 0) return;

            string badgeText = count > 99 ? "99+" : count.ToString();
            var font = GetFont(9f, FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(badgeText, font);
            int pad = owner != null ? DpiScalingHelper.ScaleValue(12, owner) : 12;
            int badgeWidth = Math.Max(owner != null ? DpiScalingHelper.ScaleValue(24, owner) : 24, textSize.Width + pad);
            int badgeHeight = owner != null ? DpiScalingHelper.ScaleValue(20, owner) : 20;

            var badgeRect = new Rectangle(location.X, location.Y, badgeWidth, badgeHeight);

            using (var path = CreateRoundedRectanglePath(badgeRect, badgeHeight / 2))
            {
                g.FillPath(GetBrush(accentColor), path);
                g.DrawPath(GetPen(Color.FromArgb(60, accentColor), 2f), path);
            }

            TextRenderer.DrawText(g, badgeText, font, badgeRect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Paints a validation indicator icon (error, warning, success)
        /// </summary>
        protected void PaintValidationIndicator(Graphics g, Rectangle bounds, FilterValidationLevel level, string? tooltip = null)
        {
            Color indicatorColor;
            string iconText;
            var theme = BeepThemesManager.CurrentTheme;

            switch (level)
            {
                case FilterValidationLevel.Error:
                    indicatorColor = theme?.ErrorColor ?? Color.FromArgb(220, 53, 69); // Red
                    iconText = "✕";
                    break;

                case FilterValidationLevel.Warning:
                    indicatorColor = theme?.WarningColor ?? Color.FromArgb(255, 193, 7); // Amber
                    iconText = "⚠";
                    break;

                case FilterValidationLevel.Success:
                    indicatorColor = theme?.SuccessColor ?? Color.FromArgb(40, 167, 69); // Green
                    iconText = "✓";
                    break;

                default:
                    return;
            }

            // Draw circle background
            g.FillEllipse(GetBrush(indicatorColor), bounds);

            // Draw icon (white on colored fill)
            TextRenderer.DrawText(g, iconText, GetFont(bounds.Height * 0.6f, FontStyle.Bold), bounds, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Paints a column type icon for filter pills
        /// </summary>
        protected void PaintColumnTypeIcon(Graphics g, Rectangle bounds, Utilities.DbFieldCategory columnType, Color color)
        {
            FilterIconProvider.DrawTypeIcon(g, bounds, columnType, color);
        }

        /// <summary>
        /// Paints a keyboard shortcut hint
        /// </summary>
        protected void PaintKeyboardHint(Graphics g, Point location, string shortcut, Color backgroundColor, Color textColor, Control? owner = null)
        {
            var font = GetFont(8f);
            var textSize = TextRenderer.MeasureText(shortcut, font);
            int padW = owner != null ? DpiScalingHelper.ScaleValue(12, owner) : 12;
            int padH = owner != null ? DpiScalingHelper.ScaleValue(6,  owner) : 6;
            var hintRect = new Rectangle(
                location.X,
                location.Y,
                textSize.Width + padW,
                textSize.Height + padH
            );

            int radius = owner != null ? DpiScalingHelper.ScaleValue(4, owner) : 4;

            // Draw rounded background
            using (var path = CreateRoundedRectanglePath(hintRect, radius))
            {
                g.FillPath(GetBrush(Color.FromArgb(200, backgroundColor)), path);
                g.DrawPath(GetPen(Color.FromArgb(150, textColor), 1f), path);
            }

            // Draw text
            TextRenderer.DrawText(g, shortcut, font, hintRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Paints a loading spinner animation
        /// </summary>
        protected void PaintLoadingSpinner(Graphics g, Rectangle bounds, Color color, float progress)
        {
            float rotation = progress * 360f;
            float sweepAngle = 270f;

            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawArc(pen, bounds, rotation - 90, sweepAngle);
            }
        }

        #endregion
    }
}
