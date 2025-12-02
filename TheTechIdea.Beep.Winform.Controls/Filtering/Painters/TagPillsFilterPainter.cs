using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for TagPills filter style - horizontal row of tag chips with X buttons
    /// Based on sample image 3 - shows Field | Operator | Value in pill format
    /// Supports drag handles for reordering when enabled
    /// </summary>
    public class TagPillsFilterPainter : BaseFilterPainter
    {
        private const int PillSpacing = 8;
        private const int RowSpacing = 8;
        private const int MaxPillsPerRow = 10;
        private const int PillHeight = 32;
        private const int PillCornerRadius = 16;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.TagPills;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => true;

        /// <summary>Calculates layout positions for all filter elements.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int padding = 8;

            if (config == null || config.Criteria.Count == 0)
            {
                // Show "Add Filter" button
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    availableRect.Y + padding,
                    120,
                    PillHeight
                );
                return layout;
            }

            int currentX = availableRect.X + padding;
            int currentY = availableRect.Y + padding;
            int rowHeight = 0;
            int pillsInRow = 0;

            var tagRects = new List<Rectangle>();
            var removeRects = new List<Rectangle>();
            var dragRects = new List<Rectangle>();

            for (int i = 0; i < config.Criteria.Count; i++)
            {
                var criterion = config.Criteria[i];
                
                // Build pill text: "ColumnName Operator Value"
                var pillText = $"{criterion.ColumnName} {criterion.Operator.GetSymbol()} {criterion.Value}";
                
                // Measure pill size
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                using (var font = new Font("Segoe UI", 9f))
                {
                    var textSize = g.MeasureString(pillText, font);
                    int pillWidth = (int)textSize.Width + PillHeight + padding * 3 + (owner.EnableDragDrop ? 20 : 0);

                    // Check if we need to wrap to next row
                    if (currentX + pillWidth > availableRect.Right - padding || pillsInRow >= MaxPillsPerRow)
                    {
                        currentX = availableRect.X + padding;
                        currentY += rowHeight + RowSpacing;
                        rowHeight = 0;
                        pillsInRow = 0;
                    }

                    var pillRect = new Rectangle(currentX, currentY, pillWidth, PillHeight);
                    tagRects.Add(pillRect);

                    // Drag handle (left side of pill)
                    if (owner.EnableDragDrop)
                    {
                        var dragRect = new Rectangle(pillRect.X + 4, pillRect.Y + (pillRect.Height - 16) / 2, 12, 16);
                        dragRects.Add(dragRect);
                    }

                    // X button (right side of pill)
                    var xButtonRect = new Rectangle(
                        pillRect.Right - PillHeight + 4,
                        pillRect.Y + 4,
                        PillHeight - 8,
                        PillHeight - 8
                    );
                    removeRects.Add(xButtonRect);

                    currentX += pillWidth + PillSpacing;
                    rowHeight = Math.Max(rowHeight, PillHeight);
                    pillsInRow++;
                }
            }

            layout.TagRects = tagRects.ToArray();
            layout.RemoveButtonRects = removeRects.ToArray();
            layout.DragHandleRects = dragRects.ToArray();

            // Add "Add Filter" button at the end
            if (owner.ShowActionButtons)
            {
                if (currentX + 100 > availableRect.Right - padding)
                {
                    currentX = availableRect.X + padding;
                    currentY += rowHeight + RowSpacing;
                }

                layout.AddFilterButtonRect = new Rectangle(currentX, currentY, 100, PillHeight);
            }

            return layout;
        }

        /// <summary>Paints the filter UI elements.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (layout == null)
                return;

            var config = owner.ActiveFilter;
            
            // Paint each tag pill
            for (int i = 0; i < layout.TagRects.Length && i < config.Criteria.Count; i++)
            {
                var criterion = config.Criteria[i];
                var rect = layout.TagRects[i];
                var removeRect = i < layout.RemoveButtonRects.Length ? layout.RemoveButtonRects[i] : Rectangle.Empty;
                var dragRect = i < layout.DragHandleRects.Length ? layout.DragHandleRects[i] : Rectangle.Empty;

                PaintFilterPill(g, rect, criterion, dragRect, removeRect, owner);
            }

            // Paint "Add Filter" button
            if (owner.ShowActionButtons && !layout.AddFilterButtonRect.IsEmpty)
            {
                PaintAddFilterButton(g, layout.AddFilterButtonRect, owner);
            }

            // Phase 1: Paint filter count badge (top-right corner)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - 40,
                    layout.ContainerRect.Top + 8
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor);
            }
        }

        private void PaintFilterPill(Graphics g, Rectangle rect, FilterCriteria criterion, Rectangle dragRect, Rectangle removeRect, BeepFilter owner)
        {
            // Get colors using base class helper
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, PillCornerRadius))
            using (var brush = new SolidBrush(colors.background))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, PillCornerRadius))
            using (var pen = new Pen(colors.border, 1))
            {
                g.DrawPath(pen, path);
            }

            // Drag handle
            if (owner.EnableDragDrop && !dragRect.IsEmpty)
            {
                var dotColor = Color.FromArgb(150, colors.text);
                PaintDragHandle(g, dragRect, dotColor);
            }

            // Phase 1: Column type icon (if enabled)
            int textStartX = rect.X + (owner.EnableDragDrop ? 24 : 12);
            if (owner.ShowColumnTypeIcons)
            {
                var iconRect = new Rectangle(textStartX, rect.Y + (rect.Height - 16) / 2, 16, 16);
                // TODO: Get actual column type from EntityStructure
                var columnType = Utilities.DbFieldCategory.String; // Default
                PaintColumnTypeIcon(g, iconRect, columnType, colors.accent);
                textStartX += 20;
            }

            // Text: "Column Operator Value"
            var pillText = $"{criterion.ColumnName} {criterion.Operator.GetSymbol()} {criterion.Value}";
            var textRect = new Rectangle(
                textStartX,
                rect.Y,
                rect.Width - (textStartX - rect.X) - (owner.EnableDragDrop ? 36 : 24),
                rect.Height
            );

            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(criterion.IsEnabled ? colors.text : Color.FromArgb(150, colors.text)))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(pillText, font, brush, textRect, sf);
            }

            // Remove button (X)
            if (!removeRect.IsEmpty)
            {
                int xSize = 10;
                int xCenterX = removeRect.X + removeRect.Width / 2;
                int xCenterY = removeRect.Y + removeRect.Height / 2;

                using (var pen = new Pen(colors.text, 1.5f))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawLine(pen, xCenterX - xSize / 2, xCenterY - xSize / 2, xCenterX + xSize / 2, xCenterY + xSize / 2);
                    g.DrawLine(pen, xCenterX + xSize / 2, xCenterY - xSize / 2, xCenterX - xSize / 2, xCenterY + xSize / 2);
                }
            }
        }

        private void PaintAddFilterButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, 4))
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                g.FillPath(brush, path);
            }

            // Border (dashed)
            using (var path = CreateRoundedRectanglePath(rect, 4))
            using (var pen = new Pen(colors.border, 1))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawPath(pen, path);
            }

            // Text
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(colors.accent))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                g.DrawString("+ Add Rule", font, brush, rect, sf);
            }
        }

        /// <summary>Hit tests for tag pills, remove buttons, and drag handles.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check remove buttons first (they're on top of tags)
            for (int i = 0; i < layout.RemoveButtonRects.Length; i++)
            {
                if (layout.RemoveButtonRects[i].Contains(point))
                    return new FilterHitArea { Name = $"RemoveTag_{i}", Bounds = layout.RemoveButtonRects[i], Type = FilterHitAreaType.RemoveButton, Tag = i };
            }

            // Check drag handles
            for (int i = 0; i < layout.DragHandleRects.Length; i++)
            {
                if (layout.DragHandleRects[i].Contains(point))
                    return new FilterHitArea { Name = $"DragHandle_{i}", Bounds = layout.DragHandleRects[i], Type = FilterHitAreaType.DragHandle, Tag = i };
            }

            // Check tags
            for (int i = 0; i < layout.TagRects.Length; i++)
            {
                if (layout.TagRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Tag_{i}", Bounds = layout.TagRects[i], Type = FilterHitAreaType.FilterTag, Tag = i };
            }

            // Check add filter button
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddFilter", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            return null;
        }

        #region Generic Overloads for Non-BeepFilter Controls

        /// <summary>Calculates layout positions (generic overload for BaseControl-derived controls)</summary>
        public override FilterLayoutInfo CalculateLayout(Rectangle availableRect, FilterConfiguration config)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            int padding = 8;

            if (config == null || config.Criteria.Count == 0)
            {
                layout.AddFilterButtonRect = new Rectangle(availableRect.X + padding, availableRect.Y + padding, 120, PillHeight);
                return layout;
            }

            int currentX = availableRect.X + padding;
            int currentY = availableRect.Y + padding;
            int rowHeight = 0;
            int pillsInRow = 0;

            var tagRects = new List<Rectangle>();
            var removeRects = new List<Rectangle>();
            var dragRects = new List<Rectangle>();

            for (int i = 0; i < config.Criteria.Count; i++)
            {
                var criterion = config.Criteria[i];
                var pillText = $"{criterion.ColumnName} {criterion.Operator.GetSymbol()} {criterion.Value}";

                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                using (var font = new Font("Segoe UI", 9f))
                {
                    var textSize = g.MeasureString(pillText, font);
                    int pillWidth = (int)textSize.Width + PillHeight + padding * 3 + 20; // Include drag handle space

                    if (currentX + pillWidth > availableRect.Right - padding || pillsInRow >= MaxPillsPerRow)
                    {
                        currentX = availableRect.X + padding;
                        currentY += rowHeight + RowSpacing;
                        rowHeight = 0;
                        pillsInRow = 0;
                    }

                    var pillRect = new Rectangle(currentX, currentY, pillWidth, PillHeight);
                    tagRects.Add(pillRect);

                    var dragRect = new Rectangle(pillRect.X + 4, pillRect.Y + (pillRect.Height - 16) / 2, 12, 16);
                    dragRects.Add(dragRect);

                    var xButtonRect = new Rectangle(pillRect.Right - PillHeight + 4, pillRect.Y + 4, PillHeight - 8, PillHeight - 8);
                    removeRects.Add(xButtonRect);

                    currentX += pillWidth + PillSpacing;
                    rowHeight = Math.Max(rowHeight, PillHeight);
                    pillsInRow++;
                }
            }

            layout.TagRects = tagRects.ToArray();
            layout.RemoveButtonRects = removeRects.ToArray();
            layout.DragHandleRects = dragRects.ToArray();

            if (currentX + 100 > availableRect.Right - padding)
            {
                currentX = availableRect.X + padding;
                currentY += rowHeight + RowSpacing;
            }
            layout.AddFilterButtonRect = new Rectangle(currentX, currentY, 100, PillHeight);

            return layout;
        }

        /// <summary>Paints the filter UI (generic overload for BaseControl-derived controls)</summary>
        public override void Paint(Graphics g, Rectangle bounds, FilterConfiguration config, FilterLayoutInfo layout, IBeepTheme theme, FilterHitArea? hoveredArea, FilterHitArea? pressedArea)
        {
            if (layout == null || config == null) return;

            // Paint each tag pill
            for (int i = 0; i < layout.TagRects.Length && i < config.Criteria.Count; i++)
            {
                var criterion = config.Criteria[i];
                var rect = layout.TagRects[i];
                var removeRect = i < layout.RemoveButtonRects.Length ? layout.RemoveButtonRects[i] : Rectangle.Empty;
                var dragRect = i < layout.DragHandleRects.Length ? layout.DragHandleRects[i] : Rectangle.Empty;

                PaintFilterPillGeneric(g, rect, criterion, dragRect, removeRect, theme, hoveredArea, pressedArea, i);
            }

            // Paint "Add Filter" button
            if (!layout.AddFilterButtonRect.IsEmpty)
            {
                PaintAddFilterButtonGeneric(g, layout.AddFilterButtonRect, theme, hoveredArea);
            }
        }

        private void PaintFilterPillGeneric(Graphics g, Rectangle rect, FilterCriteria criterion, Rectangle dragRect, Rectangle removeRect, IBeepTheme theme, FilterHitArea? hoveredArea, FilterHitArea? pressedArea, int index)
        {
            bool isHovered = hoveredArea?.Tag is int hIndex && hIndex == index;
            bool isPressed = pressedArea?.Tag is int pIndex && pIndex == index;

            // Get colors from theme
            var bgColor = isPressed ? theme.ButtonHoverBackColor : isHovered ? theme.ButtonBackColor : theme.BackColor;
            var borderColor = theme.BorderColor;
            var textColor = theme.ForeColor;

            // Background
            using (var path = CreateRoundedRectanglePath(rect, PillCornerRadius))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, PillCornerRadius))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }

            // Drag handle
            if (!dragRect.IsEmpty)
            {
                using (var pen = new Pen(textColor, 2))
                {
                    g.DrawLine(pen, dragRect.X, dragRect.Y + 4, dragRect.X, dragRect.Bottom - 4);
                    g.DrawLine(pen, dragRect.X + 4, dragRect.Y + 4, dragRect.X + 4, dragRect.Bottom - 4);
                    g.DrawLine(pen, dragRect.X + 8, dragRect.Y + 4, dragRect.X + 8, dragRect.Bottom - 4);
                }
            }

            // Text
            var pillText = $"{criterion.ColumnName} {criterion.Operator.GetSymbol()} {criterion.Value}";
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(rect.X + (!dragRect.IsEmpty ? 24 : 8), rect.Y, rect.Width - 32, rect.Height);
                var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(pillText, font, brush, textRect, sf);
            }

            // X button
            if (!removeRect.IsEmpty)
            {
                bool xHovered = hoveredArea?.Type == FilterHitAreaType.RemoveButton && hoveredArea?.Tag is int rhIndex && rhIndex == index;
                using (var brush = new SolidBrush(xHovered ? Color.FromArgb(220, 53, 69) : Color.FromArgb(108, 117, 125)))
                using (var pen = new Pen(xHovered ? Color.White : Color.FromArgb(173, 181, 189), 2))
                {
                    if (xHovered)
                    {
                        g.FillEllipse(brush, removeRect);
                    }
                    int cx = removeRect.X + removeRect.Width / 2;
                    int cy = removeRect.Y + removeRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy - 4, cx + 4, cy + 4);
                    g.DrawLine(pen, cx - 4, cy + 4, cx + 4, cy - 4);
                }
            }
        }

        private void PaintAddFilterButtonGeneric(Graphics g, Rectangle rect, IBeepTheme theme, FilterHitArea? hoveredArea)
        {
            bool isHovered = hoveredArea?.Type == FilterHitAreaType.AddFilterButton;

            using (var path = CreateRoundedRectanglePath(rect, 4))
            using (var pen = new Pen(theme.BorderColor, 1))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawPath(pen, path);
            }

            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(isHovered ? theme.AccentColor : theme.ForeColor))
            {
                var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                g.DrawString("+ Add Rule", font, brush, rect, sf);
            }
        }

        #endregion
    }
}
