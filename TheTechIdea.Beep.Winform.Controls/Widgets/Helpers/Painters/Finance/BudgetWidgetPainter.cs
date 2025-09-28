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
    /// BudgetWidget - Budget progress tracking painter with enhanced visual presentation
    /// Displays budget vs actual spending with progress indicators and variance analysis
    /// </summary>
    internal sealed class BudgetWidgetPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public BudgetWidgetPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Budget title and period
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Budget progress bar area
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                32
            );
            
            // Budget details (spent, remaining, variance)
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                50
            );
            
            // Budget status and alerts
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad - 4
            );
            
            return ctx;
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
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var financeItems = ctx.CustomData.ContainsKey("FinanceItems") ? 
                (List<FinanceItem>)ctx.CustomData["FinanceItems"] : new List<FinanceItem>();
            var primaryValue = ctx.CustomData.ContainsKey("PrimaryValue") ? (decimal)ctx.CustomData["PrimaryValue"] : 5000m;
            var secondaryValue = ctx.CustomData.ContainsKey("SecondaryValue") ? (decimal)ctx.CustomData["SecondaryValue"] : 3200m;
            var currencySymbol = ctx.CustomData.ContainsKey("CurrencySymbol") ? ctx.CustomData["CurrencySymbol"].ToString() : "$";

            var validColor = ctx.CustomData.ContainsKey("ValidColor") ? (Color)ctx.CustomData["ValidColor"] : Color.Green;
            var errorColor = ctx.CustomData.ContainsKey("ErrorColor") ? (Color)ctx.CustomData["ErrorColor"] : Color.Red;
            var warningColor = ctx.CustomData.ContainsKey("WarningColor") ? (Color)ctx.CustomData["WarningColor"] : Color.Orange;

            // Calculate budget metrics
            decimal budgetAmount = primaryValue; // Total budget
            decimal spentAmount = secondaryValue; // Amount spent
            decimal remainingAmount = budgetAmount - spentAmount;
            decimal usagePercentage = budgetAmount > 0 ? (spentAmount / budgetAmount) * 100 : 0;
            
            // Draw budget header
            DrawBudgetHeader(g, ctx.HeaderRect, ctx.Title, ctx.Value, budgetAmount, currencySymbol, ctx.AccentColor);
            
            // Draw budget progress bar
            DrawBudgetProgress(g, ctx.IconRect, usagePercentage, validColor, warningColor, errorColor);
            
            // Draw budget details
            DrawBudgetDetails(g, ctx.ContentRect, budgetAmount, spentAmount, remainingAmount, currencySymbol);
            
            // Draw budget status
            DrawBudgetStatus(g, ctx.FooterRect, usagePercentage, remainingAmount, currencySymbol, validColor, warningColor, errorColor);
        }

        private void DrawBudgetHeader(Graphics g, Rectangle rect, string title, string subtitle, 
            decimal budgetAmount, string currencySymbol, Color accentColor)
        {
            // Draw budget title
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(title, titleFont, titleBrush, rect.X, rect.Y);
            
            // Draw budget period or subtitle
            if (!string.IsNullOrEmpty(subtitle))
            {
                using var subtitleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var subtitleBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X, rect.Y + 16);
            }
            
            // Draw total budget amount
            using var amountFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var amountBrush = new SolidBrush(accentColor);
            string budgetText = $"{currencySymbol}{budgetAmount:N0}";
            var amountSize = g.MeasureString(budgetText, amountFont);
            g.DrawString(budgetText, amountFont, amountBrush, rect.Right - amountSize.Width, rect.Y);
            
            // Draw "Total Budget" label
            using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string labelText = "Total Budget";
            var labelSize = g.MeasureString(labelText, labelFont);
            g.DrawString(labelText, labelFont, labelBrush, rect.Right - labelSize.Width, rect.Y + 16);
        }

        private void DrawBudgetProgress(Graphics g, Rectangle rect, decimal usagePercentage, 
            Color validColor, Color warningColor, Color errorColor)
        {
            // Determine progress color based on usage
            Color progressColor;
            if (usagePercentage <= 70)
                progressColor = validColor;
            else if (usagePercentage <= 90)
                progressColor = warningColor;
            else
                progressColor = errorColor;
            
            // Draw progress background
            using var bgBrush = new SolidBrush(Color.FromArgb(40, Color.Gray));
            using var bgPath = CreateRoundedPath(rect, 16);
            g.FillPath(bgBrush, bgPath);
            
            // Draw progress fill
            float progressWidth = Math.Min((float)(usagePercentage / 100.0) * rect.Width, rect.Width);
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, (int)progressWidth, rect.Height);
                using var progressBrush = new SolidBrush(Color.FromArgb(200, progressColor));
                using var progressPath = CreateRoundedPath(progressRect, 16);
                g.FillPath(progressBrush, progressPath);
            }
            
            // Draw progress text
            using var progressFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var progressTextBrush = new SolidBrush(Color.White);
            string progressText = $"{usagePercentage:F1}%";
            var textSize = g.MeasureString(progressText, progressFont);
            var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(progressText, progressFont, progressTextBrush, rect, textFormat);
            
            // Draw usage indicator icons
            DrawUsageIndicators(g, rect, usagePercentage, validColor, warningColor, errorColor);
        }

        private void DrawUsageIndicators(Graphics g, Rectangle rect, decimal usagePercentage, 
            Color validColor, Color warningColor, Color errorColor)
        {
            // Draw milestone markers at 50%, 75%, 100%
            var milestones = new[] { 50f, 75f, 100f };
            var milestoneColors = new[] { validColor, warningColor, errorColor };
            
            for (int i = 0; i < milestones.Length; i++)
            {
                float milestoneX = rect.X + (milestones[i] / 100f) * rect.Width;
                
                // Only draw milestone if it's within bounds
                if (milestoneX <= rect.Right)
                {
                    using var milestonePen = new Pen(milestoneColors[i], 2);
                    g.DrawLine(milestonePen, milestoneX, rect.Y - 2, milestoneX, rect.Bottom + 2);
                    
                    // Draw milestone percentage
                    using var milestoneFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
                    using var milestoneBrush = new SolidBrush(milestoneColors[i]);
                    g.DrawString($"{milestones[i]:F0}%", milestoneFont, milestoneBrush, milestoneX - 8, rect.Bottom + 4);
                }
            }
        }

        private void DrawBudgetDetails(Graphics g, Rectangle rect, decimal budgetAmount, decimal spentAmount, 
            decimal remainingAmount, string currencySymbol)
        {
            int columnWidth = rect.Width / 3;
            
            // Draw spent amount
            DrawBudgetMetric(g, new Rectangle(rect.X, rect.Y, columnWidth, rect.Height),
                "Spent", $"{currencySymbol}{spentAmount:N0}", Color.FromArgb(244, 67, 54));
            
            // Draw remaining amount
            Color remainingColor = remainingAmount >= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            DrawBudgetMetric(g, new Rectangle(rect.X + columnWidth, rect.Y, columnWidth, rect.Height),
                "Remaining", $"{currencySymbol}{Math.Abs(remainingAmount):N0}", remainingColor);
            
            // Draw variance percentage
            decimal variancePercentage = budgetAmount > 0 ? ((spentAmount - budgetAmount) / budgetAmount) * 100 : 0;
            Color varianceColor = variancePercentage <= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            string varianceText = $"{(variancePercentage >= 0 ? "+" : "")}{variancePercentage:F1}%";
            
            DrawBudgetMetric(g, new Rectangle(rect.X + columnWidth * 2, rect.Y, columnWidth, rect.Height),
                "Variance", varianceText, varianceColor);
        }

        private void DrawBudgetMetric(Graphics g, Rectangle rect, string label, string value, Color color)
        {
            // Draw value
            using var valueFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var valueBrush = new SolidBrush(color);
            var valueFormat = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(value, valueFont, valueBrush, rect, valueFormat);
            
            // Draw label
            using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var labelRect = new Rectangle(rect.X, rect.Y + 20, rect.Width, rect.Height - 20);
            g.DrawString(label, labelFont, labelBrush, labelRect, valueFormat);
        }

        private void DrawBudgetStatus(Graphics g, Rectangle rect, decimal usagePercentage, decimal remainingAmount, 
            string currencySymbol, Color validColor, Color warningColor, Color errorColor)
        {
            // Determine status based on usage and remaining amount
            string statusText;
            Color statusColor;
            string statusIcon;
            
            if (usagePercentage <= 70)
            {
                statusText = "On Track - Budget looks healthy";
                statusColor = validColor;
                statusIcon = "?";
            }
            else if (usagePercentage <= 90)
            {
                statusText = "Monitor - Approaching budget limit";
                statusColor = warningColor;
                statusIcon = "?";
            }
            else if (usagePercentage <= 100)
            {
                statusText = "Alert - Near budget exhaustion";
                statusColor = errorColor;
                statusIcon = "?";
            }
            else
            {
                statusText = "Over Budget - Immediate attention required";
                statusColor = errorColor;
                statusIcon = "?";
            }
            
            // Draw status icon
            using var iconFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var iconBrush = new SolidBrush(statusColor);
            g.DrawString(statusIcon, iconFont, iconBrush, rect.X, rect.Y);
            
            // Draw status text
            using var statusFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusBrush = new SolidBrush(Color.FromArgb(160, Color.Black));
            g.DrawString(statusText, statusFont, statusBrush, rect.X + 20, rect.Y + 2);
            
            // Draw days remaining or overdue (if applicable)
            if (remainingAmount > 0)
            {
                var daysRemaining = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                using var daysFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var daysBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                string daysText = $"{daysRemaining} days left in period";
                g.DrawString(daysText, daysFont, daysBrush, rect.X + 20, rect.Y + 16);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw budget category indicator or additional metrics
            var indicatorRect = new Rectangle(ctx.DrawingRect.Right - 60, ctx.DrawingRect.Top + 8, 50, 20);
            
            // Draw budget type badge with icon
            using var badgeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
            using var badgeBrush = new SolidBrush(Color.FromArgb(150, ctx.AccentColor));
            using var badgeTextBrush = new SolidBrush(Color.White);
            
            using var badgePath = CreateRoundedPath(indicatorRect, 10);
            g.FillPath(badgeBrush, badgePath);
            
            // Add budget icon to badge
            var badgeIconRect = new Rectangle(indicatorRect.X + 2, indicatorRect.Y + 2, 12, 12);
            _imagePainter.DrawSvg(g, "target", badgeIconRect, Color.White, 0.8f);
            
            var badgeTextRect = new Rectangle(badgeIconRect.Right + 2, indicatorRect.Y, 
                indicatorRect.Width - badgeIconRect.Width - 4, indicatorRect.Height);
            var badgeFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("BUDGET", badgeFont, badgeTextBrush, badgeTextRect, badgeFormat);
        }

        private void DrawBudgetHeader(Graphics g, WidgetContext ctx, decimal budgetAmount, decimal spentAmount, string currencySymbol)
        {
            // Enhanced header with budget icon
            var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "pie-chart", iconRect, Theme?.PrimaryColor ?? Color.Blue, 0.9f);

            var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
            
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            
            string titleText = ctx.Title ?? "Budget Overview";
            g.DrawString(titleText, titleFont, titleBrush, titleRect, format);
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}