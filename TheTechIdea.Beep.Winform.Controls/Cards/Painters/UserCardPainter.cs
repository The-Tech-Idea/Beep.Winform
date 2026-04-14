using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// UserCard - Team member/profile card with centered avatar, name, role, and actions.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class UserCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // User card fonts
        private Font _nameFont;
        private Font _roleFont;
        private Font _bioFont;
        private Font _badgeFont;
        
        // User card spacing
        private const int Padding = 20;
        private const int AvatarSize = 72;
        private const int AvatarBorderWidth = 3;
        private const int NameHeight = 26;
        private const int RoleHeight = 20;
        private const int BadgeWidth = 90;
        private const int BadgeHeight = 22;
        private const int BioMinHeight = 30;
        private const int ButtonHeight = 38;
        private const int ElementGap = 10;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_nameFont = titleFont;
            _roleFont = captionFont;
            _bioFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int avatarBorderWidth = DpiScalingHelper.ScaleValue(AvatarBorderWidth, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int roleHeight = DpiScalingHelper.ScaleValue(RoleHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int bioMinHeight = DpiScalingHelper.ScaleValue(BioMinHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Centered avatar at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - avatarSize) / 2,
                    drawingRect.Top + padding,
                    avatarSize,
                    avatarSize);
            }
            
            // Name (centered below avatar)
            int nameTop = ctx.ShowImage ? ctx.ImageRect.Bottom + elementGap * 2 : drawingRect.Top + padding;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                nameTop,
                drawingRect.Width - padding * 2,
                nameHeight);
            
            // Role/title (centered below name)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap / 2,
                drawingRect.Width - padding * 2,
                roleHeight);
            
            // Status badge (centered below role)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - badgeWidth) / 2,
                    ctx.SubtitleRect.Bottom + elementGap,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Bio/description
            int bioTop = ctx.SubtitleRect.Bottom + 
                (string.IsNullOrEmpty(ctx.BadgeText1) ? elementGap * 2 : badgeHeight + elementGap * 2);
            int bioHeight = Math.Max(bioMinHeight,
                drawingRect.Height - (bioTop - drawingRect.Top) - padding * 2 - 
                (ctx.ShowButton ? buttonHeight + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                bioTop,
                drawingRect.Width - padding * 2,
                bioHeight);
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - padding - buttonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (drawingRect.Width - padding * 3) / 2;
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + padding,
                        buttonTop,
                        buttonWidth,
                        buttonHeight);
                    
                    ctx.SecondaryButtonRect = new Rectangle(
                        ctx.ButtonRect.Right + padding,
                        buttonTop,
                        buttonWidth,
                        buttonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + padding,
                        buttonTop,
                        drawingRect.Width - padding * 2,
                        buttonHeight);
                }
            }
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _roleFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }

            // Draw status badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw avatar accent border ring
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Outer glow ring
                var glowRect = Rectangle.Inflate(ctx.ImageRect, 4, 4);
                using var glowPen = new Pen(Color.FromArgb(30, ctx.AccentColor), 2);
                g.DrawEllipse(glowPen, glowRect);
                
                // Main border ring
                using var borderPen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(AvatarBorderWidth, _owner));
                g.DrawEllipse(borderPen, ctx.ImageRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Status", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Status", ctx.BadgeRect));
            }
            
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Avatar", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Avatar", ctx.ImageRect));
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
