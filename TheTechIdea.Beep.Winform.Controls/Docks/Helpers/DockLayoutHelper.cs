using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper class for calculating dock item layouts and magnification effects
    /// Handles positioning, spacing, and scale calculations for various dock styles
    /// </summary>
    public static class DockLayoutHelper
    {
        /// <summary>
        /// Calculates the layout rectangles for all dock items
        /// </summary>
        /// <param name="dockBounds">The bounds of the entire dock</param>
        /// <param name="items">Collection of dock items</param>
        /// <param name="config">Dock configuration</param>
        /// <param name="hoverIndex">Index of the hovered item (-1 if none)</param>
        /// <param name="hoverProgress">Animation progress for hover effect (0-1)</param>
        /// <returns>Array of rectangles for each item</returns>
        public static Rectangle[] CalculateItemBounds(
            Rectangle dockBounds, 
            IList<SimpleItem> items, 
            DockConfig config, 
            int hoverIndex, 
            float hoverProgress)
        {
            if (items == null || items.Count == 0)
                return Array.Empty<Rectangle>();

            var bounds = new Rectangle[items.Count];
            
            if (config.Orientation == DockOrientation.Horizontal)
            {
                CalculateHorizontalLayout(dockBounds, items, config, hoverIndex, hoverProgress, bounds);
            }
            else
            {
                CalculateVerticalLayout(dockBounds, items, config, hoverIndex, hoverProgress, bounds);
            }

            return bounds;
        }

        /// <summary>
        /// Calculates horizontal layout with magnification effect
        /// </summary>
        private static void CalculateHorizontalLayout(
            Rectangle dockBounds, 
            IList<SimpleItem> items, 
            DockConfig config, 
            int hoverIndex, 
            float hoverProgress,
            Rectangle[] bounds)
        {
            int baseSize = config.ItemSize;
            int spacing = config.Spacing;
            float maxScale = config.MaxScale;

            // Calculate scales for each item based on hover
            float[] scales = CalculateMagnificationScales(items.Count, hoverIndex, hoverProgress, maxScale);

            // Calculate total width needed
            float totalWidth = 0;
            for (int i = 0; i < items.Count; i++)
            {
                totalWidth += baseSize * scales[i];
                if (i < items.Count - 1)
                    totalWidth += spacing;
            }

            // Calculate starting X position based on alignment
            float startX = CalculateStartPosition(dockBounds.Width, totalWidth, config.Alignment, config.Padding);
            float currentX = dockBounds.X + startX;

            // Position each item
            for (int i = 0; i < items.Count; i++)
            {
                float itemSize = baseSize * scales[i];
                int x = (int)currentX;
                int y = dockBounds.Y + (dockBounds.Height - (int)itemSize) / 2;
                int w = (int)itemSize;
                int h = (int)itemSize;

                bounds[i] = new Rectangle(x, y, w, h);
                currentX += itemSize + spacing;
            }
        }

        /// <summary>
        /// Calculates vertical layout with magnification effect
        /// </summary>
        private static void CalculateVerticalLayout(
            Rectangle dockBounds, 
            IList<SimpleItem> items, 
            DockConfig config, 
            int hoverIndex, 
            float hoverProgress,
            Rectangle[] bounds)
        {
            int baseSize = config.ItemSize;
            int spacing = config.Spacing;
            float maxScale = config.MaxScale;

            // Calculate scales for each item based on hover
            float[] scales = CalculateMagnificationScales(items.Count, hoverIndex, hoverProgress, maxScale);

            // Calculate total height needed
            float totalHeight = 0;
            for (int i = 0; i < items.Count; i++)
            {
                totalHeight += baseSize * scales[i];
                if (i < items.Count - 1)
                    totalHeight += spacing;
            }

            // Calculate starting Y position based on alignment
            float startY = CalculateStartPosition(dockBounds.Height, totalHeight, config.Alignment, config.Padding);
            float currentY = dockBounds.Y + startY;

            // Position each item
            for (int i = 0; i < items.Count; i++)
            {
                float itemSize = baseSize * scales[i];
                int x = dockBounds.X + (dockBounds.Width - (int)itemSize) / 2;
                int y = (int)currentY;
                int w = (int)itemSize;
                int h = (int)itemSize;

                bounds[i] = new Rectangle(x, y, w, h);
                currentY += itemSize + spacing;
            }
        }

        /// <summary>
        /// Calculates magnification scales for all items based on hover position
        /// Uses a parabolic curve for smooth magnification falloff
        /// </summary>
        /// <param name="itemCount">Total number of items</param>
        /// <param name="hoverIndex">Index of hovered item</param>
        /// <param name="hoverProgress">Animation progress (0-1)</param>
        /// <param name="maxScale">Maximum scale factor</param>
        /// <returns>Array of scale values for each item</returns>
        public static float[] CalculateMagnificationScales(int itemCount, int hoverIndex, float hoverProgress, float maxScale)
        {
            float[] scales = new float[itemCount];

            if (hoverIndex < 0 || hoverIndex >= itemCount || hoverProgress <= 0)
            {
                // No magnification
                for (int i = 0; i < itemCount; i++)
                    scales[i] = 1.0f;
                return scales;
            }

            // Apple dock-style magnification curve
            // Items closer to hover get more scale
            const int influenceRange = 2; // Number of items affected on each side

            for (int i = 0; i < itemCount; i++)
            {
                int distance = Math.Abs(i - hoverIndex);
                
                if (distance == 0)
                {
                    // Hovered item gets max scale
                    scales[i] = 1.0f + (maxScale - 1.0f) * hoverProgress;
                }
                else if (distance <= influenceRange)
                {
                    // Adjacent items get partial scale based on distance
                    float falloff = 1.0f - (float)distance / (influenceRange + 1);
                    float scaleAmount = (maxScale - 1.0f) * falloff * hoverProgress;
                    scales[i] = 1.0f + scaleAmount;
                }
                else
                {
                    // Items outside influence range stay normal
                    scales[i] = 1.0f;
                }
            }

            return scales;
        }

        /// <summary>
        /// Calculates the starting position based on alignment
        /// </summary>
        private static float CalculateStartPosition(float containerSize, float contentSize, DockAlignment alignment, int padding)
        {
            switch (alignment)
            {
                case DockAlignment.Start:
                    return padding;

                case DockAlignment.Center:
                    return (containerSize - contentSize) / 2;

                case DockAlignment.End:
                    return containerSize - contentSize - padding;

                case DockAlignment.SpaceBetween:
                case DockAlignment.SpaceAround:
                case DockAlignment.SpaceEvenly:
                    // For flex-box style alignments, start from padding
                    return padding;

                default:
                    return (containerSize - contentSize) / 2;
            }
        }

        /// <summary>
        /// Calculates spacing between items for flex-box style alignments
        /// </summary>
        public static float CalculateFlexSpacing(float containerSize, float totalItemSize, int itemCount, DockAlignment alignment, int basePadding)
        {
            if (itemCount <= 1)
                return 0;

            float availableSpace = containerSize - totalItemSize - (basePadding * 2);

            switch (alignment)
            {
                case DockAlignment.SpaceBetween:
                    return availableSpace / (itemCount - 1);

                case DockAlignment.SpaceAround:
                    return availableSpace / itemCount;

                case DockAlignment.SpaceEvenly:
                    return availableSpace / (itemCount + 1);

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculates the ideal dock size based on items and config
        /// </summary>
        public static Size CalculateDockSize(int itemCount, DockConfig config)
        {
            if (itemCount == 0)
                return new Size(100, config.DockHeight);

            int baseSize = config.ItemSize;
            int spacing = config.Spacing;
            int padding = config.Padding;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                int width = (baseSize * itemCount) + (spacing * (itemCount - 1)) + (padding * 2);
                return new Size(width, config.DockHeight);
            }
            else
            {
                int height = (baseSize * itemCount) + (spacing * (itemCount - 1)) + (padding * 2);
                return new Size(config.DockHeight, height);
            }
        }

        /// <summary>
        /// Calculates the expanded dock size with maximum magnification
        /// Used for auto-hiding and bounds calculation
        /// </summary>
        public static Size CalculateExpandedDockSize(int itemCount, DockConfig config)
        {
            if (itemCount == 0)
                return CalculateDockSize(0, config);

            int baseSize = config.ItemSize;
            int expandedSize = (int)(baseSize * config.MaxScale);
            int spacing = config.Spacing;
            int padding = config.Padding;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                // Account for magnified center item and affected neighbors
                int width = (expandedSize) + (baseSize * (itemCount - 1)) + (spacing * (itemCount - 1)) + (padding * 2);
                int height = Math.Max(config.DockHeight, expandedSize + padding * 2);
                return new Size(width, height);
            }
            else
            {
                int height = (expandedSize) + (baseSize * (itemCount - 1)) + (spacing * (itemCount - 1)) + (padding * 2);
                int width = Math.Max(config.DockHeight, expandedSize + padding * 2);
                return new Size(width, height);
            }
        }

        /// <summary>
        /// Calculates smooth easing for animations
        /// </summary>
        public static float EaseOutCubic(float t)
        {
            return 1 - (float)Math.Pow(1 - t, 3);
        }

        /// <summary>
        /// Calculates smooth easing with spring physics
        /// </summary>
        public static float EaseOutElastic(float t)
        {
            const float c4 = (2 * (float)Math.PI) / 3;
            return t == 0 ? 0 : t == 1 ? 1 : (float)(Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1);
        }
    }
}
