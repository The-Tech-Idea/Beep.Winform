using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for navigation chevron/angled buttons with diagonal cuts
    /// Creates modern UI navigation buttons with icon sections and angled edges
    /// Common uses: Menu navigation, action panels, dashboard sections
    /// </summary>
    public class NavigationChevronButtonPainter : BaseButtonPainter
    {
        /// <summary>
        /// Paint the chevron button with diagonal cuts and icon sections
        /// </summary>
        /// <param name="context">Paint context with button state and styling</param>
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Determine chevron style based on context
            ChevronStyle style = DetermineChevronStyle(context);

            // Draw shadow for elevation
            if (context.ShowShadow && context.State != AdvancedButtonState.Disabled)
            {
                DrawChevronShadow(g, buttonBounds, style, context.ShadowBlur, context.ShadowColor);
            }

            // Draw button based on chevron style
            switch (style)
            {
                case ChevronStyle.LeftIconRightChevron:
                    DrawLeftIconRightChevron(g, context, metrics, buttonBounds);
                    break;

                case ChevronStyle.LeftChevronRightIcon:
                    DrawLeftChevronRightIcon(g, context, metrics, buttonBounds);
                    break;

                case ChevronStyle.BothChevrons:
                    DrawBothChevrons(g, context, metrics, buttonBounds);
                    break;

                case ChevronStyle.LeftIconCenterChevron:
                    DrawLeftIconCenterChevron(g, context, metrics, buttonBounds);
                    break;

                default:
                    DrawLeftIconRightChevron(g, context, metrics, buttonBounds);
                    break;
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);
        }

        /// <summary>
        /// Determine which chevron style to use
        /// </summary>
        private ChevronStyle DetermineChevronStyle(AdvancedButtonPaintContext context)
        {
            // Use the chevron style from context
            if (context.ChevronStyle != ChevronStyle.LeftIconRightChevron)
            {
                return context.ChevronStyle;
            }

            // Auto-detect based on icon positions
            bool hasLeftIcon = !string.IsNullOrEmpty(context.IconLeft);
            bool hasRightIcon = !string.IsNullOrEmpty(context.IconRight);
            bool hasCenterIcon = context.ImagePainter != null && !string.IsNullOrEmpty(context.ImagePainter.ImagePath) && !hasLeftIcon && !hasRightIcon;

            if (hasLeftIcon && hasRightIcon)
            {
                return ChevronStyle.BothChevrons;
            }
            else if (hasCenterIcon)
            {
                return ChevronStyle.LeftIconCenterChevron;
            }
            else if (hasLeftIcon)
            {
                return ChevronStyle.LeftIconRightChevron;
            }
            else if (hasRightIcon)
            {
                return ChevronStyle.LeftChevronRightIcon;
            }

            return ChevronStyle.LeftIconRightChevron; // Default
        }

        #region "Chevron Style Renderers"

        /// <summary>
        /// Left icon section (white) + Main colored button with right chevron
        /// Example: Activate, Support, Home, Profile, Likes, Bookmark
        /// </summary>
        private void DrawLeftIconRightChevron(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 0.9); // Icon section width
            int chevronAngle = 20; // Diagonal cut width
            int separatorWidth = 30; // Small separator section

            // Calculate sections
            Rectangle iconSection = new Rectangle(bounds.X, bounds.Y, iconSectionWidth, bounds.Height);
            Rectangle separatorSection = new Rectangle(
                bounds.X + iconSectionWidth - chevronAngle / 2,
                bounds.Y,
                separatorWidth + chevronAngle,
                bounds.Height
            );
            Rectangle mainSection = new Rectangle(
                bounds.X + iconSectionWidth + separatorWidth,
                bounds.Y,
                bounds.Width - iconSectionWidth - separatorWidth,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : Color.White;
            Color separatorColor = Color.FromArgb(Math.Max(0, bgColor.R - 30), 
                Math.Max(0, bgColor.G - 30), Math.Max(0, bgColor.B - 30));

            // Draw left icon section (white/light background, straight edges)
            using (Brush iconBrush = new SolidBrush(iconBgColor))
            {
                g.FillRectangle(iconBrush, iconSection);
            }

            // Draw small diagonal separator
            using (GraphicsPath sepPath = new GraphicsPath())
            {
                sepPath.AddPolygon(new Point[]
                {
                    new Point(separatorSection.X, separatorSection.Y),
                    new Point(separatorSection.X + chevronAngle, separatorSection.Y),
                    new Point(separatorSection.X + chevronAngle + separatorWidth, separatorSection.Bottom),
                    new Point(separatorSection.X + separatorWidth, separatorSection.Bottom)
                });

                using (Brush sepBrush = new SolidBrush(separatorColor))
                {
                    g.FillPath(sepBrush, sepPath);
                }
            }

            // Draw main button section with right chevron
            using (GraphicsPath mainPath = new GraphicsPath())
            {
                mainPath.AddPolygon(new Point[]
                {
                    new Point(mainSection.X - chevronAngle, mainSection.Y),
                    new Point(mainSection.Right - chevronAngle, mainSection.Y),
                    new Point(mainSection.Right, mainSection.Bottom),
                    new Point(mainSection.X, mainSection.Bottom)
                });

                using (Brush mainBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw icon in left section (centered)
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    iconSection.X + (iconSection.Width - metrics.IconSize) / 2,
                    iconSection.Y + (iconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text in main section (centered, with padding)
            Rectangle textBounds = new Rectangle(
                mainSection.X + metrics.PaddingHorizontal,
                mainSection.Y,
                mainSection.Width - chevronAngle - metrics.PaddingHorizontal * 2,
                mainSection.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Left chevron + Main colored button + Right icon section (white)
        /// Example: Download, Settings, Share, Security, Volume, Overview
        /// </summary>
        private void DrawLeftChevronRightIcon(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 0.9); // Icon section width
            int chevronAngle = 20; // Diagonal cut width
            int separatorWidth = 30; // Small separator section

            // Calculate sections
            Rectangle mainSection = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width - iconSectionWidth - separatorWidth,
                bounds.Height
            );
            Rectangle separatorSection = new Rectangle(
                mainSection.Right - chevronAngle,
                bounds.Y,
                separatorWidth + chevronAngle,
                bounds.Height
            );
            Rectangle iconSection = new Rectangle(
                separatorSection.Right - chevronAngle / 2,
                bounds.Y,
                iconSectionWidth,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : Color.White;
            Color separatorColor = Color.FromArgb(Math.Max(0, bgColor.R - 30), 
                Math.Max(0, bgColor.G - 30), Math.Max(0, bgColor.B - 30));

            // Draw main button section with left chevron
            using (GraphicsPath mainPath = new GraphicsPath())
            {
                mainPath.AddPolygon(new Point[]
                {
                    new Point(mainSection.X + chevronAngle, mainSection.Y),
                    new Point(mainSection.Right + chevronAngle, mainSection.Y),
                    new Point(mainSection.Right, mainSection.Bottom),
                    new Point(mainSection.X, mainSection.Bottom)
                });

                using (Brush mainBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw small diagonal separator
            using (GraphicsPath sepPath = new GraphicsPath())
            {
                sepPath.AddPolygon(new Point[]
                {
                    new Point(separatorSection.X, separatorSection.Y),
                    new Point(separatorSection.X + chevronAngle + separatorWidth, separatorSection.Y),
                    new Point(separatorSection.X + separatorWidth, separatorSection.Bottom),
                    new Point(separatorSection.X - chevronAngle, separatorSection.Bottom)
                });

                using (Brush sepBrush = new SolidBrush(separatorColor))
                {
                    g.FillPath(sepBrush, sepPath);
                }
            }

            // Draw right icon section (white/light background, straight edges)
            using (Brush iconBrush = new SolidBrush(iconBgColor))
            {
                g.FillRectangle(iconBrush, iconSection);
            }

            // Draw text in main section (centered, with padding)
            Rectangle textBounds = new Rectangle(
                mainSection.X + chevronAngle + metrics.PaddingHorizontal,
                mainSection.Y,
                mainSection.Width - chevronAngle - metrics.PaddingHorizontal * 2,
                mainSection.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));

            // Draw icon in right section (centered)
            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconBounds = new Rectangle(
                    iconSection.X + (iconSection.Width - metrics.IconSize) / 2,
                    iconSection.Y + (iconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconRight);
            }
        }

        /// <summary>
        /// Left icon section + Main section + Right icon section (both white)
        /// For buttons with icons on both sides and center text
        /// </summary>
        private void DrawBothChevrons(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 0.85);

            // Draw left icon section (white)
            Rectangle leftIconSection = new Rectangle(bounds.X, bounds.Y, iconSectionWidth, bounds.Height);
            using (GraphicsPath leftIconPath = CreateLeftChevronPath(leftIconSection, isIconSection: true))
            {
                Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                    context.IconBackgroundColor : Color.White;
                
                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, leftIconPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, leftIconPath);
                }
            }

            // Draw main button section
            Rectangle mainSection = new Rectangle(
                bounds.X + iconSectionWidth - 15,
                bounds.Y,
                bounds.Width - (iconSectionWidth * 2) + 30,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            using (Brush mainBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(mainBrush, mainSection);
            }

            // Draw right icon section (white)
            Rectangle rightIconSection = new Rectangle(
                bounds.Right - iconSectionWidth,
                bounds.Y,
                iconSectionWidth,
                bounds.Height
            );

            using (GraphicsPath rightIconPath = CreateRightChevronPath(rightIconSection, isIconSection: true))
            {
                Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                    context.IconBackgroundColor : Color.White;
                
                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, rightIconPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, rightIconPath);
                }
            }

            // Draw left icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    leftIconSection.X + (leftIconSection.Width - 30 - metrics.IconSize) / 2,
                    leftIconSection.Y + (leftIconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text in center
            DrawText(g, context, mainSection, GetForegroundColor(context));

            // Draw right icon
            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconBounds = new Rectangle(
                    rightIconSection.X + (rightIconSection.Width - metrics.IconSize) / 2 + 15,
                    rightIconSection.Y + (rightIconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconRight);
            }
        }

        /// <summary>
        /// Left icon section + Center icon section (both white) + Colored background
        /// For buttons with a small left icon and a larger center icon/indicator
        /// </summary>
        private void DrawLeftIconCenterChevron(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int leftIconWidth = (int)(bounds.Height * 0.85);
            int centerIconWidth = (int)(bounds.Height * 0.75);

            // Draw left icon section (white)
            Rectangle leftIconSection = new Rectangle(bounds.X, bounds.Y, leftIconWidth, bounds.Height);
            using (GraphicsPath leftIconPath = CreateLeftChevronPath(leftIconSection, isIconSection: true))
            {
                using (Brush iconBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(iconBrush, leftIconPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, leftIconPath);
                }
            }

            // Draw main button section with right chevron
            Rectangle mainSection = new Rectangle(
                bounds.X + leftIconWidth - 15,
                bounds.Y,
                bounds.Width - leftIconWidth - centerIconWidth + 30,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            using (Brush mainBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(mainBrush, mainSection);
            }

            // Draw center icon section (white)
            Rectangle centerIconSection = new Rectangle(
                bounds.Right - centerIconWidth,
                bounds.Y,
                centerIconWidth,
                bounds.Height
            );

            using (GraphicsPath centerIconPath = CreateRightChevronPath(centerIconSection, isIconSection: true))
            {
                using (Brush iconBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(iconBrush, centerIconPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, centerIconPath);
                }
            }

            // Draw left icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    leftIconSection.X + (leftIconSection.Width - 30 - metrics.IconSize) / 2,
                    leftIconSection.Y + (leftIconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text in main section
            DrawText(g, context, mainSection, GetForegroundColor(context));

            // Draw center icon
            if (context.ImagePainter != null && !string.IsNullOrEmpty(context.ImagePainter.ImagePath))
            {
                Rectangle iconBounds = new Rectangle(
                    centerIconSection.X + (centerIconSection.Width - metrics.IconSize) / 2 + 10,
                    centerIconSection.Y + (centerIconSection.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
            }
        }

        #endregion

        #region "Helper Methods - Path Creation"

        /// <summary>
        /// Create left-pointing chevron path (angled left edge)
        /// </summary>
        private GraphicsPath CreateLeftChevronPath(Rectangle bounds, int chevronWidth = 0, bool isIconSection = false)
        {
            GraphicsPath path = new GraphicsPath();
            int angle = isIconSection ? 20 : (chevronWidth > 0 ? chevronWidth : 25);

            Point[] points = new Point[]
            {
                new Point(bounds.X + angle, bounds.Y),                    // Top left (angled)
                new Point(bounds.Right, bounds.Y),                        // Top right
                new Point(bounds.Right, bounds.Bottom),                   // Bottom right
                new Point(bounds.X + angle, bounds.Bottom),               // Bottom left (angled)
                new Point(bounds.X, bounds.Y + bounds.Height / 2)         // Left point (chevron tip)
            };

            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Create right-pointing chevron path (angled right edge)
        /// </summary>
        private GraphicsPath CreateRightChevronPath(Rectangle bounds, int chevronWidth = 0, bool isIconSection = false)
        {
            GraphicsPath path = new GraphicsPath();
            int angle = isIconSection ? 20 : (chevronWidth > 0 ? chevronWidth : 25);

            Point[] points = new Point[]
            {
                new Point(bounds.X, bounds.Y),                            // Top left
                new Point(bounds.Right - angle, bounds.Y),                // Top right (angled)
                new Point(bounds.Right, bounds.Y + bounds.Height / 2),    // Right point (chevron tip)
                new Point(bounds.Right - angle, bounds.Bottom),           // Bottom right (angled)
                new Point(bounds.X, bounds.Bottom)                        // Bottom left
            };

            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draw shadow for chevron buttons
        /// </summary>
        private void DrawChevronShadow(Graphics g, Rectangle bounds, ChevronStyle style, int blur, Color shadowColor)
        {
            // Offset shadow slightly
            Rectangle shadowBounds = new Rectangle(
                bounds.X + 2,
                bounds.Y + 3,
                bounds.Width,
                bounds.Height
            );

            using (GraphicsPath shadowPath = style == ChevronStyle.LeftChevronRightIcon ?
                CreateLeftChevronPath(shadowBounds) : CreateRightChevronPath(shadowBounds))
            {
                using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                {
                    shadowBrush.CenterColor = Color.FromArgb(40, shadowColor);
                    shadowBrush.SurroundColors = new Color[] { Color.FromArgb(0, shadowColor) };
                    shadowBrush.FocusScales = new PointF(0.8f, 0.8f);

                    g.FillPath(shadowBrush, shadowPath);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Chevron button layout styles
    /// </summary>
    public enum ChevronStyle
    {
        /// <summary>Left icon section (white) + Main colored section with right chevron</summary>
        LeftIconRightChevron,

        /// <summary>Left chevron + Main colored section + Right icon section (white)</summary>
        LeftChevronRightIcon,

        /// <summary>Both left and right icon sections (white) with center colored section</summary>
        BothChevrons,

        /// <summary>Left icon + Main section + Center icon section</summary>
        LeftIconCenterChevron
    }
}
