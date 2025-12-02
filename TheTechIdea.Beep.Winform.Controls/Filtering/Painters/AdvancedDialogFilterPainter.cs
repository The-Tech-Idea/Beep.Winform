using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for AdvancedDialog filter style - modal or slide-in panel with rich features
    /// Features: tabbed sections, save/load configurations, preview results, full-featured editor
    /// Suitable for complex filtering scenarios requiring saved filter sets
    /// </summary>
    public class AdvancedDialogFilterPainter : BaseFilterPainter
    {
        private const int TabHeight = 40;
        private const int RowHeight = 36;
        private const int Padding = 16;
        private const int RowSpacing = 8;
        private const int ColumnWidth = 150;
        private const int OperatorWidth = 120;
        private const int ValueWidth = 150;
        private const int ItemSpacing = 8;
        private const int ButtonHeight = 36;
        private const int PreviewHeight = 60;
        private const int TabButtonWidth = 120;

        private enum DialogTab
        {
            Basic,
            Advanced,
            Saved
        }

        private DialogTab _currentTab = DialogTab.Basic;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.AdvancedDialog;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => true;

        /// <summary>Calculates layout positions for dialog sections and controls.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = new Rectangle(
                    availableRect.X,
                    availableRect.Y + TabHeight,
                    availableRect.Width,
                    availableRect.Height - TabHeight
                )
            };

            var config = owner.ActiveFilter;
            int currentY = layout.ContentRect.Y + Padding;
            int contentWidth = layout.ContentRect.Width - Padding * 2;

            var rowRects = new List<Rectangle>();
            var dragHandleRects = new List<Rectangle>();

            // Filter rows
            if (config != null && config.Criteria.Count > 0)
            {
                foreach (var criterion in config.Criteria)
                {
                    // Drag handle
                    if (owner.EnableDragDrop)
                    {
                        dragHandleRects.Add(new Rectangle(
                            availableRect.X + Padding,
                            currentY + (RowHeight - 16) / 2,
                            20,
                            16
                        ));
                    }

                    // Filter row
                    Rectangle rowRect = new Rectangle(
                        availableRect.X + Padding + (owner.EnableDragDrop ? 24 : 0),
                        currentY,
                        contentWidth - (owner.EnableDragDrop ? 24 : 0),
                        RowHeight
                    );
                    rowRects.Add(rowRect);
                    currentY += RowHeight + RowSpacing;
                }

                layout.RowRects = rowRects.ToArray();
                layout.DragHandleRects = dragHandleRects.ToArray();
            }

            // Add filter button
            currentY += RowSpacing;
            layout.AddFilterButtonRect = new Rectangle(
                availableRect.X + Padding,
                currentY,
                140,
                32
            );

            // Preview section at bottom
            int previewY = availableRect.Bottom - PreviewHeight - ButtonHeight - Padding * 3;
            layout.AddGroupButtonRect = new Rectangle(
                availableRect.X + Padding,
                previewY,
                contentWidth,
                PreviewHeight
            );

            return layout;
        }

        /// <summary>Paints the advanced dialog filter UI.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (g == null || owner == null) return;

            var colors = GetStyleColors(owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Dialog background
            using (var brush = new SolidBrush(colors.background))
            {
                g.FillRectangle(brush, layout.ContainerRect);
            }

            // Dialog border
            using (var pen = new Pen(colors.border, 2f))
            {
                g.DrawRectangle(pen, layout.ContainerRect);
            }

            // Paint tab headers
            PaintTabHeaders(g, layout.ContainerRect, colors);

            // Paint tab content based on current tab
            switch (_currentTab)
            {
                case DialogTab.Basic:
                    PaintBasicTab(g, owner, layout, config, colors);
                    break;
                case DialogTab.Advanced:
                    PaintAdvancedTab(g, owner, layout, config, colors);
                    break;
                case DialogTab.Saved:
                    PaintSavedTab(g, layout, colors);
                    break;
            }

            // Paint preview section
            if (layout.AddGroupButtonRect != Rectangle.Empty)
            {
                PaintPreviewSection(g, layout.AddGroupButtonRect, config, colors);
            }

            // Phase 1: Paint filter count badge (top-right corner of dialog)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - 50,
                    layout.ContainerRect.Top + 12
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor);
            }

            // Paint action buttons at bottom
            PaintDialogButtons(g, layout.ContainerRect, colors);
        }

        private void PaintTabHeaders(Graphics g, Rectangle containerRect, (Color background, Color border, Color text, Color accent) colors)
        {
            Rectangle tabArea = new Rectangle(containerRect.X, containerRect.Y, containerRect.Width, TabHeight);

            // Tab background
            using (var brush = new SolidBrush(Color.FromArgb(245, colors.background)))
            {
                g.FillRectangle(brush, tabArea);
            }

            // Tab buttons
            string[] tabNames = { "Basic", "Advanced", "Saved Filters" };
            int currentX = containerRect.X + Padding;

            for (int i = 0; i < tabNames.Length; i++)
            {
                DialogTab tab = (DialogTab)i;
                bool isActive = tab == _currentTab;

                Rectangle tabRect = new Rectangle(currentX, containerRect.Y + 8, TabButtonWidth, TabHeight - 8);
                PaintTab(g, tabRect, tabNames[i], isActive, colors);
                currentX += TabButtonWidth + 4;
            }

            // Bottom border
            using (var pen = new Pen(colors.border, 1f))
            {
                g.DrawLine(pen, containerRect.X, containerRect.Y + TabHeight, containerRect.Right, containerRect.Y + TabHeight);
            }
        }

        private void PaintTab(Graphics g, Rectangle rect, string text, bool isActive, (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isActive ? colors.background : Color.FromArgb(230, colors.background);
            Color textColor = isActive ? colors.accent : colors.text;

            // Background
            using (var brush = new SolidBrush(bgColor))
            {
                using (var path = CreateRoundedRectanglePath(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + 8), 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Active tab indicator
            if (isActive)
            {
                using (var pen = new Pen(colors.accent, 3f))
                {
                    g.DrawLine(pen, rect.X, rect.Bottom, rect.Right, rect.Bottom);
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 9f, isActive ? FontStyle.Bold : FontStyle.Regular))
            {
                TextRenderer.DrawText(g, text, font, rect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintBasicTab(
            Graphics g,
            BeepFilter owner,
            FilterLayoutInfo layout,
            FilterConfiguration config,
            (Color background, Color border, Color text, Color accent) colors)
        {
            // Paint filter rows
            if (config != null && config.Criteria.Count > 0)
            {
                for (int i = 0; i < layout.RowRects.Length && i < config.Criteria.Count; i++)
                {
                    PaintFilterRow(g, layout.RowRects[i], config.Criteria[i], colors);
                }

                // Paint drag handles
                if (owner.EnableDragDrop && layout.DragHandleRects != null)
                {
                    foreach (var dragRect in layout.DragHandleRects)
                    {
                        PaintDragHandle(g, dragRect, colors.border);
                    }
                }
            }

            // Paint add button
            if (layout.AddFilterButtonRect != Rectangle.Empty)
            {
                PaintActionButton(g, layout.AddFilterButtonRect, "+ Add Condition", colors, false);
            }
        }

        private void PaintAdvancedTab(
            Graphics g,
            BeepFilter owner,
            FilterLayoutInfo layout,
            FilterConfiguration config,
            (Color background, Color border, Color text, Color accent) colors)
        {
            // Advanced tab shows all filter rows plus additional options
            PaintBasicTab(g, owner, layout, config, colors);

            // Additional advanced options could be shown here
            // For example: case sensitivity toggle, wildcard options, regex support
        }

        private void PaintSavedTab(Graphics g, FilterLayoutInfo layout, (Color background, Color border, Color text, Color accent) colors)
        {
            int currentY = layout.ContentRect.Y + Padding;
            int contentWidth = layout.ContentRect.Width - Padding * 2;

            // Saved filters list placeholder
            using (var font = new Font("Segoe UI", 9f, FontStyle.Italic))
            {
                string message = "No saved filters yet.\nClick 'Save Current' to save this filter configuration.";
                Rectangle messageRect = new Rectangle(
                    layout.ContentRect.X + Padding,
                    currentY + 40,
                    contentWidth,
                    100
                );
                TextRenderer.DrawText(g, message, font, messageRect, Color.FromArgb(150, colors.text),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }

            // Save current button
            Rectangle saveButtonRect = new Rectangle(
                layout.ContentRect.X + Padding,
                currentY,
                150,
                ButtonHeight
            );
            PaintActionButton(g, saveButtonRect, "Save Current", colors, false);
        }

        private void PaintFilterRow(
            Graphics g,
            Rectangle rowRect,
            FilterCriteria criterion,
            (Color background, Color border, Color text, Color accent) colors)
        {
            if (rowRect.Width <= 0 || rowRect.Height <= 0) return;

            int currentX = rowRect.X;

            // Column dropdown
            Rectangle columnRect = new Rectangle(currentX, rowRect.Y, ColumnWidth, rowRect.Height);
            PaintDropdown(g, columnRect, criterion.ColumnName ?? "Select column...", colors);
            currentX += ColumnWidth + ItemSpacing;

            // Operator dropdown
            Rectangle operatorRect = new Rectangle(currentX, rowRect.Y, OperatorWidth, rowRect.Height);
            PaintDropdown(g, operatorRect, criterion.Operator.ToString(), colors);
            currentX += OperatorWidth + ItemSpacing;

            // Value input
            Rectangle valueRect = new Rectangle(currentX, rowRect.Y, ValueWidth, rowRect.Height);
            PaintValueInput(g, valueRect, criterion.Value?.ToString() ?? "", colors);
            currentX += ValueWidth + ItemSpacing;

            // Remove button
            Rectangle removeRect = new Rectangle(currentX, rowRect.Y + (rowRect.Height - 24) / 2, 24, 24);
            PaintRemoveButton(g, removeRect, colors);
        }

        private void PaintDropdown(Graphics g, Rectangle rect, string text, (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(colors.background))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.border, 1f))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                Rectangle textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - 24, rect.Height);
                TextRenderer.DrawText(g, text, font, textRect, colors.text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Dropdown arrow
            int arrowSize = 6;
            int arrowX = rect.Right - arrowSize - 8;
            int arrowY = rect.Y + rect.Height / 2;
            
            using (var pen = new Pen(colors.text, 1.5f))
            {
                g.DrawLine(pen, arrowX - arrowSize / 2, arrowY - 2, arrowX, arrowY + 2);
                g.DrawLine(pen, arrowX, arrowY + 2, arrowX + arrowSize / 2, arrowY - 2);
            }
        }

        private void PaintValueInput(Graphics g, Rectangle rect, string text, (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(colors.background))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.border, 1f))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                string displayText = string.IsNullOrEmpty(text) ? "Enter value..." : text;
                Color textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(150, colors.text) : colors.text;
                
                Rectangle textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - 16, rect.Height);
                TextRenderer.DrawText(g, displayText, font, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, (Color background, Color border, Color text, Color accent) colors)
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

                int padding = 6;
                g.DrawLine(pen, 
                    rect.Left + padding, rect.Top + padding,
                    rect.Right - padding, rect.Bottom - padding);
                g.DrawLine(pen,
                    rect.Right - padding, rect.Top + padding,
                    rect.Left + padding, rect.Bottom - padding);
            }
        }

        private void PaintPreviewSection(
            Graphics g,
            Rectangle rect,
            FilterConfiguration config,
            (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(250, colors.background)))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.border, 1f))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Title
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            {
                Rectangle titleRect = new Rectangle(rect.X + 12, rect.Y + 8, rect.Width - 24, 20);
                TextRenderer.DrawText(g, "Filter Preview", font, titleRect, colors.text,
                    TextFormatFlags.Left | TextFormatFlags.Top);
            }

            // Filter count and expression preview
            int filterCount = config?.EnabledFilterCount ?? 0;
            string previewText = filterCount > 0 
                ? $"{filterCount} filter{(filterCount != 1 ? "s" : "")} active"
                : "No filters applied";

            using (var font = new Font("Segoe UI", 8.5f))
            {
                Rectangle previewRect = new Rectangle(rect.X + 12, rect.Y + 30, rect.Width - 24, 24);
                TextRenderer.DrawText(g, previewText, font, previewRect, Color.FromArgb(150, colors.text),
                    TextFormatFlags.Left | TextFormatFlags.Top);
            }
        }

        private void PaintDialogButtons(Graphics g, Rectangle containerRect, (Color background, Color border, Color text, Color accent) colors)
        {
            int buttonY = containerRect.Bottom - ButtonHeight - Padding;
            int buttonWidth = 100;
            int buttonSpacing = 8;

            // OK button
            Rectangle okRect = new Rectangle(
                containerRect.Right - buttonWidth * 3 - buttonSpacing * 2 - Padding,
                buttonY,
                buttonWidth,
                ButtonHeight
            );
            PaintActionButton(g, okRect, "OK", colors, true);

            // Apply button
            Rectangle applyRect = new Rectangle(
                okRect.Right + buttonSpacing,
                buttonY,
                buttonWidth,
                ButtonHeight
            );
            PaintActionButton(g, applyRect, "Apply", colors, false);

            // Cancel button
            Rectangle cancelRect = new Rectangle(
                applyRect.Right + buttonSpacing,
                buttonY,
                buttonWidth,
                ButtonHeight
            );
            PaintActionButton(g, cancelRect, "Cancel", colors, false);
        }

        private void PaintActionButton(
            Graphics g,
            Rectangle rect,
            string text,
            (Color background, Color border, Color text, Color accent) colors,
            bool isPrimary)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isPrimary ? colors.accent : Color.FromArgb(240, colors.background);
            Color textColor = isPrimary ? Color.White : colors.text;

            // Background
            using (var brush = new SolidBrush(bgColor))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            if (!isPrimary)
            {
                using (var pen = new Pen(colors.border, 1f))
                {
                    using (var path = CreateRoundedRectanglePath(rect, 4))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, text, font, rect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>Hit tests for tabs, filter rows, dropdowns, buttons, and dialog controls.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check tab headers
            Rectangle tabArea = new Rectangle(layout.ContainerRect.X, layout.ContainerRect.Y, layout.ContainerRect.Width, TabHeight);
            if (tabArea.Contains(point))
            {
                int currentX = layout.ContainerRect.X + Padding;
                string[] tabNames = { "Basic", "Advanced", "Saved Filters" };
                
                for (int i = 0; i < tabNames.Length; i++)
                {
                    Rectangle tabRect = new Rectangle(currentX, layout.ContainerRect.Y + 8, TabButtonWidth, TabHeight - 8);
                    if (tabRect.Contains(point))
                        return new FilterHitArea { Name = $"Tab_{tabNames[i]}", Bounds = tabRect, Type = FilterHitAreaType.CollapseButton, Tag = i };
                    currentX += TabButtonWidth + 4;
                }
            }

            // Check dialog action buttons at bottom
            int buttonY = layout.ContainerRect.Bottom - ButtonHeight - Padding;
            int buttonWidth = 100;
            int buttonSpacing = 8;

            Rectangle okRect = new Rectangle(
                layout.ContainerRect.Right - buttonWidth * 3 - buttonSpacing * 2 - Padding,
                buttonY, buttonWidth, ButtonHeight
            );
            if (okRect.Contains(point))
                return new FilterHitArea { Name = "OK", Bounds = okRect, Type = FilterHitAreaType.ApplyButton };

            Rectangle applyRect = new Rectangle(okRect.Right + buttonSpacing, buttonY, buttonWidth, ButtonHeight);
            if (applyRect.Contains(point))
                return new FilterHitArea { Name = "Apply", Bounds = applyRect, Type = FilterHitAreaType.ApplyButton };

            Rectangle cancelRect = new Rectangle(applyRect.Right + buttonSpacing, buttonY, buttonWidth, ButtonHeight);
            if (cancelRect.Contains(point))
                return new FilterHitArea { Name = "Cancel", Bounds = cancelRect, Type = FilterHitAreaType.ClearAllButton };

            // Check drag handles
            for (int i = 0; i < layout.DragHandleRects.Length; i++)
            {
                if (layout.DragHandleRects[i].Contains(point))
                    return new FilterHitArea { Name = $"DragHandle_{i}", Bounds = layout.DragHandleRects[i], Type = FilterHitAreaType.DragHandle, Tag = i };
            }

            // Check filter rows and their components
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                Rectangle rowRect = layout.RowRects[i];
                
                // Remove button (right side)
                Rectangle removeRect = new Rectangle(rowRect.Right - 32, rowRect.Y + (rowRect.Height - 24) / 2, 24, 24);
                if (removeRect.Contains(point))
                    return new FilterHitArea { Name = $"Remove_{i}", Bounds = removeRect, Type = FilterHitAreaType.RemoveButton, Tag = i };

                // Calculate dropdown positions
                int currentX = rowRect.X;

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

                // Entire row as fallback
                if (rowRect.Contains(point))
                    return new FilterHitArea { Name = $"Row_{i}", Bounds = rowRect, Type = FilterHitAreaType.FieldDropdown, Tag = i };
            }

            // Check add filter button
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "AddFilter", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };

            // Check preview section
            if (layout.AddGroupButtonRect.Contains(point))
                return new FilterHitArea { Name = "PreviewSection", Bounds = layout.AddGroupButtonRect, Type = FilterHitAreaType.FilterTag };

            return null;
        }
    }
}
