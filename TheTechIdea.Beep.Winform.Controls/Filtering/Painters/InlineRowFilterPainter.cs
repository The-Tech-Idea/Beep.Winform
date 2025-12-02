using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for InlineRow filter style - compact single-line filter rows
    /// Minimal spacing, space-efficient design
    /// </summary>
    public class InlineRowFilterPainter : BaseFilterPainter
    {
        private const int RowHeight = 28;
        private const int RowSpacing = 4;
        private const int ColumnWidth = 120;
        private const int OperatorWidth = 90;
        private const int ValueWidth = 120;
        private const int ButtonSize = 22;
        private const int ItemSpacing = 4;

        public override FilterStyle FilterStyle => FilterStyle.InlineRow;
        public override bool SupportsDragDrop => false;

        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int padding = 4;
            int currentY = availableRect.Y + padding;

            if (config == null || config.Criteria.Count == 0)
            {
                // Show compact "+" button
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    ButtonSize,
                    ButtonSize
                );
                return layout;
            }

            var rowRects = new List<Rectangle>();

            for (int i = 0; i < config.Criteria.Count; i++)
            {
                int currentX = availableRect.X + padding;

                // Full row rect
                var rowRect = new Rectangle(
                    currentX,
                    currentY,
                    ColumnWidth + OperatorWidth + ValueWidth + ButtonSize + ItemSpacing * 3,
                    RowHeight
                );
                rowRects.Add(rowRect);

                currentY += RowHeight + RowSpacing;
            }

            layout.RowRects = rowRects.ToArray();

            // Add Filter button
            if (owner.ShowActionButtons)
            {
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    ButtonSize,
                    ButtonSize
                );
            }

            return layout;
        }

        /// <summary>Paints the filter UI elements.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (layout == null)
                return;

            var config = owner.ActiveFilter;

            // Paint each inline row
            for (int i = 0; i < layout.RowRects.Length && i < config.Criteria.Count; i++)
            {
                PaintInlineRow(g, layout.RowRects[i], config.Criteria[i], owner);
            }

            // Paint "Add" button
            if (owner.ShowActionButtons && !layout.AddFilterButtonRect.IsEmpty)
            {
                PaintAddButton(g, layout.AddFilterButtonRect, owner);
            }

            // Phase 1: Paint filter count badge (compact, top-right)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - 35,
                    layout.ContainerRect.Top + 4
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor);
            }
        }

        private void PaintInlineRow(Graphics g, Rectangle rect, FilterCriteria criterion, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);
            int currentX = rect.X;

            // Column dropdown
            var columnRect = new Rectangle(currentX, rect.Y, ColumnWidth, RowHeight);
            PaintCompactDropdown(g, columnRect, criterion.ColumnName, owner);
            currentX += ColumnWidth + ItemSpacing;

            // Operator dropdown
            var operatorRect = new Rectangle(currentX, rect.Y, OperatorWidth, RowHeight);
            PaintCompactDropdown(g, operatorRect, criterion.Operator.GetDisplayName(), owner);
            currentX += OperatorWidth + ItemSpacing;

            // Value input
            var valueRect = new Rectangle(currentX, rect.Y, ValueWidth, RowHeight);
            PaintCompactInput(g, valueRect, criterion.Value?.ToString() ?? "", owner);
            currentX += ValueWidth + ItemSpacing;

            // Remove button
            var removeRect = new Rectangle(currentX, rect.Y + (RowHeight - ButtonSize) / 2, ButtonSize, ButtonSize);
            PaintRemoveButton(g, removeRect, owner);
        }

        private void PaintCompactDropdown(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 3))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 3))
            {
                g.DrawPath(pen, path);
            }

            // Text
            var textRect = new Rectangle(rect.X + 6, rect.Y, rect.Width - 20, rect.Height);
            using (var font = new Font("Segoe UI", 8.5f))
            using (var brush = new SolidBrush(colors.text))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(text, font, brush, textRect, sf);
            }

            // Arrow
            int arrowX = rect.Right - 12;
            int arrowY = rect.Y + rect.Height / 2;
            using (var pen = new Pen(Color.FromArgb(150, colors.text), 1.2f))
            {
                g.DrawLine(pen, arrowX - 3, arrowY - 2, arrowX, arrowY + 1);
                g.DrawLine(pen, arrowX, arrowY + 1, arrowX + 3, arrowY - 2);
            }
        }

        private void PaintCompactInput(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 3))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 3))
            {
                g.DrawPath(pen, path);
            }

            // Text
            var displayText = string.IsNullOrEmpty(text) ? "Value" : text;
            var textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(180, colors.text) : colors.text;

            var textRect = new Rectangle(rect.X + 6, rect.Y, rect.Width - 12, rect.Height);
            using (var font = new Font("Segoe UI", 8.5f))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(displayText, font, brush, textRect, sf);
            }
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
            {
                g.FillEllipse(brush, rect);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
            {
                g.DrawEllipse(pen, rect);
            }

            // X
            int xSize = 6;
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            using (var pen = new Pen(Color.FromArgb(150, 150, 150), 1.3f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, centerX - xSize / 2, centerY - xSize / 2, centerX + xSize / 2, centerY + xSize / 2);
                g.DrawLine(pen, centerX + xSize / 2, centerY - xSize / 2, centerX - xSize / 2, centerY + xSize / 2);
            }
        }

        private void PaintAddButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                g.FillEllipse(brush, rect);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawEllipse(pen, rect);
            }

            // Plus sign
            int plusSize = 8;
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            using (var pen = new Pen(colors.accent, 1.5f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, centerX, centerY - plusSize / 2, centerX, centerY + plusSize / 2);
                g.DrawLine(pen, centerX - plusSize / 2, centerY, centerX + plusSize / 2, centerY);
            }
        }

        /// <summary>Hit tests for inline row dropdowns and inputs.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check rows and their components
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                Rectangle rowRect = layout.RowRects[i];
                
                // Calculate positions within inline row
                int currentX = rowRect.X;
                
                // Remove button (right side)
                Rectangle removeRect = new Rectangle(rowRect.Right - 16, rowRect.Y + (rowRect.Height - 14) / 2, 14, 14);
                if (removeRect.Contains(point))
                    return new FilterHitArea { Name = $"Remove_{i}", Bounds = removeRect, Type = FilterHitAreaType.RemoveButton, Tag = i };

                // Column dropdown
                Rectangle columnRect = new Rectangle(currentX, rowRect.Y, ColumnWidth, rowRect.Height);
                if (columnRect.Contains(point))
                    return new FilterHitArea { Name = $"Field_{i}", Bounds = columnRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
                currentX += ColumnWidth + ItemSpacing;

                // Operator dropdown
                Rectangle operatorRect = new Rectangle(currentX, rowRect.Y, OperatorWidth, rowRect.Height);
                if (operatorRect.Contains(point))
                    return new FilterHitArea { Name = $"Operator_{i}", Bounds = operatorRect, Type = FilterHitAreaType.OperatorDropdown, Tag = i };
                currentX += OperatorWidth + ItemSpacing;

                // Value input
                Rectangle valueRect = new Rectangle(currentX, rowRect.Y, ValueWidth, rowRect.Height);
                if (valueRect.Contains(point))
                    return new FilterHitArea { Name = $"Value_{i}", Bounds = valueRect, Type = FilterHitAreaType.ValueInput, Tag = i };
            }

            // Check add button
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddFilter", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            return null;
        }
    }
}
