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
    /// BalanceCard - Account balance showcase with enhanced visual presentation and interactive elements
    /// Uses BaseControl DrawingRect and BeepAppBar-Style hit area methodology
    /// </summary>
    internal sealed class BalanceCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        
        // Interactive area rectangles - defined similar to BeepAppBar's rectangles
        private Rectangle logoRect;
        private Rectangle balanceRect;
        private Rectangle accountRect;
        private Rectangle securityRect;

        public BalanceCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            // Use BaseControl's DrawingRect as the foundation
            ctx.DrawingRect = Owner?.DrawingRect ?? drawingRect;
            
            // Calculate interactive areas within BaseControl's DrawingRect
            CalculateLayout(out logoRect, out balanceRect, out accountRect, out securityRect, ctx);
            
            // Back-compat helper rects
            int pad = 20;
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                45
            );
            
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                40
            );

            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("BalanceCard_Logo", logoRect, null, () => { HandleLogoClick(ctx); notifyAreaHit?.Invoke("BalanceCard_Logo", logoRect); });
            owner.AddHitArea("BalanceCard_Balance", balanceRect, null, () => { HandleBalanceClick(ctx); notifyAreaHit?.Invoke("BalanceCard_Balance", balanceRect); });
            owner.AddHitArea("BalanceCard_Account", accountRect, null, () => { HandleAccountClick(ctx); notifyAreaHit?.Invoke("BalanceCard_Account", accountRect); });
            owner.AddHitArea("BalanceCard_Security", securityRect, null, () => { HandleSecurityClick(ctx); notifyAreaHit?.Invoke("BalanceCard_Security", securityRect); });
        }

        /// <summary>
        /// Calculate layout positions for interactive elements - BeepAppBar CalculateLayout pattern
        /// </summary>
        private void CalculateLayout(out Rectangle logoRect, out Rectangle balanceRect, 
            out Rectangle accountRect, out Rectangle securityRect, WidgetContext ctx)
        {
            // Framework handles DPI scaling
            int padding = 20;
            int spacing = 8;

            int leftEdge = ctx.DrawingRect.Left + padding;
            int rightEdge = ctx.DrawingRect.Right - padding;
            int topEdge = ctx.DrawingRect.Top + padding;

            securityRect = new Rectangle(rightEdge - 24, topEdge, 24, 16);
            logoRect = new Rectangle(leftEdge, topEdge, 24, 20);
            balanceRect = new Rectangle(leftEdge, topEdge + 36, ctx.DrawingRect.Width - padding * 2, 45);
            accountRect = new Rectangle(leftEdge, balanceRect.Bottom + spacing, (ctx.DrawingRect.Width - padding * 2) / 2, 20);
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var drawRect = ctx.DrawingRect;
            using var brush = new LinearGradientBrush(drawRect, Color.FromArgb(70, 130, 180), Color.FromArgb(100, 149, 237), LinearGradientMode.ForwardDiagonal);
            using var bgPath = CreateRoundedPath(drawRect, ctx.CornerRadius);
            g.FillPath(brush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme?.ThemeName;
            _imagePainter.ApplyThemeOnImage = true;

            decimal balance = ctx.PrimaryValue ?? 12345.67m;
            string currencySymbol = ctx.CurrencySymbol ?? "$";
            string accountNumber = ctx.AccountNumber ?? "****1234";

            DrawLogoComponent(g, ctx, IsAreaHovered("BalanceCard_Logo"));
            DrawBalanceComponent(g, ctx, balance, currencySymbol, IsAreaHovered("BalanceCard_Balance"));
            DrawAccountComponent(g, ctx, accountNumber, IsAreaHovered("BalanceCard_Account"));
            DrawSecurityComponent(g, ctx, IsAreaHovered("BalanceCard_Security"));
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Subtle hover accents
            if (IsAreaHovered("BalanceCard_Logo"))
            {
                using var pen = new Pen(Color.FromArgb(120, Color.White), 1);
                g.DrawRectangle(pen, logoRect);
            }
            if (IsAreaHovered("BalanceCard_Balance"))
            {
                using var glow = new SolidBrush(Color.FromArgb(20, Color.White));
                using var p = CreateRoundedPath(Rectangle.Inflate(balanceRect, 4, 4), 6);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("BalanceCard_Account"))
            {
                using var pen = new Pen(Color.FromArgb(120, Color.White), 1);
                g.DrawRectangle(pen, accountRect);
            }
            if (IsAreaHovered("BalanceCard_Security"))
            {
                using var pen = new Pen(Color.FromArgb(150, Color.White), 1);
                g.DrawRectangle(pen, securityRect);
            }
        }

        private void DrawLogoComponent(Graphics g, WidgetContext ctx, bool isHovered)
        {
            Color iconColor = isHovered ? Color.FromArgb(255, Color.White) : Color.FromArgb(220, Color.White);
            var iconRect = logoRect; if (isHovered) iconRect.Inflate(1, 1);
            _imagePainter.DrawSvg(g, "credit-card", iconRect, iconColor, 0.9f);

            var titleRect = new Rectangle(logoRect.Right + 8, logoRect.Y, 150, logoRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, isHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(220, Color.White));
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            string accountType = ctx.AccountType ?? "Checking Account";
            string displayText = isHovered ? $"{accountType} - Click for details" : accountType;
            g.DrawString(displayText, titleFont, titleBrush, titleRect, format);
        }

        private void DrawBalanceComponent(Graphics g, WidgetContext ctx, decimal balance, string currencySymbol, bool isHovered)
        {
            string balanceText = $"{currencySymbol}{balance:N2}";
            using var balanceFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 24f, isHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            Color balanceColor = isHovered ? Color.FromArgb(255, 255, 255, 200) : Color.White;
            using var balanceBrush = new SolidBrush(balanceColor);
            var balanceFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            if (isHovered) balanceText += " - Click for history";
            g.DrawString(balanceText, balanceFont, balanceBrush, balanceRect, balanceFormat);
        }

        private void DrawAccountComponent(Graphics g, WidgetContext ctx, string accountNumber, bool isHovered)
        {
            var accountIconRect = new Rectangle(accountRect.X, accountRect.Y, 12, 12);
            Color iconColor = isHovered ? Color.FromArgb(220, Color.White) : Color.FromArgb(180, Color.White);
            _imagePainter.DrawSvg(g, "hash", accountIconRect, iconColor, 0.8f);

            var accountTextRect = new Rectangle(accountIconRect.Right + 4, accountRect.Y, accountRect.Width - accountIconRect.Width - 4, accountRect.Height);
            using var accountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, isHovered ? FontStyle.Regular | FontStyle.Underline : FontStyle.Regular);
            Color textColor = isHovered ? Color.FromArgb(220, Color.White) : Color.FromArgb(180, Color.White);
            using var accountBrush = new SolidBrush(textColor);
            var accountFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            string displayText = isHovered ? $"{accountNumber} - Click to copy" : accountNumber;
            g.DrawString(displayText, accountFont, accountBrush, accountTextRect, accountFormat);
        }

        private void DrawSecurityComponent(Graphics g, WidgetContext ctx, bool isHovered)
        {
            Color chipColor = isHovered ? Color.FromArgb(60, Color.White) : Color.FromArgb(40, Color.White);
            using var chipBrush = new SolidBrush(chipColor);
            var chipRect = securityRect; if (isHovered) chipRect.Inflate(2, 1);
            g.FillRoundedRectangle(chipBrush, chipRect, 3);
            var chipIconRect = new Rectangle(chipRect.X + 4, chipRect.Y + 2, 16, 12);
            Color chipIconColor = isHovered ? Color.FromArgb(200, Color.White) : Color.FromArgb(150, Color.White);
            _imagePainter.DrawSvg(g, "cpu", chipIconRect, chipIconColor, 0.7f);

            if (isHovered)
            {
                var tooltipRect = new Rectangle(chipRect.X - 60, chipRect.Bottom + 4, 80, 16);
                using var tooltipBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                using var tooltipPath = CreateRoundedPath(tooltipRect, 4);
                g.FillPath(tooltipBrush, tooltipPath);
                using var tooltipFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                using var tooltipTextBrush = new SolidBrush(Color.White);
                var tooltipFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Security Options", tooltipFont, tooltipTextBrush, tooltipRect, tooltipFormat);
            }
        }

        // Action handlers - invoked from BaseControl hit areas
        private void HandleLogoClick(WidgetContext ctx) { Owner?.Invalidate(); }
        private void HandleBalanceClick(WidgetContext ctx) { ctx.ShowBalanceHistory = !ctx.ShowBalanceHistory; Owner?.Invalidate(); }
        private void HandleAccountClick(WidgetContext ctx) { string accountNumber = ctx.AccountNumber ?? "****1234"; try { System.Windows.Forms.Clipboard.SetText(accountNumber); ctx.ShowCopiedMessage = true; } catch { } Owner?.Invalidate(); }
        private void HandleSecurityClick(WidgetContext ctx) { Owner?.Invalidate(); }

        public void Dispose() { _imagePainter?.Dispose(); }
    }
}