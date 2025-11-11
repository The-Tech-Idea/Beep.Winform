using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// BudgetWidget - Budget progress tracking painter with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class BudgetWidgetPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Interactive rects
        private Rectangle _progressRect;
        private Rectangle _spentRect;
        private Rectangle _remainingRect;
        private Rectangle _varianceRect;
        private Rectangle _statusRect;

        public BudgetWidgetPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = baseRect;
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 28);
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 32);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.IconRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 50);
            ctx.FooterRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ContentRect.Bottom + 4, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad - 4);

            // Store interactive zones
            _progressRect = ctx.IconRect;
            int columnWidth = Math.Max(1, ctx.ContentRect.Width / 3);
            _spentRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, columnWidth, ctx.ContentRect.Height);
            _remainingRect = new Rectangle(ctx.ContentRect.X + columnWidth, ctx.ContentRect.Y, columnWidth, ctx.ContentRect.Height);
            _varianceRect = new Rectangle(ctx.ContentRect.X + columnWidth * 2, ctx.ContentRect.Y, columnWidth, ctx.ContentRect.Height);
            _statusRect = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width, Math.Min(24, ctx.FooterRect.Height));

            return ctx;
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("Budget_Progress", _progressRect, null, () => { HandleProgressClick(ctx); notifyAreaHit?.Invoke("Budget_Progress", _progressRect); });
            owner.AddHitArea("Budget_Spent", _spentRect, null, () => { HandleSpentClick(ctx); notifyAreaHit?.Invoke("Budget_Spent", _spentRect); });
            owner.AddHitArea("Budget_Remaining", _remainingRect, null, () => { HandleRemainingClick(ctx); notifyAreaHit?.Invoke("Budget_Remaining", _remainingRect); });
            owner.AddHitArea("Budget_Variance", _varianceRect, null, () => { HandleVarianceClick(ctx); notifyAreaHit?.Invoke("Budget_Variance", _varianceRect); });
            owner.AddHitArea("Budget_Status", _statusRect, null, () => { HandleStatusClick(ctx); notifyAreaHit?.Invoke("Budget_Status", _statusRect); });
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            var primaryValue = ctx.PrimaryValue ?? 5000m;
            var secondaryValue = ctx.SecondaryValue ?? 3200m;
            var currencySymbol = ctx.CurrencySymbol ?? "$";

            var validColor = ctx.ValidColor;
            var errorColor = ctx.ErrorColor;
            var warningColor = ctx.WarningColor;

            decimal budgetAmount = primaryValue;
            decimal spentAmount = secondaryValue;
            decimal remainingAmount = budgetAmount - spentAmount;
            decimal usagePercentage = budgetAmount > 0 ? (spentAmount / budgetAmount) * 100 : 0;
            
            DrawBudgetHeader(g, ctx.HeaderRect, ctx.Title, ctx.Value, budgetAmount, currencySymbol, ctx.AccentColor);
            DrawBudgetProgress(g, _progressRect, usagePercentage, validColor, warningColor, errorColor);
            DrawBudgetDetails(g, ctx.ContentRect, budgetAmount, spentAmount, remainingAmount, currencySymbol);
            DrawBudgetStatus(g, ctx.FooterRect, usagePercentage, remainingAmount, currencySymbol, validColor, warningColor, errorColor);
        }

        private void DrawBudgetHeader(Graphics g, Rectangle rect, string title, string subtitle, decimal budgetAmount, string currencySymbol, Color accentColor)
        {
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Theme?.ForeColor ?? Color.Black));
            g.DrawString(title, titleFont, titleBrush, rect.X, rect.Y);
            if (!string.IsNullOrEmpty(subtitle))
            {
                using var subtitleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
                using var subtitleBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X, rect.Y + 16);
            }
            using var amountFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(accentColor);
            string budgetText = $"{currencySymbol}{budgetAmount:N0}";
            var amountSize = TextUtils.MeasureText(g,budgetText, amountFont);
            g.DrawString(budgetText, amountFont, amountBrush, rect.Right - amountSize.Width, rect.Y);
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
            string labelText = "Total Budget";
            var labelSize = TextUtils.MeasureText(g,labelText, labelFont);
            g.DrawString(labelText, labelFont, labelBrush, rect.Right - labelSize.Width, rect.Y + 16);
        }

        private void DrawBudgetProgress(Graphics g, Rectangle rect, decimal usagePercentage, Color validColor, Color warningColor, Color errorColor)
        {
            Color progressColor = usagePercentage <= 70 ? validColor : usagePercentage <= 90 ? warningColor : errorColor;
            using var bgBrush = new SolidBrush(Color.FromArgb(40, Theme?.ProgressBarBorderColor ?? Color.Gray));
            using var bgPath = CreateRoundedPath(rect, 16);
            g.FillPath(bgBrush, bgPath);
            // compute progress width using decimal math to avoid precision issues
            float progressWidth = Math.Min(((float)(usagePercentage / 100m)) * rect.Width, rect.Width);
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, (int)progressWidth, rect.Height);
                using var progressBrush = new SolidBrush(Color.FromArgb(200, progressColor));
                using var progressPath = CreateRoundedPath(progressRect, 16);
                g.FillPath(progressBrush, progressPath);
            }
            using var progressFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
            using var progressTextBrush = new SolidBrush(Theme?.ProgressBarForeColor ?? Color.White);
            string progressText = $"{usagePercentage:F1}%";
            var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(progressText, progressFont, progressTextBrush, rect, textFormat);
            DrawUsageIndicators(g, rect, usagePercentage, validColor, warningColor, errorColor);
        }

        private void DrawUsageIndicators(Graphics g, Rectangle rect, decimal usagePercentage, Color validColor, Color warningColor, Color errorColor)
        {
            var milestones = new[] { 50f, 75f, 100f };
            var milestoneColors = new[] { validColor, warningColor, errorColor };
            for (int i = 0; i < milestones.Length; i++)
            {
                float milestoneX = rect.X + (milestones[i] / 100f) * rect.Width;
                if (milestoneX <= rect.Right)
                {
                    using var milestonePen = new Pen(milestoneColors[i], 2);
                    g.DrawLine(milestonePen, milestoneX, rect.Y - 2, milestoneX, rect.Bottom + 2);
                    using var milestoneFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                    using var milestoneBrush = new SolidBrush(milestoneColors[i]);
                    g.DrawString($"{milestones[i]:F0}%", milestoneFont, milestoneBrush, milestoneX - 8, rect.Bottom + 4);
                }
            }
        }

        private void DrawBudgetDetails(Graphics g, Rectangle rect, decimal budgetAmount, decimal spentAmount, decimal remainingAmount, string currencySymbol)
        {
            int columnWidth = rect.Width / 3;
            DrawBudgetMetric(g, _spentRect, "Spent", $"{currencySymbol}{spentAmount:N0}", Color.FromArgb(244, 67, 54));
            Color remainingColor = remainingAmount >= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            DrawBudgetMetric(g, _remainingRect, "Remaining", $"{currencySymbol}{Math.Abs(remainingAmount):N0}", remainingColor);
            decimal variancePercentage = budgetAmount > 0 ? ((spentAmount - budgetAmount) / budgetAmount) * 100 : 0;
            Color varianceColor = variancePercentage <= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            string varianceText = $"{(variancePercentage >= 0 ? "+" : "")}{variancePercentage:F1}%";
            DrawBudgetMetric(g, _varianceRect, "Variance", varianceText, varianceColor);
        }

        private void DrawBudgetMetric(Graphics g, Rectangle rect, string label, string value, Color color)
        {
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
            using var valueBrush = new SolidBrush(color);
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(value, valueFont, valueBrush, rect, valueFormat);
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
            var labelRect = new Rectangle(rect.X, rect.Y + 20, rect.Width, rect.Height - 20);
            g.DrawString(label, labelFont, labelBrush, labelRect, valueFormat);
        }

        private void DrawBudgetStatus(Graphics g, Rectangle rect, decimal usagePercentage, decimal remainingAmount, string currencySymbol, Color validColor, Color warningColor, Color errorColor)
        {
            string statusText; Color statusColor; string statusIcon;
            if (usagePercentage <= 70) { statusText = "On Track - Budget looks healthy"; statusColor = validColor; statusIcon = "?"; }
            else if (usagePercentage <= 90) { statusText = "Monitor - Approaching budget limit"; statusColor = warningColor; statusIcon = "!"; }
            else if (usagePercentage <= 100) { statusText = "Alert - Near budget exhaustion"; statusColor = errorColor; statusIcon = "!"; }
            else { statusText = "Over Budget - Immediate attention required"; statusColor = errorColor; statusIcon = "!"; }
            using var iconFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var iconBrush = new SolidBrush(statusColor);
            g.DrawString(statusIcon, iconFont, iconBrush, rect.X, rect.Y);
            using var statusFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var statusBrush = new SolidBrush(Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
            g.DrawString(statusText, statusFont, statusBrush, rect.X + 20, rect.Y + 2);
            if (remainingAmount > 0)
            {
                var daysRemaining = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                using var daysFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
                using var daysBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                string daysText = $"{daysRemaining} days left in period";
                g.DrawString(daysText, daysFont, daysBrush, rect.X + 20, rect.Y + 16);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (IsAreaHovered("Budget_Progress"))
            {
                using var glow = new SolidBrush(Color.FromArgb(16, Theme?.PrimaryColor ?? Color.Blue));
                using var p = CreateRoundedPath(Rectangle.Inflate(_progressRect, 4, 2), 10);
                g.FillPath(glow, p);
            }
            if (IsAreaHovered("Budget_Spent"))
            {
                using var pen = new Pen(Color.FromArgb(150, 244, 67, 54), 1);
                g.DrawRectangle(pen, _spentRect);
            }
            if (IsAreaHovered("Budget_Remaining"))
            {
                using var pen = new Pen(Color.FromArgb(150, 76, 175, 80), 1);
                g.DrawRectangle(pen, _remainingRect);
            }
            if (IsAreaHovered("Budget_Variance"))
            {
                using var pen = new Pen(Color.FromArgb(150, Theme?.AccentColor ?? Color.Gray), 1);
                g.DrawRectangle(pen, _varianceRect);
            }
            if (IsAreaHovered("Budget_Status"))
            {
                using var glow = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(glow, _statusRect);
            }
        }

        private void HandleProgressClick(WidgetContext ctx) { ctx.ToggleProgressView = true; Owner?.Invalidate(); }
        private void HandleSpentClick(WidgetContext ctx) { ctx.ShowSpentDetails = true; Owner?.Invalidate(); }
        private void HandleRemainingClick(WidgetContext ctx) { ctx.ShowRemainingDetails = true; Owner?.Invalidate(); }
        private void HandleVarianceClick(WidgetContext ctx) { ctx.ShowVarianceAnalysis = true; Owner?.Invalidate(); }
        private void HandleStatusClick(WidgetContext ctx) { ctx.ShowStatusInfo = true; Owner?.Invalidate(); }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}