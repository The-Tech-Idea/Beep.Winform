using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// MenuBar - Horizontal menu bar navigation
    /// </summary>
    internal sealed class MenuBarPainter : WidgetPainterBase, IDisposable
    {
        private Font? _menuFont;
        private int _overflowStartIndex = int.MaxValue;

        protected override void RebuildFonts()
        {
            _menuFont?.Dispose();
            _menuFont = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f }, true);
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 4,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 8);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgBrush   = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.Empty);
            var borderPen = PaintersFactory.GetPen(Color.FromArgb(30, Theme?.SecondaryTextColor ?? Color.Empty), 1f);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
            g.DrawRectangle(borderPen, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList() ?? CreateSampleMenuItems();
            if (!items.Any()) return;

            var font         = _menuFont ?? SystemFonts.DefaultFont;
            var primaryColor = Theme?.PrimaryColor ?? Color.Empty;
            int iconSz       = Dp(16);
            int overflowW    = Dp(28); // space reserved for ⋯
            int minItemW     = Dp(60);

            // Determine how many items fit
            int naturalW = ctx.ContentRect.Width / items.Count;
            int itemW    = Math.Max(minItemW, naturalW);
            bool overflow = items.Count * itemW > ctx.ContentRect.Width;
            int visibleCount = overflow
                ? Math.Max(1, (ctx.ContentRect.Width - overflowW) / itemW)
                : items.Count;
            _overflowStartIndex = overflow ? visibleCount : int.MaxValue;

            for (int i = 0; i < visibleCount && i < items.Count; i++)
            {
                var item     = items[i];
                var itemRect = new Rectangle(ctx.ContentRect.X + i * itemW, ctx.ContentRect.Y, itemW, ctx.ContentRect.Height);

                if (item.IsActive)
                {
                    var hoverBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, primaryColor));
                    g.FillRectangle(hoverBrush, itemRect);
                }

                // Icon
                var iconSvg = GetMenuIconSvg(item.Text, i);
                if (iconSvg != null)
                {
                    var iconRect = new Rectangle(itemRect.X + Dp(8), itemRect.Y + (itemRect.Height - iconSz) / 2, iconSz, iconSz);
                    using var iconPath = CreateRoundedPath(iconRect, 0);
                    StyledImagePainter.PaintWithTint(g, iconPath, iconSvg, primaryColor, 0.8f);
                }

                // Text
                var textBrush = PaintersFactory.GetSolidBrush(item.IsActive ? primaryColor : (Theme?.ForeColor ?? Color.Empty));
                int textX     = iconSvg != null ? itemRect.X + Dp(28) : itemRect.X + Dp(8);
                var textRect  = new Rectangle(textX, itemRect.Y, itemRect.Right - textX, itemRect.Height);
                var format    = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, font, textBrush, textRect, format);
            }

            // Overflow indicator ⋯
            if (overflow)
            {
                var overflowRect = new Rectangle(ctx.ContentRect.Right - overflowW, ctx.ContentRect.Y, overflowW, ctx.ContentRect.Height);
                var ovBg = PaintersFactory.GetSolidBrush(Color.FromArgb(15, primaryColor));
                g.FillRectangle(ovBg, overflowRect);
                var ovBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
                var ovFmt   = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("\u22ef", font, ovBrush, overflowRect, ovFmt); // ⋯
            }
        }

        private List<NavigationItem> CreateSampleMenuItems()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "File", IsActive = false },
                new NavigationItem { Text = "Edit", IsActive = true },
                new NavigationItem { Text = "View", IsActive = false },
                new NavigationItem { Text = "Help", IsActive = false }
            };
        }

        private string? GetMenuIconSvg(string menuText, int index)
        {
            var text = menuText?.ToLower() ?? "";
            if (text.Contains("file"))  return SvgsUI.FileText;
            if (text.Contains("edit"))  return SvgsUI.Edit;
            if (text.Contains("view"))  return SvgsUI.Eye;
            if (text.Contains("help"))  return SvgsUI.HelpCircle;
            return SvgsUI.Menu;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _menuFont?.Dispose();
        }
    }
}