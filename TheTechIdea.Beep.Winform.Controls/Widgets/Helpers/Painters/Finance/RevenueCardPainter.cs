using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// RevenueCard - Revenue tracking display painter
    /// </summary>
    internal sealed class RevenueCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public RevenueCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }

            // Revenue content area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );

            // Icon area (top-right corner)
            if (ctx.ShowIcon)
            {
                ctx.IconRect = new Rectangle(ctx.DrawingRect.Right - 32, ctx.DrawingRect.Top + 8, 24, 24);
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw main card background with subtle gradient for revenue (positive)
            using var bgBrush = new LinearGradientBrush(ctx.DrawingRect,
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(248, 252, 248), // Slight green tint
                LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Draw subtle border
            using var borderPen = new Pen(Color.FromArgb(220, 220, 220), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            // Draw title with revenue icon
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                DrawRevenueHeader(g, ctx);
            }

            // Draw revenue data
            DrawRevenueData(g, ctx);

            // Draw icon if enabled
            if (ctx.ShowIcon && !ctx.IconRect.IsEmpty)
            {
                DrawRevenueIcon(g, ctx);
            }
        }

        private void DrawRevenueHeader(Graphics g, WidgetContext ctx)
        {
            // Revenue icon
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "trending-up", iconRect, 
                Color.FromArgb(76, 175, 80), 0.9f);

            // Revenue title
            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush, titleRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw growth trend indicator
            DrawGrowthIndicator(g, ctx);

            // Draw revenue category badge
            DrawRevenueBadge(g, ctx);
        }

        private void DrawRevenueData(Graphics g, WidgetContext ctx)
        {
            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ?
                ctx.CustomData["FinanceItems"] as List<FinanceItem> : null;

            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ?
                ctx.CustomData["CurrencySymbol"]?.ToString() ?? "$" : "$";

            bool showCurrency = ctx.CustomData.ContainsKey("ShowCurrency") ?
                Convert.ToBoolean(ctx.CustomData["ShowCurrency"]) : true;

            if (financeItems != null && financeItems.Count > 0)
            {
                // Display total revenue (positive values)
                double totalRevenue = financeItems.Where(item => item.Value > 0).Sum(item => item.Value);

                using var amountFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
                using var amountBrush = new SolidBrush(GetRevenueColor(ctx, totalRevenue));

                string amountText = showCurrency ?
                    $"{currencySymbol}{totalRevenue.ToString("N0", CultureInfo.CurrentCulture)}" :
                    totalRevenue.ToString("N0", CultureInfo.CurrentCulture);

                var amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10,
                                             ctx.ContentRect.Width, 20);
                g.DrawString(amountText, amountFont, amountBrush, amountRect);

                // Display revenue transaction count
                int revenueCount = financeItems.Count(item => item.Value > 0);
                using var countFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var countBrush = new SolidBrush(Color.FromArgb(150, Color.Black));

                string countText = $"{revenueCount} revenue transaction{(revenueCount != 1 ? "s" : "")}";
                var countRect = new Rectangle(ctx.ContentRect.Left, amountRect.Bottom + 4,
                                            ctx.ContentRect.Width, 14);
                g.DrawString(countText, countFont, countBrush, countRect);

                // Display period and growth
                var lastUpdated = ctx.CustomData.ContainsKey("LastUpdated") ?
                    ctx.CustomData["LastUpdated"] : null;
                if (lastUpdated != null)
                {
                    using var periodFont = new Font(Owner.Font.Font.FontFamily, 7f, FontStyle.Regular);
                    using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

                    string periodText = $"Updated {lastUpdated}";
                    var periodRect = new Rectangle(ctx.ContentRect.Left, countRect.Bottom + 2,
                                                 ctx.ContentRect.Width, 12);
                    g.DrawString(periodText, periodFont, periodBrush, periodRect);
                }
            }
            else
            {
                // Draw sample revenue data
                DrawSampleRevenueData(g, ctx, currencySymbol, showCurrency);
            }
        }

        private void DrawSampleRevenueData(Graphics g, WidgetContext ctx, string currencySymbol, bool showCurrency)
        {
            using var amountFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(Color.FromArgb(34, 139, 34)); // Forest green for revenue

            string amountText = showCurrency ? $"{currencySymbol}45,230" : "45230";
            var amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10,
                                         ctx.ContentRect.Width, 20);
            g.DrawString(amountText, amountFont, amountBrush, amountRect);

            using var countFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var countBrush = new SolidBrush(Color.FromArgb(150, Color.Black));

            string countText = "28 transactions";
            var countRect = new Rectangle(ctx.ContentRect.Left, amountRect.Bottom + 4,
                                        ctx.ContentRect.Width, 14);
            g.DrawString(countText, countFont, countBrush, countRect);

            using var periodFont = new Font(Owner.Font.Font.FontFamily, 7f, FontStyle.Regular);
            using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            string periodText = "This month";
            var periodRect = new Rectangle(ctx.ContentRect.Left, countRect.Bottom + 2,
                                         ctx.ContentRect.Width, 12);
            g.DrawString(periodText, periodFont, periodBrush, periodRect);
        }

        private void DrawRevenueIcon(Graphics g, WidgetContext ctx)
        {
            // Draw a revenue/trending up icon
            using var iconBrush = new SolidBrush(Color.FromArgb(34, 139, 34)); // Green for revenue
            g.FillEllipse(iconBrush, ctx.IconRect);

            // Draw upward arrow symbol
            using var symbolFont = new Font("Arial", 12f, FontStyle.Bold);
            using var symbolBrush = new SolidBrush(Color.White);
            g.DrawString("↗", symbolFont, symbolBrush, ctx.IconRect,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private void DrawGrowthIndicator(Graphics g, WidgetContext ctx)
        {
            // Draw growth percentage if available
            var trend = ctx.CustomData.ContainsKey("Trend") ? ctx.CustomData["Trend"]?.ToString() : null;
            if (!string.IsNullOrEmpty(trend))
            {
                using var trendFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                Color trendColor = trend.Contains("+") || trend.Contains("↑") ? Color.Green : Color.Red;
                using var trendBrush = new SolidBrush(trendColor);

                var trendRect = new Rectangle(ctx.ContentRect.Right - 50, ctx.ContentRect.Top + 8, 45, 12);
                g.DrawString(trend, trendFont, trendBrush, trendRect,
                           new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawRevenueBadge(Graphics g, WidgetContext ctx)
        {
            // Draw a small revenue badge indicator
            var badgeRect = new Rectangle(ctx.DrawingRect.Left + 8, ctx.DrawingRect.Top + 8, 12, 12);
            using var badgeBrush = new SolidBrush(Color.FromArgb(34, 139, 34));
            g.FillEllipse(badgeBrush, badgeRect);

            // Draw small dollar symbol
            using var badgeFont = new Font("Arial", 8f, FontStyle.Bold);
            using var badgeSymbolBrush = new SolidBrush(Color.White);
            g.DrawString("$", badgeFont, badgeSymbolBrush, badgeRect,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private Color GetRevenueColor(WidgetContext ctx, double amount)
        {
            // Color code based on revenue amount ranges (positive = good)
            if (amount > 50000)
                return Color.FromArgb(34, 139, 34); // Dark green for high revenue
            else if (amount > 10000)
                return Color.FromArgb(50, 205, 50); // Lime green for good revenue
            else
                return ctx.AccentColor; // Accent color for moderate revenue
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}