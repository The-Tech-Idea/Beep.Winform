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
using TheTechIdea.Beep.Winform.Controls.Common;
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
        ContactCard         // Contact information card with phone, email, location
    }


    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A comprehensive card control supporting multiple modern card styles.")]
    public class BeepCard : BaseControl
    {
        #region "Fields"
        // NO child controls - paint everything directly like BeepBreadcrumb
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private string buttonText = "Action";
        private string secondaryButtonText = "More";
        private bool showSecondaryButton = true;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopCenter;
        private ContentAlignment textAlignment = ContentAlignment.TopLeft;
        private CardStyle _style = CardStyle.BasicCard;
        private bool showButton = true;
        private string imagePath = string.Empty;
        private ICardPainter _painter;
        private Color _accentColor = Color.FromArgb(0, 150, 136); // teal
        
        // Layout rectangles computed by painter
        private LayoutContext _layoutContext;
        
        // Hover tracking
        private string _hoveredArea = null;

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

        // Events - using BaseControl's built-in event system
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        public event EventHandler<BeepEventDataArgs> BadgeClicked;
        public event EventHandler<BeepEventDataArgs> RatingClicked;
        #endregion
       
        // Constructor
        // Constructor - NO child controls, paint everything directly
        public BeepCard():base()
        {
            IsChild = true;
            Padding = new Padding(12);
            PainterKind = BaseControlPainterKind.None; // No BaseControl painting - card painters handle everything
            BoundProperty = "ParagraphText";
            this.Size = new Size(320, 200);
            ApplyThemeToChilds = false;
            CanBeHovered = true;
            
            InitializePainter();
            ApplyDesignTimeData(); // Add dummy data in designer
            ApplyTheme();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                // Profile & User Cards
                case CardStyle.ProfileCard:
                    _painter = new ProfileCardPainter();
                    break;
                case CardStyle.CompactProfile:
                    _painter = new CompactProfileCardPainter();
                    break;
                case CardStyle.UserCard:
                case CardStyle.TeamMemberCard:
                    _painter = new UserCardPainter();
                    break;
                
                // Content & Blog Cards
                case CardStyle.ContentCard:
                    _painter = new ContentCardPainter();
                    break;
                case CardStyle.BlogCard:
                case CardStyle.NewsCard:
                    _painter = new BlogCardPainter();
                    break;
                case CardStyle.MediaCard:
                case CardStyle.ImageCard:
                case CardStyle.VideoCard:
                    _painter = new MediaCardPainter();
                    break;
                
                // Feature & Service Cards
                case CardStyle.FeatureCard:
                    _painter = new FeatureCardPainter();
                    break;
                case CardStyle.ServiceCard:
                case CardStyle.IconCard:
                case CardStyle.BenefitCard:
                    _painter = new ServiceCardPainter();
                    break;
                
                // E-commerce & Product Cards
                case CardStyle.ProductCard:
                case CardStyle.PricingCard:
                case CardStyle.OfferCard:
                case CardStyle.CartItemCard:
                    _painter = new ProductCardPainter();
                    break;
                
                // Social & Interaction Cards
                case CardStyle.TestimonialCard:
                    _painter = new TestimonialCardPainter();
                    break;
                case CardStyle.ReviewCard:
                case CardStyle.CommentCard:
                    _painter = new ReviewCardPainter();
                    break;
                case CardStyle.SocialMediaCard:
                    _painter = new SocialMediaCardPainter();
                    break;
                
                // Dashboard & Analytics Cards
                case CardStyle.StatCard:
                    _painter = new StatCardPainter();
                    break;
                case CardStyle.ChartCard:
                case CardStyle.MetricCard:
                case CardStyle.ActivityCard:
                    _painter = new MetricCardPainter();
                    break;
                
                // Communication & Messaging Cards
                case CardStyle.NotificationCard:
                case CardStyle.MessageCard:
                case CardStyle.AlertCard:
                case CardStyle.AnnouncementCard:
                    _painter = new CommunicationCardPainter();
                    break;
                
                // Event & Calendar Cards
                case CardStyle.EventCard:
                    _painter = new EventCardPainter();
                    break;
                case CardStyle.CalendarEventCard:
                case CardStyle.ScheduleCard:
                case CardStyle.TaskCard:
                    _painter = new CalendarCardPainter();
                    break;
                
                // List & Data Cards
                case CardStyle.ListCard:
                    _painter = new ListCardPainter();
                    break;
                case CardStyle.DataCard:
                case CardStyle.FormCard:
                case CardStyle.SettingsCard:
                    _painter = new DataCardPainter();
                    break;
                
                // Specialized Cards
                case CardStyle.DialogCard:
                    _painter = new DialogCardPainter();
                    break;
                case CardStyle.BasicCard:
                    _painter = new BasicCardPainter();
                    break;
                case CardStyle.HoverCard:
                case CardStyle.InteractiveCard:
                case CardStyle.DownloadCard:
                case CardStyle.ContactCard:
                    _painter = new InteractiveCardPainter();
                    break;
                
                default:
                    _painter = new BasicCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }



        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            // Build layout context
            _layoutContext = BuildLayoutContext();
            
            // Let painter adjust layout
            _painter?.Initialize(this, _currentTheme);
            _layoutContext = _painter?.AdjustLayout(DrawingRect, _layoutContext) ?? _layoutContext;

            // Draw background
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.DrawBackground(g, _layoutContext);
            }
            else
            {
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }

            // Paint image using StyledImagePainter
            if (_layoutContext.ShowImage && _layoutContext.ImageRect != Rectangle.Empty)
            {
                string pathToPaint = GetImagePath();
                if (!string.IsNullOrEmpty(pathToPaint))
                {
                    try
                    {
                        if (Enabled)
                        {
                            StyledImagePainter.Paint(g, _layoutContext.ImageRect, pathToPaint);
                        }
                        else
                        {
                            StyledImagePainter.PaintDisabled(g, _layoutContext.ImageRect, pathToPaint, BackColor);
                        }
                    }
                    catch
                    {
                        // Swallow painting errors to prevent designer crashes
                    }
                }
            }

            // Paint header text using TextRenderer
            if (!string.IsNullOrEmpty(headerText) && _layoutContext.HeaderRect != Rectangle.Empty)
            {
                var headerColor = _currentTheme?.CardHeaderStyle.TextColor ?? ForeColor;
                var headerFont = _currentTheme != null ? BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle) : Font;
                
                TextRenderer.DrawText(g, headerText, headerFont, _layoutContext.HeaderRect, 
                    headerColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            }

            // Paint paragraph text using TextRenderer
            if (!string.IsNullOrEmpty(paragraphText) && _layoutContext.ParagraphRect != Rectangle.Empty)
            {
                var paragraphColor = _currentTheme?.CardTextForeColor ?? ForeColor;
                var paragraphFont = _currentTheme != null ? BeepThemesManager.ToFont(_currentTheme.Paragraph) : Font;
                
                TextRenderer.DrawText(g, paragraphText, paragraphFont, _layoutContext.ParagraphRect,
                    paragraphColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }

            // Paint primary button
            if (_layoutContext.ShowButton && _layoutContext.ButtonRect != Rectangle.Empty)
            {
                bool isHovered = IsButtonHovered(_layoutContext.ButtonRect);
                PaintButton(g, _layoutContext.ButtonRect, buttonText, _accentColor, isHovered);
            }

            // Paint secondary button
            if (_layoutContext.ShowSecondaryButton && _layoutContext.SecondaryButtonRect != Rectangle.Empty)
            {
                bool isHovered = IsButtonHovered(_layoutContext.SecondaryButtonRect);
                var secondaryColor = _currentTheme?.CardBackColor ?? Color.Gray;
                PaintButton(g, _layoutContext.SecondaryButtonRect, secondaryButtonText, secondaryColor, isHovered);
            }

            // Draw foreground accents (badges, ratings, etc.)
            _painter?.DrawForegroundAccents(g, _layoutContext);

            // Register hit areas for interaction
            RefreshHitAreas(_layoutContext);
        }

        private void RefreshLayout()
        {
            // Only keep DrawingRect calculation; painters decide internal layout
            Padding = new Padding(3);
            UpdateDrawingRect();
        }

        private void RefreshHitAreas(LayoutContext ctx)
        {
            ClearHitList();

            // Image hit area
            if (ctx.ShowImage && ctx.ImageRect != Rectangle.Empty)
            {
                AddHitArea("Image", ctx.ImageRect, null, () =>
                {
                    ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
                });
            }

            // Header hit area
            if (!string.IsNullOrEmpty(headerText) && ctx.HeaderRect != Rectangle.Empty)
            {
                AddHitArea("Header", ctx.HeaderRect, null, () =>
                {
                    HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
                });
            }

            // Paragraph hit area
            if (!string.IsNullOrEmpty(paragraphText) && ctx.ParagraphRect != Rectangle.Empty)
            {
                AddHitArea("Paragraph", ctx.ParagraphRect, null, () =>
                {
                    ParagraphClicked?.Invoke(this, new BeepEventDataArgs("ParagraphClicked", this));
                });
            }

            // Primary button hit area
            if (ctx.ShowButton && ctx.ButtonRect != Rectangle.Empty)
            {
                AddHitArea("Button", ctx.ButtonRect, null, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                });
            }

            // Secondary button hit area
            if (ctx.ShowSecondaryButton && ctx.SecondaryButtonRect != Rectangle.Empty)
            {
                AddHitArea("SecondaryButton", ctx.SecondaryButtonRect, null, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("SecondaryButtonClicked", this));
                });
            }
        }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            BackColor = _currentTheme.CardBackColor;
           
            InitializePainter();
            Invalidate();
        }

        #region Helper Methods

        // Build layout context with all current card data
        private LayoutContext BuildLayoutContext()
        {
            // Determine if we should show image (check both property and design-time samples)
            bool hasImage = !string.IsNullOrEmpty(imagePath) || (DesignMode && !string.IsNullOrEmpty(GetDesignTimeSampleImage(_style)));
            
            return new LayoutContext
            {
                DrawingRect = DrawingRect,
                ImageRect = Rectangle.Empty, // Painter will calculate
                HeaderRect = Rectangle.Empty,
                ParagraphRect = Rectangle.Empty,
                ButtonRect = Rectangle.Empty,
                SecondaryButtonRect = Rectangle.Empty,
                ShowImage = hasImage,
                ShowButton = showButton,
                ShowSecondaryButton = showSecondaryButton,
                Radius = BorderRadius,
                AccentColor = _accentColor,
                Tags = _tags,
                BadgeText1 = _badgeText1,
                Badge1BackColor = _badge1BackColor,
                Badge1ForeColor = _badge1ForeColor,
                BadgeText2 = _badgeText2,
                Badge2BackColor = _badge2BackColor,
                Badge2ForeColor = _badge2ForeColor,
                SubtitleText = _subtitleText,
                StatusText = _statusText,
                StatusColor = _statusColor,
                ShowStatus = _showStatus,
                Rating = _rating,
                ShowRating = _showRating
            };
        }

        // Get image path with design-time fallback
        private string GetImagePath()
        {
            if (!string.IsNullOrEmpty(ImagePath))
                return ImagePath;

            if (DesignMode)
                return GetDesignTimeSampleImage(CardStyle);

            return null;
        }

        // Paint a button directly
        private void PaintButton(Graphics g, Rectangle rect, string text, Color backColor, bool isHovered)
        {
            if (rect == Rectangle.Empty || string.IsNullOrEmpty(text))
                return;

            // Adjust color for hover
            Color btnColor = isHovered ? ControlPaint.Light(backColor, 0.2f) : backColor;

            // Draw button background
            using (var brush = new SolidBrush(btnColor))
            {
                if (IsRounded && BorderRadius > 0)
                {
                    using (var path = CreateRoundedRectanglePath(rect, BorderRadius))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Draw button border if needed
            if (BorderThickness > 0)
            {
                using (var pen = new Pen(_currentTheme?.BorderColor ?? Color.Gray, BorderThickness))
                {
                    if (IsRounded && BorderRadius > 0)
                    {
                        using (var path = CreateRoundedRectanglePath(rect, BorderRadius))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
            }

            // Draw button text
            var textColor = _currentTheme?.CardTitleForeColor ?? Color.White;
            var textFont = _currentTheme != null ? BeepThemesManager.ToFont(_currentTheme.ButtonStyle) : Font;
            TextRenderer.DrawText(g, text, textFont, rect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // Create rounded rectangle path for buttons
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            // Top left
            path.AddArc(arc, 180, 90);
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        // Check if a button is hovered
        private bool IsButtonHovered(Rectangle buttonRect)
        {
            if (string.IsNullOrEmpty(_hoveredArea))
                return false;

            return _hoveredArea == "Button" && buttonRect == _layoutContext.ButtonRect ||
                   _hoveredArea == "SecondaryButton" && buttonRect == _layoutContext.SecondaryButtonRect;
        }

        // Override mouse move to track hover state
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string newHoveredArea = null;

            // Check which area is hovered
            if (_layoutContext.ButtonRect.Contains(e.Location))
                newHoveredArea = "Button";
            else if (_layoutContext.SecondaryButtonRect.Contains(e.Location))
                newHoveredArea = "SecondaryButton";
            else if (_layoutContext.ImageRect.Contains(e.Location))
                newHoveredArea = "Image";
            else if (_layoutContext.HeaderRect.Contains(e.Location))
                newHoveredArea = "Header";
            else if (_layoutContext.ParagraphRect.Contains(e.Location))
                newHoveredArea = "Paragraph";

            // Trigger repaint if hover state changed
            if (newHoveredArea != _hoveredArea)
            {
                _hoveredArea = newHoveredArea;
                Invalidate();
            }
        }

        #endregion

        #region Design-Time Data

        // Apply design-time dummy data based on card Style - inspired by modern web frameworks
        private void ApplyDesignTimeData()
        {
            if (!DesignMode) return;

            switch (_style)
            {
                #region Profile & User Cards
                
                case CardStyle.ProfileCard:
                    headerText = "Alex Morgan";
                    paragraphText = "Senior Full Stack Developer | Cloud Architecture Specialist\nPassionate about building scalable solutions";
                    _subtitleText = "@alexmorgan";
                    buttonText = "Follow";
                    secondaryButtonText = "Message";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Cat;
                    _badgeText1 = "Pro";
                    _badge1BackColor = Color.FromArgb(255, 193, 7); // Amber
                    _badge1ForeColor = Color.Black;
                    break;

                case CardStyle.CompactProfile:
                    headerText = "Jordan Chen";
                    paragraphText = "UI/UX Designer • 12K followers";
                    _subtitleText = "Active now";
                    buttonText = "Connect";
                    showButton = true;
                    showSecondaryButton = false;
                    imagePath = Svgs.Person;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80); // Green
                    break;

                case CardStyle.UserCard:
                    headerText = "Taylor Swift";
                    paragraphText = "Product Manager at TechCorp";
                    _subtitleText = "San Francisco, CA";
                    buttonText = "View Profile";
                    showButton = true;
                    imagePath = Svgs.User;
                    _badgeText1 = "2.5K";
                    _badgeText2 = "150";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    _badge2BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge2ForeColor = Color.White;
                    break;

                case CardStyle.TeamMemberCard:
                    headerText = "Morgan Lee";
                    paragraphText = "Lead DevOps Engineer\nAWS Certified Solutions Architect";
                    _subtitleText = "Team Lead";
                    buttonText = "Contact";
                    secondaryButtonText = "Schedule";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.PersonEdit;
                    break;

                #endregion

                #region Content & Blog Cards

                case CardStyle.ContentCard:
                    headerText = "10 Best Practices for Modern UI Design";
                    paragraphText = "Discover the latest trends and techniques that top designers use to create stunning user interfaces.";
                    _subtitleText = "Design • 5 min read";
                    _badgeText1 = "NEW";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    buttonText = "Read Article";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.BlogCard:
                    headerText = "Building Scalable Microservices with .NET";
                    paragraphText = "Learn how to architect and deploy microservices that can handle millions of requests per day.";
                    _subtitleText = "By John Smith • Dec 15, 2024";
                    _badgeText1 = "Development";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    buttonText = "Continue Reading";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.NewsCard:
                    headerText = "Microsoft Announces New AI Features";
                    paragraphText = "Tech giant unveils groundbreaking AI capabilities in latest product update";
                    _subtitleText = "TechNews • 1 hour ago";
                    _badgeText1 = "Breaking";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    buttonText = "Read More";
                    showButton = true;
                    imagePath = Svgs.InfoAlert;
                    break;

                case CardStyle.MediaCard:
                    headerText = "Stunning Landscape Photography";
                    paragraphText = "Explore breathtaking views from around the world";
                    _subtitleText = "Photography Collection";
                    buttonText = "View Gallery";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    break;

                #endregion

                #region Feature & Service Cards

                case CardStyle.FeatureCard:
                    headerText = "Advanced Analytics";
                    paragraphText = "Get deep insights into your data with our powerful analytics engine. Real-time dashboards and custom reports.";
                    _badgeText1 = "PRO";
                    _badge1BackColor = Color.FromArgb(255, 152, 0); // Orange
                    _badge1ForeColor = Color.White;
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.TrendUp;
                    break;

                case CardStyle.ServiceCard:
                    headerText = "Cloud Hosting";
                    paragraphText = "Deploy your applications with confidence. 99.9% uptime guarantee.";
                    _subtitleText = "Starting at $9.99/month";
                    buttonText = "Get Started";
                    showButton = true;
                    imagePath = Svgs.DataSources;
                    _badgeText1 = "Popular";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.IconCard:
                    headerText = "Secure & Encrypted";
                    paragraphText = "Your data is protected with enterprise-grade security";
                    buttonText = "View Security";
                    showButton = true;
                    imagePath = Svgs.Keys;
                    break;

                case CardStyle.BenefitCard:
                    headerText = "Why Choose Us?";
                    paragraphText = "✓ 24/7 Support\n✓ Easy Integration\n✓ Scalable Solutions\n✓ Cost Effective";
                    buttonText = "Compare Plans";
                    showButton = true;
                    imagePath = Svgs.CheckCircle;
                    break;

                #endregion

                #region E-commerce & Product Cards

                case CardStyle.ProductCard:
                    headerText = "Wireless Headphones Pro";
                    paragraphText = "Premium noise-cancelling headphones with 40hr battery life";
                    _subtitleText = "$299.99";
                    _priceText = "$299.99";
                    buttonText = "Add to Cart";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    _rating = 5;
                    _showRating = true;
                    _badgeText1 = "-20%";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.PricingCard:
                    headerText = "Professional Plan";
                    paragraphText = "Perfect for growing teams\n• Unlimited projects\n• Advanced features\n• Priority support\n• Custom integrations";
                    _subtitleText = "$49/month";
                    _priceText = "$49";
                    buttonText = "Choose Plan";
                    secondaryButtonText = "Compare";
                    showButton = true;
                    showSecondaryButton = true;
                    _badgeText1 = "Most Popular";
                    _badge1BackColor = Color.FromArgb(255, 193, 7); // Amber
                    _badge1ForeColor = Color.Black;
                    break;

                case CardStyle.OfferCard:
                    headerText = "Black Friday Sale!";
                    paragraphText = "Save up to 70% on select items. Limited time offer!";
                    _subtitleText = "Ends in 2 days";
                    buttonText = "Shop Now";
                    showButton = true;
                    _badgeText1 = "HOT DEAL";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.Star;
                    break;

                case CardStyle.CartItemCard:
                    headerText = "Premium T-Shirt";
                    paragraphText = "Size: L • Color: Navy Blue";
                    _subtitleText = "$29.99 × 2";
                    buttonText = "Update";
                    secondaryButtonText = "Remove";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Cat;
                    break;

                #endregion

                #region Social & Interaction Cards

                case CardStyle.TestimonialCard:
                    headerText = "Emma Wilson";
                    paragraphText = "\"This product completely transformed our workflow. The team is more productive than ever, and our clients are thrilled with the results!\"";
                    _subtitleText = "CEO, TechVision Inc.";
                    _rating = 5;
                    _showRating = true;
                    imagePath = Svgs.Person;
                    break;

                case CardStyle.ReviewCard:
                    headerText = "Outstanding Service!";
                    paragraphText = "I've been using this for 6 months and it's been fantastic. Highly recommend to anyone looking for a reliable solution.";
                    _subtitleText = "David Martinez • Verified Purchase";
                    _rating = 5;
                    _showRating = true;
                    buttonText = "Helpful";
                    showButton = true;
                    imagePath = Svgs.ThumbUp;
                    _badgeText1 = "Verified";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.CommentCard:
                    headerText = "Michael Chen";
                    paragraphText = "Great article! I especially liked the section about performance optimization. Would love to see more content like this.";
                    _subtitleText = "2 hours ago";
                    buttonText = "Reply";
                    secondaryButtonText = "Like";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Comment;
                    _badgeText1 = "❤ 15";
                    break;

                case CardStyle.SocialMediaCard:
                    headerText = "Sarah Johnson";
                    paragraphText = "Just shipped a major update! 🚀 Check out the new features we've been working on. Your feedback would mean a lot! #development #coding";
                    _subtitleText = "3 hours ago";
                    _badgeText1 = "♥ 234";
                    _badgeText2 = "💬 42";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    _badge2BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge2ForeColor = Color.White;
                    buttonText = "Like";
                    secondaryButtonText = "Share";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Person;
                    break;

                #endregion

                #region Dashboard & Analytics Cards

                case CardStyle.StatCard:
                    headerText = "12,458";
                    paragraphText = "Active Users";
                    _subtitleText = "+18.2% from last month";
                    _badgeText1 = "↑";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.TrendUp;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80);
                    break;

                case CardStyle.ChartCard:
                    headerText = "Revenue Overview";
                    paragraphText = "Monthly revenue trends and projections for Q4 2024";
                    buttonText = "View Report";
                    showButton = true;
                    imagePath = Svgs.TrendUp;
                    _subtitleText = "$124,500";
                    break;

                case CardStyle.MetricCard:
                    headerText = "Conversion Rate";
                    paragraphText = "3.8%";
                    _subtitleText = "+0.5% vs last week";
                    _badgeText1 = "Target: 4%";
                    _badge1BackColor = Color.FromArgb(255, 152, 0); // Orange
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.Sum;
                    break;

                case CardStyle.ActivityCard:
                    headerText = "New Order Placed";
                    paragraphText = "Order #4567 by John Smith\nTotal: $156.99";
                    _subtitleText = "5 minutes ago";
                    buttonText = "View Order";
                    showButton = true;
                    imagePath = Svgs.InfoAlert;
                    _badgeText1 = "NEW";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    break;

                #endregion

                #region Communication & Messaging Cards

                case CardStyle.NotificationCard:
                    headerText = "System Update Available";
                    paragraphText = "A new version is ready to install. Update now to get the latest features and security improvements.";
                    _subtitleText = "Just now";
                    buttonText = "Update Now";
                    secondaryButtonText = "Later";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.InfoInfo;
                    break;

                case CardStyle.MessageCard:
                    headerText = "Lisa Anderson";
                    paragraphText = "Hey! Did you get a chance to review the proposal? Let me know if you have any questions.";
                    _subtitleText = "10:32 AM";
                    buttonText = "Reply";
                    showButton = true;
                    imagePath = Svgs.Mail;
                    break;

                case CardStyle.AlertCard:
                    headerText = "Action Required";
                    paragraphText = "Your payment method will expire soon. Please update your billing information to avoid service interruption.";
                    _subtitleText = "Expires in 7 days";
                    buttonText = "Update Payment";
                    showButton = true;
                    imagePath = Svgs.ExclamationTriangle;
                    _badgeText1 = "URGENT";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.AnnouncementCard:
                    headerText = "New Feature Released!";
                    paragraphText = "We've added dark mode support and improved performance. Check out what's new in the latest update.";
                    _subtitleText = "December 15, 2024";
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    _badgeText1 = "v2.5";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    break;

                #endregion

                #region Event & Calendar Cards

                case CardStyle.EventCard:
                    headerText = "Annual Developer Conference 2025";
                    paragraphText = "Join industry leaders for three days of workshops, keynotes, and networking.\nJanuary 15-17, 2025";
                    _subtitleText = "San Francisco Convention Center";
                    _badgeText1 = "Early Bird";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    buttonText = "Register Now";
                    secondaryButtonText = "Details";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Calendar;
                    break;

                case CardStyle.CalendarEventCard:
                    headerText = "Team Meeting";
                    paragraphText = "Q4 Planning & Strategy Review";
                    _subtitleText = "Today at 2:00 PM";
                    buttonText = "Join Meeting";
                    showButton = true;
                    imagePath = Svgs.fi_tr_calendar;
                    _badgeText1 = "15";
                    _badgeText2 = "DEC";
                    break;

                case CardStyle.ScheduleCard:
                    headerText = "Doctor's Appointment";
                    paragraphText = "Dr. Sarah Williams • Annual Checkup";
                    _subtitleText = "Tomorrow at 10:30 AM";
                    buttonText = "View Details";
                    secondaryButtonText = "Reschedule";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.AlarmClock;
                    break;

                case CardStyle.TaskCard:
                    headerText = "Complete Project Documentation";
                    paragraphText = "Write comprehensive docs for the new API endpoints";
                    _subtitleText = "Due: Dec 20, 2024";
                    buttonText = "Mark Complete";
                    showButton = true;
                    imagePath = Svgs.Check;
                    _badgeText1 = "High Priority";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    _showStatus = true;
                    _statusText = "In Progress";
                    _statusColor = Color.FromArgb(255, 152, 0); // Orange
                    break;

                #endregion

                #region List & Data Cards

                case CardStyle.ListCard:
                    headerText = "Project Milestones";
                    paragraphText = "✓ Requirements gathering\n✓ UI/UX design\n○ Development\n○ Testing\n○ Deployment";
                    _statusText = "Phase 2 of 5";
                    _statusColor = Color.FromArgb(33, 150, 243); // Blue
                    _showStatus = true;
                    buttonText = "View Timeline";
                    showButton = true;
                    imagePath = Svgs.Bullet;
                    break;

                case CardStyle.DataCard:
                    headerText = "Server Status";
                    paragraphText = "CPU: 45%\nMemory: 62%\nDisk: 78%\nNetwork: 12 MB/s";
                    _subtitleText = "Last updated: 2 minutes ago";
                    buttonText = "Details";
                    showButton = true;
                    imagePath = Svgs.DataView;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80); // Green
                    break;

                case CardStyle.FormCard:
                    headerText = "Contact Information";
                    paragraphText = "Update your profile details";
                    buttonText = "Save Changes";
                    secondaryButtonText = "Cancel";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Edit;
                    break;

                case CardStyle.SettingsCard:
                    headerText = "Email Notifications";
                    paragraphText = "Receive updates about your account activity";
                    _subtitleText = "Currently enabled";
                    buttonText = "Configure";
                    showButton = true;
                    imagePath = Svgs.Settings;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80); // Green
                    break;

                #endregion

                #region Specialized Cards

                case CardStyle.DialogCard:
                    headerText = "Delete Account?";
                    paragraphText = "This action cannot be undone. All your data will be permanently deleted from our servers.";
                    buttonText = "Delete";
                    secondaryButtonText = "Cancel";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.ExclamationTriangle;
                    _badgeText1 = "WARNING";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.BasicCard:
                    headerText = "Simple Card Example";
                    paragraphText = "This is a basic card with minimal styling. Perfect for displaying straightforward information.";
                    buttonText = "Action";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.HoverCard:
                    headerText = "Interactive Experience";
                    paragraphText = "Hover over this card to see the smooth transition effects";
                    buttonText = "Explore";
                    showButton = true;
                    imagePath = Svgs.Cool;
                    break;

                case CardStyle.InteractiveCard:
                    headerText = "Multi-Action Card";
                    paragraphText = "This card supports multiple actions and interactions";
                    buttonText = "Primary Action";
                    secondaryButtonText = "Secondary";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.More;
                    break;

                case CardStyle.ImageCard:
                    headerText = "Beautiful Landscapes";
                    paragraphText = "Explore stunning photography from around the world";
                    buttonText = "View Gallery";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    break;

                case CardStyle.VideoCard:
                    headerText = "Product Demo Video";
                    paragraphText = "Watch our 5-minute introduction to get started quickly";
                    _subtitleText = "5:42";
                    buttonText = "Play Video";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    _badgeText1 = "▶";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.DownloadCard:
                    headerText = "Q4_2024_Report.pdf";
                    paragraphText = "Annual financial report and analysis";
                    _subtitleText = "2.4 MB • PDF Document";
                    buttonText = "Download";
                    showButton = true;
                    imagePath = Svgs.File;
                    break;

                case CardStyle.ContactCard:
                    headerText = "Customer Support";
                    paragraphText = "📧 support@example.com\n📞 +1 (555) 123-4567\n📍 123 Main St, San Francisco";
                    _subtitleText = "Available 24/7";
                    buttonText = "Contact Us";
                    showButton = true;
                    imagePath = Svgs.AddressBook;
                    break;

                #endregion

                default:
                    headerText = "Modern Card Design";
                    paragraphText = "This card showcases modern UI design principles with clean typography and thoughtful spacing.";
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;
            }

            Invalidate();
        }

        #endregion

        // Returns a sample image path for design-time previews when ImagePath is empty.
        private string GetDesignTimeSampleImage(CardStyle style)
        {
            // Use Svgs static class for all image references
            switch (style)
            {
                // Profile & User
                case CardStyle.ProfileCard:
                case CardStyle.UserCard:
                    return Svgs.Cat;
                case CardStyle.CompactProfile:
                    return Svgs.Person;
                case CardStyle.TeamMemberCard:
                    return Svgs.PersonEdit;

                // Content & Blog
                case CardStyle.ContentCard:
                case CardStyle.BlogCard:
                case CardStyle.MediaCard:
                    return Svgs.Beep;
                case CardStyle.NewsCard:
                    return Svgs.InfoAlert;

                // Feature & Service
                case CardStyle.FeatureCard:
                    return Svgs.TrendUp;
                case CardStyle.ServiceCard:
                    return Svgs.DataSources;
                case CardStyle.IconCard:
                    return Svgs.Keys;
                case CardStyle.BenefitCard:
                    return Svgs.CheckCircle;

                // E-commerce & Product
                case CardStyle.ProductCard:
                case CardStyle.CartItemCard:
                    return Svgs.Cat;
                case CardStyle.OfferCard:
                    return Svgs.Star;

                // Social & Interaction
                case CardStyle.TestimonialCard:
                case CardStyle.SocialMediaCard:
                    return Svgs.Person;
                case CardStyle.ReviewCard:
                    return Svgs.ThumbUp;
                case CardStyle.CommentCard:
                    return Svgs.Comment;

                // Dashboard & Analytics
                case CardStyle.StatCard:
                case CardStyle.ChartCard:
                    return Svgs.TrendUp;
                case CardStyle.MetricCard:
                    return Svgs.Sum;
                case CardStyle.ActivityCard:
                    return Svgs.InfoAlert;

                // Communication & Messaging
                case CardStyle.NotificationCard:
                    return Svgs.InfoInfo;
                case CardStyle.MessageCard:
                    return Svgs.Mail;
                case CardStyle.AlertCard:
                case CardStyle.DialogCard:
                    return Svgs.ExclamationTriangle;
                case CardStyle.AnnouncementCard:
                    return Svgs.Beep;

                // Event & Calendar
                case CardStyle.EventCard:
                    return Svgs.Calendar;
                case CardStyle.CalendarEventCard:
                    return Svgs.fi_tr_calendar;
                case CardStyle.ScheduleCard:
                    return Svgs.AlarmClock;
                case CardStyle.TaskCard:
                    return Svgs.Check;

                // List & Data
                case CardStyle.ListCard:
                    return Svgs.Bullet;
                case CardStyle.DataCard:
                    return Svgs.DataView;
                case CardStyle.FormCard:
                    return Svgs.Edit;
                case CardStyle.SettingsCard:
                    return Svgs.Settings;

                // Specialized
                case CardStyle.BasicCard:
                case CardStyle.HoverCard:
                    return Svgs.Beep;
                case CardStyle.InteractiveCard:
                    return Svgs.More;
                case CardStyle.ImageCard:
                case CardStyle.VideoCard:
                    return Svgs.Cat;
                case CardStyle.DownloadCard:
                    return Svgs.File;
                case CardStyle.ContactCard:
                    return Svgs.AddressBook;

                default:
                    return Svgs.Beep;
            }
        }

        private string PathCombineGfx(string folder, string file)
        {
            try
            {
                var baseDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location) ?? string.Empty, "GFX");
                var candidate = System.IO.Path.Combine(baseDir, folder, file);
                if (System.IO.File.Exists(candidate)) return candidate;
            }
            catch { }
            // fallback to logical name (ImagePainter/ImageListHelper may resolve)
            return file;
        }

        #region Enhanced Properties
        [Category("Appearance")]
        [Description("Visual Style of the card layout and design.")]
        public CardStyle CardStyle
        {
            get => _style;
            set 
            { 
                _style = value; 
                InitializePainter(); 
                ApplyDesignTimeData(); // Refresh dummy data when Style changes
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Accent color used for highlights and accents.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Content")]
        [Description("Subtitle text displayed below the header.")]
        public string SubtitleText
        {
            get => _subtitleText;
            set { _subtitleText = value; Invalidate(); }
        }

        [Category("Content")]
        [Description("Status text (e.g., 'Available for work').")]
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color of the status indicator.")]
        public Color StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the status indicator.")]
        public bool ShowStatus
        {
            get => _showStatus;
            set { _showStatus = value; Invalidate(); }
        }

        [Category("Content")]
        [Description("Rating value (0-5 stars) for testimonial cards.")]
        public int Rating
        {
            get => _rating;
            set { _rating = Math.Max(0, Math.Min(5, value)); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the rating stars.")]
        public bool ShowRating
        {
            get => _showRating;
            set { _showRating = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary badge text, e.g., PRO or FREE.")]
        public string BadgeText1
        {
            get => _badgeText1;
            set { _badgeText1 = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color Badge1BackColor
        {
            get => _badge1BackColor;
            set { _badge1BackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color Badge1ForeColor
        {
            get => _badge1ForeColor;
            set { _badge1ForeColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Secondary badge text (optional).")]
        public string BadgeText2
        {
            get => _badgeText2;
            set { _badgeText2 = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color Badge2BackColor
        {
            get => _badge2BackColor;
            set { _badge2BackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color Badge2ForeColor
        {
            get => _badge2ForeColor;
            set { _badge2ForeColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Tags/chips rendered on the card.")]
        public List<string> Tags
        {
            get => _tags;
            set { _tags = value ?? new List<string>(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text for secondary button.")]
        public string SecondaryButtonText
        {
            get => secondaryButtonText;
            set { secondaryButtonText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the secondary button.")]
        public bool ShowSecondaryButton
        {
            get => showSecondaryButton;
            set { showSecondaryButton = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set { headerText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the paragraph of the card.")]
        public string ParagraphText
        {
            get => paragraphText;
            set { paragraphText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed on the primary action button.")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Determines whether the primary action button is visible.")]
        public bool ShowButton
        {
            get => showButton;
            set { showButton = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
              Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The alignment of the header text.")]
        public ContentAlignment HeaderAlignment
        {
            get => headerAlignment;
            set { headerAlignment = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the image (Style-specific usage).")]
        public ContentAlignment ImageAlignment
        {
            get => imageAlignment;
            set { imageAlignment = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the paragraph text.")]
        public ContentAlignment TextAlignment
        {
            get => textAlignment;
            set { textAlignment = value; Invalidate(); }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _painter = null;
            }
            base.Dispose(disposing);
        }
    }
}