using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for login layout calculations and positioning
    /// </summary>
    public static class LoginLayoutHelpers
    {
        /// <summary>
        /// Gets the available container width accounting for padding
        /// </summary>
        public static int GetContainerWidth(Rectangle bounds, Padding padding)
        {
            return bounds.Width - padding.Left - padding.Right;
        }

        /// <summary>
        /// Gets the available container height accounting for padding
        /// </summary>
        public static int GetContainerHeight(Rectangle bounds, Padding padding)
        {
            return bounds.Height - padding.Top - padding.Bottom;
        }

        /// <summary>
        /// Gets standard spacing for a view type
        /// </summary>
        public static int GetSpacing(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Compact => 15,
                LoginViewType.Social => 15,
                LoginViewType.SocialView2 => 15,
                LoginViewType.Modern => 15,
                LoginViewType.Extended => 10,
                LoginViewType.Full => 10,
                _ => 10 // Simple, Minimal, Avatar
            };
        }

        /// <summary>
        /// Gets standard margin for a view type
        /// </summary>
        public static int GetMargin(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Compact => 15,
                LoginViewType.Social => 15,
                LoginViewType.SocialView2 => 15,
                LoginViewType.Modern => 15,
                _ => 10
            };
        }

        /// <summary>
        /// Gets standard size for a control type
        /// </summary>
        public static Size GetControlSize(string controlType, LoginViewType viewType)
        {
            return controlType.ToLowerInvariant() switch
            {
                "title" => new Size(0, 0), // Auto-size
                "subtitle" => new Size(0, 0), // Auto-size
                "username" => new Size(250, 30),
                "password" => new Size(250, 30),
                "loginbutton" => viewType == LoginViewType.SocialView2 ? new Size(150, 40) : new Size(100, 30),
                "socialbutton" => new Size(320, 40),
                "avatar" => new Size(60, 60),
                "logo" => new Size(60, 60),
                "checkbox" => new Size(0, 0), // Auto-size
                "link" => new Size(0, 0), // Auto-size
                _ => new Size(100, 30)
            };
        }

        /// <summary>
        /// Centers a control horizontally within the container
        /// </summary>
        public static Point CenterHorizontally(int containerWidth, int controlWidth, int y, Padding padding)
        {
            int x = padding.Left + (containerWidth - controlWidth) / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// Centers a control vertically within the container
        /// </summary>
        public static Point CenterVertically(int containerHeight, int controlHeight, int x, Padding padding)
        {
            int y = padding.Top + (containerHeight - controlHeight) / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// Centers a control both horizontally and vertically
        /// </summary>
        public static Point CenterControl(Rectangle containerBounds, Size controlSize, Padding padding)
        {
            int containerWidth = GetContainerWidth(containerBounds, padding);
            int containerHeight = GetContainerHeight(containerBounds, padding);
            
            int x = padding.Left + (containerWidth - controlSize.Width) / 2;
            int y = padding.Top + (containerHeight - controlSize.Height) / 2;
            
            return new Point(x, y);
        }

        /// <summary>
        /// Aligns a control to the left edge
        /// </summary>
        public static Point AlignLeft(int containerWidth, int controlWidth, int y, Padding padding, int margin = 0)
        {
            int x = padding.Left + margin;
            return new Point(x, y);
        }

        /// <summary>
        /// Aligns a control to the right edge
        /// </summary>
        public static Point AlignRight(int containerWidth, int controlWidth, int y, Padding padding, int margin = 0)
        {
            int x = padding.Left + containerWidth - controlWidth - margin;
            return new Point(x, y);
        }

        /// <summary>
        /// Distributes controls evenly across a row
        /// </summary>
        public static List<Point> DistributeControlsHorizontally(
            int containerWidth, 
            List<Size> controlSizes, 
            int y, 
            Padding padding, 
            int spacing = 10)
        {
            var positions = new List<Point>();
            
            if (controlSizes.Count == 0) return positions;

            // Calculate total width needed
            int totalWidth = 0;
            foreach (var size in controlSizes)
            {
                totalWidth += size.Width;
            }
            totalWidth += spacing * (controlSizes.Count - 1);

            // Calculate starting X to center the group
            int startX = padding.Left + (containerWidth - totalWidth) / 2;
            int currentX = startX;

            foreach (var size in controlSizes)
            {
                positions.Add(new Point(currentX, y));
                currentX += size.Width + spacing;
            }

            return positions;
        }

        /// <summary>
        /// Places two controls side by side (left and right)
        /// </summary>
        public static (Point left, Point right) PlaceSideBySide(
            int containerWidth,
            Size leftSize,
            Size rightSize,
            int y,
            Padding padding,
            int spacing = 10)
        {
            Point leftPos = new Point(padding.Left, y);
            Point rightPos = new Point(padding.Left + containerWidth - rightSize.Width, y);
            
            return (leftPos, rightPos);
        }

        /// <summary>
        /// Calculates the preferred size for a view type
        /// </summary>
        public static Size GetPreferredSize(LoginViewType viewType)
        {
            return LoginViewConfig.CreateForViewType(viewType).PreferredSize;
        }

        /// <summary>
        /// Gets the recommended padding for a view type
        /// </summary>
        public static Padding GetRecommendedPadding(LoginViewType viewType)
        {
            return LoginViewConfig.CreateForViewType(viewType).Padding;
        }

        /// <summary>
        /// Calculates metrics for a simple vertical stack layout
        /// </summary>
        public static void CalculateVerticalStack(
            LoginLayoutMetrics metrics,
            List<string> controlNames,
            Dictionary<string, Size> controlSizes,
            int spacing)
        {
            int currentY = metrics.ContainerPadding.Top + metrics.Margin;
            int containerWidth = metrics.ContainerWidth;

            foreach (var controlName in controlNames)
            {
                if (controlSizes.TryGetValue(controlName, out var size))
                {
                    int x = metrics.ContainerPadding.Left + (containerWidth - size.Width) / 2;
                    metrics.SetControlBounds(controlName, new Rectangle(x, currentY, size.Width, size.Height));
                    currentY += size.Height + spacing;
                }
            }
        }
    }
}

