using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            Color rippleColor = context.RippleColor == Color.Empty ? Color.White : context.RippleColor;
            
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

            return context.ImagePainter?.ImagePath ?? string.Empty;
        }

        protected int MeasureContextTextWidth(AdvancedButtonPaintContext context, string? text = null)
        {
            return BeepAdvancedButtonHelper.MeasureTextWidth(text ?? context.Text ?? string.Empty, context.TextFont);
        }

        protected Font GetDerivedTextFont(AdvancedButtonPaintContext context, float sizeScale = 1f, FontStyle? styleOverride = null, float sizeDelta = 0f)
        {
            return GetDerivedTextFont(context.TextFont, sizeScale, styleOverride, sizeDelta);
        }

        protected static Font GetDerivedTextFont(Font baseFont, float sizeScale = 1f, FontStyle? styleOverride = null, float sizeDelta = 0f)
        {
            var source = baseFont ?? SystemFonts.DefaultFont;
            FontStyle style = styleOverride ?? source.Style;
            float targetSize = Math.Max(6f, (source.Size * sizeScale) + sizeDelta);

            var typography = new TypographyStyle
            {
                FontFamily = source.FontFamily.Name,
                FontSize = targetSize,
                FontStyle = style,
                FontWeight = style.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Regular,
                IsUnderlined = style.HasFlag(FontStyle.Underline),
                IsStrikeout = style.HasFlag(FontStyle.Strikeout)
            };

            // Base font is already resolved for control DPI in most cases, avoid double scaling here.
            return BeepThemesManager.ToFont(typography, applyDpiScaling: false);
        }

        protected void DrawText(Graphics g, AdvancedButtonPaintContext context, Rectangle textBounds, Color textColor)
        {
            if (string.IsNullOrEmpty(context.Text)) return;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(context.Text, context.TextFont, textBrush, textBounds, sf);
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
    }
}
