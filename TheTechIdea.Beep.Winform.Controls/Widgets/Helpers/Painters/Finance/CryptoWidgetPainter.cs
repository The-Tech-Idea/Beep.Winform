using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Linq;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// CryptoWidget - Cryptocurrency progress/stats with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class CryptoWidgetPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive areas
        private Rectangle _iconRect;
        private Rectangle _nameRect;
        private Rectangle _priceRect;
        private Rectangle _changeRect;
        private Rectangle _chartRect;

        public CryptoWidgetPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = baseRect;
            
            // Crypto icon area
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 32, 32);
            _iconRect = ctx.IconRect;

            // Crypto name and symbol
            ctx.HeaderRect = new Rectangle(ctx.IconRect.Right + 12, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3, 32);
            _nameRect = ctx.HeaderRect;

            // Current price
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.IconRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2, 30);
            _priceRect = ctx.ContentRect;

            // Change percentage and chart
            ctx.FooterRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ContentRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad * 2);
            _changeRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width, 20);
            _chartRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y + 20, ctx.FooterRect.Width, Math.Max(0, ctx.FooterRect.Height - 20));
            
            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Crypto_Icon", _iconRect, null, () => { HandleIconClick(ctx); notifyAreaHit?.Invoke("Crypto_Icon", _iconRect); });
            owner.AddHitArea("Crypto_Name", _nameRect, null, () => { HandleNameClick(ctx); notifyAreaHit?.Invoke("Crypto_Name", _nameRect); });
            owner.AddHitArea("Crypto_Price", _priceRect, null, () => { HandlePriceClick(ctx); notifyAreaHit?.Invoke("Crypto_Price", _priceRect); });
            owner.AddHitArea("Crypto_Change", _changeRect, null, () => { HandleChangeClick(ctx); notifyAreaHit?.Invoke("Crypto_Change", _changeRect); });
            owner.AddHitArea("Crypto_Chart", _chartRect, null, () => { HandleChartClick(ctx); notifyAreaHit?.Invoke("Crypto_Chart", _chartRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var brush = new LinearGradientBrush(ctx.DrawingRect, Theme?.BackColor ?? Color.FromArgb(250, 250, 255), Theme?.SecondaryColor ?? Color.FromArgb(240, 245, 255), LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(brush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            decimal value = ctx.CustomData.ContainsKey("PrimaryValue") ? Convert.ToDecimal(ctx.CustomData["PrimaryValue"]) : 0m;
            decimal percentage = ctx.CustomData.ContainsKey("Percentage") ? Convert.ToDecimal(ctx.CustomData["Percentage"]) : 0m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            DrawCryptoIcon(g, ctx);
            DrawCryptoInfo(g, ctx, value, percentage, currencySymbol);
        }

        private void DrawCryptoIcon(Graphics g, WidgetContext ctx)
        {
            string cryptoName = ctx.CustomData.ContainsKey("CryptoName") ? ctx.CustomData["CryptoName"].ToString().ToLower() : "bitcoin";
            string iconName = cryptoName switch { "bitcoin" or "btc" => "dollar-sign", "ethereum" or "eth" => "hexagon", "litecoin" or "ltc" => "circle", "ripple" or "xrp" => "zap", _ => "trending-up" };
            Color cryptoColor = ctx.CustomData.ContainsKey("CryptoColor") ? (Color)ctx.CustomData["CryptoColor"] : (Theme?.AccentColor ?? Color.FromArgb(255, 193, 7));
            _imagePainter.DrawSvg(g, iconName, _iconRect, cryptoColor, 0.9f);
        }

        private void DrawCryptoInfo(Graphics g, WidgetContext ctx, decimal value, decimal percentage, string currencySymbol)
        {
            var trend = ctx.CustomData.ContainsKey("Trend") ? (FinanceTrend)ctx.CustomData["Trend"] : FinanceTrend.Neutral;
            var positiveColor = ctx.CustomData.ContainsKey("PositiveColor") ? (Color)ctx.CustomData["PositiveColor"] : Color.Green;
            var negativeColor = ctx.CustomData.ContainsKey("NegativeColor") ? (Color)ctx.CustomData["NegativeColor"] : Color.Red;

            string cryptoName = ctx.CustomData.ContainsKey("CryptoName") ? ctx.CustomData["CryptoName"].ToString() : "Bitcoin";
            string cryptoSymbol = ctx.CustomData.ContainsKey("CryptoSymbol") ? ctx.CustomData["CryptoSymbol"].ToString() : "BTC";

            // Name and symbol
            var nameIconRect = new Rectangle(_nameRect.X, _nameRect.Y, 16, 16);
            _imagePainter.DrawSvg(g, "circle", nameIconRect, Theme?.AccentColor ?? Color.Gray, 0.6f);
            var nameTextRect = new Rectangle(nameIconRect.Right + 4, _nameRect.Y, _nameRect.Width - nameIconRect.Width - 4, 16);
            using var nameFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            g.DrawString(cryptoName, nameFont, nameBrush, nameTextRect.X, nameTextRect.Y);
            using var symbolFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var symbolBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
            g.DrawString(cryptoSymbol, symbolFont, symbolBrush, nameTextRect.X, nameTextRect.Y + 16);

            // Current price
            bool priceHovered = IsAreaHovered("Crypto_Price");
            using var priceFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 16f, priceHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            using var priceBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            string priceText = $"{currencySymbol}{value:N2}" + (priceHovered ? " - Click to view orderbook" : string.Empty);
            g.DrawString(priceText, priceFont, priceBrush, _priceRect);

            // Change percentage
            Color trendColor = trend == FinanceTrend.Up ? positiveColor : trend == FinanceTrend.Down ? negativeColor : Color.Gray;
            bool changeHovered = IsAreaHovered("Crypto_Change");
            if (changeHovered) trendColor = Color.FromArgb(Math.Min(255, trendColor.R + 30), Math.Min(255, trendColor.G + 30), Math.Min(255, trendColor.B + 30));
            var trendIconRect = new Rectangle(_changeRect.X, _changeRect.Y + 2, 16, 16);
            string trendIconName = trend == FinanceTrend.Up ? "trending-up" : trend == FinanceTrend.Down ? "trending-down" : "minus";
            _imagePainter.DrawSvg(g, trendIconName, trendIconRect, trendColor, 0.8f);
            var changeTextRect = new Rectangle(trendIconRect.Right + 4, _changeRect.Y, _changeRect.Width - trendIconRect.Width - 4, _changeRect.Height);
            using var changeFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
            using var changeBrush = new SolidBrush(trendColor);
            string changeText = $"{percentage:+0.00;-0.00;0.00}%";
            var changeFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(changeText, changeFont, changeBrush, changeTextRect, changeFormat);

            // Mini chart
            DrawMiniChart(g, _chartRect, trendColor);
        }

        private void DrawMiniChart(Graphics g, Rectangle rect, Color trendColor)
        {
            if (rect.Width < 20 || rect.Height < 10) return;
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
            if (IsAreaHovered("Crypto_Icon"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 1);
                g.DrawRectangle(pen, _iconRect);
            }
            if (IsAreaHovered("Crypto_Name"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 1);
                g.DrawRectangle(pen, _nameRect);
            }
            if (IsAreaHovered("Crypto_Price"))
            {
                using var glow = new SolidBrush(Color.FromArgb(14, Theme?.PrimaryColor ?? Color.Blue));
                using var p = CreateRoundedPath(Rectangle.Inflate(_priceRect, 4, 2), 6);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("Crypto_Change"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.AccentColor ?? Color.Gray), 1);
                g.DrawRectangle(pen, _changeRect);
            }
            if (IsAreaHovered("Crypto_Chart"))
            {
                using var glow = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(glow, _chartRect);
            }
        }

        private void HandleIconClick(WidgetContext ctx) { ctx.CustomData["ShowCryptoInfo"] = true; Owner?.Invalidate(); }
        private void HandleNameClick(WidgetContext ctx) { ctx.CustomData["ShowCryptoDetails"] = true; Owner?.Invalidate(); }
        private void HandlePriceClick(WidgetContext ctx) { ctx.CustomData["ShowOrderBook"] = true; Owner?.Invalidate(); }
        private void HandleChangeClick(WidgetContext ctx) { ctx.CustomData["ShowChangeHistory"] = true; Owner?.Invalidate(); }
        private void HandleChartClick(WidgetContext ctx) { ctx.CustomData["ShowPriceChart"] = true; Owner?.Invalidate(); }

        public void Dispose() { _imagePainter?.Dispose(); }
    }
}