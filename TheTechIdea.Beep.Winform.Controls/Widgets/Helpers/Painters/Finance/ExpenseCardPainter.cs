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
    /// ExpenseCard - Expense category display painter
    /// </summary>
    internal sealed class ExpenseCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ExpenseCardPainter()
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

            // Expense content area
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
            // Draw main card background
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
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

            // Draw title with expense icon
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                DrawExpenseHeader(g, ctx);
            }

            // Draw expense data
            DrawExpenseData(g, ctx);

            // Draw icon if enabled
            if (ctx.ShowIcon && !ctx.IconRect.IsEmpty)
            {
                DrawExpenseIcon(g, ctx);
            }
        }

        private void DrawExpenseHeader(Graphics g, WidgetContext ctx)
        {
            // Expense icon
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "credit-card", iconRect, 
                Color.FromArgb(244, 67, 54), 0.9f);

            // Expense title
            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush, titleRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw expense category indicator
            DrawCategoryIndicator(g, ctx);
        }

        private void DrawExpenseData(Graphics g, WidgetContext ctx)
        {
            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ?
                ctx.CustomData["FinanceItems"] as List<FinanceItem> : null;

            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ?
                ctx.CustomData["CurrencySymbol"]?.ToString() ?? "$" : "$";

            bool showCurrency = ctx.CustomData.ContainsKey("ShowCurrency") ?
                Convert.ToBoolean(ctx.CustomData["ShowCurrency"]) : true;

            if (financeItems != null && financeItems.Count > 0)
            {
                // Display total expenses
                double totalExpenses = financeItems.Where(item => item.Value < 0).Sum(item => Math.Abs((double)item.Value));

                using var amountFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
                using var amountBrush = new SolidBrush(GetExpenseColor(ctx, totalExpenses));

                string amountText = showCurrency ?
                    $"{currencySymbol}{totalExpenses.ToString("N0", CultureInfo.CurrentCulture)}" :
                    totalExpenses.ToString("N0", CultureInfo.CurrentCulture);

                var amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10,
                                             ctx.ContentRect.Width, 20);
                g.DrawString(amountText, amountFont, amountBrush, amountRect);

                // Display expense count
                int expenseCount = financeItems.Count(item => item.Value < 0);
                using var countFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var countBrush = new SolidBrush(Color.FromArgb(150, Color.Black));

                string countText = $"{expenseCount} expense{(expenseCount != 1 ? "s" : "")}";
                var countRect = new Rectangle(ctx.ContentRect.Left, amountRect.Bottom + 4,
                                            ctx.ContentRect.Width, 14);
                g.DrawString(countText, countFont, countBrush, countRect);

                // Display period if available
                var lastUpdated = ctx.CustomData.ContainsKey("LastUpdated") ?
                    ctx.CustomData["LastUpdated"] : null;
                if (lastUpdated != null)
                {
                    using var periodFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
                    using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

                    string periodText = $"Updated {lastUpdated}";
                    var periodRect = new Rectangle(ctx.ContentRect.Left, countRect.Bottom + 2,
                                                 ctx.ContentRect.Width, 12);
                    g.DrawString(periodText, periodFont, periodBrush, periodRect);
                }
            }
            else
            {
                // Draw sample expense data
                DrawSampleExpenseData(g, ctx, currencySymbol, showCurrency);
            }
        }

        private void DrawSampleExpenseData(Graphics g, WidgetContext ctx, string currencySymbol, bool showCurrency)
        {
            using var amountFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(ctx.AccentColor);

            string amountText = showCurrency ? $"{currencySymbol}2,450" : "2450";
            var amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10,
                                         ctx.ContentRect.Width, 20);
            g.DrawString(amountText, amountFont, amountBrush, amountRect);

            using var countFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var countBrush = new SolidBrush(Color.FromArgb(150, Color.Black));

            string countText = "12 expenses";
            var countRect = new Rectangle(ctx.ContentRect.Left, amountRect.Bottom + 4,
                                        ctx.ContentRect.Width, 14);
            g.DrawString(countText, countFont, countBrush, countRect);

            using var periodFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var periodBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            string periodText = "This month";
            var periodRect = new Rectangle(ctx.ContentRect.Left, countRect.Bottom + 2,
                                         ctx.ContentRect.Width, 12);
            g.DrawString(periodText, periodFont, periodBrush, periodRect);
        }

        private void DrawExpenseIcon(Graphics g, WidgetContext ctx)
        {
            // Check if custom icon path is provided
            var iconPath = ctx.CustomData.ContainsKey("IconPath") ? ctx.CustomData["IconPath"]?.ToString() : null;
            
            if (!string.IsNullOrEmpty(iconPath))
            {
                // Configure icon painter
                _iconPainter.CurrentTheme = Theme;
                _iconPainter.ImagePath = iconPath;
                _iconPainter.ClipShape = ImageClipShape.Circle;
                _iconPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                _iconPainter.ApplyThemeOnImage = true;
                
                // Draw icon with ImagePainter
                _iconPainter.DrawImage(g, ctx.IconRect);
            }
            else
            {
                // Fallback: Draw a simple expense icon (dollar sign in circle)
                using var iconBrush = new SolidBrush(ctx.AccentColor);
                g.FillEllipse(iconBrush, ctx.IconRect);

                using var symbolFont = new Font("Arial", 10f, FontStyle.Bold);
                using var symbolBrush = new SolidBrush(Color.White);
                g.DrawString("$", symbolFont, symbolBrush, ctx.IconRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawCategoryIndicator(Graphics g, WidgetContext ctx)
        {
            // Draw a small colored indicator based on expense category
            var indicatorRect = new Rectangle(ctx.DrawingRect.Left + 8, ctx.DrawingRect.Top + 8, 4, 16);
            using var indicatorBrush = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(indicatorBrush, indicatorRect);
        }

        private Color GetExpenseColor(WidgetContext ctx, double amount)
        {
            // Color code based on expense amount ranges
            if (amount > 5000)
                return ctx.CustomData.ContainsKey("NegativeColor") ?
                    (Color)ctx.CustomData["NegativeColor"] : Color.Red;
            else if (amount > 1000)
                return ctx.CustomData.ContainsKey("NeutralColor") ?
                    (Color)ctx.CustomData["NeutralColor"] : Color.Orange;
            else
                return ctx.AccentColor;
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}