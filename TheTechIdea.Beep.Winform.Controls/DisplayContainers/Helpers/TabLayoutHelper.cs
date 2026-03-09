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
        private Font _font = SystemFonts.MessageBoxFont;

        public Control OwnerControl { get; set; }
        public Font TextFont { get => _font; set => _font = value ?? SystemFonts.MessageBoxFont; }
        
        public struct TabLayoutResult
        {
            public bool NeedsScrolling;
            public Rectangle ScrollLeftButton;
            public Rectangle ScrollRightButton; 
            public Rectangle OverflowButton;
            public Rectangle NewTabButton;
        }
        
        /// <summary>
        /// Updates the control style and font used for calculating tab metrics
        /// </summary>
        public void UpdateStyle(BeepControlStyle style, Font font)
        {
            _controlStyle = style;
            _font = font ?? SystemFonts.MessageBoxFont;
        }

        public TabLayoutResult CalculateTabLayout(List<AddinTab> tabs, Rectangle tabArea, 
            TabPosition position, int minWidth, int maxWidth, int scrollOffset)
        {
            if (tabs == null || tabs.Count == 0)
                return CalculateUtilityButtonLayout(tabArea, position, false);

            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;
            int stripLength = isHorizontal ? tabArea.Width : tabArea.Height;

            // Calculate actual tab widths based on content and style metrics
            var tabWidths = CalculateTabWidths(tabs, minWidth, maxWidth);
            // Include inter-tab gaps in the overflow check (previously they were missing,
            // causing the scroll path to trigger too late and cram tabs together).
            int gap = TabHeaderMetrics.TabGap(OwnerControl);
            int totalTabWidth = tabWidths.Sum() + gap * Math.Max(0, tabs.Count - 1);
            int noScrollAvailableSpace = stripLength - TabHeaderMetrics.NewTabButtonReservedWidth(OwnerControl);
            bool needsScrolling = totalTabWidth > noScrollAvailableSpace;
            int reservedSpace = needsScrolling
                ? TabHeaderMetrics.UtilityButtonsReservedWidth(OwnerControl)
                : TabHeaderMetrics.NewTabButtonReservedWidth(OwnerControl);
            int availableSpace = Math.Max(0, stripLength - reservedSpace);

            if (needsScrolling)
            {
                CalculateScrollingLayout(tabs, tabArea, position, minWidth, scrollOffset, availableSpace, tabWidths);
            }
            else
            {
                CalculateFixedLayoutWithMetrics(tabs, tabArea, position, tabWidths, availableSpace);
            }

            return new TabLayoutResult
            {
                NeedsScrolling = needsScrolling,
                ScrollLeftButton = needsScrolling ? GetScrollLeftButtonRect(tabArea, position) : Rectangle.Empty,
                ScrollRightButton = needsScrolling ? GetScrollRightButtonRect(tabArea, position) : Rectangle.Empty,
                OverflowButton = needsScrolling ? GetOverflowButtonRect(tabArea, position) : Rectangle.Empty,
                NewTabButton = GetNewTabButtonRect(tabArea, position)
            };
        }

        public TabLayoutResult CalculateUtilityButtonLayout(Rectangle tabArea, TabPosition position, bool needsScrolling)
        {
            if (tabArea.IsEmpty)
                return new TabLayoutResult();

            return new TabLayoutResult
            {
                NeedsScrolling = needsScrolling,
                ScrollLeftButton = needsScrolling ? GetScrollLeftButtonRect(tabArea, position) : Rectangle.Empty,
                ScrollRightButton = needsScrolling ? GetScrollRightButtonRect(tabArea, position) : Rectangle.Empty,
                OverflowButton = needsScrolling ? GetOverflowButtonRect(tabArea, position) : Rectangle.Empty,
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

            // Pinned tabs are compact (icon-only width)
            if (tab.IsPinned)
                return TabHeaderMetrics.PinnedTabWidth(OwnerControl) + chromeWidth;
            
            // Measure text width using TextRenderer (safe and accurate)
            int textWidth = 0;
            try
            {
                var measureFont = _font;
                var textSize = TextRenderer.MeasureText(tab.Title, measureFont, 
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
            int closeButtonWidth = tab.CanClose ? TabHeaderMetrics.CloseButtonSlotWidth(OwnerControl) : 0;
            int internalPadding = TabHeaderMetrics.TextContentPadding(OwnerControl);

            // Add icon slot width when an icon path is specified
            int iconWidth = !string.IsNullOrEmpty(tab.IconPath) ? TabHeaderMetrics.IconSlotWidth(OwnerControl) : 0;
            
            // Total content width = icon + text + close button + internal padding
            int contentWidth = iconWidth + textWidth + closeButtonWidth + internalPadding;
            
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
                    currentX += tabWidth + TabHeaderMetrics.TabGap(OwnerControl);
                }
                else
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X,
                        currentY,
                        tabArea.Width,
                        tabWidth
                    );
                    currentY += tabWidth + TabHeaderMetrics.TabGap(OwnerControl);
                }
                tab.IsVisible = true;
            }
        }

        private void CalculateFixedLayout(List<AddinTab> tabs, Rectangle tabArea, TabPosition position,
            int minWidth, int maxWidth, int availableSpace)
        {
            int tabCount = tabs.Count;
            int gap = TabHeaderMetrics.TabGap(OwnerControl);
            // Fit as many tabs as possible without going over available space.
            int tabWidth = Math.Min(maxWidth, Math.Max(minWidth,
                tabCount > 0 ? (availableSpace - gap * (tabCount - 1)) / tabCount : availableSpace));
            int stride = tabWidth + gap;
            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;

            for (int i = 0; i < tabCount; i++)
            {
                var tab = tabs[i];
                if (isHorizontal)
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X + i * stride,
                        tabArea.Y,
                        tabWidth,
                        tabArea.Height
                    );
                }
                else
                {
                    tab.Bounds = new Rectangle(
                        tabArea.X,
                        tabArea.Y + i * stride,
                        tabArea.Width,
                        tabWidth
                    );
                }
                tab.IsVisible = true;
            }
        }

        private void CalculateScrollingLayout(List<AddinTab> tabs, Rectangle tabArea, TabPosition position,
            int minWidth, int scrollOffset, int availableSpace, List<int> tabWidths)
        {
            // Pixel-advancing scroll layout: uses real per-tab content widths so each
            // tab header is exactly as wide as its text + chrome, not a fixed stride.
            int gap = TabHeaderMetrics.TabGap(OwnerControl);
            bool isHorizontal = position == TabPosition.Top || position == TabPosition.Bottom;
            int startIndex = Math.Max(0, Math.Min(scrollOffset, tabs.Count - 1));

            int currentPos = isHorizontal ? tabArea.X : tabArea.Y;
            int endPos = currentPos + availableSpace;

            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];

                // Tabs before the scroll window are hidden (they've been scrolled past)
                if (i < startIndex)
                {
                    tab.IsVisible = false;
                    continue;
                }

                // Use the real content-measured width for this tab
                int w = (tabWidths != null && i < tabWidths.Count) ? tabWidths[i] : minWidth;

                // No more room in the visible strip — hide this and all remaining tabs
                if (currentPos + w > endPos)
                {
                    for (int j = i; j < tabs.Count; j++)
                        tabs[j].IsVisible = false;
                    break;
                }

                tab.Bounds = isHorizontal
                    ? new Rectangle(currentPos, tabArea.Y, w, tabArea.Height)
                    : new Rectangle(tabArea.X, currentPos, tabArea.Width, w);
                tab.IsVisible = true;
                currentPos += w + gap;
            }
        }

        private Rectangle GetScrollLeftButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = TabHeaderMetrics.UtilityButtonPadding(OwnerControl);
            int size = TabHeaderMetrics.UtilityButtonSize(OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(
                        tabArea.Right - (size * 4 + pad),
                        tabArea.Y + pad,
                        size,
                        tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(
                        tabArea.X + pad,
                        tabArea.Bottom - (size * 4 + pad),
                        tabArea.Width - pad * 2,
                        size),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetScrollRightButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = TabHeaderMetrics.UtilityButtonPadding(OwnerControl);
            int size = TabHeaderMetrics.UtilityButtonSize(OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(
                        tabArea.Right - (size * 3 + pad),
                        tabArea.Y + pad,
                        size,
                        tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(
                        tabArea.X + pad,
                        tabArea.Bottom - (size * 3 + pad),
                        tabArea.Width - pad * 2,
                        size),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetOverflowButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = TabHeaderMetrics.UtilityButtonPadding(OwnerControl);
            int size = TabHeaderMetrics.UtilityButtonSize(OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(
                        tabArea.Right - (size * 2 + pad),
                        tabArea.Y + pad,
                        size,
                        tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(
                        tabArea.X + pad,
                        tabArea.Bottom - (size * 2 + pad),
                        tabArea.Width - pad * 2,
                        size),
                _ => Rectangle.Empty
            };
        }

        private Rectangle GetNewTabButtonRect(Rectangle tabArea, TabPosition position)
        {
            int pad = TabHeaderMetrics.UtilityButtonPadding(OwnerControl);
            int size = TabHeaderMetrics.UtilityButtonSize(OwnerControl);
            return position switch
            {
                TabPosition.Top or TabPosition.Bottom => 
                    new Rectangle(
                        tabArea.Right - (size + pad),
                        tabArea.Y + pad,
                        size,
                        tabArea.Height - pad * 2),
                TabPosition.Left or TabPosition.Right => 
                    new Rectangle(
                        tabArea.X + pad,
                        tabArea.Bottom - (size + pad),
                        tabArea.Width - pad * 2,
                        size),
                _ => Rectangle.Empty
            };
        }

        public AddinTab GetTabAt(List<AddinTab> tabs, Point point)
        {
            return tabs?.FirstOrDefault(tab => tab.IsVisible && tab.Bounds.Contains(point));
        }

        public Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            return TabHeaderMetrics.GetCloseButtonBounds(tabBounds, OwnerControl);
        }
    }
}