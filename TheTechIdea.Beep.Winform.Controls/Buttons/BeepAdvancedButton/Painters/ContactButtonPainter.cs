using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for contact/call-to-action buttons with icon sections
    /// Supports various layouts: icon-left, icon-right, icon-only, split backgrounds
    /// Common uses: Contact Us, Call Now, WhatsApp, Social media buttons
    /// </summary>
    public class ContactButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Determine button layout style
            ContactButtonLayout layout = DetermineLayout(context);

            // Draw shadow
            if (context.ShowShadow && context.State != AdvancedButtonState.Disabled)
            {
                DrawShadow(g, buttonBounds, context.BorderRadius, context.ShadowBlur, context.ShadowColor);
            }

            // Draw button based on layout style
            switch (layout)
            {
                // New numbered styles from image
                case ContactButtonLayout.IconCircleLeft1:
                    DrawIconCircleLeft1(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconSquareLeft2:
                    DrawIconSquareLeft2(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconCirclesLeftPill3:
                    DrawIconCirclesLeftPill3(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconCircleRightPill4:
                    DrawIconCircleRightPill4(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconDiagonalLeft5:
                    DrawIconDiagonalLeft5(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconSquareLeft6:
                    DrawIconSquareLeft6(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconArrowLeftPill7:
                    DrawIconArrowLeftPill7(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconInsidePillBorder8:
                    DrawIconInsidePillBorder8(g, context, metrics, buttonBounds);
                    break;

                // Legacy styles
                case ContactButtonLayout.IconLeftSplit:
                    DrawIconLeftSplitLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconRightSplit:
                    DrawIconRightSplitLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconLeftCircle:
                    DrawIconLeftCircleLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconRightCircle:
                    DrawIconRightCircleLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconLeftAngled:
                    DrawIconLeftAngledLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.IconLeftArrow:
                    DrawIconLeftArrowLayout(g, context, metrics, buttonBounds);
                    break;

                case ContactButtonLayout.Outlined:
                    DrawOutlinedLayout(g, context, metrics, buttonBounds);
                    break;

                default:
                    DrawStandardLayout(g, context, metrics, buttonBounds);
                    break;
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);
        }

        /// <summary>
        /// Determine which layout style to use based on context properties
        /// </summary>
        private ContactButtonLayout DetermineLayout(AdvancedButtonPaintContext context)
        {
            // Check for custom layout hint
            if (context.ContactLayout != null )
            {
                return context.ContactLayout;
            }

            // Auto-detect based on properties
            if (context.IsOutlined)
            {
                return ContactButtonLayout.Outlined;
            }

            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                if (context.IconCircleBackground)
                    return ContactButtonLayout.IconLeftCircle;
                if (context.IconAngledBackground)
                    return ContactButtonLayout.IconLeftAngled;
                if (context.IconArrowBackground)
                    return ContactButtonLayout.IconLeftArrow;
                return ContactButtonLayout.IconLeftSplit;
            }

            if (!string.IsNullOrEmpty(context.IconRight))
            {
                if (context.IconCircleBackground)
                    return ContactButtonLayout.IconRightCircle;
                return ContactButtonLayout.IconRightSplit;
            }

            return ContactButtonLayout.Standard;
        }

        #region "Layout Renderers"

        /// <summary>
        /// Icon on left with different background color (split button)
        /// </summary>
        private void DrawIconLeftSplitLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 0.9); // Square-ish icon section
            
            // Draw icon section background
            Rectangle iconSection = new Rectangle(bounds.X, bounds.Y, iconSectionWidth, bounds.Height);
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : DarkenColor(context.SolidBackground, 20);
            
            using (GraphicsPath iconPath = CreateRoundedRectanglePath(iconSection, context.BorderRadius, 
                roundTopLeft: true, roundBottomLeft: true))
            {
                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, iconPath);
                }
            }

            // Draw text section background
            Rectangle textSection = new Rectangle(bounds.X + iconSectionWidth, bounds.Y, 
                bounds.Width - iconSectionWidth, bounds.Height);
            Color textBgColor = GetBackgroundColor(context);
            
            using (GraphicsPath textPath = CreateRoundedRectanglePath(textSection, context.BorderRadius, 
                roundTopRight: true, roundBottomRight: true))
            {
                using (Brush textBrush = new SolidBrush(textBgColor))
                {
                    g.FillPath(textBrush, textPath);
                }
            }

            // Draw icon
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

            // Draw text
            DrawText(g, context, textSection, GetForegroundColor(context));
        }

        /// <summary>
        /// Icon on right with different background color
        /// </summary>
        private void DrawIconRightSplitLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 0.9);
            
            // Draw text section background
            Rectangle textSection = new Rectangle(bounds.X, bounds.Y, 
                bounds.Width - iconSectionWidth, bounds.Height);
            Color textBgColor = GetBackgroundColor(context);
            
            using (GraphicsPath textPath = CreateRoundedRectanglePath(textSection, context.BorderRadius, 
                roundTopLeft: true, roundBottomLeft: true))
            {
                using (Brush textBrush = new SolidBrush(textBgColor))
                {
                    g.FillPath(textBrush, textPath);
                }
            }

            // Draw icon section background
            Rectangle iconSection = new Rectangle(bounds.Right - iconSectionWidth, bounds.Y, 
                iconSectionWidth, bounds.Height);
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : DarkenColor(context.SolidBackground, 20);
            
            using (GraphicsPath iconPath = CreateRoundedRectanglePath(iconSection, context.BorderRadius, 
                roundTopRight: true, roundBottomRight: true))
            {
                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, iconPath);
                }
            }

            // Draw text
            DrawText(g, context, textSection, GetForegroundColor(context));

            // Draw icon
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
        /// Icon in circle on left, text on right
        /// </summary>
        private void DrawIconLeftCircleLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int circleSize = (int)(bounds.Height * 1.2); // Slightly larger than button height
            int circleOffset = -5; // Protrude left

            // Draw main button background
            using (Brush bgBrush = new SolidBrush(GetBackgroundColor(context)))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Draw icon circle (overlapping left edge)
            Rectangle circleBounds = new Rectangle(
                bounds.X + circleOffset, 
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize, 
                circleSize
            );
            
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : Color.White;

            using (Brush circleBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw circle border
            using (Pen circlePen = new Pen(context.SolidBackground, 3))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Draw icon in circle
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text (offset to avoid circle)
            Rectangle textBounds = new Rectangle(
                bounds.X + circleSize,
                bounds.Y,
                bounds.Width - circleSize,
                bounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Icon in circle on right, text on left
        /// </summary>
        private void DrawIconRightCircleLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int circleSize = (int)(bounds.Height * 1.2);
            int circleOffset = 5; // Protrude right

            // Draw main button background
            using (Brush bgBrush = new SolidBrush(GetBackgroundColor(context)))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Draw icon circle (overlapping right edge)
            Rectangle circleBounds = new Rectangle(
                bounds.Right - circleSize + circleOffset, 
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize, 
                circleSize
            );
            
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : DarkenColor(context.SolidBackground, 20);

            using (Brush circleBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw circle border
            using (Pen circlePen = new Pen(Color.White, 3))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Draw icon in circle
            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconRight);
            }

            // Draw text (offset to avoid circle)
            Rectangle textBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Width - circleSize - metrics.PaddingHorizontal,
                bounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Icon section with angled cut (trapezoid shape)
        /// </summary>
        private void DrawIconLeftAngledLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 1.2);
            int angleOffset = 20;

            // Draw main background
            using (Brush bgBrush = new SolidBrush(GetBackgroundColor(context)))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Draw angled icon section
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : DarkenColor(context.SolidBackground, 20);

            using (GraphicsPath angledPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + iconSectionWidth, bounds.Y),
                    new Point(bounds.X + iconSectionWidth + angleOffset, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                angledPath.AddPolygon(points);

                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, angledPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (iconSectionWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + iconSectionWidth + angleOffset + 10,
                bounds.Y,
                bounds.Width - iconSectionWidth - angleOffset - 10,
                bounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Icon section with arrow shape pointing right
        /// </summary>
        private void DrawIconLeftArrowLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSectionWidth = (int)(bounds.Height * 1.1);
            int arrowPoint = 15;

            // Draw text background
            using (Brush bgBrush = new SolidBrush(GetBackgroundColor(context)))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Draw arrow-shaped icon section
            Color iconBgColor = context.IconBackgroundColor != Color.Empty ? 
                context.IconBackgroundColor : Color.White;

            using (GraphicsPath arrowPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;
                
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + iconSectionWidth, bounds.Y),
                    new Point(bounds.X + iconSectionWidth + arrowPoint, midY),
                    new Point(bounds.X + iconSectionWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                arrowPath.AddPolygon(points);

                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, arrowPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (iconSectionWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + iconSectionWidth + arrowPoint + 10,
                bounds.Y,
                bounds.Width - iconSectionWidth - arrowPoint - 10,
                bounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Outlined button with icon and text
        /// </summary>
        private void DrawOutlinedLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            // Draw background (white or transparent)
            using (Brush bgBrush = new SolidBrush(Color.White))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Draw border
            Color borderColor = context.BorderColor != Color.Empty ? context.BorderColor : context.SolidBackground;
            using (Pen borderPen = new Pen(borderColor, context.BorderThickness))
            {
                ButtonShapeHelper.DrawShape(g, context.ButtonShape, bounds, context.BorderRadius, borderPen);
            }

            // Calculate layout for icon and text
            CalculateStandardLayout(context, metrics, bounds, out Rectangle iconBounds, out Rectangle textBounds);

            // Draw icon
            if (!string.IsNullOrEmpty(context.ImagePainter?.ImagePath) && !iconBounds.IsEmpty)
            {
                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
            }

            // Draw text
            DrawText(g, context, textBounds, context.SolidBackground);
        }

        /// <summary>
        /// Standard layout (solid background with icon and text)
        /// </summary>
        private void DrawStandardLayout(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            // Draw background
            using (Brush bgBrush = new SolidBrush(GetBackgroundColor(context)))
            {
                ButtonShapeHelper.FillShape(g, context.ButtonShape, bounds, context.BorderRadius, bgBrush);
            }

            // Calculate layout
            CalculateStandardLayout(context, metrics, bounds, out Rectangle iconBounds, out Rectangle textBounds);

            // Draw icon
            if (!string.IsNullOrEmpty(context.ImagePainter?.ImagePath) && !iconBounds.IsEmpty)
            {
                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
            }

            // Draw text
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        #endregion

        #region "Helper Methods"

        /// <summary>
        /// Calculate standard icon and text layout
        /// </summary>
        private void CalculateStandardLayout(AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics,
            Rectangle bounds, out Rectangle iconBounds, out Rectangle textBounds)
        {
            bool hasIcon = !string.IsNullOrEmpty(context.ImagePainter?.ImagePath);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                iconBounds = new Rectangle(
                    bounds.X + metrics.PaddingHorizontal,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );

                textBounds = new Rectangle(
                    iconBounds.Right + metrics.IconTextGap,
                    bounds.Y,
                    bounds.Width - iconBounds.Width - metrics.PaddingHorizontal * 2 - metrics.IconTextGap,
                    bounds.Height
                );
            }
            else if (hasIcon)
            {
                iconBounds = new Rectangle(
                    bounds.X + (bounds.Width - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                textBounds = Rectangle.Empty;
            }
            else
            {
                iconBounds = Rectangle.Empty;
                textBounds = new Rectangle(
                    bounds.X + metrics.PaddingHorizontal,
                    bounds.Y,
                    bounds.Width - metrics.PaddingHorizontal * 2,
                    bounds.Height
                );
            }

        }
        #region "New Numbered Contact Button Styles"

        /// <summary>
        /// Style 1: Icon circle on left + colored rectangle (top-left image)
        /// </summary>
        private void DrawIconCircleLeft1(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int circleSize = (int)(bounds.Height * 0.8);
            int circleOffset = 8;

            // Draw icon circle (white background, left side)
            Rectangle circleBounds = new Rectangle(
                bounds.X + circleOffset,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            Color iconBgColor = Color.White;
            using (Brush circleBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw main colored rectangle (rounded corners)
            Rectangle mainBounds = new Rectangle(
                bounds.X + circleSize / 2 + circleOffset,
                bounds.Y,
                bounds.Width - circleSize / 2 - circleOffset,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            using (GraphicsPath mainPath = CreateRoundedRectanglePath(mainBounds, context.BorderRadius,
                roundTopLeft: false, roundTopRight: true, roundBottomLeft: false, roundBottomRight: true))
            {
                using (Brush mainBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal + 20,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2 - 20,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Style 2: Icon square on left + colored rectangle (top-right image)
        /// </summary>
        private void DrawIconSquareLeft2(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int squareSize = (int)(bounds.Height * 0.75);

            // Draw icon square section (white background)
            Rectangle squareBounds = new Rectangle(bounds.X, bounds.Y, squareSize, bounds.Height);
            using (Brush squareBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(squareBrush, squareBounds);
            }

            // Draw main colored rectangle
            Rectangle mainBounds = new Rectangle(
                bounds.X + squareSize,
                bounds.Y,
                bounds.Width - squareSize,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            using (Brush mainBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    squareBounds.X + (squareBounds.Width - metrics.IconSize) / 2,
                    squareBounds.Y + (squareBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Style 3: Icon circles on left + colored pill (middle-left image)
        /// </summary>
        private void DrawIconCirclesLeftPill3(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int outerCircleSize = (int)(bounds.Height * 1.1);
            int innerCircleSize = (int)(bounds.Height * 0.85);

            // Draw outer circle (maroon/dark)
            Rectangle outerCircleBounds = new Rectangle(
                bounds.X - 10,
                bounds.Y - (outerCircleSize - bounds.Height) / 2,
                outerCircleSize,
                outerCircleSize
            );

            Color darkCircleColor = Color.FromArgb(120, 60, 80);
            using (Brush outerBrush = new SolidBrush(darkCircleColor))
            {
                g.FillEllipse(outerBrush, outerCircleBounds);
            }

            // Draw inner circle (white)
            Rectangle innerCircleBounds = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - innerCircleSize) / 2,
                innerCircleSize,
                innerCircleSize
            );

            using (Brush innerBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(innerBrush, innerCircleBounds);
            }

            // Draw main colored pill
            int pillLeft = bounds.X + innerCircleSize / 2;
            Rectangle pillBounds = new Rectangle(
                pillLeft,
                bounds.Y,
                bounds.Width - innerCircleSize / 2,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                pillPath.AddArc(pillBounds.Right - radius * 2, pillBounds.Y, radius * 2, pillBounds.Height, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X, pillBounds.Bottom);
                pillPath.AddLine(pillBounds.X, pillBounds.Bottom, pillBounds.X, pillBounds.Y);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    innerCircleBounds.X + (innerCircleBounds.Width - metrics.IconSize) / 2,
                    innerCircleBounds.Y + (innerCircleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal + 20,
                pillBounds.Y,
                pillBounds.Width - metrics.PaddingHorizontal * 2 - radius - 20,
                pillBounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Style 4: Colored pill + icon circle on right (middle-right image)
        /// </summary>
        private void DrawIconCircleRightPill4(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int circleSize = (int)(bounds.Height * 0.85);

            // Draw main colored pill (left-rounded)
            Rectangle pillBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width - circleSize / 2,
                bounds.Height
            );

            Color bgColor = Color.White;
            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                pillPath.AddArc(pillBounds.X, pillBounds.Y, radius * 2, pillBounds.Height, 90, 180);
                pillPath.AddLine(pillBounds.X + radius, pillBounds.Y, pillBounds.Right, pillBounds.Y);
                pillPath.AddLine(pillBounds.Right, pillBounds.Y, pillBounds.Right, pillBounds.Bottom);
                pillPath.AddLine(pillBounds.Right, pillBounds.Bottom, pillBounds.X + radius, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon circle (colored background, right side)
            Rectangle circleBounds = new Rectangle(
                bounds.Right - circleSize - 8,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            Color iconBgColor = GetBackgroundColor(context);
            using (Brush circleBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw text
            Color textColor = Color.FromArgb(100, 80, 120);
            Rectangle textBounds = new Rectangle(
                pillBounds.X + radius + metrics.PaddingHorizontal,
                pillBounds.Y,
                pillBounds.Width - radius - metrics.PaddingHorizontal * 2 - 20,
                pillBounds.Height
            );
            DrawText(g, context, textBounds, textColor);

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconRight);
            }
        }

        /// <summary>
        /// Style 5: Icon diagonal section + colored rectangle (bottom-left top)
        /// </summary>
        private void DrawIconDiagonalLeft5(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int diagonalWidth = (int)(bounds.Height * 0.9);
            int angleWidth = 20;

            // Draw icon diagonal section (colored)
            using (GraphicsPath iconPath = new GraphicsPath())
            {
                iconPath.AddPolygon(new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + diagonalWidth + angleWidth, bounds.Y),
                    new Point(bounds.X + diagonalWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                });

                Color iconSectionColor = GetBackgroundColor(context);
                using (Brush iconBrush = new SolidBrush(iconSectionColor))
                {
                    g.FillPath(iconBrush, iconPath);
                }
            }

            // Draw main white rectangle
            Rectangle mainBounds = new Rectangle(
                bounds.X + diagonalWidth + angleWidth - 5,
                bounds.Y,
                bounds.Width - diagonalWidth - angleWidth + 5,
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (diagonalWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Color textColor = Color.FromArgb(140, 80, 140);
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, textColor);
        }

        /// <summary>
        /// Style 6: Icon square + colored rectangle (bottom-left bottom)
        /// </summary>
        private void DrawIconSquareLeft6(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int squareSize = (int)(bounds.Height * 0.7);
            int squareOffset = 8;

            // Draw icon square section (white background with rounded corner)
            Rectangle squareBounds = new Rectangle(
                bounds.X + squareOffset,
                bounds.Y + (bounds.Height - squareSize) / 2,
                squareSize,
                squareSize
            );

            int cornerRadius = 8;
            using (GraphicsPath squarePath = CreateRoundedRectanglePath(squareBounds, cornerRadius,
                roundTopLeft: true, roundTopRight: false, roundBottomLeft: true, roundBottomRight: false))
            {
                using (Brush squareBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(squareBrush, squarePath);
                }
            }

            // Draw main colored pill (right-rounded)
            Rectangle pillBounds = new Rectangle(
                bounds.X + squareSize / 2 + squareOffset,
                bounds.Y,
                bounds.Width - squareSize / 2 - squareOffset,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                pillPath.AddLine(pillBounds.X, pillBounds.Y, pillBounds.Right - radius, pillBounds.Y);
                pillPath.AddArc(pillBounds.Right - radius * 2, pillBounds.Y, radius * 2, pillBounds.Height, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    squareBounds.X + (squareBounds.Width - metrics.IconSize) / 2,
                    squareBounds.Y + (squareBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal + 20,
                pillBounds.Y,
                pillBounds.Width - metrics.PaddingHorizontal * 2 - radius - 20,
                pillBounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Style 7: Icon arrow left + colored pill (bottom-right top)
        /// </summary>
        private void DrawIconArrowLeftPill7(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int arrowWidth = (int)(bounds.Height * 0.9);
            int arrowPoint = 15;

            // Draw icon arrow section (white background)
            using (GraphicsPath arrowPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;

                arrowPath.AddPolygon(new Point[]
                {
                    new Point(bounds.X + arrowPoint, bounds.Y),
                    new Point(bounds.X + arrowWidth + arrowPoint, bounds.Y),
                    new Point(bounds.X + arrowWidth, bounds.Bottom),
                    new Point(bounds.X, midY)
                });

                using (Brush arrowBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(arrowBrush, arrowPath);
                }
            }

            // Draw main colored pill
            Rectangle pillBounds = new Rectangle(
                bounds.X + arrowWidth,
                bounds.Y,
                bounds.Width - arrowWidth,
                bounds.Height
            );

            Color bgColor = GetBackgroundColor(context);
            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                pillPath.AddLine(pillBounds.X, pillBounds.Y, pillBounds.Right - radius, pillBounds.Y);
                pillPath.AddArc(pillBounds.Right - radius * 2, pillBounds.Y, radius * 2, pillBounds.Height, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (arrowWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal + 10,
                pillBounds.Y,
                pillBounds.Width - metrics.PaddingHorizontal * 2 - radius - 10,
                pillBounds.Height
            );
            DrawText(g, context, textBounds, GetForegroundColor(context));
        }

        /// <summary>
        /// Style 8: Colored pill with border + icon inside (bottom-right bottom)
        /// </summary>
        private void DrawIconInsidePillBorder8(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color bgColor = GetBackgroundColor(context);
            int radius = bounds.Height / 2;

            // Draw pill border
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                pillPath.AddArc(bounds.X, bounds.Y, radius * 2, bounds.Height, 90, 180);
                pillPath.AddLine(bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
                pillPath.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, bounds.Height, 270, 180);
                pillPath.AddLine(bounds.Right - radius, bounds.Bottom, bounds.X + radius, bounds.Bottom);
                pillPath.CloseFigure();

                // Draw border
                using (Pen borderPen = new Pen(bgColor, 3))
                {
                    g.DrawPath(borderPen, pillPath);
                }
            }

            // Calculate icon circle position (left side, inside pill)
            int circleSize = (int)(bounds.Height * 0.7);
            Rectangle circleBounds = new Rectangle(
                bounds.X + (bounds.Height - circleSize) / 2,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            // Draw icon circle
            using (Brush circleBrush = new SolidBrush(bgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                circleBounds.Right + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Width - circleBounds.Right - metrics.PaddingHorizontal - radius,
                bounds.Height
            );
            DrawText(g, context, textBounds, bgColor);
        }

        #endregion

        /// <summary>
        /// Create rounded rectangle path with selective corners
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius,
            bool roundTopLeft = false, bool roundTopRight = false, 
            bool roundBottomLeft = false, bool roundBottomRight = false)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // Top-left corner
            if (roundTopLeft)
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            else
                path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);

            // Top-right corner
            if (roundTopRight)
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            else
                path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);

            // Bottom-right corner
            if (roundBottomRight)
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            else
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);

            // Bottom-left corner
            if (roundBottomLeft)
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            else
                path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Darken a color by percentage
        /// </summary>
        private Color DarkenColor(Color color, int percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (color.R * percent / 100)),
                Math.Max(0, color.G - (color.G * percent / 100)),
                Math.Max(0, color.B - (color.B * percent / 100))
            );
        }

        #endregion
    }

    /// <summary>
    /// Contact button layout styles - matching design variations
    /// </summary>
    public enum ContactButtonLayout
    {
        /// <summary>Style 1: Icon circle on left + colored rectangle (top-left image)</summary>
        IconCircleLeft1,
        
        /// <summary>Style 2: Icon square on left + colored rectangle (top-right image)</summary>
        IconSquareLeft2,
        
        /// <summary>Style 3: Icon circles on left + colored pill (middle-left image)</summary>
        IconCirclesLeftPill3,
        
        /// <summary>Style 4: Colored pill + icon circle on right (middle-right image)</summary>
        IconCircleRightPill4,
        
        /// <summary>Style 5: Icon diagonal section + colored rectangle (bottom-left top)</summary>
        IconDiagonalLeft5,
        
        /// <summary>Style 6: Icon square + colored rectangle (bottom-left bottom)</summary>
        IconSquareLeft6,
        
        /// <summary>Style 7: Icon arrow left + colored pill (bottom-right top)</summary>
        IconArrowLeftPill7,
        
        /// <summary>Style 8: Colored pill with border + icon inside (bottom-right bottom)</summary>
        IconInsidePillBorder8,
        
        // Legacy/auto-detect styles
        Standard,
        IconLeftSplit,
        IconRightSplit,
        IconLeftCircle,
        IconRightCircle,
        IconLeftAngled,
        IconLeftArrow,
        Outlined
    }
}
