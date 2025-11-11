using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// TransactionCard - Transaction entry display using BaseControl's hit area system
    /// </summary>
    internal sealed class TransactionCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Store rectangles for layout
        private Rectangle _categoryAreaRect;
        private Rectangle _amountAreaRect;
        private Rectangle _detailsAreaRect;
        private Rectangle _statusAreaRect;

        public TransactionCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 10;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = baseRect;
            
            // Transaction header (merchant/category)
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            
            // Transaction content (amount, date, details)
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 2 - 4
            );

            // Compute rectangles for hit areas (registration happens in UpdateHitAreas)
            _categoryAreaRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, ctx.HeaderRect.Width / 2, ctx.HeaderRect.Height);
            _amountAreaRect = new Rectangle(ctx.ContentRect.X + ctx.ContentRect.Width - 80, ctx.ContentRect.Y, 80, 20);
            _detailsAreaRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + 22, ctx.ContentRect.Width - 85, 16);
            _statusAreaRect = new Rectangle(ctx.DrawingRect.Right - 24, ctx.DrawingRect.Y + 4, 20, 20);
            
            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Transaction_Category", _categoryAreaRect, null, () => { HandleCategoryClick(ctx); notifyAreaHit?.Invoke("Transaction_Category", _categoryAreaRect); });
            owner.AddHitArea("Transaction_Amount", _amountAreaRect, null, () => { HandleAmountClick(ctx); notifyAreaHit?.Invoke("Transaction_Amount", _amountAreaRect); });
            owner.AddHitArea("Transaction_Details", _detailsAreaRect, null, () => { HandleDetailsClick(ctx); notifyAreaHit?.Invoke("Transaction_Details", _detailsAreaRect); });
            owner.AddHitArea("Transaction_Status", _statusAreaRect, null, () => { HandleStatusClick(ctx); notifyAreaHit?.Invoke("Transaction_Status", _statusAreaRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 4);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray));
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            string merchant = ctx.Merchant ?? "Amazon";
            string category = ctx.Category ?? "Shopping";
            decimal amount = ctx.Amount ?? -45.99m;
            string currency = ctx.Currency ?? "$";
            DateTime date = ctx.Date ?? DateTime.Now.AddDays(-1);
            string status = ctx.Status ?? "Posted";

            DrawTransactionHeader(g, ctx, merchant, category);
            DrawTransactionDetails(g, ctx, amount, currency, date, status);
        }

        private void DrawTransactionHeader(Graphics g, WidgetContext ctx, string merchant, string category)
        {
            bool isCategoryHovered = IsAreaHovered("Transaction_Category");
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 16, 16);
            string iconName = category.ToLower() switch
            {
                "shopping" => "shopping-bag",
                "food" => "utensils",
                "transport" => "car",
                "entertainment" => "film",
                "utilities" => "zap",
                _ => "dollar-sign"
            };
            Color iconColor = isCategoryHovered ? Theme?.PrimaryColor ?? Color.Blue : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
            _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, 0.8f);

            var merchantRect = new Rectangle(iconRect.Right + 6, ctx.HeaderRect.Y, _categoryAreaRect.Width - iconRect.Width - 6, ctx.HeaderRect.Height);
            using var merchantFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, isCategoryHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            Color merchantColor = isCategoryHovered ? Theme?.PrimaryColor ?? Color.Blue : Theme?.ForeColor ?? Color.Black;
            using var merchantBrush = new SolidBrush(merchantColor);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            string merchantText = isCategoryHovered ? $"{merchant} - Filter by {category}" : merchant;
            g.DrawString(merchantText, merchantFont, merchantBrush, merchantRect, format);
        }

        private void DrawTransactionDetails(Graphics g, WidgetContext ctx, decimal amount, string currency, DateTime date, string status)
        {
            bool isAmountHovered = IsAreaHovered("Transaction_Amount");
            bool isCredit = amount > 0;
            Color amountColor = isCredit ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            if (isAmountHovered) amountColor = Color.FromArgb(Math.Min(255, amountColor.R + 40), Math.Min(255, amountColor.G + 40), Math.Min(255, amountColor.B + 40));
            using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 11f, isAmountHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            using var amountBrush = new SolidBrush(amountColor);
            string amountText = $"{(isCredit ? "+" : "")}{currency}{Math.Abs(amount):N2}";
            if (isAmountHovered) amountText += " - View details";
            var amountFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            g.DrawString(amountText, amountFont, amountBrush, _amountAreaRect, amountFormat);

            bool isDetailsHovered = IsAreaHovered("Transaction_Details");
            using var detailsFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, isDetailsHovered ? FontStyle.Regular | FontStyle.Underline : FontStyle.Regular);
            Color detailsColor = isDetailsHovered ? Theme?.PrimaryColor ?? Color.Blue : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
            using var detailsBrush = new SolidBrush(detailsColor);
            string detailsText = isDetailsHovered ? $"{date:MMM dd} � {status} - Click for full details" : $"{date:MMM dd} � {status}";
            var detailsFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(detailsText, detailsFont, detailsBrush, _detailsAreaRect, detailsFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            bool isStatusHovered = IsAreaHovered("Transaction_Status");
            string status = ctx.Status ?? "Posted";
            Color statusColor = status.ToLower() switch { "posted" => Color.FromArgb(76, 175, 80), "pending" => Color.FromArgb(255, 193, 7), "failed" => Color.FromArgb(244, 67, 54), _ => Color.FromArgb(120, 120, 120) };
            if (isStatusHovered) statusColor = Color.FromArgb(Math.Min(255, statusColor.R + 40), Math.Min(255, statusColor.G + 40), Math.Min(255, statusColor.B + 40));
            var statusRect = _statusAreaRect; if (isStatusHovered) statusRect.Inflate(2, 2);
            using var statusBrush = new SolidBrush(statusColor);
            g.FillEllipse(statusBrush, new Rectangle(statusRect.X + 6, statusRect.Y + 6, 8, 8));
            if (isStatusHovered)
            {
                var tooltipRect = new Rectangle(statusRect.X - 60, statusRect.Bottom + 2, 80, 16);
                using var tooltipBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                using var tooltipPath = CreateRoundedPath(tooltipRect, 4);
                g.FillPath(tooltipBrush, tooltipPath);
                using var tooltipFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                using var tooltipTextBrush = new SolidBrush(Color.White);
                var tooltipFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"Status: {status}", tooltipFont, tooltipTextBrush, tooltipRect, tooltipFormat);
            }
        }

        private void HandleCategoryClick(WidgetContext ctx)
        {
            ctx.FilterByCategory = ctx.Category ?? "Shopping";
            Owner?.Invalidate();
        }

        private void HandleAmountClick(WidgetContext ctx)
        {
            ctx.ShowTransactionDetails = true;
            Owner?.Invalidate();
        }

        private void HandleDetailsClick(WidgetContext ctx)
        {
            ctx.ShowFullDetails = true;
            Owner?.Invalidate();
        }

        private void HandleStatusClick(WidgetContext ctx)
        {
            ctx.ShowStatusInfo = true;
            Owner?.Invalidate();
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}