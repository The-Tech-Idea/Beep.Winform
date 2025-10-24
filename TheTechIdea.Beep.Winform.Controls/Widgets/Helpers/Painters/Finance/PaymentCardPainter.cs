using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// PaymentCard - Payment method display painter with enhanced visual presentation and interactive elements
    /// Uses BaseControl DrawingRect and BeepAppBar-Style hit area methodology
    /// </summary>
    internal sealed class PaymentCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive area rectangles - defined similar to BeepAppBar's rectangles
        private Rectangle cardDetailsRect;
        private Rectangle balanceRect;
        private Rectangle expiryRect;
        private Rectangle cardIconRect;

        public PaymentCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Owner?.DrawingRect ?? drawingRect;
            CalculateLayout(out cardDetailsRect, out balanceRect, out expiryRect, out cardIconRect, ctx);
            // Framework handles DPI scaling
            int pad = 12;
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(
                    ctx.DrawingRect.Left + pad, 
                    ctx.DrawingRect.Top + pad, 
                    ctx.DrawingRect.Width - pad * 2, 
                    24
                );
            }
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );
            return ctx;
        }

        /// <summary>
        /// Calculate layout positions for interactive elements - BeepAppBar CalculateLayout pattern
        /// </summary>
        private void CalculateLayout(out Rectangle cardDetailsRect, out Rectangle balanceRect, 
            out Rectangle expiryRect, out Rectangle cardIconRect, WidgetContext ctx)
        {
            // Framework handles DPI scaling
            int padding = 12;
            int spacing = 8;

            // CRITICAL: Calculate available areas within BaseControl's DrawingRect
            int leftEdge = ctx.DrawingRect.Left + padding;
            int topEdge = ctx.DrawingRect.Top + padding;
            int contentWidth = ctx.DrawingRect.Width - padding * 2;

            // Adjust for title if present
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                topEdge += 32; // 24 for title + 8 spacing
            }

            // Card icon (left side of card details)
            cardIconRect = new Rectangle(
                leftEdge,
                topEdge,
                16, 16
            );

            // Card details area (card number with eye icon)
            cardDetailsRect = new Rectangle(
                leftEdge,
                topEdge,
                contentWidth,
                20
            );

            // Balance area (main content - center)
            balanceRect = new Rectangle(
                leftEdge,
                cardDetailsRect.Bottom + spacing,
                contentWidth,
                24
            );

            // Expiry area (bottom)
            expiryRect = new Rectangle(
                leftEdge,
                balanceRect.Bottom + 4,
                contentWidth,
                16
            );
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            owner.AddHitArea("PaymentCard_Details", cardDetailsRect, null, () => { HandleCardDetailsClick(ctx); notifyAreaHit?.Invoke("PaymentCard_Details", cardDetailsRect); });
            owner.AddHitArea("PaymentCard_Balance", balanceRect, null, () => { HandleBalanceClick(ctx); notifyAreaHit?.Invoke("PaymentCard_Balance", balanceRect); });
            owner.AddHitArea("PaymentCard_Expiry", expiryRect, null, () => { HandleExpiryClick(ctx); notifyAreaHit?.Invoke("PaymentCard_Expiry", expiryRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // CRITICAL: Ensure we're drawing within BaseControl's DrawingRect
            var drawRect = ctx.DrawingRect; // This is now properly set to Owner.DrawingRect
            
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(drawRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            // Payment method data
            string paymentMethod = ctx.CustomData.ContainsKey("PaymentMethod") ? ctx.CustomData["PaymentMethod"].ToString() : "Credit Card";
            string cardNumber = ctx.CustomData.ContainsKey("CardNumber") ? ctx.CustomData["CardNumber"].ToString() : "**** **** **** 1234";
            string expiryDate = ctx.CustomData.ContainsKey("ExpiryDate") ? ctx.CustomData["ExpiryDate"].ToString() : "12/26";
            decimal balance = ctx.CustomData.ContainsKey("Balance") ? (decimal)ctx.CustomData["Balance"] : 2500.00m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            // Draw components with hover states - BeepAppBar pattern
            DrawPaymentHeader(g, ctx, paymentMethod);
            DrawCardDetailsComponent(g, ctx, cardNumber, IsAreaHovered("PaymentCard_Details"));
            DrawBalanceComponent(g, ctx, balance, currencySymbol, IsAreaHovered("PaymentCard_Balance"));
            DrawExpiryComponent(g, ctx, expiryDate, IsAreaHovered("PaymentCard_Expiry"));
        }

        /// <summary>
        /// Check if a hit area is currently hovered - integrates with BaseControl's hit testing
        /// </summary>
        private bool IsAreaHovered(string areaName)
        {
            return Owner?.HitAreaEventOn == true && Owner?.HitTestControl?.Name == areaName && Owner?.HitTestControl?.IsHovered == true;
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
                _imagePainter.DrawSvg(g, iconName, iconRect, Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

                // Payment method title
                var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title ?? paymentMethod, titleFont, titleBrush, titleRect, format);
            }
        }

        private void DrawCardDetailsComponent(Graphics g, WidgetContext ctx, string cardNumber, bool isHovered)
        {
            // Eye icon changes based on hover state - BeepAppBar pattern
            var eyeIconRect = new Rectangle(cardDetailsRect.X, cardDetailsRect.Y + 2, 16, 16);
            
            // Show different icon based on hover/click state
            bool showFullNumber = ctx.CustomData.ContainsKey("ShowFullNumber") && (bool)ctx.CustomData["ShowFullNumber"];
            string eyeIcon = (showFullNumber || isHovered) ? "eye" : "eye-off";
            
            Color eyeColor = isHovered 
                ? Color.FromArgb(180, Theme?.PrimaryColor ?? Color.Blue)
                : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
            
            _imagePainter.DrawSvg(g, eyeIcon, eyeIconRect, eyeColor, 0.7f);

            // Card number text with interactive feedback - BeepAppBar pattern
            var cardTextRect = new Rectangle(eyeIconRect.Right + 6, cardDetailsRect.Y, 
                cardDetailsRect.Width - eyeIconRect.Width - 6, cardDetailsRect.Height);
            
            // Show full or masked number based on hover/click state
            string displayNumber = (showFullNumber || isHovered) ? "4532 1234 5678 9012" : cardNumber;
            if (isHovered && !showFullNumber)
            {
                displayNumber += " - Click to reveal";
            }
            
            using var cardFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, 
                isHovered ? FontStyle.Regular | FontStyle.Underline : FontStyle.Regular);
            
            Color textColor = isHovered 
                ? Theme?.PrimaryColor ?? Color.Blue
                : Theme?.ForeColor ?? Color.Black;
            
            using var cardBrush = new SolidBrush(textColor);
            var cardFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(displayNumber, cardFont, cardBrush, cardTextRect, cardFormat);
        }

        private void DrawBalanceComponent(Graphics g, WidgetContext ctx, decimal balance, string currencySymbol, bool isHovered)
        {
            // Enhanced balance area with hover effects - BeepAppBar pattern
            using var balanceFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 16f, 
                isHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            
            Color balanceColor = isHovered
                ? Color.FromArgb(Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).R + 30)), Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).G + 30)), Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).B + 30)))
                : Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            
            using var balanceBrush = new SolidBrush(balanceColor);
            string balanceText = $"{currencySymbol}{balance:N2}";
            if (isHovered)
            {
                balanceText += " - Click to refresh";
            }
            
            var balanceFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(balanceText, balanceFont, balanceBrush, balanceRect, balanceFormat);
        }

        private void DrawExpiryComponent(Graphics g, WidgetContext ctx, string expiryDate, bool isHovered)
        {
            if (string.IsNullOrEmpty(expiryDate)) return;
            using var expiryFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, 
                isHovered ? FontStyle.Regular | FontStyle.Underline : FontStyle.Regular);
            
            Color expiryColor = isHovered ? Theme?.PrimaryColor ?? Color.Blue : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
            
            using var expiryBrush = new SolidBrush(expiryColor);
            
            string expiryText = isHovered 
                ? $"Expires: {expiryDate} - Click to renew"
                : $"Expires: {expiryDate}";
            
            var expiryFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(expiryText, expiryFont, expiryBrush, expiryRect, expiryFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Add visual indicators for interactive areas when hovered - BeepAppBar pattern
            if (IsAreaHovered("PaymentCard_Details"))
            {
                // Subtle highlight border around card details area
                using var highlightPen = new Pen(Color.FromArgb(100, Theme?.PrimaryColor ?? Color.Blue), 1);
                var highlightRect = Rectangle.Inflate(cardDetailsRect, 2, 2);
                using var highlightPath = CreateRoundedPath(highlightRect, 4);
                g.DrawPath(highlightPen, highlightPath);
            }
            
            if (IsAreaHovered("PaymentCard_Balance"))
            {
                // Subtle glow effect around balance
                using var glowBrush = new SolidBrush(Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue));
                var glowRect = Rectangle.Inflate(balanceRect, 4, 4);
                using var glowPath = CreateRoundedPath(glowRect, 6);
                g.FillPath(glowBrush, glowPath);
            }
            
            if (IsAreaHovered("PaymentCard_Expiry"))
            {
                // Subtle underline for expiry area
                using var underlinePen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 1);
                g.DrawLine(underlinePen, 
                    expiryRect.Left, expiryRect.Bottom - 2,
                    expiryRect.Right, expiryRect.Bottom - 2);
            }
        }

        // Action handlers - actions invoked via BaseControl hit areas
        private void HandleCardDetailsClick(WidgetContext ctx)
        {
            // Toggle show/hide full card number
            bool currentState = ctx.CustomData.ContainsKey("ShowFullNumber") && (bool)ctx.CustomData["ShowFullNumber"];
            ctx.CustomData["ShowFullNumber"] = !currentState;
            
            // Trigger parent invalidation to redraw
            Owner?.Invalidate();
        }

        private void HandleBalanceClick(WidgetContext ctx)
        {
            // Refresh balance or show recent transactions - BeepAppBar pattern
            string balanceView = ctx.CustomData.ContainsKey("BalanceView") ? 
                ctx.CustomData["BalanceView"].ToString() : "Current";
            
            string nextView = balanceView switch
            {
                "Current" => "Available",
                "Available" => "Transactions",
                "Transactions" => "Current",
                _ => "Current"
            };
            
            ctx.CustomData["BalanceView"] = nextView;
            
            // Trigger parent invalidation to redraw
            Owner?.Invalidate();
        }

        private void HandleExpiryClick(WidgetContext ctx)
        {
            // Show card renewal options - BeepAppBar pattern
            ctx.CustomData["ShowRenewalOptions"] = true;
            
            // Trigger parent invalidation to redraw
            Owner?.Invalidate();
        }

        public void Dispose() { _imagePainter?.Dispose(); }
    }
}