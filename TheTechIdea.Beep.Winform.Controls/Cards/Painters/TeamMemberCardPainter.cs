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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_nameFont = titleFont;
            _roleFont = captionFont;
            _bioFont = bodyFont;
            _statsFont = captionFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int roleHeight = DpiScalingHelper.ScaleValue(RoleHeight, _owner);
            int bioMinHeight = DpiScalingHelper.ScaleValue(BioMinHeight, _owner);
            int socialIconSize = DpiScalingHelper.ScaleValue(SocialIconSize, _owner);
            int socialIconGap = DpiScalingHelper.ScaleValue(SocialIconGap, _owner);
            int statHeight = DpiScalingHelper.ScaleValue(StatHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Avatar (centered at top)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - avatarSize) / 2,
                    drawingRect.Top + padding,
                    avatarSize,
                    avatarSize);
            }
            
            // Status badge (top-right, overlapping avatar)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + elementGap : drawingRect.Top + padding;
            
            // Name (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                drawingRect.Width - padding * 2,
                nameHeight);
            
            // Role/Title (centered)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + 2,
                drawingRect.Width - padding * 2,
                roleHeight);
            
            // Bio/description
            int bioHeight = Math.Max(bioMinHeight,
                drawingRect.Height - (ctx.SubtitleRect.Bottom - drawingRect.Top) - padding * 2 - 
                statHeight - elementGap * 2 - (ctx.ShowButton ? socialIconSize + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.SubtitleRect.Bottom + elementGap,
                drawingRect.Width - padding * 2,
                bioHeight);
            
            // Stats row (followers, projects, etc.)
            ctx.RatingRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.ParagraphRect.Bottom + elementGap,
                drawingRect.Width - padding * 2,
                statHeight);
            
            // Social links row at bottom
            bool showSocialLinks = ctx.ShowButton;
            if (showSocialLinks)
            {
                int socialRowWidth = (socialIconSize + socialIconGap) * 4 - socialIconGap;
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - socialRowWidth) / 2,
                    drawingRect.Bottom - padding - socialIconSize,
                    socialRowWidth,
                    socialIconSize);
            }
            else
            {
                ctx.ButtonRect = Rectangle.Empty;
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
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _roleFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }

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
            if (!ctx.ButtonRect.IsEmpty)
            {
                DrawSocialIcons(g, ctx);
            }
        }
        
        private void DrawAvatarRing(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw ring around avatar
            var ringRect = new Rectangle(
                ctx.ImageRect.X - DpiScalingHelper.ScaleValue(3, _owner),
                ctx.ImageRect.Y - DpiScalingHelper.ScaleValue(3, _owner),
                ctx.ImageRect.Width + DpiScalingHelper.ScaleValue(6, _owner),
                ctx.ImageRect.Height + DpiScalingHelper.ScaleValue(6, _owner));
            
            using var ringPen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawEllipse(ringPen, ringRect);
            
            // Draw online status dot
            if (ctx.ShowStatus)
            {
                int dotSize = DpiScalingHelper.ScaleValue(16, _owner);
                var dotRect = new Rectangle(
                    ctx.ImageRect.Right - dotSize + DpiScalingHelper.ScaleValue(2, _owner),
                    ctx.ImageRect.Bottom - dotSize + DpiScalingHelper.ScaleValue(2, _owner),
                    dotSize,
                    dotSize);
                
                using var dotBrush = new SolidBrush(ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.FromArgb(76, 175, 80));
                g.FillEllipse(dotBrush, dotRect);
                
                using var dotBorderPen = new Pen(Color.White, DpiScalingHelper.ScaleValue(2, _owner));
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
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(parts[0], _nameFont, numberBrush, numberRect, format);
                    
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
                int socialIconSize = DpiScalingHelper.ScaleValue(SocialIconSize, _owner);
                int socialIconGap = DpiScalingHelper.ScaleValue(SocialIconGap, _owner);
                var iconRect = new Rectangle(x, ctx.ButtonRect.Top, socialIconSize, socialIconSize);
                
                // Circle background
                using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillEllipse(bgBrush, iconRect);
                
                // Icon text
                using var textBrush = new SolidBrush(Color.FromArgb(150, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(icon, _statsFont, textBrush, iconRect, format);
                
                x += socialIconSize + socialIconGap;
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
            if (!ctx.ButtonRect.IsEmpty)
            {
                int x = ctx.ButtonRect.Left;
                string[] icons = { "LinkedIn", "Twitter", "GitHub", "Email" };
                
                foreach (var icon in icons)
                {
                    int socialIconSize = DpiScalingHelper.ScaleValue(SocialIconSize, _owner);
                    int socialIconGap = DpiScalingHelper.ScaleValue(SocialIconGap, _owner);
                    var iconRect = new Rectangle(x, ctx.ButtonRect.Top, socialIconSize, socialIconSize);
                    owner.AddHitArea(icon, iconRect, null,
                        () => notifyAreaHit?.Invoke(icon, iconRect));
                    x += socialIconSize + socialIconGap;
                }
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

