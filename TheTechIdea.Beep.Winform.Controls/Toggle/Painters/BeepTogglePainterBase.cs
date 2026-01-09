using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Base class for toggle painters
    /// Provides common painting logic, layout calculation, and hit testing for each style
    /// </summary>
    internal abstract class BeepTogglePainterBase
    {
        protected readonly BeepToggle Owner;
        protected readonly BeepToggleLayoutHelper Layout;

        // Layout regions for hit testing
        protected Rectangle TrackRegion;
        protected Rectangle ThumbRegion;
        protected Rectangle OnLabelRegion;
        protected Rectangle OffLabelRegion;
        protected Rectangle IconRegion;

        protected BeepTogglePainterBase(BeepToggle owner, BeepToggleLayoutHelper layout)
        {
            Owner = owner;
            Layout = layout;
        }

        #region Layout Calculation (Painter-Specific)

        /// <summary>
        /// Calculate layout for this specific painter style
        /// Each painter implements its own layout logic
        /// </summary>
        public abstract void CalculateLayout(Rectangle bounds);

        #endregion

        #region Hit Testing

        /// <summary>
        /// Test if point is within the toggle track
        /// </summary>
        public virtual bool HitTestTrack(Point point)
        {
            return TrackRegion.Contains(point);
        }

        /// <summary>
        /// Test if point is within the toggle thumb
        /// </summary>
        public virtual bool HitTestThumb(Point point)
        {
            return ThumbRegion.Contains(point);
        }

        /// <summary>
        /// Test if point is within any interactive region
        /// </summary>
        public virtual bool HitTest(Point point)
        {
            return HitTestTrack(point) || HitTestThumb(point);
        }

        /// <summary>
        /// Get the region being hovered (for different visual feedback)
        /// </summary>
        public virtual ToggleHitRegion GetHitRegion(Point point)
        {
            if (ThumbRegion.Contains(point))
                return ToggleHitRegion.Thumb;
            
            if (OnLabelRegion.Contains(point))
                return ToggleHitRegion.OnLabel;
            
            if (OffLabelRegion.Contains(point))
                return ToggleHitRegion.OffLabel;
            
            if (IconRegion.Contains(point))
                return ToggleHitRegion.Icon;
            
            if (TrackRegion.Contains(point))
                return ToggleHitRegion.Track;
            
            return ToggleHitRegion.None;
        }

        #endregion

        #region Painting

        /// <summary>
        /// Main paint method - calculates layout then calls specific painting methods
        /// </summary>
        public virtual void Paint(Graphics g, Rectangle bounds, ControlState state)
        {
            // Calculate layout first (painter-specific)
            CalculateLayout(bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            PaintTrack(g, TrackRegion, state);
            PaintLabels(g, state);
            PaintThumb(g, ThumbRegion, state);
            PaintIcons(g, state);
        }

        /// <summary>
        /// Paint the track (background)
        /// </summary>
        protected abstract void PaintTrack(Graphics g, Rectangle trackRect, ControlState state);

        /// <summary>
        /// Paint the thumb (slider/button)
        /// </summary>
        protected abstract void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state);

        /// <summary>
        /// Paint labels (ON/OFF text)
        /// </summary>
        protected virtual void PaintLabels(Graphics g, ControlState state)
        {
            // Override in specific painters if labels are needed
        }

        /// <summary>
        /// Paint icons
        /// </summary>
        protected virtual void PaintIcons(Graphics g, ControlState state)
        {
            // Override in specific painters if icons are needed
        }

        #endregion

        #region Helper Methods

        protected Color GetTrackColor(ControlState state)
        {
            // Use ToggleThemeHelpers for theme-aware colors
            var theme = Owner._currentTheme;
            var useThemeColors = Owner.UseThemeColors;

            Color baseColor = ToggleThemeHelpers.GetToggleTrackColor(
                theme,
                useThemeColors,
                Owner.IsOn,
                Owner.OnColor,
                Owner.OffColor);

            // Apply high contrast adjustments if enabled
            if (ToggleAccessibilityHelpers.IsHighContrastMode())
            {
                var (onColor, offColor, thumbColor, textColor) = ToggleAccessibilityHelpers.GetHighContrastColors();
                baseColor = Owner.IsOn ? onColor : offColor;
            }

            return state switch
            {
                ControlState.Hover => LightenColor(baseColor, 0.1f),
                ControlState.Pressed => DarkenColor(baseColor, 0.1f),
                ControlState.Disabled => Color.FromArgb(180, baseColor),
                _ => baseColor
            };
        }

        protected Color GetThumbColor(ControlState state)
        {
            // Use ToggleThemeHelpers for theme-aware colors
            var theme = Owner._currentTheme;
            var useThemeColors = Owner.UseThemeColors;

            Color? customOnColor = Owner.OnThumbColor != Color.White ? Owner.OnThumbColor : null;
            Color? customOffColor = Owner.OffThumbColor != Color.White ? Owner.OffThumbColor : null;

            Color baseColor = ToggleThemeHelpers.GetToggleThumbColor(
                theme,
                useThemeColors,
                Owner.IsOn,
                customOnColor,
                customOffColor);

            // Apply high contrast adjustments if enabled
            if (ToggleAccessibilityHelpers.IsHighContrastMode())
            {
                var (onColor, offColor, thumbColor, textColor) = ToggleAccessibilityHelpers.GetHighContrastColors();
                baseColor = thumbColor;
            }

            return state switch
            {
                ControlState.Hover => LightenColor(baseColor, 0.05f),
                ControlState.Pressed => DarkenColor(baseColor, 0.05f),
                ControlState.Disabled => Color.FromArgb(180, baseColor),
                _ => baseColor
            };
        }

        protected Color GetTextColor(ControlState state, bool isOn)
        {
            // Use ToggleThemeHelpers for theme-aware text colors
            var theme = Owner._currentTheme;
            var useThemeColors = Owner.UseThemeColors;

            Color baseColor = ToggleThemeHelpers.GetToggleTextColor(theme, useThemeColors, isOn);

            // Apply high contrast adjustments if enabled
            if (ToggleAccessibilityHelpers.IsHighContrastMode())
            {
                var (onColor, offColor, thumbColor, textColor) = ToggleAccessibilityHelpers.GetHighContrastColors();
                baseColor = textColor;
            }

            return state switch
            {
                ControlState.Disabled => Color.FromArgb(180, baseColor),
                _ => baseColor
            };
        }

        protected Color LightenColor(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * amount)),
                Math.Min(255, (int)(color.G + (255 - color.G) * amount)),
                Math.Min(255, (int)(color.B + (255 - color.B) * amount))
            );
        }

        protected Color DarkenColor(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - amount))),
                Math.Max(0, (int)(color.G * (1 - amount))),
                Math.Max(0, (int)(color.B * (1 - amount)))
            );
        }

        protected void DrawCenteredText(Graphics g, string text, Font font, Color color, Rectangle rect)
        {
            if (string.IsNullOrEmpty(text))
                return;

            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(text, font, brush, rect, format);
            }
        }

        /// <summary>
        /// Gets label font using ToggleFontHelpers
        /// Integrates with BeepFontManager and ControlStyle
        /// </summary>
        protected Font GetLabelFont(bool isOn)
        {
            return ToggleFontHelpers.GetLabelFont(
                Owner,
                Owner.ToggleStyle,
                Owner.ControlStyle,
                isOn);
        }

        /// <summary>
        /// Gets button font using ToggleFontHelpers
        /// </summary>
        protected Font GetButtonFont(bool isActive)
        {
            return ToggleFontHelpers.GetButtonFont(
                Owner,
                Owner.ToggleStyle,
                Owner.ControlStyle,
                isActive);
        }

        /// <summary>
        /// Paints icon using ToggleIconHelpers
        /// Integrates with StyledImagePainter and theme system
        /// </summary>
        protected void PaintIcon(
            Graphics g,
            Rectangle iconBounds,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (iconBounds.IsEmpty || Owner == null)
                return;

            ToggleIconHelpers.PaintIcon(
                g,
                iconBounds,
                Owner,
                Owner.ToggleStyle,
                isOn,
                state,
                theme,
                useThemeColors,
                Owner.ControlStyle);
        }

        /// <summary>
        /// Paints icon in circle using ToggleIconHelpers
        /// </summary>
        protected void PaintIconInCircle(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (Owner == null || radius <= 0)
                return;

            ToggleIconHelpers.PaintIconInCircle(
                g,
                centerX,
                centerY,
                radius,
                Owner,
                Owner.ToggleStyle,
                isOn,
                state,
                theme,
                useThemeColors);
        }

        /// <summary>
        /// Gets icon path using ToggleIconHelpers
        /// </summary>
        protected string GetIconPath(bool isOn)
        {
            if (Owner == null)
                return null;

            return ToggleIconHelpers.GetIconPath(
                Owner.ToggleStyle,
                isOn,
                Owner.OnIconPath,
                Owner.OffIconPath);
        }

        /// <summary>
        /// Calculates icon bounds within thumb using ToggleIconHelpers
        /// </summary>
        protected Rectangle CalculateIconBounds(Rectangle thumbBounds)
        {
            if (Owner == null || thumbBounds.IsEmpty)
                return Rectangle.Empty;

            return ToggleIconHelpers.CalculateIconBounds(
                thumbBounds,
                Owner,
                Owner.ToggleStyle);
        }

        protected GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Gets border radius using ToggleStyleHelpers
        /// </summary>
        protected int GetBorderRadius()
        {
            return ToggleStyleHelpers.GetBorderRadius(Owner.ToggleStyle, Owner.ControlStyle);
        }

        /// <summary>
        /// Gets track shape using ToggleStyleHelpers
        /// </summary>
        protected ToggleTrackShape GetTrackShape()
        {
            return ToggleStyleHelpers.GetTrackShape(Owner.ToggleStyle);
        }

        /// <summary>
        /// Gets thumb shape using ToggleStyleHelpers
        /// </summary>
        protected ToggleThumbShape GetThumbShape()
        {
            return ToggleStyleHelpers.GetThumbShape(Owner.ToggleStyle);
        }

        /// <summary>
        /// Should show shadow using ToggleStyleHelpers
        /// </summary>
        protected bool ShouldShowShadow()
        {
            return ToggleStyleHelpers.ShouldShowShadow(Owner.ToggleStyle, Owner.ControlStyle);
        }

        /// <summary>
        /// Gets shadow color using ToggleStyleHelpers
        /// </summary>
        protected Color GetShadowColor(bool isOn)
        {
            var theme = Owner._currentTheme;
            var useThemeColors = Owner.UseThemeColors;
            return ToggleStyleHelpers.GetShadowColor(theme, useThemeColors, isOn);
        }

        /// <summary>
        /// Gets shadow offset using ToggleStyleHelpers
        /// </summary>
        protected Point GetShadowOffset()
        {
            return ToggleStyleHelpers.GetShadowOffset(Owner.ToggleStyle, Owner.ControlStyle);
        }

        #endregion
    }
}
