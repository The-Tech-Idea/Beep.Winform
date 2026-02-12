using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// ContactCard - Comprehensive contact information card painter
    /// Displays contact details, status, action buttons, and social information
    /// </summary>
    internal sealed class ContactCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private const int AVATAR_SIZE = 60;
        private const int ACTION_BUTTON_SIZE = 28;

        public ContactCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Contact layout type (horizontal or vertical)
            bool isVerticalLayout = ctx.IsVerticalLayout;
            
            if (isVerticalLayout)
            {
                LayoutVertical(ctx, pad);
            }
            else
            {
                LayoutHorizontal(ctx, pad);
            }
            
            return ctx;
        }

        private void LayoutHorizontal(WidgetContext ctx, int pad)
        {
            // Avatar area (left side)
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                AVATAR_SIZE,
                AVATAR_SIZE
            );
            
            // Contact info area (right of avatar)
            int infoLeft = ctx.IconRect.Right + 16;
            int infoWidth = ctx.DrawingRect.Width - (infoLeft - ctx.DrawingRect.X) - pad;
            
            // Contact name and status
            ctx.HeaderRect = new Rectangle(
                infoLeft,
                ctx.DrawingRect.Top + pad,
                infoWidth,
                24
            );
            
            // Contact details (email, phone, etc.)
            ctx.ContentRect = new Rectangle(
                infoLeft,
                ctx.HeaderRect.Bottom + 8,
                infoWidth,
                60
            );
            
            // Action buttons area
            ctx.FooterRect = new Rectangle(
                infoLeft,
                ctx.ContentRect.Bottom + 8,
                infoWidth,
                ACTION_BUTTON_SIZE
            );
        }

        private void LayoutVertical(WidgetContext ctx, int pad)
        {
            // Avatar centered at top
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - AVATAR_SIZE) / 2,
                ctx.DrawingRect.Top + pad,
                AVATAR_SIZE,
                AVATAR_SIZE
            );
            
            // Contact name centered below avatar
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Contact details centered
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                60
            );
            
            // Action buttons centered at bottom
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ACTION_BUTTON_SIZE
            );
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
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            // Get contact data
            var contact = ctx.ContactInfo ?? CreateSampleContact();
            
            bool isVerticalLayout = ctx.IsVerticalLayout;

            // Draw avatar
            DrawContactAvatar(g, ctx.IconRect, contact);
            
            // Draw contact name and status
            DrawContactHeader(g, ctx.HeaderRect, contact, isVerticalLayout);
            
            // Draw contact details
            DrawContactDetails(g, ctx.ContentRect, contact, isVerticalLayout);
            
            // Draw action buttons
            DrawActionButtons(g, ctx.FooterRect, contact, ctx.AccentColor, isVerticalLayout);
        }

        private void DrawContactAvatar(Graphics g, Rectangle rect, ContactInfo contact)
        {
            // Draw avatar background with status color
            Color statusColor = GetStatusColor(contact.Status);
            using var avatarBgBrush = new SolidBrush(Color.FromArgb(30, statusColor));
            g.FillEllipse(avatarBgBrush, rect);
            
            // Draw avatar image or initials
            if (!string.IsNullOrEmpty(contact.AvatarPath))
            {
                // Use the StyledImagePainter to paint avatars in circle shape with caching & tint support
                try
                {
                    float radius = Math.Min(rect.Width, rect.Height) / 2f;
                    float cx = rect.X + rect.Width / 2f;
                    float cy = rect.Y + rect.Height / 2f;
                    StyledImagePainter.PaintInCircle(g, cx, cy, radius, contact.AvatarPath);
                }
                catch
                {
                    // Fallback: previous ImagePainter behavior
                    _imagePainter.ImagePath = contact.AvatarPath;
                    _imagePainter.ClipShape = Vis.Modules.ImageClipShape.Circle;
                    _imagePainter.DrawImage(g, contact.AvatarPath, rect);
                }
            }
            else
            {
                // Draw initials
                string initials = GetInitials(contact.Name);
                using var initialsFont = new Font(Owner.Font.FontFamily, rect.Width / 3.5f, FontStyle.Bold);
                using var initialsBrush = new SolidBrush(statusColor);
                
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialsFont, initialsBrush, rect, format);
            }
            
            // Draw status indicator
            DrawStatusIndicator(g, rect, contact.Status);
            
            // Draw avatar border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 2);
            g.DrawEllipse(borderPen, rect);
        }

        private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, ContactStatus status)
        {
            // Status indicator at bottom-right of avatar
            var statusRect = new Rectangle(
                avatarRect.Right - 16, 
                avatarRect.Bottom - 16, 
                12, 12
            );
            
            Color statusColor = GetStatusColor(status);
            using var statusBrush = new SolidBrush(statusColor);
            g.FillEllipse(statusBrush, statusRect);
            
            // White border around status indicator
            using var statusBorder = new Pen(Color.White, 2);
            g.DrawEllipse(statusBorder, statusRect);
        }

        private void DrawContactHeader(Graphics g, Rectangle rect, ContactInfo contact, bool isVertical)
        {
            // Contact name
            using var nameFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            
            var nameFormat = new StringFormat 
            { 
                Alignment = isVertical ? StringAlignment.Center : StringAlignment.Near, 
                LineAlignment = StringAlignment.Center 
            };
            
            g.DrawString(contact.Name, nameFont, nameBrush, 
                new Rectangle(rect.X, rect.Y, rect.Width, 18), nameFormat);
            
            // Title/Role and status
            var titleRect = new Rectangle(rect.X, rect.Y + 18, rect.Width, rect.Height - 18);
            
            using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            string titleText = !string.IsNullOrEmpty(contact.Title) ? contact.Title : GetStatusText(contact.Status);
            g.DrawString(titleText, titleFont, titleBrush, titleRect, nameFormat);
        }

        private void DrawContactDetails(Graphics g, Rectangle rect, ContactInfo contact, bool isVertical)
        {
            using var detailFont = new Font(Owner.Font.FontFamily, 8.5f, FontStyle.Regular);
            using var detailBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            using var iconBrush = new SolidBrush(Color.FromArgb(100, Color.Gray));
            
            int lineHeight = 16;
            int currentY = rect.Y;
            int iconSize = 12;
            int iconTextSpacing = 18;
            
            var alignment = isVertical ? StringAlignment.Center : StringAlignment.Near;
            var format = new StringFormat { Alignment = alignment, LineAlignment = StringAlignment.Center };
            
            // Email
            if (!string.IsNullOrEmpty(contact.Email))
            {
                var emailRect = new Rectangle(rect.X, currentY, rect.Width, lineHeight);
                
                if (!isVertical)
                {
                    var iconRect = new Rectangle(rect.X, currentY + 2, iconSize, iconSize);
                    _imagePainter.DrawSvg(g, "mail", iconRect, Color.FromArgb(100, Color.Gray), 0.8f);
                    emailRect.X += iconTextSpacing;
                    emailRect.Width -= iconTextSpacing;
                }
                
                g.DrawString(contact.Email, detailFont, detailBrush, emailRect, format);
                currentY += lineHeight;
            }
            
            // Phone
            if (!string.IsNullOrEmpty(contact.Phone))
            {
                var phoneRect = new Rectangle(rect.X, currentY, rect.Width, lineHeight);
                
                if (!isVertical)
                {
                    var iconRect = new Rectangle(rect.X, currentY + 2, iconSize, iconSize);
                    _imagePainter.DrawSvg(g, "phone", iconRect, Color.FromArgb(100, Color.Gray), 0.8f);
                    phoneRect.X += iconTextSpacing;
                    phoneRect.Width -= iconTextSpacing;
                }
                
                g.DrawString(contact.Phone, detailFont, detailBrush, phoneRect, format);
                currentY += lineHeight;
            }
            
            // Department/Company
            if (!string.IsNullOrEmpty(contact.Department))
            {
                var deptRect = new Rectangle(rect.X, currentY, rect.Width, lineHeight);
                
                if (!isVertical)
                {
                    var iconRect = new Rectangle(rect.X, currentY + 2, iconSize, iconSize);
                    _imagePainter.DrawSvg(g, "building", iconRect, Color.FromArgb(100, Color.Gray), 0.8f);
                    deptRect.X += iconTextSpacing;
                    deptRect.Width -= iconTextSpacing;
                }
                
                g.DrawString(contact.Department, detailFont, detailBrush, deptRect, format);
                currentY += lineHeight;
            }
            
            // Last contact time
            if (contact.LastContact.HasValue)
            {
                string lastContactText = FormatLastContact(contact.LastContact.Value);
                var contactRect = new Rectangle(rect.X, currentY, rect.Width, lineHeight);
                
                using var lastContactBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
                g.DrawString($"Last contact: {lastContactText}", detailFont, lastContactBrush, contactRect, format);
            }
        }

        private void DrawActionButtons(Graphics g, Rectangle rect, ContactInfo contact, Color accentColor, bool isVertical)
        {
            var actions = GetContactActions(contact);
            if (!actions.Any()) return;
            
            int buttonSpacing = 8;
            int buttonCount = Math.Min(actions.Count, isVertical ? 3 : 4);
            int totalButtonWidth = buttonCount * ACTION_BUTTON_SIZE + (buttonCount - 1) * buttonSpacing;
            
            int startX = isVertical ? 
                rect.X + (rect.Width - totalButtonWidth) / 2 : 
                rect.X;
            
            for (int i = 0; i < buttonCount; i++)
            {
                var action = actions[i];
                var buttonRect = new Rectangle(
                    startX + i * (ACTION_BUTTON_SIZE + buttonSpacing),
                    rect.Y,
                    ACTION_BUTTON_SIZE,
                    ACTION_BUTTON_SIZE
                );
                
                DrawActionButton(g, buttonRect, action, accentColor);
            }
            
            // Show "more" button if there are additional actions
            if (actions.Count > buttonCount)
            {
                var moreRect = new Rectangle(
                    startX + buttonCount * (ACTION_BUTTON_SIZE + buttonSpacing),
                    rect.Y,
                    ACTION_BUTTON_SIZE,
                    ACTION_BUTTON_SIZE
                );
                
                DrawMoreButton(g, moreRect, actions.Count - buttonCount, accentColor);
            }
        }

        private void DrawActionButton(Graphics g, Rectangle rect, ContactAction action, Color accentColor)
        {
            // Button background
            Color buttonColor = action.IsPrimary ? accentColor : Color.FromArgb(240, 240, 240);
            using var buttonBrush = new SolidBrush(buttonColor);
            g.FillEllipse(buttonBrush, rect);
            
            // Button border
            Color borderColor = action.IsPrimary ? accentColor : Color.FromArgb(200, 200, 200);
            using var borderPen = new Pen(borderColor, 1);
            g.DrawEllipse(borderPen, rect);
            
            // Button icon
            var iconRect = Rectangle.Inflate(rect, -6, -6);
            Color iconColor = action.IsPrimary ? Color.White : Color.FromArgb(100, Color.Black);
            _imagePainter.DrawSvg(g, action.IconName, iconRect, iconColor, 0.8f);
            
            // Hover effect (simplified)
            if (action.IsHovered)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
                g.FillEllipse(hoverBrush, rect);
            }
        }

        private void DrawMoreButton(Graphics g, Rectangle rect, int moreCount, Color accentColor)
        {
            // More button background
            using var moreBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            g.FillEllipse(moreBrush, rect);
            
            // More button border
            using var borderPen = new Pen(Color.FromArgb(180, 180, 180), 1);
            g.DrawEllipse(borderPen, rect);
            
            // More dots icon
            var iconRect = Rectangle.Inflate(rect, -6, -6);
            _imagePainter.DrawSvg(g, "more-horizontal", iconRect, Color.FromArgb(100, Color.Black), 0.8f);
            
            // More count badge
            if (moreCount > 0)
            {
                var badgeRect = new Rectangle(rect.Right - 10, rect.Y - 2, 12, 12);
                using var badgeBrush = new SolidBrush(Color.FromArgb(244, 67, 54));
                g.FillEllipse(badgeBrush, badgeRect);
                
                using var countFont = new Font(Owner.Font.FontFamily, 6f, FontStyle.Bold);
                using var countBrush = new SolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(moreCount.ToString(), countFont, countBrush, badgeRect, format);
            }
        }

        private ContactInfo CreateSampleContact()
        {
            return new ContactInfo
            {
                Name = "Sarah Johnson",
                Title = "Senior Developer",
                Email = "sarah.johnson@company.com",
                Phone = "+1 (555) 123-4567",
                Department = "Engineering",
                Status = ContactStatus.Online,
                LastContact = DateTime.Now.AddDays(-2),
                IsFavorite = true
            };
        }

        private List<ContactAction> GetContactActions(ContactInfo contact)
        {
            var actions = new List<ContactAction>();
            
            // Primary actions
            if (!string.IsNullOrEmpty(contact.Email))
                actions.Add(new ContactAction { Type = ActionType.Email, IconName = "mail", IsPrimary = true });
            
            if (!string.IsNullOrEmpty(contact.Phone))
                actions.Add(new ContactAction { Type = ActionType.Call, IconName = "phone", IsPrimary = true });
            
            // Secondary actions
            actions.Add(new ContactAction { Type = ActionType.Message, IconName = "message-circle", IsPrimary = false });
            actions.Add(new ContactAction { Type = ActionType.VideoCall, IconName = "video", IsPrimary = false });
            
            // Additional actions
            if (contact.IsFavorite)
                actions.Add(new ContactAction { Type = ActionType.Favorite, IconName = "heart-filled", IsPrimary = false });
            else
                actions.Add(new ContactAction { Type = ActionType.Favorite, IconName = "heart", IsPrimary = false });
            
            actions.Add(new ContactAction { Type = ActionType.Edit, IconName = "edit", IsPrimary = false });
            actions.Add(new ContactAction { Type = ActionType.Share, IconName = "share", IsPrimary = false });
            
            return actions;
        }

        private Color GetStatusColor(ContactStatus status)
        {
            return status switch
            {
                ContactStatus.Online => Color.FromArgb(76, 175, 80),
                ContactStatus.Away => Color.FromArgb(255, 193, 7),
                ContactStatus.Busy => Color.FromArgb(244, 67, 54),
                ContactStatus.Offline => Color.FromArgb(158, 158, 158),
                _ => Color.FromArgb(158, 158, 158)
            };
        }

        private string GetStatusText(ContactStatus status)
        {
            return status switch
            {
                ContactStatus.Online => "Online",
                ContactStatus.Away => "Away",
                ContactStatus.Busy => "Busy",
                ContactStatus.Offline => "Offline",
                _ => "Unknown"
            };
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        }

        private string FormatLastContact(DateTime lastContact)
        {
            var timeSpan = DateTime.Now - lastContact;
            
            if (timeSpan.TotalDays < 1) return "Today";
            if (timeSpan.TotalDays < 2) return "Yesterday";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";
            return lastContact.ToString("MMM dd, yyyy");
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            var contact = ctx.ContactInfo ?? new ContactInfo();
            
            // Draw favorite star if contact is favorited
            if (contact.IsFavorite)
            {
                var starRect = new Rectangle(ctx.DrawingRect.Right - 20, ctx.DrawingRect.Top + 8, 12, 12);
                _imagePainter.DrawSvg(g, "star-filled", starRect, Color.FromArgb(255, 193, 7), 0.9f);
            }
            
            // Draw new contact indicator
            if (contact.IsNewContact)
            {
                var newRect = new Rectangle(ctx.DrawingRect.Right - 32, ctx.DrawingRect.Top + 8, 24, 12);
                using var newBrush = new SolidBrush(Color.FromArgb(100, Color.Blue));
                using var newPath = CreateRoundedPath(newRect, 6);
                g.FillPath(newBrush, newPath);

                using var newFont = new Font(Owner.Font.FontFamily, 6f, FontStyle.Bold);
                using var newTextBrush = new SolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("NEW", newFont, newTextBrush, newRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    public class ContactAction
    {
        public ActionType Type { get; set; }
        public string IconName { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsHovered { get; set; }
    }

    public enum ActionType
    {
        Email,
        Call,
        Message,
        VideoCall,
        Favorite,
        Edit,
        Share,
        Delete
    }
}
