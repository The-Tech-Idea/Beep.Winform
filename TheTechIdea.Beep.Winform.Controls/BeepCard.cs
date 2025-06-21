using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum CardViewMode
    {
        FullImage,    // Image at top, text and button below (First image)
        Compact,      // No image, text and button only (Second image)
        ImageLeft     // Image on left, text and button on right (Third image)
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays an image, header, paragraph, and action button with multiple views.")]
    public class BeepCard : BeepControl
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
        private bool showButton = true;
        private string imagePath = string.Empty;
        private Rectangle imageRect, headerRect, paragraphRect, buttonRect;

        // Events
        public event EventHandler<BeepEventDataArgs> ImageClicked;
        public event EventHandler<BeepEventDataArgs> HeaderClicked;
        public event EventHandler<BeepEventDataArgs> ParagraphClicked;
        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        #endregion

        // Constructor
        public BeepCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            IsChild = false;
            Padding = new Padding(0);
            BoundProperty = "ParagraphText";
            InitializeComponents();
            this.Size = new Size(400, 300);
            ApplyThemeToChilds = false;
            ApplyTheme();
            CanBeHovered = true;
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

            // Calculate the layout rectangles
            RefreshLayout();

            // Draw components
            if (!string.IsNullOrEmpty(imagePath) && imageBox.Visible)
            {
                imageBox.Draw(g, imageRect);
            }

            headerLabel.Draw(g, headerRect);
            paragraphLabel.Draw(g, paragraphRect);

            if (showButton)
            {
                actionButton.Draw(g, buttonRect);
            }

            // Refresh hit areas 
            RefreshHitAreas();
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

        private void RefreshHitAreas()
        {
            ClearHitList();

            if (!string.IsNullOrEmpty(imagePath) && imageBox.Visible)
            {
                AddHitArea("Image", imageRect, imageBox, () => 
                {
                    ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
                });
            }

            AddHitArea("Header", headerRect, headerLabel, () => 
            {
                HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
            });

            AddHitArea("Paragraph", paragraphRect, paragraphLabel, () => 
            {
                ParagraphClicked?.Invoke(this, new BeepEventDataArgs("ParagraphClicked", this));
            });

            if (showButton)
            {
                AddHitArea("Button", buttonRect, actionButton, () => 
                {
                    ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                });
            }
        }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            
            BackColor = _currentTheme.CardBackColor;
            
            // Apply theme to header label
            headerLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardHeaderStyle.TextColor;
            headerLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            headerLabel.BackColor = _currentTheme.CardBackColor;
            
            // Apply theme to paragraph label
            paragraphLabel.Theme = Theme;
            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;
            
            // Apply theme to action button
            actionButton.Theme = Theme;
            actionButton.IsRounded = IsRounded;
            actionButton.BorderRadius = BorderRadius;
            actionButton.BorderThickness = BorderThickness;
            
            // Apply theme to image
            imageBox.Theme = Theme;
            imageBox.BackColor = _currentTheme.CardBackColor;
            
            Invalidate();
        }

        // Add property change notifications to trigger layout updates
        [Category("Appearance")]
        [Description("The view mode of the card (FullImage, Compact, ImageLeft).")]
        public CardViewMode ViewMode
        {
            get => viewMode;
            set { viewMode = value; RefreshLayout(); Invalidate(); }
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