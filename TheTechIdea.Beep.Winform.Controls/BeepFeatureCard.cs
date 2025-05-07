using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Feature Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays a list of features with a logo and title using BeepListBox.")]
    public class BeepFeatureCard : BeepControl
    {
        #region "Fields"
        private BeepImage logoImage;
        private BeepLabel titleLabel;
        private BeepLabel subtitleLabel;
        private BeepListBox featuresListBox;
        private BeepImage actionIcon1;
        private BeepImage actionIcon2;
        private BeepImage cardIcon;

        private string logoPath = "";
        private string titleText = "Sphere UI";
        private string subtitleText = "Charts version";
        private List<SimpleItem> bulletPoints = new List<SimpleItem>
        {
            new SimpleItem { Text = "80+ combined charts in 4 layouts.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "300+ components.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "10+ pages in 2 color schemes: blue & green.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "Autolayout for all elements & components.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "Fully connected library of fonts & color.", ItemType = Vis.Modules.MenuItemType.Main }
        };
        private string bulletIconPath = "bullet.svg";
        private string actionIcon1Path = "action1.svg";
        private string actionIcon2Path = "action2.svg";
        private string cardIconPath = "simpleinfoapps.svg";
        #endregion "Fields"

        #region "Properties"
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the logo image (SVG, PNG, etc.).")]
        public string LogoPath
        {
            get => logoPath;
            set
            {
                logoPath = value;
                logoImage.ImagePath = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("The title text displayed at the top of the card.")]
        public string TitleText
        {
            get => titleText;
            set
            {
                titleText = value;
                titleLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("The subtitle text displayed below the title.")]
        public string SubtitleText
        {
            get => subtitleText;
            set
            {
                subtitleText = value;
                subtitleLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("The list of bullet points displayed on the card.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> BulletPoints
        {
            get => bulletPoints;
            set
            {
                bulletPoints = value;
                UpdateFeaturesList();
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the bullet icon SVG.")]
        public string BulletIconPath
        {
            get => bulletIconPath;
            set
            {
                bulletIconPath = value;
                UpdateFeaturesList();
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the first action icon SVG (top right).")]
        public string ActionIcon1Path
        {
            get => actionIcon1Path;
            set
            {
                actionIcon1Path = value;
                actionIcon1.ImagePath = value;
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the second action icon SVG (top right).")]
        public string ActionIcon2Path
        {
            get => actionIcon2Path;
            set
            {
                actionIcon2Path = value;
                actionIcon2.ImagePath = value;
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the card icon SVG (top right).")]
        public string CardIconPath
        {
            get => cardIconPath;
            set
            {
                cardIconPath = value;
                cardIcon.ImagePath = value;
                RefreshLayout();
            }
        }
        #endregion "Properties"

        // Constructor
        public BeepFeatureCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            IsChild = false;
            Padding = new Padding(10);
            BoundProperty = "TitleText";
            InitializeComponents();
            this.Size = new Size(300, 200);
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

            logoImage = new BeepImage
            {
                ImagePath = logoPath,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Size = new Size(24, 24) // Small logo size
            };
            Controls.Add(logoImage);

            titleLabel = new BeepLabel
            {
                Text = titleText,
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(titleLabel);

            subtitleLabel = new BeepLabel
            {
                Text = subtitleText,
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(subtitleLabel);

            // Initialize BeepListBox for features
            featuresListBox = new BeepListBox
            {
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowImage = true,
                ShowCheckBox = false,
                ShowHilightBox = true,
                ShowTitleLine = false,
                MenuItemHeight = 30,
                ImageSize = 20,
                ShowTitle=false,
                AutoSize = false // We'll set the size in RefreshLayout
            };
            UpdateFeaturesList();
            Controls.Add(featuresListBox);

            actionIcon1 = new BeepImage
            {
                ImagePath = actionIcon1Path,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Size = new Size(32, 32) // Small icon size
            };
            Controls.Add(actionIcon1);

            actionIcon2 = new BeepImage
            {
                ImagePath = actionIcon2Path,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Size = new Size(32, 32) // Small icon size
            };
            Controls.Add(actionIcon2);

            cardIcon = new BeepImage
            {
                ImagePath = cardIconPath,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Size = new Size(64, 64) // Larger icon size for card icon
            };
            Controls.Add(cardIcon);
            AutoScroll = false;
            ApplyTheme();
            RefreshLayout();
        }

        // Apply the theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;

            BackColor = _currentTheme.CardBackColor;
            ParentBackColor = _currentTheme.CardBackColor;
            
            logoImage.BackColor = _currentTheme.CardBackColor;
            logoImage.ParentBackColor = _currentTheme.CardBackColor;
            titleLabel.Theme = Theme;
            titleLabel.ForeColor = _currentTheme.CardTitleForeColor;
            titleLabel.TextFont = new Font(BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle).FontFamily, 14, FontStyle.Bold);
            titleLabel.BackColor = _currentTheme.CardBackColor;
            subtitleLabel.Theme = Theme;
            subtitleLabel.ForeColor = _currentTheme.CardTitleForeColor;
            subtitleLabel.TextFont = new Font(BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle).FontFamily, 10);
            subtitleLabel.BackColor = _currentTheme.CardBackColor;
            featuresListBox.Theme = Theme;
            featuresListBox.BackColor = _currentTheme.CardBackColor;
            featuresListBox.TextFont = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            featuresListBox.ForeColor = _currentTheme.CardTextForeColor;
            featuresListBox.SetColors();
            featuresListBox.Invalidate();
            actionIcon1.FillColor = Color.White;
            actionIcon1.StrokeColor = Color.White;
            actionIcon1.BackColor = _currentTheme.CardBackColor;
            actionIcon1.ParentBackColor = _currentTheme.CardBackColor;
            actionIcon2.FillColor = Color.White;
            actionIcon2.StrokeColor = Color.White;
            actionIcon2.BackColor = _currentTheme.CardBackColor;
            actionIcon2.ParentBackColor = _currentTheme.CardBackColor;
            cardIcon.FillColor = Color.White;
            cardIcon.ParentBackColor= _currentTheme.CardBackColor;
            cardIcon.StrokeColor = Color.White;
            cardIcon.BackColor = _currentTheme.CardBackColor;
            
            Invalidate();
        }

        // Handle layout adjustments
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            RefreshLayout();
        }

        // Adjust the layout based on the components
        private void RefreshLayout()
        {
            int padding = 1;
            UpdateDrawingRect();

            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
                return;

            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);
            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);

            // Logo at the top left
            logoImage.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);

            // Title to the right of the logo
            titleLabel.Location = new Point(logoImage.Right + 10, DrawingRect.Top + padding);

            // Subtitle below the title
            subtitleLabel.Location = new Point(titleLabel.Left, titleLabel.Bottom + 2);

            // Action icons and card icon at the top right
            cardIcon.Location = new Point(DrawingRect.Right - padding - cardIcon.Width, DrawingRect.Top + padding);
            actionIcon2.Location = new Point(cardIcon.Left - 5 - actionIcon2.Width, DrawingRect.Top + padding);
            actionIcon1.Location = new Point(actionIcon2.Left - 5 - actionIcon1.Width, DrawingRect.Top + padding);

            // Features list (BeepListBox) below the subtitle
            int listTop = subtitleLabel.Bottom + 10;
            int listHeight = featuresListBox.GetMaxHeight();
            featuresListBox.Location = new Point(DrawingRect.Left + padding, listTop);
            featuresListBox.Size = new Size(availableWidth, listHeight);
            //AutoScrollMinSize = new Size(availableWidth, listHeight);
            //AutoScroll = true;
            // Ensure proper z-order
            logoImage.BringToFront();
            titleLabel.BringToFront();
            subtitleLabel.BringToFront();
            featuresListBox.BringToFront();
            actionIcon1.BringToFront();
            actionIcon2.BringToFront();
            cardIcon.BringToFront();
        }

        private void UpdateFeaturesList()
        {
            // Ensure each SimpleItem has the bullet icon if not already set
            foreach (var item in bulletPoints)
            {
                if (string.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = bulletIconPath;
                }
                if (item.ItemType != Vis.Modules.MenuItemType.Main)
                {
                    item.ItemType = Vis.Modules.MenuItemType.Main;
                }
            }

            // Convert List<SimpleItem> to BindingList<SimpleItem> for BeepListBox
            var simpleItems = new BindingList<SimpleItem>(bulletPoints);
            featuresListBox.ListItems = simpleItems;
           
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = RoundedRect(DrawingRect, 10))
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }

        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logoImage?.Dispose();
                titleLabel?.Dispose();
                subtitleLabel?.Dispose();
                featuresListBox?.Dispose();
                actionIcon1?.Dispose();
                actionIcon2?.Dispose();
                cardIcon?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}