using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// CryptoWidget - Cryptocurrency progress/stats with enhanced visual presentation
    /// </summary>
    internal sealed class CryptoWidgetPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public CryptoWidgetPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Crypto icon area
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                32, 32
            );
            
            // Crypto name and symbol
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                32
            );
            
            // Current price
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                30
            );
            
            // Change percentage and chart
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
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            
            // Gradient background for crypto feel
            using var brush = new LinearGradientBrush(ctx.DrawingRect, 
                Color.FromArgb(250, 250, 255), 
                Color.FromArgb(240, 245, 255), 
                LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(brush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            decimal value = ctx.CustomData.ContainsKey("PrimaryValue") ? (decimal)ctx.CustomData["PrimaryValue"] : 0m;
            decimal percentage = ctx.CustomData.ContainsKey("Percentage") ? (decimal)ctx.CustomData["Percentage"] : 0m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            DrawCryptoIcon(g, ctx);
            DrawCryptoInfo(g, ctx, value, percentage, currencySymbol);
        }

        private void DrawCryptoIcon(Graphics g, WidgetContext ctx)
        {
            // Enhanced crypto icon with dynamic selection
            string cryptoName = ctx.CustomData.ContainsKey("CryptoName") ? 
                ctx.CustomData["CryptoName"].ToString().ToLower() : "bitcoin";
            
            // Map crypto names to appropriate icons
            string iconName = cryptoName switch
            {
                "bitcoin" or "btc" => "dollar-sign", // Bitcoin representation
                "ethereum" or "eth" => "hexagon", // Ethereum representation
                "litecoin" or "ltc" => "circle", // Litecoin representation
                "ripple" or "xrp" => "zap", // Ripple representation
                _ => "trending-up" // Default crypto icon
            };

            Color cryptoColor = ctx.CustomData.ContainsKey("CryptoColor") ? 
                (Color)ctx.CustomData["CryptoColor"] : Color.FromArgb(255, 193, 7); // Bitcoin gold
            
            _imagePainter.DrawSvg(g, iconName, ctx.IconRect, cryptoColor, 0.9f);
        }

        private void DrawCryptoInfo(Graphics g, WidgetContext ctx, decimal value, decimal percentage, string currencySymbol)
        {
            var trend = ctx.CustomData.ContainsKey("Trend") ? (FinanceTrend)ctx.CustomData["Trend"] : FinanceTrend.Neutral;
            var positiveColor = ctx.CustomData.ContainsKey("PositiveColor") ? (Color)ctx.CustomData["PositiveColor"] : Color.Green;
            var negativeColor = ctx.CustomData.ContainsKey("NegativeColor") ? (Color)ctx.CustomData["NegativeColor"] : Color.Red;

            // Enhanced crypto name display with icon
            string cryptoName = ctx.CustomData.ContainsKey("CryptoName") ? 
                ctx.CustomData["CryptoName"].ToString() : "Bitcoin";
            string cryptoSymbol = ctx.CustomData.ContainsKey("CryptoSymbol") ? 
                ctx.CustomData["CryptoSymbol"].ToString() : "BTC";

            // Draw crypto name with symbol
            var nameIconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, 16, 16);
            _imagePainter.DrawSvg(g, "circle", nameIconRect, Theme?.AccentColor ?? Color.Gray, 0.6f);

            var nameTextRect = new Rectangle(nameIconRect.Right + 4, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - nameIconRect.Width - 4, 16);
            using var nameFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            g.DrawString(cryptoName, nameFont, nameBrush, nameTextRect.X, nameTextRect.Y);
            
            // Draw symbol
            using var symbolFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var symbolBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
            g.DrawString(cryptoSymbol, symbolFont, symbolBrush, nameTextRect.X, nameTextRect.Y + 16);

            // Draw current price with trend
            using var priceFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
            using var priceBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            string priceText = $"{currencySymbol}{value:N2}";
            g.DrawString(priceText, priceFont, priceBrush, ctx.ContentRect);

            // Enhanced percentage change with trend icon
            Color trendColor = trend == FinanceTrend.Up ? positiveColor : 
                             trend == FinanceTrend.Down ? negativeColor : Color.Gray;
            
            var changeRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width, 20);
            var trendIconRect = new Rectangle(changeRect.X, changeRect.Y + 2, 16, 16);
            
            string trendIconName = trend == FinanceTrend.Up ? "trending-up" : 
                                 trend == FinanceTrend.Down ? "trending-down" : "minus";
            _imagePainter.DrawSvg(g, trendIconName, trendIconRect, trendColor, 0.8f);

            var changeTextRect = new Rectangle(trendIconRect.Right + 4, changeRect.Y, 
                changeRect.Width - trendIconRect.Width - 4, changeRect.Height);
            using var changeFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var changeBrush = new SolidBrush(trendColor);
            string changeText = $"{percentage:+0.00;-0.00;0.00}%";
            var changeFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(changeText, changeFont, changeBrush, changeTextRect, changeFormat);

            // Draw mini price chart placeholder
            DrawMiniChart(g, new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y + 20, ctx.FooterRect.Width, 30), trendColor);
        }

        private void DrawMiniChart(Graphics g, Rectangle rect, Color trendColor)
        {
            if (rect.Width < 20 || rect.Height < 10) return;

            // Simple price chart simulation
            var points = new List<PointF>();
            int steps = 20;
            for (int i = 0; i < steps; i++)
            {
                float x = rect.X + (float)i / (steps - 1) * rect.Width;
                float y = rect.Y + rect.Height / 2 + (float)Math.Sin(i * 0.5) * rect.Height / 4;
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                using var pen = new Pen(trendColor, 2);
                g.DrawLines(pen, points.ToArray());
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw live indicator for real-time crypto data
            if (ctx.CustomData.ContainsKey("IsLive") && (bool)ctx.CustomData["IsLive"])
            {
                var liveRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
                using var liveBrush = new SolidBrush(Color.FromArgb(30, Color.Green));
                g.FillRoundedRectangle(liveBrush, liveRect, 4);
                
                var liveIconRect = new Rectangle(liveRect.X + 2, liveRect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "radio", liveIconRect, Color.FromArgb(150, Color.Green), 0.7f);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}