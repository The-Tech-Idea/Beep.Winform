using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// SocialProfileCard - User profile display card for social widgets
    /// </summary>
    internal sealed class SocialProfileCardPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _avatarPainter;

        public SocialProfileCardPainter()
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
            int avatarSize = 60;
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - avatarSize) / 2,
                ctx.DrawingRect.Top + pad,
                avatarSize,
                avatarSize
            );
            
            // User name area
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // User role area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            // Status area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                24
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
            string userName = ctx.UserName ?? "User";
            string userRole = ctx.UserRole ?? "Role";
            string userStatus = ctx.UserStatus ?? "Offline";
            var avatarImage = ctx.AvatarImage as Image;
            var statusColor = ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.Gray;
            bool showStatus = ctx.ShowStatus;

            // Draw avatar
            DrawAvatar(g, ctx, ctx.IconRect, avatarImage, userName, ctx.AccentColor);
            
            // Draw status indicator on avatar if enabled
            if (showStatus)
            {
                DrawStatusIndicator(g, ctx.IconRect, statusColor);
            }
            
            // Draw user name
            using var nameFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            var nameFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(userName, nameFont, nameBrush, ctx.HeaderRect, nameFormat);
            
            // Draw user role
            using var roleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var roleBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            var roleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(userRole, roleFont, roleBrush, ctx.ContentRect, roleFormat);
            
            // Draw status text
            if (showStatus)
            {
                using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var statusBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"? {userStatus}", statusFont, statusBrush, ctx.FooterRect, statusFormat);
            }
        }

        private void DrawAvatar(Graphics g, WidgetContext ctx, Rectangle rect, Image avatarImage, string userName, Color accentColor)
        {
            // Draw avatar background
            using var avatarBgBrush = new SolidBrush(Color.FromArgb(30, accentColor));
            g.FillEllipse(avatarBgBrush, rect);
            
            // Draw avatar border
            using var borderPen = new Pen(Color.FromArgb(200, Color.White), 3);
            g.DrawEllipse(borderPen, rect);
            
            if (avatarImage != null || !string.IsNullOrEmpty(ctx.AvatarImagePath))
            {
                // Configure avatar painter
                _avatarPainter.CurrentTheme = Theme;
                _avatarPainter.ClipShape = ImageClipShape.Circle;
                _avatarPainter.ScaleMode = ImageScaleMode.Fill;
                
                // Set image source
                var avatarImagePath = ctx.AvatarImagePath;
                if (!string.IsNullOrEmpty(avatarImagePath))
                {
                    _avatarPainter.ImagePath = avatarImagePath;
                }
                else if (avatarImage != null)
                {
                    _avatarPainter.Image = avatarImage;
                }
                
                // Draw avatar with ImagePainter
                _avatarPainter.DrawImage(g, rect);
            }
            else
            {
                // Draw initials
                string initials = GetInitials(userName);
                using var initialsFont = new Font(Owner.Font.FontFamily, rect.Width / 4, FontStyle.Bold);
                using var initialsBrush = new SolidBrush(accentColor);
                
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialsFont, initialsBrush, rect, format);
            }
        }

        private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, Color statusColor)
        {
            int indicatorSize = 16;
            var indicatorRect = new Rectangle(
                avatarRect.Right - indicatorSize,
                avatarRect.Bottom - indicatorSize,
                indicatorSize,
                indicatorSize
            );
            
            // Draw status background
            using var statusBgBrush = new SolidBrush(Color.White);
            g.FillEllipse(statusBgBrush, indicatorRect);
            
            // Draw status color
            var innerRect = Rectangle.Inflate(indicatorRect, -2, -2);
            using var statusBrush = new SolidBrush(statusColor);
            g.FillEllipse(statusBrush, innerRect);
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw hover effects or selection indicators
        }

        public void Dispose()
        {
            _avatarPainter?.Dispose();
        }
    }
}