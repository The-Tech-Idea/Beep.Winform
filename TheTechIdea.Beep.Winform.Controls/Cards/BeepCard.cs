using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum CardViewMode
    {
        FullImage,
        Compact,
        ImageLeft
    }

    public enum CardStyle
    {
        Classic,
        MaterialElevated,
        SoftShadow,
        Outline,
        AccentHeader,
        ListTile,
        Glass
    }

    public sealed class CardAreaClickedEventArgs : EventArgs
    {
        public string AreaName { get; }
        public Rectangle AreaRect { get; }
        public CardAreaClickedEventArgs(string name, Rectangle rect) { AreaName = name; AreaRect = rect; }
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays an image, header, paragraph, and action button with multiple views.")]
    public class BeepCard : BaseControl
    {
        #region "Fields"
        private BeepImage imageBox;
        private BeepLabel headerLabel;
        private BeepLabel paragraphLabel;
        private BeepButton actionButton;
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private string buttonText = "Action";
        private int maxImageSize = 64;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopRight;
        private ContentAlignment textAlignment = ContentAlignment.TopLeft;
        private CardViewMode viewMode = CardViewMode.FullImage;
        private CardStyle _style = CardStyle.MaterialElevated;
        private bool showButton = true;
        private string imagePath = string.Empty;
        private Rectangle imageRect, headerRect, paragraphRect, buttonRect;
        private ICardPainter _painter;
        private Color _accentColor = Color.FromArgb(0, 150, 136); // teal

        // Events
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        public event EventHandler<CardAreaClickedEventArgs> AreaClicked; // painter-provided
        #endregion

        // Constructor
        public BeepCard():base()
        {
            IsChild = false;
            Padding = new Padding(5);
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
                case CardStyle.MaterialElevated:
                    _painter = new MaterialElevatedCardPainter();
                    break;
                case CardStyle.SoftShadow:
                    _painter = new SoftShadowCardPainter();
                    break;
                case CardStyle.Outline:
                    _painter = new OutlineCardPainter();
                    break;
                case CardStyle.AccentHeader:
                    _painter = new AccentHeaderCardPainter();
                    break;
                case CardStyle.ListTile:
                    _painter = new ListTileCardPainter();
                    break;
                case CardStyle.Glass:
                    _painter = new GlassCardPainter();
                    break;
                case CardStyle.Classic:
                default:
                    _painter = new OutlineCardPainter();
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

            ApplyTheme();
            RefreshLayout();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            RefreshLayout();

            // Prepare painter layout context and let painter draw background and accents
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
                AccentColor = _accentColor
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);

            if (ctx.ShowImage && imageBox.Visible && ctx.ImageRect != Rectangle.Empty)
            {
                imageBox.Draw(g, ctx.ImageRect);
            }

            headerLabel.Draw(g, ctx.HeaderRect);
            paragraphLabel.Draw(g, ctx.ParagraphRect);

            if (ctx.ShowButton)
            {
                actionButton.Draw(g, ctx.ButtonRect);
            }

            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => AreaClicked?.Invoke(this, new CardAreaClickedEventArgs(name, rect)));
        }

        private void RefreshLayout()
        {
            Padding = new Padding(3);
            int padding = Padding.All;
            UpdateDrawingRect();

            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
                return;

            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);
            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);

            Size measuredHeader = headerLabel.GetPreferredSize(Size.Empty);
            int headerMeasuredHeight = measuredHeader.Height;

            Size measuredParagraph = paragraphLabel.GetPreferredSize(Size.Empty);
            int paragraphMeasuredHeight = measuredParagraph.Height;

            Size buttonSize = actionButton.GetPreferredSize(Size.Empty);
            int buttonHeight = buttonSize.Height;
            int buttonWidth = buttonSize.Width;

            // Layout based on view mode
            switch (viewMode)
            {
                case CardViewMode.FullImage:
                    if (imageBox.Visible)
                    {
                        int imageHeight = (int)(availableHeight * 0.6);
                        imageRect = new Rectangle(DrawingRect.Left + padding, DrawingRect.Top + padding, availableWidth, imageHeight);
                        headerRect = new Rectangle(DrawingRect.Left + padding, imageRect.Bottom + padding, availableWidth, headerMeasuredHeight);
                        paragraphRect = new Rectangle(DrawingRect.Left + padding, headerRect.Bottom + padding, availableWidth, Math.Max(0, DrawingRect.Bottom - headerRect.Bottom - buttonHeight - padding * 3));
                        buttonRect = new Rectangle(DrawingRect.Right - buttonWidth - padding, DrawingRect.Bottom - buttonHeight - padding, buttonWidth, buttonHeight);
                    }
                    else
                    {
                        headerRect = new Rectangle(DrawingRect.Left + padding, DrawingRect.Top + padding, availableWidth, headerMeasuredHeight);
                        paragraphRect = new Rectangle(DrawingRect.Left + padding, headerRect.Bottom + padding, availableWidth, Math.Max(0, DrawingRect.Bottom - headerRect.Bottom - buttonHeight - padding * 3));
                        buttonRect = new Rectangle(DrawingRect.Right - buttonWidth - padding, DrawingRect.Bottom - buttonHeight - padding, buttonWidth, buttonHeight);
                    }
                    break;

                case CardViewMode.Compact:
                    headerRect = new Rectangle(DrawingRect.Left + padding, DrawingRect.Top + padding, availableWidth, headerMeasuredHeight);
                    paragraphRect = new Rectangle(DrawingRect.Left + padding, headerRect.Bottom + padding, availableWidth, Math.Max(0, DrawingRect.Bottom - headerRect.Bottom - buttonHeight - padding * 3));
                    buttonRect = new Rectangle(DrawingRect.Right - buttonWidth - padding, DrawingRect.Bottom - buttonHeight - padding, buttonWidth, buttonHeight);
                    imageRect = Rectangle.Empty;
                    break;

                case CardViewMode.ImageLeft:
                    if (imageBox.Visible)
                    {
                        int imageWidth = (int)(availableWidth * 0.33);
                        imageRect = new Rectangle(DrawingRect.Left + padding, DrawingRect.Top + padding, imageWidth, availableHeight - buttonHeight - padding);

                        int textAreaX = imageRect.Right + padding;
                        int textAreaWidth = Math.Max(0, DrawingRect.Right - padding - textAreaX);

                        headerRect = new Rectangle(textAreaX, DrawingRect.Top + padding, textAreaWidth, headerMeasuredHeight);
                        paragraphRect = new Rectangle(textAreaX, headerRect.Bottom + padding, textAreaWidth, Math.Max(0, DrawingRect.Bottom - headerRect.Bottom - buttonHeight - padding * 3));
                        buttonRect = new Rectangle(DrawingRect.Right - buttonWidth - padding, DrawingRect.Bottom - buttonHeight - padding, buttonWidth, buttonHeight);
                    }
                    else
                    {
                        headerRect = new Rectangle(DrawingRect.Left + padding, DrawingRect.Top + padding, availableWidth, headerMeasuredHeight);
                        paragraphRect = new Rectangle(DrawingRect.Left + padding, headerRect.Bottom + padding, availableWidth, Math.Max(0, DrawingRect.Bottom - headerRect.Bottom - buttonHeight - padding * 3));
                        buttonRect = new Rectangle(DrawingRect.Right - buttonWidth - padding, DrawingRect.Bottom - buttonHeight - padding, buttonWidth, buttonHeight);
                        imageRect = Rectangle.Empty;
                    }
                    break;
            }
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
            paragraphLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;

            actionButton.Theme = Theme;
            actionButton.IsRounded = IsRounded;
            actionButton.BorderRadius = BorderRadius;
            actionButton.BorderThickness = BorderThickness;

            imageBox.Theme = Theme;
            imageBox.BackColor = _currentTheme.CardBackColor;

            InitializePainter();
            Invalidate();
        }

        #region Properties
        [Category("Appearance")]
        [Description("The view mode of the card (FullImage, Compact, ImageLeft).")]
        public CardViewMode ViewMode
        {
            get => viewMode;
            set { viewMode = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Visual style painter for the card background and accents.")]
        public CardStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Accent color used by some styles (e.g., header stripe, badges).")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set { headerText = value; headerLabel.Text = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the paragraph of the card.")]
        public string ParagraphText
        {
            get => paragraphText;
            set { paragraphText = value; paragraphLabel.Text = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text displayed on the action button.")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value; actionButton.Text = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Determines whether the action button is visible.")]
        public bool ShowButton
        {
            get => showButton;
            set { showButton = value; actionButton.Visible = value; RefreshLayout(); Invalidate(); }
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
                RefreshLayout();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Maximum size of the image displayed on the card.")]
        public int MaxImageSize
        {
            get => maxImageSize;
            set { maxImageSize = value; imageBox.Size = new Size(value, value); RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the header text.")]
        public ContentAlignment HeaderAlignment
        {
            get => headerAlignment;
            set { headerAlignment = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the image.")]
        public ContentAlignment ImageAlignment
        {
            get => imageAlignment;
            set { imageAlignment = value; RefreshLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the paragraph text.")]
        public ContentAlignment TextAlignment
        {
            get => textAlignment;
            set { textAlignment = value; paragraphLabel.TextAlign = value; RefreshLayout(); Invalidate(); }
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
            }
            base.Dispose(disposing);
        }
    }
}