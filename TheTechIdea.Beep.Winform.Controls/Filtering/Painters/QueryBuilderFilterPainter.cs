using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for QueryBuilder filter style - tree-style hierarchical query builder
    /// Based on sample images 1 and 7 - shows If/Then/Else logic with visual tree structure
    /// Features: nested groups, drag handles, visual connectors, expandable/collapsible groups
    /// </summary>
    public class QueryBuilderFilterPainter : BaseFilterPainter
    {
        private const int IndentWidth = 32;
        private const int RowHeight = 40;
        private const int RowSpacing = 4;
        private const int DragHandleWidth = 20;
        private const int ExpandToggleSize = 16;
        private const int ColumnWidth = 140;
        private const int OperatorWidth = 100;
        private const int ValueWidth = 140;
        private const int ItemSpacing = 8;
        private const int GroupHeaderHeight = 36;
        private const int ConnectorLineWidth = 2;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.QueryBuilder;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => true;

        /// <summary>Calculates layout positions for all filter elements in tree structure.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int padding = 8;
            int currentY = availableRect.Y + padding;

            if (config == null || config.Criteria.Count == 0)
            {
                // Show root "Add Filter" button
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    150,
                    32
                );
                return layout;
            }

            var rowRects = new List<Rectangle>();
            var connectorRects = new List<Rectangle>();
            var dragHandleRects = new List<Rectangle>();
            var toggleRects = new List<Rectangle>();

            // Calculate tree structure layout
            currentY = CalculateGroupLayout(
                owner,
                availableRect.X + padding,
                currentY,
                availableRect.Width - padding * 2,
                0, // indent level
                config.Criteria,
                rowRects,
                connectorRects,
                dragHandleRects,
                toggleRects
            );

            layout.RowRects = rowRects.ToArray();
            layout.ConnectorRects = connectorRects.ToArray();
            layout.DragHandleRects = dragHandleRects.ToArray();

            // Add root "Add Filter" button at bottom
            currentY += RowSpacing * 2;
            layout.AddFilterButtonRect = new Rectangle(
                availableRect.X + padding + IndentWidth,
                currentY,
                120,
                28
            );

            // Add "Add Group" button
            layout.AddGroupButtonRect = new Rectangle(
                layout.AddFilterButtonRect.Right + ItemSpacing,
                currentY,
                120,
                28
            );

            return layout;
        }

        private int CalculateGroupLayout(
            BeepFilter owner,
            int startX,
            int startY,
            int availableWidth,
            int indentLevel,
            List<FilterCriteria> criteria,
            List<Rectangle> rowRects,
            List<Rectangle> connectorRects,
            List<Rectangle> dragHandleRects,
            List<Rectangle> toggleRects)
        {
            int currentY = startY;
            int indent = indentLevel * IndentWidth;
            int rowX = startX + indent;

            // Group header: "All of the following are true" / "Any of the following are true"
            if (indentLevel > 0)
            {
                Rectangle groupHeaderRect = new Rectangle(
                    rowX,
                    currentY,
                    availableWidth - indent,
                    GroupHeaderHeight
                );
                connectorRects.Add(groupHeaderRect); // Reuse for group headers
                currentY += GroupHeaderHeight + RowSpacing;
            }

            for (int i = 0; i < criteria.Count; i++)
            {
                var criterion = criteria[i];
                
                // Drag handle
                if (owner.EnableDragDrop)
                {
                    dragHandleRects.Add(new Rectangle(
                        rowX,
                        currentY + (RowHeight - 16) / 2,
                        DragHandleWidth,
                        16
                    ));
                }

                int contentX = rowX + (owner.EnableDragDrop ? DragHandleWidth + 4 : 0);

                // Row for condition: Column | Operator | Value
                Rectangle rowRect = new Rectangle(
                    contentX,
                    currentY,
                    ColumnWidth + OperatorWidth + ValueWidth + ItemSpacing * 2,
                    RowHeight
                );
                rowRects.Add(rowRect);

                // Connector line from parent (if not first item)
                if (i > 0)
                {
                    Rectangle connectorRect = new Rectangle(
                        rowX - 20,
                        currentY - RowSpacing / 2,
                        60,
                        24
                    );
                    connectorRects.Add(connectorRect);
                }

                currentY += RowHeight + RowSpacing;

                // Check if this criterion has nested group (placeholder for future)
                // This would require FilterCriteria to support child criteria
                // For now, we just show a simple list
            }

            return currentY;
        }

        /// <summary>Paints the filter UI elements in tree structure.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (g == null || owner == null) return;

            var colors = GetStyleColors(owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Paint tree connectors first (behind content)
            PaintTreeConnectors(g, layout, colors);

            // Paint filter rows
            if (config != null && config.Criteria.Count > 0)
            {
                for (int i = 0; i < layout.RowRects.Length && i < config.Criteria.Count; i++)
                {
                    PaintQueryBuilderRow(g, layout.RowRects[i], config.Criteria[i], colors, owner);
                }

                // Paint drag handles
                if (owner.EnableDragDrop && layout.DragHandleRects != null)
                {
                    foreach (var dragRect in layout.DragHandleRects)
                    {
                        PaintDragHandle(g, dragRect, colors.border);
                    }
                }

                // Paint AND/OR connectors
                if (layout.ConnectorRects != null && layout.ConnectorRects.Length > 1)
                {
                    var logicalOp = config.Logic;
                    for (int i = 1; i < layout.ConnectorRects.Length; i++) // Skip first (group header)
                    {
                        PaintLogicConnector(g, layout.ConnectorRects[i], logicalOp, colors);
                    }
                }
            }

            // Paint action buttons
            if (owner.ShowActionButtons)
            {
                if (layout.AddFilterButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddFilterButtonRect, "+ Add Condition", colors);
                }

                if (layout.AddGroupButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddGroupButtonRect, "+ Add Group", colors);
                }
            }

            // Phase 1: Paint filter count badge (top-right corner)
            if (owner.ShowFilterCountBadge && config != null && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - 40,
                    layout.ContainerRect.Top + 8
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor);
            }
        }

        private void PaintTreeConnectors(Graphics g, FilterLayoutInfo layout, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            // Draw vertical and horizontal connector lines for tree structure
            if (layout.RowRects == null || layout.RowRects.Length < 2) return;

            using (var pen = new Pen(Color.FromArgb(100, colors.BorderColor), ConnectorLineWidth))
            {
                pen.DashStyle = DashStyle.Dot;

                // Simple vertical line connecting rows
                for (int i = 0; i < layout.RowRects.Length - 1; i++)
                {
                    var rect1 = layout.RowRects[i];
                    var rect2 = layout.RowRects[i + 1];

                    int lineX = rect1.Left - 10;
                    g.DrawLine(pen, lineX, rect1.Bottom, lineX, rect2.Top);
                }
            }
        }

        private void PaintQueryBuilderRow(
            Graphics g,
            Rectangle rowRect,
            FilterCriteria criterion,
            (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors,
            BeepFilter owner)
        {
            if (rowRect.Width <= 0 || rowRect.Height <= 0) return;

            int currentX = rowRect.X;
            int centerY = rowRect.Y + rowRect.Height / 2;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(250, colors.BackColor)))
            {
                using (var path = CreateRoundedRectanglePath(rowRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                using (var path = CreateRoundedRectanglePath(rowRect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Column dropdown
            Rectangle columnRect = new Rectangle(currentX + 8, rowRect.Y + 8, ColumnWidth, rowRect.Height - 16);
            PaintDropdown(g, columnRect, criterion.ColumnName ?? "Select field...", colors);
            currentX += ColumnWidth + ItemSpacing;

            // Operator dropdown
            Rectangle operatorRect = new Rectangle(currentX, rowRect.Y + 8, OperatorWidth, rowRect.Height - 16);
            PaintDropdown(g, operatorRect, criterion.Operator.ToString(), colors);
            currentX += OperatorWidth + ItemSpacing;

            // Value input/dropdown
            Rectangle valueRect = new Rectangle(currentX, rowRect.Y + 8, ValueWidth, rowRect.Height - 16);
            PaintValueInput(g, valueRect, criterion.Value?.ToString() ?? "", colors);

            // Remove button (X) on the right
            int removeButtonSize = 20;
            Rectangle removeRect = new Rectangle(
                rowRect.Right - removeButtonSize - 8,
                centerY - removeButtonSize / 2,
                removeButtonSize,
                removeButtonSize
            );
            PaintRemoveButton(g, removeRect, colors);
        }

        private void PaintDropdown(Graphics g, Rectangle rect, string text, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(colors.BackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                g.DrawRectangle(pen, rect);
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                TextRenderer.DrawText(g, text, font, rect, colors.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Dropdown arrow
            int arrowSize = 6;
            int arrowX = rect.Right - arrowSize - 6;
            int arrowY = rect.Y + rect.Height / 2;
            
            using (var pen = new Pen(colors.ForeColor, 1.5f))
            {
                g.DrawLine(pen, arrowX - arrowSize / 2, arrowY - 2, arrowX, arrowY + 2);
                g.DrawLine(pen, arrowX, arrowY + 2, arrowX + arrowSize / 2, arrowY - 2);
            }
        }

        private void PaintValueInput(Graphics g, Rectangle rect, string text, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(colors.BackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                g.DrawRectangle(pen, rect);
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                string displayText = string.IsNullOrEmpty(text) ? "Enter value..." : text;
                Color textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(150, colors.ForeColor) : colors.ForeColor;
                
                TextRenderer.DrawText(g, displayText, font, rect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintLogicConnector(Graphics g, Rectangle rect, FilterLogic logicalOp, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            string text = logicalOp == FilterLogic.And ? "AND" : "OR";
            Color bgColor = logicalOp == FilterLogic.And 
                ? Color.FromArgb(33, 150, 243)  // Blue for AND
                : Color.FromArgb(156, 39, 176); // Purple for OR

            // Background pill
            using (var brush = new SolidBrush(bgColor))
            {
                using (var path = CreateRoundedRectanglePath(rect, 12))
                {
                    g.FillPath(brush, path);
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, text, font, rect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Circle background
            using (var brush = new SolidBrush(Color.FromArgb(244, 67, 54))) // Red
            {
                g.FillEllipse(brush, rect);
            }

            // X mark
            using (var pen = new Pen(Color.White, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int padding = 5;
                g.DrawLine(pen, 
                    rect.Left + padding, rect.Top + padding,
                    rect.Right - padding, rect.Bottom - padding);
                g.DrawLine(pen,
                    rect.Right - padding, rect.Top + padding,
                    rect.Left + padding, rect.Bottom - padding);
            }
        }

        private void PaintActionButton(Graphics g, Rectangle rect, string text, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(240, colors.BackColor)))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.AccentColor, 1f))
            {
                pen.DashStyle = DashStyle.Dash;
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                TextRenderer.DrawText(g, text, font, rect, colors.AccentColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>Hit tests for query builder rows, dropdowns, remove buttons, connectors, and drag handles.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check drag handles first
            for (int i = 0; i < layout.DragHandleRects.Length; i++)
            {
                if (layout.DragHandleRects[i].Contains(point))
                    return new FilterHitArea { Name = $"DragHandle_{i}", Bounds = layout.DragHandleRects[i], Type = FilterHitAreaType.DragHandle, Tag = i };
            }

            // Check logic connectors (AND/OR buttons)
            if (layout.ConnectorRects != null && layout.ConnectorRects.Length > 1)
            {
                for (int i = 1; i < layout.ConnectorRects.Length; i++) // Skip first (group header)
                {
                    if (layout.ConnectorRects[i].Contains(point))
                        return new FilterHitArea { Name = $"Connector_{i}", Bounds = layout.ConnectorRects[i], Type = FilterHitAreaType.LogicConnector, Tag = i };
                }
            }

            // Check rows and their components
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                Rectangle rowRect = layout.RowRects[i];
                
                // Remove button (right side of row)
                Rectangle removeRect = new Rectangle(rowRect.Right - 28, rowRect.Y + (rowRect.Height - 20) / 2, 20, 20);
                if (removeRect.Contains(point))
                    return new FilterHitArea { Name = $"Remove_{i}", Bounds = removeRect, Type = FilterHitAreaType.RemoveButton, Tag = i };

                // Calculate dropdown positions within row
                int currentX = rowRect.X + 8;

                // Column dropdown
                Rectangle columnRect = new Rectangle(currentX, rowRect.Y + 8, ColumnWidth, rowRect.Height - 16);
                if (columnRect.Contains(point))
                    return new FilterHitArea { Name = $"Field_{i}", Bounds = columnRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
                currentX += ColumnWidth + ItemSpacing;

                // Operator dropdown
                Rectangle operatorRect = new Rectangle(currentX, rowRect.Y + 8, OperatorWidth, rowRect.Height - 16);
                if (operatorRect.Contains(point))
                    return new FilterHitArea { Name = $"Operator_{i}", Bounds = operatorRect, Type = FilterHitAreaType.OperatorDropdown, Tag = i };
                currentX += OperatorWidth + ItemSpacing;

                // Value input
                Rectangle valueRect = new Rectangle(currentX, rowRect.Y + 8, ValueWidth, rowRect.Height - 16);
                if (valueRect.Contains(point))
                    return new FilterHitArea { Name = $"Value_{i}", Bounds = valueRect, Type = FilterHitAreaType.ValueInput, Tag = i };

                // Entire row as fallback
                if (rowRect.Contains(point))
                    return new FilterHitArea { Name = $"Row_{i}", Bounds = rowRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
            }

            // Check action buttons
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddCondition", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            if (layout.AddGroupButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddGroup", Bounds = layout.AddGroupButtonRect, Type = FilterHitAreaType.AddGroupButton };

            return null;
        }
    }
}
