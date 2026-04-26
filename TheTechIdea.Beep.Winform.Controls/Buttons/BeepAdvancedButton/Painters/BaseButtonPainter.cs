using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Base painter with common painting functionality
    /// </summary>
    public abstract class BaseButtonPainter : IAdvancedButtonPainter
    {
        public abstract void Paint(AdvancedButtonPaintContext context);

        #region "Protected Helper Methods"

        protected GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top left
            path.AddArc(arc, 180, 90);
            
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        protected void DrawShadow(Graphics g, Rectangle bounds, int radius, int shadowBlur, Color shadowColor)
        {
            if (shadowBlur <= 0) return;

            // Draw multiple layers for blur effect
            for (int i = shadowBlur; i > 0; i--)
            {
                int alpha = (int)((shadowColor.A / (float)shadowBlur) * (shadowBlur - i + 1));
                Color layerColor = Color.FromArgb(alpha, shadowColor);
                
                Rectangle shadowBounds = new Rectangle(
                    bounds.X + i / 2,
                    bounds.Y + i / 2,
                    bounds.Width,
                    bounds.Height
                );

                using (GraphicsPath path = GetRoundedRectanglePath(shadowBounds, radius))
                using (Brush brush = new SolidBrush(layerColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        protected void FillRoundedRectangle(Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = GetRoundedRectanglePath(bounds, radius))
            {
                g.FillPath(brush, path);
            }
        }

        protected void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = GetRoundedRectanglePath(bounds, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        protected void DrawRippleEffect(Graphics g, AdvancedButtonPaintContext context)
        {
            if (!context.RippleActive) return;

            // Calculate ripple radius
            float maxDistance = (float)Math.Sqrt(
                Math.Pow(context.Bounds.Width, 2) + 
                Math.Pow(context.Bounds.Height, 2)
            );
            float rippleRadius = maxDistance * context.RippleProgress;
            
            // Calculate alpha based on progress (fade out as it expands)
            int alpha = (int)(Math.Clamp(1f - context.RippleProgress, 0f, 1f) * 80);
            Color rippleColor = context.RippleColor != Color.Empty ? context.RippleColor : Color.FromArgb(255, 255, 255);
            
            GraphicsState state = g.Save();
            try
            {
                using GraphicsPath clipPath = GetRoundedRectanglePath(context.Bounds, context.BorderRadius);
                g.SetClip(clipPath, CombineMode.Intersect);

                using Brush rippleBrush = new SolidBrush(Color.FromArgb(alpha, rippleColor));
                g.FillEllipse(rippleBrush,
                    context.RippleCenter.X - rippleRadius,
                    context.RippleCenter.Y - rippleRadius,
                    rippleRadius * 2,
                    rippleRadius * 2
                );
            }
            finally
            {
                g.Restore(state);
            }
        }

        protected void DrawIcon(Graphics g, AdvancedButtonPaintContext context, Rectangle iconBounds, string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath)) return;

            using (GraphicsPath iconPathGeometry = iconBounds.ToGraphicsPath())
            {
                StyledImagePainter.PaintWithTint(g, iconPathGeometry, iconPath, GetForegroundColor(context));
            }
        }

        protected static bool HasPrimaryIcon(AdvancedButtonPaintContext context)
        {
            return !string.IsNullOrWhiteSpace(GetPrimaryIconPath(context));
        }

        protected static string GetPrimaryIconPath(AdvancedButtonPaintContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.ImagePath))
            {
                return context.ImagePath;
            }

            return context.IconLeft ?? string.Empty;
        }

        protected int MeasureContextTextWidth(AdvancedButtonPaintContext context, string? text = null)
        {
            return BeepAdvancedButtonHelper.MeasureTextWidth(text ?? context.Text ?? string.Empty, context.TextFont);
        }

        protected Font GetDerivedTextFont(AdvancedButtonPaintContext context, float sizeScale = 1f, FontStyle? styleOverride = null, float sizeDelta = 0f)
        {
            return GetDerivedTextFont(context.TextFont, sizeScale, styleOverride, sizeDelta);
        }

        protected static Font GetDerivedTextFont(Font? baseFont, float sizeScale = 1f, FontStyle? styleOverride = null, float sizeDelta = 0f)
        {
            // Use BeepFontManager.DefaultFont as the safe fallback
            var defaultFont = BeepFontManager.ToFont(BeepThemesManager.CurrentTheme.ButtonFont);

            // Safely extract properties from baseFont, fallback to defaults
            string fontFamily;
            float fontSize;
            FontStyle fontStyle;

            try
            {
                fontFamily = baseFont?.FontFamily?.Name ?? defaultFont.FontFamily.Name;
                fontSize = baseFont?.Size > 0 ? baseFont.Size : defaultFont.Size;
                fontStyle = styleOverride ?? baseFont?.Style ?? defaultFont.Style;
            }
            catch
            {
                // If any property access fails, use defaults
                fontFamily = defaultFont.FontFamily.Name;
                fontSize = defaultFont.Size;
                fontStyle = styleOverride ?? defaultFont.Style;
            }

            float targetSize = Math.Max(6f, (fontSize * sizeScale) + sizeDelta);

            var typography = new TypographyStyle
            {
                FontFamily = fontFamily,
                FontSize = targetSize,
                FontStyle = fontStyle,
                FontWeight = fontStyle.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Regular,
                IsUnderlined = fontStyle.HasFlag(FontStyle.Underline),
                IsStrikeout = fontStyle.HasFlag(FontStyle.Strikeout)
            };

            return BeepThemesManager.ToFont(typography, applyDpiScaling: false);
        }

        protected void DrawText(Graphics g, AdvancedButtonPaintContext context, Rectangle textBounds, Color textColor)
        {
            if (string.IsNullOrEmpty(context.Text)) return;

            var safeFont = context.TextFont ?? SystemFonts.DefaultFont;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(context.Text, safeFont, textBrush, textBounds, sf);
                }
            }
        }

        protected void DrawLoadingSpinner(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds, Color color)
        {
            DrawLoadingSpinner(g, bounds, color, context.ReduceMotion ? 0f : context.LoadingRotationAngle);
        }

        protected void DrawLoadingSpinner(Graphics g, Rectangle bounds, Color color)
        {
            DrawLoadingSpinner(g, bounds, color, 0f);
        }

        private static void DrawLoadingSpinner(Graphics g, Rectangle bounds, Color color, float startAngle)
        {
            // Simple rotating spinner
            int spinnerSize = Math.Min(bounds.Width, bounds.Height) / 2;
            Rectangle spinnerBounds = new Rectangle(
                bounds.X + (bounds.Width - spinnerSize) / 2,
                bounds.Y + (bounds.Height - spinnerSize) / 2,
                spinnerSize,
                spinnerSize
            );

            using (Pen spinnerPen = new Pen(color, 2))
            {
                spinnerPen.StartCap = LineCap.Round;
                spinnerPen.EndCap = LineCap.Round;
                
                g.DrawArc(spinnerPen, spinnerBounds, startAngle, 270);
            }
        }

        protected Color GetBackgroundColor(AdvancedButtonPaintContext context)
        {
            if (context.State == AdvancedButtonState.Disabled)
            {
                return context.DisabledBackground;
            }

            // Unified transition pipeline: pressed takes precedence over hover.
            if (context.PressProgress > 0f)
            {
                return Blend(context.SolidBackground, context.PressedBackground, context.PressProgress);
            }

            if (context.HoverProgress > 0f)
            {
                return Blend(context.SolidBackground, context.HoverBackground, context.HoverProgress);
            }

            return context.State switch
            {
                AdvancedButtonState.Pressed => context.PressedBackground,
                AdvancedButtonState.Hover => context.HoverBackground,
                _ => context.SolidBackground
            };
        }

        protected Color GetForegroundColor(AdvancedButtonPaintContext context)
        {
            if (context.State == AdvancedButtonState.Disabled)
            {
                return context.DisabledForeground;
            }

            if (context.PressProgress > 0f)
            {
                return Blend(context.SolidForeground, context.SolidForeground, context.PressProgress);
            }

            if (context.HoverProgress > 0f)
            {
                return Blend(context.SolidForeground, context.HoverForeground, context.HoverProgress);
            }

            return context.State switch
            {
                AdvancedButtonState.Pressed => context.SolidForeground,
                AdvancedButtonState.Hover => context.HoverForeground,
                _ => context.SolidForeground
            };
        }

        protected AdvancedButtonMetrics GetMetrics(AdvancedButtonPaintContext context)
        {
            return AdvancedButtonMetrics.GetMetrics(context.ButtonSize, context.OwnerControl);
        }

        protected void DrawFocusRingPrimitive(Graphics g, AdvancedButtonPaintContext context)
        {
            if (!context.ShowFocusRing || context.State != AdvancedButtonState.Focused || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
            {
                return;
            }

            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle ringBounds = context.Bounds;
            ringBounds.Inflate(-tokens.FocusRingOffset, -tokens.FocusRingOffset);
            if (ringBounds.Width <= 0 || ringBounds.Height <= 0)
            {
                return;
            }

            using GraphicsPath ringPath = ButtonShapeHelper.CreateShapePath(context.Shape == ButtonShape.Split ? ButtonShape.RoundedRectangle : context.Shape, ringBounds, Math.Max(0, tokens.BorderRadius + tokens.FocusRingRadiusDelta));
            using Pen focusPen = new Pen(context.FocusRingColor, tokens.FocusRingThickness)
            {
                Alignment = PenAlignment.Inset
            };
            g.DrawPath(focusPen, ringPath);
        }

        protected static Color Blend(Color from, Color to, float amount)
        {
            float t = Math.Clamp(amount, 0f, 1f);
            int a = (int)(from.A + (to.A - from.A) * t);
            int r = (int)(from.R + (to.R - from.R) * t);
            int g = (int)(from.G + (to.G - from.G) * t);
            int b = (int)(from.B + (to.B - from.B) * t);
            return Color.FromArgb(a, r, g, b);
        }

        #endregion

        #region "Fallback Icon Drawing Methods"

        /// <summary>
        /// Standard red color for LIVE badges in news banners
        /// </summary>
        protected static readonly Color LiveBadgeRed = Color.FromArgb(220, 30, 30);

        /// <summary>
        /// Draws a globe/world icon using the embedded SVG, or falls back to a simple circle with lines
        /// </summary>
        protected void DrawFallbackGlobeIcon(Graphics g, Rectangle bounds, Color iconColor)
        {
            // Try to draw the SVG icon first
            try
            {
                using (GraphicsPath iconPathGeometry = bounds.ToGraphicsPath())
                {
                    StyledImagePainter.PaintWithTint(g, iconPathGeometry, SvgsUI.World, iconColor);
                }
            }
            catch
            {
                // Fallback: draw simple globe wireframe
                DrawSimpleGlobe(g, bounds, iconColor);
            }
        }

        /// <summary>
        /// Draws a lightning bolt icon using the embedded SVG, or falls back to a simple bolt shape
        /// </summary>
        protected void DrawFallbackLightningIcon(Graphics g, Rectangle bounds, Color iconColor)
        {
            try
            {
                using (GraphicsPath iconPathGeometry = bounds.ToGraphicsPath())
                {
                    StyledImagePainter.PaintWithTint(g, iconPathGeometry, SvgsUI.Bolt, iconColor);
                }
            }
            catch
            {
                // Fallback: draw simple lightning bolt
                DrawSimpleLightningBolt(g, bounds, iconColor);
            }
        }

        /// <summary>
        /// Draws a soccer ball icon for sport news using the embedded SVG
        /// </summary>
        protected void DrawFallbackSportIcon(Graphics g, Rectangle bounds, Color iconColor)
        {
            try
            {
                using (GraphicsPath iconPathGeometry = bounds.ToGraphicsPath())
                {
                    StyledImagePainter.PaintWithTint(g, iconPathGeometry, SvgsSports.SoccerBall, iconColor);
                }
            }
            catch
            {
                // Fallback: draw simple soccer ball shape
                DrawSimpleSoccerBall(g, bounds, iconColor);
            }
        }

        /// <summary>
        /// Draws a warning/alert icon for fake news using the embedded SVG
        /// </summary>
        protected void DrawFallbackAlertIcon(Graphics g, Rectangle bounds, Color iconColor)
        {
            try
            {
                using (GraphicsPath iconPathGeometry = bounds.ToGraphicsPath())
                {
                    StyledImagePainter.PaintWithTint(g, iconPathGeometry, SvgsUI.AlertTriangle, iconColor);
                }
            }
            catch
            {
                // Fallback: draw simple warning triangle
                DrawSimpleWarningTriangle(g, bounds, iconColor);
            }
        }

        /// <summary>
        /// Draws a standard red LIVE badge rectangle with text
        /// </summary>
        protected void DrawLiveBadge(Graphics g, Rectangle bounds, Font baseFont, bool useRedBackground = true)
        {
            Color bgColor = useRedBackground ? LiveBadgeRed : Color.White;
            Color textColor = useRedBackground ? Color.White : LiveBadgeRed;

            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            using (Font liveFont = GetDerivedTextFont(baseFont, sizeScale: 0.7f, styleOverride: FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(textColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("LIVE", liveFont, textBrush, bounds, format);
            }
        }

        /// <summary>
        /// Draws a LIVE badge with a dot indicator (● LIVE)
        /// </summary>
        protected void DrawLiveBadgeWithDot(Graphics g, Rectangle bounds, Font baseFont, Color badgeColor, bool useWhiteBackground = true)
        {
            Color bgColor = useWhiteBackground ? Color.White : badgeColor;
            Color textColor = useWhiteBackground ? badgeColor : Color.White;

            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            using (Font liveFont = GetDerivedTextFont(baseFont, sizeScale: 0.6f, styleOverride: FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(textColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("● LIVE", liveFont, textBrush, bounds, format);
            }
        }

        #region "Simple Shape Fallbacks"

        private void DrawSimpleGlobe(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 1.5f))
            {
                // Outer circle
                g.DrawEllipse(pen, bounds);

                // Horizontal line
                int midY = bounds.Y + bounds.Height / 2;
                g.DrawLine(pen, bounds.X, midY, bounds.Right, midY);

                // Vertical ellipse (meridian)
                int inset = bounds.Width / 4;
                Rectangle meridian = new Rectangle(bounds.X + inset, bounds.Y, bounds.Width - inset * 2, bounds.Height);
                g.DrawEllipse(pen, meridian);
            }
        }

        private void DrawSimpleLightningBolt(Graphics g, Rectangle bounds, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            using (GraphicsPath path = new GraphicsPath())
            {
                float x = bounds.X;
                float y = bounds.Y;
                float w = bounds.Width;
                float h = bounds.Height;

                // Lightning bolt shape
                path.AddPolygon(new PointF[]
                {
                    new PointF(x + w * 0.6f, y),
                    new PointF(x + w * 0.3f, y + h * 0.45f),
                    new PointF(x + w * 0.55f, y + h * 0.45f),
                    new PointF(x + w * 0.4f, y + h),
                    new PointF(x + w * 0.7f, y + h * 0.55f),
                    new PointF(x + w * 0.45f, y + h * 0.55f)
                });
                g.FillPath(brush, path);
            }
        }

        private void DrawSimpleSoccerBall(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 1.5f))
            using (Brush brush = new SolidBrush(color))
            {
                // Outer circle
                g.DrawEllipse(pen, bounds);

                // Draw pentagon pattern (simplified)
                int cx = bounds.X + bounds.Width / 2;
                int cy = bounds.Y + bounds.Height / 2;
                int r = bounds.Width / 5;

                // Center pentagon
                PointF[] pentagon = new PointF[5];
                for (int i = 0; i < 5; i++)
                {
                    double angle = -Math.PI / 2 + i * 2 * Math.PI / 5;
                    pentagon[i] = new PointF(
                        cx + (float)(r * Math.Cos(angle)),
                        cy + (float)(r * Math.Sin(angle))
                    );
                }
                g.FillPolygon(brush, pentagon);
            }
        }

        private void DrawSimpleWarningTriangle(Graphics g, Rectangle bounds, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            using (GraphicsPath path = new GraphicsPath())
            {
                float x = bounds.X;
                float y = bounds.Y;
                float w = bounds.Width;
                float h = bounds.Height;

                path.AddPolygon(new PointF[]
                {
                    new PointF(x + w / 2, y),
                    new PointF(x + w, y + h),
                    new PointF(x, y + h)
                });
                g.FillPath(brush, path);

                // Draw exclamation mark in center
                using (Brush whiteBrush = new SolidBrush(Color.White))
                {
                    Rectangle exclamation = new Rectangle((int)(x + w * 0.45), (int)(y + h * 0.35), (int)(w * 0.1), (int)(h * 0.35));
                    g.FillRectangle(whiteBrush, exclamation);

                    Rectangle dot = new Rectangle((int)(x + w * 0.45), (int)(y + h * 0.75), (int)(w * 0.1), (int)(w * 0.1));
                    g.FillEllipse(whiteBrush, dot);
                }
            }
        }

        #endregion

        #endregion
    }
}
