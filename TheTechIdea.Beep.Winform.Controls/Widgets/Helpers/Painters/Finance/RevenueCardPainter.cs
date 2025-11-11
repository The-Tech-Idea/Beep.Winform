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
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// RevenueCard - Revenue tracking display painter
    /// </summary>
    internal sealed class RevenueCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive areas
        private Rectangle _amountRect;
        private Rectangle _countRect;
        private Rectangle _trendRect;
        private Rectangle _iconRect;

        public RevenueCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = baseRect;

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

            // Compute common content rectangles for interaction
            _amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10, ctx.ContentRect.Width, 20);
            _countRect = new Rectangle(ctx.ContentRect.Left, _amountRect.Bottom + 4, ctx.ContentRect.Width, 14);
            _trendRect = new Rectangle(ctx.ContentRect.Right - 50, ctx.ContentRect.Top + 8, 45, 12);
            _iconRect = ctx.IconRect;

            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Revenue_Amount", _amountRect, null, () => { HandleAmountClick(ctx); notifyAreaHit?.Invoke("Revenue_Amount", _amountRect); });
            owner.AddHitArea("Revenue_Count", _countRect, null, () => { HandleCountClick(ctx); notifyAreaHit?.Invoke("Revenue_Count", _countRect); });
            owner.AddHitArea("Revenue_Trend", _trendRect, null, () => { HandleTrendClick(ctx); notifyAreaHit?.Invoke("Revenue_Trend", _trendRect); });
            if (!_iconRect.IsEmpty)
                owner.AddHitArea("Revenue_Icon", _iconRect, null, () => { HandleIconClick(ctx); notifyAreaHit?.Invoke("Revenue_Icon", _iconRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new LinearGradientBrush(ctx.DrawingRect,
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(248, 252, 248),
                LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(Color.FromArgb(220, 220, 220), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

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
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "trending-up", iconRect, Color.FromArgb(76, 175, 80), 0.9f);

            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y,
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush, titleRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover accents
            if (IsAreaHovered("Revenue_Amount"))
            {
                using var glow = new SolidBrush(Color.FromArgb(16, 76, 175, 80));
                var r = Rectangle.Inflate(_amountRect, 4, 2);
                using var p = CreateRoundedPath(r, 4);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("Revenue_Trend"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Green), 1);
                g.DrawRectangle(pen, _trendRect);
            }
            if (IsAreaHovered("Revenue_Icon") && !_iconRect.IsEmpty)
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Green), 1);
                g.DrawEllipse(pen, _iconRect);
            }
        }

        private void DrawRevenueData(Graphics g, WidgetContext ctx)
        {
            var financeItems = ctx.FinanceItems?.Cast<FinanceItem>().ToList() ?? new List<FinanceItem>();

            string currencySymbol = ctx.CurrencySymbol ?? "$";

            bool showCurrency = ctx.ShowCurrency;

            if (financeItems.Count > 0)
            {
                // Sum might be decimal depending on FinanceItem.Value type; convert explicitly to double
                var totalRevenueDecimal = financeItems.Where(item => item.Value > 0).Sum(item => item.Value);
                double totalRevenue = (double)totalRevenueDecimal;

                bool amountHovered = IsAreaHovered("Revenue_Amount");
                using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 14f, amountHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
                using var amountBrush = new SolidBrush(GetRevenueColor(ctx, totalRevenue));

                string amountText = showCurrency ?
                    $"{currencySymbol}{totalRevenue.ToString("N0", CultureInfo.CurrentCulture)}" :
                    totalRevenue.ToString("N0", CultureInfo.CurrentCulture);
                if (amountHovered) amountText += " - Click to drilldown";

                g.DrawString(amountText, amountFont, amountBrush, _amountRect);

                int revenueCount = financeItems.Count(item => item.Value > 0);
                bool countHovered = IsAreaHovered("Revenue_Count");
                using var countFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, countHovered ? FontStyle.Underline : FontStyle.Regular);
                using var countBrush = new SolidBrush(countHovered ? Theme?.PrimaryColor ?? Color.Green : Color.FromArgb(150, Color.Black));

                string countText = $"{revenueCount} revenue transaction{(revenueCount != 1 ? "s" : "")}";
                g.DrawString(countText, countFont, countBrush, _countRect);

                var lastUpdated = ctx.LastUpdated;
                if (lastUpdated != null)
                {
                    using var periodFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                    using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    string periodText = $"Updated {lastUpdated}";
                    var periodRect = new Rectangle(_countRect.Left, _countRect.Bottom + 2, _countRect.Width, 12);
                    g.DrawString(periodText, periodFont, periodBrush, periodRect);
                }

                // Trend area visual
                string trend = ctx.Trend;
                if (!string.IsNullOrEmpty(trend))
                {
                    using var trendFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Bold);
                    Color trendColor = trend.Contains("+") || trend.Contains("↑") ? Color.Green : Color.Red;
                    bool trendHovered = IsAreaHovered("Revenue_Trend");
                    if (trendHovered)
                        trendColor = Color.FromArgb(Math.Min(255, trendColor.R + 30), Math.Min(255, trendColor.G + 30), Math.Min(255, trendColor.B + 30));
                    using var trendBrush = new SolidBrush(trendColor);
                    var fmt = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(trend, trendFont, trendBrush, _trendRect, fmt);
                }
            }
            else
            {
                DrawSampleRevenueData(g, ctx, currencySymbol, showCurrency);
            }
        }

        private void DrawSampleRevenueData(Graphics g, WidgetContext ctx, string currencySymbol, bool showCurrency)
        {
            using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(Color.FromArgb(34, 139, 34));
            string amountText = showCurrency ? $"{currencySymbol}45,230" : "45230";
            g.DrawString(amountText, amountFont, amountBrush, _amountRect);

            using var countFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var countBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            string countText = "28 transactions";
            g.DrawString(countText, countFont, countBrush, _countRect);

            using var periodFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
            using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string periodText = "This month";
            var periodRect = new Rectangle(_countRect.Left, _countRect.Bottom + 2, _countRect.Width, 12);
            g.DrawString(periodText, periodFont, periodBrush, periodRect);
        }

        private void DrawRevenueIcon(Graphics g, WidgetContext ctx)
        {
            using var iconBrush = new SolidBrush(Color.FromArgb(34, 139, 34));
            g.FillEllipse(iconBrush, ctx.IconRect);
            using var symbolFont = new Font("Arial", 12f, FontStyle.Bold);
            using var symbolBrush = new SolidBrush(Color.White);
            g.DrawString("↗", symbolFont, symbolBrush, ctx.IconRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private Color GetRevenueColor(WidgetContext ctx, double amount)
        {
            if (amount > 50000)
                return Color.FromArgb(34, 139, 34);
            else if (amount > 10000)
                return Color.FromArgb(50, 205, 50);
            else
                return ctx.AccentColor;
        }

        private void HandleAmountClick(WidgetContext ctx)
        {
            ctx.ShowRevenueDetails = true;
            Owner?.Invalidate();
        }
        private void HandleCountClick(WidgetContext ctx)
        {
            ctx.ShowTransactionsList = true;
            Owner?.Invalidate();
        }
        private void HandleTrendClick(WidgetContext ctx)
        {
            ctx.ShowTrendChart = true;
            Owner?.Invalidate();
        }
        private void HandleIconClick(WidgetContext ctx)
        {
            ctx.ShowIconInfo = true;
            Owner?.Invalidate();
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}