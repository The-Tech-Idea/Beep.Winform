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
    /// InvestmentCard - Investment tracking card painter with enhanced visual presentation and interactive elements
    /// </summary>
    internal sealed class InvestmentCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive area rectangles - following BeepAppBar pattern
        private Rectangle _titleAreaRect;
        private Rectangle _currentValueAreaRect;
        private Rectangle _gainLossAreaRect;
        private Rectangle _portfolioAreaRect;

        public InvestmentCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            // Use BaseControl's DrawingRect as base
            ctx.DrawingRect = Owner?.DrawingRect ?? drawingRect;
            
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

            // Calculate interactive rectangles (store for UpdateHitAreas and drawing)
            if (!ctx.HeaderRect.IsEmpty)
            {
                _titleAreaRect = ctx.HeaderRect;
            }

            _currentValueAreaRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 30);
            _gainLossAreaRect = new Rectangle(ctx.ContentRect.X, _currentValueAreaRect.Bottom + 8, ctx.ContentRect.Width, 24);

            if (ctx.IsPortfolio)
            {
                _portfolioAreaRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
            }
            else
            {
                _portfolioAreaRect = Rectangle.Empty;
            }
            
            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_titleAreaRect.IsEmpty)
            {
                owner.AddHitArea("Investment_Title", _titleAreaRect, null, () => { HandleTitleClick(ctx); notifyAreaHit?.Invoke("Investment_Title", _titleAreaRect); });
            }

            owner.AddHitArea("Investment_CurrentValue", _currentValueAreaRect, null, () => { HandleValueClick(ctx); notifyAreaHit?.Invoke("Investment_CurrentValue", _currentValueAreaRect); });
            owner.AddHitArea("Investment_GainLoss", _gainLossAreaRect, null, () => { HandleGainLossClick(ctx); notifyAreaHit?.Invoke("Investment_GainLoss", _gainLossAreaRect); });

            if (!_portfolioAreaRect.IsEmpty)
            {
                owner.AddHitArea("Investment_Portfolio", _portfolioAreaRect, null, () => { HandlePortfolioClick(ctx); notifyAreaHit?.Invoke("Investment_Portfolio", _portfolioAreaRect); });
            }
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
            _imagePainter.Theme = Theme?.ThemeName;
            _imagePainter.UseThemeColors = true;

            // Investment data
            decimal currentValue = ctx.CurrentValue ?? 15750.00m;
            decimal initialValue = ctx.InitialValue ?? 10000.00m;
            decimal gainLoss = currentValue - initialValue;
            decimal gainLossPercent = initialValue > 0 ? (gainLoss / initialValue) * 100 : 0;
            string currencySymbol = ctx.CurrencySymbol ?? "$";

            // Enhanced drawing with interactive feedback - similar to BeepAppBar components
            DrawInvestmentHeader(g, ctx, IsAreaHovered("Investment_Title"));
            DrawInvestmentValue(g, ctx, currentValue, gainLoss, gainLossPercent, currencySymbol);
        }

        private void DrawInvestmentHeader(Graphics g, WidgetContext ctx, bool isTitleHovered)
        {
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                // Icon with hover effect
                var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
                
                Color iconColor = isTitleHovered 
                    ? Color.FromArgb(Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).R + 40)),
                                     Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).G + 40)),
                                     Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).B + 40)))
                    : Theme?.PrimaryColor ?? Color.FromArgb(76, 175, 80);

                // Scale icon slightly when hovered
                if (isTitleHovered)
                {
                    iconRect.Inflate(1, 1);
                }
                
                _imagePainter.DrawSvg(g, "trending-up", iconRect, iconColor, 0.9f);

                // Title with hover effect
                var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                    ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
                
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, 
                    isTitleHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
                
                Color titleColor = isTitleHovered 
                    ? Theme?.PrimaryColor ?? Color.Blue
                    : Theme?.ForeColor ?? Color.Black;
                
                using var titleBrush = new SolidBrush(titleColor);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                
                string titleText = isTitleHovered ? $"{ctx.Title} - Click for details" : ctx.Title;
                g.DrawString(titleText, titleFont, titleBrush, titleRect, format);
            }
        }

        private void DrawInvestmentValue(Graphics g, WidgetContext ctx, decimal currentValue, decimal gainLoss, decimal gainLossPercent, string currencySymbol)
        {
            // Current value with hover effects
            bool isValueHovered = IsAreaHovered("Investment_CurrentValue");
            
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 18f, 
                isValueHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            
            Color valueColor = isValueHovered
                ? Theme?.PrimaryColor ?? Color.Blue
                : Theme?.ForeColor ?? Color.Black;
            
            using var valueBrush = new SolidBrush(valueColor);
            
            string valueText = isValueHovered 
                ? $"{currencySymbol}{currentValue:N2} - Click for history"
                : $"{currencySymbol}{currentValue:N2}";
            
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(valueText, valueFont, valueBrush, _currentValueAreaRect, valueFormat);

            // Gain/Loss with enhanced hover effects
            bool isGainLossHovered = IsAreaHovered("Investment_GainLoss");
            bool isGain = gainLoss >= 0;
            
            Color gainLossColor = isGain ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            if (isGainLossHovered)
            {
                // Brighten color on hover
                gainLossColor = Color.FromArgb(Math.Min(255, gainLossColor.R + 30),
                                             Math.Min(255, gainLossColor.G + 30),
                                             Math.Min(255, gainLossColor.B + 30));
            }
            
            // Icon with animation effect when hovered
            var iconRect = new Rectangle(_gainLossAreaRect.X + (_gainLossAreaRect.Width - 120) / 2, 
                                        _gainLossAreaRect.Y + 4, 16, 16);
            if (isGainLossHovered)
            {
                // Slightly larger icon when hovered
                iconRect.Inflate(2, 2);
            }
            
            string trendIcon = isGain ? "trending-up" : "trending-down";
            _imagePainter.DrawSvg(g, trendIcon, iconRect, gainLossColor, 0.9f);
            
            // Gain/Loss text with hover enhancement
            var gainLossTextRect = new Rectangle(iconRect.Right + 4, _gainLossAreaRect.Y, 100, _gainLossAreaRect.Height);
            using var gainLossFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, 
                isGainLossHovered ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
            
            using var gainLossBrush = new SolidBrush(gainLossColor);
            
            string gainLossText = isGainLossHovered
                ? $"{(isGain ? "+" : "")}{currencySymbol}{Math.Abs(gainLoss):N2} ({gainLossPercent:+0.00;-0.00}%) - Click for analytics"
                : $"{(isGain ? "+" : "")}{currencySymbol}{Math.Abs(gainLoss):N2} ({gainLossPercent:+0.00;-0.00}%)";
            
            var gainLossFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(gainLossText, gainLossFont, gainLossBrush, gainLossTextRect, gainLossFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Portfolio indicator with hover effects
            if (!_portfolioAreaRect.IsEmpty)
            {
                bool isPortfolioHovered = IsAreaHovered("Investment_Portfolio");
                
                Color bgColor = isPortfolioHovered
                    ? Color.FromArgb(50, Theme?.AccentColor ?? Color.Blue)
                    : Color.FromArgb(30, Theme?.AccentColor ?? Color.Blue);
                
                using var portfolioBrush = new SolidBrush(bgColor);
                
                var portfolioRect = _portfolioAreaRect;
                if (isPortfolioHovered) portfolioRect.Inflate(2, 2);
                
                using var path = CreateRoundedPath(portfolioRect, 4);
                g.FillPath(portfolioBrush, path);
                
                var iconRect = new Rectangle(portfolioRect.X + 2, portfolioRect.Y + 2, 16, 16);
                Color iconColor = isPortfolioHovered
                    ? Color.FromArgb(220, Theme?.AccentColor ?? Color.Blue)
                    : Color.FromArgb(150, Theme?.AccentColor ?? Color.Blue);
                
                _imagePainter.DrawSvg(g, "pie-chart", iconRect, iconColor, 0.8f);
                
                if (isPortfolioHovered)
                {
                    var tooltipRect = new Rectangle(portfolioRect.X - 80, portfolioRect.Bottom + 2, 100, 16);
                    using var tooltipBrush = new SolidBrush(Color.FromArgb(200, 50, 50, 50));
                    using var tooltipPath = CreateRoundedPath(tooltipRect, 4);
                    g.FillPath(tooltipBrush, tooltipPath);
                    
                    using var tooltipFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                    using var tooltipTextBrush = new SolidBrush(Color.White);
                    var tooltipFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("View Portfolio", tooltipFont, tooltipTextBrush, tooltipRect, tooltipFormat);
                }
            }

            // Other interactive hover visuals
            if (!_titleAreaRect.IsEmpty && IsAreaHovered("Investment_Title"))
            {
                using var highlightPen = new Pen(Color.FromArgb(100, Theme?.PrimaryColor ?? Color.Blue), 1);
                var highlightRect = Rectangle.Inflate(_titleAreaRect, 2, 2);
                using var highlightPath = CreateRoundedPath(highlightRect, 4);
                g.DrawPath(highlightPen, highlightPath);
            }

            if (IsAreaHovered("Investment_CurrentValue"))
            {
                using var glowBrush = new SolidBrush(Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue));
                var glowRect = Rectangle.Inflate(_currentValueAreaRect, 4, 4);
                using var glowPath = CreateRoundedPath(glowRect, 6);
                g.FillPath(glowBrush, glowPath);
            }

            if (IsAreaHovered("Investment_GainLoss"))
            {
                bool isGain = true;
                if (ctx.CurrentValue.HasValue && ctx.InitialValue.HasValue)
                {
                    decimal current = ctx.CurrentValue.Value;
                    decimal initial = ctx.InitialValue.Value;
                    isGain = current >= initial;
                }
                
                Color highlightColor = isGain 
                    ? Color.FromArgb(20, 76, 175, 80)
                    : Color.FromArgb(20, 244, 67, 54);
                
                using var highlightBrush = new SolidBrush(highlightColor);
                var highlightRect = Rectangle.Inflate(_gainLossAreaRect, 4, 4);
                using var highlightPath = CreateRoundedPath(highlightRect, 4);
                g.FillPath(highlightBrush, highlightPath);
            }
        }

        // Action handlers (no painter-level events; invoke via BaseControl hit areas)
        private void HandleTitleClick(WidgetContext ctx)
        {
            ctx.ShowInvestmentDetails = true;
            Owner?.Invalidate();
        }

        private void HandleValueClick(WidgetContext ctx)
        {
            string chartPeriod = ctx.ChartPeriod ?? "1M";            string nextPeriod = chartPeriod switch
            {
                "1D" => "1W",
                "1W" => "1M", 
                "1M" => "3M",
                "3M" => "1Y",
                "1Y" => "ALL",
                "ALL" => "1D",
                _ => "1M"
            };
            
            ctx.ChartPeriod = nextPeriod;
            ctx.ShowValueHistory = true;
            Owner?.Invalidate();
        }

        private void HandleGainLossClick(WidgetContext ctx)
        {
            ctx.ShowPerformanceAnalytics = true;
            Owner?.Invalidate();
        }

        private void HandlePortfolioClick(WidgetContext ctx)
        {
            ctx.ShowPortfolioBreakdown = true;
            Owner?.Invalidate();
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}