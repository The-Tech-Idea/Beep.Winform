using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Base class for dock painters providing common functionality
    /// </summary>
    public abstract class DockPainterBase : IDockPainter
    {
        #region Metrics

        /// <summary>
        /// Gets the metrics for this painter. Override to provide custom metrics.
        /// </summary>
        public virtual DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            return DockPainterMetrics.DefaultFor(config.Style, theme, useThemeColors);
        }

        /// <summary>
        /// Metrics in device pixels, scaled by the control's DPI.
        /// </summary>
        /// <remarks>
        /// The scale comes from <see cref="DockConfig.DpiScale"/>, which <c>BeepDock</c> sets from
        /// <c>Control.DeviceDpi</c>. It used to come from <c>DpiScalingHelper.GetDpiScaleFactor(g)</c>
        /// - the overload that helper warns produces incorrect values and should only be used when no
        /// Control is available. It was correct in an offscreen bitmap and wrong on screen, which is
        /// the most expensive kind of wrong: a harness rendering to bitmaps confirms it works.
        /// The <paramref name="g"/> parameter is kept only as a fallback for callers that paint
        /// without a configured control.
        /// </remarks>
        protected DockPainterMetrics GetScaledMetrics(DockConfig config, IBeepTheme theme, Graphics g, bool useThemeColors = true)
        {
            var metrics = GetMetrics(config, theme, useThemeColors);
            var dpiScale = config.DpiScale > 0f && !DpiScalingHelper.AreScaleFactorsEqual(config.DpiScale, 1.0f)
                ? config.DpiScale
                : DpiScalingHelper.GetDpiScaleFactor(g);
            if (!DpiScalingHelper.AreScaleFactorsEqual(dpiScale, 1.0f))
            {
                metrics.ItemSize = DpiScalingHelper.ScaleValue(metrics.ItemSize, dpiScale);
                metrics.ItemSpacing = DpiScalingHelper.ScaleValue(metrics.ItemSpacing, dpiScale);
                metrics.ItemPadding = DpiScalingHelper.ScaleValue(metrics.ItemPadding, dpiScale);
                metrics.CornerRadius = DpiScalingHelper.ScaleValue(metrics.CornerRadius, dpiScale);
                metrics.ItemCornerRadius = DpiScalingHelper.ScaleValue(metrics.ItemCornerRadius, dpiScale);
                metrics.BorderWidth = DpiScalingHelper.ScaleValue(metrics.BorderWidth, dpiScale);
                metrics.IndicatorSize = DpiScalingHelper.ScaleValue(metrics.IndicatorSize, dpiScale);
                metrics.IndicatorOffset = DpiScalingHelper.ScaleValue(metrics.IndicatorOffset, dpiScale);
                metrics.ShadowBlur = DpiScalingHelper.ScaleValue(metrics.ShadowBlur, dpiScale);
                metrics.GlowBlur = DpiScalingHelper.ScaleValue(metrics.GlowBlur, dpiScale);
                metrics.SeparatorWidth = DpiScalingHelper.ScaleValue(metrics.SeparatorWidth, dpiScale);
            }

            return metrics;
        }

        #endregion

        #region Colour resolution

        // A painter declares its style's colours here and never assigns them to DockConfig. The
        // config is shared with the control and outlives any one paint, so a write persists: the
        // first style to paint used to fill in the config's nullable colours, every later style's
        // ??= became a no-op, and switching style stopped changing the background for the life of
        // the control. Declaring the default and resolving it at read time cannot do that.
        //
        // Resolution order is the one DockThemeHelpers already documents - user colour, then theme,
        // then the style's palette - with one deliberate exception, IsNamedPalette below.

        /// <summary>
        /// True when the style is named after a colour scheme, so its palette is the reason the user
        /// chose it. A Dracula dock rendered in the ambient theme's colours is not a Dracula dock, so
        /// these painters skip the theme step and keep their palette even when
        /// <see cref="DockConfig.UseThemeColors"/> is on. Styles named after a platform or a design
        /// system (Apple, Windows 11, Material 3, GNOME, Plasma...) are theme-led and leave this
        /// false; their palette is the fallback for when the theme has no opinion.
        /// </summary>
        protected virtual bool IsNamedPalette => false;

        /// <summary>The style's own background colour. Null means the style has no opinion.</summary>
        protected virtual Color? StyleBackgroundColor => null;

        /// <summary>The style's own border colour. Null means the style has no opinion.</summary>
        protected virtual Color? StyleBorderColor => null;

        /// <summary>The style's own background opacity. Null means the style has no opinion.</summary>
        protected virtual float? StyleBackgroundOpacity => null;

        /// <summary>The style's own hover colour. Null means the style has no opinion.</summary>
        protected virtual Color? StyleHoverColor => null;

        /// <summary>The style's own selection colour. Null means the style has no opinion.</summary>
        protected virtual Color? StyleSelectedColor => null;

        /// <summary>
        /// Hover colour: the user's, else the theme's, else the style's - through
        /// <see cref="DockThemeHelpers"/>, so it matches how every other Beep control resolves.
        /// </summary>
        protected Color ResolveHoverColor(DockConfig config, IBeepTheme theme)
        {
            var hover = DockThemeHelpers.GetDockItemHoverColor(
                theme, config.UseThemeColors && !IsNamedPalette, config.HoverColor ?? StyleHoverColor);
            return HighContrast.Selection(hover);
        }

        /// <summary>Selection colour, same precedence as <see cref="ResolveHoverColor"/>.</summary>
        protected Color ResolveSelectedColor(DockConfig config, IBeepTheme theme)
        {
            var selected = DockThemeHelpers.GetDockItemSelectedColor(
                theme, config.UseThemeColors && !IsNamedPalette, config.SelectedColor ?? StyleSelectedColor);
            return HighContrast.Selection(selected);
        }

        /// <summary>Accent used for focus and indicators, same precedence as the others.</summary>
        protected Color ResolveAccentColor(DockConfig config, IBeepTheme theme)
        {
            if (config.IndicatorColorOrNull.HasValue)
                return config.IndicatorColorOrNull.Value;

            if (!IsNamedPalette && config.UseThemeColors && theme != null)
                return DockThemeHelpers.GetIndicatorColor(theme, true, null);

            return DockPainterMetrics.AccentFor(config.Style);
        }

        /// <summary>Foreground/text colour.</summary>
        protected Color ResolveForegroundColor(DockConfig config, IBeepTheme theme)
            => HighContrast.Foreground(
                config.ForegroundColor
                ?? DockThemeHelpers.GetDockForegroundColor(theme, config.UseThemeColors && !IsNamedPalette));

        /// <summary>
        /// Background colour to paint with: the user's if they set one; otherwise the theme's, when
        /// the control asked for theme colours and this is not a named palette; otherwise the style's.
        /// </summary>
        protected Color ResolveBackground(DockConfig config, IBeepTheme theme, Color styleFallback)
        {
            float opacity = ResolveBackgroundOpacity(config);

            if (config.BackgroundColor.HasValue)
                return HighContrast.Background(GetColor(config.BackgroundColor, styleFallback, opacity));

            if (!IsNamedPalette && config.UseThemeColors && theme != null)
                return HighContrast.Background(DockThemeHelpers.GetDockBackgroundColor(theme, true, null, opacity));

            return HighContrast.Background(GetColor(StyleBackgroundColor, styleFallback, opacity));
        }

        /// <summary>
        /// Border colour to paint with, at the caller's opacity. Same precedence as
        /// <see cref="ResolveBackground"/>.
        /// </summary>
        protected Color ResolveBorder(DockConfig config, IBeepTheme theme, Color styleFallback, float opacity = 1f)
        {
            if (config.BorderColor.HasValue)
                return HighContrast.Border(DockThemeHelpers.GetDockBorderColor(theme, false, config.BorderColor, opacity));

            if (!IsNamedPalette && config.UseThemeColors && theme?.BorderColor != null &&
                theme.BorderColor != Color.Empty)
                return HighContrast.Border(DockThemeHelpers.GetDockBorderColor(theme, true, null, opacity));

            return HighContrast.Border(DockThemeHelpers.GetDockBorderColor(theme, false, StyleBorderColor ?? styleFallback, opacity));
        }

        /// <summary>
        /// Background opacity to paint with.
        /// </summary>
        /// <remarks>
        /// The style wins here, where it loses for colours. That is not an inconsistency, it is the
        /// limit of what this stage can express: <see cref="DockConfig.BackgroundOpacity"/> is a
        /// non-nullable float that the control's style setter overwrites on every style change
        /// (BeepDock.Properties.cs:48), so there is no value that means "the user did not set this"
        /// and no way to tell a user's 0.9 from a style's. Preferring the style preserves what the
        /// unconditional writes this replaced already did, without persisting them. Making the
        /// dimension nullable, so the user can win, is stage 03.
        /// </remarks>
        protected float ResolveBackgroundOpacity(DockConfig config)
            => StyleBackgroundOpacity ?? config.BackgroundOpacity;

        #endregion

        #region Abstract Methods

        public abstract void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme);
        public abstract void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Paints an item. Sealed: the style's own rendering goes in <see cref="PaintDockItemCore"/>,
        /// and this guarantees the interaction states are visible whether or not the style bothered.
        /// </summary>
        /// <remarks>
        /// Measured before this existed: <c>Normal = Pressed = Focused = Disabled</c> in **all 18
        /// styles**, 75 state collisions in total, and no style rendering more than 5 of its 8 states
        /// distinctly. The flags were tracked accurately by <c>BeepDock.InteractionState.cs</c> and
        /// thrown away at the paint boundary. Making every painter remember would be 18 chances to
        /// forget; making the base guarantee it is one.
        /// </remarks>
        public void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            // DockAnimationStyle.Rotate drives CurrentRotation; applying it here is what makes the
            // enum value mean something in all 18 styles instead of only wherever a painter
            // remembered. Saved and restored so nothing downstream inherits the transform.
            GraphicsState rotated = null;
            if (itemState != null && Math.Abs(itemState.CurrentRotation) > 0.01f && !itemState.Bounds.IsEmpty)
            {
                rotated = g.Save();
                var centre = new PointF(
                    itemState.Bounds.Left + itemState.Bounds.Width / 2f,
                    itemState.Bounds.Top + itemState.Bounds.Height / 2f);
                g.TranslateTransform(centre.X, centre.Y);
                g.RotateTransform(itemState.CurrentRotation);
                g.TranslateTransform(-centre.X, -centre.Y);
            }

            PaintDockItemCore(g, itemState, config, theme);

            if (rotated != null)
                g.Restore(rotated);

            PaintStateChrome(g, itemState, config, theme);
        }

        /// <summary>The style's own item rendering.</summary>
        protected abstract void PaintDockItemCore(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Draws the states the style did not. Every colour comes from the theme resolvers and every
        /// dimension from the style's own metrics, so this is not one uniform overlay stamped on 18
        /// docks - a Terminal item (4px corners, terminal palette) and a Pill item (28px corners,
        /// surface palette) get visibly different chrome from the same code. Override to replace it
        /// entirely where a style wants its own treatment.
        /// </summary>
        protected virtual void PaintStateChrome(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState == null || itemState.Bounds.Width <= 0)
                return;

            var state = GetInteractionState(itemState);

            // Hover, selection and running are what the styles already distinguish; adding chrome
            // there would paint over the very thing that makes them look like themselves.
            if (state is DockInteractionState.Normal or DockInteractionState.Hovered
                      or DockInteractionState.Selected or DockInteractionState.Running)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var metrics = GetScaledMetrics(config, theme, g);
            int radius = Math.Max(2, metrics.ItemCornerRadius);
            var bounds = itemState.Bounds;

            switch (state)
            {
                case DockInteractionState.Pressed:
                {
                    // Pushed in: the item shrinks toward its own selection colour.
                    var inset = Rectangle.Inflate(bounds, -Math.Max(1, bounds.Width / 24), -Math.Max(1, bounds.Height / 24));
                    var pressed = ResolveSelectedColor(config, theme);
                    using var path = CreateRoundedPath(inset, radius);
                    using var brush = new SolidBrush(Color.FromArgb(90, pressed));
                    g.FillPath(brush, path);
                    break;
                }

                case DockInteractionState.Focused:
                {
                    // A ring inside the item, in the accent colour. This is additional to the
                    // control-level focus rectangle BeepDock draws, which stays identical across
                    // styles on purpose - a focus indicator that varies by theme is an accessibility
                    // problem, not a feature.
                    var accent = ResolveAccentColor(config, theme);
                    int width = Math.Max(1, metrics.BorderWidth + 1);
                    var ring = Rectangle.Inflate(bounds, -width, -width);
                    using var path = CreateRoundedPath(ring, Math.Max(1, radius - width));
                    using var pen = new Pen(Color.FromArgb(200, accent), width);
                    g.DrawPath(pen, path);
                    break;
                }

                case DockInteractionState.Disabled:
                {
                    // Non-interactive, not merely dim. The opacity fade lives in the animator, so a
                    // disabled item that is only faded still reads as clickable; this washes it
                    // toward the dock's own background so it recedes into the surface.
                    var wash = DockThemeHelpers.GetDockBackgroundColor(theme, config.UseThemeColors, null, 1f);
                    using var path = CreateRoundedPath(bounds, radius);
                    using var brush = new SolidBrush(Color.FromArgb(120, wash));
                    g.FillPath(brush, path);
                    break;
                }

                case DockInteractionState.Dragging:
                {
                    // Lifted off the dock: a dashed outline in the accent, and the item ghosted.
                    var accent = ResolveAccentColor(config, theme);
                    using var path = CreateRoundedPath(bounds, radius);
                    using (var ghost = new SolidBrush(Color.FromArgb(70, DockThemeHelpers.GetDockBackgroundColor(theme, config.UseThemeColors, null, 1f))))
                    {
                        g.FillPath(ghost, path);
                    }
                    using var pen = new Pen(Color.FromArgb(220, accent), Math.Max(1, metrics.BorderWidth + 1))
                    {
                        DashStyle = DashStyle.Dash
                    };
                    g.DrawPath(pen, path);
                    break;
                }
            }
        }

        #endregion

        #region Separator Painting

        public virtual void PaintSeparator(Graphics g, Point position, DockConfig config, IBeepTheme theme)
        {
            if (config.SeparatorStyle == DockSeparatorStyle.None)
                return;

            var color = config.SeparatorColor;
            if (color == Color.Empty && theme != null)
                color = Color.FromArgb(100, theme.BorderColor);

            switch (config.SeparatorStyle)
            {
                case DockSeparatorStyle.Line:
                    PaintLineSeparator(g, position, config, color);
                    break;

                case DockSeparatorStyle.Dot:
                    PaintDotSeparator(g, position, config, color);
                    break;

                case DockSeparatorStyle.Space:
                    // Just spacing, no visual element
                    break;
            }
        }

        protected virtual void PaintLineSeparator(Graphics g, Point position, DockConfig config, Color color)
        {
            using (var pen = new Pen(color, 1))
            {
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    int y1 = position.Y;
                    int y2 = position.Y + config.DockHeight - config.Padding * 2;
                    g.DrawLine(pen, position.X, y1, position.X, y2);
                }
                else
                {
                    int x1 = position.X;
                    int x2 = position.X + config.DockHeight - config.Padding * 2;
                    g.DrawLine(pen, x1, position.Y, x2, position.Y);
                }
            }
        }

        protected virtual void PaintDotSeparator(Graphics g, Point position, DockConfig config, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                int dotSize = 4;
                var dotRect = new Rectangle(
                    position.X - dotSize / 2,
                    position.Y - dotSize / 2,
                    dotSize,
                    dotSize
                );
                g.FillEllipse(brush, dotRect);
            }
        }

        #endregion

        #region Layout Calculations

        // The default geometry for all 19 styles: delegate to the helpers that were already doing
        // this work, so wiring the painter in changes nothing until a style chooses to override.

        /// <inheritdoc />
        public virtual Rectangle[] CalculateItemBounds(
            Rectangle dockBounds,
            IList<SimpleItem> items,
            DockConfig config,
            int hoverIndex,
            float hoverProgress)
            => DockLayoutHelper.CalculateItemBounds(dockBounds, items, config, hoverIndex, hoverProgress);

        /// <inheritdoc />
        public virtual Size CalculateDockSize(int itemCount, DockConfig config)
            => DockLayoutHelper.CalculateDockSize(itemCount, config);

        /// <inheritdoc />
        public virtual int HitTest(Point location, List<DockItemState> itemStates)
            => DockHitTestHelper.HitTest(location, itemStates);

        #endregion

        #region Hit Testing

        public virtual DockItemState HitTest(Point location, List<DockItemState> itemStates, DockConfig config)
        {
            // Test in reverse order (top items first)
            for (int i = itemStates.Count - 1; i >= 0; i--)
            {
                var state = itemStates[i];
                if (state.HitBounds.Contains(location))
                {
                    return state;
                }
            }

            return null;
        }

        #endregion

        #region Helper Methods

        protected DockInteractionState GetInteractionState(DockItemState itemState)
        {
            if (itemState == null)
            {
                return DockInteractionState.Normal;
            }

            if (itemState.IsDragging) return DockInteractionState.Dragging;
            if (itemState.IsDisabled) return DockInteractionState.Disabled;
            if (itemState.IsPressed) return DockInteractionState.Pressed;
            if (itemState.IsFocused) return DockInteractionState.Focused;
            if (itemState.IsHovered) return DockInteractionState.Hovered;
            if (itemState.IsSelected) return DockInteractionState.Selected;
            if (itemState.IsRunning) return DockInteractionState.Running;
            return DockInteractionState.Normal;
        }

        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
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

        protected Color GetColor(Color? customColor, Color themeColor, float opacity = 1f)
        {
            var baseColor = customColor ?? themeColor;
            if (opacity < 1f)
            {
                return Color.FromArgb((int)(255 * opacity), baseColor);
            }
            return baseColor;
        }

        #region Icon mode

        /// <summary>
        /// Divides an item's box between its icon and its label according to
        /// <see cref="DockConfig.IconMode"/>. An empty label rectangle means "no label".
        /// </summary>
        protected virtual (Rectangle Icon, Rectangle Label) SplitForIconMode(
            DockItemState itemState, DockConfig config, Graphics g)
        {
            var bounds = itemState.Bounds;

            bool wantsLabel = config.IconMode switch
            {
                DockIconMode.IconWithLabel => true,
                DockIconMode.DetailedIcon => true,
                DockIconMode.IconWithHoverLabel => itemState.IsHovered || itemState.IsFocused,
                _ => false
            };

            if (!wantsLabel || string.IsNullOrEmpty(itemState.Item?.Text))
                return (bounds, Rectangle.Empty);

            using var font = DockFontHelpers.GetDockItemFont(
                DockStyleHelpers.GetControlStyleForDock(config.Style), itemState.IsHovered);
            int lineHeight = (int)Math.Ceiling(g.MeasureString("Ag", font).Height);

            if (config.IconMode == DockIconMode.DetailedIcon)
            {
                // Icon on the leading edge, text beside it.
                int side = Math.Min(bounds.Height, bounds.Width / 2);
                var icon = new Rectangle(bounds.Left, bounds.Top + (bounds.Height - side) / 2, side, side);
                var label = Rectangle.FromLTRB(icon.Right + 4, bounds.Top, bounds.Right, bounds.Bottom);
                return (icon, label.Width > 0 ? label : Rectangle.Empty);
            }

            // Label under the icon, taken out of the item's own box so nothing is drawn outside the
            // rectangle the layout reserved.
            int labelHeight = Math.Min(lineHeight, Math.Max(0, bounds.Height / 3));
            if (labelHeight <= 0)
                return (bounds, Rectangle.Empty);

            var iconRect = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height - labelHeight);
            var labelRect = new Rectangle(bounds.Left, bounds.Bottom - labelHeight, bounds.Width, labelHeight);
            return (iconRect, labelRect);
        }

        /// <summary>Draws the item's caption in the rectangle <see cref="SplitForIconMode"/> reserved.</summary>
        protected virtual void PaintItemLabel(
            Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme,
            Rectangle labelRect, float opacity)
        {
            var text = itemState.Item?.Text;
            if (string.IsNullOrEmpty(text) || labelRect.IsEmpty)
                return;

            var colour = config.ForegroundColor
                ?? (config.UseThemeColors ? DockThemeHelpers.GetDockForegroundColor(theme, true) : Color.White);

            int alpha = (int)(255 * Math.Clamp(itemState.IsDisabled ? Math.Min(0.4f, opacity) : opacity, 0f, 1f));

            using var font = DockFontHelpers.GetDockItemFont(
                DockStyleHelpers.GetControlStyleForDock(config.Style), itemState.IsHovered);
            using var brush = new SolidBrush(Color.FromArgb(alpha, colour));
            using var format = new StringFormat
            {
                Alignment = config.IconMode == DockIconMode.DetailedIcon ? StringAlignment.Near : StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            g.DrawString(text, font, brush, labelRect, format);
        }

        #endregion

        protected void PaintItemIcon(Graphics g, Rectangle bounds, string imagePath, DockConfig config, IBeepTheme theme, float opacity = 1f)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            // Use icon helpers for consistent icon rendering
            var iconColor = TheTechIdea.Beep.Winform.Controls.Docks.Helpers.DockIconHelpers.GetIconColor(
                theme, 
                theme != null, // Assume UseThemeColors if theme is available
                config.ApplyThemeToIcons,
                false, // isHovered - could be enhanced
                false); // isSelected - could be enhanced

            TheTechIdea.Beep.Winform.Controls.Docks.Helpers.DockIconHelpers.PaintIcon(
                g,
                bounds,
                imagePath,
                iconColor,
                theme,
                theme != null,
                config.ApplyThemeToIcons,
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3,
                opacity,
                config.CornerRadius / 2);
        }

        protected void PaintItemIcon(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme, float opacity = 1f)
        {
            if (itemState?.Item == null || string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                return;
            }

            var iconColor = DockIconHelpers.GetIconColor(
                theme,
                theme != null,
                config.ApplyThemeToIcons,
                itemState.IsHovered || itemState.IsFocused,
                itemState.IsSelected);

            // DockIconMode used to have no reader anywhere: all four values drew an icon and no
            // label, so IconOnly was not the default that happened to be right - it was the only
            // behaviour that existed. The label has to be measured out of the item box before the
            // icon is drawn, or it lands outside the bounds hit-testing and layout agreed on.
            var (iconRect, labelRect) = SplitForIconMode(itemState, config, g);

            DockIconHelpers.PaintIcon(
                g,
                iconRect,
                itemState.Item.ImagePath,
                iconColor,
                theme,
                theme != null,
                config.ApplyThemeToIcons,
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3,
                itemState.IsDisabled ? Math.Min(0.4f, opacity) : opacity,
                config.CornerRadius / 2);

            if (!labelRect.IsEmpty)
            {
                PaintItemLabel(g, itemState, config, theme, labelRect, opacity);
            }
        }

        protected void PaintShadow(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            if (!config.ShowShadow)
                return;

            // Use simple shadow for performance
            int shadowSize = 10;
            int shadowOffset = 3;

            using (var shadowPath = CreateRoundedPath(
                new Rectangle(
                    bounds.X + shadowOffset,
                    bounds.Y + shadowOffset,
                    bounds.Width,
                    bounds.Height
                ),
                config.CornerRadius
            ))
            {
                for (int i = shadowSize; i > 0; i--)
                {
                    int alpha = (int)(30 * (i / (float)shadowSize));
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                    using (var pen = new Pen(brush, 1))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        protected void PaintGlow(Graphics g, Rectangle bounds, Color glowColor, int glowSize = 10)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds);

                using (var pgb = new PathGradientBrush(path))
                {
                    pgb.CenterColor = Color.FromArgb(100, glowColor);
                    pgb.SurroundColors = new[] { Color.FromArgb(0, glowColor) };

                    var inflated = bounds;
                    inflated.Inflate(glowSize, glowSize);
                    g.FillEllipse(pgb, inflated);
                }
            }
        }

        #endregion
    }
}
