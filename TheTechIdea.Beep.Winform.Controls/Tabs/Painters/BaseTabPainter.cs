using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;


namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public abstract class BaseTabPainter : ITabPainter
    {
        public BeepTabs TabControl { get; set; }
        public IBeepTheme Theme { get; set; }
        public Font TextFont { get; set; }

        public BaseTabPainter(BeepTabs tabControl)
        {
            TabControl = tabControl;
        }

        public virtual void PaintHeaderBackground(Graphics g, Rectangle headerBounds)
        {
             // Resolved through the colour seam so high contrast reaches it, and so the header
             // background is one value rather than two that can drift apart.
             Color panelColor = TabThemeHelpers.GetHeaderBackgroundColor(Theme);
             var brush = PaintersFactory.GetSolidBrush(panelColor);
             g.FillRectangle(brush, headerBounds);
        }

        /// <summary>
        /// Most styles express selection through the tab body itself (fill, border, elevation) and
        /// draw no separate accent bar. <see cref="UnderlineTabPainter"/> overrides this.
        /// </summary>
        public virtual void PaintSelectionAccent(Graphics g, RectangleF accentBounds, float alpha = 1.0f)
        {
            // intentionally empty: a style with no selection accent draws nothing here. Overriding
            // is opt-in, so adding a painter does not require thinking about the accent at all.
        }

        /// <summary>
        /// The colour actually behind a tab's text — what the label must remain readable against.
        /// </summary>
        /// <remarks>
        /// Painters that fill a tab body (Classic, Capsule, Card, Segmented, Button) leave this as
        /// the tab background. Painters that draw no fill at all must override it with the header
        /// background, otherwise the selected-tab text colour — which is chosen to sit on a filled,
        /// accented tab — is drawn straight onto the header. That was live: under DefaultTheme the
        /// Underline and Minimal painters rendered the selected tab's title white on white, so the
        /// label simply vanished when you selected it. The contact sheet found it; no assertion did.
        /// </remarks>
        protected virtual Color GetTabSurfaceColor(BeepTabItem item)
        {
            return TabThemeHelpers.GetTabBackgroundColor(
                Theme, item.IsSelected, item.IsHovered);
        }

        public virtual SizeF MeasureTab(Graphics g, int index, Font font)
        {
            if (index < 0)
            {
                return SizeF.Empty;
            }

            var items = TabControl.GetHostedSourceItemsSnapshot();
            if (index >= items.Count)
            {
                return SizeF.Empty;
            }

            BeepTabItem item = items[index];
            Font baseFont = TabFontHelpers.GetTabFont(Theme, item.IsSelected);
            SizeF titleSize = TextUtils.MeasureText(g, item.Title ?? string.Empty, baseFont);
            float contentWidth = titleSize.Width;
            float contentHeight = titleSize.Height;

            if (item.HasSubText)
            {
                Font subTextFont = TabFontHelpers.GetTabSubtextFont(Theme, TabControl);
                SizeF subTextSize = TextUtils.MeasureText(g, item.SubText ?? string.Empty, subTextFont);
                contentWidth = Math.Max(contentWidth, subTextSize.Width);
                contentHeight += subTextSize.Height + 2f;
            }

            bool showCloseButton = item.CanClose && item.CloseVisible != false && TabControl.ShowCloseButtons;
            float width = contentWidth + BeepTabAdornmentLayoutHelper.MeasureHorizontalAdornmentWidth(item.GetAdornmentState(), showCloseButton);
            float height = Math.Max(contentHeight + GetScaledTextPadding(), titleSize.Height + (GetScaledTextPadding() * 2));

            return new SizeF(width, height);
        }

        /// <summary>
        /// Scales a design-time pixel value for the current display.
        /// </summary>
        /// <remarks>
        /// Every literal a painter draws with — insets, gaps, corner radii, rule thicknesses — has to
        /// go through this. A hardcoded 3px gap is 3px on a 200% display too, so at high DPI the
        /// chrome shrinks to a third of its intended weight while the text scales normally. The rest
        /// of this control already scales through <c>DpiScalingHelper</c>; painters had no equivalent
        /// and so were written with raw constants.
        /// </remarks>
        protected int Scale(int designPixels) => DpiScalingHelper.ScaleValue(designPixels, TabControl);

        protected int GetScaledCloseButtonSize() => DpiScalingHelper.ScaleValue(24, TabControl);
        protected int GetScaledCloseButtonPadding() => DpiScalingHelper.ScaleValue(8, TabControl);
        protected int GetScaledTextPadding() => DpiScalingHelper.ScaleValue(12, TabControl);

        protected void DrawCloseButton(Graphics g, RectangleF tabRect, bool vertical)
        {
            DrawCloseButton(g, Rectangle.Round(GetCloseButtonRect(tabRect, vertical)), false, 1f);
        }

        protected void DrawCloseButton(Graphics g, Rectangle closeBounds, bool isHovered, float alpha)
        {
            if (closeBounds.IsEmpty)
            {
                return;
            }

            Color baseColor = TabIconHelpers.GetCloseIconColor(Theme, isHovered);
            Color iconColor = Color.FromArgb((int)(Math.Clamp(alpha, 0f, 1f) * 255f), baseColor);
            TabIconHelpers.PaintIcon(
                g,
                closeBounds,
                TabIconHelpers.GetCloseIconPath(),
                iconColor);
        }

        public RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical)
        {
            int scaledCloseButtonSize = GetScaledCloseButtonSize();
            int scaledCloseButtonPadding = GetScaledCloseButtonPadding();

            if (vertical)
            {
                return new RectangleF(
                    tabRect.X + (tabRect.Width - scaledCloseButtonSize) / 2,
                    tabRect.Bottom - scaledCloseButtonSize - scaledCloseButtonPadding,
                    scaledCloseButtonSize,
                    scaledCloseButtonSize
                );
            }
            return new RectangleF(
                tabRect.Right - scaledCloseButtonSize - scaledCloseButtonPadding,
                tabRect.Top + (tabRect.Height - scaledCloseButtonSize) / 2,
                scaledCloseButtonSize,
                scaledCloseButtonSize
            );
        }

        protected GraphicsPath GetRoundedRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int maxRadius = (int)Math.Min(rect.Width / 2f, rect.Height / 2f);
            int safeRadius = Math.Max(0, Math.Min(radius, maxRadius));
            if (safeRadius < 1)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = safeRadius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        
        /// <summary>
        /// Paints one tab. The default is the shared content pass — icon, title, subtext, close
        /// button and adornments — with no chrome of its own, which is exactly what a style that
        /// draws no tab body needs. Styles that draw chrome override this, render their shape, and
        /// then call <see cref="DrawTabItemContent"/>.
        /// </summary>
        /// <remarks>
        /// There used to be a second entry point, <c>PaintTab(Graphics, RectangleF, int, bool, bool,
        /// float)</c>, declared on the interface and overridden by all seven painters. It was
        /// unreachable: it is called only from this method's former body, and every painter
        /// overrides this method, so that body never ran. Each painter therefore carried **two
        /// implementations of the same visual** — the same colours, radius and fill written once
        /// against <c>tabRect</c>/<c>isSelected</c> and once against <c>itemLayout.Item</c> — and
        /// only the second was ever displayed. Measured with a probe subclass, after reading had
        /// twice given the wrong answer in opposite directions.
        /// </remarks>
        public virtual void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            DrawTabItemContent(g, itemLayout, alpha);
        }

        protected virtual void DrawTabItemContent(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha, Color? overrideTextColor = null)
        {
            if (g == null || itemLayout == null || itemLayout.Bounds.IsEmpty)
            {
                return;
            }

            bool isHorizontal = TabControl.HeaderPosition == TabHeaderPosition.Top ||
                TabControl.HeaderPosition == TabHeaderPosition.Bottom;
            Font baseFont = TabFontHelpers.GetTabFont(Theme, itemLayout.Item.IsSelected);
            BeepTabAdornmentLayoutHelper.Calculate(itemLayout, baseFont, itemLayout.HasCloseButton, isHorizontal);

            BeepTabItem item = itemLayout.Item;
            float effectiveAlpha = item.IsEnabled ? alpha : alpha * 0.55f;
            Color baseTextColor = overrideTextColor ?? TabThemeHelpers.GetTabTextColor(Theme, item.IsSelected);
            // Guarantee the label is legible against whatever this painter actually drew behind it.
            baseTextColor = ColorUtils.EnsureReadable(baseTextColor, GetTabSurfaceColor(item));
            Color textColor = Color.FromArgb((int)(Math.Clamp(effectiveAlpha, 0f, 1f) * 255f), baseTextColor);

            if (item.HasIcon && !itemLayout.IconBounds.IsEmpty)
            {
                TabIconHelpers.PaintIcon(g, itemLayout.IconBounds, item.IconPath, textColor);
            }

            if (TabControl.ShouldShowTabText(item.Index))
            {
                // baseFont is the same font MeasureTab measured this title with.
                DrawTextInBounds(g, item.Title ?? string.Empty, itemLayout.TextBounds, textColor, baseFont, isHorizontal);

                if (item.HasSubText && !itemLayout.SubTextBounds.IsEmpty)
                {
                    Font subFont = TabFontHelpers.GetTabSubtextFont(Theme, TabControl);
                    Color subTextColor = Color.FromArgb((int)(Math.Clamp(effectiveAlpha * 0.72f, 0f, 1f) * 255f), baseTextColor);
                    TextRenderer.DrawText(g, item.SubText ?? string.Empty, subFont, itemLayout.SubTextBounds, subTextColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                }
            }

            if (itemLayout.HasCloseButton && !itemLayout.CloseButtonBounds.IsEmpty)
            {
                DrawCloseButton(g, itemLayout.CloseButtonBounds, item.IsCloseButtonHovered || item.IsCloseButtonPressed, effectiveAlpha);
            }

            DrawAdornments(g, itemLayout, effectiveAlpha);
        }

        /// <summary>
        /// Draws the adornment elements using the pre-calculated bounds on
        /// <paramref name="itemLayout"/>. Called from <see cref="PaintTabItem"/>.
        /// Subclasses may override for style-specific adornment rendering.
        /// </summary>
        protected virtual void DrawAdornments(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha)
        {
            BeepTabItem item = itemLayout.Item;
            BeepTabAdornmentState adornment = item.GetAdornmentState();

            // Dirty dot
            if (adornment.IsDirty && !itemLayout.DirtyMarkerBounds.IsEmpty)
            {
                DrawDirtyMarker(g, itemLayout.DirtyMarkerBounds, alpha, GetTabSurfaceColor(item));
            }

            // Badge
            if (adornment.HasBadge && !itemLayout.BadgeBounds.IsEmpty)
            {
                DrawBadge(g, itemLayout.BadgeBounds, adornment, alpha, GetTabSurfaceColor(item));
            }

            // Busy spinner (simple arc for now; subclasses can draw animated versions)
            if (adornment.IsBusy && !itemLayout.BusyIndicatorBounds.IsEmpty)
            {
                DrawBusyIndicator(g, itemLayout.BusyIndicatorBounds, alpha, GetTabSurfaceColor(item));
            }
        }

        protected BeepControlStyle GetTabControlStyle()
        {
            return TabStyleHelpers.GetControlStyleForTab(TabControl.TabStyle);
        }

        /// <summary>
        /// Draws the tab title into its measured bounds.
        /// </summary>
        /// <param name="font">
        /// Must be the font <see cref="MeasureTab"/> measured with — i.e.
        /// <c>TabFontHelpers.GetTabFont(Theme, item.IsSelected)</c>. This parameter exists because
        /// the method previously hardcoded <see cref="SystemFonts.DefaultFont"/> while the measure
        /// side used the theme font, so every tab was sized for one font and painted in another:
        /// theme fonts never reached the drawn title, and a selected tab measured bold but drew
        /// regular. That is the same defect that clipped every label in BeepTree.
        /// </param>
        /// <param name="isHorizontal">
        /// <see langword="false"/> for Left/Right header positions, where the label is rotated to run
        /// down the tab. The deleted <c>DrawTabText</c> handled that rotation and this method did
        /// not, so removing the unreachable <c>PaintTab</c> path took vertical text with it and
        /// Left/Right tabs drew their labels horizontally into a tall, narrow rect — clipped to a
        /// character or two. Restored here, in the one text path that survives.
        /// </param>
        private static void DrawTextInBounds(Graphics g, string text, Rectangle bounds, Color textColor,
                                             Font font, bool isHorizontal)
        {
            if (string.IsNullOrWhiteSpace(text) || bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0) return;

            const TextFormatFlags Flags =
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;

            if (isHorizontal)
            {
                TextRenderer.DrawText(g, text, font, bounds, textColor, TextFormatFlags.Left | Flags);
                return;
            }

            // Rotate about the centre of the measured bounds so the label runs down the tab.
            //
            // GDI+ (Graphics.DrawString), not TextRenderer, because TextRenderer draws through GDI
            // and *ignores the world transform entirely*. The rotation silently does nothing and the
            // text lands at the untransformed rect — off the tab. The deleted DrawTabText had this
            // same bug, so vertical tabs have never rendered a rotated caption.
            GraphicsState state = g.Save();
            try
            {
                g.TranslateTransform(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                g.RotateTransform(90f);

                var rotated = new RectangleF(
                    -bounds.Height / 2f, -bounds.Width / 2f, bounds.Height, bounds.Width);

                using var format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                };
                using var brush = new SolidBrush(textColor);
                g.DrawString(text, font, brush, rotated, format);
            }
            finally
            {
                g.Restore(state);
            }
        }

        private void DrawDirtyMarker(Graphics g, Rectangle bounds, float alpha, Color surface)
        {
            Color dotColor = Color.FromArgb((int)(alpha * 220),
                SeparateFromSurface(TabThemeHelpers.GetDirtyMarkerColor(Theme), surface));
            using var brush = new SolidBrush(dotColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(brush, bounds);
        }

        /// <summary>
        /// Keeps an adornment visible against the tab it is drawn on. The dirty dot, the busy ring
        /// and the Info/Count badge all resolve to the theme's primary colour — which is also the
        /// selected tab's fill, so on a selected tab each of them was drawn blue-on-blue and simply
        /// disappeared. They were "rendering" the whole time; nothing could see them.
        /// </summary>
        private static Color SeparateFromSurface(Color adornment, Color surface)
        {
            if (Math.Abs(adornment.R - surface.R) + Math.Abs(adornment.G - surface.G)
                + Math.Abs(adornment.B - surface.B) > 24)
            {
                return adornment;
            }

            return surface.GetBrightness() > 0.5f
                ? ControlPaint.Dark(adornment, 0.25f)
                : ControlPaint.Light(adornment, 0.45f);
        }

        private void DrawBadge(Graphics g, Rectangle bounds, BeepTabAdornmentState adornment,
                               float alpha, Color surface)
        {
            int a = (int)(alpha * 220);
            Color badgeFill = TabThemeHelpers.GetBadgeColor(Theme, adornment.BadgeKind);

            badgeFill = SeparateFromSurface(badgeFill, surface);
            Color backColor = Color.FromArgb(a, badgeFill);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (adornment.BadgeKind == BeepTabBadgeKind.Dot)
            {
                using var brush = new SolidBrush(backColor);
                g.FillEllipse(brush, bounds);
                return;
            }

            int radius = bounds.Height / 2;
            using (var path = GetRoundedRect(bounds, radius))
            using (var brush = new SolidBrush(backColor))
                g.FillPath(brush, path);

            if (!string.IsNullOrWhiteSpace(adornment.BadgeText))
            {
                Font font = TabFontHelpers.GetTabFont(Theme);

                // The count was drawn in hardcoded white. On a light badge — a pale Warning amber,
                // or any light theme's Success green — white on light is unreadable, and in high
                // contrast it ignored the system palette entirely. Pick against the actual fill.
                Color badgeText = ColorUtils.EnsureReadable(
                    badgeFill.GetBrightness() > 0.55f ? Color.Black : Color.White, badgeFill);

                TextRenderer.DrawText(g, adornment.BadgeText, font, bounds, badgeText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
        }

        private void DrawBusyIndicator(Graphics g, Rectangle bounds, float alpha, Color surface)
        {
            // Was hardcoded to SystemColors.ControlDark: the only adornment that never responded to
            // the theme, and invisible against a dark theme's header.
            Color busy = SeparateFromSurface(
                TabThemeHelpers.GetBusyIndicatorColor(Theme), surface);
            using var pen = new Pen(Color.FromArgb((int)(alpha * 180), busy), 2f);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawArc(pen, bounds, 0, 270);
        }
    }
}
