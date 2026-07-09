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

        /// <summary>
        /// Sets current dialog tab by zero-based index (0=Basic, 1=Advanced, 2=Saved).
        /// </summary>
        public void SetCurrentTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex > 2)
            {
                return;
            }

            _currentTab = (DialogTab)tabIndex;
        }

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.AdvancedDialog;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => true;

        /// <summary>Calculates layout positions for dialog sections and controls.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            int sTabHeight = DpiScalingHelper.ScaleValue(TabHeight, owner);
            int sPadding = DpiScalingHelper.ScaleValue(Padding, owner);
            int sRowHeight = DpiScalingHelper.ScaleValue(RowHeight, owner);
            int sRowSpacing = DpiScalingHelper.ScaleValue(RowSpacing, owner);
            int sButtonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, owner);
            int sPreviewHeight = DpiScalingHelper.ScaleValue(PreviewHeight, owner);
            int s24 = DpiScalingHelper.ScaleValue(24, owner);
            int s20 = DpiScalingHelper.ScaleValue(20, owner);
            int s16 = DpiScalingHelper.ScaleValue(16, owner);

            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = new Rectangle(
                    availableRect.X,
                    availableRect.Y + sTabHeight,
                    availableRect.Width,
                    availableRect.Height - sTabHeight
                )
            };

            layout.DpiScale = Helpers.DpiScalingHelper.GetDpiScaleFactor(owner);

            var config = owner.ActiveFilter;
            int currentY = layout.ContentRect.Y + sPadding;
            int contentWidth = layout.ContentRect.Width - sPadding * 2;

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
                            availableRect.X + sPadding,
                            currentY + (sRowHeight - s16) / 2,
                            s20,
                            s16
                        ));
                    }

                    // Filter row
                    Rectangle rowRect = new Rectangle(
                        availableRect.X + sPadding + (owner.EnableDragDrop ? s24 : 0),
                        currentY,
                        contentWidth - (owner.EnableDragDrop ? s24 : 0),
                        sRowHeight
                    );
                    rowRects.Add(rowRect);
                    currentY += sRowHeight + sRowSpacing;
                }

                layout.RowRects = rowRects.ToArray();
                layout.DragHandleRects = dragHandleRects.ToArray();
            }

            // Add filter button
            currentY += sRowSpacing;
            layout.AddFilterButtonRect = new Rectangle(
                availableRect.X + sPadding,
                currentY,
                DpiScalingHelper.ScaleValue(140, owner),
                DpiScalingHelper.ScaleValue(32, owner)
            );

            // Preview section at bottom
            int previewY = availableRect.Bottom - sPreviewHeight - sButtonHeight - sPadding * 3;
            layout.AddGroupButtonRect = new Rectangle(
                availableRect.X + sPadding,
                previewY,
                contentWidth,
                sPreviewHeight
            );

            return layout;
        }

        /// <summary>Paints the advanced dialog filter UI.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (g == null || owner == null) return;

            var colors = GetStyleColors(owner, owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Dialog background
            g.FillRectangle(GetBrush(colors.background), layout.ContainerRect);

            // Dialog border
            g.DrawRectangle(GetPen(colors.border, DpiScalingHelper.ScaleValue(2, owner)), layout.ContainerRect);

            // Paint tab headers
            PaintTabHeaders(g, layout.ContainerRect, colors, owner);

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
                    PaintSavedTab(g, layout, colors, owner);
                    break;
            }

            // Paint preview section
            if (layout.AddGroupButtonRect != Rectangle.Empty)
            {
                PaintPreviewSection(g, layout.AddGroupButtonRect, config, colors, owner);
            }

            // Phase 1: Paint filter count badge (top-right corner of dialog)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - DpiScalingHelper.ScaleValue(50, owner),
                    layout.ContainerRect.Top + DpiScalingHelper.ScaleValue(12, owner)
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor, owner);
            }

            // Paint action buttons at bottom
            PaintDialogButtons(g, layout.ContainerRect, colors, owner);
        }

        private void PaintTabHeaders(Graphics g, Rectangle containerRect, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            Rectangle tabArea = new Rectangle(containerRect.X, containerRect.Y, containerRect.Width, DpiScalingHelper.ScaleValue(TabHeight, owner));

            // Tab background
            g.FillRectangle(GetBrush(Color.FromArgb(245, colors.background)), tabArea);

            // Tab buttons
            string[] tabNames = { "Basic", "Advanced", "Saved Filters" };
            int currentX = containerRect.X + DpiScalingHelper.ScaleValue(Padding, owner);

            for (int i = 0; i < tabNames.Length; i++)
            {
                DialogTab tab = (DialogTab)i;
                bool isActive = tab == _currentTab;

                int sTabButtonWidth = DpiScalingHelper.ScaleValue(TabButtonWidth, owner);
                int scaled8 = DpiScalingHelper.ScaleValue(8, owner);
                Rectangle tabRect = new Rectangle(currentX, containerRect.Y + scaled8, sTabButtonWidth, DpiScalingHelper.ScaleValue(TabHeight, owner) - scaled8);
                PaintTab(g, tabRect, tabNames[i], isActive, colors, owner);
                currentX += sTabButtonWidth + DpiScalingHelper.ScaleValue(4, owner);
            }

            // Bottom border
            g.DrawLine(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), containerRect.X, containerRect.Y + DpiScalingHelper.ScaleValue(TabHeight, owner), containerRect.Right, containerRect.Y + DpiScalingHelper.ScaleValue(TabHeight, owner));
        }

        private void PaintTab(Graphics g, Rectangle rect, string text, bool isActive, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isActive ? colors.background : Color.FromArgb(230, colors.background);
            Color textColor = isActive ? colors.accent : colors.text;

            // Background
            int scaled4 = DpiScalingHelper.ScaleValue(4, owner);
            int scaled8 = DpiScalingHelper.ScaleValue(8, owner);
            using (var path = CreateRoundedRectanglePath(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + scaled8), scaled4))
            {
                g.FillPath(GetBrush(bgColor), path);
            }

            // Active tab indicator
            if (isActive)
            {
                g.DrawLine(GetPen(colors.accent, DpiScalingHelper.ScaleValue(3, owner)), rect.X, rect.Bottom, rect.Right, rect.Bottom);
            }

            // Text
            var font = GetFont(9f, isActive ? FontStyle.Bold : FontStyle.Regular);
            TextRenderer.DrawText(g, text, font, rect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                    PaintFilterRow(g, layout.RowRects[i], config.Criteria[i], colors, owner);
                }

                // Paint drag handles
                if (owner.EnableDragDrop && layout.DragHandleRects != null)
                {
                    foreach (var dragRect in layout.DragHandleRects)
                    {
                        PaintDragHandle(g, dragRect, colors.border, owner);
                    }
                }
            }

            // Paint add button
            if (layout.AddFilterButtonRect != Rectangle.Empty)
            {
                PaintActionButton(g, layout.AddFilterButtonRect, "+ Add Condition", colors, false, owner);
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

        private void PaintSavedTab(Graphics g, FilterLayoutInfo layout, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            int sPadding = DpiScalingHelper.ScaleValue(Padding, owner);
            int currentY = layout.ContentRect.Y + sPadding;
            int contentWidth = layout.ContentRect.Width - sPadding * 2;

            // Saved filters list placeholder
            var font = GetFont(9f, FontStyle.Italic);
            string message = "No saved filters yet.\nClick 'Save Current' to save this filter configuration.";
            Rectangle messageRect = new Rectangle(
                layout.ContentRect.X + sPadding,
                currentY + DpiScalingHelper.ScaleValue(40, owner),
                contentWidth,
                DpiScalingHelper.ScaleValue(100, owner)
            );
            TextRenderer.DrawText(g, message, font, messageRect, Color.FromArgb(150, colors.text),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);

            // Save current button
            Rectangle saveButtonRect = new Rectangle(
                layout.ContentRect.X + sPadding,
                currentY,
                DpiScalingHelper.ScaleValue(150, owner),
                DpiScalingHelper.ScaleValue(ButtonHeight, owner)
            );
            PaintActionButton(g, saveButtonRect, "Save Current", colors, false, owner);
        }

        private void PaintFilterRow(
            Graphics g,
            Rectangle rowRect,
            FilterCriteria criterion,
            (Color background, Color border, Color text, Color accent) colors,
            Control? owner = null)
        {
            if (rowRect.Width <= 0 || rowRect.Height <= 0) return;

            int sColW = DpiScalingHelper.ScaleValue(ColumnWidth, owner);
            int sOpW = DpiScalingHelper.ScaleValue(OperatorWidth, owner);
            int sValW = DpiScalingHelper.ScaleValue(ValueWidth, owner);
            int sItemSp = DpiScalingHelper.ScaleValue(ItemSpacing, owner);
            int s24 = DpiScalingHelper.ScaleValue(24, owner);

            int currentX = rowRect.X;

            // Column dropdown
            Rectangle columnRect = new Rectangle(currentX, rowRect.Y, sColW, rowRect.Height);
            PaintDropdown(g, columnRect, criterion.ColumnName ?? "Select column...", colors, owner);
            currentX += sColW + sItemSp;

            // Operator dropdown
            Rectangle operatorRect = new Rectangle(currentX, rowRect.Y, sOpW, rowRect.Height);
            PaintDropdown(g, operatorRect, criterion.Operator.ToString(), colors, owner);
            currentX += sOpW + sItemSp;

            // Value input
            Rectangle valueRect = new Rectangle(currentX, rowRect.Y, sValW, rowRect.Height);
            PaintValueInput(g, valueRect, criterion.Value?.ToString() ?? "", colors, owner);
            currentX += sValW + sItemSp;

            // Remove button
            Rectangle removeRect = new Rectangle(currentX, rowRect.Y + (rowRect.Height - s24) / 2, s24, s24);
            PaintRemoveButton(g, removeRect, colors, owner);
        }

        private void PaintDropdown(Graphics g, Rectangle rect, string text, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int s4 = DpiScalingHelper.ScaleValue(4, owner);
            int s8 = DpiScalingHelper.ScaleValue(8, owner);
            int s24 = DpiScalingHelper.ScaleValue(24, owner);
            int sArrow = DpiScalingHelper.ScaleValue(6, owner);

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
            var font = GetFont(8.5f);
            Rectangle textRect = new Rectangle(rect.X + s8, rect.Y, rect.Width - s24, rect.Height);
            TextRenderer.DrawText(g, text, font, textRect, colors.text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Dropdown arrow
            int arrowX = rect.Right - sArrow - s8;
            int arrowY = rect.Y + rect.Height / 2;

            var arrowPen = GetPen(colors.text, 1.5f);
            g.DrawLine(arrowPen, arrowX - sArrow / 2, arrowY - 2, arrowX, arrowY + 2);
            g.DrawLine(arrowPen, arrowX, arrowY + 2, arrowX + sArrow / 2, arrowY - 2);
        }

        private void PaintValueInput(Graphics g, Rectangle rect, string text, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

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

            // Text
            var font = GetFont(8.5f);
            string displayText = string.IsNullOrEmpty(text) ? "Enter value..." : text;
            Color textColor = string.IsNullOrEmpty(text) ? Color.FromArgb(150, colors.text) : colors.text;

            Rectangle textRect = new Rectangle(rect.X + s8, rect.Y, rect.Width - s16, rect.Height);
            TextRenderer.DrawText(g, displayText, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        private void PaintRemoveButton(Graphics g, Rectangle rect, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Circle background
            g.FillEllipse(GetBrush(Color.FromArgb(244, 67, 54)), rect); // Red

            // X mark
            using (var pen = new Pen(Color.White, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int padding = DpiScalingHelper.ScaleValue(6, owner);
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
            (Color background, Color border, Color text, Color accent) colors,
            Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int s4 = DpiScalingHelper.ScaleValue(4, owner);
            int s8 = DpiScalingHelper.ScaleValue(8, owner);
            int s12 = DpiScalingHelper.ScaleValue(12, owner);
            int s20 = DpiScalingHelper.ScaleValue(20, owner);
            int s24 = DpiScalingHelper.ScaleValue(24, owner);
            int s30 = DpiScalingHelper.ScaleValue(30, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.FillPath(GetBrush(Color.FromArgb(250, colors.background)), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Title
            var titleFont = GetFont(8f, FontStyle.Bold);
            Rectangle titleRect = new Rectangle(rect.X + s12, rect.Y + s8, rect.Width - s24, s20);
            TextRenderer.DrawText(g, "Filter Preview", titleFont, titleRect, colors.text,
                TextFormatFlags.Left | TextFormatFlags.Top);

            // Filter count and expression preview
            int filterCount = config?.EnabledFilterCount ?? 0;
            string previewText = filterCount > 0
                ? $"{filterCount} filter{(filterCount != 1 ? "s" : "")} active"
                : "No filters applied";

            var previewFont = GetFont(8.5f);
            Rectangle previewRect = new Rectangle(rect.X + s12, rect.Y + s30, rect.Width - s24, s24);
            TextRenderer.DrawText(g, previewText, previewFont, previewRect, Color.FromArgb(150, colors.text),
                TextFormatFlags.Left | TextFormatFlags.Top);
        }

        private void PaintDialogButtons(Graphics g, Rectangle containerRect, (Color background, Color border, Color text, Color accent) colors, Control? owner = null)
        {
            int sButtonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, owner);
            int sPadding = DpiScalingHelper.ScaleValue(Padding, owner);
            int buttonY = containerRect.Bottom - sButtonHeight - sPadding;
            int buttonWidth = DpiScalingHelper.ScaleValue(100, owner);
            int buttonSpacing = DpiScalingHelper.ScaleValue(8, owner);

            // OK button
            Rectangle okRect = new Rectangle(
                containerRect.Right - buttonWidth * 3 - buttonSpacing * 2 - sPadding,
                buttonY,
                buttonWidth,
                sButtonHeight
            );
            PaintActionButton(g, okRect, "OK", colors, true, owner);

            // Apply button
            Rectangle applyRect = new Rectangle(
                okRect.Right + buttonSpacing,
                buttonY,
                buttonWidth,
                sButtonHeight
            );
            PaintActionButton(g, applyRect, "Apply", colors, false, owner);

            // Cancel button
            Rectangle cancelRect = new Rectangle(
                applyRect.Right + buttonSpacing,
                buttonY,
                buttonWidth,
                sButtonHeight
            );
            PaintActionButton(g, cancelRect, "Cancel", colors, false, owner);
        }

        private void PaintActionButton(
            Graphics g,
            Rectangle rect,
            string text,
            (Color background, Color border, Color text, Color accent) colors,
            bool isPrimary,
            Control? owner = null)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isPrimary ? colors.accent : Color.FromArgb(240, colors.background);
            Color textColor = isPrimary ? Color.White : colors.text;

            int s4 = DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, s4))
            {
                g.FillPath(GetBrush(bgColor), path);
            }

            // Border
            if (!isPrimary)
            {
                using (var path = CreateRoundedRectanglePath(rect, s4))
                {
                    g.DrawPath(GetPen(colors.border, DpiScalingHelper.ScaleValue(1, owner)), path);
                }
            }

            // Text
            var font = GetFont(9f, FontStyle.Regular);
            TextRenderer.DrawText(g, text, font, rect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>Hit tests for tabs, filter rows, dropdowns, buttons, and dialog controls.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check tab headers
            Rectangle tabArea = new Rectangle(layout.ContainerRect.X, layout.ContainerRect.Y, layout.ContainerRect.Width, Helpers.DpiScalingHelper.ScaleValue(TabHeight, layout.DpiScale));
            if (tabArea.Contains(point))
            {
                int currentX = layout.ContainerRect.X + Helpers.DpiScalingHelper.ScaleValue(Padding, layout.DpiScale);
                string[] tabNames = { "Basic", "Advanced", "Saved Filters" };
                
                for (int i = 0; i < tabNames.Length; i++)
                {
                    Rectangle tabRect = new Rectangle(currentX, layout.ContainerRect.Y + 8, TabButtonWidth, Helpers.DpiScalingHelper.ScaleValue(TabHeight, layout.DpiScale) - 8);
                    if (tabRect.Contains(point))
                        return new FilterHitArea { Name = $"Tab_{tabNames[i]}", Bounds = tabRect, Type = FilterHitAreaType.CollapseButton, Tag = i };
                    currentX += TabButtonWidth + 4;
                }
            }

            // Check dialog action buttons at bottom
            int buttonY = layout.ContainerRect.Bottom - ButtonHeight - Helpers.DpiScalingHelper.ScaleValue(Padding, layout.DpiScale);
            int buttonWidth = 100;
            int buttonSpacing = 8;

            Rectangle okRect = new Rectangle(
                layout.ContainerRect.Right - buttonWidth * 3 - buttonSpacing * 2 - Helpers.DpiScalingHelper.ScaleValue(Padding, layout.DpiScale),
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
