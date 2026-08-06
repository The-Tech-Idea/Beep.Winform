using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal abstract class BaseBottomBarPainter : IBottomBarPainter
    {
        protected BeepBottomBarLayoutHelper _layoutHelper;

        public virtual string Name => "BaseBottomBarPainter";

        public virtual void Dispose() { }

        /// <summary>
        /// Width the selected cell needs, as a multiple of a normal cell. 1.0 keeps the equal grid.
        /// </summary>
        /// <remarks>
        /// Most of the reference designs keep a strict equal-cell grid and show selection with colour,
        /// a pill behind the icon, or an indicator bar. A few draw a pill containing the icon *and*
        /// its label side by side, and those genuinely need a wider cell - a 74px cell cannot hold a
        /// 24px icon plus a word, which is why that style rendered "H..." where the reference reads
        /// "Home". Declaring it per painter keeps the grid strict everywhere it should be, instead of
        /// widening every style's selection because one of them needs it.
        /// </remarks>
        protected virtual float SelectedCellWidthFactor => 1.0f;

        public virtual void CalculateLayout(BottomBarPainterContext context)
        {
            _layoutHelper = context.LayoutHelper ?? new BeepBottomBarLayoutHelper();

            // The style's requirement is a floor, not an override: a caller who asked for more keeps it.
            float needed = Math.Max(_layoutHelper.SelectedWidthFactor, SelectedCellWidthFactor);
            if (Math.Abs(_layoutHelper.SelectedWidthFactor - needed) > 0.001f)
            {
                _layoutHelper.SelectedWidthFactor = needed;
                _layoutHelper.InvalidateLayout();
            }

            _layoutHelper.EnsureLayout(GetContentBounds(context), context.Items, context.CTAIndex, context.SelectedIndex);
        }

        /// <summary>
        /// The area the icon/label grid may occupy. Override to reserve a band for style chrome.
        /// </summary>
        /// <remarks>
        /// A style that draws a track or rail along an edge has to take that space out of the layout,
        /// or the grid is laid out over the full height and the chrome is drawn straight through the
        /// labels - which is exactly what SegmentedTrack did, striking a line through the selected
        /// item's caption.
        /// </remarks>
        protected virtual Rectangle GetContentBounds(BottomBarPainterContext context) => context.Bounds;

        public virtual void RegisterHitAreas(BottomBarPainterContext context)
        {
        }

        public virtual void Paint(BottomBarPainterContext context)
        {
            if (context.Graphics == null) return;
            var g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            PaintBarBackground(g, context);
            PaintBarBorder(g, context);
            PaintIndicator(g, context);
            PaintItems(g, context);

            g.SmoothingMode = SmoothingMode.Default;
        }

        protected virtual void PaintBarBackground(Graphics g, BottomBarPainterContext context)
        {
            using var b = new SolidBrush(ResolveBarBack(context));
            g.FillRectangle(b, context.Bounds);
        }

        protected virtual void PaintBarBorder(Graphics g, BottomBarPainterContext context)
        {
            if (context.NavigationBorderColor == Color.Empty) return;
            using var pen = new Pen(context.NavigationBorderColor, 1f);
            g.DrawLine(pen, context.Bounds.Left, context.Bounds.Top, context.Bounds.Right, context.Bounds.Top);
        }

        protected virtual void PaintIndicator(Graphics g, BottomBarPainterContext context)
        {
            var indicatorRect = _layoutHelper.GetIndicatorRect();
            RectangleF indicator = indicatorRect;
            if (context.AnimatedIndicatorWidth > 0f)
            {
                indicator = new RectangleF(context.AnimatedIndicatorX, indicatorRect.Top, context.AnimatedIndicatorWidth, indicatorRect.Height);
            }
            if (indicator.Width <= 0) return;

            var accent = ResolveAccent(context);
            DrawIndicatorPill(g, indicator, accent, 0.25f);
        }

        protected virtual void PaintItems(Graphics g, BottomBarPainterContext context)
        {
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(g, item, rects[i], i, context);
            }
        }

        protected virtual void PaintMenuItem(Graphics g, SimpleItem item, Rectangle rect, BottomBarPainterContext context)
        {
            int index = context.Items.IndexOf(item);
            PaintMenuItem(g, item, rect, index, context);
        }

        protected virtual void PaintMenuItem(Graphics g, SimpleItem item, Rectangle rect, int index, BottomBarPainterContext context)
        {
            bool isSelected = index == context.SelectedIndex;
            bool isHovered = index == context.HoverIndex;
            bool isFocused = index >= 0 && context.HoverIndex == index;

            if (isHovered || isFocused)
            {
                PaintItemHoverBackground(g, rect, context);
            }

            var iconRect = _layoutHelper.GetIconRect(index);
            if (iconRect == Rectangle.Empty)
            {
                // Spec icon size, same as the layout helper - this fallback carried the same
                // Math.Min(24, height / 3) that kept icons a quarter under size.
                int iconSize = Math.Min(24, Math.Max(8, rect.Height - 14));
                iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + 7, iconSize, iconSize);
            }

            PaintItemIcon(g, item, iconRect, isSelected, isHovered, context);
            PaintItemBadge(g, item, iconRect, context);

            bool showLabel = context.LabelPolicy switch
            {
                LabelVisibilityPolicy.Always => true,
                LabelVisibilityPolicy.SelectedOnly => isSelected,
                LabelVisibilityPolicy.IconOnly => false,
                _ => true
            };

            if (showLabel)
            {
                var labelRect = _layoutHelper.GetLabelRect(index);
                if (labelRect == Rectangle.Empty)
                {
                    labelRect = new Rectangle(rect.Left + 2, iconRect.Bottom + 2, rect.Width - 4, rect.Bottom - iconRect.Bottom - 4);
                }
                PaintItemLabel(g, item, labelRect, isSelected, isHovered, context);
            }
        }

        protected virtual void PaintItemHoverBackground(Graphics g, Rectangle rect, BottomBarPainterContext context)
        {
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(rect, 8);
            using var brush = new SolidBrush(Color.FromArgb(20, ResolveAccent(context)));
            g.FillPath(brush, path);
        }

        protected virtual void PaintItemIcon(Graphics g, SimpleItem item, Rectangle iconRect, bool isSelected, bool isHovered, BottomBarPainterContext context)
        {
            var tint = isSelected
                ? ResolveAccent(context)
                : (isHovered ? ResolveHoverFore(context) : ResolveBarFore(context));

            PaintTintedIcon(g, string.IsNullOrEmpty(item?.ImagePath) ? context.DefaultImagePath : item.ImagePath,
                            iconRect, tint, context);
        }

        /// <summary>
        /// Draws an icon in a given colour.
        /// </summary>
        /// <remarks>
        /// Every painter used to do this by setting <c>ImagePainter.ApplyThemeOnImage = false</c> and
        /// then assigning <c>FillColor</c> - but <c>FillColor</c> is only applied by
        /// <c>ApplyThemeToSvg</c>, which the <c>ApplyThemeOnImage</c> setter calls **only when set to
        /// true**. So the assignment invalidated a cache and tinted nothing: every icon rendered in
        /// the SVG's own colours, measured at pure black for both the selected and unselected item.
        /// The selected icon being accent-coloured is the most visible thing in every one of the
        /// reference designs, and no style had it.
        ///
        /// <c>StyledImagePainter.PaintWithTint</c> is the supported path and is what the Docks
        /// painters already use.
        /// </remarks>
        protected static void PaintTintedIcon(Graphics g, string imagePath, Rectangle iconRect, Color tint,
                                              BottomBarPainterContext context)
        {
            if (string.IsNullOrEmpty(imagePath) || iconRect.Width <= 0 || iconRect.Height <= 0)
                return;

            using var path = new GraphicsPath();
            path.AddRectangle(iconRect);
            StyledImagePainter.PaintWithTint(g, path, imagePath, tint);
        }

        protected virtual void PaintItemBadge(Graphics g, SimpleItem item, Rectangle iconRect, BottomBarPainterContext context)
        {
            if (string.IsNullOrEmpty(item.BadgeText)) return;

            var badgeText = item.BadgeText;
            Font badgeFont = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.LabelSmall) ?? SystemFonts.DefaultFont;
            Size sz = TextRenderer.MeasureText(badgeText, badgeFont);
            int padding = 6;
            int badgeW = Math.Max(sz.Width + padding, 16);
            int badgeH = Math.Max(sz.Height + 4, 12);
            int badgeX = iconRect.Right - badgeW / 2;
            int badgeY = iconRect.Top - badgeH / 2;
            var badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);

            var badgeBack = (Color)(item.BadgeBackColor == Color.Empty ? ResolveBadgeBack(context) : (Color)item.BadgeBackColor);
            var badgeFore = (Color)(item.BadgeForeColor == Color.Empty ? ResolveBadgeFore(context) : (Color)item.BadgeForeColor);

            using (var brush = new SolidBrush(badgeBack))
            {
                if (item.BadgeShape == BadgeShape.Circle)
                {
                    int size = Math.Max(badgeW, badgeH);
                    var circRect = new Rectangle(iconRect.Right - size / 2, iconRect.Top - size / 2, size, size);
                    g.FillEllipse(brush, circRect);
                }
                else if (item.BadgeShape == BadgeShape.RoundedRectangle)
                {
                    using var path = GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, badgeH / 2);
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, badgeRect);
                }
            }
            TextRenderer.DrawText(g, badgeText, badgeFont, badgeRect, badgeFore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        protected virtual void PaintItemLabel(Graphics g, SimpleItem item, Rectangle textRect, bool isSelected, bool isHovered, BottomBarPainterContext context)
        {
            Font font = ResolveItemFont(context);
            Color fg = isSelected ? ResolveAccent(context) : (isHovered ? ResolveHoverFore(context) : ResolveBarFore(context));
            TextRenderer.DrawText(g, item.Text ?? "", font, textRect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        /// <summary>Label size in points, from the design spec.</summary>
        /// <remarks>
        /// The reference sheet specifies 12px labels. The family still comes from the theme, so a
        /// themed font is honoured where it matters and the control's own proportions are honoured
        /// where they matter - taking the size from the theme too made the label whatever
        /// <c>BodySmall</c> happened to be, which is how the labels drifted off-spec.
        /// </remarks>
        protected const float LabelPointSize = 9f;   // 12px at 96dpi

        protected virtual Font ResolveItemFont(BottomBarPainterContext context)
        {
            var themed = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodySmall);
            var family = themed?.FontFamily ?? SystemFonts.DefaultFont.FontFamily;
            var style = themed?.Style ?? FontStyle.Regular;
            return new Font(family, LabelPointSize, style, GraphicsUnit.Point);
        }

        protected void DrawIndicatorPill(Graphics g, RectangleF rect, Color accent, float alpha)
        {
            using var brush = new SolidBrush(Color.FromArgb((int)(alpha * 255), accent));
            using var gp = new GraphicsPath();
            var r = Rectangle.Round(rect);
            int radius = (int)Math.Min(r.Height / 2f, r.Width / 4f);
            if (radius < 2) radius = 2;
            gp.AddArc(r.Left, r.Top, radius * 2, radius * 2, 180, 90);
            gp.AddArc(r.Right - radius * 2, r.Top, radius * 2, radius * 2, 270, 90);
            gp.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            gp.AddArc(r.Left, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            gp.CloseFigure();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, gp);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected void DrawIndicatorLine(Graphics g, RectangleF rect, Color accent, float thickness)
        {
            using var pen = new Pen(accent, thickness);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            float y = rect.Bottom - thickness / 2f;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(pen, rect.Left + 4, y, rect.Right - 4, y);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected void DrawIndicatorDot(Graphics g, RectangleF rect, Color accent, float diameter)
        {
            using var brush = new SolidBrush(accent);
            float cx = (rect.Left + rect.Right) / 2f;
            float cy = rect.Bottom - diameter / 2f - 2;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(brush, cx - diameter / 2f, cy - diameter / 2f, diameter, diameter);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected void DrawIndicatorSegment(Graphics g, RectangleF rect, Color accent, float height)
        {
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(
                new RectangleF((rect.Left + rect.Right) / 2f - 20, rect.Bottom - height - 2, 40, height), 2);
            using var brush = new SolidBrush(accent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, path);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected void DrawRipple(Graphics g, Point center, int radius, Color accent, float alpha)
        {
            using var brush = new SolidBrush(Color.FromArgb((int)(alpha * 120), accent));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected void DrawSoftShadow(Graphics g, Rectangle bounds, Color shadowColor, int blur, int offsetY)
        {
            if (shadowColor.A == 0) return;
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(bounds, 8);
            using var brush = new SolidBrush(shadowColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(0, offsetY);
            g.FillPath(brush, path);
            g.TranslateTransform(0, -offsetY);
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected Color ResolveBarBack(BottomBarPainterContext context)
            => context.BarBackColor != Color.Empty ? context.BarBackColor : Color.White;

        protected Color ResolveBarFore(BottomBarPainterContext context)
            => context.BarForeColor != Color.Empty ? context.BarForeColor : Color.FromArgb(96, 96, 96);

        protected Color ResolveHoverFore(BottomBarPainterContext context)
            => context.BarHoverForeColor != Color.Empty ? context.BarHoverForeColor : ResolveBarFore(context);

        protected Color ResolveBadgeBack(BottomBarPainterContext context)
            => context.BadgeBackColor != Color.Empty ? context.BadgeBackColor : ResolveAccent(context);

        protected Color ResolveBadgeFore(BottomBarPainterContext context)
            => context.BadgeForeColor != Color.Empty ? context.BadgeForeColor : Color.White;

        protected Color ResolveAccent(BottomBarPainterContext context)
            => context.AccentColor != Color.Empty ? context.AccentColor : Color.FromArgb(96, 80, 255);

        protected Color ResolveOnAccent(BottomBarPainterContext context)
            => context.OnAccentColor != Color.Empty ? context.OnAccentColor : Color.White;

        protected Color ResolveBorderColor(BottomBarPainterContext context)
            => context.NavigationBorderColor != Color.Empty ? context.NavigationBorderColor : Color.FromArgb(30, 0, 0, 0);

        protected Color ResolveShadowColor(BottomBarPainterContext context)
            => context.NavigationShadowColor != Color.Empty ? context.NavigationShadowColor : Color.FromArgb(20, 0, 0, 0);
    }
}
