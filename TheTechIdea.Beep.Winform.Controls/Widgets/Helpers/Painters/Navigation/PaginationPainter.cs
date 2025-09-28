using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Pagination - Page navigation with modern styling
    /// </summary>
    internal sealed class PaginationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public PaginationPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            int currentPage = ctx.CustomData.ContainsKey("CurrentPage") ? (int)ctx.CustomData["CurrentPage"] : 1;
            int totalPages = ctx.CustomData.ContainsKey("TotalPages") ? (int)ctx.CustomData["TotalPages"] : 10;
            
            DrawModernPagination(g, ctx, currentPage, totalPages);
        }

        private void DrawModernPagination(Graphics g, WidgetContext ctx, int currentPage, int totalPages)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int buttonSize = 32;
            int spacing = 4;
            int maxVisiblePages = 7; // Show max 7 page numbers
            
            using var pageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);
            
            // Calculate visible pages range
            int startPage = Math.Max(1, currentPage - 3);
            int endPage = Math.Min(totalPages, startPage + maxVisiblePages - 1);
            if (endPage - startPage + 1 < maxVisiblePages)
                startPage = Math.Max(1, endPage - maxVisiblePages + 1);
            
            int x = ctx.ContentRect.X;
            
            // Previous button
            var prevRect = new Rectangle(x, ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2, buttonSize, buttonSize);
            DrawPageButton(g, prevRect, "", currentPage > 1, false, "chevron-left");
            x += buttonSize + spacing;
            
            // First page + ellipsis if needed
            if (startPage > 1)
            {
                var firstRect = new Rectangle(x, ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2, buttonSize, buttonSize);
                DrawPageButton(g, firstRect, "1", true, currentPage == 1);
                x += buttonSize + spacing;
                
                if (startPage > 2)
                {
                    using var ellipsisBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.DrawString("...", pageFont, ellipsisBrush, new Point(x + 8, ctx.ContentRect.Y + ctx.ContentRect.Height / 2 - 6));
                    x += 24;
                }
            }
            
            // Page numbers
            for (int page = startPage; page <= endPage; page++)
            {
                var pageRect = new Rectangle(x, ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2, buttonSize, buttonSize);
                DrawPageButton(g, pageRect, page.ToString(), true, page == currentPage);
                x += buttonSize + spacing;
            }
            
            // Last page + ellipsis if needed
            if (endPage < totalPages)
            {
                if (endPage < totalPages - 1)
                {
                    using var ellipsisBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.DrawString("...", pageFont, ellipsisBrush, new Point(x + 8, ctx.ContentRect.Y + ctx.ContentRect.Height / 2 - 6));
                    x += 24;
                }
                
                var lastRect = new Rectangle(x, ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2, buttonSize, buttonSize);
                DrawPageButton(g, lastRect, totalPages.ToString(), true, currentPage == totalPages);
                x += buttonSize + spacing;
            }
            
            // Next button
            var nextRect = new Rectangle(x, ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2, buttonSize, buttonSize);
            DrawPageButton(g, nextRect, "", currentPage < totalPages, false, "chevron-right");
        }

        private void DrawPageButton(Graphics g, Rectangle rect, string text, bool enabled, bool isActive, string iconName = null)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            
            // Button background
            Color bgColor = isActive ? primaryColor : enabled ? Color.White : Color.FromArgb(248, 249, 250);
            Color borderColor = isActive ? primaryColor : Color.FromArgb(220, 220, 220);
            
            using var bgBrush = new SolidBrush(bgColor);
            using var borderPen = new Pen(borderColor, 1);
            using var buttonPath = CreateRoundedPath(rect, 4);
            
            g.FillPath(bgBrush, buttonPath);
            g.DrawPath(borderPen, buttonPath);
            
            // Button content
            if (!string.IsNullOrEmpty(iconName))
            {
                // Navigation icon
                var iconRect = new Rectangle(rect.X + (rect.Width - 16) / 2, rect.Y + (rect.Height - 16) / 2, 16, 16);
                Color iconColor = enabled ? (isActive ? Color.White : primaryColor) : Color.FromArgb(150, Color.Gray);
                _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, 0.8f);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                // Page number
                using var textFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);
                Color textColor = isActive ? Color.White : enabled ? Color.FromArgb(60, 60, 60) : Color.FromArgb(150, Color.Gray);
                using var textBrush = new SolidBrush(textColor);
                
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, textFont, textBrush, rect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Page info text
            if (ctx.CustomData.ContainsKey("ShowPageInfo") && (bool)ctx.CustomData["ShowPageInfo"])
            {
                int currentPage = ctx.CustomData.ContainsKey("CurrentPage") ? (int)ctx.CustomData["CurrentPage"] : 1;
                int totalPages = ctx.CustomData.ContainsKey("TotalPages") ? (int)ctx.CustomData["TotalPages"] : 10;
                
                using var infoFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var infoBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                
                var infoText = $"Page {currentPage} of {totalPages}";
                var infoRect = new Rectangle(ctx.ContentRect.Right - 80, ctx.ContentRect.Y, 80, ctx.ContentRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(infoText, infoFont, infoBrush, infoRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}