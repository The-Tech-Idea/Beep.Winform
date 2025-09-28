using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepSocialWidget with all social styles
    /// </summary>
    public static class BeepSocialWidgetSamples
    {
        /// <summary>
        /// Creates a profile card social widget
        /// Uses SocialProfileCardPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateProfileCardWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.ProfileCard,
                Title = "User Profile",
                UserName = "Sarah Johnson",
                UserRole = "Senior Developer",
                UserStatus = "Online",
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(280, 180),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a team members social widget
        /// Uses TeamMembersPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateTeamMembersWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.TeamMembers,
                Title = "Team Members",
                Subtitle = "Development Team",
                OnlineCount = 3,
                TotalCount = 5,
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(320, 200),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a message card social widget
        /// Uses MessageCardPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateMessageCardWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.MessageCard,
                Title = "New Message",
                UserName = "Mike Chen",
                UserRole = "Project Manager",
                UserStatus = "Online",
                ShowAvatar = true,
                Size = new Size(300, 150),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates an activity stream social widget
        /// Uses ActivityStreamPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateActivityStreamWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.ActivityStream,
                Title = "Recent Activity",
                Subtitle = "Team Updates",
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(350, 250),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates a user list social widget
        /// Uses UserListPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateUserListWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.UserList,
                Title = "Team Directory",
                Subtitle = "All Members",
                OnlineCount = 4,
                TotalCount = 8,
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(300, 280),
                AccentColor = Color.FromArgb(244, 67, 54)
            };
        }

        /// <summary>
        /// Creates a chat widget social widget
        /// Uses ChatWidgetPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateChatWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.ChatWidget,
                Title = "Team Chat",
                Subtitle = "General Discussion",
                OnlineCount = 5,
                TotalCount = 12,
                ShowAvatar = true,
                Size = new Size(320, 200),
                AccentColor = Color.FromArgb(255, 152, 0)
            };
        }

        /// <summary>
        /// Creates a comment thread social widget
        /// Uses CommentThreadPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateCommentThreadWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.CommentThread,
                Title = "Comments",
                Subtitle = "Discussion Thread",
                ShowAvatar = true,
                Size = new Size(350, 220),
                AccentColor = Color.FromArgb(103, 58, 183)
            };
        }

        /// <summary>
        /// Creates a social feed widget
        /// Uses SocialFeedPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateSocialFeedWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.SocialFeed,
                Title = "Company Feed",
                Subtitle = "Latest Updates",
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(380, 300),
                AccentColor = Color.FromArgb(0, 150, 136)
            };
        }

        /// <summary>
        /// Creates a user stats social widget
        /// Uses UserStatsPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateUserStatsWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.UserStats,
                Title = "User Statistics",
                UserName = "Alex Rodriguez",
                OnlineCount = 7,
                TotalCount = 15,
                ShowStatus = true,
                Size = new Size(280, 160),
                AccentColor = Color.FromArgb(63, 81, 181)
            };
        }

        /// <summary>
        /// Creates a contact card social widget
        /// Uses ContactCardPainter.cs
        /// </summary>
        public static BeepSocialWidget CreateContactCardWidget()
        {
            return new BeepSocialWidget
            {
                Style = SocialWidgetStyle.ContactCard,
                Title = "Contact Info",
                UserName = "Emma Thompson",
                UserRole = "UX Designer",
                UserStatus = "Away",
                ShowAvatar = true,
                ShowStatus = true,
                Size = new Size(280, 180),
                AccentColor = Color.FromArgb(233, 30, 99)
            };
        }

        /// <summary>
        /// Gets all social widget samples
        /// </summary>
        public static BeepSocialWidget[] GetAllSamples()
        {
            return new BeepSocialWidget[]
            {
                CreateProfileCardWidget(),
                CreateTeamMembersWidget(),
                CreateMessageCardWidget(),
                CreateActivityStreamWidget(),
                CreateUserListWidget(),
                CreateChatWidget(),
                CreateCommentThreadWidget(),
                CreateSocialFeedWidget(),
                CreateUserStatsWidget(),
                CreateContactCardWidget()
            };
        }
    }
}