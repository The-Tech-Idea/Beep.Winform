using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    public class BeepCard : BeepControl
    {
        private BeepImage imageBox;
        private Label headerLabel;
        private Label paragraphLabel;
        private string headerText = "Card Title";
        private string paragraphText = "Card Description";
        private int maxImageSize = 64;
        private ContentAlignment headerAlignment = ContentAlignment.TopLeft;
        private ContentAlignment imageAlignment = ContentAlignment.TopRight;
        private ContentAlignment textAlignment = ContentAlignment.BottomCenter;


        // Properties

      

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
         //       headerLabel.Text = value;
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
        [Description("Image to display (supports SVG, PNG, JPG).")]
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
        // Expose BorderThickness from BeepPanel

        // Constructor
        public BeepCard()
        {
            Padding = new Padding(0);
            //ShowTitle = false;
            //ShowTitleLine = false;
            InitializeComponents();
            Console.WriteLine("BeepCard Constructor");
           // InitializeComponents();
            Console.WriteLine("BeepCard Constructor End");
           
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            InitializeComponents();
            PerformLayout();
            Invalidate();
            ApplyTheme(); // Apply the default theme initially
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

            headerLabel = new Label
            {
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
                Text = headerText  // Default text
            };
            Controls.Add(headerLabel);

            paragraphLabel = new Label
            {
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.Transparent,
                Text = paragraphText  // Default text
            };
            Controls.Add(paragraphLabel);
            paragraphLabel.MouseEnter += (s, e) => { BorderColor = HoverBackColor;  };
            paragraphLabel.MouseLeave += (s, e) => { BorderColor = _currentTheme.BorderColor; };
            RefreshLayout();
        }

        // Apply the theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;

            headerLabel.ForeColor = _currentTheme.CardTitleForeColor;
            headerLabel.Font = _currentTheme.GetBlockHeaderFont();

            paragraphLabel.ForeColor = _currentTheme.CardTextForeColor;
            paragraphLabel.Font = _currentTheme.GetBlockTextFont();
            imageBox.Theme = Theme;
            RefreshLayout();
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

            int             padding = Padding.All;
            // Ensure the control has a minimum size to avoid negative or zero size issues
            if (DrawingRect.Width <= padding * 2 || Height <= padding * 2)
            {
                return; // Skip layout if there's not enough space
            }

            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);
            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);

            // Header size and alignment
            Size headerSize = headerLabel.PreferredSize;
            headerLabel.Size = headerSize;
            // MessageBox.Show(headerSize.ToString());
            // Image size and alignment
            Size imageSize = new Size(Math.Min(maxImageSize, availableWidth / 2), Math.Min(maxImageSize, availableHeight / 2));
            imageBox.Size = imageSize;

            // Determine top row layout (header + image)
            int topRowHeight = Math.Max(headerSize.Height, imageSize.Height);
            int topRowY = padding;

            // Position header and image based on alignment
            if (HeaderAlignment == ContentAlignment.TopLeft || HeaderAlignment == ContentAlignment.MiddleLeft)
            {
                // Header left, image right
                headerLabel.Location = new Point(DrawingRect.Left+padding, DrawingRect.Top + topRowY);
                imageBox.Location = new Point(DrawingRect.Left + DrawingRect.Width - imageSize.Width - padding, DrawingRect.Top + topRowY);
            }
            else if (HeaderAlignment == ContentAlignment.TopRight || HeaderAlignment == ContentAlignment.MiddleRight)
            {
                // Header right, image left
                headerLabel.Location = new Point(DrawingRect.Left + DrawingRect.Width - headerSize.Width - padding, DrawingRect.Top + topRowY);
                imageBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + topRowY);
            }
            else
            {
                // Header center, image right
                headerLabel.Location = new Point(DrawingRect.Left + (DrawingRect.Width - headerSize.Width) / 2, DrawingRect.Top + topRowY);
                imageBox.Location = new Point(DrawingRect.Left + DrawingRect.Width - imageSize.Width - padding, DrawingRect.Top + topRowY);
            }

            // Paragraph
            int remainingHeightForText = availableHeight - topRowHeight - padding;
            if (remainingHeightForText > 0)
            {
                paragraphLabel.Size = new Size(availableWidth, remainingHeightForText);
                paragraphLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + topRowY + topRowHeight + padding);
                paragraphLabel.Visible = true;
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
