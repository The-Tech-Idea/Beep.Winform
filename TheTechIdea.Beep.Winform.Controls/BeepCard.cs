using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

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
        private string imagePath = string.Empty; // Store the image path directly
        #endregion "Fields"

        #region "Properties"
        [Category("Appearance")]
        [Description("The view mode of the card (FullImage, Compact, ImageLeft).")]
        public CardViewMode ViewMode
        {
            get => viewMode;
            set
            {
                viewMode = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                headerLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Text displayed as the paragraph of the card.")]
        public string ParagraphText
        {
            get => paragraphText;
            set
            {
                paragraphText = value;
                paragraphLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Text displayed on the action button.")]
        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                actionButton.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Determines whether the action button is visible.")]
        public bool ShowButton
        {
            get => showButton;
            set
            {
                showButton = value;
                actionButton.Visible = value;
                RefreshLayout();
            }
        }

        [Browsable(true)]
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
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size of the image displayed on the card.")]
        public int MaxImageSize
        {
            get => maxImageSize;
            set
            {
                maxImageSize = value;
                imageBox.Size = new Size(value, value);
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the header text.")]
        public ContentAlignment HeaderAlignment
        {
            get => headerAlignment;
            set
            {
                headerAlignment = value;
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the image.")]
        public ContentAlignment ImageAlignment
        {
            get => imageAlignment;
            set
            {
                imageAlignment = value;
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the paragraph text.")]
        public ContentAlignment TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;
                paragraphLabel.TextAlign = value;
                RefreshLayout();
            }
        }
        #endregion "Properties"

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
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            ApplyTheme();
            RefreshLayout();
        }

        // Initialize the components
        private void InitializeComponents()
        {
            Controls.Clear();

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
            Controls.Add(imageBox);

            headerLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Text = headerText,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                Height = 23,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            Controls.Add(headerLabel);

            paragraphLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = false,
                Text = paragraphText,
                Multiline=true
            };
            Controls.Add(paragraphLabel);
            paragraphLabel.MouseEnter += (s, e) => { BorderColor = HoverBackColor; };
            paragraphLabel.MouseLeave += (s, e) => { BorderColor = _currentTheme.BorderColor; };

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
            Controls.Add(actionButton);

            ApplyTheme();
            RefreshLayout();
        }

        // Apply the theme
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
            //actionButton.BackColor = _currentTheme.ButtonBackColor;
            //actionButton.ForeColor = _currentTheme.ButtonForeColor;
            //actionButton.HoverBackColor = _currentTheme.ButtonHoverBackColor;
            //actionButton.HoverForeColor = _currentTheme.ButtonHoverForeColor;
            //actionButton.PressedBackColor = _currentTheme.ButtonActiveBackColor;
            //actionButton.PressedForeColor = _currentTheme.ButtonActiveForeColor;
            //actionButton.TextFont = _currentTheme.GetButtonFont();
            imageBox.Theme = Theme;
            imageBox.BackColor = _currentTheme.CardBackColor;
            imageBox.Invalidate();

            Invalidate();
        }

        // Handle layout adjustments
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            RefreshLayout();
        }

        // Adjust the layout based on the view mode
        private void RefreshLayout()
        {
            Padding = new Padding(10);
            int padding = Padding.All;
            UpdateDrawingRect();

            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
                return;

            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);
            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);

            // Declare variables at method level to avoid redeclaration
            int paragraphY = 0;
            int remainingHeight = 0;

            // Measure the header and paragraph text
            Size measuredHeader = headerLabel.GetPreferredSize(Size.Empty);//  TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font);
            int headerMeasuredHeight = measuredHeader.Height;

            Size measuredParagraph = paragraphLabel.GetPreferredSize(Size.Empty); TextRenderer.MeasureText(paragraphLabel.Text, paragraphLabel.Font);
            int paragraphMeasuredHeight = measuredParagraph.Height;

            // Measure the button
            Size measuredButton = TextRenderer.MeasureText(actionButton.Text, actionButton.Font);
            int buttonHeight = measuredButton.Height + 20;
            int buttonWidth = measuredButton.Width + 40;

            actionButton.Size = new Size(buttonWidth, buttonHeight);

            // Set image visibility based on view mode
            switch (viewMode)
            {
                case CardViewMode.FullImage:
                    // View 1: Image at top, text and button below (First image)
                    imageBox.Visible = !string.IsNullOrEmpty(imagePath);

                    if (imageBox.Visible)
                    {
                        // Image spans the full width at the top, taking ~60% of the height
                        int imageHeight = (int)(availableHeight * 0.6);
                        imageBox.Size = new Size(availableWidth, imageHeight);
                        imageBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);

                        // Header below the image
                        int headerY = imageBox.Bottom + padding;
                        headerLabel.Location = new Point(DrawingRect.Left + padding, headerY);
                        headerLabel.Size = new Size(availableWidth, headerMeasuredHeight);
                        headerLabel.TextAlign = ContentAlignment.TopLeft;

                        // Paragraph below the header
                        paragraphY = headerLabel.Bottom + padding;
                        remainingHeight = DrawingRect.Bottom - paragraphY - buttonHeight - padding * 2;
                        paragraphLabel.Location = new Point(DrawingRect.Left + padding, paragraphY);
                        paragraphLabel.Size = new Size(availableWidth, Math.Max(0, remainingHeight));
                        paragraphLabel.TextAlign = ContentAlignment.TopLeft;

                        // Button at the bottom right
                        actionButton.Location = new Point(DrawingRect.Right - padding - buttonWidth, DrawingRect.Bottom - padding - buttonHeight);
                    }
                    else
                    {
                        // No image, stack header, paragraph, and button
                        headerLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                        headerLabel.Size = new Size(availableWidth, headerMeasuredHeight);
                        headerLabel.TextAlign = ContentAlignment.TopLeft;

                        paragraphY = headerLabel.Bottom + padding;
                        remainingHeight = DrawingRect.Bottom - paragraphY - buttonHeight - padding * 2;
                        paragraphLabel.Location = new Point(DrawingRect.Left + padding, paragraphY);
                        paragraphLabel.Size = new Size(availableWidth, Math.Max(0, remainingHeight));
                        paragraphLabel.TextAlign = ContentAlignment.TopLeft;

                        actionButton.Location = new Point(DrawingRect.Right - padding - buttonWidth, DrawingRect.Bottom - padding - buttonHeight);
                    }
                    break;

                case CardViewMode.Compact:
                    // View 2: No image, text and button only (Second image)
                    imageBox.Visible = false;

                    // Header at the top
                    headerLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                    headerLabel.Size = new Size(availableWidth, headerMeasuredHeight);
                    headerLabel.TextAlign = ContentAlignment.TopLeft;

                    // Paragraph below the header
                    paragraphY = headerLabel.Bottom + padding;
                    remainingHeight = DrawingRect.Bottom - paragraphY - buttonHeight - padding * 2;
                    paragraphLabel.Location = new Point(DrawingRect.Left + padding, paragraphY);
                    paragraphLabel.Size = new Size(availableWidth, Math.Max(0, remainingHeight));
                    paragraphLabel.TextAlign = ContentAlignment.TopLeft;

                    // Button at the bottom right
                    actionButton.Location = new Point(DrawingRect.Right - padding - buttonWidth, DrawingRect.Bottom - padding - buttonHeight);
                    break;

                case CardViewMode.ImageLeft:
                    // View 3: Image on left, text and button on right (Third image)
                    imageBox.Visible = !string.IsNullOrEmpty(imagePath);

                    if (imageBox.Visible)
                    {
                        // Image on the left, taking ~1/3 of the width and full height (minus padding)
                        int imageWidth = (int)(availableWidth * 0.33);
                        int imageHeight = availableHeight - buttonHeight - padding;
                        imageBox.Size = new Size(imageWidth, imageHeight);
                        imageBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);

                        // Text area on the right
                        int textAreaX = imageBox.Right + padding;
                        int textAreaWidth = Math.Max(0, DrawingRect.Right - padding - textAreaX);
                        int textAreaHeight = availableHeight - buttonHeight - padding;

                        // Header at the top of the text area
                        headerLabel.Location = new Point(textAreaX, DrawingRect.Top + padding);
                        headerLabel.Size = new Size(textAreaWidth, headerMeasuredHeight);
                        headerLabel.TextAlign = ContentAlignment.TopLeft;

                        // Paragraph below the header
                        paragraphY = headerLabel.Bottom + padding;
                        remainingHeight = DrawingRect.Bottom - paragraphY - buttonHeight - padding * 2;
                        paragraphLabel.Location = new Point(textAreaX, paragraphY);
                        paragraphLabel.Size = new Size(textAreaWidth, Math.Max(0, remainingHeight));
                        paragraphLabel.TextAlign = ContentAlignment.TopLeft;

                        // Button at the bottom right of the text area
                        actionButton.Location = new Point(DrawingRect.Right - padding - buttonWidth, DrawingRect.Bottom - padding - buttonHeight);
                    }
                    else
                    {
                        // No image, stack header, paragraph, and button
                        headerLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                        headerLabel.Size = new Size(availableWidth, headerMeasuredHeight);
                        headerLabel.TextAlign = ContentAlignment.TopLeft;

                        paragraphY = headerLabel.Bottom + padding;
                        remainingHeight = DrawingRect.Bottom - paragraphY - buttonHeight - padding * 2;
                        paragraphLabel.Location = new Point(DrawingRect.Left + padding, paragraphY);
                        paragraphLabel.Size = new Size(availableWidth, Math.Max(0, remainingHeight));
                        paragraphLabel.TextAlign = ContentAlignment.TopLeft;

                        actionButton.Location = new Point(DrawingRect.Right - padding - buttonWidth, DrawingRect.Bottom - padding - buttonHeight);
                    }
                    break;
            }

            // Ensure proper z-order
            headerLabel.BringToFront();
            paragraphLabel.BringToFront();
            actionButton.BringToFront();
            Controls.SetChildIndex(headerLabel, 0);
            Controls.SetChildIndex(paragraphLabel, 1);
            Controls.SetChildIndex(actionButton, 2);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
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