using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public abstract class BaseTabStripPainter : ITabStripPainter
    {
        public abstract string Name { get; }

        public virtual void PaintTab(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            if (tab.TabRect.IsEmpty) return;
            PaintTabBackground(g, tab, context);
            PaintTabContent(g, tab, index, context);
        }

        public virtual void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var path = CreateTabPath(tab.TabRect, context);
            using var br = new SolidBrush(fill);
            g.FillPath(br, path);
        }

        public virtual void PaintTabContent(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            bool drawIcon = context.ResponsiveMode != TabResponsiveMode.Compact;
            bool drawTitle = context.ResponsiveMode == TabResponsiveMode.Normal
                || context.ResponsiveMode == TabResponsiveMode.Compact
                || (context.ResponsiveMode == TabResponsiveMode.IconOnly && (tab.IsActive || string.IsNullOrEmpty(tab.IconPath)))
                || (context.ResponsiveMode == TabResponsiveMode.ActiveOnly && tab.IsActive);

            if (drawIcon && !tab.IconRect.IsEmpty)
                PaintTabIcon(g, tab, context);

            if (drawTitle && !tab.TitleRect.IsEmpty)
                PaintTabText(g, tab, context);

            if (!tab.DirtyRect.IsEmpty)
                PaintDirtyDot(g, tab, context);

            if (context.ShowCloseButton(tab, index))
                PaintCloseButton(g, tab, context);

            if (!string.IsNullOrEmpty(tab.BadgeText))
                PaintBadge(g, tab, context);

            if (context.IsTabActive(tab))
                PaintAccentBar(g, tab, context);

            if (context.ColorMode != TabColorMode.None && tab.TabColor != Color.Empty)
                PaintTabColorOverlay(g, tab, context);
        }

        public virtual void PaintTabIcon(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            if (tab.IconRect.IsEmpty) return;

            Image img = null;
            bool ownImage = false;

            try
            {
                if (tab.ImageList != null && tab.ImageIndex >= 0 && tab.ImageIndex < tab.ImageList.Images.Count)
                {
                    img = tab.ImageList.Images[tab.ImageIndex];
                }

                if (img == null && !string.IsNullOrEmpty(tab.IconPath))
                {
                    img = TryLoadIcon(tab.IconPath);
                    ownImage = img != null;
                }

                if (img == null) return;

                var path = new GraphicsPath();
                path.AddRectangle(tab.IconRect);

                if (tab.IsActive)
                {
                    StyledImagePainter.Paint(g, path, tab.IconPath ?? string.Empty);
                }
                else
                {
                    StyledImagePainter.PaintWithTint(g, path, tab.IconPath ?? string.Empty, context.Theme.SecondaryTextColor, 0.55f);
                }

                path.Dispose();
            }
            finally
            {
                if (ownImage) img?.Dispose();
            }
        }

        public virtual void PaintTabText(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            if (tab.TitleRect.IsEmpty) return;

            Color textColor = context.GetTextColor(tab);
            var font = tab.IsActive
                ? new Font(context.TextFont, FontStyle.Bold)
                : context.TextFont;

            using var fmt = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            using var textBr = new SolidBrush(textColor);
            g.DrawString(tab.Title, font, textBr, tab.TitleRect, fmt);

            if (tab.IsActive && font != context.TextFont) font.Dispose();
        }

        public virtual void PaintCloseButton(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            bool hovered = context.HoverClose;
            if (hovered)
            {
                using var hBr = new SolidBrush(context.Theme.ErrorColor);
                g.FillEllipse(hBr, tab.CloseRect);
            }

            int m = System.Math.Max(3, tab.CloseRect.Width / 4);
            var cr = tab.CloseRect;
            using var xPen = new Pen(hovered ? Color.White : context.Theme.SecondaryTextColor, 1.5f);
            g.DrawLine(xPen, cr.Left + m, cr.Top + m, cr.Right - m, cr.Bottom - m);
            g.DrawLine(xPen, cr.Right - m, cr.Top + m, cr.Left + m, cr.Bottom - m);
        }

        public virtual void PaintBadge(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            if (string.IsNullOrEmpty(tab.BadgeText)) return;

            SizeF textSize = g.MeasureString(tab.BadgeText, context.BadgeFont);
            int padH = context.Scale(4);
            int padV = context.Scale(2);
            int pillW = System.Math.Max((int)textSize.Width + padH * 2, context.Scale(14));
            int pillH = (int)textSize.Height + padV * 2;

            int bx = tab.TabRect.Right - pillW - context.Scale(2);
            int by = tab.TabRect.Top + context.Scale(1);
            var pillRect = new Rectangle(bx, by, pillW, pillH);
            int radius = pillH / 2;

            Color fillCol = tab.BadgeColor == Color.Empty ? context.Theme.ErrorColor : tab.BadgeColor;
            using (var fillBr = new SolidBrush(fillCol))
            using (var path = CreateRoundedRect(pillRect, radius))
                g.FillPath(fillBr, path);

            using var textBr = new SolidBrush(Color.White);
            var textRect = new RectangleF(pillRect.X, pillRect.Y, pillRect.Width, pillRect.Height);
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(tab.BadgeText, context.BadgeFont, textBr, textRect, fmt);
        }

        public virtual void PaintDirtyDot(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            using var dotBr = new SolidBrush(context.Theme.WarningColor);
            g.FillEllipse(dotBr, tab.DirtyRect);
        }

        public virtual void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color accent = context.GetAccentColor(tab);
            int inset = context.TabRadius;
            using var pen = new Pen(accent, DocTokens.IndicatorThickness)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            g.DrawLine(pen,
                tab.TabRect.Left + inset, tab.TabRect.Top + DocTokens.IndicatorThickness / 2,
                tab.TabRect.Right - inset, tab.TabRect.Top + DocTokens.IndicatorThickness / 2);
        }

        public virtual void PaintPinnedTab(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            bool active = tab.IsActive;
            bool hovered = context.IsTabHovered(index);

            Color fill = active ? context.Theme.BackgroundColor
                : hovered ? BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.BorderColor, 0.3f)
                : context.Theme.PanelBackColor;

            if (context.TabStyle == DocumentTabStyle.Chrome)
            {
                using var path = CreateChromeTabPath(tab.TabRect, context.TabRadius);
                using var br = new SolidBrush(fill);
                g.FillPath(br, path);
                using var pen = new Pen(context.Theme.BorderColor, 1f);
                g.DrawPath(pen, path);
            }
            else
            {
                using var br = new SolidBrush(fill);
                g.FillRectangle(br, tab.TabRect);
                using var pen = new Pen(context.Theme.BorderColor, 1f);
                g.DrawLine(pen, tab.TabRect.Right - 1, tab.TabRect.Top, tab.TabRect.Right - 1, tab.TabRect.Bottom);
            }

            if (active) PaintAccentBar(g, tab, context);

            if (!tab.IconRect.IsEmpty)
            {
                PaintTabIcon(g, tab, context);
            }
            else
            {
                int r = context.Scale(5);
                int cx = tab.TabRect.Left + tab.TabRect.Width / 2;
                int cy = tab.TabRect.Top + tab.TabRect.Height / 2 - context.Scale(1);
                Color pinColor = active ? context.Theme.PrimaryColor
                    : BeepDocumentTabStrip.Blend(context.Theme.SecondaryTextColor, context.Theme.PanelBackColor, 0.4f);
                using var br = new SolidBrush(pinColor);
                g.FillEllipse(br, cx - r, cy - r, r * 2, r * 2);
                using var pen = new Pen(pinColor, context.Scale(2));
                g.DrawLine(pen, cx, cy + r, cx, cy + r + context.Scale(4));
            }

            if (tab.IsModified)
            {
                int ds = context.Scale(5);
                using var dotBr = new SolidBrush(context.Theme.WarningColor);
                g.FillEllipse(dotBr,
                    tab.TabRect.Right - ds - context.Scale(3),
                    tab.TabRect.Bottom - ds - context.Scale(3),
                    ds, ds);
            }
        }

        public virtual void PaintEmptyState(Graphics g, Rectangle bounds, TabStripPaintContext context)
        {
            using var hintBr = new SolidBrush(Color.FromArgb(100, context.Theme.SecondaryTextColor));
            using var hintFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };
            g.DrawString("No open documents", context.TextFont, hintBr, (RectangleF)bounds, hintFmt);
        }

        public virtual void PaintStripBackground(Graphics g, Rectangle bounds, TabStripPaintContext context)
        {
            using var bg = new SolidBrush(context.Theme.PanelBackColor);
            g.FillRectangle(bg, bounds);
        }

        public virtual void PaintSeparator(Graphics g, Rectangle bounds, bool isVertical, TabStripPaintContext context)
        {
            using var sep = new Pen(context.Theme.BorderColor, 1);
            if (isVertical)
                g.DrawLine(sep, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            else
                g.DrawLine(sep, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        public virtual void PaintAddButton(Graphics g, Rectangle bounds, bool isHovered, TabStripPaintContext context)
        {
            Color fill = isHovered ? BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.PrimaryColor, 0.25f) : context.Theme.PanelBackColor;
            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, bounds);

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int arm = context.Scale(6);
            using var pen = new Pen(context.Theme.SecondaryTextColor, 1.5f);
            g.DrawLine(pen, cx - arm, cy, cx + arm, cy);
            g.DrawLine(pen, cx, cy - arm, cx, cy + arm);
        }

        public virtual void PaintScrollButton(Graphics g, Rectangle bounds, bool isLeft, bool isHovered, bool isVertical, TabStripPaintContext context)
        {
            Color fill = isHovered ? BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.BorderColor, 0.4f) : context.Theme.PanelBackColor;
            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, bounds);

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int arm = context.Scale(5);
            using var pen = new Pen(context.Theme.SecondaryTextColor, 1.5f);

            if (isVertical)
            {
                if (isLeft)
                {
                    g.DrawLine(pen, cx - arm, cy + arm, cx, cy - arm);
                    g.DrawLine(pen, cx, cy - arm, cx + arm, cy + arm);
                }
                else
                {
                    g.DrawLine(pen, cx - arm, cy - arm, cx, cy + arm);
                    g.DrawLine(pen, cx, cy + arm, cx + arm, cy - arm);
                }
            }
            else
            {
                if (isLeft)
                {
                    g.DrawLine(pen, cx + arm, cy - arm, cx - arm, cy);
                    g.DrawLine(pen, cx - arm, cy, cx + arm, cy + arm);
                }
                else
                {
                    g.DrawLine(pen, cx - arm, cy - arm, cx + arm, cy);
                    g.DrawLine(pen, cx + arm, cy, cx - arm, cy + arm);
                }
            }
        }

        public virtual void PaintOverflowButton(Graphics g, Rectangle bounds, bool isHovered, TabStripPaintContext context)
        {
            if (bounds.IsEmpty) return;
            Color fill = isHovered ? BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.PrimaryColor, 0.25f) : context.Theme.PanelBackColor;
            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, bounds);

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int arm = context.Scale(5);
            var pts = new Point[]
            {
                new Point(cx - arm, cy - arm / 2),
                new Point(cx, cy + arm / 2),
                new Point(cx + arm, cy - arm / 2)
            };
            using var pen = new Pen(context.Theme.SecondaryTextColor, 1.5f);
            pen.EndCap = pen.StartCap = LineCap.Round;
            g.DrawLines(pen, pts);
        }

        public virtual void PaintGroupHeader(Graphics g, TabGroup group, BeepDocumentTab firstTab, TabStripPaintContext context)
        {
            int hdrH = context.Scale(16);
            int tabH = context.Scale(32);
            int x = firstTab.TabRect.Left;
            int y = firstTab.TabRect.Top;

            Color barColor = group.GroupColor.IsEmpty ? context.Theme.BorderColor : group.GroupColor;
            using (var barBr = new SolidBrush(barColor))
                g.FillRectangle(barBr, x, y, context.Scale(4), tabH);

            if (!string.IsNullOrEmpty(group.GroupName))
            {
                using var lblFmt = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap, LineAlignment = StringAlignment.Center };
                using var lblBr = new SolidBrush(Color.FromArgb(160, context.Theme.SecondaryTextColor));
                using var lblFont = new Font(context.TextFont.FontFamily, context.TextFont.SizeInPoints - 1f, FontStyle.Regular);
                var lblRect = new Rectangle(x + context.Scale(6), y, context.Scale(80), tabH);
                g.DrawString(group.GroupName, lblFont, lblBr, (RectangleF)lblRect, lblFmt);
            }

            group.HeaderRect = new Rectangle(x, y, context.Scale(4) + context.Scale(80), tabH);
        }

        public virtual void PaintFocusIndicator(Graphics g, BeepDocumentTab tab, bool isFocused, TabStripPaintContext context)
        {
            if (!isFocused) return;
            if (tab.TabRect.IsEmpty) return;

            var focusRect = Rectangle.Inflate(tab.TabRect, -2, -2);
            if (focusRect.Width < 4 || focusRect.Height < 4) return;

            if (SystemInformation.HighContrast)
            {
                using var pen = new Pen(ColorUtils.MapSystemColor(SystemColors.Highlight), 3f);
                int r = System.Math.Min(context.TabRadius, focusRect.Width / 2);
                using var path = CreateRoundedRect(focusRect, r);
                g.DrawPath(pen, path);
            }
            else
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        public virtual GraphicsPath CreateTabPath(Rectangle bounds, TabStripPaintContext context)
        {
            return CreateChromeTabPath(bounds, context.TabRadius);
        }

        protected static GraphicsPath CreateChromeTabPath(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = radius * 2;
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddLine(r.Right, r.Top + radius, r.Right, r.Bottom);
            p.AddLine(r.Right, r.Bottom, r.Left, r.Bottom);
            p.AddLine(r.Left, r.Bottom, r.Left, r.Top + radius);
            p.CloseFigure();
            return p;
        }

        protected static GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = System.Math.Max(1, radius * 2);
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        protected virtual void PaintTabColorOverlay(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color c = tab.TabColor;
            if (c == Color.Empty) return;
            int barH = context.Scale(3);

            switch (context.ColorMode)
            {
                case TabColorMode.AccentBar:
                    using (var br = new SolidBrush(c))
                        g.FillRectangle(br, new Rectangle(tab.TabRect.Left + 2, tab.TabRect.Top, tab.TabRect.Width - 4, barH));
                    break;
                case TabColorMode.FullBackground:
                    using (var br = new SolidBrush(Color.FromArgb(48, c)))
                        g.FillRectangle(br, tab.TabRect);
                    break;
                case TabColorMode.BottomBorder:
                    using (var br = new SolidBrush(c))
                        g.FillRectangle(br, new Rectangle(tab.TabRect.Left + 2, tab.TabRect.Bottom - barH, tab.TabRect.Width - 4, barH));
                    break;
            }
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Image> _iconCache = new();

        protected static Image TryLoadIcon(string path)
        {
            if (_iconCache.TryGetValue(path, out var cached)) return cached;
            try
            {
                if (!System.IO.File.Exists(path)) return null;
                var img = Image.FromFile(path);
                _iconCache[path] = img;
                return img;
            }
            catch { return null; }
        }
    }
}
