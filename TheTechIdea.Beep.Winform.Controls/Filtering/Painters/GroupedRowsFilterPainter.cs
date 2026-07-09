using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for GroupedRows filter style - filter rows with AND/OR logic connectors
    /// Based on samples 1, 2, 6 - supports nested groups, drag handles, and visual indentation
    /// </summary>
    public class GroupedRowsFilterPainter : BaseFilterPainter
    {
        private const int RowHeight = 40;
        private const int RowSpacing = 8;
        private const int IndentWidth = 32;
        private const int ConnectorWidth = 60;
        private const int ConnectorHeight = 24;
        private const int DragHandleWidth = 20;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.GroupedRows;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => true;

        /// <summary>Calculates layout positions for all filter elements.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            int sPadding = DpiScalingHelper.ScaleValue(8, owner);
            int sRowHeight = DpiScalingHelper.ScaleValue(RowHeight, owner);
            int sRowSpacing = DpiScalingHelper.ScaleValue(RowSpacing, owner);
            int sDragHandleWidth = DpiScalingHelper.ScaleValue(DragHandleWidth, owner);
            int sConnectorWidth = DpiScalingHelper.ScaleValue(ConnectorWidth, owner);
            int sConnectorHeight = DpiScalingHelper.ScaleValue(ConnectorHeight, owner);
            int sButtonWidth = DpiScalingHelper.ScaleValue(150, owner);
            int sGap8 = DpiScalingHelper.ScaleValue(8, owner);
            int sConnectorGap4 = DpiScalingHelper.ScaleValue(4, owner);
            int sDragHandleH16 = DpiScalingHelper.ScaleValue(16, owner);

            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int padding = sPadding;

            if (config == null || config.Criteria.Count == 0)
            {
                // Show "Add Filter" button
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    availableRect.Y + padding,
                    sButtonWidth,
                    sRowHeight
                );
                return layout;
            }

            int currentY = availableRect.Y + padding;
            int rowWidth = availableRect.Width - padding * 2;

            var rowRects = new List<Rectangle>();
            var connectorRects = new List<Rectangle>();
            var dragHandleRects = new List<Rectangle>();

            for (int i = 0; i < config.Criteria.Count; i++)
            {
                // Logic connector (AND/OR) - except for first row
                if (i > 0)
                {
                    var connectorRect = new Rectangle(
                        availableRect.X + padding + sDragHandleWidth,
                        currentY,
                        sConnectorWidth,
                        sConnectorHeight
                    );
                    connectorRects.Add(connectorRect);
                    currentY += sConnectorHeight + sConnectorGap4;
                }

                // Filter row
                var rowRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    rowWidth,
                    sRowHeight
                );
                rowRects.Add(rowRect);

                // Drag handle
                if (owner.EnableDragDrop)
                {
                    var dragRect = new Rectangle(
                        rowRect.X,
                        rowRect.Y + (rowRect.Height - sDragHandleH16) / 2,
                        sDragHandleWidth,
                        sDragHandleH16
                    );
                    dragHandleRects.Add(dragRect);
                }

                currentY += sRowHeight + sRowSpacing;
            }

            layout.RowRects = rowRects.ToArray();
            layout.ConnectorRects = connectorRects.ToArray();
            layout.DragHandleRects = dragHandleRects.ToArray();

            // Add Filter button
            if (owner.ShowActionButtons)
            {
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding + sDragHandleWidth,
                    currentY,
                    sButtonWidth,
                    sRowHeight
                );

                // Add Group button (next to Add Filter)
                layout.AddGroupButtonRect = new Rectangle(
                    layout.AddFilterButtonRect.Right + sGap8,
                    currentY,
                    sButtonWidth,
                    sRowHeight
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
            int connectorIndex = 0;

            for (int i = 0; i < layout.RowRects.Length && i < config.Criteria.Count; i++)
            {
                // Paint logic connector before row (except first)
                if (i > 0 && connectorIndex < layout.ConnectorRects.Length)
                {
                    PaintLogicConnectorButton(g, layout.ConnectorRects[connectorIndex], config.Logic.ToString().ToUpper(), owner);
                    connectorIndex++;
                }

                // Paint drag handle
                if (owner.EnableDragDrop && i < layout.DragHandleRects.Length)
                {
                    var colors = GetStyleColors(owner, owner.ControlStyle);
                    PaintDragHandle(g, layout.DragHandleRects[i], Color.FromArgb(150, colors.text), owner);
                }

                // Paint filter row
                PaintFilterRow(g, layout.RowRects[i], config.Criteria[i], owner);
            }

            // Paint action buttons
            if (owner.ShowActionButtons)
            {
                if (!layout.AddFilterButtonRect.IsEmpty)
                    PaintAddFilterButton(g, layout.AddFilterButtonRect, "+ Add filter", owner);

                if (!layout.AddGroupButtonRect.IsEmpty)
                    PaintAddFilterButton(g, layout.AddGroupButtonRect, "+ Add inner group", owner, true);
            }

            // Phase 1: Paint filter count badge (top-right corner)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - DpiScalingHelper.ScaleValue(40, owner),
                    layout.ContainerRect.Top + DpiScalingHelper.ScaleValue(8, owner)
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor, owner);
            }
        }

        private void PaintFilterRow(Graphics g, Rectangle rect, FilterCriteria criterion, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s8 = DpiScalingHelper.ScaleValue(8, owner);
            int s4 = DpiScalingHelper.ScaleValue(4, owner);
            int s40 = DpiScalingHelper.ScaleValue(40, owner);
            int s36 = DpiScalingHelper.ScaleValue(36, owner);
            int s28 = DpiScalingHelper.ScaleValue(28, owner);
            int dragOffset = owner.EnableDragDrop ? DpiScalingHelper.ScaleValue(DragHandleWidth, owner) + s8 : s8;

            // Calculate areas within row
            int availableWidth = rect.Width - dragOffset - s40; // 40 for remove button
            int columnWidth = availableWidth / 3;
            int operatorWidth = availableWidth / 3;
            int valueWidth = availableWidth / 3;

            var columnRect = new Rectangle(rect.X + dragOffset, rect.Y + s4, columnWidth - s4, rect.Height - s8);
            var operatorRect = new Rectangle(columnRect.Right + s4, rect.Y + s4, operatorWidth - s4, rect.Height - s8);
            var valueRect = new Rectangle(operatorRect.Right + s4, rect.Y + s4, valueWidth - s4, rect.Height - s8);
            var removeRect = new Rectangle(rect.Right - s36, rect.Y + (rect.Height - s28) / 2, s28, s28);

            // Background
            g.FillRectangle(GetBrush(colors.background), rect);

            // Dropdowns
            PaintDropdownField(g, columnRect, criterion.ColumnName, owner);
            PaintDropdownField(g, operatorRect, criterion.Operator.GetDisplayName(), owner);
            PaintValueField(g, valueRect, criterion.Value?.ToString() ?? "", owner);

            // Remove button
            PaintRemoveButton(g, removeRect, owner);
        }

        private void PaintDropdownField(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s4 = DpiScalingHelper.ScaleValue(4, owner);
            int s8 = DpiScalingHelper.ScaleValue(8, owner);
            int s24 = DpiScalingHelper.ScaleValue(24, owner);
            int s16 = DpiScalingHelper.ScaleValue(16, owner);
            int sArrow4 = DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.FillPath(GetBrush(colors.background), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text
            var textRect = new Rectangle(rect.X + s8, rect.Y, rect.Width - s24, rect.Height);
            TextRenderer.DrawText(g, text, GetFont(9f), textRect, colors.text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Dropdown arrow
            int arrowX = rect.Right - s16;
            int arrowY = rect.Y + rect.Height / 2;
            var arrowPen = GetPen(colors.text, 1.5f);
            g.DrawLine(arrowPen, arrowX - sArrow4, arrowY - 2, arrowX, arrowY + 2);
            g.DrawLine(arrowPen, arrowX, arrowY + 2, arrowX + sArrow4, arrowY - 2);
        }

        private void PaintValueField(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s4 = DpiScalingHelper.ScaleValue(4, owner);
            int s8 = DpiScalingHelper.ScaleValue(8, owner);
            int s16 = DpiScalingHelper.ScaleValue(16, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.FillPath(GetBrush(colors.background), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text or placeholder
            var displayText = string.IsNullOrEmpty(text) ? "Enter value..." : text;
            var textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(150, colors.text) : colors.text;

            var textRect = new Rectangle(rect.X + s8, rect.Y, rect.Width - s16, rect.Height);
            TextRenderer.DrawText(g, displayText, GetFont(9f), textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void PaintLogicConnectorButton(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            // Background (rounded pill)
            using (var path = CreateRoundedRectanglePath(rect, rect.Height / 2))
            {
                g.FillPath(GetBrush(Color.FromArgb(245, 245, 245)), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, rect.Height / 2))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text
            TextRenderer.DrawText(g, text, GetFont(8f, FontStyle.Bold), rect, colors.text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            // Circle background
            g.FillEllipse(GetBrush(Color.FromArgb(250, 250, 250)), rect);

            // Border
            g.DrawEllipse(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), rect);

            // X
            int xSize = DpiScalingHelper.ScaleValue(8, owner);
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            using (var pen = new Pen(Color.FromArgb(200, 100, 100), 1.5f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, centerX - xSize / 2, centerY - xSize / 2, centerX + xSize / 2, centerY + xSize / 2);
                g.DrawLine(pen, centerX + xSize / 2, centerY - xSize / 2, centerX - xSize / 2, centerY + xSize / 2);
            }
        }

        private void PaintAddFilterButton(Graphics g, Rectangle rect, string text, BeepFilter owner, bool isGroup = false)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);
            int s4 = DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.FillPath(GetBrush(Color.FromArgb(248, 248, 248)), path);
            }

            // Border (dashed)
            using (var pen = new Pen(colors.border, DpiScalingHelper.ScaleValue(1, owner)))
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawPath(pen, path);
            }

            // Text
            TextRenderer.DrawText(g, text, GetFont(9f), rect, colors.accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>Hit tests for filter rows, dropdowns, remove buttons, connectors, and drag handles.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check drag handles first
            for (int i = 0; i < layout.DragHandleRects.Length; i++)
            {
                if (layout.DragHandleRects[i].Contains(point))
                    return new FilterHitArea { Name = $"DragHandle_{i}", Bounds = layout.DragHandleRects[i], Type = FilterHitAreaType.DragHandle, Tag = i };
            }

            // Check logic connectors (AND/OR buttons)
            for (int i = 0; i < layout.ConnectorRects.Length; i++)
            {
                if (layout.ConnectorRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Connector_{i}", Bounds = layout.ConnectorRects[i], Type = FilterHitAreaType.LogicConnector, Tag = i };
            }

            // Check rows and their dropdowns
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                Rectangle rowRect = layout.RowRects[i];
                
                // Check remove button (right side of row)
                Rectangle removeRect = new Rectangle(rowRect.Right - 30, rowRect.Y + (rowRect.Height - 24) / 2, 24, 24);
                if (removeRect.Contains(point))
                    return new FilterHitArea { Name = $"Remove_{i}", Bounds = removeRect, Type = FilterHitAreaType.RemoveButton, Tag = i };

                // Check field dropdown
                if (i < layout.FieldDropdownRects.Length && layout.FieldDropdownRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Field_{i}", Bounds = layout.FieldDropdownRects[i], Type = FilterHitAreaType.FieldDropdown, Tag = i };

                // Check operator dropdown
                if (i < layout.OperatorDropdownRects.Length && layout.OperatorDropdownRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Operator_{i}", Bounds = layout.OperatorDropdownRects[i], Type = FilterHitAreaType.OperatorDropdown, Tag = i };

                // Check value input
                if (i < layout.ValueDropdownRects.Length && layout.ValueDropdownRects[i].Contains(point))
                    return new FilterHitArea { Name = $"Value_{i}", Bounds = layout.ValueDropdownRects[i], Type = FilterHitAreaType.ValueInput, Tag = i };

                // Entire row as fallback
                if (rowRect.Contains(point))
                    return new FilterHitArea { Name = $"Row_{i}", Bounds = rowRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
            }

            // Check action buttons
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddFilter", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            if (layout.AddGroupButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddGroup", Bounds = layout.AddGroupButtonRect, Type = FilterHitAreaType.AddGroupButton };

            return null;
        }
    }
}
