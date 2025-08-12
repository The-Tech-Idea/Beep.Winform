using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Stat Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays statistical data with a trend indicator.")]
    public class BeepStatCard : BeepControl
    {
        #region "Fields"
        private BeepLabel headerLabel;
        private BeepLabel percentageLabel;

        private BeepLabel valueLabel;
        private BeepLabel trendLabel;
       
        private BeepLabel infoLabel;
        private BeepImage trendImage;
        private BeepImage cardIcon;

        private string headerText = "Total Revenue";
        private string percentageText = "+12.5%";
        private string valueText = "$1,250.00";
        private string trendText = "Trending up this month";
        private string infoText = "Visitors for the last 6 months";
        private bool isTrendingUp = true;
        private string trendUpSvgPath = "trendup.svg";
        private string trendDownSvgPath = "trenddown.svg";
        private string cardiconSvgPath = "simpleinfoapps.svg";
        #endregion "Fields"

        #region "Properties"
        [Category("Appearance")]
        [Description("The header text displayed at the top of the card.")]
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
        [Description("The percentage change text displayed at the top right.")]
        public string PercentageText
        {
            get => percentageText;
            set
            {
                percentageText = value;
                percentageLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("The main value displayed in the center of the card.")]
        public string ValueText
        {
            get => valueText;
            set
            {
                valueText = value;
                valueLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("The trend text displayed below the main value.")]
        public string TrendText
        {
            get => trendText;
            set
            {
                trendText = value;
                trendLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Additional information displayed at the bottom of the card.")]
        public string InfoText
        {
            get => infoText;
            set
            {
                infoText = value;
                infoLabel.Text = value;
                RefreshLayout();
            }
        }

        [Category("Appearance")]
        [Description("Indicates whether the trend is up (true) or down (false).")]
        public bool IsTrendingUp
        {
            get => isTrendingUp;
            set
            {
                isTrendingUp = value;
                UpdateTrendIcons();
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the SVG file for the upward trend icon.")]
        public string TrendUpSvgPath
        {
            get => trendUpSvgPath;
            set
            {
                trendUpSvgPath = value;
                UpdateTrendIcons();
                RefreshLayout();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the SVG file for the downward trend icon.")]
        public string TrendDownSvgPath
        {
            get => trendDownSvgPath;
            set
            {
                trendDownSvgPath = value;
                UpdateTrendIcons();
                RefreshLayout();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the SVG file for the Card Icon icon.")]
        public string Icon
        {
            get => cardiconSvgPath;
            set
            {
                cardiconSvgPath = value;
                UpdateTrendIcons();
                RefreshLayout();
            }
        }
        #endregion "Properties"

        // Constructor
        public BeepStatCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            IsChild = false;
            Padding = new Padding(10);
            BoundProperty = "ValueText";
            InitializeComponents();
            this.Size = new Size(300, 150);
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

            headerLabel = new BeepLabel
            {
                Text = headerText,
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(headerLabel);

            percentageLabel = new BeepLabel
            {
                Text = percentageText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
               // IsRounded = true,
                AutoSize = true
            };
            Controls.Add(percentageLabel);


            valueLabel = new BeepLabel
            {
                Text = valueText,
                TextAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(valueLabel);

            trendLabel = new BeepLabel
            {
                Text = trendText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign= ContentAlignment.MiddleRight,
                TextImageRelation = TextImageRelation.TextBeforeImage,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(trendLabel);

           

            infoLabel = new BeepLabel
            {
                Text = infoText,
                TextAlign = ContentAlignment.TopLeft,
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            Controls.Add(infoLabel);


            cardIcon = new BeepImage
            {
                Text = "",
               
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,Size = new Size(32, 32)

            };
            Controls.Add(cardIcon);
            trendImage = new BeepImage
            {
                Text = "",
               
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };
            trendImage.Size = new Size(32, 32);
            Controls.Add(trendImage);
            ApplyTheme();
            RefreshLayout();
        }

        // Apply the theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;

            BackColor = _currentTheme.CardBackColor;
            ParentBackColor = _currentTheme.CardBackColor;
            headerLabel.Theme = Theme;
            headerLabel.ForeColor = _currentTheme.CardHeaderStyle.TextColor;
            headerLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
            headerLabel.BackColor = _currentTheme.CardBackColor;
            percentageLabel.Theme = Theme;
            percentageLabel.ForeColor = isTrendingUp ? Color.Green : Color.Red; // Adjust based on trend
            percentageLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
            percentageLabel.BackColor = _currentTheme.CardBackColor;

            valueLabel.Theme = Theme;
            valueLabel.ForeColor = _currentTheme.CardTextForeColor;
            valueLabel.TextFont = new Font(BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle).FontFamily, 24, FontStyle.Bold); // Larger font for value
            valueLabel.BackColor = _currentTheme.CardBackColor;
            trendLabel.Theme = Theme;
            trendLabel.ForeColor = _currentTheme.CardTextForeColor;
            trendLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle);
            trendLabel.BackColor = _currentTheme.CardBackColor;

            infoLabel.Theme = Theme;
            infoLabel.ForeColor = _currentTheme.CardTextForeColor;
            infoLabel.TextFont = new Font(BeepThemesManager.ToFont(_currentTheme.CardparagraphStyle).FontFamily, 8); // Smaller font for info
            infoLabel.BackColor = _currentTheme.CardBackColor;
            
            cardIcon.BackColor = _currentTheme.CardBackColor;
            cardIcon.ParentBackColor = _currentTheme.CardBackColor;
            trendImage.BackColor = _currentTheme.CardBackColor;
            trendImage.ParentBackColor = _currentTheme.CardBackColor;

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
            int padding = Padding.All;
            UpdateDrawingRect();

            if (DrawingRect.Width <= padding * 2 || DrawingRect.Height <= padding * 2)
                return;

            int availableWidth = Math.Max(0, DrawingRect.Width - padding * 2);
            int availableHeight = Math.Max(0, DrawingRect.Height - padding * 2);

            // Header at the top left
            headerLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);

            // Percentage at the top right
            percentageLabel.Location = new Point(DrawingRect.Right  - percentageLabel.Width, DrawingRect.Top + infoLabel.Height+padding);

      
            // Value in the center
            valueLabel.Location = new Point(DrawingRect.Left + (availableWidth - valueLabel.Width) / 2, DrawingRect.Top + (availableHeight - valueLabel.Height) / 2);

            // Trend label below the value
            trendLabel.Location = new Point(DrawingRect.Left + padding, valueLabel.Bottom + 5);
            trendImage.Location = new Point(DrawingRect.Right - padding - trendImage.Width, trendLabel.Top + (trendLabel.Height - trendImage.Height) / 2);

            // Info label at the bottom
            infoLabel.Location = new Point(DrawingRect.Left + padding, DrawingRect.Bottom - padding - infoLabel.Height);
            cardIcon.Location = new Point(DrawingRect.Right-cardIcon.Width-padding ,5);
            UpdateTrendIcons();
            // Ensure proper z-order
            headerLabel.BringToFront();
            percentageLabel.BringToFront();
           
            valueLabel.BringToFront();
            trendLabel.BringToFront();
           
            infoLabel.BringToFront();
        }

        private void UpdateTrendIcons()
        {
            string trendSvgPath = isTrendingUp ? trendUpSvgPath : trendDownSvgPath;
            trendImage.ImagePath = trendSvgPath;
            cardIcon.ImagePath = cardiconSvgPath;
            //  trendLabel.ImagePath = trendSvgPath;
            //percentageTrendIcon.FillColor = isTrendingUp ? Color.Green : Color.Red;
            //percentageTrendIcon.StrokeColor = isTrendingUp ? Color.Green : Color.Red;
            //trendIcon.FillColor = isTrendingUp ? Color.Green : Color.Red;
            //trendIcon.StrokeColor = isTrendingUp ? Color.Green : Color.Red;
            percentageLabel.ForeColor = isTrendingUp ? Color.Green : Color.Red;
        }

     

        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                headerLabel?.Dispose();
                percentageLabel?.Dispose();
                trendImage?.Dispose();
                valueLabel?.Dispose();
                trendLabel?.Dispose();
                cardIcon?.Dispose();
                infoLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}