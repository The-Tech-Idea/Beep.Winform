using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// PaymentCard - Payment method display painter with enhanced visual presentation
    /// </summary>
    internal sealed class PaymentCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public PaymentCardPainter()
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
            
            // Payment card content area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            // Payment method data
            string paymentMethod = ctx.CustomData.ContainsKey("PaymentMethod") ? ctx.CustomData["PaymentMethod"].ToString() : "Credit Card";
            string cardNumber = ctx.CustomData.ContainsKey("CardNumber") ? ctx.CustomData["CardNumber"].ToString() : "**** **** **** 1234";
            string expiryDate = ctx.CustomData.ContainsKey("ExpiryDate") ? ctx.CustomData["ExpiryDate"].ToString() : "12/26";
            decimal balance = ctx.CustomData.ContainsKey("Balance") ? (decimal)ctx.CustomData["Balance"] : 2500.00m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            DrawPaymentHeader(g, ctx, paymentMethod);
            DrawPaymentDetails(g, ctx, cardNumber, expiryDate, balance, currencySymbol);
        }

        private void DrawPaymentHeader(Graphics g, WidgetContext ctx, string paymentMethod)
        {
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                // Payment method icon
                var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
                string iconName = paymentMethod.ToLower() switch
                {
                    "credit card" => "credit-card",
                    "debit card" => "credit-card",
                    "paypal" => "dollar-sign",
                    "bank transfer" => "arrow-right-left",
                    _ => "credit-card"
                };
                _imagePainter.DrawSvg(g, iconName, iconRect, 
                    Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

                // Payment method title
                var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                    ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title ?? paymentMethod, titleFont, titleBrush, titleRect, format);
            }
        }

        private void DrawPaymentDetails(Graphics g, WidgetContext ctx, string cardNumber, string expiryDate, decimal balance, string currencySymbol)
        {
            // Card number with security
            var cardRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 20);
            var cardIconRect = new Rectangle(cardRect.X, cardRect.Y + 2, 16, 16);
            _imagePainter.DrawSvg(g, "eye-off", cardIconRect, Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray), 0.7f);

            var cardTextRect = new Rectangle(cardIconRect.Right + 6, cardRect.Y, 
                cardRect.Width - cardIconRect.Width - 6, cardRect.Height);
            using var cardFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var cardBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var cardFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(cardNumber, cardFont, cardBrush, cardTextRect, cardFormat);

            // Balance
            var balanceRect = new Rectangle(ctx.ContentRect.X, cardRect.Bottom + 8, ctx.ContentRect.Width, 24);
            using var balanceFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
            using var balanceBrush = new SolidBrush(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243));
            string balanceText = $"{currencySymbol}{balance:N2}";
            var balanceFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(balanceText, balanceFont, balanceBrush, balanceRect, balanceFormat);

            // Expiry date
            if (!string.IsNullOrEmpty(expiryDate))
            {
                var expiryRect = new Rectangle(ctx.ContentRect.X, balanceRect.Bottom + 4, ctx.ContentRect.Width, 16);
                using var expiryFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var expiryBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
                var expiryFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"Expires: {expiryDate}", expiryFont, expiryBrush, expiryRect, expiryFormat);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw payment card indicators
        }
    }
}