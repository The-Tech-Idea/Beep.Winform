using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _nameFont?.Dispose(); } catch { }
            try { _roleFont?.Dispose(); } catch { }
            try { _bioFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _nameFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _roleFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _bioFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Centered avatar at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - AvatarSize) / 2,
                    drawingRect.Top + Padding,
                    AvatarSize,
                    AvatarSize);
            }
            
            // Name (centered below avatar)
            int nameTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap * 2 : drawingRect.Top + Padding;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                nameTop,
                drawingRect.Width - Padding * 2,
                NameHeight);
            
            // Role/title (centered below name)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                RoleHeight);
            
            // Status badge (centered below role)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - BadgeWidth) / 2,
                    ctx.SubtitleRect.Bottom + ElementGap,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Bio/description
            int bioTop = ctx.SubtitleRect.Bottom + 
                (string.IsNullOrEmpty(ctx.BadgeText1) ? ElementGap * 2 : BadgeHeight + ElementGap * 2);
            int bioHeight = Math.Max(BioMinHeight,
                drawingRect.Height - (bioTop - drawingRect.Top) - Padding * 2 - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                bioTop,
                drawingRect.Width - Padding * 2,
                bioHeight);
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - Padding - ButtonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (drawingRect.Width - Padding * 3) / 2;
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        buttonTop,
                        buttonWidth,
                        ButtonHeight);
                    
                    ctx.SecondaryButtonRect = new Rectangle(
                        ctx.ButtonRect.Right + Padding,
                        buttonTop,
                        buttonWidth,
                        ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        buttonTop,
                        drawingRect.Width - Padding * 2,
                        ButtonHeight);
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
                using var borderPen = new Pen(ctx.AccentColor, AvatarBorderWidth);
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
            
            _nameFont?.Dispose();
            _roleFont?.Dispose();
            _bioFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
