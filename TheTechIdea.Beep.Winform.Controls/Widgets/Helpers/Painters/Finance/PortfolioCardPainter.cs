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

        public PortfolioCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Title area
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Total value area (large)
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Performance area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                60
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
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ? 
                (List<FinanceItem>)ctx.CustomData["FinanceItems"] : new List<FinanceItem>();
            
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
            // Portfolio icon
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "pie-chart", iconRect, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

            // Portfolio title
            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString("Portfolio Value", titleFont, titleBrush, titleRect, format);
        }

        private void DrawPortfolioValue(Graphics g, WidgetContext ctx, decimal totalValue, string currencySymbol)
        {
            using var valueFont = new Font(Owner.Font.FontFamily, 20f, FontStyle.Bold);
            using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            string valueText = $"{currencySymbol}{totalValue:N0}";
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(valueText, valueFont, valueBrush, ctx.ContentRect, valueFormat);
        }

        private void DrawPerformanceMetrics(Graphics g, WidgetContext ctx, decimal percentage, FinanceTrend trend, Color positiveColor, Color negativeColor)
        {
            // Performance indicator with icon
            Color trendColor = trend switch
            {
                FinanceTrend.Up => positiveColor,
                FinanceTrend.Down => negativeColor,
                _ => Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray)
            };

            var perfRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width, 24);
            
            // Trend icon
            string trendIcon = trend switch
            {
                FinanceTrend.Up => "trending-up",
                FinanceTrend.Down => "trending-down",
                _ => "minus"
            };
            
            var iconRect = new Rectangle(perfRect.X, perfRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, trendIcon, iconRect, trendColor, 0.9f);

            // Performance text
            var textRect = new Rectangle(iconRect.Right + 8, perfRect.Y, 
                perfRect.Width - iconRect.Width - 8, perfRect.Height);
            using var perfFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
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
                
                // Draw category dot
                using var categoryBrush = new SolidBrush(Color.FromArgb(100 + i * 50, accentColor));
                g.FillEllipse(categoryBrush, rect.X, y, 8, 8);
                
                // Draw category info
                using var categoryFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var categoryBrush2 = new SolidBrush(Color.FromArgb(140, Color.Black));
                string categoryText = $"{item.Name} ({allocation:F1}%)";
                g.DrawString(categoryText, categoryFont, categoryBrush2, rect.X + 12, y - 2);
                
                y += 16;
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Diversification indicator
            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ? 
                (List<FinanceItem>)ctx.CustomData["FinanceItems"] : new List<FinanceItem>();
            
            if (financeItems.Count >= 5)
            {
                var diversityRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
                using var diversityBrush = new SolidBrush(Color.FromArgb(30, Color.FromArgb(33, 150, 243)));
                g.FillRoundedRectangle(diversityBrush, diversityRect, 4);
                
                var iconRect = new Rectangle(diversityRect.X + 2, diversityRect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "activity", iconRect, 
                    Color.FromArgb(150, Color.FromArgb(33, 150, 243)), 0.8f);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}