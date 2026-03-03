using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// TaskCard - Task/todo item with checkbox, priority indicator, and due date.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class TaskCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Task card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _dueDateFont;
        private Font _priorityFont;
        private Font _badgeFont;
        
        // Task card spacing
        private const int Padding = 14;
        private const int CheckboxSize = 24;
        private const int PriorityBarWidth = 4;
        private const int TitleHeight = 24;
        private const int DescHeight = 40;
        private const int MetaHeight = 18;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int ElementGap = 8;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _dueDateFont = titleFont;
            _priorityFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int checkboxSize = DpiScalingHelper.ScaleValue(CheckboxSize, _owner);
            int priorityBarWidth = DpiScalingHelper.ScaleValue(PriorityBarWidth, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descHeightMin = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int metaHeight = DpiScalingHelper.ScaleValue(MetaHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Priority indicator bar on left edge
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    priorityBarWidth,
                    drawingRect.Height);
            }
            
            int contentLeft = drawingRect.Left + padding + (ctx.ShowStatus ? priorityBarWidth : 0);
            
            // Checkbox (vertically centered on left)
            ctx.ImageRect = new Rectangle(
                contentLeft,
                drawingRect.Top + (drawingRect.Height - checkboxSize) / 2,
                checkboxSize,
                checkboxSize);
            
            // Content area (right of checkbox)
            int textLeft = ctx.ImageRect.Right + contentGap;
            int textWidth = drawingRect.Width - (textLeft - drawingRect.Left) - padding;
            
            // Priority/Status badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
                textWidth -= badgeWidth + elementGap;
            }
            
            // Task title
            ctx.HeaderRect = new Rectangle(
                textLeft,
                drawingRect.Top + padding,
                textWidth,
                titleHeight);
            
            // Task description
            int descHeight = Math.Max(descHeightMin, drawingRect.Height - padding * 2 - titleHeight - metaHeight - elementGap * 2);
            ctx.ParagraphRect = new Rectangle(
                textLeft,
                ctx.HeaderRect.Bottom + elementGap / 2,
                textWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                descHeight);
            
            // Due date and metadata
            ctx.SubtitleRect = new Rectangle(
                textLeft,
                ctx.ParagraphRect.Bottom + elementGap,
                textWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                metaHeight);
            
            // Tags area (assignee, labels)
            if (ctx.Tags != null)
            {
                ctx.TagsRect = new Rectangle(
                    textLeft,
                    drawingRect.Bottom - padding - metaHeight,
                    textWidth,
                    metaHeight);
            }
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw priority indicator bar
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.StatusColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }
            
            // Draw checkbox
            DrawCheckbox(g, ctx);
            
            // Draw priority badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw due date with icon
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                // Calendar icon placeholder
                int iconSize = 14;
                var iconRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top + 2, iconSize, iconSize);
                
                using var iconBrush = new SolidBrush(Color.FromArgb(100, ctx.AccentColor));
                g.FillRectangle(iconBrush, iconRect);
                
                // Due date text
                var textRect = new Rectangle(
                    iconRect.Right + 4,
                    ctx.SubtitleRect.Top,
                    ctx.SubtitleRect.Width - iconSize - 4,
                    ctx.SubtitleRect.Height);
                
                using var textBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _dueDateFont, textBrush, textRect, format);
            }
            
            // Draw tags/labels
            if (ctx.Tags != null && !ctx.TagsRect.IsEmpty)
            {
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags, _descFont);
            }
        }
        
        private void DrawCheckbox(Graphics g, LayoutContext ctx)
        {
            if (ctx.ImageRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Determine if checked (use ShowRating as a proxy for checked state)
            bool isChecked = ctx.ShowRating;
            
            // Draw checkbox background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 4);
            
            if (isChecked)
            {
                // Filled checkbox
                using var fillBrush = new SolidBrush(ctx.AccentColor);
                g.FillPath(fillBrush, bgPath);
                
                // Draw checkmark
                using var checkPen = new Pen(Color.White, 2.5f);
                checkPen.StartCap = LineCap.Round;
                checkPen.EndCap = LineCap.Round;
                
                int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
                int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
                g.DrawLine(checkPen, cx - 5, cy, cx - 1, cy + 4);
                g.DrawLine(checkPen, cx - 1, cy + 4, cx + 6, cy - 4);
            }
            else
            {
                // Empty checkbox with border
                using var borderPen = new Pen(Color.FromArgb(100, ctx.AccentColor), 2);
                g.DrawPath(borderPen, bgPath);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Checkbox hit area
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Checkbox", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Checkbox", ctx.ImageRect));
            }
            
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Priority", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Priority", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
_disposed = true;
        }
        
        #endregion
    }
}

