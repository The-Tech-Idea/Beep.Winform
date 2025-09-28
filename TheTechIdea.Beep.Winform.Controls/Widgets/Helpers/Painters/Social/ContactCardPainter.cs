using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// ContactCard - Contact information card painter
    /// </summary>
    internal sealed class ContactCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Avatar area (left side)
            int avatarSize = 48;
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                avatarSize,
                avatarSize
            );
            
            // Contact name area
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 2 - 12,
                20
            );
            
            // Contact details area
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 2 - 12,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 2 - 8
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 4, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            string contactName = ctx.CustomData.ContainsKey("ContactName") ? ctx.CustomData["ContactName"].ToString() : "Contact Name";
            string contactEmail = ctx.CustomData.ContainsKey("ContactEmail") ? ctx.CustomData["ContactEmail"].ToString() : "email@example.com";
            string contactPhone = ctx.CustomData.ContainsKey("ContactPhone") ? ctx.CustomData["ContactPhone"].ToString() : "+1 (555) 123-4567";
            var avatarImage = ctx.CustomData.ContainsKey("AvatarImage") ? ctx.CustomData["AvatarImage"] as Image : null;

            // Draw avatar
            DrawContactAvatar(g, ctx.IconRect, avatarImage, contactName, ctx.AccentColor);
            
            // Draw contact name
            using var nameFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(contactName, nameFont, nameBrush, ctx.HeaderRect);
            
            // Draw contact details
            using var detailsFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var detailsBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            var contentRect = ctx.ContentRect;
            g.DrawString(contactEmail, detailsFont, detailsBrush, contentRect.X, contentRect.Y);
            g.DrawString(contactPhone, detailsFont, detailsBrush, contentRect.X, contentRect.Y + 16);
        }

        private void DrawContactAvatar(Graphics g, Rectangle rect, Image avatarImage, string contactName, Color accentColor)
        {
            // Draw avatar background
            using var avatarBgBrush = new SolidBrush(Color.FromArgb(30, accentColor));
            g.FillEllipse(avatarBgBrush, rect);
            
            // Draw avatar border
            using var borderPen = new Pen(Color.FromArgb(200, Color.White), 2);
            g.DrawEllipse(borderPen, rect);
            
            if (avatarImage != null)
            {
                // Draw avatar image
                using var avatarPath = new System.Drawing.Drawing2D.GraphicsPath();
                avatarPath.AddEllipse(rect);
                g.SetClip(avatarPath);
                g.DrawImage(avatarImage, rect);
                g.ResetClip();
            }
            else
            {
                // Draw initials
                string initials = GetInitials(contactName);
                using var initialsFont = new Font(Owner.Font.FontFamily, rect.Width / 4, FontStyle.Bold);
                using var initialsBrush = new SolidBrush(accentColor);
                
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialsFont, initialsBrush, rect, format);
            }
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
            // Optional: Draw contact action buttons or status indicators
        }
    }
}