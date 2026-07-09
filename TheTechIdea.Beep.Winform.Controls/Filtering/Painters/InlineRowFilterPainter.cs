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
            int sButtonSize = DpiScalingHelper.ScaleValue(ButtonSize, owner);
            int sColumnWidth = DpiScalingHelper.ScaleValue(ColumnWidth, owner);
            int sOperatorWidth = DpiScalingHelper.ScaleValue(OperatorWidth, owner);
            int sValueWidth = DpiScalingHelper.ScaleValue(ValueWidth, owner);
            int sRowHeight = DpiScalingHelper.ScaleValue(RowHeight, owner);
            int sRowSpacing = DpiScalingHelper.ScaleValue(RowSpacing, owner);
            int sItemSpacing = DpiScalingHelper.ScaleValue(ItemSpacing, owner);

            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            layout.DpiScale = Helpers.DpiScalingHelper.GetDpiScaleFactor(owner);

            var config = owner.ActiveFilter;
            int padding = DpiScalingHelper.ScaleValue(4, owner);
            int currentY = availableRect.Y + padding;

            if (config == null || config.Criteria.Count == 0)
            {
                // Show compact "+" button
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    sButtonSize,
                    sButtonSize
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
                    sColumnWidth + sOperatorWidth + sValueWidth + sButtonSize + sItemSpacing * 3,
                    sRowHeight
                );
                rowRects.Add(rowRect);

                currentY += sRowHeight + sRowSpacing;
            }

            layout.RowRects = rowRects.ToArray();

            // Add Filter button
            if (owner.ShowActionButtons)
            {
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    sButtonSize,
                    sButtonSize
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
                    layout.ContainerRect.Right - DpiScalingHelper.ScaleValue(35, owner),
                    layout.ContainerRect.Top + DpiScalingHelper.ScaleValue(4, owner)
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor, owner);
            }
        }

        private void PaintInlineRow(Graphics g, Rectangle rect, FilterCriteria criterion, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int sColumnWidth = DpiScalingHelper.ScaleValue(ColumnWidth, owner);
            int sOperatorWidth = DpiScalingHelper.ScaleValue(OperatorWidth, owner);
            int sValueWidth = DpiScalingHelper.ScaleValue(ValueWidth, owner);
            int sButtonSize = DpiScalingHelper.ScaleValue(ButtonSize, owner);
            int sItemSpacing = DpiScalingHelper.ScaleValue(ItemSpacing, owner);
            int sRowHeight = DpiScalingHelper.ScaleValue(RowHeight, owner);
            int currentX = rect.X;

            // Column dropdown
            var columnRect = new Rectangle(currentX, rect.Y, sColumnWidth, sRowHeight);
            PaintCompactDropdown(g, columnRect, criterion.ColumnName, owner);
            currentX += sColumnWidth + sItemSpacing;

            // Operator dropdown
            var operatorRect = new Rectangle(currentX, rect.Y, sOperatorWidth, sRowHeight);
            PaintCompactDropdown(g, operatorRect, criterion.Operator.GetDisplayName(), owner);
            currentX += sOperatorWidth + sItemSpacing;

            // Value input
            var valueRect = new Rectangle(currentX, rect.Y, sValueWidth, sRowHeight);
            PaintCompactInput(g, valueRect, criterion.Value?.ToString() ?? "", owner);
            currentX += sValueWidth + sItemSpacing;

            // Remove button
            var removeRect = new Rectangle(currentX, rect.Y + (sRowHeight - sButtonSize) / 2, sButtonSize, sButtonSize);
            PaintRemoveButton(g, removeRect, owner);
        }

        private void PaintCompactDropdown(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s3 = DpiScalingHelper.ScaleValue(3, owner);
            int s6 = DpiScalingHelper.ScaleValue(6, owner);
            int s20 = DpiScalingHelper.ScaleValue(20, owner);
            int s12 = DpiScalingHelper.ScaleValue(12, owner);
            int sArrow3 = DpiScalingHelper.ScaleValue(3, owner);
            int s2 = DpiScalingHelper.ScaleValue(2, owner);
            int s1 = DpiScalingHelper.ScaleValue(1, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s3))
            {
                g.FillPath(GetBrush(colors.background), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, s3))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text
            var textRect = new Rectangle(rect.X + s6, rect.Y, rect.Width - s20, rect.Height);
            TextRenderer.DrawText(g, text, GetFont(8.5f), textRect, colors.text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Arrow
            int arrowX = rect.Right - s12;
            int arrowY = rect.Y + rect.Height / 2;
            var arrowPen = GetPen(Color.FromArgb(150, colors.text), 1.2f);
            g.DrawLine(arrowPen, arrowX - sArrow3, arrowY - s2, arrowX, arrowY + s1);
            g.DrawLine(arrowPen, arrowX, arrowY + s1, arrowX + sArrow3, arrowY - s2);
        }

        private void PaintCompactInput(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s3 = DpiScalingHelper.ScaleValue(3, owner);
            int s6 = DpiScalingHelper.ScaleValue(6, owner);
            int s12 = DpiScalingHelper.ScaleValue(12, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s3))
            {
                g.FillPath(GetBrush(Color.White), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, s3))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text
            var displayText = string.IsNullOrEmpty(text) ? "Value" : text;
            var textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(180, colors.text) : colors.text;

            var textRect = new Rectangle(rect.X + s6, rect.Y, rect.Width - s12, rect.Height);
            TextRenderer.DrawText(g, displayText, GetFont(8.5f), textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            // Background
            g.FillEllipse(GetBrush(Color.FromArgb(245, 245, 245)), rect);

            // Border
            g.DrawEllipse(GetPen(Color.FromArgb(220, 220, 220), DpiScalingHelper.ScaleValue(1, owner)), rect);

            // X
            int xSize = DpiScalingHelper.ScaleValue(6, owner);
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
            var colors = GetStyleColors(owner, owner.ControlStyle);

            // Background
            g.FillEllipse(GetBrush(Color.FromArgb(240, 240, 240)), rect);

            // Border
            using (var pen = new Pen(colors.border, DpiScalingHelper.ScaleValue(1, owner)))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawEllipse(pen, rect);
            }

            // Plus sign
            int plusSize = DpiScalingHelper.ScaleValue(8, owner);
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
                Rectangle columnRect = new Rectangle(currentX, rowRect.Y, Helpers.DpiScalingHelper.ScaleValue(ColumnWidth, layout.DpiScale), rowRect.Height);
                if (columnRect.Contains(point))
                    return new FilterHitArea { Name = $"Field_{i}", Bounds = columnRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
                currentX += Helpers.DpiScalingHelper.ScaleValue(ColumnWidth, layout.DpiScale) + ItemSpacing;

                // Operator dropdown
                Rectangle operatorRect = new Rectangle(currentX, rowRect.Y, Helpers.DpiScalingHelper.ScaleValue(OperatorWidth, layout.DpiScale), rowRect.Height);
                if (operatorRect.Contains(point))
                    return new FilterHitArea { Name = $"Operator_{i}", Bounds = operatorRect, Type = FilterHitAreaType.OperatorDropdown, Tag = i };
                currentX += Helpers.DpiScalingHelper.ScaleValue(OperatorWidth, layout.DpiScale) + ItemSpacing;

                // Value input
                Rectangle valueRect = new Rectangle(currentX, rowRect.Y, Helpers.DpiScalingHelper.ScaleValue(ValueWidth, layout.DpiScale), rowRect.Height);
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
