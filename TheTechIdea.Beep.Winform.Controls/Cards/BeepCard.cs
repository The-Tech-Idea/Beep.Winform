using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls
{
    public enum CardStyle
    {
        // Profile-oriented styles
        ProfileCard,     // Vertical profile with large image/avatar (like Corey Tawney)
        CompactProfile,  // Smaller profile variant
        
        // Content-oriented styles  
        ContentCard,     // Banner image top, content below (like course cards)
        FeatureCard,     // Icon + title + description (like app features)
        
        // List-oriented styles
        ListCard,        // Horizontal with avatar/icon (like Director listings)
        TestimonialCard, // Quote style with avatar and rating stars
        
        // Simple styles
        DialogCard,      // Simple modal-style (like confirmation dialogs)
        BasicCard,       // Minimal card for general content
        
        // E-commerce styles
        ProductCard,     // Product showcase with image, price, rating
        ProductCompactCard, // Horizontal compact product for lists
        
        // Data/Analytics styles
        StatCard,        // Statistics and KPIs display
        
        // Social/Communication styles
        EventCard,       // Events, appointments, time-based content
        SocialMediaCard  // Social posts, feeds, announcements
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A comprehensive card control supporting multiple modern card styles.")]
    public class BeepCard : BaseControl
    {
        #region "Fields"
        private BeepImage imageBox;
        private BeepLabel headerLabel;
        private BeepLabel paragraphLabel;
        private BeepButton actionButton;
        private BeepButton secondaryButton;
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private string buttonText = "Action";
        private string secondaryButtonText = "More";
        private bool showSecondaryButton = true;
        private int maxImageSize = 64;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopRight;
        private ContentAlignment textAlignment = ContentAlignment.TopLeft;
        private CardStyle _style = CardStyle.ProfileCard;
        private bool showButton = true;
        private string imagePath = string.Empty;
        private Rectangle imageRect, headerRect, paragraphRect, buttonRect;
        private ICardPainter _painter;
        private Color _accentColor = Color.FromArgb(0, 150, 136); // teal

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

        // Events - using BaseControl's built-in event system
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        // Remove CardAreaClickedEventArgs - use BaseControl's HitDetected event instead
        #endregion
       
        // Constructor
        public BeepCard():base()
        {
            IsChild = true;
            Padding = new Padding(5);
            PainterKind= BaseControlPainterKind.Classic;
            BoundProperty = "ParagraphText";
            InitializeComponents();
            this.Size = new Size(400, 300);
            ApplyThemeToChilds = false;
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case CardStyle.ProfileCard:
                    _painter = new ProfileCardPainter();
                    break;
                case CardStyle.CompactProfile:
                    _painter = new CompactProfileCardPainter();
                    break;
                case CardStyle.ContentCard:
                    _painter = new ContentCardPainter();
                    break;
                case CardStyle.FeatureCard:
                    _painter = new FeatureCardPainter();
                    break;
                case CardStyle.ListCard:
                    _painter = new ListCardPainter();
                    break;
                case CardStyle.TestimonialCard:
                    _painter = new TestimonialCardPainter();
                    break;
                case CardStyle.DialogCard:
                    _painter = new DialogCardPainter();
                    break;
                case CardStyle.BasicCard:
                    _painter = new BasicCardPainter();
                    break;
                case CardStyle.ProductCard:
                    _painter = new ProductCardPainter();
                    break;
                case CardStyle.ProductCompactCard:
                    _painter = new ProductCompactCardPainter();
                    break;
                case CardStyle.StatCard:
                    _painter = new StatCardPainter();
                    break;
                case CardStyle.EventCard:
                    _painter = new EventCardPainter();
                    break;
                case CardStyle.SocialMediaCard:
                    _painter = new SocialMediaCardPainter();
                    break;
                default:
                    _painter = new ProfileCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }

        // Initialize the components
        private void InitializeComponents()
        {
            imageBox = new BeepImage
            {
                Size = new Size(maxImageSize, maxImageSize),
                Theme = Theme,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ApplyThemeOnImage = false,
                IsFrameless = true,
                Visible = !string.IsNullOrEmpty(imagePath)
            };

            headerLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Text = headerText,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                Height = 23,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };

            paragraphLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = false,
                Text = paragraphText,
                Multiline = true
            };

            actionButton = new BeepButton
            {
                Text = buttonText,
                Theme = Theme,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Visible = showButton
            };

            secondaryButton = new BeepButton
            {
                Text = secondaryButtonText,
                Theme = Theme,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Visible = showSecondaryButton
            };

            ApplyTheme();
            RefreshLayout();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            RefreshLayout();

            // Enhanced LayoutContext with new properties
            var ctx = new LayoutContext
            {
                DrawingRect = DrawingRect,
                ImageRect = imageRect,
                HeaderRect = headerRect,
                ParagraphRect = paragraphRect,
                ButtonRect = buttonRect,
                ShowImage = !string.IsNullOrEmpty(imagePath) && imageBox.Visible,
                ShowButton = showButton,
                Radius = BorderRadius,
                AccentColor = _accentColor,
                Tags = _tags,
                ShowSecondaryButton = showSecondaryButton,
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
            UpdateDrawingRect();
            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

           // _painter?.DrawBackground(g, ctx);
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.DrawBackground(g, ctx);
            }
            else
            {
                // Paint background based on selected style
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            if (ctx.ShowImage && imageBox.Visible && ctx.ImageRect != Rectangle.Empty)
            {
                imageBox.Draw(g, ctx.ImageRect);
            }

            headerLabel.Draw(g, ctx.HeaderRect);
            paragraphLabel.Draw(g, ctx.ParagraphRect);

            if (ctx.ShowButton)
            {
                actionButton.Text = buttonText;
                actionButton.Draw(g, ctx.ButtonRect);
            }

            if (ctx.ShowSecondaryButton)
            {
                secondaryButton.Text = secondaryButtonText;
                var r = ctx.SecondaryButtonRect == Rectangle.Empty
                    ? new Rectangle(Math.Max(DrawingRect.Left + 12, ctx.ButtonRect.Left - 120 - 12), ctx.ButtonRect.Top, 120, ctx.ButtonRect.Height)
                    : ctx.SecondaryButtonRect;
                secondaryButton.Draw(g, r);
            }

            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            
            // Use BaseControl's hit area system - painters can register custom hit areas
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => 
            {
                // This will trigger the HitDetected event from BaseControl
                // No need for custom CardAreaClickedEventArgs
            });
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

            if (!string.IsNullOrEmpty(imagePath) && imageBox.Visible && ctx.ImageRect != Rectangle.Empty)
            {
                AddHitArea("Image", ctx.ImageRect, imageBox, () =>
                {
                    ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
                });
            }

            AddHitArea("Header", ctx.HeaderRect, headerLabel, () =>
            {
                HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
            });

            AddHitArea("Paragraph", ctx.ParagraphRect, paragraphLabel, () =>
            {
                ParagraphClicked?.Invoke(this, new BeepEventDataArgs("ParagraphClicked", this));
            });

            if (ctx.ShowButton && ctx.ButtonRect != Rectangle.Empty)
            {
                AddHitArea("Button", ctx.ButtonRect, actionButton, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                });
            }

            if (ctx.ShowSecondaryButton && ctx.SecondaryButtonRect != Rectangle.Empty)
            {
                AddHitArea("SecondaryButton", ctx.SecondaryButtonRect, secondaryButton, () =>
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("SecondaryButtonClicked", this));
                });
            }
        }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;

            BackColor = _currentTheme.CardBackColor;

            headerLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardHeaderStyle.TextColor;
            headerLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            headerLabel.BackColor = _currentTheme.CardBackColor;

            paragraphLabel.Theme = Theme;
            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.Paragraph);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;

            actionButton.Theme = Theme;
            actionButton.IsRounded = IsRounded;
            actionButton.BorderRadius = BorderRadius;
            actionButton.BorderThickness = BorderThickness;

            if (secondaryButton == null)
            {
                secondaryButton = new BeepButton();
            }
            secondaryButton.Theme = Theme;
            secondaryButton.IsRounded = IsRounded;
            secondaryButton.BorderRadius = BorderRadius;
            secondaryButton.BorderThickness = BorderThickness;

            imageBox.Theme = Theme;
            imageBox.BackColor = _currentTheme.CardBackColor;

            InitializePainter();
            Invalidate();
        }

        #region Enhanced Properties
        [Category("Appearance")]
        [Description("Visual style of the card layout and design.")]
        public CardStyle CardStyle
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
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
            set { secondaryButtonText = value; if (secondaryButton != null) secondaryButton.Text = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the secondary button.")]
        public bool ShowSecondaryButton
        {
            get => showSecondaryButton;
            set { showSecondaryButton = value; if (secondaryButton != null) secondaryButton.Visible = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set { headerText = value; headerLabel.Text = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the paragraph of the card.")]
        public string ParagraphText
        {
            get => paragraphText;
            set { paragraphText = value; paragraphLabel.Text = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed on the primary action button.")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value; actionButton.Text = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Determines whether the primary action button is visible.")]
        public bool ShowButton
        {
            get => showButton;
            set { showButton = value; actionButton.Visible = value; Invalidate(); }
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
                imageBox.ImagePath = value;
                imageBox.Visible = !string.IsNullOrEmpty(value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Maximum size of the image displayed on the card (if used by style).")]
        public int MaxImageSize
        {
            get => maxImageSize;
            set { maxImageSize = value; imageBox.Size = new Size(value, value); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the header text.")]
        public ContentAlignment HeaderAlignment
        {
            get => headerAlignment;
            set { headerAlignment = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the image (style-specific usage).")]
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
            set { textAlignment = value; paragraphLabel.TextAlign = value; Invalidate(); }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                imageBox?.Dispose();
                headerLabel?.Dispose();
                paragraphLabel?.Dispose();
                actionButton?.Dispose();
                secondaryButton?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}