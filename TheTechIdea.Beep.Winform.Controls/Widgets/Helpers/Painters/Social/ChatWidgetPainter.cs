using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// ChatWidget - Full-featured chat interface painter
    /// Displays chat messages, user avatars, typing indicators, and input area
    /// </summary>
    internal sealed class ChatWidgetPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private const int MESSAGE_PADDING = 8;
        private const int AVATAR_SIZE = 32;
        private const int MESSAGE_BUBBLE_RADIUS = 12;

        // Interactive caches
        private readonly List<(Rectangle rect, int index, bool mine)> _messageRects = new();
        private Rectangle _headerIconRect;
        private Rectangle _inputRectCache;
        private Rectangle _sendRectCache;
        private Rectangle _messagesRectCache;

        public ChatWidgetPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Chat header with participant info
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad, 
                ctx.DrawingRect.Top + pad, 
                ctx.DrawingRect.Width - pad * 2, 
                40 // Increased for avatar and status
            );
            
            // Chat messages area (scrollable)
            int contentTop = ctx.HeaderRect.Bottom + 8;
            int inputHeight = 36;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad - inputHeight - 8
            );
            
            // Chat input area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - inputHeight,
                ctx.DrawingRect.Width - pad * 2,
                inputHeight
            );

            // Cache interactivity rects
            _headerIconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 4, AVATAR_SIZE, AVATAR_SIZE);
            _messagesRectCache = ctx.ContentRect;
            _inputRectCache = ctx.FooterRect;
            _sendRectCache = new Rectangle(ctx.FooterRect.Right - 32, ctx.FooterRect.Y + 4, 28, 28);
            _messageRects.Clear();
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.Empty);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            // Draw chat header
            DrawChatHeader(g, ctx.HeaderRect, ctx);

            // Draw messages area
            var messages = ctx.ChatMessages?.Cast<ChatMessage>().ToList() ?? CreateSampleMessages();
            
            DrawMessages(g, ctx.ContentRect, messages, ctx);

            // Draw typing indicator if someone is typing
            bool someoneTyping = ctx.IsTyping;
            if (someoneTyping)
            {
                DrawTypingIndicator(g, ctx.ContentRect, ctx);
            }

            // Draw input area
            DrawInputArea(g, ctx.FooterRect, ctx);
        }

        private void DrawChatHeader(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            string chatTitle = ctx.Title ?? "Chat";
            var participants = ctx.ChatParticipants?.Cast<ChatParticipant>().ToList() ?? new List<ChatParticipant>();
            
            // Chat icon or participant avatar
            var iconRect = _headerIconRect;
            
            if (participants.Count == 1)
            {
                // Single participant - show their avatar
                DrawParticipantAvatar(g, iconRect, participants[0]);
            }
            else
            {
                // Group chat - show chat icon
                _imagePainter.DrawSvg(g, "users", iconRect, ctx.AccentColor, 0.9f);
            }

            // Chat title and participant info
            var titleRect = new Rectangle(iconRect.Right + 12, rect.Y, 
                rect.Width - iconRect.Width - 12, 20);
            
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(chatTitle, titleFont, titleBrush, titleRect, 
                new StringFormat { LineAlignment = StringAlignment.Center });

            // Participant status or count
            var statusRect = new Rectangle(iconRect.Right + 12, rect.Y + 20, 
                rect.Width - iconRect.Width - 12, 16);
            
            string statusText = GetChatStatusText(participants);
            using var statusFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(statusText, statusFont, statusBrush, statusRect);

            // Online indicator for active participants
            if (participants.Any(p => p.IsOnline))
            {
                var onlineRect = new Rectangle(rect.Right - 12, rect.Y + 4, 8, 8);
                using var onlineBrush = new SolidBrush(Color.FromArgb(76, 175, 80));
                g.FillEllipse(onlineBrush, onlineRect);
            }
        }

        private void DrawMessages(Graphics g, Rectangle rect, List<ChatMessage> messages, WidgetContext ctx)
        {
            if (!messages.Any())
            {
                DrawEmptyChat(g, rect);
                return;
            }

            int currentY = rect.Y;
            string currentUserId = ctx.CurrentUserId ?? "current_user";

            // Show most recent messages (scroll to bottom behavior)
            var visibleMessages = messages.TakeLast(10).ToList();
            int baseIndex = messages.Count - visibleMessages.Count;
            _messageRects.Clear();

            for (int i = 0; i < visibleMessages.Count; i++)
            {
                var message = visibleMessages[i];
                if (currentY >= rect.Bottom) break;

                Rectangle bubbleRect;
                int messageHeight = DrawMessage(g, new Rectangle(rect.X, currentY, rect.Width, rect.Bottom - currentY), 
                    message, currentUserId == message.SenderId, ctx, out bubbleRect);
                
                _messageRects.Add((bubbleRect, baseIndex + i, currentUserId == message.SenderId));
                currentY += messageHeight + 8; // Message spacing
            }

            // Draw "scroll up for more" indicator if there are more messages
            if (messages.Count > visibleMessages.Count)
            {
                var moreRect = new Rectangle(rect.X, rect.Y, rect.Width, 20);
                using var moreBrush = new SolidBrush(Color.FromArgb(40, Theme?.SecondaryTextColor ?? Color.Empty));
                g.FillRectangle(moreBrush, moreRect);

                using var moreFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Italic);
                using var moreTextBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"{messages.Count - visibleMessages.Count} earlier messages", 
                    moreFont, moreTextBrush, moreRect, format);
            }
        }

        private int DrawMessage(Graphics g, Rectangle availableRect, ChatMessage message, bool isCurrentUser, WidgetContext ctx, out Rectangle bubbleRectOut)
        {
            int messageWidth = (int)(availableRect.Width * 0.75); // Max 75% width
            int minHeight = 40;

            // Calculate message bubble dimensions
            using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            var textSize = TextUtils.MeasureText(g,message.Content, messageFont, messageWidth - MESSAGE_PADDING * 2);
            int bubbleHeight = Math.Max(minHeight, (int)textSize.Height + MESSAGE_PADDING * 2);

            Rectangle bubbleRect;
            Rectangle avatarRect;
            
            if (isCurrentUser)
            {
                // Current user messages - right aligned, no avatar
                bubbleRect = new Rectangle(
                    availableRect.Right - messageWidth - 8,
                    availableRect.Y,
                    messageWidth,
                    bubbleHeight
                );
                avatarRect = Rectangle.Empty;
            }
            else
            {
                // Other user messages - left aligned with avatar
                avatarRect = new Rectangle(availableRect.X, availableRect.Y, AVATAR_SIZE, AVATAR_SIZE);
                bubbleRect = new Rectangle(
                    avatarRect.Right + 8,
                    availableRect.Y,
                    messageWidth - AVATAR_SIZE - 8,
                    bubbleHeight
                );
            }

            // Draw avatar for other users
            if (!avatarRect.IsEmpty)
            {
                DrawMessageAvatar(g, avatarRect, message.SenderName, message.SenderAvatar);
            }

            // Draw message bubble
            Color bubbleColor = isCurrentUser ? (ctx.AccentColor == Color.Empty ? Theme?.AccentColor ?? Color.Empty : ctx.AccentColor) : Theme?.CardBackColor ?? Color.Empty;
            
            using var bubbleBrush = new SolidBrush(bubbleColor);
            using var bubblePath = CreateRoundedPath(bubbleRect, MESSAGE_BUBBLE_RADIUS);
            g.FillPath(bubbleBrush, bubblePath);

            // Hover outline
            if (IsAreaHovered($"Chat_Message_{message.Timestamp.Ticks}"))
            {
                using var hoverPen = new Pen(Theme?.AccentColor ?? Color.Empty, 1.2f);
                g.DrawPath(hoverPen, bubblePath);
            }

            // Draw message content
            Color textColor = isCurrentUser ? Theme?.OnPrimaryColor ?? Color.Empty : Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty);
            using var textBrush = new SolidBrush(textColor);
            var textRect = Rectangle.Inflate(bubbleRect, -MESSAGE_PADDING, -MESSAGE_PADDING);
            g.DrawString(message.Content, messageFont, textBrush, textRect);

            // Draw timestamp
            string timeText = FormatMessageTime(message.Timestamp);
            using var timeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
            var timeSize = TextUtils.MeasureText(g,timeText, timeFont);
            
            float timeX = isCurrentUser ? 
                bubbleRect.X - timeSize.Width - 4 : 
                bubbleRect.Right + 4;
            
            g.DrawString(timeText, timeFont, timeBrush, timeX, bubbleRect.Bottom - 12);

            // Draw message status for current user messages
            if (isCurrentUser)
            {
                DrawMessageStatus(g, new Rectangle((int)(timeX - 16), bubbleRect.Bottom - 12, 12, 8), message.Status);
            }

            bubbleRectOut = bubbleRect;
            return bubbleHeight;
        }

        private void DrawMessageAvatar(Graphics g, Rectangle rect, string senderName, string avatarPath)
        {
            // Draw avatar background
            using var avatarBrush = new SolidBrush(Theme?.CardBackColor ?? Color.Empty);
            g.FillEllipse(avatarBrush, rect);

            if (!string.IsNullOrEmpty(avatarPath))
            {
                try
                {
                    float radius = Math.Min(rect.Width, rect.Height) / 2f;
                    float cx = rect.X + rect.Width / 2f;
                    float cy = rect.Y + rect.Height / 2f;
                    StyledImagePainter.PaintInCircle(g, cx, cy, radius, avatarPath);
                }
                catch
                {
                    _imagePainter.ImagePath = avatarPath;
                    _imagePainter.ClipShape = Vis.Modules.ImageClipShape.Circle;
                    _imagePainter.DrawImage(g, avatarPath, rect);
                }
            }
            else
            {
                // Draw initials
                string initials = GetUserInitials(senderName);
                using var initialFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var initialBrush = new SolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialFont, initialBrush, rect, format);
            }
        }

        private void DrawMessageStatus(Graphics g, Rectangle rect, MessageStatus status)
        {
            Color statusColor = status switch
            {
                MessageStatus.Sending => Color.FromArgb(150, Color.Gray),
                MessageStatus.Sent => Color.FromArgb(100, Color.Gray),
                MessageStatus.Delivered => Theme?.SuccessColor ?? Color.Empty,
                MessageStatus.Read => Theme?.PrimaryColor ?? Color.Empty,
                MessageStatus.Failed => Theme?.ErrorColor ?? Color.Empty,
                _ => Color.FromArgb(150, Color.Gray)
            };

            string statusIcon = status switch
            {
                MessageStatus.Sending => "clock",
                MessageStatus.Sent => "check",
                MessageStatus.Delivered => "check-double",
                MessageStatus.Read => "check-double",
                MessageStatus.Failed => "x",
                _ => "clock"
            };

            _imagePainter.DrawSvg(g, statusIcon, rect, statusColor, 0.8f);
        }

        private void DrawTypingIndicator(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Draw typing indicator at bottom of messages area
            var typingRect = new Rectangle(rect.X, rect.Bottom - 30, rect.Width, 25);
            
            // Avatar for typing user
            var avatarRect = new Rectangle(typingRect.X, typingRect.Y, 24, 24);
            using var avatarBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            g.FillEllipse(avatarBrush, avatarRect);

            // Typing dots animation
            var dotsRect = new Rectangle(avatarRect.Right + 8, typingRect.Y, 40, 24);
            using var dotsBrush = new SolidBrush(Color.FromArgb(100, Color.Gray));
            using var dotsPath = CreateRoundedPath(dotsRect, 12);
            g.FillPath(dotsBrush, dotsPath);

            // Animated dots (simplified - just show three dots)
            using var dotBrush = new SolidBrush(Color.FromArgb(150, Color.Gray));
            for (int i = 0; i < 3; i++)
            {
                var dotRect = new Rectangle(dotsRect.X + 8 + i * 8, dotsRect.Y + 10, 4, 4);
                g.FillEllipse(dotBrush, dotRect);
            }

            // "typing..." text
            using var typingFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Italic);
            using var typingBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            g.DrawString("typing...", typingFont, typingBrush, dotsRect.Right + 8, typingRect.Y + 8);
        }

        private void DrawInputArea(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Input background
            using var inputBgBrush = new SolidBrush(Theme?.TextBoxBackColor ?? Color.Empty);
            using var inputPath = CreateRoundedPath(rect, 18);
            g.FillPath(inputBgBrush, inputPath);

            // Input border
            using var borderPen = new Pen(Theme?.BorderColor ?? Color.Empty, 1);
            g.DrawPath(borderPen, inputPath);

            // Placeholder text or current input
            string inputText = ctx.InputText ?? "";
            
            string displayText = string.IsNullOrEmpty(inputText) ? "Type a message..." : inputText;
            Color textColor = string.IsNullOrEmpty(inputText) ? 
                Color.FromArgb(150, Theme?.SecondaryTextColor ?? Color.Empty) : Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty);

            var textRect = Rectangle.Inflate(rect, -12, 0);
            textRect.Width -= 40; // Space for send button

            using var inputFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(textColor);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(displayText, inputFont, textBrush, textRect, format);

            // Send button
            var sendRect = _sendRectCache;
            bool canSend = !string.IsNullOrEmpty(inputText?.Trim());
            
            Color sendColor = canSend ? ctx.AccentColor : Color.FromArgb(200, 200, 200);
            using var sendBrush = new SolidBrush(sendColor);
            g.FillEllipse(sendBrush, sendRect);

            // Hover outline
            if (IsAreaHovered("Chat_Send"))
            {
                using var pen = new Pen(Theme?.PrimaryColor ?? Color.Blue, 1.2f);
                g.DrawEllipse(pen, sendRect);
            }

            // Send icon
            var sendIconRect = Rectangle.Inflate(sendRect, -6, -6);
            _imagePainter.DrawSvg(g, "send", sendIconRect, Color.White, 0.8f);
        }

        private void DrawParticipantAvatar(Graphics g, Rectangle rect, ChatParticipant participant)
        {
            DrawMessageAvatar(g, rect, participant.Name, participant.Avatar);

            // Online status indicator
            if (participant.IsOnline)
            {
                var statusRect = new Rectangle(rect.Right - 8, rect.Bottom - 8, 8, 8);
                using var statusBrush = new SolidBrush(Color.FromArgb(76, 175, 80));
                g.FillEllipse(statusBrush, statusRect);
                using var statusBorder = new Pen(Color.White, 2);
                g.DrawEllipse(statusBorder, statusRect);
            }
        }

        private void DrawEmptyChat(Graphics g, Rectangle rect)
        {
            // Empty state illustration
            var illustrationRect = new Rectangle(
                rect.X + rect.Width / 2 - 32, 
                rect.Y + rect.Height / 2 - 40, 
                64, 64
            );
            _imagePainter.DrawSvg(g, "message-circle", illustrationRect, Color.FromArgb(150, Color.Gray), 0.6f);

            // Empty state text
            using var emptyFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var emptyBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var textRect = new Rectangle(rect.X, illustrationRect.Bottom + 16, rect.Width, 40);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("No messages yet", emptyFont, emptyBrush, textRect, format);

            using var subFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var subBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            var subTextRect = new Rectangle(rect.X, textRect.Bottom, rect.Width, 20);
            g.DrawString("Start the conversation!", subFont, subBrush, subTextRect, format);
        }

        private List<ChatMessage> CreateSampleMessages()
        {
            return new List<ChatMessage>
            {
                new ChatMessage 
                { 
                    SenderId = "user_1", 
                    SenderName = "Alice", 
                    Content = "Hey! How's the project going?",
                    Timestamp = DateTime.Now.AddMinutes(-15),
                    Status = MessageStatus.Read
                },
                new ChatMessage 
                { 
                    SenderId = "current_user", 
                    SenderName = "Me", 
                    Content = "Great! Just finished the dashboard implementation.",
                    Timestamp = DateTime.Now.AddMinutes(-10),
                    Status = MessageStatus.Delivered
                },
                new ChatMessage 
                { 
                    SenderId = "user_1", 
                    SenderName = "Alice", 
                    Content = "Awesome! Can't wait to see it in action. When can we review it together?",
                    Timestamp = DateTime.Now.AddMinutes(-5),
                    Status = MessageStatus.Read
                }
            };
        }

        private string GetChatStatusText(List<ChatParticipant> participants)
        {
            if (!participants.Any()) return "No participants";
            if (participants.Count == 1)
            {
                var participant = participants[0];
                return participant.IsOnline ? "Online" : $"Last seen {FormatLastSeen(participant.LastSeen)}";
            }
            
            int onlineCount = participants.Count(p => p.IsOnline);
            return onlineCount > 0 ? $"{onlineCount} online" : $"{participants.Count} participants";
        }

        private string FormatMessageTime(DateTime timestamp)
        {
            var timeSpan = DateTime.Now - timestamp;
            
            if (timeSpan.TotalMinutes < 1) return "now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m";
            if (timeSpan.TotalHours < 24) return timestamp.ToString("HH:mm");
            return timestamp.ToString("MMM dd");
        }

        private string FormatLastSeen(DateTime lastSeen)
        {
            var timeSpan = DateTime.Now - lastSeen;
            
            if (timeSpan.TotalMinutes < 5) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} min ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
            return lastSeen.ToString("MMM dd");
        }

        private string GetUserInitials(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return "?";
            
            var parts = userName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return userName.Substring(0, Math.Min(2, userName.Length)).ToUpper();
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw unread message count if any
            int unreadCount = ctx.UnreadCount;
            if (unreadCount > 0)
            {
                var badgeRect = new Rectangle(ctx.DrawingRect.Right - 24, ctx.DrawingRect.Top + 4, 16, 16);
                
                using var badgeBrush = new SolidBrush(Color.FromArgb(244, 67, 54));
                g.FillEllipse(badgeBrush, badgeRect);

                using var countFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                using var countBrush = new SolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                string countText = unreadCount > 99 ? "99+" : unreadCount.ToString();
                g.DrawString(countText, countFont, countBrush, badgeRect, format);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Header avatar/icon
            if (!_headerIconRect.IsEmpty)
            {
                owner.AddHitArea("Chat_HeaderIcon", _headerIconRect, null, () =>
                {
                    ctx.HeaderIconClicked = true;
                    notifyAreaHit?.Invoke("Chat_HeaderIcon", _headerIconRect);
                    Owner?.Invalidate();
                });
            }

            // Messages
            foreach (var (rect, index, mine) in _messageRects)
            {
                string name = $"Chat_Message_{index}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.SelectedMessageIndex = index;
                    ctx.SelectedMessageIsMine = mine;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }

            // Input
            if (!_inputRectCache.IsEmpty)
            {
                owner.AddHitArea("Chat_Input", _inputRectCache, null, () =>
                {
                    ctx.InputFocused = true;
                    notifyAreaHit?.Invoke("Chat_Input", _inputRectCache);
                    Owner?.Invalidate();
                });
            }

            // Send
            if (!_sendRectCache.IsEmpty)
            {
                string inputText = ctx.InputText ?? string.Empty;
                bool canSend = !string.IsNullOrWhiteSpace(inputText);
                owner.AddHitArea("Chat_Send", _sendRectCache, null, () =>
                {
                    if (canSend)
                    {
                        ctx.SendClicked = true;
                        notifyAreaHit?.Invoke("Chat_Send", _sendRectCache);
                    }
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    // Supporting classes for chat data
    public class ChatMessage
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
    }

    public class ChatParticipant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
    }

    public enum MessageStatus
    {
        Sending,
        Sent,
        Delivered,
        Read,
        Failed
    }

    public enum MessageType
    {
        Text,
        Image,
        File,
        System
    }
}