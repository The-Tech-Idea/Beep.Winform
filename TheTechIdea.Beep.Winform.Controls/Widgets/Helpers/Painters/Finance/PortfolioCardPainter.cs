using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// PortfolioCard - Investment portfolio display with enhanced visual presentation
    /// </summary>
    internal sealed class PortfolioCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive zones
        private Rectangle _titleRect;
        private Rectangle _valueRect;
        private Rectangle _performanceRect;

        public PortfolioCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Owner?.DrawingRect ?? drawingRect;
            
            // Title area
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            // Total value area
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 40);
            // Performance area
            ctx.FooterRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ContentRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 60);

            // Store interactive rects
            _titleRect = ctx.HeaderRect;
            _valueRect = ctx.ContentRect;
            _performanceRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width, 24);
            
            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Portfolio_Title", _titleRect, null, () => { HandleTitleClick(ctx); notifyAreaHit?.Invoke("Portfolio_Title", _titleRect); });
            owner.AddHitArea("Portfolio_Value", _valueRect, null, () => { HandleValueClick(ctx); notifyAreaHit?.Invoke("Portfolio_Value", _valueRect); });
            owner.AddHitArea("Portfolio_Performance", _performanceRect, null, () => { HandlePerformanceClick(ctx); notifyAreaHit?.Invoke("Portfolio_Performance", _performanceRect); });
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
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ? (List<FinanceItem>)ctx.CustomData["FinanceItems"] : new List<FinanceItem>();
            decimal totalValue = ctx.CustomData.ContainsKey("PrimaryValue") ? (decimal)ctx.CustomData["PrimaryValue"] : 250000m;
            decimal percentage = ctx.CustomData.ContainsKey("Percentage") ? (decimal)ctx.CustomData["Percentage"] : 12.5m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";
            var trend = ctx.CustomData.ContainsKey("Trend") ? (FinanceTrend)ctx.CustomData["Trend"] : FinanceTrend.Up;
            var positiveColor = Color.FromArgb(76, 175, 80);
            var negativeColor = Color.FromArgb(244, 67, 54);

            DrawPortfolioHeader(g, ctx);
            DrawPortfolioValue(g, ctx, totalValue, currencySymbol);
            DrawPerformanceMetrics(g, ctx, percentage, trend, positiveColor, negativeColor);
            
            if (financeItems.Any())
            {
                DrawPortfolioBreakdown(g, ctx.FooterRect, financeItems, ctx.AccentColor);
            }
        }

        private void DrawPortfolioHeader(Graphics g, WidgetContext ctx)
        {
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "pie-chart", iconRect, Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            string titleText = ctx.Title ?? "Portfolio Value";
            g.DrawString(titleText, titleFont, titleBrush, titleRect, format);
        }

        private void DrawPortfolioValue(Graphics g, WidgetContext ctx, decimal totalValue, string currencySymbol)
        {
            bool hovered = IsAreaHovered("Portfolio_Value");
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 20f, hovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            string valueText = $"{currencySymbol}{totalValue:N0}" + (hovered ? " - Click to drilldown" : string.Empty);
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(valueText, valueFont, valueBrush, ctx.ContentRect, valueFormat);
        }

        private void DrawPerformanceMetrics(Graphics g, WidgetContext ctx, decimal percentage, FinanceTrend trend, Color positiveColor, Color negativeColor)
        {
            Color trendColor = trend switch { FinanceTrend.Up => positiveColor, FinanceTrend.Down => negativeColor, _ => Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray) };
            var perfRect = _performanceRect;
            string trendIcon = trend switch { FinanceTrend.Up => "trending-up", FinanceTrend.Down => "trending-down", _ => "minus" };
            var iconRect = new Rectangle(perfRect.X, perfRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, trendIcon, iconRect, trendColor, 0.9f);
            var textRect = new Rectangle(iconRect.Right + 8, perfRect.Y, perfRect.Width - iconRect.Width - 8, perfRect.Height);
            using var perfFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var perfBrush = new SolidBrush(trendColor);
            string perfText = $"{(percentage >= 0 ? "+" : "")}{percentage:F2}%";
            var perfFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(perfText, perfFont, perfBrush, textRect, perfFormat);
        }

        private void DrawPortfolioBreakdown(Graphics g, Rectangle rect, List<FinanceItem> items, Color accentColor)
        {
            int y = rect.Y + 25;
            decimal totalValue = items.Sum(x => x.Value);
            for (int i = 0; i < Math.Min(items.Count, 3); i++)
            {
                var item = items[i];
                decimal allocation = totalValue > 0 ? (item.Value / totalValue) * 100 : 0;
                using var categoryBrush = new SolidBrush(Color.FromArgb(100 + i * 50, accentColor));
                g.FillEllipse(categoryBrush, rect.X, y, 8, 8);
                using var categoryFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
                using var categoryBrush2 = new SolidBrush(Color.FromArgb(140, Color.Black));
                string categoryText = $"{item.Name} ({allocation:F1}%)";
                g.DrawString(categoryText, categoryFont, categoryBrush2, rect.X + 12, y - 2);
                y += 16;
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (IsAreaHovered("Portfolio_Title"))
            {
                using var pen = new Pen(Color.FromArgb(100, Theme?.PrimaryColor ?? Color.Blue), 1);
                g.DrawRectangle(pen, _titleRect);
            }
            if (IsAreaHovered("Portfolio_Value"))
            {
                using var glow = new SolidBrush(Color.FromArgb(14, Theme?.PrimaryColor ?? Color.Blue));
                using var p = CreateRoundedPath(Rectangle.Inflate(_valueRect, 4, 4), 8);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("Portfolio_Performance"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.AccentColor ?? Color.Gray), 1);
                g.DrawRectangle(pen, _performanceRect);
            }
        }

        private void HandleTitleClick(WidgetContext ctx) { ctx.CustomData["ShowPortfolioDetails"] = true; Owner?.Invalidate(); }
        private void HandleValueClick(WidgetContext ctx) { ctx.CustomData["ShowValueHistory"] = true; Owner?.Invalidate(); }
        private void HandlePerformanceClick(WidgetContext ctx) { ctx.CustomData["ShowPerformance"] = true; Owner?.Invalidate(); }

        public void Dispose() { _imagePainter?.Dispose(); }
    }
}