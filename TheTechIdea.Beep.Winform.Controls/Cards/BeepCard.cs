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
        private Font _titleFont = SystemFonts.DefaultFont;
        private Font _bodyFont = SystemFonts.DefaultFont;
        private Font _captionFont = SystemFonts.DefaultFont;
        private Font _headerFont = SystemFonts.DefaultFont;
        private Font _paragraphFont = SystemFonts.DefaultFont;

        // Events - using BaseControl's built-in event system
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        public event EventHandler<BeepEventDataArgs> BadgeClicked;
        public event EventHandler<BeepEventDataArgs> RatingClicked;
        public event EventHandler SelectionChanged;
        public event EventHandler ContextMenuRequested;
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
            
            // Set accessibility properties
            AccessibleRole = AccessibleRole.Grouping;
            AccessibleName = "Card";
            AccessibleDescription = $"Card: {CardStyle}";
            
            // Enable keyboard navigation
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;

            _interactionManager = new CardInteractionManager(this, () => Invalidate());
            _expandCollapseTimer = new Timer { Interval = 16 };
            _expandCollapseTimer.Tick += ExpandCollapseTimer_Tick;
            _loadingTimer = new Timer { Interval = 33 };
            _loadingTimer.Tick += LoadingTimer_Tick;
            _rememberedExpandedHeight = Height;
            
            InitializePainter();
            ApplyDesignTimeData(); // Add dummy data in designer
            ApplyTheme();
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



        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            
            // Check if layout needs recalculation
            bool needsRecalculation = !_layoutCacheValid || 
                                     _cachedDrawingRect != DrawingRect ||
                                     _cachedDrawingRect.Size != DrawingRect.Size;
            
            if (needsRecalculation)
            {
                // Build layout context
                _layoutContext = BuildLayoutContext();
                
                // Let painter adjust layout
                _painter?.Initialize(this, _currentTheme, _titleFont, _bodyFont, _captionFont);
                _layoutContext = _painter?.AdjustLayout(DrawingRect, _layoutContext) ?? _layoutContext;
                
                // Cache the layout
                _layoutCacheValid = true;
                _cachedDrawingRect = DrawingRect;
            }

            UpdateAuxiliaryHitAreas(_layoutContext);

            // Draw background
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.DrawBackground(g, _layoutContext);
            }
            else
            {
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }

            DrawAccentBar(g);

            if (IsLoading)
            {
                DrawLoadingSkeleton(g);
            }
            else
            {
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
                    var headerFont = _headerFont ?? _titleFont ?? _bodyFont ?? SystemFonts.DefaultFont;

                    TextRenderer.DrawText(g, headerText, headerFont, _layoutContext.HeaderRect,
                        headerColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                }

                // Paint paragraph text using TextRenderer
                if (!string.IsNullOrEmpty(paragraphText) && _layoutContext.ParagraphRect != Rectangle.Empty)
                {
                    var paragraphColor = _currentTheme?.CardTextForeColor ?? ForeColor;
                    var paragraphFont = _paragraphFont ?? _bodyFont ?? _captionFont ?? SystemFonts.DefaultFont;

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
            }

            DrawAuxiliaryIcons(g);
            DrawRippleOverlay(g);
            DrawFocusRing(g);

            // Register hit areas for interaction
            RefreshHitAreas(_layoutContext);
        }

        private void RefreshLayout()
        {
            // Only keep DrawingRect calculation; painters decide internal layout
            Padding = new Padding(3);
            UpdateDrawingRect();
            InvalidateLayoutCache();
        }

        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next paint
        /// </summary>
        private void InvalidateLayoutCache()
        {
            _layoutCacheValid = false;
        }

        private void RefreshHitAreas(LayoutContext ctx)
        {
            ClearHitList();

            if (_isLoading)
            {
                if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
                {
                    AddHitArea("SelectionCheckbox", _selectionRect, null, () => IsSelected = !IsSelected);
                }

                if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
                {
                    AddHitArea("ContextMenu", _contextMenuRect, null, () => ContextMenuRequested?.Invoke(this, EventArgs.Empty));
                }

                if (_isCollapsible && !_collapseRect.IsEmpty)
                {
                    AddHitArea("CollapseChevron", _collapseRect, null, () => ToggleExpandedState());
                }

                return;
            }

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

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
            {
                AddHitArea("SelectionCheckbox", _selectionRect, null, () =>
                {
                    IsSelected = !IsSelected;
                });
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
            {
                AddHitArea("ContextMenu", _contextMenuRect, null, () =>
                {
                    ContextMenuRequested?.Invoke(this, EventArgs.Empty);
                });
            }

            if (_isCollapsible && !_collapseRect.IsEmpty)
            {
                AddHitArea("CollapseChevron", _collapseRect, null, () =>
                {
                    ToggleExpandedState();
                });
            }
        }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            BackColor = _currentTheme.CardBackColor;
            _titleFont = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
            _bodyFont = BeepThemesManager.ToFont(_currentTheme.BodyMedium);
            _captionFont = BeepThemesManager.ToFont(_currentTheme.CaptionStyle);
            _headerFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            _paragraphFont = BeepThemesManager.ToFont(_currentTheme.Paragraph);
           
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
                ShowRating = _showRating,
                IsHovered = _interactionManager?.HoverProgress > 0.001f,
                IsPressed = _interactionManager?.PressProgress > 0.001f,
                IsSelected = _isSelected,
                IsLoading = _isLoading,
                HoverProgress = _interactionManager?.HoverProgress ?? 0f,
                PressProgress = _interactionManager?.PressProgress ?? 0f,
                IsFocused = Focused,
                RipplePoint = _interactionManager?.RippleCenter ?? Point.Empty
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
            var textFont = _bodyFont ?? _captionFont ?? _headerFont ?? SystemFonts.DefaultFont;
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

        private int Scale(int value) => DpiScalingHelper.ScaleValue(value, this);

        private void DrawAccentBar(Graphics g)
        {
            if (_accentBarHeight <= 0) return;
            int stripHeight = Scale(_accentBarHeight);
            if (stripHeight <= 0) return;

            var accentRect = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, Math.Min(stripHeight, DrawingRect.Height));
            using var brush = new SolidBrush(_accentColor);
            g.FillRectangle(brush, accentRect);
        }

        private void DrawFocusRing(Graphics g)
        {
            if (!Focused) return;

            float thickness = DpiScalingHelper.ScaleValue(2f, this);
            Color ringColor = Color.FromArgb(160, _currentTheme?.AccentColor ?? _accentColor);
            CardRenderingHelpers.DrawFocusRing(g, DrawingRect, BorderRadius, ringColor, thickness);
        }

        private void DrawRippleOverlay(Graphics g)
        {
            if (_interactionManager == null || _interactionManager.RippleAlpha <= 0 || _interactionManager.RippleRadius <= 0f)
            {
                return;
            }

            Color rippleColor = _currentTheme?.AccentColor ?? _accentColor;
            CardRenderingHelpers.DrawRippleOverlay(
                g,
                DrawingRect,
                BorderRadius,
                _interactionManager.RippleCenter,
                _interactionManager.RippleRadius,
                _interactionManager.RippleAlpha,
                rippleColor);
        }

        private void DrawLoadingSkeleton(Graphics g)
        {
            var surface = _currentTheme?.CardBackColor ?? BackColor;
            var shimmerTint = _currentTheme?.CardTitleForeColor ?? ForeColor;
            var baseColor = Color.FromArgb(60, surface);
            var shimmerColor = Color.FromArgb(130, shimmerTint);

            CardRenderingHelpers.DrawShimmerSkeleton(g, DrawingRect, BorderRadius, _loadingShimmerPhase, baseColor, shimmerColor);

            int pad = Scale(16);
            int lineHeight = Scale(12);
            int lineGap = Scale(8);
            int iconSize = Scale(48);

            Rectangle iconRect = new Rectangle(DrawingRect.X + pad, DrawingRect.Y + pad, iconSize, iconSize);
            Rectangle titleRect = new Rectangle(iconRect.Right + pad, DrawingRect.Y + pad, Math.Max(60, DrawingRect.Width - iconSize - (pad * 3)), lineHeight + Scale(2));
            Rectangle line1Rect = new Rectangle(DrawingRect.X + pad, titleRect.Bottom + lineGap, Math.Max(60, DrawingRect.Width - (pad * 2)), lineHeight);
            Rectangle line2Rect = new Rectangle(DrawingRect.X + pad, line1Rect.Bottom + lineGap, Math.Max(40, (int)(DrawingRect.Width * 0.65f)), lineHeight);
            Rectangle btnRect = new Rectangle(DrawingRect.X + pad, DrawingRect.Bottom - pad - Scale(30), Scale(92), Scale(24));

            using var placeholderBrush = new SolidBrush(Color.FromArgb(85, shimmerTint));
            using var smallRadiusPath = CardRenderingHelpers.CreateRoundedPath(titleRect, Scale(4));
            using var line1Path = CardRenderingHelpers.CreateRoundedPath(line1Rect, Scale(4));
            using var line2Path = CardRenderingHelpers.CreateRoundedPath(line2Rect, Scale(4));
            using var btnPath = CardRenderingHelpers.CreateRoundedPath(btnRect, Scale(6));
            g.FillEllipse(placeholderBrush, iconRect);
            g.FillPath(placeholderBrush, smallRadiusPath);
            g.FillPath(placeholderBrush, line1Path);
            g.FillPath(placeholderBrush, line2Path);
            g.FillPath(placeholderBrush, btnPath);
        }

        private void DrawAuxiliaryIcons(Graphics g)
        {
            Color iconTint = _currentTheme?.CardTitleForeColor ?? ForeColor;

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
            {
                float opacity = _isSelected ? 1f : 0.55f;
                StyledImagePainter.PaintWithTint(g, _selectionRect, SvgsUI.CircleCheck, iconTint, opacity, Scale(4));
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
            {
                StyledImagePainter.PaintWithTint(g, _contextMenuRect, _contextMenuIcon, iconTint, 0.85f, Scale(4));
            }

            if (_isCollapsible && !_collapseRect.IsEmpty)
            {
                string chevron = _isExpanded ? SvgsUI.CircleChevronUp : SvgsUI.ChevronDown;
                StyledImagePainter.PaintWithTint(g, _collapseRect, chevron, iconTint, 0.85f, Scale(4));
            }
        }

        private void UpdateAuxiliaryHitAreas(LayoutContext ctx)
        {
            int margin = Scale(8);
            int iconSize = Scale(18);

            _selectionRect = Rectangle.Empty;
            _contextMenuRect = Rectangle.Empty;
            _collapseRect = Rectangle.Empty;

            if (ctx == null || ctx.DrawingRect == Rectangle.Empty)
            {
                return;
            }

            if (_showSelectionCheckbox)
            {
                _selectionRect = new Rectangle(
                    ctx.DrawingRect.X + margin,
                    ctx.DrawingRect.Y + margin,
                    iconSize,
                    iconSize);
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon))
            {
                _contextMenuRect = new Rectangle(
                    ctx.DrawingRect.Right - margin - iconSize,
                    ctx.DrawingRect.Y + margin,
                    iconSize,
                    iconSize);
            }

            if (_isCollapsible)
            {
                _collapseRect = new Rectangle(
                    ctx.DrawingRect.Right - margin - iconSize,
                    ctx.DrawingRect.Bottom - margin - iconSize,
                    iconSize,
                    iconSize);
            }
        }

        // Check if a button is hovered
        private bool IsButtonHovered(Rectangle buttonRect)
        {
            if (string.IsNullOrEmpty(_hoveredArea))
                return false;

            return _hoveredArea == "Button" && buttonRect == _layoutContext.ButtonRect ||
                   _hoveredArea == "SecondaryButton" && buttonRect == _layoutContext.SecondaryButtonRect;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _interactionManager?.NotifyMouseEnter();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _interactionManager?.NotifyMouseLeave();
            if (_hoveredArea != null)
            {
                _hoveredArea = null;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _interactionManager?.NotifyMouseDown(e.Button, e.Location);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _interactionManager?.NotifyMouseUp(e.Button, e.Location);
        }

        // Override mouse move to track hover state
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _interactionManager?.NotifyMouseMove(e.Location);

            string newHoveredArea = null;
            Cursor desiredCursor = Cursors.Default;

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty && _selectionRect.Contains(e.Location))
            {
                newHoveredArea = "SelectionCheckbox";
                desiredCursor = Cursors.Hand;
            }
            else if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty && _contextMenuRect.Contains(e.Location))
            {
                newHoveredArea = "ContextMenu";
                desiredCursor = Cursors.Hand;
            }
            else if (_isCollapsible && !_collapseRect.IsEmpty && _collapseRect.Contains(e.Location))
            {
                newHoveredArea = "CollapseChevron";
                desiredCursor = Cursors.Hand;
            }
            else if (_layoutContext != null)
            {
                // Check which area is hovered
                if (_layoutContext.ButtonRect.Contains(e.Location))
                {
                    newHoveredArea = "Button";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.SecondaryButtonRect.Contains(e.Location))
                {
                    newHoveredArea = "SecondaryButton";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.ImageRect.Contains(e.Location))
                {
                    newHoveredArea = "Image";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.HeaderRect.Contains(e.Location))
                {
                    newHoveredArea = "Header";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.ParagraphRect.Contains(e.Location))
                {
                    newHoveredArea = "Paragraph";
                }
            }

            Cursor = desiredCursor;

            // Trigger repaint if hover state changed
            if (newHoveredArea != _hoveredArea)
            {
                _hoveredArea = newHoveredArea;
                Invalidate();
            }
        }

        /// <summary>
        /// Handles dialog keys (Enter, Space, Tab) for better dialog integration
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!Enabled) return base.ProcessDialogKey(keyData);

            switch (keyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (Focused || TabStop)
                    {
                        // Trigger primary button click if available
                        if (_layoutContext.ShowButton && !_layoutContext.ButtonRect.IsEmpty)
                        {
                            ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                            return true;
                        }
                    }
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Redraws when focus is gained to show focus indicator
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _interactionManager?.NotifyFocusChanged(true);
            Invalidate(); // Redraw to show focus indicator
        }

        /// <summary>
        /// Redraws when focus is lost to remove focus indicator
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _interactionManager?.NotifyFocusChanged(false);
            Invalidate(); // Redraw to remove focus indicator
        }

        /// <summary>
        /// Invalidates layout cache when control is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isExpanded && !_expandCollapseTimer.Enabled)
            {
                _rememberedExpandedHeight = Height;
            }
            InvalidateLayoutCache();
        }

        private void ToggleExpandedState()
        {
            IsExpanded = !IsExpanded;
        }

        private int GetCollapsedHeight()
        {
            int minCollapsed = Scale(72) + Math.Max(0, Scale(_accentBarHeight));
            if (_layoutContext != null && !_layoutContext.HeaderRect.IsEmpty)
            {
                int headerBased = (_layoutContext.HeaderRect.Bottom - DrawingRect.Top) + Scale(12);
                return Math.Max(minCollapsed, headerBased);
            }

            return minCollapsed;
        }

        private void StartExpandCollapseAnimation(bool expanding)
        {
            _expandAnimationStart = DateTime.UtcNow;
            _expandAnimationStartHeight = Height;
            _expandAnimationTargetHeight = expanding
                ? Math.Max(GetCollapsedHeight(), _rememberedExpandedHeight)
                : GetCollapsedHeight();

            if (expanding && _rememberedExpandedHeight < _expandAnimationStartHeight)
            {
                _rememberedExpandedHeight = _expandAnimationStartHeight;
                _expandAnimationTargetHeight = _rememberedExpandedHeight;
            }

            if (!_expandCollapseTimer.Enabled)
            {
                _expandCollapseTimer.Start();
            }
        }

        private void ExpandCollapseTimer_Tick(object sender, EventArgs e)
        {
            const double durationMs = 180d;
            double elapsed = (DateTime.UtcNow - _expandAnimationStart).TotalMilliseconds;
            double t = Math.Max(0d, Math.Min(1d, elapsed / durationMs));
            double eased = 1d - Math.Pow(1d - t, 3d);

            int newHeight = (int)Math.Round(_expandAnimationStartHeight + ((_expandAnimationTargetHeight - _expandAnimationStartHeight) * eased));
            if (newHeight != Height)
            {
                Height = newHeight;
            }

            InvalidateLayoutCache();
            Invalidate();

            if (t >= 1d)
            {
                _expandCollapseTimer.Stop();
                Height = _expandAnimationTargetHeight;
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            _loadingShimmerPhase += 0.03f;
            if (_loadingShimmerPhase > 1f)
            {
                _loadingShimmerPhase -= 1f;
            }

            Invalidate();
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
                // Update accessible description when style changes
                AccessibleDescription = $"Card: {value}";
                InitializePainter(); 
                ApplyDesignTimeData(); // Refresh dummy data when Style changes
                InvalidateLayoutCache();
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

        [Category("Behavior")]
        [Description("Indicates whether the card is selected.")]
        [DefaultValue(false)]
        public new bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Shows a selectable checkbox indicator in the top-left corner.")]
        [DefaultValue(false)]
        public bool ShowSelectionCheckbox
        {
            get => _showSelectionCheckbox;
            set
            {
                if (_showSelectionCheckbox == value) return;
                _showSelectionCheckbox = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Shows loading skeleton placeholders and shimmer animation.")]
        [DefaultValue(false)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                if (_isLoading)
                {
                    _loadingShimmerPhase = 0f;
                    _loadingTimer.Start();
                }
                else
                {
                    _loadingTimer.Stop();
                }

                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Height of the accent strip at top of the card.")]
        [DefaultValue(0)]
        public int AccentBarHeight
        {
            get => _accentBarHeight;
            set
            {
                int normalized = Math.Max(0, value);
                if (_accentBarHeight == normalized) return;
                _accentBarHeight = normalized;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Icon path used for the overflow context menu button.")]
        public string ContextMenuIcon
        {
            get => _contextMenuIcon;
            set
            {
                _contextMenuIcon = string.IsNullOrWhiteSpace(value) ? SvgsUI.DotsVertical : value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Enables collapse/expand affordance in card footer.")]
        [DefaultValue(false)]
        public bool IsCollapsible
        {
            get => _isCollapsible;
            set
            {
                if (_isCollapsible == value) return;
                _isCollapsible = value;
                if (!_isCollapsible && !_isExpanded)
                {
                    _isExpanded = true;
                    StartExpandCollapseAnimation(true);
                }

                Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Current expanded/collapsed state when collapse is enabled.")]
        [DefaultValue(true)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;

                if (!value && _isExpanded)
                {
                    _rememberedExpandedHeight = Math.Max(Height, _rememberedExpandedHeight);
                }

                _isExpanded = value;
                if (_isCollapsible)
                {
                    StartExpandCollapseAnimation(_isExpanded);
                }
                else
                {
                    Invalidate();
                }
            }
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
            set { showSecondaryButton = value; InvalidateLayoutCache(); Invalidate(); }
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
                InvalidateLayoutCache();
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

        /// <summary>
        /// Accessible name for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Name of the control for accessibility and screen readers.")]
        public new string AccessibleName
        {
            get => base.AccessibleName;
            set
            {
                base.AccessibleName = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Accessible description for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Description of the control for accessibility and screen readers.")]
        public new string AccessibleDescription
        {
            get => base.AccessibleDescription;
            set
            {
                base.AccessibleDescription = value;
                Invalidate();
            }
        }
        #endregion

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
