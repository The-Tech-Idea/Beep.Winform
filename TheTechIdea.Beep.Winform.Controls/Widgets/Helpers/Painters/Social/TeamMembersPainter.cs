using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// TeamMembers - Team member avatar grid
    /// </summary>
    internal sealed class TeamMembersPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _avatarPainter;

        public TeamMembersPainter()
        {
            _avatarPainter = new ImagePainter();
            _avatarPainter.ScaleMode = ImageScaleMode.Fill;
            _avatarPainter.ClipShape = ImageClipShape.Circle;
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }
            
            // Team members grid area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title with online count
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                int onlineCount = ctx.OnlineCount;
                int totalCount = ctx.TotalCount;
                
                string titleText = $"{ctx.Title} ({onlineCount}/{totalCount} online)";
                
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                g.DrawString(titleText, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw team member avatars
            var socialItems = ctx.SocialItems?.Cast<SocialItem>().ToList() ?? new List<SocialItem>();
            
            DrawTeamMemberGrid(g, ctx.ContentRect, socialItems.Take(6).ToList(), ctx.AccentColor);
        }

        private void DrawTeamMemberGrid(Graphics g, Rectangle rect, List<SocialItem> members, Color accentColor)
        {
            if (!members.Any()) return;
            
            int avatarSize = 40;
            int spacing = 8;
            int cols = Math.Min(3, members.Count);
            int rows = (int)Math.Ceiling(members.Count / (double)cols);
            
            for (int i = 0; i < Math.Min(members.Count, 6); i++)
            {
                var member = members[i];
                
                int col = i % cols;
                int row = i / cols;
                
                int x = rect.X + col * (avatarSize + spacing);
                int y = rect.Y + row * (avatarSize + spacing + 20); // Extra space for name
                
                var avatarRect = new Rectangle(x, y, avatarSize, avatarSize);
                
                // Draw avatar
                DrawMemberAvatar(g, avatarRect, member, accentColor);
                
                // Draw member name below avatar
                var nameRect = new Rectangle(x - 10, y + avatarSize + 2, avatarSize + 20, 16);
                using var nameFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
                using var nameBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var nameFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                
                // Truncate long names
                string displayName = member.Name.Length > 8 ? member.Name.Substring(0, 8) + "..." : member.Name;
                g.DrawString(displayName, nameFont, nameBrush, nameRect, nameFormat);
            }
        }

        private void DrawMemberAvatar(Graphics g, Rectangle rect, SocialItem member, Color accentColor)
        {
            // Draw avatar background
            using var avatarBrush = new SolidBrush(Color.FromArgb(30, accentColor));
            g.FillEllipse(avatarBrush, rect);
            
            // Draw avatar border
            using var borderPen = new Pen(Color.White, 2);
            g.DrawEllipse(borderPen, rect);
            
            if (member.AvatarImage != null || !string.IsNullOrEmpty(member.Avatar))
            {
                // Configure avatar painter
                _avatarPainter.CurrentTheme = Theme;
                _avatarPainter.ClipShape = ImageClipShape.Circle;
                _avatarPainter.ScaleMode = ImageScaleMode.Fill;
                
                // Set image source
                if (!string.IsNullOrEmpty(member.Avatar))
                {
                    _avatarPainter.ImagePath = member.Avatar;
                }
                else if (member.AvatarImage != null)
                {
                    _avatarPainter.Image = member.AvatarImage;
                }
                
                // Draw avatar with ImagePainter
                _avatarPainter.DrawImage(g, member.Avatar, rect);
            }
            else
            {
                // Draw initials
                string initials = GetInitials(member.Name);
                using var initialsFont = new Font(Owner.Font.FontFamily, rect.Width / 4, FontStyle.Bold);
                using var initialsBrush = new SolidBrush(accentColor);
                
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialsFont, initialsBrush, rect, format);
            }
            
            // Draw status indicator
            var statusColor = GetStatusColor(member.Status);
            DrawStatusIndicator(g, rect, statusColor);
        }

        private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, Color statusColor)
        {
            int indicatorSize = 10;
            var indicatorRect = new Rectangle(
                avatarRect.Right - indicatorSize,
                avatarRect.Bottom - indicatorSize,
                indicatorSize,
                indicatorSize
            );
            
            using var statusBgBrush = new SolidBrush(Color.White);
            g.FillEllipse(statusBgBrush, indicatorRect);
            
            var innerRect = Rectangle.Inflate(indicatorRect, -1, -1);
            using var statusBrush = new SolidBrush(statusColor);
            g.FillEllipse(statusBrush, innerRect);
        }

        private Color GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "online" => Color.FromArgb(76, 175, 80),  // Green
                "away" => Color.FromArgb(255, 193, 7),    // Yellow
                "busy" => Color.FromArgb(244, 67, 54),    // Red
                "offline" => Color.FromArgb(158, 158, 158), // Gray
                _ => Color.FromArgb(158, 158, 158)
            };
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
            // Optional: Draw hover effects
        }

        public void Dispose()
        {
            _avatarPainter?.Dispose();
        }
    }
}