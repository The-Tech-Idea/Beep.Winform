using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    public partial class BeepAccordionMenu
    {
        private int _contentHeight = 0;
        private Rectangle _scrollBarRect;
        private bool _showScrollBar = false;
        private int _scrollOffset = 0;

        // Override DrawContent to implement custom drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Use the DrawingRect for our actual drawing
            Draw(g, DrawingRect);
        }

        private int CalculateContentHeight()
        {
            int totalHeight = headerHeight + spacing;
            foreach (var item in items)
            {
                totalHeight += itemHeight + spacing;
                if (expandedState.ContainsKey(item) && expandedState[item] && !isCollapsed)
                {
                    totalHeight += item.Children.Count * (childItemHeight + spacing);
                }
            }
            return totalHeight;
        }

        private void UpdateScrollState()
        {
            if (!AutoScrollEnabled || isCollapsed)
            {
                _showScrollBar = false;
                _scrollOffset = 0;
                return;
            }

            _contentHeight = CalculateContentHeight();
            _showScrollBar = _contentHeight > Height;

            if (_showScrollBar)
            {
                int scrollBarWidth = 12;
                _scrollBarRect = new Rectangle(
                    rectangle.Width - scrollBarWidth - 2,
                    headerHeight,
                    scrollBarWidth,
                    Height - headerHeight - 4
                );
            }
            else
            {
                _scrollOffset = 0;
            }
        }

        public void ScrollTo(int yOffset)
        {
            if (!_showScrollBar) return;
            int maxOffset = _contentHeight - Height;
            _scrollOffset = Math.Max(0, Math.Min(yOffset, maxOffset));
            Invalidate();
        }

        public void ScrollBy(int delta)
        {
            ScrollTo(_scrollOffset + delta);
        }

        // Implementation of Draw method using painter system
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Enable anti-aliasing for smoother rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Create render options
            var renderOptions = new AccordionRenderOptions
            {
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                AccordionStyle = _accordionStyle,
                ItemHeight = itemHeight,
                ChildItemHeight = childItemHeight,
                HeaderHeight = headerHeight,
                IndentationWidth = indentationWidth,
                Spacing = spacing,
                Padding = Padding.Left,
                BorderRadius = AccordionStyleHelpers.GetRecommendedBorderRadius(_accordionStyle, ControlStyle),
                HighlightWidth = AccordionStyleHelpers.GetRecommendedHighlightWidth(_accordionStyle),
                IsCollapsed = isCollapsed,
                HeaderFont = AccordionFontHelpers.GetHeaderFont(ControlStyle),
                ItemFont = AccordionFontHelpers.GetItemFont(ControlStyle),
                ChildItemFont = AccordionFontHelpers.GetChildItemFont(ControlStyle)
            };

            // Paint background
            _painter.PaintAccordionBackground(graphics, rectangle, renderOptions);

            // Draw the title/header area
            Rectangle headerRect = new Rectangle(
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                headerHeight
            );

            _painter.PaintHeader(graphics, headerRect, Title, renderOptions);

            // Draw toggle button
            _toggleButtonRect = new Rectangle(
                headerRect.Right - 40,
                headerRect.Top + (headerRect.Height - 30) / 2,
                30,
                30);

            DrawHamburgerIcon(graphics, _toggleButtonRect);

            // Clear hit list before adding new areas
            ClearHitList();

            // Add hit area for the toggle button
            AddHitArea(
                "ToggleButton",
                _toggleButtonRect,
                null,
                () => {
                    isCollapsed = !isCollapsed;
                    StartAccordionAnimation();
                    ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
                }
            );

            // Get mouse position for hover effects
            Point mousePoint = this.PointToClient(Control.MousePosition);

            // Start drawing menu items below the header
            int yOffset = headerRect.Bottom + spacing;

            // Draw each root menu item (header) and its children if expanded
            foreach (var headerItem in items)
            {
                // If the header item was previously not in the expandedState, add it
                if (!expandedState.ContainsKey(headerItem))
                {
                    expandedState[headerItem] = false;
                }

                // Draw the header item
                yOffset = DrawHeaderItem(graphics, rectangle, headerItem, yOffset, mousePoint, renderOptions);

                // If this header is expanded, draw its children
                if (expandedState[headerItem] && !isCollapsed)
                {
                    yOffset = DrawChildItems(graphics, rectangle, headerItem, yOffset, mousePoint, renderOptions);
                }

                // Draw drop indicator during drag operations
                if (_isDragging && _dropInsertIndex >= 0)
                {
                    int itemIdx = 0;
                    int indicatorY = -1;

                    foreach (var checkItem in items)
                    {
                        if (itemIdx == _dropInsertIndex)
                        {
                            indicatorY = GetItemYPosition(checkItem) - spacing / 2;
                            break;
                        }
                        itemIdx++;
                    }

                    if (indicatorY < 0 && _dropInsertIndex >= items.Count)
                    {
                        indicatorY = yOffset - spacing / 2;
                    }

                    if (indicatorY > 0)
                    {
                        DrawDropIndicator(graphics, rectangle, indicatorY);
                    }
                }
            }
        }

        private int DrawHeaderItem(Graphics graphics, Rectangle rectangle, SimpleItem headerItem, int yOffset, Point mousePoint, AccordionRenderOptions options)
        {
            // Create the overall item rectangle
            Rectangle headerItemRect = new Rectangle(
                rectangle.X,
                yOffset,
                rectangle.Width,
                itemHeight
            );

            // Check if this item is hovered or selected
            bool isHovered = headerItemRect.Contains(mousePoint);
            bool isSelected = headerItem == SelectedItem;
            bool isExpanded = expandedState[headerItem];

            // Create item state
            var itemState = new AccordionItemState
            {
                IsHovered = isHovered,
                IsSelected = isSelected,
                IsExpanded = isExpanded,
                IsChild = false,
                Level = 0
            };

            // Use painter to draw item
            _painter.PaintItem(graphics, headerItemRect, headerItem, itemState, options);

            // Draw item icon if available
            if (HasItemIcons && !string.IsNullOrEmpty(headerItem.ImagePath))
            {
                DrawItemIcon(graphics, headerItem, headerItemRect, isSelected, isHovered);
            }

            // Add hit area for expand/collapse if item has children
            if (headerItem.Children.Count > 0 && !isCollapsed)
            {
                Rectangle expanderRect = new Rectangle(
                    headerItemRect.Right - 32,
                    headerItemRect.Top + (headerItemRect.Height - 16) / 2,
                    16,
                    16);

                AddHitArea(
                    $"ExpandCollapse_{headerItem.Text}_{items.IndexOf(headerItem)}",
                    expanderRect,
                    null,
                    () => {
                        expandedState[headerItem] = !expandedState[headerItem];
                        HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                        Invalidate();
                    }
                );
            }

            // Add hit test area for this header item
            AddHitArea(
                $"HeaderItem_{headerItem.Text}_{items.IndexOf(headerItem)}",
                headerItemRect,
                null,
                () => {
                    if (headerItem.Children.Count > 0)
                    {
                        expandedState[headerItem] = !expandedState[headerItem];
                        HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                    }
                    SelectedItem = headerItem;
                    ItemClick?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                    Invalidate();
                }
            );

            // Store the item location for hit testing
            headerItem.X = headerItemRect.X;
            headerItem.Y = headerItemRect.Y;
            headerItem.Width = headerItemRect.Width;
            headerItem.Height = headerItemRect.Height;

            // Update yOffset for the next item
            return yOffset + itemHeight + spacing;
        }

        private int DrawChildItems(Graphics graphics, Rectangle rectangle, SimpleItem headerItem, int yOffset, Point mousePoint, AccordionRenderOptions options)
        {
            foreach (var childItem in headerItem.Children)
            {
                // Create the child item rectangle with indentation
                Rectangle childItemRect = new Rectangle(
                    rectangle.X + indentationWidth,
                    yOffset,
                    rectangle.Width - indentationWidth,
                    childItemHeight
                );

                // Check if this child item is hovered or selected
                bool isHovered = childItemRect.Contains(mousePoint);
                bool isSelected = childItem == SelectedItem;

                // Create item state
                var itemState = new AccordionItemState
                {
                    IsHovered = isHovered,
                    IsSelected = isSelected,
                    IsExpanded = false,
                    IsChild = true,
                    Level = 1
                };

                // Draw connector line from parent to child
                int lineX = rectangle.X + (indentationWidth / 2);
                int lineTop = headerItem.Y + itemHeight;
                int lineBottom = yOffset + childItemHeight / 2;

                _painter.PaintConnectorLine(
                    graphics,
                    new Point(lineX, lineTop),
                    new Point(childItemRect.X, lineBottom),
                    options);

                // Use painter to draw child item
                _painter.PaintChildItem(graphics, childItemRect, childItem, itemState, options);

                // Draw item icon if available
                if (HasItemIcons && !string.IsNullOrEmpty(childItem.ImagePath))
                {
                    DrawItemIcon(graphics, childItem, childItemRect, isSelected, isHovered);
                }

                // Add hit test area for this child item
                AddHitArea(
                    $"ChildItem_{childItem.Text}_{headerItem.Children.IndexOf(childItem)}",
                    childItemRect,
                    null,
                    () => {
                        SelectedItem = (SimpleItem)childItem;
                        ItemClick?.Invoke(this, new BeepMouseEventArgs(childItem.Text, childItem));
                        Invalidate();
                    }
                );

                // Store the child item location for hit testing
                childItem.X = childItemRect.X;
                childItem.Y = childItemRect.Y;
                childItem.Width = childItemRect.Width;
                childItem.Height = childItemRect.Height;

                // Update yOffset for the next item
                yOffset += childItemHeight + spacing;
            }

            return yOffset;
        }

        // Helper method to draw a hamburger icon
        private void DrawHamburgerIcon(Graphics g, Rectangle rect)
        {
            string iconPath = AccordionIconHelpers.GetHamburgerIconPath();
            Color iconColor = AccordionThemeHelpers.GetAccordionForegroundColor(
                _currentTheme,
                UseThemeColors);

            AccordionIconHelpers.PaintIcon(
                g,
                rect,
                iconPath,
                iconColor,
                _currentTheme,
                UseThemeColors,
                false,
                false,
                ControlStyle);
        }

        private void DrawItemIcon(Graphics g, SimpleItem item, Rectangle bounds, bool isSelected, bool isHovered)
        {
            if (!HasItemIcons || string.IsNullOrEmpty(item.ImagePath)) return;

            var iconRect = new Rectangle(
                bounds.X + 8,
                bounds.Y + (bounds.Height - 20) / 2,
                20,
                20);

            var iconColor = isSelected
                ? AccordionThemeHelpers.GetItemForegroundColor(_currentTheme, UseThemeColors, true)
                : (isHovered
                    ? AccordionThemeHelpers.GetItemForegroundColor(_currentTheme, UseThemeColors)
                    : AccordionThemeHelpers.GetAccordionForegroundColor(_currentTheme, UseThemeColors));

            StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconColor);
        }

        private int GetItemYPosition(SimpleItem item)
        {
            if (item != null)
                return item.Y;

            int y = headerHeight + spacing;
            foreach (var headerItem in items)
            {
                if (headerItem == item)
                    return y;
                y += itemHeight + spacing;
                if (expandedState.ContainsKey(headerItem) && expandedState[headerItem])
                {
                    y += headerItem.Children.Count * (childItemHeight + spacing);
                }
            }
            return y;
        }

        private void DrawDropIndicator(Graphics g, Rectangle bounds, int y)
        {
            using var pen = new Pen(Color.FromArgb(100, 33, 150, 243), 2);
            using var dashPen = new Pen(Color.FromArgb(180, 33, 150, 243), 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            int left = bounds.X + 4;
            int right = bounds.Right - 4;

            g.DrawLine(dashPen, left, y, right, y);

            var arrowSize = 6;
            using var brush = new SolidBrush(Color.FromArgb(180, 33, 150, 243));
            var arrowPoints = new[]
            {
                new Point(left, y),
                new Point(left + arrowSize, y - arrowSize / 2),
                new Point(left + arrowSize, y + arrowSize / 2)
            };
            g.FillPolygon(brush, arrowPoints);

            var arrowPointsRight = new[]
            {
                new Point(right, y),
                new Point(right - arrowSize, y - arrowSize / 2),
                new Point(right - arrowSize, y + arrowSize / 2)
            };
            g.FillPolygon(brush, arrowPointsRight);
        }
    }
}
