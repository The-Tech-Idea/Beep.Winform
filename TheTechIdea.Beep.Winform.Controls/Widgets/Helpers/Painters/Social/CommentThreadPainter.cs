using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using Comment = TheTechIdea.Beep.Winform.Controls.Widgets.Models.Comment;
using VoteType = TheTechIdea.Beep.Winform.Controls.Widgets.Models.VoteType;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// CommentThread - Full-featured comment/reply thread painter
    /// Displays hierarchical comments with voting, timestamps, and reply functionality
    /// </summary>
    internal sealed class CommentThreadPainter : WidgetPainterBase, IDisposable
    {
        private Font? _titleFont;
        private Font? _bodyFont;
        private Font? _smallFont;
        private Font? _tinyFont;
        private Font? _boldSmallFont;
        private bool _wheelHooked;
        private WidgetContext? _lastCtx;

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose(); _bodyFont?.Dispose(); _smallFont?.Dispose();
            _tinyFont?.Dispose(); _boldSmallFont?.Dispose();
            _titleFont     = BeepThemesManager.ToFont(Theme?.LabelMedium  ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, true);
            _bodyFont      = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 9f }, true);
            _smallFont     = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f }, true);
            _tinyFont      = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 7f }, true);
            _boldSmallFont = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 9f, FontWeight = FontWeight.Bold }, true);
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null)
            {
                Owner.MouseWheel += OnMouseWheel;
                _wheelHooked = true;
            }
        }

        private void OnMouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            _lastCtx.ScrollOffsetY -= e.Delta / 3;
            ClampScrollOffset(_lastCtx);
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(12);
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Thread header with title and stats
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                Dp(32)
            );

            // Comments area (scrollable)
            int contentTop = ctx.HeaderRect.Bottom + Dp(8);
            int inputHeight = Dp(40);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad - inputHeight - Dp(8)
            );

            // Add comment area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - inputHeight,
                ctx.DrawingRect.Width - pad * 2,
                inputHeight
            );

            // Estimate total content height for scrolling
            var comments = ctx.Comments ?? CreateSampleComments();
            int estimatedHeight = comments.Count * Dp(80);
            foreach (var c in comments)
                estimatedHeight += (c.Replies?.Count ?? 0) * Dp(70);
            ctx.TotalContentHeight = estimatedHeight;
            ClampScrollOffset(ctx);
            _lastCtx = ctx;

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            var bgBrush = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.Empty);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw thread header
            DrawThreadHeader(g, ctx.HeaderRect, ctx);

            // Draw comments
            var comments = ctx.Comments;
            if (comments == null || comments.Count == 0)
            {
                comments = CreateSampleComments();
            }
            
            DrawComments(g, ctx.ContentRect, comments, ctx);

            // Draw add comment area
            DrawAddCommentArea(g, ctx.FooterRect, ctx);
        }

        private void DrawThreadHeader(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Comments icon
            var iconRect = new Rectangle(rect.X, rect.Y + Dp(4), Dp(24), Dp(24));
            using var iconPath = CreateRoundedPath(iconRect, 0);
            StyledImagePainter.PaintWithTint(g, iconPath, SvgsUI.MessageCircle, ctx.AccentColor, 0.9f);

            // Thread title
            var titleRect = new Rectangle(iconRect.Right + Dp(8), rect.Y, rect.Width - iconRect.Width - Dp(100), Dp(20));
            string title = ctx.Title ?? "Comments";

            var titleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(200, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(title, _titleFont ?? SystemFonts.DefaultFont, titleBrush, titleRect,
                new StringFormat { LineAlignment = StringAlignment.Center });

            // Comment count and stats
            var comments = ctx.Comments;

            int totalComments = CountTotalComments(comments);
            var statsRect = new Rectangle(rect.X, rect.Y + Dp(20), rect.Width, Dp(12));

            var statsBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));

            string statsText = totalComments == 0 ? "No comments" :
                totalComments == 1 ? "1 comment" : $"{totalComments} comments";

            g.DrawString(statsText, _bodyFont ?? SystemFonts.DefaultFont, statsBrush, statsRect);

            // Sort/filter options
            var sortRect = new Rectangle(rect.Right - Dp(80), rect.Y + Dp(4), Dp(80), Dp(16));
            var sortBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
            var sortFormat = new StringFormat { Alignment = StringAlignment.Far };
            g.DrawString("Sort: Recent", _smallFont ?? SystemFonts.DefaultFont, sortBrush, sortRect, sortFormat);
        }

        private void DrawComments(Graphics g, Rectangle rect, List<Comment> comments, WidgetContext ctx)
        {
            if (!comments.Any())
            {
                DrawEmptyComments(g, rect);
                return;
            }

            var savedClip = g.Clip;
            g.SetClip(rect);

            int currentY = rect.Y - ctx.ScrollOffsetY;
            int spacing = Dp(8);

            foreach (var comment in comments)
            {
                if (currentY > rect.Bottom) break;

                int commentHeight = DrawComment(g,
                    new Rectangle(rect.X, currentY, rect.Width, rect.Height),
                    comment, 0, ctx);

                currentY += commentHeight + spacing;
            }

            g.Clip = savedClip;
        }

        private int DrawComment(Graphics g, Rectangle availableRect, Comment comment, int indentLevel, WidgetContext ctx)
        {
            int indent = indentLevel * Dp(40);
            int commentWidth = availableRect.Width - indent;
            int startY = availableRect.Y;

            // Comment container
            var commentRect = new Rectangle(availableRect.X + indent, startY, commentWidth, 0);

            // Calculate comment content height
            var contentFont = _bodyFont ?? SystemFonts.DefaultFont;
            var contentSize = TextUtils.MeasureText(g, comment.Content, contentFont, commentWidth - Dp(32) - Dp(12) * 3);
            int commentHeight = Math.Max(Dp(60), (int)contentSize.Height + Dp(40));
            
            commentRect.Height = commentHeight;

            // Draw comment background (subtle for replies)
            if (indentLevel > 0)
            {
                var replyBgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(5, Theme?.SecondaryTextColor ?? Color.Empty));
                g.FillRectangle(replyBgBrush, commentRect);

                // Draw reply connection line
                var connectionPen = PaintersFactory.GetPen(Color.FromArgb(60, Theme?.SecondaryTextColor ?? Color.Empty), 1f);
                g.DrawLine(connectionPen,
                    availableRect.X + indent - Dp(20), startY,
                    availableRect.X + indent - Dp(20), startY + commentHeight);
            }

            // User avatar
            int avSize = Dp(32);
            int cPad   = Dp(12);
            var avatarRect = new Rectangle(commentRect.X + cPad, commentRect.Y + cPad, avSize, avSize);
            DrawCommentAvatar(g, avatarRect, comment.AuthorName, comment.AuthorAvatar);

            // Comment content area
            var contentRect = new Rectangle(avatarRect.Right + Dp(12), commentRect.Y + cPad,
                commentRect.Width - avatarRect.Width - cPad * 3, commentRect.Height - cPad * 2);

            // Author name and timestamp
            var authorRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, 16);
            DrawCommentAuthor(g, authorRect, comment);

            // Comment text
            var textRect = new Rectangle(contentRect.X, authorRect.Bottom + Dp(4),
                contentRect.Width, (int)contentSize.Height);

            var textBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(comment.Content, contentFont, textBrush, textRect);

            // Comment actions (vote, reply, etc.)
            var actionsRect = new Rectangle(contentRect.X, textRect.Bottom + Dp(8),
                contentRect.Width, Dp(20));
            DrawCommentActions(g, actionsRect, comment, ctx);

            int totalHeight = commentHeight;

            // Draw replies recursively
            if (comment.Replies?.Any() == true && indentLevel < 3) // Limit nesting depth
            {
                int repliesY = commentRect.Bottom + Dp(4);

                foreach (var reply in comment.Replies)
                {
                    if (repliesY >= availableRect.Bottom) break;

                    int replyHeight = DrawComment(g,
                        new Rectangle(availableRect.X, repliesY, availableRect.Width,
                        availableRect.Bottom - repliesY),
                        reply, indentLevel + 1, ctx);

                    repliesY += replyHeight + Dp(4);
                    totalHeight += replyHeight + Dp(4);
                }
            }

            return totalHeight;
        }

        private void DrawCommentAvatar(Graphics g, Rectangle rect, string authorName, string avatarPath)
        {
            // Draw avatar background
            var avatarBrush = PaintersFactory.GetSolidBrush(Theme?.CardBackColor ?? Color.Empty);
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
                catch { /* avatar unavailable, fall through to initials */ }
            }

            if (string.IsNullOrEmpty(avatarPath))
            {
                // Draw initials
                string initials = GetUserInitials(authorName);
                var initialBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, _boldSmallFont ?? SystemFonts.DefaultFont, initialBrush, rect, format);
            }
        }

        private void DrawCommentAuthor(Graphics g, Rectangle rect, Comment comment)
        {
            // Author name
            var nameBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(160, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString(comment.AuthorName, _boldSmallFont ?? SystemFonts.DefaultFont, nameBrush, rect.X, rect.Y);

            // Timestamp
            string timeText = FormatCommentTime(comment.CreatedAt);
            var timeBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            var timeSize = TextUtils.MeasureText(g, timeText, _smallFont ?? SystemFonts.DefaultFont);
            g.DrawString(timeText, _smallFont ?? SystemFonts.DefaultFont, timeBrush, rect.Right - timeSize.Width, rect.Y);

            // Author badge (if applicable)
            if (comment.IsAuthor)
            {
                var badgeRect = new Rectangle((int)(rect.Right - timeSize.Width - Dp(40)), rect.Y, Dp(35), Dp(12));
                var badgeBrush = PaintersFactory.GetSolidBrush(Theme?.BadgeBackColor ?? Theme?.AccentColor ?? Color.Empty);
                using var badgePath = CreateRoundedPath(badgeRect, 6);
                g.FillPath(badgeBrush, badgePath);

                var badgeTextBrush = PaintersFactory.GetSolidBrush(Theme?.BadgeForeColor ?? Theme?.OnPrimaryColor ?? Color.Empty);
                var badgeFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("AUTHOR", _tinyFont ?? SystemFonts.DefaultFont, badgeTextBrush, badgeRect, badgeFormat);
            }
        }

        private void DrawCommentActions(Graphics g, Rectangle rect, Comment comment, WidgetContext ctx)
        {
            int buttonSpacing = 40;
            int currentX = rect.X;

            int btnSize = Dp(20);

            // Upvote button
            var upvoteRect = new Rectangle(currentX, rect.Y, btnSize, btnSize);
            Color upvoteColor = comment.UserVote == VoteType.Upvote ? (Theme?.SuccessColor ?? Color.Empty) : PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.SecondaryTextColor ?? Color.Empty, 150);
            using (var upPath = CreateRoundedPath(upvoteRect, 0))
                StyledImagePainter.PaintWithTint(g, upPath, SvgsUI.CircleChevronUp, upvoteColor, 0.8f);

            // Vote count
            Color voteTextColor = comment.UserVote != VoteType.None ?
                (comment.UserVote == VoteType.Upvote ? (Theme?.SuccessColor ?? Color.FromArgb(76, 175, 80)) : (Theme?.ErrorColor ?? Color.FromArgb(244, 67, 54))) :
                Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty);
            var voteBrush = PaintersFactory.GetSolidBrush(voteTextColor);

            string voteText = comment.VoteScore.ToString();
            var voteSize = TextUtils.MeasureText(g, voteText, _smallFont ?? SystemFonts.DefaultFont);
            g.DrawString(voteText, _smallFont ?? SystemFonts.DefaultFont, voteBrush, currentX + Dp(22), rect.Y + Dp(2));

            // Downvote button
            var downvoteRect = new Rectangle(currentX + Dp(22) + (int)voteSize.Width + Dp(4), rect.Y, btnSize, btnSize);
            Color downvoteColor = comment.UserVote == VoteType.Downvote ? (Theme?.ErrorColor ?? Color.Empty) : PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.SecondaryTextColor ?? Color.Empty, 150);
            using (var dnPath = CreateRoundedPath(downvoteRect, 0))
                StyledImagePainter.PaintWithTint(g, dnPath, SvgsUI.ChevronDown, downvoteColor, 0.8f);

            currentX += buttonSpacing + Dp(40);

            // Reply button
            var replyRect = new Rectangle(currentX, rect.Y + Dp(2), Dp(16), Dp(16));
            using (var replyIconPath = CreateRoundedPath(replyRect, 0))
                StyledImagePainter.PaintWithTint(g, replyIconPath, SvgsUI.CircleArrowDownRight, PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.CardTextForeColor ?? Color.Empty, 120), 0.8f);

            var replyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            g.DrawString("Reply", _smallFont ?? SystemFonts.DefaultFont, replyBrush, currentX + Dp(20), rect.Y + Dp(4));

            currentX += buttonSpacing + Dp(20);

            // More actions (edit, delete for own comments)
            if (comment.CanEdit)
            {
                var moreRect = new Rectangle(currentX, rect.Y + Dp(2), Dp(16), Dp(16));
                using var morePath = CreateRoundedPath(moreRect, 0);
                StyledImagePainter.PaintWithTint(g, morePath, SvgsUI.Dots, PathPainterHelpers.WithAlphaIfNotEmpty(Theme?.CardTextForeColor ?? Color.Empty, 120), 0.8f);
            }
        }

        private void DrawAddCommentArea(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            // Input background
            var inputBgBrush = PaintersFactory.GetSolidBrush(Theme?.TextBoxBackColor ?? Color.Empty);
            using var inputPath = CreateRoundedPath(rect, Dp(8));
            g.FillPath(inputBgBrush, inputPath);

            // Input border
            var borderPen = PaintersFactory.GetPen(Theme?.BorderColor ?? Color.Empty, 1f);
            g.DrawPath(borderPen, inputPath);

            // User avatar (small)
            var avatarRect = new Rectangle(rect.X + Dp(8), rect.Y + Dp(4), Dp(24), Dp(24));
            var avatarBrush = PaintersFactory.GetSolidBrush(Theme?.CardBackColor ?? Color.Empty);
            g.FillEllipse(avatarBrush, avatarRect);

            // Placeholder text or current input
            string inputText = ctx.CommentText ?? "";

            string displayText = string.IsNullOrEmpty(inputText) ? "Add a comment..." : inputText;
            Color textColor = string.IsNullOrEmpty(inputText) ?
                Color.FromArgb(150, Theme?.SecondaryTextColor ?? Color.Empty) : Color.FromArgb(80, Theme?.CardTextForeColor ?? Color.Empty);

            var textRect = new Rectangle(avatarRect.Right + Dp(8), rect.Y + Dp(4),
                rect.Width - avatarRect.Width - Dp(80), rect.Height - Dp(8));

            var inputTextBrush = PaintersFactory.GetSolidBrush(textColor);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(displayText, _bodyFont ?? SystemFonts.DefaultFont, inputTextBrush, textRect, format);

            // Post button
            var postRect = new Rectangle(rect.Right - Dp(60), rect.Y + Dp(8), Dp(50), Dp(24));
            bool canPost = !string.IsNullOrEmpty(inputText?.Trim());

            Color postColor = canPost ? ctx.AccentColor : Color.FromArgb(200, 200, 200);
            var postBrush = PaintersFactory.GetSolidBrush(postColor);
            using var postPath = CreateRoundedPath(postRect, Dp(4));
            g.FillPath(postBrush, postPath);

            var postTextBrush = PaintersFactory.GetSolidBrush(Color.White);
            var postFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("POST", _smallFont ?? SystemFonts.DefaultFont, postTextBrush, postRect, postFormat);
        }

        private void DrawEmptyComments(Graphics g, Rectangle rect)
        {
            // Empty state illustration
            var illustrationRect = new Rectangle(
                rect.X + rect.Width / 2 - Dp(32),
                rect.Y + rect.Height / 2 - Dp(40),
                Dp(64), Dp(64)
            );
            using (var emptyIconPath = CreateRoundedPath(illustrationRect, 0))
                StyledImagePainter.PaintWithTint(g, emptyIconPath, SvgsUI.MessageCircle, Color.FromArgb(150, Color.Gray), 0.6f);

            // Empty state text
            var emptyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Black));
            var textRect = new Rectangle(rect.X, illustrationRect.Bottom + Dp(16), rect.Width, Dp(40));
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("No comments yet", _bodyFont ?? SystemFonts.DefaultFont, emptyBrush, textRect, format);

            var subBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Black));
            var subTextRect = new Rectangle(rect.X, textRect.Bottom, rect.Width, Dp(20));
            g.DrawString("Be the first to share your thoughts!", _smallFont ?? SystemFonts.DefaultFont, subBrush, subTextRect, format);
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
                var indicatorRect = new Rectangle(ctx.DrawingRect.Right - Dp(16), ctx.DrawingRect.Top + Dp(8), Dp(8), Dp(8));
                var indicatorBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(244, 67, 54));
                g.FillEllipse(indicatorBrush, indicatorRect);
            }

            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("CommentThread_Scroll"));
        }

        public void Dispose()
        {
            if (_wheelHooked && Owner != null)
            {
                Owner.MouseWheel -= OnMouseWheel;
                _wheelHooked = false;
            }
            _titleFont?.Dispose();
            _bodyFont?.Dispose();
            _smallFont?.Dispose();
            _tinyFont?.Dispose();
            _boldSmallFont?.Dispose();
        }
    }

    // Comment and VoteType are now in Widgets.Models namespace
}
