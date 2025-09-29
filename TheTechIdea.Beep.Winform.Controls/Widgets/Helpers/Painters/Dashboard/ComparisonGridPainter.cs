using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// ComparisonGrid - Side-by-side comparisons with enhanced visual presentation
    /// </summary>
    internal sealed class ComparisonGridPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private Rectangle _leftPanelRect;
        private Rectangle _rightPanelRect;
        private Rectangle _vsIconRect;

        public ComparisonGridPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            // Precompute panel rectangles
            int panelWidth = ctx.ContentRect.Width / 2 - 12;
            int panelHeight = ctx.ContentRect.Height;
            _leftPanelRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, panelWidth, panelHeight);
            _rightPanelRect = new Rectangle(ctx.ContentRect.X + panelWidth + 24, ctx.ContentRect.Y, panelWidth, panelHeight);

            // VS icon area
            var separatorRect = new Rectangle(ctx.ContentRect.X + panelWidth + 4, ctx.ContentRect.Y + ctx.ContentRect.Height / 2 - 20, 16, 40);
            var circleRect = new Rectangle(separatorRect.X, separatorRect.Y + 12, 16, 16);
            _vsIconRect = new Rectangle(circleRect.X + 2, circleRect.Y + 2, 12, 12);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            // Draw enhanced title with comparison icon
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                DrawComparisonTitle(g, ctx);
            }
            
            if (ctx.CustomData.ContainsKey("Metrics"))
            {
                var metrics = (List<Dictionary<string, object>>)ctx.CustomData["Metrics"];
                DrawComparisonPanels(g, _leftPanelRect, _rightPanelRect, metrics);
            }
        }

        private void DrawComparisonTitle(Graphics g, WidgetContext ctx)
        {
            // Title background
            using var titleBrush = new LinearGradientBrush(
                ctx.HeaderRect,
                Color.FromArgb(15, Theme?.PrimaryColor ?? Color.Blue),
                Color.FromArgb(5, Theme?.PrimaryColor ?? Color.Blue),
                LinearGradientMode.Horizontal);
            g.FillRoundedRectangle(titleBrush, ctx.HeaderRect, 4);

            // Comparison icon
            var iconRect = new Rectangle(ctx.HeaderRect.X + 8, ctx.HeaderRect.Y + 4, 16, 16);
            _imagePainter.DrawSvg(g, "git-compare", iconRect, 
                Theme?.PrimaryColor ?? Color.Blue, 0.8f);

            // Title text
            var titleTextRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 16, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleTextBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleTextBrush, titleTextRect, format);
        }

        private void DrawComparisonPanels(Graphics g, Rectangle leftRect, Rectangle rightRect, List<Dictionary<string, object>> metrics)
        {
            if (metrics.Count < 2) return;
            
            DrawComparisonPanel(g, leftRect, metrics[0], Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), "Current", true);
            DrawComparisonPanel(g, rightRect, metrics.Count > 1 ? metrics[1] : metrics[0], 
                Theme?.AccentColor ?? Color.FromArgb(255, 152, 0), "Previous", false);
            
            // Enhanced VS separator with icon and hover
            using var vsBgBrush = new SolidBrush(Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray));
            var circleRect = new Rectangle(_vsIconRect.X - 2, _vsIconRect.Y - 2, 16, 16);
            g.FillEllipse(vsBgBrush, circleRect);
            
            bool vsHovered = IsAreaHovered("ComparisonGrid_VS");
            _imagePainter.DrawSvg(g, "git-compare", _vsIconRect, 
                vsHovered ? (Theme?.PrimaryColor ?? Color.Blue) : Color.FromArgb(150, Theme?.ForeColor ?? Color.Gray), 0.8f);
        }

        private void DrawComparisonPanel(Graphics g, Rectangle rect, Dictionary<string, object> metric, Color accentColor, string label, bool isCurrent)
        {
            // Enhanced gradient background
            using var panelBrush = new LinearGradientBrush(
                rect,
                Color.FromArgb(20, accentColor),
                Color.FromArgb(5, accentColor),
                LinearGradientMode.Vertical);
            using var panelPath = CreateRoundedPath(rect, 10);
            g.FillPath(panelBrush, panelPath);
            
            using var borderPen = new Pen(Color.FromArgb(40, accentColor), 1.5f);
            g.DrawPath(borderPen, panelPath);
            
            // Header with icon
            var headerRect = new Rectangle(rect.X + 12, rect.Y + 12, rect.Width - 24, 24);
            var iconRect = new Rectangle(headerRect.X, headerRect.Y + 4, 16, 16);
            
            string iconName = isCurrent ? "calendar" : "clock";
            _imagePainter.DrawSvg(g, iconName, iconRect, accentColor, 0.8f);
            
            var labelTextRect = new Rectangle(iconRect.Right + 6, headerRect.Y, 
                headerRect.Width - iconRect.Width - 6, headerRect.Height);
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f,FontStyle.Bold);
            using var labelBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
            var labelFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(label, labelFont, labelBrush, labelTextRect, labelFormat);
            
            // Value section
            var valueRect = new Rectangle(rect.X + 12, headerRect.Bottom + 8, rect.Width - 24, 40);
            if (metric.ContainsKey("Value"))
            {
                using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 18f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(accentColor);
                var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(metric["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
            }
            
            // Trend section with enhanced styling
            if (metric.ContainsKey("Trend"))
            {
                var trendRect = new Rectangle(rect.X + 12, valueRect.Bottom + 4, rect.Width - 24, 24);
                string trend = metric["Trend"].ToString();
                bool isPositive = trend.StartsWith("+");
                bool isNegative = trend.StartsWith("-");
                
                Color trendColor = isPositive ? Color.FromArgb(76, 175, 80) : 
                                 isNegative ? Color.FromArgb(244, 67, 54) : 
                                 Color.FromArgb(158, 158, 158);
                
                // Trend background
                using var trendBgBrush = new SolidBrush(Color.FromArgb(15, trendColor));
                var trendBgRect = new Rectangle(trendRect.X, trendRect.Y + 4, trendRect.Width, 16);
                g.FillRoundedRectangle(trendBgBrush, trendBgRect, 4);
                
                // Trend icon
                var trendIconRect = new Rectangle(trendRect.X + 8, trendRect.Y + 6, 12, 12);
                string trendIconName = isPositive ? "trending-up" : isNegative ? "trending-down" : "minus";
                _imagePainter.DrawSvg(g, trendIconName, trendIconRect, trendColor, 0.8f);
                
                // Trend text
                var trendTextRect = new Rectangle(trendIconRect.Right + 4, trendRect.Y, 
                    trendRect.Width - trendIconRect.Width - 12, trendRect.Height);
                using var trendFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f,FontStyle.Bold);
                using var trendBrush = new SolidBrush(trendColor);
                var trendFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(trend, trendFont, trendBrush, trendTextRect, trendFormat);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Title hover underline
            if (IsAreaHovered("ComparisonGrid_Title") && !string.IsNullOrEmpty(ctx.Title))
            {
                using var underlinePen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 2);
                g.DrawLine(underlinePen, ctx.HeaderRect.Left, ctx.HeaderRect.Bottom - 2, ctx.HeaderRect.Right, ctx.HeaderRect.Bottom - 2);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!string.IsNullOrEmpty(ctx.Title))
            {
                owner.AddHitArea("ComparisonGrid_Title", ctx.HeaderRect, null, () =>
                {
                    ctx.CustomData["TitleClicked"] = true;
                    notifyAreaHit?.Invoke("ComparisonGrid_Title", ctx.HeaderRect);
                    Owner?.Invalidate();
                });
            }

            if (!_leftPanelRect.IsEmpty)
            {
                owner.AddHitArea("ComparisonGrid_Left", _leftPanelRect, null, () =>
                {
                    ctx.CustomData["LeftPanelClicked"] = true;
                    notifyAreaHit?.Invoke("ComparisonGrid_Left", _leftPanelRect);
                    Owner?.Invalidate();
                });
            }
            if (!_rightPanelRect.IsEmpty)
            {
                owner.AddHitArea("ComparisonGrid_Right", _rightPanelRect, null, () =>
                {
                    ctx.CustomData["RightPanelClicked"] = true;
                    notifyAreaHit?.Invoke("ComparisonGrid_Right", _rightPanelRect);
                    Owner?.Invalidate();
                });
            }

            if (!_vsIconRect.IsEmpty)
            {
                owner.AddHitArea("ComparisonGrid_VS", _vsIconRect, null, () =>
                {
                    ctx.CustomData["VsClicked"] = true;
                    notifyAreaHit?.Invoke("ComparisonGrid_VS", _vsIconRect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}