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
                    150,
                    RowHeight
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
                        availableRect.X + padding + DragHandleWidth,
                        currentY,
                        ConnectorWidth,
                        ConnectorHeight
                    );
                    connectorRects.Add(connectorRect);
                    currentY += ConnectorHeight + 4;
                }

                // Filter row
                var rowRect = new Rectangle(
                    availableRect.X + padding,
                    currentY,
                    rowWidth,
                    RowHeight
                );
                rowRects.Add(rowRect);

                // Drag handle
                if (owner.EnableDragDrop)
                {
                    var dragRect = new Rectangle(
                        rowRect.X,
                        rowRect.Y + (rowRect.Height - 16) / 2,
                        DragHandleWidth,
                        16
                    );
                    dragHandleRects.Add(dragRect);
                }

                currentY += RowHeight + RowSpacing;
            }

            layout.RowRects = rowRects.ToArray();
            layout.ConnectorRects = connectorRects.ToArray();
            layout.DragHandleRects = dragHandleRects.ToArray();

            // Add Filter button
            if (owner.ShowActionButtons)
            {
                layout.AddFilterButtonRect = new Rectangle(
                    availableRect.X + padding + DragHandleWidth,
                    currentY,
                    150,
                    RowHeight
                );

                // Add Group button (next to Add Filter)
                layout.AddGroupButtonRect = new Rectangle(
                    layout.AddFilterButtonRect.Right + 8,
                    currentY,
                    150,
                    RowHeight
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
                    var colors = GetStyleColors(owner.ControlStyle);
                    PaintDragHandle(g, layout.DragHandleRects[i], Color.FromArgb(150, colors.text));
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
        }

        private void PaintFilterRow(Graphics g, Rectangle rect, FilterCriteria criterion, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);
            int dragOffset = owner.EnableDragDrop ? DragHandleWidth + 8 : 8;

            // Calculate areas within row
            int availableWidth = rect.Width - dragOffset - 40; // 40 for remove button
            int columnWidth = availableWidth / 3;
            int operatorWidth = availableWidth / 3;
            int valueWidth = availableWidth / 3;

            var columnRect = new Rectangle(rect.X + dragOffset, rect.Y + 4, columnWidth - 4, rect.Height - 8);
            var operatorRect = new Rectangle(columnRect.Right + 4, rect.Y + 4, operatorWidth - 4, rect.Height - 8);
            var valueRect = new Rectangle(operatorRect.Right + 4, rect.Y + 4, valueWidth - 4, rect.Height - 8);
            var removeRect = new Rectangle(rect.Right - 36, rect.Y + (rect.Height - 28) / 2, 28, 28);

            // Background
            using (var brush = new SolidBrush(colors.background))
            {
                g.FillRectangle(brush, rect);
            }

            // Dropdowns
            PaintDropdownField(g, columnRect, criterion.ColumnName, owner);
            PaintDropdownField(g, operatorRect, criterion.Operator.GetDisplayName(), owner);
            PaintValueField(g, valueRect, criterion.Value?.ToString() ?? "", owner);

            // Remove button
            PaintRemoveButton(g, removeRect, owner);
        }

        private void PaintDropdownField(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.DrawPath(pen, path);
            }

            // Text
            var textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - 24, rect.Height);
            using (var font = new Font("Segoe UI", 9f))
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

            // Dropdown arrow
            int arrowX = rect.Right - 16;
            int arrowY = rect.Y + rect.Height / 2;
            using (var pen = new Pen(colors.text, 1.5f))
            {
                g.DrawLine(pen, arrowX - 4, arrowY - 2, arrowX, arrowY + 2);
                g.DrawLine(pen, arrowX, arrowY + 2, arrowX + 4, arrowY - 2);
            }
        }

        private void PaintValueField(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.DrawPath(pen, path);
            }

            // Text or placeholder
            var displayText = string.IsNullOrEmpty(text) ? "Enter value..." : text;
            var textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(150, colors.text) : colors.text;

            var textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - 16, rect.Height);
            using (var font = new Font("Segoe UI", 9f))
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

        private void PaintLogicConnectorButton(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background (rounded pill)
            using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
            using (var path = CreateRoundedRectanglePath(rect, rect.Height / 2))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, rect.Height / 2))
            {
                g.DrawPath(pen, path);
            }

            // Text
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brush = new SolidBrush(colors.text))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                g.DrawString(text, font, brush, rect, sf);
            }
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Circle background
            using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
            {
                g.FillEllipse(brush, rect);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            {
                g.DrawEllipse(pen, rect);
            }

            // X
            int xSize = 8;
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
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(248, 248, 248)))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.FillPath(brush, path);
            }

            // Border (dashed)
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawPath(pen, path);
            }

            // Text
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(colors.accent))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                g.DrawString(text, font, brush, rect, sf);
            }
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
