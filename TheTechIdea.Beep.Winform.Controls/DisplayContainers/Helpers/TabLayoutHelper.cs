using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for calculating tab layouts and positions
    /// </summary>
    internal class TabLayoutHelper
    {
        public struct TabLayoutResult
        {
            public bool NeedsScrolling;
            public Rectangle ScrollLeftButton;
            public Rectangle ScrollRightButton; 
            public Rectangle NewTabButton;
        }

        public TabLayoutResult CalculateTabLayout(List<AddinTab> tabs, Rectangle tabArea, 
            TabPosition position, int minWidth, int maxWidth, int scrollOffset)
        {
            if (tabs == null || tabs.Count == 0)
                return new TabLayoutResult();

            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;
            int availableSpace = isHorizontal ? tabArea.Width : tabArea.Height;
            
            // Reserve space for scroll buttons (40px each) and new tab button (40px)
            availableSpace -= 120;

            int totalTabWidth = tabs.Count * minWidth;
            bool needsScrolling = totalTabWidth > availableSpace;

            if (needsScrolling)
            {
                CalculateScrollingLayout(tabs, tabArea, position, minWidth, scrollOffset, availableSpace);
            }
            else
            {
                CalculateFixedLayout(tabs, tabArea, position, minWidth, maxWidth, availableSpace);
            }

            return new TabLayoutResult
            {
                NeedsScrolling = needsScrolling,
                ScrollLeftButton = GetScrollLeftButtonRect(tabArea, position),
                ScrollRightButton = GetScrollRightButtonRect(tabArea, position),
                NewTabButton = GetNewTabButtonRect(tabArea, position)
            };
        }

        private void CalculateFixedLayout(List<AddinTab> tabs, Rectangle tabArea, TabPosition position,
            int minWidth, int maxWidth, int availableSpace)
        {
            int tabCount = tabs.Count;
            int tabWidth = Math.Min(maxWidth, Math.Max(minWidth, availableSpace / tabCount));
            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;

            for (int i = 0; i < tabCount; i++)
            {
                var tab = tabs[i];
                if (isHorizontal)
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X + i * tabWidth,
                        tabArea.Y,
                        tabWidth,
                        tabArea.Height
                    );
                }
                else
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X,
                        tabArea.Y + i * tabWidth,
                        tabArea.Width,
                        tabWidth
                    );
                }
                tab.IsVisible = true;
            }
        }

        private void CalculateScrollingLayout(List<AddinTab> tabs, Rectangle tabArea, TabPosition position,
            int minWidth, int scrollOffset, int availableSpace)
        {
            int visibleTabCount = Math.Max(1, availableSpace / minWidth);
            int startIndex = Math.Max(0, scrollOffset);
            int endIndex = Math.Min(tabs.Count, startIndex + visibleTabCount);
            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;

            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                if (i >= startIndex && i < endIndex)
                {
                    int visibleIndex = i - startIndex;
                    if (isHorizontal)
                    {
                        tab.Bounds = new Rectangle(
                            tabArea.X + 40 + visibleIndex * minWidth, // 40px offset for scroll button
                            tabArea.Y,
                            minWidth,
                            tabArea.Height
                        );
                    }
                    else
                    {
                        tab.Bounds = new Rectangle(
                            tabArea.X,
                            tabArea.Y + 40 + visibleIndex * minWidth,
                            tabArea.Width,
                            minWidth
                        );
                    }
                    tab.IsVisible = true;
                }
                else
                {
                    tab.IsVisible = false;
                }
            }
        }

        private Rectangle GetScrollLeftButtonRect(Rectangle tabArea, TabPosition position)
        {
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - 120, tabArea.Y + 4, 32, tabArea.Height - 8),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + 4, tabArea.Bottom - 120, tabArea.Width - 8, 32),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetScrollRightButtonRect(Rectangle tabArea, TabPosition position)
        {
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - 80, tabArea.Y + 4, 32, tabArea.Height - 8),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + 4, tabArea.Bottom - 80, tabArea.Width - 8, 32),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetNewTabButtonRect(Rectangle tabArea, TabPosition position)
        {
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - 40, tabArea.Y + 4, 32, tabArea.Height - 8),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + 4, tabArea.Bottom - 40, tabArea.Width - 8, 32),
                _ => Rectangle.Empty
            };
        }

        public AddinTab GetTabAt(List<AddinTab> tabs, Point point)
        {
            return tabs?.FirstOrDefault(tab => tab.IsVisible && tab.Bounds.Contains(point));
        }

        public Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            return new Rectangle(
                tabBounds.Right - 20,
                tabBounds.Y + (tabBounds.Height - 12) / 2,
                12,
                12
            );
        }
    }
}