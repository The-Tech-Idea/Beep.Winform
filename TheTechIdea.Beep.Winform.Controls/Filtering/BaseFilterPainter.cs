using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Base implementation of IFilterPainter with common functionality
    /// Provides helper methods for painting, hit testing, and layout
    /// Derived painters implement style-specific rendering
    /// </summary>
    public abstract class BaseFilterPainter : IFilterPainter, IFilterPainterMetricsProvider
    {
        #region Properties

        public abstract FilterStyle FilterStyle { get; }
        public virtual bool SupportsAnimations => true;
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
            using (var brush = new SolidBrush(theme.BackColor))
            {
                g.FillRectangle(brush, bounds);
            }
            
            using (var pen = new Pen(theme.BorderColor))
            {
                g.DrawRectangle(pen, bounds);
            }
            
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(theme.ForeColor))
            {
                var text = $"{config.Criteria.Count} filter(s) - Override Paint method";
                g.DrawString(text, font, brush, new PointF(bounds.X + 10, bounds.Y + 10));
            }
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
                using (var brush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(brush, path);
                }

                // Draw border
                if (borderWidth > 0)
                {
                    using (var pen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Paint a tag/chip pill
        /// </summary>
        protected void PaintTagPill(Graphics g, Rectangle rect, string text, Font font, Color backgroundColor, Color textColor, Color borderColor, int cornerRadius, bool showRemove = true)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                // Fill
                using (var brush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(brush, path);
                }

                // Border
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            var textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - (showRemove ? 28 : 16), rect.Height);
            TextRenderer.DrawText(g, text, font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Remove button (X)
            if (showRemove)
            {
                int xSize = 12;
                int xX = rect.Right - 18;
                int xY = rect.Y + rect.Height / 2;

                using (var pen = new Pen(textColor, 1.5f))
                {
                    g.DrawLine(pen, xX - xSize / 2, xY - xSize / 2, xX + xSize / 2, xY + xSize / 2);
                    g.DrawLine(pen, xX + xSize / 2, xY - xSize / 2, xX - xSize / 2, xY + xSize / 2);
                }
            }
        }

        /// <summary>
        /// Paint a logic connector (AND/OR)
        /// </summary>
        protected void PaintLogicConnector(Graphics g, Rectangle rect, string text, Font font, Color backgroundColor, Color textColor, Color borderColor, bool vertical = false)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background circle/pill
            using (var brush = new SolidBrush(backgroundColor))
            {
                if (vertical)
                    g.FillEllipse(brush, rect);
                else
                    g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(borderColor, 1))
            {
                if (vertical)
                    g.DrawEllipse(pen, rect);
                else
                    g.DrawRectangle(pen, rect);
            }

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
            using (var brush = new SolidBrush(backgroundColor))
            {
                g.FillEllipse(brush, rect);
            }

            // Text
            string countText = count > 99 ? "99+" : count.ToString();
            TextRenderer.DrawText(g, countText, font, rect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// Paint a drag handle (6 dots)
        /// </summary>
        protected void PaintDragHandle(Graphics g, Rectangle rect, Color dotColor)
        {
            int dotSize = 3;
            int spacing = 4;
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            using (var brush = new SolidBrush(dotColor))
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 2; col++)
                    {
                        int x = centerX - spacing / 2 + col * spacing - dotSize / 2;
                        int y = centerY - spacing + row * spacing - dotSize / 2;
                        g.FillEllipse(brush, x, y, dotSize, dotSize);
                    }
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
            // These should come from BeepStyling - placeholder for now
            // TODO: Hook into BeepStyling.GetBackgroundColor, GetBorderColor, etc.
            return (
                background: Color.White,
                border: Color.FromArgb(224, 224, 224),
                text: Color.FromArgb(33, 33, 33),
                accent: Color.FromArgb(33, 150, 243)
            );
        }

        #endregion
    }
}
