using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Renders loading skeleton screens for BeepGridPro
    /// Provides visual feedback during data loading operations
    /// </summary>
    public static class SkeletonRenderer
    {
        private static DateTime _animationStart = DateTime.Now;
        
        /// <summary>
        /// Renders a complete skeleton screen for the grid
        /// </summary>
        public static void Render(Graphics g, Rectangle gridRect, BeepGridPro grid)
        {
            if (grid == null) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Render header skeleton
            if (grid.ShowColumnHeaders && grid.Layout != null)
            {
                RenderHeaderSkeleton(g, grid.Layout.ColumnsHeaderRect, grid);
            }
            
            // Render navigation skeleton (if visible)
            // Note: Adjust these based on actual GridLayoutHelper properties
            // if (grid.ShowNavigator && grid.Layout != null)
            // {
            //     RenderNavigationSkeleton(g, grid.Layout.NavigatorRect, grid);
            // }
            
            // Render row skeletons
            // Note: Use ClientRectangle or calculate data area
            var dataRect = new Rectangle(gridRect.X, gridRect.Y + (grid.ShowColumnHeaders ? grid.ColumnHeaderHeight : 0), 
                                        gridRect.Width, gridRect.Height - (grid.ShowColumnHeaders ? grid.ColumnHeaderHeight : 0));
            RenderRowSkeletons(g, dataRect, grid);
        }
        
        /// <summary>
        /// Renders skeleton with animated shimmer effect
        /// </summary>
        public static void RenderWithShimmer(Graphics g, Rectangle gridRect, BeepGridPro grid)
        {
            // Render base skeleton
            Render(g, gridRect, grid);
            
            // Calculate shimmer animation progress (2-second cycle)
            float animationProgress = ((DateTime.Now - _animationStart).Milliseconds % 2000) / 2000f;
            
            // Apply shimmer overlay
            RenderShimmerEffect(g, gridRect, animationProgress);
        }
        
        /// <summary>
        /// Renders header skeleton
        /// </summary>
        private static void RenderHeaderSkeleton(Graphics g, Rectangle headerRect, BeepGridPro grid)
        {
            if (headerRect.IsEmpty) return;
            
            var columns = grid.Data?.Columns?.Where(c => c.Visible).ToList();
            if (columns == null || !columns.Any()) return;
            
            int x = headerRect.X;
            
            foreach (var column in columns)
            {
                var cellRect = new Rectangle(x, headerRect.Y, column.Width, headerRect.Height);
                
                // Draw skeleton bar
                int barWidth = Math.Min(cellRect.Width - 16, 120);
                var skeletonRect = new Rectangle(
                    cellRect.X + 8,
                    cellRect.Y + (cellRect.Height - 12) / 2,
                    barWidth,
                    12
                );
                
                using (var path = CreateRoundedPath(skeletonRect, 6))
                using (var brush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                {
                    g.FillPath(brush, path);
                }
                
                x += column.Width;
            }
        }
        
        /// <summary>
        /// Renders navigation skeleton
        /// </summary>
        private static void RenderNavigationSkeleton(Graphics g, Rectangle navRect, BeepGridPro grid)
        {
            if (navRect.IsEmpty) return;
            
            // Draw skeleton bars for navigation elements
            var elements = new[]
            {
                new Rectangle(navRect.X + 10, navRect.Y + (navRect.Height - 20) / 2, 60, 20),  // First
                new Rectangle(navRect.X + 80, navRect.Y + (navRect.Height - 20) / 2, 60, 20),  // Previous
                new Rectangle(navRect.X + 150, navRect.Y + (navRect.Height - 24) / 2, 100, 24), // Page info
                new Rectangle(navRect.X + 260, navRect.Y + (navRect.Height - 20) / 2, 60, 20),  // Next
                new Rectangle(navRect.X + 330, navRect.Y + (navRect.Height - 20) / 2, 60, 20)   // Last
            };
            
            foreach (var element in elements)
            {
                using (var path = CreateRoundedPath(element, 4))
                using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                {
                    g.FillPath(brush, path);
                }
            }
        }
        
        /// <summary>
        /// Renders row skeletons
        /// </summary>
        private static void RenderRowSkeletons(Graphics g, Rectangle dataRect, BeepGridPro grid)
        {
            if (dataRect.IsEmpty) return;
            
            int rowHeight = grid.RowHeight;
            int y = dataRect.Y;
            int rowCount = Math.Min(dataRect.Height / rowHeight, 20); // Max 20 skeleton rows
            
            for (int i = 0; i < rowCount; i++)
            {
                RenderRowSkeleton(g, new Rectangle(dataRect.X, y, dataRect.Width, rowHeight), grid, i);
                y += rowHeight;
            }
        }
        
        /// <summary>
        /// Renders a single row skeleton
        /// </summary>
        private static void RenderRowSkeleton(Graphics g, Rectangle rowRect, BeepGridPro grid, int rowIndex)
        {
            var columns = grid.Data?.Columns?.Where(c => c.Visible).ToList();
            if (columns == null || !columns.Any()) return;
            
            int x = rowRect.X;
            
            // Use row index for pseudo-random variation
            var random = new Random(rowIndex * 12345);
            
            foreach (var column in columns)
            {
                var cellRect = new Rectangle(x, rowRect.Y, column.Width, rowRect.Height);
                
                // Randomize skeleton bar width for variety (50-80% of cell width)
                int barWidth = (int)(cellRect.Width * (0.5 + random.NextDouble() * 0.3));
                barWidth = Math.Min(barWidth, cellRect.Width - 16);
                
                if (barWidth > 0)
                {
                    var skeletonRect = new Rectangle(
                        cellRect.X + 8,
                        cellRect.Y + (cellRect.Height - 10) / 2,
                        barWidth,
                        10
                    );
                    
                    using (var path = CreateRoundedPath(skeletonRect, 5))
                    using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                    {
                        g.FillPath(brush, path);
                    }
                }
                
                x += column.Width;
            }
        }
        
        /// <summary>
        /// Renders animated shimmer effect over skeleton
        /// </summary>
        private static void RenderShimmerEffect(Graphics g, Rectangle gridRect, float animationProgress)
        {
            // Calculate shimmer position
            int shimmerWidth = 150;
            int shimmerX = (int)(gridRect.X - shimmerWidth + (gridRect.Width + shimmerWidth * 2) * animationProgress);
            
            var shimmerRect = new Rectangle(shimmerX - shimmerWidth / 2, gridRect.Y, shimmerWidth, gridRect.Height);
            
            // Clip to grid bounds
            var originalClip = g.Clip;
            g.SetClip(gridRect);
            
            // Draw shimmer gradient
            using (var brush = new LinearGradientBrush(
                shimmerRect,
                Color.FromArgb(0, 255, 255, 255),
                Color.FromArgb(60, 255, 255, 255),
                LinearGradientMode.Horizontal))
            {
                // Create multi-stop gradient for shimmer effect
                var blend = new ColorBlend(3);
                blend.Colors = new[] 
                { 
                    Color.FromArgb(0, 255, 255, 255),
                    Color.FromArgb(60, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255)
                };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                brush.InterpolationColors = blend;
                
                g.FillRectangle(brush, shimmerRect);
            }
            
            // Restore clip
            g.Clip = originalClip;
        }
        
        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0 || bounds.Width < radius * 2 || bounds.Height < radius * 2)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            int diameter = radius * 2;
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
            
            // Top-left
            path.AddArc(arc, 180, 90);
            
            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
        
        /// <summary>
        /// Resets the shimmer animation timing
        /// </summary>
        public static void ResetShimmer()
        {
            _animationStart = DateTime.Now;
        }
        
        /// <summary>
        /// Renders skeleton for a specific area
        /// </summary>
        public static void RenderArea(Graphics g, Rectangle area, int rowCount, int columnCount, int rowHeight = 40)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            int columnWidth = area.Width / Math.Max(1, columnCount);
            int y = area.Y;
            
            for (int row = 0; row < rowCount; row++)
            {
                int x = area.X;
                
                for (int col = 0; col < columnCount; col++)
                {
                    var cellRect = new Rectangle(x, y, columnWidth, rowHeight);
                    
                    // Random bar width
                    var random = new Random((row * columnCount + col) * 54321);
                    int barWidth = (int)(cellRect.Width * (0.5 + random.NextDouble() * 0.3));
                    barWidth = Math.Min(barWidth, cellRect.Width - 16);
                    
                    if (barWidth > 0)
                    {
                        var skeletonRect = new Rectangle(
                            cellRect.X + 8,
                            cellRect.Y + (cellRect.Height - 10) / 2,
                            barWidth,
                            10
                        );
                        
                        using (var path = CreateRoundedPath(skeletonRect, 5))
                        using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    
                    x += columnWidth;
                }
                
                y += rowHeight;
            }
        }
    }
}

