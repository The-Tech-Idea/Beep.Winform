using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
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

        private bool _multiline = false;

        // show the inner textbox properties like multiline
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                paragraphLabel.Multiline = value;
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

        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        [Category("Appearance")]
        [Description("ImagePath to display (supports SVG, PNG, JPG).")]
        public string ImagePath
        {
            get => imageBox.ImagePath;
            set
            {
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
                RefreshLayout();
            }
        }
        #endregion "Properties"

        // Expose _borderThickness from BeepPanel

        // Constructor
        public BeepCard()
        {
            IsChild = false;
            Padding = new Padding(0);
            BoundProperty = "ParagraphText";
            //ShowTitle = false;
            //ShowTitleLine = false;
            InitializeComponents();
          //  this.MinimumSize = new Size(300, 200); // Set based on layout needs
            this.Size = new Size(400, 300); // Default start size
            Console.WriteLine("BeepCard Constructor");
           // InitializeComponents();
            Console.WriteLine("BeepCard Constructor End");
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
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_isControlinvalidated)
            {
                InitializeComponents(); _isControlinvalidated = false;
            }
         }
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
                IsFramless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                
            };
            Controls.Add(headerLabel);

            paragraphLabel = new BeepTextBox
            {
               
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                IsFramless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
               // AutoScroll=true,
                Multiline = _multiline,
                //ScrollBars = System.Windows.Forms.ScrollBars.Both,
                ReadOnly = true,
                Text = paragraphText  // Default text
            };
            Controls.Add(paragraphLabel);
            paragraphLabel.MouseEnter += (s, e) => { BorderColor = HoverBackColor;  };
            paragraphLabel.MouseLeave += (s, e) => { BorderColor = _currentTheme.BorderColor; };
            headerLabel.Theme = Theme;
            paragraphLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardTitleForeColor;
            headerLabel.Font = BeepThemesManager.ToFont(_currentTheme.HeadlineMedium);
            headerLabel.BackColor = _currentTheme.CardBackColor;
            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;
            BackColor = _currentTheme.CardBackColor;
            imageBox.Theme = Theme;
            RefreshLayout();
        }

        // Apply the theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            headerLabel.Theme = Theme;
            paragraphLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardTitleForeColor;
            headerLabel.Font = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            headerLabel.BackColor = _currentTheme.CardBackColor;
            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.Font = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            paragraphLabel.BackColor = _currentTheme.CardBackColor;
            BackColor = _currentTheme.CardBackColor;
            imageBox.Theme = Theme;
            _isControlinvalidated = true;
            Invalidate();
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
            Padding = new Padding(0);
            int padding = Padding.All;
            UpdateDrawingRect();

            // Ensure the control has a minimum size to avoid negative or zero size issues
            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
            {
                return; // Skip layout if there's not enough space
            }

            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);
            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);

            // Header size and alignment
            Size headerSize = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font);
            headerLabel.Size = headerSize;

            // ImagePath size and alignment
            Size imageSize = new Size(
                Math.Min(maxImageSize, availableWidth / 2),
                Math.Min(maxImageSize, availableHeight / 2)
            );
            imageBox.Size = imageSize;

            // Determine top row layout (header + image)
            int topRowHeight = Math.Max(headerSize.Height, imageSize.Height);
            int topRowY = DrawingRect.Top + padding;

            // Position header and image based on alignment
            int headerX, imageX;

            switch (HeaderAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                    headerX = DrawingRect.Left + padding;
                    imageX = DrawingRect.Right - imageSize.Width - padding;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                    headerX = DrawingRect.Right - headerSize.Width - padding;
                    imageX = DrawingRect.Left + padding;
                    break;
                default: // Center alignment
                    headerX = DrawingRect.Left + (DrawingRect.Width - headerSize.Width) / 2;
                    imageX = DrawingRect.Right - imageSize.Width - padding;
                    break;
            }

            headerLabel.Location = new Point(headerX, topRowY);
            imageBox.Location = new Point(imageX, topRowY);

            // Paragraph label
            int remainingHeightForText = DrawingRect.Bottom - (headerLabel.Bottom + padding);

            if (remainingHeightForText > 0)
            {
                paragraphLabel.Size = new Size(availableWidth, remainingHeightForText);

                // Align the bottom of the paragraphLabel to the bottom of the DrawingRect
                paragraphLabel.Location = new Point(
                    DrawingRect.Left + padding,
                    DrawingRect.Bottom - paragraphLabel.Height - padding
                );

                paragraphLabel.Visible = true;

                // Ensure paragraph label stays within bounds
                if (paragraphLabel.Right > DrawingRect.Right || paragraphLabel.Top < headerLabel.Bottom + padding)
                {
                    paragraphLabel.Size = new Size(
                        Math.Min(paragraphLabel.Width, DrawingRect.Width - 2 * padding),
                        paragraphLabel.Height
                    );

                    paragraphLabel.Location = new Point(
                        DrawingRect.Left + padding,
                        Math.Max(headerLabel.Bottom + padding, DrawingRect.Bottom - paragraphLabel.Height - padding)
                    );
                }
            }
            else
            {
                paragraphLabel.Visible = false;
            }
        }

        // Custom painting for additional features
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;



        //}

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
