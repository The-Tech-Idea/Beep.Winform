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
    /// ExpenseCard - Expense category display painter (BeepAppBar methodology) with hit areas
    /// </summary>
    internal sealed class ExpenseCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive rects
        private Rectangle _amountRect;
        private Rectangle _countRect;
        private Rectangle _iconRect;

        public ExpenseCardPainter()
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

            // Compute interactive rects
            _amountRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top + 10, ctx.ContentRect.Width, 20);
            _countRect = new Rectangle(ctx.ContentRect.Left, _amountRect.Bottom + 4, ctx.ContentRect.Width, 14);
            _iconRect = ctx.IconRect;

            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Expense_Amount", _amountRect, null, () => { HandleAmountClick(ctx); notifyAreaHit?.Invoke("Expense_Amount", _amountRect); });
            owner.AddHitArea("Expense_Count", _countRect, null, () => { HandleCountClick(ctx); notifyAreaHit?.Invoke("Expense_Count", _countRect); });
            if (!_iconRect.IsEmpty)
                owner.AddHitArea("Expense_Icon", _iconRect, null, () => { HandleIconClick(ctx); notifyAreaHit?.Invoke("Expense_Icon", _iconRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(Theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                DrawExpenseHeader(g, ctx);
            }

            DrawExpenseData(g, ctx);

            if (ctx.ShowIcon && !ctx.IconRect.IsEmpty)
            {
                DrawExpenseIcon(g, ctx);
            }
        }

        private void DrawExpenseHeader(Graphics g, WidgetContext ctx)
        {
            // Expense icon (credit card) next to title
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "credit-card", iconRect, Theme?.PrimaryColor ?? Color.FromArgb(244, 67, 54), 0.9f);

            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y,
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush, titleRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (IsAreaHovered("Expense_Amount"))
            {
                using var glow = new SolidBrush(Color.FromArgb(16, 244, 67, 54));
                var r = Rectangle.Inflate(_amountRect, 4, 2);
                using var p = CreateRoundedPath(r, 4);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("Expense_Icon") && !_iconRect.IsEmpty)
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.FromArgb(244, 67, 54)), 1);
                g.DrawEllipse(pen, _iconRect);
            }
        }

        private void DrawExpenseData(Graphics g, WidgetContext ctx)
        {
            var financeItems = ctx.Transactions;

            string currencySymbol = ctx.CurrencySymbol ?? "$";

            bool showCurrency = ctx.ShowCurrency;

            if (financeItems != null && financeItems.Count > 0)
            {
                double totalExpenses = financeItems.Where(item => item.Amount < 0).Sum(item => Math.Abs((double)item.Amount));

                bool amountHovered = IsAreaHovered("Expense_Amount");
                using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 14f, amountHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
                using var amountBrush = new SolidBrush(GetExpenseColor(ctx, totalExpenses));

                string amountText = showCurrency ?
                    $"{currencySymbol}{totalExpenses.ToString("N0", CultureInfo.CurrentCulture)}" :
                    totalExpenses.ToString("N0", CultureInfo.CurrentCulture);
                if (amountHovered) amountText += " - Click to drilldown";

                g.DrawString(amountText, amountFont, amountBrush, _amountRect);

                int expenseCount = financeItems.Count(item => item.Amount < 0);
                bool countHovered = IsAreaHovered("Expense_Count");
                using var countFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, countHovered ? FontStyle.Underline : FontStyle.Regular);
                using var countBrush = new SolidBrush(countHovered ? (Theme?.PrimaryColor ?? Color.Red) : Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));

                string countText = $"{expenseCount} expense{(expenseCount != 1 ? "s" : "")}";
                g.DrawString(countText, countFont, countBrush, _countRect);

                var lastUpdated = ctx.LastUpdated;
                if (lastUpdated != null)
                {
                    using var periodFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                    using var periodBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                    string periodText = $"Updated {lastUpdated}";
                    var periodRect = new Rectangle(_countRect.Left, _countRect.Bottom + 2, _countRect.Width, 12);
                    g.DrawString(periodText, periodFont, periodBrush, periodRect);
                }
            }
            else
            {
                DrawSampleExpenseData(g, ctx, currencySymbol, showCurrency);
            }
        }

        private void DrawSampleExpenseData(Graphics g, WidgetContext ctx, string currencySymbol, bool showCurrency)
        {
            using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(ctx.AccentColor);
            string amountText = showCurrency ? $"{currencySymbol}2,450" : "2450";
            g.DrawString(amountText, amountFont, amountBrush, _amountRect);

            using var countFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var countBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
            string countText = "12 expenses";
            g.DrawString(countText, countFont, countBrush, _countRect);

            using var periodFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
            using var periodBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
            string periodText = "This month";
            var periodRect = new Rectangle(_countRect.Left, _countRect.Bottom + 2, _countRect.Width, 12);
            g.DrawString(periodText, periodFont, periodBrush, periodRect);
        }

        private void DrawExpenseIcon(Graphics g, WidgetContext ctx)
        {
            var iconPath = ctx.IconPath;
            if (!string.IsNullOrEmpty(iconPath))
            {
                _imagePainter.CurrentTheme = Theme;
                _imagePainter.ApplyThemeOnImage = true;
                _imagePainter.ImagePath = iconPath;
                _imagePainter.DrawImage(g, ctx.IconRect);
            }
            else
            {
                using var iconBrush = new SolidBrush(ctx.AccentColor);
                g.FillEllipse(iconBrush, ctx.IconRect);
                using var symbolFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
                using var symbolBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
                g.DrawString("$", symbolFont, symbolBrush, ctx.IconRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private Color GetExpenseColor(WidgetContext ctx, double amount)
        {
            if (amount > 5000)
                return ctx.NegativeColor;
            else if (amount > 1000)
                return ctx.NeutralColor;
            else
                return ctx.AccentColor;
        }

        private void HandleAmountClick(WidgetContext ctx)
        {
            ctx.ShowExpenseDetails = true;
            Owner?.Invalidate();
        }
        private void HandleCountClick(WidgetContext ctx)
        {
            ctx.ShowExpensesList = true;
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