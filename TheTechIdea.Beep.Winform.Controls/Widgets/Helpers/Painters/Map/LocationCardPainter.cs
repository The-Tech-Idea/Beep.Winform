using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map
{
    /// <summary>
    /// LocationCard - Location information display painter
    /// </summary>
    internal sealed class LocationCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Location icon area
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                32, 32
            );
            
            // Location name and address
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                32
            );
            
            // Map preview area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                60
            );
            
            // Coordinates and details
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            string address = ctx.Address;
            double latitude = ctx.Latitude;
            double longitude = ctx.Longitude;

            // Draw location icon
            using var iconBrush = new SolidBrush(ctx.AccentColor);
            g.FillEllipse(iconBrush, ctx.IconRect);
            using var iconFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
            using var iconTextBrush = new SolidBrush(Color.White);
            var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("??", iconFont, iconTextBrush, ctx.IconRect, iconFormat);

            // Draw location name and address
            using var nameFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(ctx.Title, nameFont, nameBrush, ctx.HeaderRect.X, ctx.HeaderRect.Y);
            
            using var addressFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var addressBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            g.DrawString(address, addressFont, addressBrush, ctx.HeaderRect.X, ctx.HeaderRect.Y + 16);

            // Draw map preview placeholder
            DrawMapPreview(g, ctx.ContentRect, ctx.AccentColor);

            // Draw coordinates
            using var coordFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var coordBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string coordText = $"Lat: {latitude:F4}, Lng: {longitude:F4}";
            g.DrawString(coordText, coordFont, coordBrush, ctx.FooterRect);

            // Draw last updated
            using var updateFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var updateBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            var lastUpdated = ctx.LastUpdated;
            string updateText = $"Updated: {lastUpdated:HH:mm}";
            var updateSize = TextUtils.MeasureText(g,updateText, updateFont);
            g.DrawString(updateText, updateFont, updateBrush, ctx.FooterRect.Right - updateSize.Width, ctx.FooterRect.Y);
        }

        private void DrawMapPreview(Graphics g, Rectangle rect, Color accentColor)
        {
            if (rect.Width < 20 || rect.Height < 20) return;

            // Draw map background
            using var mapBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
            g.FillRectangle(mapBrush, rect);

            // Draw grid lines to simulate map
            using var gridPen = new Pen(Color.FromArgb(220, 220, 220), 1);
            int gridSize = 20;
            for (int x = rect.X; x < rect.Right; x += gridSize)
            {
                g.DrawLine(gridPen, x, rect.Y, x, rect.Bottom);
            }
            for (int y = rect.Y; y < rect.Bottom; y += gridSize)
            {
                g.DrawLine(gridPen, rect.X, y, rect.Right, y);
            }

            // Draw location marker in center
            int markerSize = 12;
            var markerRect = new Rectangle(
                rect.X + rect.Width / 2 - markerSize / 2,
                rect.Y + rect.Height / 2 - markerSize / 2,
                markerSize, markerSize
            );
            using var markerBrush = new SolidBrush(accentColor);
            g.FillEllipse(markerBrush, markerRect);

            // Draw border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 1);
            g.DrawRectangle(borderPen, rect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw additional location indicators
        }
    }
}