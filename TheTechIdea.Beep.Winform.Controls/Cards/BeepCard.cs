using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Cards.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Modern card styles inspired by popular web frameworks (Material UI, Bootstrap, Ant Design, Chakra UI, Tailwind)
    /// </summary>
    public enum CardStyle
    {
        // Profile & User Cards (Material UI, Bootstrap Card, Ant Design Card)
        ProfileCard,        // Vertical profile card with avatar, name, bio, action buttons
        CompactProfile,     // Minimal profile badge/chip Style
        UserCard,           // User info card with stats/badges (followers, posts, etc.)
        TeamMemberCard,     // Team member showcase with role and social links
        
        // Content & Blog Cards (Material UI Card, Bootstrap Card)
        ContentCard,        // Article/blog card with hero image, title, excerpt, read more
        BlogCard,           // Blog post card with author, date, tags, category badge
        NewsCard,           // News article with thumbnail, headline, source
        MediaCard,          // Media-focused card with large image and minimal text
        
        // Feature & Service Cards (Tailwind, Chakra UI)
        FeatureCard,        // Icon-based feature highlight with title and description
        ServiceCard,        // Service offering card with icon, title, price/CTA
        IconCard,           // Centered icon with text below (landing page Style)
        BenefitCard,        // Benefit/value proposition with check marks
        
        // E-commerce & Product Cards (Material UI, Ant Design)
        ProductCard,        // Product image, name, price, rating, add to cart
        PricingCard,        // Pricing tier with features list and CTA button
        OfferCard,          // Special offer/deal card with discount badge
        CartItemCard,       // Shopping cart item with quantity controls
        
        // Social & Interaction Cards (Twitter, Facebook, LinkedIn Style)
        SocialMediaCard,    // Social post with avatar, content, likes, comments, share
        TestimonialCard,    // Customer testimonial with quote, avatar, name, rating
        ReviewCard,         // Product/service review with rating and detailed feedback
        CommentCard,        // Comment/reply card with nested structure support
        
        // Dashboard & Analytics Cards (Material Dashboard, Ant Design Pro)
        StatCard,           // KPI/metric card with number, label, trend indicator
        ChartCard,          // Card with embedded chart/graph
        MetricCard,         // Metric display with icon, value, change percentage
        ActivityCard,       // Activity feed item with timestamp and icon
        
        // Communication & Messaging Cards
        NotificationCard,   // Notification item with icon, message, timestamp
        MessageCard,        // Chat message bubble Style
        AlertCard,          // Alert/warning card with icon and actions
        AnnouncementCard,   // Announcement banner card
        
        // Event & Calendar Cards
        EventCard,          // Event card with date badge, title, location, time
        CalendarEventCard,  // Calendar event with day/month badge
        ScheduleCard,       // Schedule/agenda item card
        TaskCard,           // Task/todo item card with checkbox and priority
        
        // List & Data Cards
        ListCard,           // Horizontal list item card with icon/avatar
        DataCard,           // Data display card with label-value pairs
        FormCard,           // Form section card with grouped inputs
        SettingsCard,       // Settings option card with toggle/switch
        
        // Specialized Cards
        DialogCard,         // Modal/dialog content card
        BasicCard,          // Plain card with title and content
        HoverCard,          // Card with pronounced hover effects and transitions
        InteractiveCard,    // Multi-action card with button group
        ImageCard,          // Full-width image card with overlay text
        VideoCard,          // Video thumbnail card with play button
        DownloadCard,       // File download card with icon, name, size, download button
        ContactCard,         // Contact information card with phone, email, location
        BlankCard,         // Blank card with no content
        HeaderOnlyCard,    // Card with only header text
        ParagraphOnlyCard, // Card with only paragraph text
        ButtonOnlyCard,    // Card with only button
        SecondaryButtonOnlyCard, // Card with only secondary button
        BadgeOnlyCard,     // Card with only badge
        RatingOnlyCard,    // Card with only rating
        StatusOnlyCard,    // Card with only status
        ImageOnlyCard,     // Card with only image
        VideoOnlyCard,     // Card with only video
        DownloadOnlyCard,  // Card with only download
        ContactOnlyCard,   // Card with only contact
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A comprehensive card control supporting multiple modern card styles.")]
    public partial class BeepCard : BaseControl
    {
        // NO child controls - paint everything directly like BeepBreadcrumb
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private string buttonText = "Action";
        private string secondaryButtonText = "More";
        private bool showSecondaryButton = true;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopCenter;
        private ContentAlignment textAlignment = ContentAlignment.TopLeft;
        private CardStyle _style = CardStyle.BlankCard;
        private bool showButton = true;
        private string imagePath = string.Empty;
        private ICardPainter _painter;
        private Color _accentColor = Color.FromArgb(0, 150, 136); // teal
        
        // Layout rectangles computed by painter
        private LayoutContext _layoutContext;
        private bool _layoutCacheValid = false;
        private Rectangle _cachedDrawingRect = Rectangle.Empty;
        
        // Hover tracking
        private string _hoveredArea = null;
        private readonly CardInteractionManager _interactionManager;
        private readonly Timer _expandCollapseTimer;
        private readonly Timer _loadingTimer;
        private DateTime _expandAnimationStart = DateTime.MinValue;
        private int _expandAnimationStartHeight;
        private int _expandAnimationTargetHeight;
        private int _rememberedExpandedHeight;
        private float _loadingShimmerPhase;
        private Rectangle _selectionRect = Rectangle.Empty;
        private Rectangle _contextMenuRect = Rectangle.Empty;
        private Rectangle _collapseRect = Rectangle.Empty;
        private bool _isExpanded = true;
        private bool _isLoading;
        private bool _isSelected;
        private bool _showSelectionCheckbox;
        private bool _isCollapsible;
        private int _accentBarHeight;
        private string _contextMenuIcon = SvgsUI.DotsVertical;

        // Enhanced properties for new styles
        private string _badgeText1 = string.Empty; // Primary badge (e.g., PRO, FREE)
        private Color _badge1BackColor = Color.FromArgb(255, 235, 59); // amber
        private Color _badge1ForeColor = Color.Black;
        private string _badgeText2 = string.Empty; // Secondary badge
        private Color _badge2BackColor = Color.FromArgb(33, 150, 243); // blue
        private Color _badge2ForeColor = Color.White;
        private List<string> _tags = new List<string>(); // For chips/tags
        private string _subtitleText = string.Empty; // For profile subtitles
        private int _rating = 0; // 0-5 stars for testimonials
        private bool _showRating = false;
        private string _statusText = string.Empty; // e.g., "Available for work"
        private Color _statusColor = Color.Green;
        private bool _showStatus = false;
        private string _priceText = string.Empty; // For product cards
        // CRITICAL: Clone SystemFonts.DefaultFont to avoid GDI+ "Parameter is not valid" errors
        // SystemFonts.DefaultFont is a shared static reference - cloning prevents shared-state bugs
        private Font _titleFont = new Font(SystemFonts.DefaultFont, SystemFonts.DefaultFont.Style);
        private Font _bodyFont = new Font(SystemFonts.DefaultFont, SystemFonts.DefaultFont.Style);
        private Font _captionFont = new Font(SystemFonts.DefaultFont, SystemFonts.DefaultFont.Style);
        private Font _headerFont = new Font(SystemFonts.DefaultFont, SystemFonts.DefaultFont.Style);
        private Font _paragraphFont = new Font(SystemFonts.DefaultFont, SystemFonts.DefaultFont.Style);

        // Events - using BaseControl's built-in event system
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        public event EventHandler<BeepEventDataArgs> BadgeClicked;
        public event EventHandler<BeepEventDataArgs> RatingClicked;
        public event EventHandler SelectionChanged;
        public event EventHandler ContextMenuRequested;
        
        // Constructor
        public BeepCard():base()
        {
            try
            {
                IsChild = true;
                Padding = new Padding(12);
               
                BoundProperty = "ParagraphText";
                this.Size = new Size(320, 200);
                ApplyThemeToChilds = false;
                CanBeHovered = true;
                
                AccessibleRole = AccessibleRole.Grouping;
                AccessibleName = "Card";
                AccessibleDescription = $"Card: {_style}";
                
                SetStyle(ControlStyles.Selectable, true);
                TabStop = true;

                _interactionManager = new CardInteractionManager(this, () => SafeInvalidate());
                _expandCollapseTimer = new Timer { Interval = 16 };
                _expandCollapseTimer.Tick += ExpandCollapseTimer_Tick;
                _loadingTimer = new Timer { Interval = 33 };
                _loadingTimer.Tick += LoadingTimer_Tick;
                _rememberedExpandedHeight = Height;
                
                // CRITICAL: Apply theme FIRST to set fonts and colors before creating painters
                ApplyTheme();
                
                // Then apply design-time data (uses fonts set by theme)
                ApplyDesignTimeData();
            }
            catch (Exception ex)
            {
                // Prevent design-time crashes by swallowing initialization errors
                System.Diagnostics.Debug.WriteLine($"BeepCard initialization error: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely invalidates the control, preventing exceptions during design-time
        /// </summary>
        private void SafeInvalidate()
        {
            try
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    Invalidate();
                }
            }
            catch
            {
                // Swallow to prevent designer crashes
            }
        }
        private void InitializePainter()
        {
            // Dispose old painter if it implements IDisposable
            (_painter as IDisposable)?.Dispose();
            
            // 1:1 mapping - each CardStyle gets its own distinct painter
            _painter = _style switch
            {
                // Profile & User Cards
                CardStyle.ProfileCard => new ProfileCardPainter(),
                CardStyle.CompactProfile => new CompactProfileCardPainter(),
                CardStyle.UserCard => new UserCardPainter(),
                CardStyle.TeamMemberCard => new TeamMemberCardPainter(),
                
                // Content & Blog Cards
                CardStyle.ContentCard => new ContentCardPainter(),
                CardStyle.BlogCard => new BlogCardPainter(),
                CardStyle.NewsCard => new NewsCardPainter(),
                CardStyle.MediaCard => new MediaCardPainter(),
                
                // Feature & Service Cards
                CardStyle.FeatureCard => new FeatureCardPainter(),
                CardStyle.ServiceCard => new ServiceCardPainter(),
                CardStyle.IconCard => new IconCardPainter(),
                CardStyle.BenefitCard => new BenefitCardPainter(),
                
                // E-commerce & Product Cards
                CardStyle.ProductCard => new ProductCardPainter(),
                CardStyle.PricingCard => new PricingCardPainter(),
                CardStyle.OfferCard => new OfferCardPainter(),
                CardStyle.CartItemCard => new CartItemCardPainter(),
                
                // Social & Interaction Cards
                CardStyle.SocialMediaCard => new SocialMediaCardPainter(),
                CardStyle.TestimonialCard => new TestimonialCardPainter(),
                CardStyle.ReviewCard => new ReviewCardPainter(),
                CardStyle.CommentCard => new CommentCardPainter(),
                
                // Dashboard & Analytics Cards
                CardStyle.StatCard => new StatCardPainter(),
                CardStyle.ChartCard => new ChartCardPainter(),
                CardStyle.MetricCard => new MetricCardPainter(),
                CardStyle.ActivityCard => new ActivityCardPainter(),
                
                // Communication & Messaging Cards
                CardStyle.NotificationCard => new NotificationCardPainter(),
                CardStyle.MessageCard => new MessageCardPainter(),
                CardStyle.AlertCard => new AlertCardPainter(),
                CardStyle.AnnouncementCard => new AnnouncementCardPainter(),
                
                // Event & Calendar Cards
                CardStyle.EventCard => new EventCardPainter(),
                CardStyle.CalendarEventCard => new CalendarCardPainter(),
                CardStyle.ScheduleCard => new ScheduleCardPainter(),
                CardStyle.TaskCard => new TaskCardPainter(),
                
                // List & Data Cards
                CardStyle.ListCard => new ListCardPainter(),
                CardStyle.DataCard => new DataCardPainter(),
                CardStyle.FormCard => new FormCardPainter(),
                CardStyle.SettingsCard => new SettingsCardPainter(),
                
                // Specialized Cards
                CardStyle.DialogCard => new DialogCardPainter(),
                CardStyle.BasicCard => new BasicCardPainter(),
                CardStyle.HoverCard => new HoverCardPainter(),
                CardStyle.InteractiveCard => new InteractiveCardPainter(),
                CardStyle.DownloadCard => new InteractiveCardPainter(), // Download uses Interactive style
                CardStyle.ContactCard => new InteractiveCardPainter(), // Contact uses Interactive style
                CardStyle.ImageCard => new MediaCardPainter(), // Image uses Media style
                CardStyle.VideoCard => new VideoCardPainter(),
                
                // Single-element "Only" Cards
                CardStyle.BlankCard => new BlankCardPainter(),
                CardStyle.HeaderOnlyCard => new HeaderOnlyCardPainter(),
                CardStyle.ParagraphOnlyCard => new ParagraphOnlyCardPainter(),
                CardStyle.ButtonOnlyCard => new ButtonOnlyCardPainter(),
                CardStyle.SecondaryButtonOnlyCard => new SecondaryButtonOnlyCardPainter(),
                CardStyle.BadgeOnlyCard => new BadgeOnlyCardPainter(),
                CardStyle.RatingOnlyCard => new RatingOnlyCardPainter(),
                CardStyle.StatusOnlyCard => new StatusOnlyCardPainter(),
                CardStyle.ImageOnlyCard => new ImageOnlyCardPainter(),
                CardStyle.VideoOnlyCard => new VideoOnlyCardPainter(),
                CardStyle.DownloadOnlyCard => new DownloadOnlyCardPainter(),
                CardStyle.ContactOnlyCard => new ContactOnlyCardPainter(),
                
                _ => new BlankCardPainter()
            };
            
            _painter?.Initialize(this, _currentTheme, _titleFont, _bodyFont, _captionFont);
        }



        public override void ApplyTheme()
        {
            // CRITICAL: Always set default fonts first so painters have valid fonts even without theme
            if (_currentTheme != null)
            {
                base.ApplyTheme();
                BackColor = _currentTheme.CardBackColor;
                
                // Safely replace fonts - dispose old only if new was created
                ReplaceFont(ref _titleFont, _currentTheme.TitleMedium);
                ReplaceFont(ref _bodyFont, _currentTheme.BodyMedium);
                ReplaceFont(ref _captionFont, _currentTheme.CaptionStyle);
                ReplaceFont(ref _headerFont, _currentTheme.CardHeaderStyle);
                ReplaceFont(ref _paragraphFont, _currentTheme.Paragraph);
            }
            
            // CRITICAL: Always initialize painter so control renders at design-time
            InitializePainter();
            Invalidate();
        }

        private void ReplaceFont(ref Font field, TheTechIdea.Beep.Vis.Modules.TypographyStyle style)
        {
            var newFont = BeepThemesManager.ToFont(style);
            if (newFont != null && newFont != field)
            {
                field?.Dispose();
                field = newFont;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loadingTimer?.Stop();
                _loadingTimer?.Dispose();
                _expandCollapseTimer?.Stop();
                _expandCollapseTimer?.Dispose();
                _interactionManager?.Dispose();
                _painter = null;
            }
            base.Dispose(disposing);
        }
    }
}
