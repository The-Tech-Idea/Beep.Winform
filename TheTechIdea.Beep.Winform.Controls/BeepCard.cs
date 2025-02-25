using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays an image, header, and paragraph.")]
    public class BeepCard : BeepControl
    {
        #region "Properties"
        private BeepImage imageBox;
        private BeepLabel headerLabel;
        private BeepTextBox paragraphLabel;
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private int maxImageSize = 64;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopRight;
        private ContentAlignment textAlignment = ContentAlignment.BottomCenter;


        // Properties

        //private bool _multiline = false;

        //// show the inner textbox properties like multiline
        //[Browsable(true)]
        //[Category("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //public bool Multiline
        //{
        //    get => _multiline;
        //    set
        //    {
        //        _multiline = value;
        //        paragraphLabel.Multiline = value;
        //        RefreshLayout();

        //    }
        //}

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                headerLabel.Text = value;
              //  RefreshLayout();
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
              //  RefreshLayout();
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
        private string imagepath=string.Empty;
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => imagepath;
            set
            {
                
                imagepath = value;
                imageBox.ImagePath = value;
                imageBox.Visible = !string.IsNullOrEmpty(value);
              //  RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size of the image displayed on the card.")]
        public int MaxImageSize
        {
            get => imageBox.Size.Width;
            set
            {
                imageBox.Size=new Size(value,value);
              //  RefreshLayout();
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
                RefreshLayout();
            }
        }
        #endregion "Properties"

        // Expose _borderThickness from BeepPanel

        // Constructor
        public BeepCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
           // SetStyle(ControlStyles.SupportsTransparentBackColor, true); // Ensure we handle transparent backcolors

            IsChild = false;
            Padding = new Padding(0);
            BoundProperty = "ParagraphText";
            //ShowTitle = false;
            //ShowTitleLine = false;
            InitializeComponents();
          //  this.MinimumSize = new Size(300, 200); // Set based on layout needs
            this.Size = new Size(400, 300); // Default start size
           //// Console.WriteLine("BeepCard Constructor");
           // InitializeComponents();
          // // Console.WriteLine("BeepCard Constructor End");
            ApplyThemeToChilds = false;
            ApplyTheme(); // Apply the default theme initially

        }
        protected override void InitLayout()
        {
            base.InitLayout();
           // InitializeComponents();
          //  PerformLayout();
            ApplyTheme(); // Apply the default theme initially
          //  _isControlinvalidated = true;
           // Invalidate();
        }
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    if (_isControlinvalidated)
        //    {
        //        InitializeComponents(); _isControlinvalidated = false;
        //    }
        // }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
           
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
                ImagePath = imagepath,
                IsFrameless = true,
                Visible = false  // Initially hide image until set
            };
            Controls.Add(imageBox);

            headerLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Text = headerText,  // Default text
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                Height=23,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                
            };
            Controls.Add(headerLabel);

            paragraphLabel = new BeepTextBox
            {
               
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                IsFrameless = true,
                IsChild = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
               // AutoScroll=true,
                Multiline=true,
                //ScrollBars = System.Windows.Forms.ScrollBars.Both,
                ReadOnly = true,
                Text = paragraphText  // Default text
            };
            Controls.Add(paragraphLabel);
            paragraphLabel.MouseEnter += (s, e) => { BorderColor = HoverBackColor;  };
            paragraphLabel.MouseLeave += (s, e) => { BorderColor = _currentTheme.BorderColor; };
            ApplyTheme();
            RefreshLayout();
        }

        // Apply the theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
           // headerLabel.Theme = Theme;
            imageBox.IsFrameless = true;
           // paragraphLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardHeaderStyle.TextColor;
            headerLabel.Font = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            headerLabel.BackColor = _currentTheme.CardBackColor;
            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;
            BackColor = _currentTheme.CardBackColor;
            imageBox.Theme = Theme;
            _isControlinvalidated = true;
           // Invalidate();
        }
        // Handle layout adjustments
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            RefreshLayout();
        }
        // Adjust the layout of the image and text
        private void RefreshLayout()
        {
            // Basic checks & padding
            Padding = new Padding(2);
            int padding = Padding.All;
            UpdateDrawingRect();

            // Exit early if control is too small
            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
                return;

            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);
            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);

            // 1) Measure the header text to get the row height needed
            //    (We only really need its height now; for width, we’ll fill instead.)
            Size measuredHeader = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font);
            int headerMeasuredHeight = measuredHeader.Height; // This is the “natural” text height

            // 2) Determine the image’s actual size (bounded by maxImageSize, half the row, etc.)
            Size imageSize = new Size(
                Math.Min(maxImageSize, availableWidth / 2),
                Math.Min(maxImageSize, availableHeight / 2)
            );
            imageBox.Size = imageSize;

            // 3) Figure out how tall the top row is (whichever is bigger: the image or the header text)
            int topRowHeight = Math.Max(headerMeasuredHeight, imageSize.Height);
            int topRowY = DrawingRect.Top + padding;

            // --------------------------------------------
            // EXAMPLE: If HeaderAlignment is Left, place header on the left, image on the right.
            // --------------------------------------------
            if (HeaderAlignment == ContentAlignment.TopLeft
                || HeaderAlignment == ContentAlignment.MiddleLeft)
            {
                // (A) Place the image on the right side
                int imageX = DrawingRect.Right - padding - imageSize.Width;
                int imageY = topRowY + (topRowHeight - imageSize.Height) / 2;
                imageBox.Location = new Point(imageX, imageY);

                // (B) Make headerLabel fill from the left to just before the image
                int labelX = DrawingRect.Left + padding;
                int labelWidth = Math.Max(0, imageX - labelX - 5);
                // ‘5’ is optional spacing between text & image
                int labelHeight = headerMeasuredHeight;

                // (C) Vertical positioning for the header
                int headerY = topRowY + (topRowHeight - labelHeight) / 2;

                // Set the new position & size
                headerLabel.Location = new Point(labelX, headerY);
                headerLabel.Size = new Size(labelWidth, labelHeight);

                // Optionally ensure the text itself is left-aligned within that box
                headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            }
            else if (HeaderAlignment == ContentAlignment.TopRight
                  || HeaderAlignment == ContentAlignment.MiddleRight)
            {
                // (A) Place the image on the left side
                int imageX = DrawingRect.Left + padding;
                int imageY = topRowY + (topRowHeight - imageSize.Height) / 2;
                imageBox.Location = new Point(imageX, imageY);

                // (B) Make headerLabel fill from right after the image to the control’s right edge
                int labelX = imageX + imageSize.Width + 5; // some spacing
                int labelWidth = Math.Max(0, (DrawingRect.Right - padding) - labelX);
                int labelHeight = headerMeasuredHeight;

                int headerY = topRowY + (topRowHeight - labelHeight) / 2;
                headerLabel.Location = new Point(labelX, headerY);
                headerLabel.Size = new Size(labelWidth, labelHeight);

                headerLabel.TextAlign = ContentAlignment.MiddleRight;
                // or MiddleLeft if you prefer text pinned left in that region
            }
            else
            {
                // Example: If you want "Center" alignment, you could do something else:
                // * Place image somewhere in the row.
                // * Then let the label fill the remaining space, maybe centered horizontally.
                // (Implementation depends on your exact needs.)
            }

            // 4) Next, position the paragraph label below
            int contentTop = Math.Max(headerLabel.Bottom, imageBox.Bottom) + padding;
            int remainingHeight = DrawingRect.Bottom - contentTop - padding;

            paragraphLabel.Size = new Size(availableWidth, Math.Max(0, remainingHeight));
            paragraphLabel.Location = new Point(DrawingRect.Left + padding, contentTop);

            // 5) Ensure the header is on top if needed
            headerLabel.BringToFront();
            Controls.SetChildIndex(headerLabel, 0);

            // 6) Let paragraph label anchor to all sides
            paragraphLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left
                                  | AnchorStyles.Right | AnchorStyles.Top;
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
            }
            base.Dispose(disposing);
        }
    }
}
