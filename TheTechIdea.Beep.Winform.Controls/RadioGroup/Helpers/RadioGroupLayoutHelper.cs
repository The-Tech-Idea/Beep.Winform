using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for managing layout calculations and positioning for BeepRadioGroupAdvanced
    /// </summary>
    internal class RadioGroupLayoutHelper
    {
        private readonly BaseControl _owner;
        
        public RadioGroupLayoutHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Layout Properties
        public RadioGroupOrientation Orientation { get; set; } = RadioGroupOrientation.Vertical;
        public RadioGroupAlignment Alignment { get; set; } = RadioGroupAlignment.Left;
        public Size ItemSize { get; set; } = new Size(200, 40);
        public int ItemSpacing { get; set; } = 8;
        public int ColumnCount { get; set; } = 1;
        public bool AutoSize { get; set; } = true;
        public Padding ItemPadding { get; set; } = new Padding(8);

        // Optional external measurer (renderer-provided)
        public Func<SimpleItem, Graphics, Size> ItemMeasurer { get; set; }
        #endregion

        #region Layout Calculation
        /// <summary>
        /// Calculates the layout rectangles for all items based on the current settings
        /// </summary>
        public List<Rectangle> CalculateItemRectangles(List<SimpleItem> items, Rectangle containerRect)
        {
            var rectangles = new List<Rectangle>();
            
            if (items == null || items.Count == 0)
                return rectangles;

            switch (Orientation)
            {
                case RadioGroupOrientation.Vertical:
                    return CalculateVerticalLayout(items, containerRect);
                    
                case RadioGroupOrientation.Horizontal:
                    return CalculateHorizontalLayout(items, containerRect);
                    
                case RadioGroupOrientation.Grid:
                    return CalculateGridLayout(items, containerRect);
                    
                case RadioGroupOrientation.Flow:
                    return CalculateFlowLayout(items, containerRect);
                    
                default:
                    return CalculateVerticalLayout(items, containerRect);
            }
        }

        private List<Rectangle> CalculateVerticalLayout(List<SimpleItem> items, Rectangle containerRect)
        {
            var rectangles = new List<Rectangle>();
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            int currentY = containerRect.Y + scaledPadding.Top;
            int itemWidth = containerRect.Width - scaledPadding.Horizontal;

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemHeight;
                    if (AutoSize)
                    {
                        var measured = ItemMeasurer?.Invoke(item, g);
                        itemHeight = measured?.Height > 0 ? measured.Value.Height : CalculateItemHeight_Fallback(item, g);
                    }
                    else
                    {
                        itemHeight = DpiScalingHelper.ScaleValue(ItemSize.Height, _owner);
                    }
                    
                    Rectangle itemRect = new Rectangle(
                        containerRect.X + scaledPadding.Left,
                        currentY,
                        itemWidth,
                        itemHeight
                    );

                    rectangles.Add(itemRect);
                    currentY += itemHeight + spacing;
                }
            }

            return rectangles;
        }

        private List<Rectangle> CalculateHorizontalLayout(List<SimpleItem> items, Rectangle containerRect)
        {
            var rectangles = new List<Rectangle>();
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            int currentX = containerRect.X + scaledPadding.Left;
            int itemHeight = containerRect.Height - scaledPadding.Vertical;

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemWidth;
                    if (AutoSize)
                    {
                        var measured = ItemMeasurer?.Invoke(item, g);
                        itemWidth = measured?.Width > 0 ? measured.Value.Width : CalculateItemWidth_Fallback(item, g);
                    }
                    else
                    {
                        itemWidth = DpiScalingHelper.ScaleValue(ItemSize.Width, _owner);
                    }
                    
                    Rectangle itemRect = new Rectangle(
                        currentX,
                        containerRect.Y + scaledPadding.Top,
                        itemWidth,
                        itemHeight
                    );

                    rectangles.Add(itemRect);
                    currentX += itemWidth + spacing;
                }
            }

            return rectangles;
        }

        private List<Rectangle> CalculateGridLayout(List<SimpleItem> items, Rectangle containerRect)
        {
            var rectangles = new List<Rectangle>();
            int columns = Math.Max(1, ColumnCount);
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            int rows = (int)Math.Ceiling((double)items.Count / columns);
            
            int availableWidth = containerRect.Width - scaledPadding.Horizontal;
            int availableHeight = containerRect.Height - scaledPadding.Vertical;
            
            int itemWidth;
            int itemHeight;
            using (Graphics g = _owner.CreateGraphics())
            {
                if (AutoSize)
                {
                    var measured = ItemMeasurer?.Invoke(items.FirstOrDefault(), g);
                    if (measured.HasValue && measured.Value.Width > 0 && measured.Value.Height > 0)
                    {
                        itemWidth = measured.Value.Width;
                        itemHeight = measured.Value.Height;
                    }
                    else
                    {
                        itemWidth = CalculateItemWidth_Fallback(items.FirstOrDefault(), g);
                        itemHeight = CalculateItemHeight_Fallback(items.FirstOrDefault(), g);
                    }
                }
                else
                {
                    itemWidth = DpiScalingHelper.ScaleValue(ItemSize.Width, _owner);
                    itemHeight = DpiScalingHelper.ScaleValue(ItemSize.Height, _owner);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                
                Rectangle itemRect = new Rectangle(
                    containerRect.X + scaledPadding.Left + col * (itemWidth + spacing),
                    containerRect.Y + scaledPadding.Top + row * (itemHeight + spacing),
                    itemWidth,
                    itemHeight
                );

                rectangles.Add(itemRect);
            }

            return rectangles;
        }

        private List<Rectangle> CalculateFlowLayout(List<SimpleItem> items, Rectangle containerRect)
        {
            var rectangles = new List<Rectangle>();
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            int currentX = containerRect.X + scaledPadding.Left;
            int currentY = containerRect.Y + scaledPadding.Top;
            int maxWidth = containerRect.Width - scaledPadding.Horizontal;
            int lineHeight = 0;

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemWidth = AutoSize ? (ItemMeasurer?.Invoke(item, g).Width ?? CalculateItemWidth_Fallback(item, g)) : ItemSize.Width;
                    int itemHeight = AutoSize ? (ItemMeasurer?.Invoke(item, g).Height ?? CalculateItemHeight_Fallback(item, g)) : ItemSize.Height;

                    // Check if we need to wrap to next line
                    if (currentX + itemWidth > containerRect.X + maxWidth && currentX > containerRect.X + scaledPadding.Left)
                    {
                        currentX = containerRect.X + scaledPadding.Left;
                        currentY += lineHeight + spacing;
                        lineHeight = 0;
                    }

                    Rectangle itemRect = new Rectangle(currentX, currentY, itemWidth, itemHeight);
                    rectangles.Add(itemRect);

                    currentX += itemWidth + spacing;
                    lineHeight = Math.Max(lineHeight, itemHeight);
                }
            }

            return rectangles;
        }
        #endregion

        #region Size Calculation (fallbacks)
        private int CalculateItemWidth_Fallback(SimpleItem item, Graphics g)
        {
            if (item == null) return DpiScalingHelper.ScaleValue(ItemSize.Width, _owner);

            // Base width for selector (radio button or checkbox)
            int selectorWidth = DpiScalingHelper.ScaleValue(24, _owner); // Standard size for radio/checkbox
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? DpiScalingHelper.ScaleValue(24, _owner) : 0;
            int textWidth = 0;

            // Calculate text width if text exists
            if (!string.IsNullOrEmpty(item.Text))
            {
                SizeF textSize = TextUtils.MeasureText(g,item.Text, _owner.Font);
                textWidth = (int)Math.Ceiling(textSize.Width);
            }

            // Add spacing between components
            int spacing = DpiScalingHelper.ScaleValue(8, _owner);
            int components = 1; // Always have selector
            if (imageWidth > 0) components++;
            if (textWidth > 0) components++;

            int totalSpacing = (components - 1) * spacing;
            int contentWidth = selectorWidth + imageWidth + textWidth + totalSpacing;

            // Add padding
            return contentWidth + DpiScalingHelper.ScalePadding(ItemPadding, _owner).Horizontal;
        }

        private int CalculateItemHeight_Fallback(SimpleItem item, Graphics g)
        {
            if (item == null) return DpiScalingHelper.ScaleValue(ItemSize.Height, _owner);

            int textHeight = 0;
            int imageHeight = !string.IsNullOrEmpty(item.ImagePath) ? DpiScalingHelper.ScaleValue(24, _owner) : 0;
            int selectorHeight = DpiScalingHelper.ScaleValue(24, _owner); // Standard size for radio/checkbox

            // Calculate text height if text exists
            if (!string.IsNullOrEmpty(item.Text))
            {
                SizeF textSize = TextUtils.MeasureText(g,item.Text, _owner.Font);
                textHeight = (int)Math.Ceiling(textSize.Height);
            }

            // Take the maximum height of all components
            int contentHeight = Math.Max(Math.Max(textHeight, imageHeight), selectorHeight);

            // Add padding
            return contentHeight + DpiScalingHelper.ScalePadding(ItemPadding, _owner).Vertical;
        }
        #endregion

        #region Total Size Calculation
        /// <summary>
        /// Calculates the total size needed for the control based on items and layout
        /// </summary>
        public Size CalculateTotalSize(List<SimpleItem> items, Size maxSize)
        {
            if (items == null || items.Count == 0)
                return new Size(100, 50);

            switch (Orientation)
            {
                case RadioGroupOrientation.Vertical:
                    return CalculateVerticalTotalSize(items, maxSize);
                    
                case RadioGroupOrientation.Horizontal:
                    return CalculateHorizontalTotalSize(items, maxSize);
                    
                case RadioGroupOrientation.Grid:
                    return CalculateGridTotalSize(items, maxSize);
                    
                case RadioGroupOrientation.Flow:
                    return CalculateFlowTotalSize(items, maxSize);
                    
                default:
                    return CalculateVerticalTotalSize(items, maxSize);
            }
        }

        private Size CalculateVerticalTotalSize(List<SimpleItem> items, Size maxSize)
        {
            int totalHeight = ItemPadding.Vertical;
            int maxWidth = 0;
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemHeight = AutoSize ? (ItemMeasurer?.Invoke(item, g).Height ?? CalculateItemHeight_Fallback(item, g)) : ItemSize.Height;
                    int itemWidth = AutoSize ? (ItemMeasurer?.Invoke(item, g).Width ?? CalculateItemWidth_Fallback(item, g)) : ItemSize.Width;
                    
                    totalHeight += itemHeight + spacing;
                    maxWidth = Math.Max(maxWidth, itemWidth);
                }
            }

            totalHeight -= spacing; // Remove last spacing
            return new Size(maxWidth + scaledPadding.Horizontal, Math.Min(totalHeight, maxSize.Height));
        }

        private Size CalculateHorizontalTotalSize(List<SimpleItem> items, Size maxSize)
        {
            int totalWidth = ItemPadding.Horizontal;
            int maxHeight = 0;
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemWidth = AutoSize ? (ItemMeasurer?.Invoke(item, g).Width ?? CalculateItemWidth_Fallback(item, g)) : ItemSize.Width;
                    int itemHeight = AutoSize ? (ItemMeasurer?.Invoke(item, g).Height ?? CalculateItemHeight_Fallback(item, g)) : ItemSize.Height;
                    
                    totalWidth += itemWidth + spacing;
                    maxHeight = Math.Max(maxHeight, itemHeight);
                }
            }

            totalWidth -= spacing; // Remove last spacing
            return new Size(Math.Min(totalWidth, maxSize.Width), maxHeight + scaledPadding.Vertical);
        }

        private Size CalculateGridTotalSize(List<SimpleItem> items, Size maxSize)
        {
            int columns = Math.Max(1, ColumnCount);
            int rows = (int)Math.Ceiling((double)items.Count / columns);
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            
            int itemWidth;
            int itemHeight;
            using (Graphics g = _owner.CreateGraphics())
            {
                if (AutoSize)
                {
                    var measured = ItemMeasurer?.Invoke(items.FirstOrDefault(), g);
                    if (measured.HasValue && measured.Value.Width > 0 && measured.Value.Height > 0)
                    {
                        itemWidth = measured.Value.Width;
                        itemHeight = measured.Value.Height;
                    }
                    else
                    {
                        itemWidth = CalculateItemWidth_Fallback(items.FirstOrDefault(), g);
                        itemHeight = CalculateItemHeight_Fallback(items.FirstOrDefault(), g);
                    }
                }
                else
                {
                    itemWidth = DpiScalingHelper.ScaleValue(ItemSize.Width, _owner);
                    itemHeight = DpiScalingHelper.ScaleValue(ItemSize.Height, _owner);
                }
            }
            
            int totalWidth = scaledPadding.Horizontal + (columns * itemWidth) + ((columns - 1) * spacing);
            int totalHeight = scaledPadding.Vertical + (rows * itemHeight) + ((rows - 1) * spacing);
            
            return new Size(
                Math.Min(totalWidth, maxSize.Width),
                Math.Min(totalHeight, maxSize.Height)
            );
        }

        private Size CalculateFlowTotalSize(List<SimpleItem> items, Size maxSize)
        {
            // Simulate flow layout to calculate size
            int currentX = DpiScalingHelper.ScalePadding(ItemPadding, _owner).Left;
            int currentY = DpiScalingHelper.ScalePadding(ItemPadding, _owner).Top;
            int spacing = DpiScalingHelper.ScaleValue(ItemSpacing, _owner);
            var scaledPadding = DpiScalingHelper.ScalePadding(ItemPadding, _owner);
            int maxWidth = maxSize.Width - scaledPadding.Horizontal;
            int lineHeight = 0;
            int totalWidth = 0;

            using (Graphics g = _owner.CreateGraphics())
            {
                foreach (var item in items)
                {
                    int itemWidth = AutoSize ? (ItemMeasurer?.Invoke(item, g).Width ?? CalculateItemWidth_Fallback(item, g)) : ItemSize.Width;
                    int itemHeight = AutoSize ? (ItemMeasurer?.Invoke(item, g).Height ?? CalculateItemHeight_Fallback(item, g)) : ItemSize.Height;

                    // Check if we need to wrap
                    if (currentX + itemWidth > maxWidth && currentX > scaledPadding.Left)
                    {
                        totalWidth = Math.Max(totalWidth, currentX - spacing);
                        currentX = scaledPadding.Left;
                        currentY += lineHeight + spacing;
                        lineHeight = 0;
                    }

                    currentX += itemWidth + spacing;
                    lineHeight = Math.Max(lineHeight, itemHeight);
                }
            }

            totalWidth = Math.Max(totalWidth, currentX - spacing);
            int totalHeight = currentY + lineHeight + scaledPadding.Bottom;

            return new Size(totalWidth + scaledPadding.Horizontal, totalHeight);
        }
        #endregion

        #region Alignment Helpers
        /// <summary>
        /// Adjusts item rectangles based on alignment settings
        /// </summary>
        public void ApplyAlignment(List<Rectangle> itemRects, Rectangle containerRect)
        {
            if (itemRects == null || itemRects.Count == 0)
                return;

            switch (Alignment)
            {
                case RadioGroupAlignment.Center:
                    ApplyCenterAlignment(itemRects, containerRect);
                    break;
                case RadioGroupAlignment.Right:
                    ApplyRightAlignment(itemRects, containerRect);
                    break;
                case RadioGroupAlignment.Stretch:
                    ApplyStretchAlignment(itemRects, containerRect);
                    break;
                // Left alignment is default, no adjustment needed
            }
        }

        private void ApplyCenterAlignment(List<Rectangle> itemRects, Rectangle containerRect)
        {
            if (Orientation == RadioGroupOrientation.Vertical)
            {
                // Center each item horizontally
                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    int centerX = containerRect.X + (containerRect.Width - rect.Width) / 2;
                    itemRects[i] = new Rectangle(centerX, rect.Y, rect.Width, rect.Height);
                }
            }
            else if (Orientation == RadioGroupOrientation.Horizontal)
            {
                // Center the entire group horizontally
                int totalWidth = itemRects.Last().Right - itemRects.First().Left;
                int startX = containerRect.X + (containerRect.Width - totalWidth) / 2;
                int offset = startX - itemRects.First().Left;

                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    itemRects[i] = new Rectangle(rect.X + offset, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        private void ApplyRightAlignment(List<Rectangle> itemRects, Rectangle containerRect)
        {
            if (Orientation == RadioGroupOrientation.Vertical)
            {
                // Right-align each item
                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    int rightX = containerRect.Right - rect.Width - ItemPadding.Right;
                    itemRects[i] = new Rectangle(rightX, rect.Y, rect.Width, rect.Height);
                }
            }
            else if (Orientation == RadioGroupOrientation.Horizontal)
            {
                // Right-align the entire group
                int totalWidth = itemRects.Last().Right - itemRects.First().Left;
                int startX = containerRect.Right - totalWidth - ItemPadding.Right;
                int offset = startX - itemRects.First().Left;

                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    itemRects[i] = new Rectangle(rect.X + offset, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        private void ApplyStretchAlignment(List<Rectangle> itemRects, Rectangle containerRect)
        {
            if (Orientation == RadioGroupOrientation.Vertical)
            {
                // Stretch each item to fill width
                int stretchWidth = containerRect.Width - ItemPadding.Horizontal;
                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    itemRects[i] = new Rectangle(
                        containerRect.X + ItemPadding.Left,
                        rect.Y,
                        stretchWidth,
                        rect.Height
                    );
                }
            }
            else if (Orientation == RadioGroupOrientation.Horizontal)
            {
                // Distribute items evenly across width
                int availableWidth = containerRect.Width - ItemPadding.Horizontal;
                int totalSpacing = (itemRects.Count - 1) * ItemSpacing;
                int itemWidth = (availableWidth - totalSpacing) / itemRects.Count;

                for (int i = 0; i < itemRects.Count; i++)
                {
                    Rectangle rect = itemRects[i];
                    int x = containerRect.X + ItemPadding.Left + i * (itemWidth + ItemSpacing);
                    itemRects[i] = new Rectangle(x, rect.Y, itemWidth, rect.Height);
                }
            }
        }
        #endregion
    }

    #region Enums
    public enum RadioGroupOrientation
    {
        Vertical,
        Horizontal,
        Grid,
        Flow
    }

    public enum RadioGroupAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }
    #endregion
}