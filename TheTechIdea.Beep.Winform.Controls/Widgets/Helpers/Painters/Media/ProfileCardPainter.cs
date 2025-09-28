using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// ProfileCard - User profile card with avatar, name, role, and stats
    /// </summary>
    internal sealed class ProfileCardPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _avatarPainter;

        public ProfileCardPainter()
        {
            _avatarPainter = new ImagePainter();
            _avatarPainter.ScaleMode = ImageScaleMode.Fill;
            _avatarPainter.ClipShape = ImageClipShape.Circle;
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Avatar area (top center)
            int avatarSize = Math.Min(80, ctx.DrawingRect.Width / 3);
            ctx.AvatarRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - avatarSize) / 2,
                ctx.DrawingRect.Top + pad,
                avatarSize,
                avatarSize
            );
            
            // Name area
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.AvatarRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Role/subtitle area
            ctx.SubHeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                16
            );
            
            // Content/stats area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.SubHeaderRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.SubHeaderRect.Bottom - pad * 2
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
            // Update theme configuration
            if (Theme != null)
            {
                _avatarPainter.Theme = Theme;
            }

            // Draw avatar
            DrawAvatar(g, ctx.AvatarRect, ctx);
            
            // Draw avatar border
            using var avatarBorderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
            g.DrawEllipse(avatarBorderPen, ctx.AvatarRect);

            // Draw name
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var nameFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var nameBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, nameFont, nameBrush, ctx.HeaderRect, format);
            }

            // Draw role/subtitle
            if (!string.IsNullOrEmpty(ctx.Subtitle))
            {
                using var roleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var roleBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Subtitle, roleFont, roleBrush, ctx.SubHeaderRect, format);
            }

            // Draw stats/content area
            DrawProfileStats(g, ctx.ContentRect, ctx);
        }

        private void DrawAvatar(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Try to draw custom avatar using ImagePainter
            bool avatarDrawn = false;
            if (!string.IsNullOrEmpty(ctx.ImagePath))
            {
                try
                {
                    _avatarPainter.DrawImage(g, ctx.ImagePath, rect);
                    avatarDrawn = true;
                }
                catch { }
            }
            
            // Draw placeholder avatar if no custom image
            if (!avatarDrawn)
            {
                DrawPlaceholderAvatar(g, rect, ctx.AccentColor);
            }
        }

        private void DrawPlaceholderAvatar(Graphics g, Rectangle rect, Color color)
        {
            // Draw gradient background
            using var gradientBrush = new LinearGradientBrush(
                rect, 
                Color.FromArgb(150, color), 
                Color.FromArgb(80, color), 
                LinearGradientMode.Vertical);
            
            g.FillEllipse(gradientBrush, rect);
            
            // Draw simple person silhouette
            using var personPen = new Pen(Color.FromArgb(200, Color.White), 3);
            
            // Head circle
            var headSize = rect.Width / 3;
            var headRect = new Rectangle(
                rect.X + (rect.Width - headSize) / 2,
                rect.Y + rect.Height / 4,
                headSize,
                headSize
            );
            g.DrawEllipse(personPen, headRect);
            
            // Body arc
            var bodyRect = new Rectangle(
                rect.X + rect.Width / 6,
                rect.Y + rect.Height * 2 / 3,
                rect.Width * 2 / 3,
                rect.Height / 2
            );
            g.DrawArc(personPen, bodyRect, 0, 180);
        }

        private void DrawProfileStats(Graphics g, Rectangle area, WidgetContext ctx)
        {
            var stats = new[] 
            {
                ("Following", "142"),
                ("Followers", "1.2k"),
                ("Posts", "89")
            };
            
            int statWidth = area.Width / stats.Length;
            
            for (int i = 0; i < stats.Length; i++)
            {
                var statRect = new Rectangle(
                    area.X + i * statWidth,
                    area.Y,
                    statWidth,
                    area.Height
                );
                
                // Draw stat value
                var valueRect = new Rectangle(statRect.X, statRect.Y, statRect.Width, statRect.Height / 2);
                using var valueFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(stats[i].Item2, valueFont, valueBrush, valueRect, format);
                
                // Draw stat label
                var labelRect = new Rectangle(statRect.X, statRect.Y + statRect.Height / 2, statRect.Width, statRect.Height / 2);
                using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                g.DrawString(stats[i].Item1, labelFont, labelBrush, labelRect, format);
                
                // Draw separator line
                if (i < stats.Length - 1)
                {
                    using var separatorPen = new Pen(Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray), 1);
                    g.DrawLine(separatorPen, 
                        statRect.Right, statRect.Y + 8, 
                        statRect.Right, statRect.Bottom - 8);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw online status indicator
            if (ctx.ShowStatus)
            {
                var statusRect = new Rectangle(
                    ctx.AvatarRect.Right - 16, 
                    ctx.AvatarRect.Bottom - 16, 
                    12, 12
                );
                
                using var statusBrush = new SolidBrush(Color.LimeGreen);
                using var statusBorder = new Pen(Theme?.BackColor ?? Color.White, 2);
                g.FillEllipse(statusBrush, statusRect);
                g.DrawEllipse(statusBorder, statusRect);
            }
        }

        public void Dispose()
        {
            _avatarPainter?.Dispose();
        }
    }
}
