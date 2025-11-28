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
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// CommentThread - Full-featured comment/reply thread painter
    /// Displays hierarchical comments with voting, timestamps, and reply functionality
    /// </summary>
    internal sealed class CommentThreadPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private const int COMMENT_PADDING = 12;
        private const int AVATAR_SIZE = 32;
        private const int REPLY_INDENT = 40;
        private const int VOTE_BUTTON_SIZE = 20;

        public CommentThreadPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Thread header with title and stats
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad, 
                ctx.DrawingRect.Top + pad, 
                ctx.DrawingRect.Width - pad * 2, 
                32
            );
            
            // Comments area (scrollable)
            int contentTop = ctx.HeaderRect.Bottom + 8;
            int inputHeight = 40;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad - inputHeight - 8
            );
            
            // Add comment area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - inputHeight,
                ctx.DrawingRect.Width - pad * 2,
                inputHeight
            );
            
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

            // Draw thread header
            DrawThreadHeader(g, ctx.HeaderRect, ctx);

            // Draw comments
            var comments = ctx.Comments?.Cast<Comment>().ToList() ?? CreateSampleComments();
            
            DrawComments(g, ctx.ContentRect, comments, ctx);

            // Draw add comment area
            DrawAddCommentArea(g, ctx.FooterRect, ctx);
        }

        private void DrawThreadHeader(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Comments icon
            var iconRect = new Rectangle(rect.X, rect.Y + 4, 24, 24);
            _imagePainter.DrawSvg(g, "message-square", iconRect, ctx.AccentColor, 0.9f);

            // Thread title
            var titleRect = new Rectangle(iconRect.Right + 8, rect.Y, rect.Width - iconRect.Width - 100, 20);
            string title = ctx.Title ?? "Comments";
            
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(title, titleFont, titleBrush, titleRect, 
                new StringFormat { LineAlignment = StringAlignment.Center });

            // Comment count and stats
            var comments = ctx.Comments?.Cast<Comment>().ToList() ?? new List<Comment>();
            
            int totalComments = CountTotalComments(comments);
            var statsRect = new Rectangle(rect.X, rect.Y + 20, rect.Width, 12);
            
            using var statsFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statsBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            
            string statsText = totalComments == 0 ? "No comments" : 
                totalComments == 1 ? "1 comment" : $"{totalComments} comments";
            
            g.DrawString(statsText, statsFont, statsBrush, statsRect);

            // Sort/filter options
            var sortRect = new Rectangle(rect.Right - 80, rect.Y + 4, 80, 16);
            using var sortFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var sortBrush = new SolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
            var sortFormat = new StringFormat { Alignment = StringAlignment.Far };
            g.DrawString("Sort: Recent", sortFont, sortBrush, sortRect, sortFormat);
        }

        private void DrawComments(Graphics g, Rectangle rect, List<Comment> comments, WidgetContext ctx)
        {
            if (!comments.Any())
            {
                DrawEmptyComments(g, rect);
                return;
            }

            int currentY = rect.Y;
            int maxHeight = rect.Height;

            foreach (var comment in comments)
            {
                if (currentY >= rect.Bottom) break;

                int commentHeight = DrawComment(g, 
                    new Rectangle(rect.X, currentY, rect.Width, maxHeight - (currentY - rect.Y)), 
                    comment, 0, ctx);
                
                currentY += commentHeight + 8; // Comment spacing
            }
        }

        private int DrawComment(Graphics g, Rectangle availableRect, Comment comment, int indentLevel, WidgetContext ctx)
        {
            int indent = indentLevel * REPLY_INDENT;
            int commentWidth = availableRect.Width - indent;
            int startY = availableRect.Y;
            
            // Comment container
            var commentRect = new Rectangle(availableRect.X + indent, startY, commentWidth, 0);
            
            // Calculate comment content height
            using var contentFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            var contentSize = TextUtils.MeasureText(g,comment.Content, contentFont, commentWidth - AVATAR_SIZE - COMMENT_PADDING * 3);
            int commentHeight = Math.Max(60, (int)contentSize.Height + 40); // Min height for avatar + padding
            
            commentRect.Height = commentHeight;

            // Draw comment background (subtle for replies)
            if (indentLevel > 0)
            {
                using var replyBgBrush = new SolidBrush(Color.FromArgb(5, Theme?.SecondaryTextColor ?? Color.Empty));
                g.FillRectangle(replyBgBrush, commentRect);
                
                // Draw reply connection line
                using var connectionPen = new Pen(Color.FromArgb(60, Theme?.SecondaryTextColor ?? Color.Empty), 1);
                g.DrawLine(connectionPen, 
                    availableRect.X + indent - 20, startY,
                    availableRect.X + indent - 20, startY + commentHeight);
            }

            // User avatar
            var avatarRect = new Rectangle(commentRect.X + COMMENT_PADDING, 
                commentRect.Y + COMMENT_PADDING, AVATAR_SIZE, AVATAR_SIZE);
            DrawCommentAvatar(g, avatarRect, comment.AuthorName, comment.AuthorAvatar);

            // Comment content area
            var contentRect = new Rectangle(avatarRect.Right + 12, commentRect.Y + COMMENT_PADDING,
                commentRect.Width - avatarRect.Width - COMMENT_PADDING * 3, commentRect.Height - COMMENT_PADDING * 2);

            // Author name and timestamp
            var authorRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, 16);
            DrawCommentAuthor(g, authorRect, comment);

            // Comment text
            var textRect = new Rectangle(contentRect.X, authorRect.Bottom + 4, 
                contentRect.Width, (int)contentSize.Height);
            
            using var textBrush = new SolidBrush(Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(comment.Content, contentFont, textBrush, textRect);

            // Comment actions (vote, reply, etc.)
            var actionsRect = new Rectangle(contentRect.X, textRect.Bottom + 8, 
                contentRect.Width, 20);
            DrawCommentActions(g, actionsRect, comment, ctx);

            int totalHeight = commentHeight;

            // Draw replies recursively
            if (comment.Replies?.Any() == true && indentLevel < 3) // Limit nesting depth
            {
                int repliesY = commentRect.Bottom + 4;
                
                foreach (var reply in comment.Replies)
                {
                    if (repliesY >= availableRect.Bottom) break;

                    int replyHeight = DrawComment(g,
                        new Rectangle(availableRect.X, repliesY, availableRect.Width, 
                        availableRect.Bottom - repliesY),
                        reply, indentLevel + 1, ctx);
                    
                    repliesY += replyHeight + 4;
                    totalHeight += replyHeight + 4;
                }
            }

            return totalHeight;
        }

        private void DrawCommentAvatar(Graphics g, Rectangle rect, string authorName, string avatarPath)
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
                string initials = GetUserInitials(authorName);
                using var initialFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var initialBrush = new SolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialFont, initialBrush, rect, format);
            }
        }

        private void DrawCommentAuthor(Graphics g, Rectangle rect, Comment comment)
        {
            // Author name
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.FromArgb(160, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(comment.AuthorName, nameFont, nameBrush, rect.X, rect.Y);

            // Timestamp
            string timeText = FormatCommentTime(comment.CreatedAt);
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            var timeSize = TextUtils.MeasureText(g,timeText, timeFont);
            g.DrawString(timeText, timeFont, timeBrush, rect.Right - timeSize.Width, rect.Y);

            // Author badge (if applicable)
            if (comment.IsAuthor)
            {
                var badgeRect = new Rectangle((int)(rect.Right - timeSize.Width - 40), rect.Y, 35, 12);
                using var badgeBrush = new SolidBrush(Theme?.BadgeBackColor ?? Theme?.AccentColor ?? Color.Empty);
                using var badgePath = CreateRoundedPath(badgeRect, 6);
                g.FillPath(badgeBrush, badgePath);

                using var badgeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                using var badgeTextBrush = new SolidBrush(Theme?.BadgeForeColor ?? Theme?.OnPrimaryColor ?? Color.Empty);
                var badgeFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("AUTHOR", badgeFont, badgeTextBrush, badgeRect, badgeFormat);
            }
        }

        private void DrawCommentActions(Graphics g, Rectangle rect, Comment comment, WidgetContext ctx)
        {
            int buttonSpacing = 40;
            int currentX = rect.X;

            // Upvote button
                var upvoteRect = new Rectangle(currentX, rect.Y, VOTE_BUTTON_SIZE, VOTE_BUTTON_SIZE);
                Color upvoteColor = comment.UserVote == VoteType.Upvote ? (Theme?.SuccessColor ?? Color.Empty) : PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.SecondaryTextColor ?? Color.Empty, 150);
                _imagePainter.DrawSvg(g, "chevron-up", upvoteRect, upvoteColor, 0.8f);

            // Vote count
            using var voteFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            Color voteTextColor = comment.UserVote != VoteType.None ? 
                (comment.UserVote == VoteType.Upvote ? (Theme?.SuccessColor ?? Color.FromArgb(76, 175, 80)) : (Theme?.ErrorColor ?? Color.FromArgb(244, 67, 54))) : 
                Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty);
            using var voteBrush = new SolidBrush(voteTextColor);
            
            string voteText = comment.VoteScore.ToString();
            var voteSize = TextUtils.MeasureText(g,voteText, voteFont);
            g.DrawString(voteText, voteFont, voteBrush, currentX + 22, rect.Y + 2);

            // Downvote button
                var downvoteRect = new Rectangle(currentX + 22 + (int)voteSize.Width + 4, rect.Y, VOTE_BUTTON_SIZE, VOTE_BUTTON_SIZE);
                Color downvoteColor = comment.UserVote == VoteType.Downvote ? (Theme?.ErrorColor ?? Color.Empty) : PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.SecondaryTextColor ?? Color.Empty, 150);
                _imagePainter.DrawSvg(g, "chevron-down", downvoteRect, downvoteColor, 0.8f);

            currentX += buttonSpacing + 40;

            // Reply button
            var replyRect = new Rectangle(currentX, rect.Y + 2, 16, 16);
            _imagePainter.DrawSvg(g, "corner-down-right", replyRect, PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.CardTextForeColor ?? Color.Empty, 120), 0.8f);
            
            using var replyFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var replyBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString("Reply", replyFont, replyBrush, currentX + 20, rect.Y + 4);

            currentX += buttonSpacing + 20;

            // More actions (edit, delete for own comments)
            if (comment.CanEdit)
            {
                var moreRect = new Rectangle(currentX, rect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "more-horizontal", moreRect, PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.CardTextForeColor ?? Color.Empty, 120), 0.8f);
            }
        }

        private void DrawAddCommentArea(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Input background
            using var inputBgBrush = new SolidBrush(Theme?.TextBoxBackColor ?? Color.Empty);
            using var inputPath = CreateRoundedPath(rect, 8);
            g.FillPath(inputBgBrush, inputPath);

            // Input border
            using var borderPen = new Pen(Theme?.BorderColor ?? Color.Empty, 1);
            g.DrawPath(borderPen, inputPath);

            // User avatar (small)
            var avatarRect = new Rectangle(rect.X + 8, rect.Y + 4, 24, 24);
            using var avatarBrush = new SolidBrush(Theme?.CardBackColor ?? Color.Empty);
            g.FillEllipse(avatarBrush, avatarRect);

            // Placeholder text or current input
            string inputText = ctx.CommentText ?? "";
            
            string displayText = string.IsNullOrEmpty(inputText) ? "Add a comment..." : inputText;
            Color textColor = string.IsNullOrEmpty(inputText) ? 
                Color.FromArgb(150, Theme?.SecondaryTextColor ?? Color.Empty) : Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty);

            var textRect = new Rectangle(avatarRect.Right + 8, rect.Y + 4, 
                rect.Width - avatarRect.Width - 80, rect.Height - 8);

            using var inputFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(textColor);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(displayText, inputFont, textBrush, textRect, format);

            // Post button
            var postRect = new Rectangle(rect.Right - 60, rect.Y + 8, 50, 24);
            bool canPost = !string.IsNullOrEmpty(inputText?.Trim());
            
            Color postColor = canPost ? ctx.AccentColor : Color.FromArgb(200, 200, 200);
            using var postBrush = new SolidBrush(postColor);
            using var postPath = CreateRoundedPath(postRect, 4);
            g.FillPath(postBrush, postPath);

            using var postFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var postTextBrush = new SolidBrush(Color.White);
            var postFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("POST", postFont, postTextBrush, postRect, postFormat);
        }

        private void DrawEmptyComments(Graphics g, Rectangle rect)
        {
            // Empty state illustration
            var illustrationRect = new Rectangle(
                rect.X + rect.Width / 2 - 32, 
                rect.Y + rect.Height / 2 - 40, 
                64, 64
            );
            _imagePainter.DrawSvg(g, "message-square", illustrationRect, Color.FromArgb(150, Color.Gray), 0.6f);

            // Empty state text
            using var emptyFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var emptyBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var textRect = new Rectangle(rect.X, illustrationRect.Bottom + 16, rect.Width, 40);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("No comments yet", emptyFont, emptyBrush, textRect, format);

            using var subFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var subBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            var subTextRect = new Rectangle(rect.X, textRect.Bottom, rect.Width, 20);
            g.DrawString("Be the first to share your thoughts!", subFont, subBrush, subTextRect, format);
        }

        private List<Comment> CreateSampleComments()
        {
            return new List<Comment>
            {
                new Comment 
                { 
                    Id = "1",
                    AuthorName = "Sarah Wilson", 
                    Content = "This is a really interesting implementation! I love how clean the design is.",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    VoteScore = 5,
                    UserVote = VoteType.None,
                    Replies = new List<Comment>
                    {
                        new Comment
                        {
                            Id = "2",
                            AuthorName = "John Doe",
                            Content = "Thanks! I spent a lot of time on the UI/UX details.",
                            CreatedAt = DateTime.Now.AddHours(-1),
                            VoteScore = 2,
                            UserVote = VoteType.Upvote,
                            IsAuthor = true
                        }
                    }
                },
                new Comment 
                { 
                    Id = "3",
                    AuthorName = "Mike Johnson", 
                    Content = "Have you considered adding dark mode support? It would be a great addition.",
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    VoteScore = 3,
                    UserVote = VoteType.None,
                    CanEdit = false
                }
            };
        }

        private int CountTotalComments(List<Comment> comments)
        {
            int count = comments.Count;
            foreach (var comment in comments)
            {
                if (comment.Replies?.Any() == true)
                {
                    count += CountTotalComments(comment.Replies);
                }
            }
            return count;
        }

        private string FormatCommentTime(DateTime timestamp)
        {
            var timeSpan = DateTime.Now - timestamp;
            
            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
            return timestamp.ToString("MMM dd, yyyy");
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
            // Draw new comment indicator if there are unread comments
            if (ctx.HasNewComments)
            {
                var indicatorRect = new Rectangle(ctx.DrawingRect.Right - 16, ctx.DrawingRect.Top + 8, 8, 8);
                using var indicatorBrush = new SolidBrush(Color.FromArgb(244, 67, 54));
                g.FillEllipse(indicatorBrush, indicatorRect);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    // Supporting classes for comment data
    public class Comment
    {
        public string Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public int VoteScore { get; set; }
        public VoteType UserVote { get; set; }
        public List<Comment> Replies { get; set; } = new List<Comment>();
        public bool IsAuthor { get; set; }
        public bool CanEdit { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum VoteType
    {
        None,
        Upvote,
        Downvote
    }
}