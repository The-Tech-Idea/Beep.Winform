using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for calculating tab layouts and positions with BeepStyling metrics support
    /// </summary>
    internal class TabLayoutHelper
    {
        private BeepControlStyle _controlStyle = BeepControlStyle.Modern;
        private Font _font = new Font("Segoe UI", 9f);

        public Control OwnerControl { get; set; }
        public Font TextFont { get => _font; set => _font = value ?? new Font("Segoe UI", 9f); }
        
        public struct TabLayoutResult
        {
            public bool NeedsScrolling;
            public Rectangle ScrollLeftButton;
            public Rectangle ScrollRightButton; 
            public Rectangle NewTabButton;
        }
        
        /// <summary>
        /// Updates the control style and font used for calculating tab metrics
        /// </summary>
        public void UpdateStyle(BeepControlStyle style, Font font)
        {
            _controlStyle = style;
            _font = font ?? new Font("Segoe UI", 9f);
        }

        public TabLayoutResult CalculateTabLayout(List<AddinTab> tabs, Rectangle tabArea, 
            TabPosition position, int minWidth, int maxWidth, int scrollOffset)
        {
            if (tabs == null || tabs.Count == 0)
                return new TabLayoutResult();

            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;
            int availableSpace = isHorizontal ? tabArea.Width : tabArea.Height;
            
            // Reserve space for scroll buttons and new tab button (scaled)
            int reservedSpace = DpiScalingHelper.ScaleValue(120, OwnerControl);
            availableSpace -= reservedSpace;

            // Calculate actual tab widths based on content and style metrics
            var tabWidths = CalculateTabWidths(tabs, minWidth, maxWidth);
            int totalTabWidth = tabWidths.Sum();
            bool needsScrolling = totalTabWidth > availableSpace;

            if (needsScrolling)
            {
                CalculateScrollingLayout(tabs, tabArea, position, minWidth, scrollOffset, availableSpace);
            }
            else
            {
                CalculateFixedLayoutWithMetrics(tabs, tabArea, position, tabWidths, availableSpace);
            }

            return new TabLayoutResult
            {
                NeedsScrolling = needsScrolling,
                ScrollLeftButton = GetScrollLeftButtonRect(tabArea, position),
                ScrollRightButton = GetScrollRightButtonRect(tabArea, position),
                NewTabButton = GetNewTabButtonRect(tabArea, position)
            };
        }
        
        /// <summary>
        /// Calculates the width for each tab based on text content and BeepStyling metrics
        /// </summary>
        private List<int> CalculateTabWidths(List<AddinTab> tabs, int minWidth, int maxWidth)
        {
            var widths = new List<int>();
            
            // Get style metrics from BeepStyling
            float borderWidth = BeepStyling.GetBorderThickness(_controlStyle);
            int padding = BeepStyling.GetPadding(_controlStyle);
            int shadowDepth = StyleShadows.HasShadow(_controlStyle) ? 
                Math.Max(2, StyleShadows.GetShadowBlur(_controlStyle) / 2) : 0;
            
            // Total chrome (border + padding + shadow) on each side
            int totalChromeWidth = (int)Math.Ceiling(borderWidth * 2) + (padding * 2) + (shadowDepth * 2);
            
            foreach (var tab in tabs)
            {
                int contentWidth = CalculateTabContentWidth(tab, totalChromeWidth);
                int finalWidth = Math.Max(minWidth, Math.Min(maxWidth, contentWidth));
                widths.Add(finalWidth);
            }
            
            return widths;
        }
        
        /// <summary>
        /// Calculates the content width for a single tab including text and close button
        /// </summary>
        private int CalculateTabContentWidth(AddinTab tab, int chromeWidth)
        {
            int minWidthScaled = DpiScalingHelper.ScaleValue(80, OwnerControl);
            if (tab == null || string.IsNullOrEmpty(tab.Title))
                return minWidthScaled + chromeWidth; // Minimum width
            
            // Measure text width using TextRenderer (safe and accurate)
            int textWidth = 0;
            try
            {
                var textSize = TextRenderer.MeasureText(tab.Title, _font, 
                    new Size(int.MaxValue, int.MaxValue), 
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                textWidth = textSize.Width;
            }
            catch
            {
                // Fallback: estimate based on character count (scaled)
                textWidth = tab.Title.Length * DpiScalingHelper.ScaleValue(7, OwnerControl);
            }
            
            // Add space for close button (if enabled) and internal padding
            int closeButtonWidth = tab.CanClose ? DpiScalingHelper.ScaleValue(20, OwnerControl) : 0;
            int internalPadding = DpiScalingHelper.ScaleValue(16, OwnerControl); // 8px on each side
            
            // Total content width = text + close button + internal padding
            int contentWidth = textWidth + closeButtonWidth + internalPadding;
            
            // Add chrome width (border + padding + shadow)
            return contentWidth + chromeWidth;
        }
        
        /// <summary>
        /// Calculates fixed layout with individual tab widths based on content
        /// </summary>
        private void CalculateFixedLayoutWithMetrics(List<AddinTab> tabs, Rectangle tabArea, 
            TabPosition position, List<int> tabWidths, int availableSpace)
        {
            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;
            int currentX = tabArea.X;
            int currentY = tabArea.Y;
            
            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                int tabWidth = tabWidths[i];
                
                if (isHorizontal)
                {
                    tab.Bounds = new Rectangle(
                        currentX,
                        tabArea.Y,
                        tabWidth,
                        tabArea.Height
                    );
                    currentX += tabWidth + DpiScalingHelper.ScaleValue(2, OwnerControl); // gap between tabs
                }
                else
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X,
                        currentY,
                        tabArea.Width,
                        tabWidth
                    );
                    currentY += tabWidth + DpiScalingHelper.ScaleValue(2, OwnerControl); // gap between tabs
                }
                tab.IsVisible = true;
            }
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
                    int scrollOffsetScaled = DpiScalingHelper.ScaleValue(40, OwnerControl);
                    if (isHorizontal)
                    {
                        tab.Bounds = new Rectangle(
                            tabArea.X + scrollOffsetScaled + visibleIndex * minWidth,
                            tabArea.Y,
                            minWidth,
                            tabArea.Height
                        );
                    }
                    else
                    {
                        tab.Bounds = new Rectangle(
                            tabArea.X,
                            tabArea.Y + scrollOffsetScaled + visibleIndex * minWidth,
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
            int pad = DpiScalingHelper.ScaleValue(4, OwnerControl);
            int size = DpiScalingHelper.ScaleValue(32, OwnerControl);
            int pos120 = DpiScalingHelper.ScaleValue(120, OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - pos120, tabArea.Y + pad, size, tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + pad, tabArea.Bottom - pos120, tabArea.Width - pad * 2, size),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetScrollRightButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = DpiScalingHelper.ScaleValue(4, OwnerControl);
            int size = DpiScalingHelper.ScaleValue(32, OwnerControl);
            int pos80 = DpiScalingHelper.ScaleValue(80, OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - pos80, tabArea.Y + pad, size, tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + pad, tabArea.Bottom - pos80, tabArea.Width - pad * 2, size),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetNewTabButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = DpiScalingHelper.ScaleValue(4, OwnerControl);
            int size = DpiScalingHelper.ScaleValue(32, OwnerControl);
            int pos40 = DpiScalingHelper.ScaleValue(40, OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(tabArea.Right - pos40, tabArea.Y + pad, size, tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(tabArea.X + pad, tabArea.Bottom - pos40, tabArea.Width - pad * 2, size),
                _ => Rectangle.Empty
            };
        }

        public AddinTab GetTabAt(List<AddinTab> tabs, Point point)
        {
            return tabs?.FirstOrDefault(tab => tab.IsVisible && tab.Bounds.Contains(point));
        }

        public Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            int closeW = DpiScalingHelper.ScaleValue(20, OwnerControl);
            int closeSize = DpiScalingHelper.ScaleValue(12, OwnerControl);
            return new Rectangle(
                tabBounds.Right - closeW,
                tabBounds.Y + (tabBounds.Height - closeSize) / 2,
                closeSize,
                closeSize
            );
        }
    }
}