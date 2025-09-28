using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// TransactionCard - Financial transaction display with enhanced visual presentation
    /// </summary>
    internal sealed class TransactionCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TransactionCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Transaction type icon
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                24, 24
            );
            
            // Transaction details
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                24
            );
            
            // Amount
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Date and status
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                20
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
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            // Transaction data
            string transactionType = ctx.CustomData.ContainsKey("TransactionType") ? ctx.CustomData["TransactionType"].ToString() : "payment";
            string description = ctx.CustomData.ContainsKey("Description") ? ctx.CustomData["Description"].ToString() : "Payment to Merchant";
            decimal amount = ctx.CustomData.ContainsKey("PrimaryValue") ? (decimal)ctx.CustomData["PrimaryValue"] : -50.00m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";
            DateTime transactionDate = ctx.CustomData.ContainsKey("TransactionDate") ? (DateTime)ctx.CustomData["TransactionDate"] : DateTime.Now;
            string status = ctx.CustomData.ContainsKey("Status") ? ctx.CustomData["Status"].ToString() : "Completed";

            DrawTransactionIcon(g, ctx, transactionType, amount);
            DrawTransactionDetails(g, ctx, description, amount, currencySymbol, transactionDate, status);
        }

        private void DrawTransactionIcon(Graphics g, WidgetContext ctx, string transactionType, decimal amount)
        {
            // Icon background
            var bgColor = amount < 0 ? Color.FromArgb(244, 67, 54) : Color.FromArgb(76, 175, 80);
            using var iconBrush = new SolidBrush(Color.FromArgb(30, bgColor));
            g.FillRoundedRectangle(iconBrush, ctx.IconRect, 12);

            // Transaction category icon
            string iconName = transactionType.ToLower() switch
            {
                "income" or "salary" => "arrow-down-left",
                "expense" or "payment" => "arrow-up-right", 
                "transfer" => "arrow-right-left",
                "investment" => "trending-up",
                "withdrawal" => "minus",
                "deposit" => "plus",
                _ => "dollar-sign"
            };
            
            var iconRect = Rectangle.Inflate(ctx.IconRect, -4, -4);
            _imagePainter.DrawSvg(g, iconName, iconRect, bgColor, 0.9f);
        }

        private void DrawTransactionDetails(Graphics g, WidgetContext ctx, string description, decimal amount, string currencySymbol, DateTime transactionDate, string status)
        {
            // Transaction description
            using var descFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Medium);
            using var descBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var descFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(description, descFont, descBrush, ctx.HeaderRect, descFormat);

            // Amount with proper formatting
            var amountColor = amount < 0 ? Color.FromArgb(244, 67, 54) : Color.FromArgb(76, 175, 80);
            using var amountFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(amountColor);
            string amountText = $"{(amount < 0 ? "-" : "+")}{currencySymbol}{Math.Abs(amount):N2}";
            var amountFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(amountText, amountFont, amountBrush, ctx.ContentRect, amountFormat);

            // Date and status
            using var dateFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
            string dateText = transactionDate.ToString("MMM dd, yyyy");
            var dateFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(dateText, dateFont, dateBrush, ctx.FooterRect, dateFormat);

            // Status with indicator
            var statusColor = status.ToLower() switch
            {
                "completed" => Color.FromArgb(76, 175, 80),
                "pending" => Color.FromArgb(255, 193, 7), 
                "failed" => Color.FromArgb(244, 67, 54),
                _ => Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray)
            };
            
            using var statusFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);
            using var statusBrush = new SolidBrush(statusColor);
            var statusSize = g.MeasureString(status, statusFont);
            var statusRect = new Rectangle(
                ctx.FooterRect.Right - (int)statusSize.Width, 
                ctx.FooterRect.Y, 
                (int)statusSize.Width, 
                ctx.FooterRect.Height
            );
            var statusFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(status, statusFont, statusBrush, statusRect, statusFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Category badge for transaction type
            if (ctx.CustomData.ContainsKey("Category"))
            {
                string category = ctx.CustomData["Category"].ToString();
                var badgeRect = new Rectangle(ctx.DrawingRect.Right - 60, ctx.DrawingRect.Y + 8, 52, 16);
                
                var categoryColor = category.ToLower() switch
                {
                    "food" => Color.FromArgb(255, 152, 0),
                    "transport" => Color.FromArgb(33, 150, 243),
                    "entertainment" => Color.FromArgb(156, 39, 176),
                    "utilities" => Color.FromArgb(76, 175, 80),
                    "shopping" => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(96, 125, 139)
                };
                
                using var categoryBrush = new SolidBrush(Color.FromArgb(20, categoryColor));
                g.FillRoundedRectangle(categoryBrush, badgeRect, 8);
                
                using var categoryFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Medium);
                using var categoryTextBrush = new SolidBrush(categoryColor);
                var categoryFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(category.ToUpper(), categoryFont, categoryTextBrush, badgeRect, categoryFormat);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}