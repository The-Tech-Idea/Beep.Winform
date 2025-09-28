using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map
{
    /// <summary>
    /// AddressCard - Address/location details painter
    /// </summary>
    internal sealed class AddressCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Address icon
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                24, 24
            );
            
            // Address title
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                24
            );
            
            // Address details
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.IconRect.Bottom - pad * 3
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            
            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(30, Color.Black), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            string address = ctx.CustomData.ContainsKey("Address") ? ctx.CustomData["Address"].ToString() : "";
            string city = ctx.CustomData.ContainsKey("City") ? ctx.CustomData["City"].ToString() : "";
            string region = ctx.CustomData.ContainsKey("Region") ? ctx.CustomData["Region"].ToString() : "";
            string country = ctx.CustomData.ContainsKey("Country") ? ctx.CustomData["Country"].ToString() : "";
            string postalCode = ctx.CustomData.ContainsKey("PostalCode") ? ctx.CustomData["PostalCode"].ToString() : "";

            // Draw address icon
            using var iconBrush = new SolidBrush(ctx.AccentColor);
            g.FillEllipse(iconBrush, ctx.IconRect);
            using var iconFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var iconTextBrush = new SolidBrush(Color.White);
            var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("??", iconFont, iconTextBrush, ctx.IconRect, iconFormat);

            // Draw address title
            using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);

            // Draw address components
            int y = ctx.ContentRect.Y;
            int lineHeight = 18;
            
            using var addressFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var addressBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            if (!string.IsNullOrEmpty(address))
            {
                g.DrawString($"Street: {address}", addressFont, addressBrush, ctx.ContentRect.X, y);
                y += lineHeight;
            }
            
            if (!string.IsNullOrEmpty(city))
            {
                g.DrawString($"City: {city}", addressFont, addressBrush, ctx.ContentRect.X, y);
                y += lineHeight;
            }
            
            if (!string.IsNullOrEmpty(region))
            {
                g.DrawString($"Region: {region}", addressFont, addressBrush, ctx.ContentRect.X, y);
                y += lineHeight;
            }
            
            if (!string.IsNullOrEmpty(country))
            {
                g.DrawString($"Country: {country}", addressFont, addressBrush, ctx.ContentRect.X, y);
                y += lineHeight;
            }
            
            if (!string.IsNullOrEmpty(postalCode))
            {
                g.DrawString($"Postal: {postalCode}", addressFont, addressBrush, ctx.ContentRect.X, y);
            }

            // Draw coordinates if available
            double latitude = ctx.CustomData.ContainsKey("Latitude") ? (double)ctx.CustomData["Latitude"] : 0.0;
            double longitude = ctx.CustomData.ContainsKey("Longitude") ? (double)ctx.CustomData["Longitude"] : 0.0;
            
            if (latitude != 0.0 || longitude != 0.0)
            {
                using var coordFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var coordBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                string coordText = $"Coordinates: {latitude:F4}, {longitude:F4}";
                g.DrawString(coordText, coordFont, coordBrush, ctx.ContentRect.X, ctx.ContentRect.Bottom - 16);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw address validation indicators
        }
    }
}