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
    /// InvestmentCard - Investment tracking card painter with enhanced visual presentation
    /// </summary>
    internal sealed class InvestmentCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public InvestmentCardPainter()
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
            
            // Investment content area
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

            // Investment data
            decimal currentValue = ctx.CustomData.ContainsKey("CurrentValue") ? (decimal)ctx.CustomData["CurrentValue"] : 15750.00m;
            decimal initialValue = ctx.CustomData.ContainsKey("InitialValue") ? (decimal)ctx.CustomData["InitialValue"] : 10000.00m;
            decimal gainLoss = currentValue - initialValue;
            decimal gainLossPercent = initialValue > 0 ? (gainLoss / initialValue) * 100 : 0;
            string currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            DrawInvestmentHeader(g, ctx);
            DrawInvestmentValue(g, ctx, currentValue, gainLoss, gainLossPercent, currencySymbol);
        }

        private void DrawInvestmentHeader(Graphics g, WidgetContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                // Investment icon
                var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
                _imagePainter.DrawSvg(g, "trending-up", iconRect, 
                    Theme?.PrimaryColor ?? Color.FromArgb(76, 175, 80), 0.9f);

                // Investment title
                var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                    ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, titleRect, format);
            }
        }

        private void DrawInvestmentValue(Graphics g, WidgetContext ctx, decimal currentValue, decimal gainLoss, decimal gainLossPercent, string currencySymbol)
        {
            // Current value
            var valueRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 30);
            using var valueFont = new Font(Owner.Font.FontFamily, 18f, FontStyle.Bold);
            using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            string valueText = $"{currencySymbol}{currentValue:N2}";
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(valueText, valueFont, valueBrush, valueRect, valueFormat);

            // Gain/Loss display with icon
            var gainLossRect = new Rectangle(ctx.ContentRect.X, valueRect.Bottom + 8, ctx.ContentRect.Width, 24);
            bool isGain = gainLoss >= 0;
            Color gainLossColor = isGain ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            
            var iconRect = new Rectangle(gainLossRect.X + (gainLossRect.Width - 120) / 2, gainLossRect.Y + 4, 16, 16);
            string iconName = isGain ? "arrow-up" : "arrow-down";
            _imagePainter.DrawSvg(g, iconName, iconRect, gainLossColor, 0.9f);

            var gainLossTextRect = new Rectangle(iconRect.Right + 4, gainLossRect.Y, 100, gainLossRect.Height);
            using var gainLossFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.SemiBold);
            using var gainLossBrush = new SolidBrush(gainLossColor);
            string gainLossText = $"{(isGain ? "+" : "")}{currencySymbol}{Math.Abs(gainLoss):N2} ({gainLossPercent:+0.00;-0.00}%)";
            var gainLossFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(gainLossText, gainLossFont, gainLossBrush, gainLossTextRect, gainLossFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Portfolio diversification indicator
            if (ctx.CustomData.ContainsKey("IsPortfolio") && (bool)ctx.CustomData["IsPortfolio"])
            {
                var portfolioRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
                using var portfolioBrush = new SolidBrush(Color.FromArgb(30, Theme?.AccentColor ?? Color.Blue));
                g.FillRoundedRectangle(portfolioBrush, portfolioRect, 4);
                
                var iconRect = new Rectangle(portfolioRect.X + 2, portfolioRect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "pie-chart", iconRect, 
                    Color.FromArgb(150, Theme?.AccentColor ?? Color.Blue), 0.7f);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}