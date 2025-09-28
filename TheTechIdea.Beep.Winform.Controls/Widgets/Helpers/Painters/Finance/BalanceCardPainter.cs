using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// BalanceCard - Account balance showcase with enhanced visual presentation
    /// </summary>
    internal sealed class BalanceCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public BalanceCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 20;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Account type
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            // Balance amount (large)
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                45
            );
            
            // Account number and details
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Gradient background like a bank card
            using var brush = new LinearGradientBrush(ctx.DrawingRect, 
                Color.FromArgb(70, 130, 180), 
                Color.FromArgb(100, 149, 237), 
                LinearGradientMode.ForwardDiagonal);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(brush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            decimal balance = ctx.CustomData.ContainsKey("PrimaryValue") ? (decimal)ctx.CustomData["PrimaryValue"] : 12345.67m;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";
            string accountNumber = ctx.CustomData.ContainsKey("AccountNumber") ? ctx.CustomData["AccountNumber"].ToString() : "****1234";

            DrawBalanceHeader(g, ctx);
            DrawBalanceAmount(g, ctx, balance, currencySymbol);
            DrawAccountInfo(g, ctx, accountNumber);
        }

        private void DrawBalanceHeader(Graphics g, WidgetContext ctx)
        {
            // Bank/card icon
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, 20, 20);
            _imagePainter.DrawSvg(g, "credit-card", iconRect, Color.White, 0.9f);

            // Account type text
            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.SemiBold);
            using var titleBrush = new SolidBrush(Color.FromArgb(220, Color.White));
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            
            string accountType = ctx.CustomData.ContainsKey("AccountType") ? 
                ctx.CustomData["AccountType"].ToString() : "Checking Account";
            g.DrawString(accountType, titleFont, titleBrush, titleRect, format);
        }

        private void DrawBalanceAmount(Graphics g, WidgetContext ctx, decimal balance, string currencySymbol)
        {
            // Main balance display
            string balanceText = $"{currencySymbol}{balance:N2}";
            using var balanceFont = new Font(Owner.Font.FontFamily, 24f, FontStyle.Bold);
            using var balanceBrush = new SolidBrush(Color.White);
            var balanceFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(balanceText, balanceFont, balanceBrush, ctx.ContentRect, balanceFormat);
        }

        private void DrawAccountInfo(Graphics g, WidgetContext ctx, string accountNumber)
        {
            // Account number with icon
            var accountRect = ctx.CustomData.ContainsKey("FooterRect") ? 
                (Rectangle)ctx.CustomData["FooterRect"] : 
                new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Bottom + 8, ctx.ContentRect.Width, 16);

            var accountIconRect = new Rectangle(accountRect.X, accountRect.Y, 12, 12);
            _imagePainter.DrawSvg(g, "hash", accountIconRect, Color.FromArgb(180, Color.White), 0.8f);

            var accountTextRect = new Rectangle(accountIconRect.Right + 4, accountRect.Y, 
                accountRect.Width - accountIconRect.Width - 4, accountRect.Height);
            using var accountFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var accountBrush = new SolidBrush(Color.FromArgb(180, Color.White));
            var accountFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(accountNumber, accountFont, accountBrush, accountTextRect, accountFormat);

            // Draw account type
            using var typeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var typeBrush = new SolidBrush(Color.FromArgb(200, Color.White));
            g.DrawString("Checking Account", typeFont, typeBrush, ctx.HeaderRect);

            // Draw balance
            using var balanceFont = new Font(Owner.Font.FontFamily, 22f, FontStyle.Bold);
            using var balanceBrush = new SolidBrush(Color.White);
            string balanceText = $"{currencySymbol}{balance:N2}";
            g.DrawString(balanceText, balanceFont, balanceBrush, ctx.ContentRect);

            // Draw account number
            using var accountFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var accountBrush = new SolidBrush(Color.FromArgb(180, Color.White));
            g.DrawString($"Account: {accountNumber}", accountFont, accountBrush, ctx.FooterRect);

            // Draw last updated
            using var updateFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var updateBrush = new SolidBrush(Color.FromArgb(160, Color.White));
            string updateText = $"Updated: {DateTime.Now:HH:mm}";
            var updateSize = g.MeasureString(updateText, updateFont);
            g.DrawString(updateText, updateFont, updateBrush, ctx.FooterRect.Right - updateSize.Width, ctx.FooterRect.Bottom - updateSize.Height);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Security chip indicator
            var chipRect = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Y + 12, 24, 16);
            using var chipBrush = new SolidBrush(Color.FromArgb(40, Color.White));
            g.FillRoundedRectangle(chipBrush, chipRect, 3);
            
            var chipIconRect = new Rectangle(chipRect.X + 4, chipRect.Y + 2, 16, 12);
            _imagePainter.DrawSvg(g, "cpu", chipIconRect, Color.FromArgb(150, Color.White), 0.7f);
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}