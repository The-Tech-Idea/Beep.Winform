using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum SocialWidgetStyle
    {
        ProfileCard,      // User profile display card
        TeamMembers,      // Team member avatar grid
        MessageCard,      // Communication/message display
        ActivityStream,   // Social activity feed
        UserList,         // Contact/user listing
        ChatWidget,       // Chat interface component
        CommentThread,    // Comment/reply thread
        SocialFeed,       // Social media style feed
        UserStats,        // User statistics display
        ContactCard       // Contact information card
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Social Widget")]
    [Category("Beep Widgets")]
    [Description("Social widget for user profiles, teams, messaging, and social interactions.")]
    public class BeepSocialWidget : BaseControl
    {
        #region Fields
        private SocialWidgetStyle _style = SocialWidgetStyle.ProfileCard;
        private IWidgetPainter _painter;
        private string _title = "Social Widget";
        private string _subtitle = "Subtitle";
        private string _userName = "John Doe";
        private string _userRole = "Developer";
        private string _userStatus = "Online";
        private string _userAvatar = "";
        private Image _avatarImage = null;
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _statusColor = Color.FromArgb(76, 175, 80);
        private Color _cardBackColor = Color.White;
        private Color _surfaceColor = Color.FromArgb(250, 250, 250);
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _userNameForeColor = Color.Black;
        private Color _contentForeColor = Color.FromArgb(100, 100, 100);
        private Color _metadataForeColor = Color.FromArgb(150, 150, 150);
        private Color _hoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _highlightBackColor = Color.FromArgb(240, 240, 240);
        private Color _engagementColor = Color.FromArgb(33, 150, 243);
        private Color _positiveColor = Color.FromArgb(76, 175, 80);
        private Color _primaryColor = Color.FromArgb(33, 150, 243);
        private Color _secondaryColor = Color.FromArgb(158, 158, 158);
        private Color _avatarBorderColor = Color.FromArgb(200, 200, 200);
        private Color _onProfileColor = Color.FromArgb(150, 150, 150);
        private List<SocialItem> _socialItems = new List<SocialItem>();
        private bool _showStatus = true;
        private bool _showAvatar = true;
        private int _onlineCount = 0;
        private int _totalCount = 0;

        // Events
        public event EventHandler<BeepEventDataArgs> UserClicked;
        public event EventHandler<BeepEventDataArgs> AvatarClicked;
        public event EventHandler<BeepEventDataArgs> MessageClicked;
        public event EventHandler<BeepEventDataArgs> StatusClicked;
        public event EventHandler<BeepEventDataArgs> ActionClicked;
        #endregion

        #region Constructor
        public BeepSocialWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(300, 200);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _socialItems.AddRange(new[]
            {
                new SocialItem { Name = "John Doe", Role = "Senior Developer", Status = "Online", Avatar = "", LastSeen = "Now" },
                new SocialItem { Name = "Jane Smith", Role = "UI/UX Designer", Status = "Away", Avatar = "", LastSeen = "5 min ago" },
                new SocialItem { Name = "Bob Wilson", Role = "Project Manager", Status = "Busy", Avatar = "", LastSeen = "1 hour ago" },
                new SocialItem { Name = "Alice Brown", Role = "QA Engineer", Status = "Offline", Avatar = "", LastSeen = "2 hours ago" },
                new SocialItem { Name = "Charlie Davis", Role = "DevOps Engineer", Status = "Online", Avatar = "", LastSeen = "Now" }
            });

            _onlineCount = _socialItems.Count(x => x.Status == "Online");
            _totalCount = _socialItems.Count;
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case SocialWidgetStyle.ProfileCard:
                    _painter = new SocialProfileCardPainter();
                    break;
                case SocialWidgetStyle.TeamMembers:
                    _painter = new TeamMembersPainter();
                    break;
                case SocialWidgetStyle.MessageCard:
                    _painter = new MessageCardPainter();
                    break;
                case SocialWidgetStyle.ActivityStream:
                    _painter = new ActivityStreamPainter();
                    break;
                case SocialWidgetStyle.UserList:
                    _painter = new UserListPainter();
                    break;
                case SocialWidgetStyle.ChatWidget:
                    _painter = new ChatWidgetPainter();
                    break;
                case SocialWidgetStyle.CommentThread:
                    _painter = new CommentThreadPainter();
                    break;
                case SocialWidgetStyle.SocialFeed:
                    _painter = new SocialFeedPainter();
                    break;
                case SocialWidgetStyle.UserStats:
                    _painter = new UserStatsPainter();
                    break;
                case SocialWidgetStyle.ContactCard:
                    _painter = new ContactCardPainter();
                    break;
                default:
                    _painter = new SocialProfileCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Social")]
        [Description("Visual style of the social widget.")]
        public SocialWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Social")]
        [Description("Title text for the social widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Subtitle text for the social widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("User name for profile widgets.")]
        public string UserName
        {
            get => _userName;
            set { _userName = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("User role or title.")]
        public string UserRole
        {
            get => _userRole;
            set { _userRole = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("User status (Online, Away, Busy, Offline).")]
        public string UserStatus
        {
            get => _userStatus;
            set { _userStatus = value; UpdateStatusColor(); Invalidate(); }
        }

        [Category("Social")]
        [Description("Path to user avatar image.")]
        public string UserAvatar
        {
            get => _userAvatar;
            set { _userAvatar = value; LoadAvatarImage(); Invalidate(); }
        }

        [Category("Social")]
        [Description("User avatar image.")]
        public Image AvatarImage
        {
            get => _avatarImage;
            set { _avatarImage = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the social widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Status indicator color.")]
        public Color StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Whether to show user status indicator.")]
        public bool ShowStatus
        {
            get => _showStatus;
            set { _showStatus = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Whether to show user avatar.")]
        public bool ShowAvatar
        {
            get => _showAvatar;
            set { _showAvatar = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Number of online users.")]
        public int OnlineCount
        {
            get => _onlineCount;
            set { _onlineCount = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Total number of users.")]
        public int TotalCount
        {
            get => _totalCount;
            set { _totalCount = value; Invalidate(); }
        }

        [Category("Social")]
        [Description("Collection of social items for lists and feeds.")]
        public List<SocialItem> SocialItems
        {
            get => _socialItems;
            set { _socialItems = value ?? new List<SocialItem>(); UpdateCounts(); Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = _showAvatar,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                CustomData = new Dictionary<string, object>
                {
                    ["SocialItems"] = _socialItems,
                    ["UserName"] = _userName,
                    ["UserRole"] = _userRole,
                    ["UserStatus"] = _userStatus,
                    ["AvatarImage"] = _avatarImage,
                    ["StatusColor"] =_statusColor,
                    ["ShowStatus"] = _showStatus,
                    ["ShowAvatar"] = _showAvatar,
                    ["OnlineCount"] = _onlineCount,
                    ["TotalCount"] = _totalCount
                }
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("User", ctx.ContentRect, null, () =>
                {
                    UserClicked?.Invoke(this, new BeepEventDataArgs("UserClicked", this));
                });
            }

            if (!ctx.IconRect.IsEmpty)
            {
                AddHitArea("Avatar", ctx.IconRect, null, () =>
                {
                    AvatarClicked?.Invoke(this, new BeepEventDataArgs("AvatarClicked", this));
                });
            }

            if (!ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Message", ctx.HeaderRect, null, () =>
                {
                    MessageClicked?.Invoke(this, new BeepEventDataArgs("MessageClicked", this));
                });
            }

            if (_showStatus && !ctx.FooterRect.IsEmpty)
            {
                AddHitArea("Status", ctx.FooterRect, null, () =>
                {
                    StatusClicked?.Invoke(this, new BeepEventDataArgs("StatusClicked", this));
                });
            }

            // Add hit areas for individual social items based on style
            if (_style == SocialWidgetStyle.TeamMembers || _style == SocialWidgetStyle.UserList)
            {
                for (int i = 0; i < _socialItems.Count && i < 6; i++) // Limit to 6 visible items
                {
                    int itemIndex = i; // Capture for closure
                    AddHitArea($"Item{i}", new Rectangle(), null, () =>
                    {
                        ActionClicked?.Invoke(this, new BeepEventDataArgs("ActionClicked", this) { EventData = _socialItems[itemIndex] });
                    });
                }
            }
        }

        private void LoadAvatarImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_userAvatar) && System.IO.File.Exists(_userAvatar))
                {
                    _avatarImage?.Dispose();
                    _avatarImage = Image.FromFile(_userAvatar);
                }
            }
            catch
            {
                _avatarImage = null; // Failed to load image
            }
        }

        private void UpdateStatusColor()
        {
            _statusColor = _userStatus?.ToLower() switch
            {
                "online" => Color.FromArgb(76, 175, 80),  // Green
                "away" => Color.FromArgb(255, 193, 7),    // Yellow
                "busy" => Color.FromArgb(244, 67, 54),    // Red
                "offline" => Color.FromArgb(158, 158, 158), // Gray
                _ => Color.FromArgb(76, 175, 80)
            };
        }

        private void UpdateCounts()
        {
            if (_socialItems != null)
            {
                _onlineCount = _socialItems.Count(x => x.Status?.ToLower() == "online");
                _totalCount = _socialItems.Count;
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply social-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update card and content colors
            _cardBackColor = _currentTheme.CardBackColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            _panelBackColor = _currentTheme.PanelBackColor;
            
            // Update user and content text colors
            _userNameForeColor = _currentTheme.CardTitleForeColor;
            _contentForeColor = _currentTheme.CardTextForeColor;
            _metadataForeColor = _currentTheme.CardSubTitleForeColor;
            
            // Update interactive colors
            _hoverBackColor = _currentTheme.ButtonHoverBackColor;
            _highlightBackColor = _currentTheme.HighlightBackColor;
            _engagementColor = _currentTheme.AccentColor;
            
            // Update status and accent colors
            _positiveColor = _currentTheme.SuccessColor;
            _primaryColor = _currentTheme.PrimaryColor;
            _secondaryColor = _currentTheme.SecondaryColor;
            
            // Update avatar and profile colors
            _avatarBorderColor = _currentTheme.BorderColor;
            _onProfileColor = _currentTheme.OnBackgroundColor;
            
            InitializePainter();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _avatarImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Social item data structure for users, messages, and social content
    /// </summary>
    public class SocialItem
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline";
        public string Avatar { get; set; } = string.Empty;
        public Image AvatarImage { get; set; }
        public string LastSeen { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public Color StatusColor { get; set; } = Color.Gray;
        public bool IsOnline { get; set; } = false;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}