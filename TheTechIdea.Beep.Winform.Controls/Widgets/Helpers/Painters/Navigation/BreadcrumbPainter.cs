using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Breadcrumb - Breadcrumb navigation with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class BreadcrumbPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private readonly List<(Rectangle rect, int index)> _itemRects = new();
        private Rectangle _homeRect;

        public BreadcrumbPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Breadcrumb items take full area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            _itemRects.Clear();
            _homeRect = Rectangle.Empty;
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Minimal background for breadcrumbs
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleBreadcrumb();
            
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : items.Count - 1;
            
            if (!items.Any()) return;
            
            DrawModernBreadcrumb(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleBreadcrumb()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Dashboard", IsActive = false },
                new NavigationItem { Text = "Analytics", IsActive = false },
                new NavigationItem { Text = "Reports", IsActive = true }
            };
        }

        private void DrawModernBreadcrumb(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            using var regularFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular);
            using var activeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular);
            using var regularBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
            using var activeBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            
            int x = ctx.ContentRect.X + 8;
            int y = ctx.ContentRect.Y + ctx.ContentRect.Height / 2;
            
            // Home icon
            _homeRect = new Rectangle(x, y - 8, 16, 16);
            _imagePainter.DrawSvg(g, "home", _homeRect, Theme?.ForeColor ?? Color.Black, 0.9f);
            x += 24;
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isActive = i == currentIndex;
                
                // Separator chevron (except before first item)
                if (i > 0)
                {
                    var chevronRect = new Rectangle(x, y - 6, 12, 12);
                    _imagePainter.DrawSvg(g, "chevron-right", chevronRect, Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray), 0.8f);
                    x += 16;
                }
                
                // Item text
                var font = isActive ? activeFont : regularFont;
                var brush = isActive ? activeBrush : regularBrush;
                var textSize = g.MeasureString(item.Text, font);
                var textRect = new Rectangle(x - 4, y - 10, (int)textSize.Width + 8, 20);

                // Item background for active or hovered item
                if (isActive || IsAreaHovered($"Breadcrumb_Item_{i}"))
                {
                    using var bgBrush = new SolidBrush(Color.FromArgb(isActive ? 15 : 8, Theme?.PrimaryColor ?? Color.Blue));
                    using var bgPath = CreateRoundedPath(textRect, 4);
                    g.FillPath(bgBrush, bgPath);
                }
                
                // Item text
                var textPoint = new PointF(x, y - textSize.Height / 2);
                g.DrawString(item.Text, font, brush, textPoint);
                
                _itemRects.Add((textRect, i));
                x += (int)textSize.Width + 8;
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Home hover accent
            if (!_homeRect.IsEmpty && IsAreaHovered("Breadcrumb_Home"))
            {
                using var hover = new SolidBrush(Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue));
                g.FillEllipse(hover, Rectangle.Inflate(_homeRect, 4, 4));
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Home
            if (!_homeRect.IsEmpty)
            {
                owner.AddHitArea("Breadcrumb_Home", _homeRect, null, () =>
                {
                    ctx.CustomData["BreadcrumbHomeClicked"] = true;
                    notifyAreaHit?.Invoke("Breadcrumb_Home", _homeRect);
                    Owner?.Invalidate();
                });
            }

            // Items
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = _itemRects[i].index;
                var rect = _itemRects[i].rect;
                owner.AddHitArea($"Breadcrumb_Item_{idx}", rect, null, () =>
                {
                    ctx.CustomData["BreadcrumbIndex"] = idx;
                    notifyAreaHit?.Invoke($"Breadcrumb_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}