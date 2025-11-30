using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// TeamMemberCard - Team member showcase with role, social links, and stats.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class TeamMemberCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Team member card fonts
        private Font _nameFont;
        private Font _roleFont;
        private Font _bioFont;
        private Font _statsFont;
        private Font _badgeFont;
        
        // Team member card spacing
        private const int Padding = 16;
        private const int AvatarSize = 80;
        private const int NameHeight = 26;
        private const int RoleHeight = 20;
        private const int BioMinHeight = 40;
        private const int SocialIconSize = 28;
        private const int SocialIconGap = 10;
        private const int StatWidth = 60;
        private const int StatHeight = 40;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int ElementGap = 8;
        
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
            try { _statsFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _nameFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _roleFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _bioFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _statsFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Avatar (centered at top)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - AvatarSize) / 2,
                    drawingRect.Top + Padding,
                    AvatarSize,
                    AvatarSize);
            }
            
            // Status badge (top-right, overlapping avatar)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap : drawingRect.Top + Padding;
            
            // Name (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                NameHeight);
            
            // Role/Title (centered)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + 2,
                drawingRect.Width - Padding * 2,
                RoleHeight);
            
            // Bio/description
            int bioHeight = Math.Max(BioMinHeight,
                drawingRect.Height - (ctx.SubtitleRect.Bottom - drawingRect.Top) - Padding * 2 - 
                StatHeight - ElementGap * 2 - (ctx.ShowButton ? SocialIconSize + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.SubtitleRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                bioHeight);
            
            // Stats row (followers, projects, etc.)
            ctx.RatingRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.ParagraphRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                StatHeight);
            
            // Social links row at bottom
            if (ctx.ShowButton)
            {
                int socialRowWidth = (SocialIconSize + SocialIconGap) * 4 - SocialIconGap;
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - socialRowWidth) / 2,
                    drawingRect.Bottom - Padding - SocialIconSize,
                    socialRowWidth,
                    SocialIconSize);
            }
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw avatar ring
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawAvatarRing(g, ctx);
            }
            
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw stats
            if (!ctx.RatingRect.IsEmpty)
            {
                DrawStats(g, ctx);
            }
            
            // Draw social icons
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                DrawSocialIcons(g, ctx);
            }
        }
        
        private void DrawAvatarRing(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw ring around avatar
            var ringRect = new Rectangle(
                ctx.ImageRect.X - 3,
                ctx.ImageRect.Y - 3,
                ctx.ImageRect.Width + 6,
                ctx.ImageRect.Height + 6);
            
            using var ringPen = new Pen(ctx.AccentColor, 3);
            g.DrawEllipse(ringPen, ringRect);
            
            // Draw online status dot
            if (ctx.ShowStatus)
            {
                int dotSize = 16;
                var dotRect = new Rectangle(
                    ctx.ImageRect.Right - dotSize + 2,
                    ctx.ImageRect.Bottom - dotSize + 2,
                    dotSize,
                    dotSize);
                
                using var dotBrush = new SolidBrush(ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.FromArgb(76, 175, 80));
                g.FillEllipse(dotBrush, dotRect);
                
                using var dotBorderPen = new Pen(Color.White, 2);
                g.DrawEllipse(dotBorderPen, dotRect);
            }
        }
        
        private void DrawStats(Graphics g, LayoutContext ctx)
        {
            // Parse stats from StatusText (format: "123 Projects | 456 Followers | 789 Following")
            if (string.IsNullOrEmpty(ctx.StatusText)) return;
            
            string[] stats = ctx.StatusText.Split(new[] { '|', '•', '·' }, StringSplitOptions.RemoveEmptyEntries);
            if (stats.Length == 0) return;
            
            int statCount = Math.Min(stats.Length, 4);
            int statWidth = ctx.RatingRect.Width / statCount;
            
            for (int i = 0; i < statCount; i++)
            {
                var statRect = new Rectangle(
                    ctx.RatingRect.Left + i * statWidth,
                    ctx.RatingRect.Top,
                    statWidth,
                    ctx.RatingRect.Height);
                
                string[] parts = stats[i].Trim().Split(' ');
                if (parts.Length >= 2)
                {
                    // Number
                    var numberRect = new Rectangle(statRect.X, statRect.Y, statRect.Width, statRect.Height / 2 + 4);
                    using var numberBrush = new SolidBrush(ctx.AccentColor);
                    using var numberFont = new Font(_owner.Font.FontFamily, 14f, FontStyle.Bold);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(parts[0], numberFont, numberBrush, numberRect, format);
                    
                    // Label
                    var labelRect = new Rectangle(statRect.X, statRect.Y + statRect.Height / 2, statRect.Width, statRect.Height / 2);
                    using var labelBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.DrawString(string.Join(" ", parts, 1, parts.Length - 1), _statsFont, labelBrush, labelRect, format);
                }
            }
        }
        
        private void DrawSocialIcons(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw placeholder social icons (4 icons)
            string[] icons = { "Li", "Tw", "Gh", "Em" }; // LinkedIn, Twitter, GitHub, Email
            
            int x = ctx.ButtonRect.Left;
            
            foreach (var icon in icons)
            {
                var iconRect = new Rectangle(x, ctx.ButtonRect.Top, SocialIconSize, SocialIconSize);
                
                // Circle background
                using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillEllipse(bgBrush, iconRect);
                
                // Icon text
                using var textBrush = new SolidBrush(Color.FromArgb(150, ctx.AccentColor));
                using var iconFont = new Font(_owner.Font.FontFamily, 8f, FontStyle.Bold);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(icon, iconFont, textBrush, iconRect, format);
                
                x += SocialIconSize + SocialIconGap;
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Avatar hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Avatar", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Avatar", ctx.ImageRect));
            }
            
            // Social icons hit area
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                int x = ctx.ButtonRect.Left;
                string[] icons = { "LinkedIn", "Twitter", "GitHub", "Email" };
                
                foreach (var icon in icons)
                {
                    var iconRect = new Rectangle(x, ctx.ButtonRect.Top, SocialIconSize, SocialIconSize);
                    owner.AddHitArea(icon, iconRect, null,
                        () => notifyAreaHit?.Invoke(icon, iconRect));
                    x += SocialIconSize + SocialIconGap;
                }
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
            _statsFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

