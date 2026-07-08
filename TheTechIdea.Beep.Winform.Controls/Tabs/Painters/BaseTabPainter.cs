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

        public virtual void PaintBackground(Graphics g, Rectangle bounds)
        {
             Color backgroundColor = Theme?.BackgroundColor ?? SystemColors.Control;
             g.Clear(backgroundColor);
        }

        public virtual void PaintHeaderBackground(Graphics g, Rectangle headerBounds)
        {
             Color panelColor = Theme?.BackgroundColor ?? SystemColors.Control;
             var brush = PaintersFactory.GetSolidBrush(panelColor);
             g.FillRectangle(brush, headerBounds);
        }

        public abstract void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f);

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

            Color baseColor = TabIconHelpers.GetCloseIconColor(Theme, Theme != null, isHovered);
            Color iconColor = Color.FromArgb((int)(Math.Clamp(alpha, 0f, 1f) * 255f), baseColor);
            TabIconHelpers.PaintIcon(
                g,
                closeBounds,
                TabIconHelpers.GetCloseIconPath(),
                iconColor,
                Theme,
                Theme != null,
                GetTabControlStyle());
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
        
        protected void DrawTabText(Graphics g, RectangleF tabRect, string text, int tabIndex, bool isSelected, bool vertical, float alpha = 1.0f)
        {
            if (!TabControl.ShouldShowTabText(tabIndex)) return;

            Color baseColor = TabThemeHelpers.GetTabTextColor(Theme, Theme != null, isSelected);
            Color textColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);

            // BT-01: Font from theme — never disposed
            Font font = TabFontHelpers.GetTabFont(Theme, isSelected);

            if (!vertical)
            {
                var textRect = new Rectangle(
                    (int)(tabRect.X + GetScaledTextPadding()),
                    (int)tabRect.Y,
                    (int)(tabRect.Width - GetScaledTextPadding() * 2),
                    (int)tabRect.Height);
                TextRenderer.DrawText(g, text, font, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            }
            else
            {
                GraphicsState state = g.Save();
                g.TranslateTransform(tabRect.X + tabRect.Width / 2, tabRect.Y + tabRect.Height / 2);
                g.RotateTransform(90);
                Size sz = TextRenderer.MeasureText(g, text, font);
                var vRect = new Rectangle(-sz.Width / 2, -sz.Height / 2, sz.Width, sz.Height);
                TextRenderer.DrawText(g, text, font, vRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                g.Restore(state);
            }
        }

        /// <summary>
        /// Phase 2 default implementation. Delegates to the legacy <see cref="PaintTab"/> overload
        /// using the item's bounds and interaction state. Subclasses may override to draw adornments
        /// (icon, badge, subtext, dirty marker, busy indicator) using the pre-calculated bounds
        /// in <paramref name="itemLayout"/>.
        /// </summary>
        public virtual void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            if (g == null || itemLayout == null || itemLayout.Bounds.IsEmpty)
            {
                return;
            }

            bool isHorizontal = TabControl.HeaderPosition == TabHeaderPosition.Top ||
                TabControl.HeaderPosition == TabHeaderPosition.Bottom;
            Font baseFont = TabFontHelpers.ResolveSafeFont(TextFont ?? TabControl.Font, TabControl);
            BeepTabAdornmentLayoutHelper.Calculate(itemLayout, baseFont, itemLayout.HasCloseButton, isHorizontal);

            BeepTabItem item = itemLayout.Item;
            bool isSelected = item.IsSelected;
            bool isHovered = item.IsHovered;

            // Fall back to the legacy paint path so existing painters still work.
            PaintTab(g, itemLayout.Bounds, item.Index, isSelected, isHovered, alpha);

            // Draw adornment elements that the legacy path does not know about.
            DrawAdornments(g, itemLayout, alpha);
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
            Color baseTextColor = overrideTextColor ?? TabThemeHelpers.GetTabTextColor(Theme, Theme != null, item.IsSelected);
            Color textColor = Color.FromArgb((int)(Math.Clamp(effectiveAlpha, 0f, 1f) * 255f), baseTextColor);

            if (item.HasIcon && !itemLayout.IconBounds.IsEmpty)
            {
                TabIconHelpers.PaintIcon(g, itemLayout.IconBounds, item.IconPath, textColor, Theme, Theme != null, GetTabControlStyle());
            }

            if (TabControl.ShouldShowTabText(item.Index))
            {
                DrawTextInBounds(g, item.Title ?? string.Empty, itemLayout.TextBounds, textColor, isHorizontal);

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
        /// Draws the Phase 2 adornment elements using the pre-calculated bounds on
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
                DrawDirtyMarker(g, itemLayout.DirtyMarkerBounds, alpha);
            }

            // Badge
            if (adornment.HasBadge && !itemLayout.BadgeBounds.IsEmpty)
            {
                DrawBadge(g, itemLayout.BadgeBounds, adornment, alpha);
            }

            // Busy spinner (simple arc for now; subclasses can draw animated versions)
            if (adornment.IsBusy && !itemLayout.BusyIndicatorBounds.IsEmpty)
            {
                DrawBusyIndicator(g, itemLayout.BusyIndicatorBounds, alpha);
            }
        }

        protected BeepControlStyle GetTabControlStyle()
        {
            return TabStyleHelpers.GetControlStyleForTab(TabControl.TabStyle);
        }

        private static void DrawTextInBounds(Graphics g, string text, Rectangle bounds, Color textColor, bool isHorizontal)
        {
            if (string.IsNullOrWhiteSpace(text) || bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0) return;
            TextRenderer.DrawText(g, text, SystemFonts.DefaultFont, bounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        private void DrawDirtyMarker(Graphics g, Rectangle bounds, float alpha)
        {
            Color dotColor = Color.FromArgb((int)(alpha * 220),
                Theme?.PrimaryColor ?? SystemColors.Highlight);
            using var brush = new SolidBrush(dotColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(brush, bounds);
        }

        private void DrawBadge(Graphics g, Rectangle bounds, BeepTabAdornmentState adornment, float alpha)
        {
            int a = (int)(alpha * 220);
            Color backColor = adornment.BadgeKind switch
            {
                BeepTabBadgeKind.Error => Color.FromArgb(a, Theme?.ErrorColor ?? SystemColors.Highlight),
                BeepTabBadgeKind.Warning => Color.FromArgb(a, Theme?.WarningColor ?? SystemColors.Highlight),
                BeepTabBadgeKind.Success => Color.FromArgb(a, Theme?.SuccessColor ?? SystemColors.Highlight),
                BeepTabBadgeKind.Info => Color.FromArgb(a, Theme?.PrimaryColor ?? SystemColors.Highlight),
                _ => Color.FromArgb(a, Theme?.PrimaryColor ?? SystemColors.Highlight)
            };

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
                TextRenderer.DrawText(g, adornment.BadgeText, font, bounds, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
        }

        private static void DrawBusyIndicator(Graphics g, Rectangle bounds, float alpha)
        {
            using var pen = new Pen(Color.FromArgb((int)(alpha * 180), SystemColors.ControlDark), 2f);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawArc(pen, bounds, 0, 270);
        }
    }
}
