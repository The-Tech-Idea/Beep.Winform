using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum CompanyProfileViewType
    {
        Classic,
        Compact,
        Minimal,
        SocialCard,
        Detailed
    }

    [Browsable(true)]
    [Category("UI")]
    [Description("A control that displays a company profile.")]
    [DisplayName("Beep Company Profile")]
    public class BeepCompanyProfile : BeepControl
    {
        #region Private Fields
        private BeepLabel lblCompanyName;
        private BeepLabel lblCompanyType;
        private BeepTextBox lblCompanyDesc;
        private BeepLabel lblLikes;
        private LinkLabel linkWebsite;
        private BeepButton btnLikePage;
        private BeepButton btnCompanyProfile;
        private ContextMenuStrip dropdownMenu;
        private BeepButton btnDropdown;
        private BeepCircularButton btnCompanyLogo;

        private CompanyProfileViewType _viewType = CompanyProfileViewType.Classic;
        private string _companyName = "Company Name";
        private string _companyType = "Type";
        private string _description = "Description";
        private string _likes = "0 Likes";
        private string _website = "www.example.com";
        private string _logoPath = "";

        // Layout rectangles for direct drawing
        private Rectangle logoRect;
        private Rectangle nameRect;
        private Rectangle typeRect;
        private Rectangle descRect;
        private Rectangle likesRect;
        private Rectangle websiteRect;
        private Rectangle likeButtonRect;
        private Rectangle profileButtonRect;
        private Rectangle dropdownRect;

        // Hit test areas
        private ControlHitTest logoHitTest;
        private ControlHitTest likeButtonHitTest;
        private ControlHitTest profileButtonHitTest;
        private ControlHitTest websiteHitTest;
        private ControlHitTest dropdownHitTest;
        #endregion

        #region Constructor
        public BeepCompanyProfile()
        {
            this.Size = new Size(400, 350);
            this.Padding = new Padding(2);

            InitializeComponents();
            SetupHitTesting();
        }
        #endregion

        #region Component Initialization
        private void InitializeComponents()
        {
            // Initialize components but don't add them to controls collection
            // They will be used as "templates" for drawing

            lblCompanyName = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 12, FontStyle.Bold),
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Text = _companyName
            };

            lblCompanyType = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 10, FontStyle.Regular),
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true,
                Text = _companyType
            };

            lblCompanyDesc = new BeepTextBox
            {
                TextFont = new Font("Segoe UI", 9, FontStyle.Regular),
                IsChild = true,
                IsFrameless = true,
                Multiline = true,
                ScrollBars = ScrollBars.None,
                ReadOnly = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = false,
                Text = _description
            };

            lblLikes = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 9, FontStyle.Regular),
                IsChild = true,
                IsFrameless = true,
                AutoSize = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Text = _likes
            };

            linkWebsite = new LinkLabel
            {
                AutoSize = true,
                Text = _website
            };

            btnLikePage = new BeepButton
            {
                Text = "Like",
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Size = new Size(80, 30)
            };

            btnCompanyProfile = new BeepButton
            {
                Text = "Profile",
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Size = new Size(80, 30)
            };

            dropdownMenu = new ContextMenuStrip();
            dropdownMenu.Items.Add("Option 1");
            dropdownMenu.Items.Add("Option 2");

            btnDropdown = new BeepButton
            {
                Text = "•••",
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsRounded = false,
                Size = new Size(30, 30)
            };

            btnCompanyLogo = new BeepCircularButton
            {
                Size = new Size(60, 60),
                IsFrameless = true,
                IsChild = true
            };

            // Apply theme to components
            ApplyTheme();

            // Calculate layout based on view type
            CalculateLayout();
        }

        private void SetDummyData()
        {
            CompanyName = "Sample Inc.";
            CompanyType = "Software Development";
            Description = "An innovative company delivering cutting-edge software solutions.";
            Likes = "5,678 Likes";
            Website = "www.sampleinc.com";
            LogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
            Theme = "DefaultTheme";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                SetDummyData();
            }
        }

        public void BeginInit() { }

        public void EndInit()
        {
            if (DesignMode)
            {
                SetDummyData();
            }
        }
        #endregion

        #region Layout Calculation
        private void CalculateLayout()
        {
            // Calculate layout based on the current view type
            switch (_viewType)
            {
                case CompanyProfileViewType.Classic:
                    CalculateClassicLayout();
                    break;
                case CompanyProfileViewType.Compact:
                    CalculateCompactLayout();
                    break;
                case CompanyProfileViewType.Minimal:
                    CalculateMinimalLayout();
                    break;
                case CompanyProfileViewType.SocialCard:
                    CalculateSocialCardLayout();
                    break;
                case CompanyProfileViewType.Detailed:
                    CalculateDetailedLayout();
                    break;
            }

            // Update hit testing areas
            SetupHitTesting();

            // Force redraw
            Invalidate();
        }

        private void CalculateClassicLayout()
        {
            int padding = 15;

            // Set the size for this view
            this.Size = new Size(400, 350);

            logoRect = new Rectangle(
                padding, padding,
                60, 60);

            nameRect = new Rectangle(
                padding,
                padding + logoRect.Height + 10,
                Width - (2 * padding),
                25);

            typeRect = new Rectangle(
                padding,
                nameRect.Bottom + 10,
                Width - (2 * padding),
                20);

            descRect = new Rectangle(
                padding,
                typeRect.Bottom + 10,
                Width - (2 * padding),
                80);

            likesRect = new Rectangle(
                padding,
                descRect.Bottom + 10,
                Width / 2 - padding,
                20);

            websiteRect = new Rectangle(
                padding,
                likesRect.Bottom + 10,
                Width - (2 * padding),
                20);

            likeButtonRect = new Rectangle(
                padding,
                websiteRect.Bottom + 20,
                80,
                30);

            profileButtonRect = new Rectangle(
                likeButtonRect.Right + 10,
                websiteRect.Bottom + 20,
                80,
                30);

            dropdownRect = new Rectangle(
                Width - padding - 30,
                padding,
                30,
                30);
        }

        private void CalculateCompactLayout()
        {
            int padding = 15;

            // Set the size for this view
            this.Size = new Size(250, 300);

            logoRect = new Rectangle(
                padding, padding,
                60, 60);

            nameRect = new Rectangle(
                padding,
                padding + logoRect.Height + 10,
                Width - (2 * padding),
                25);

            typeRect = new Rectangle(
                padding,
                nameRect.Bottom + 10,
                Width - (2 * padding),
                20);

            descRect = new Rectangle(
                padding,
                typeRect.Bottom + 10,
                Width - (2 * padding),
                50);

            likesRect = new Rectangle(
                padding,
                descRect.Bottom + 10,
                Width / 2 - padding,
                20);

            websiteRect = new Rectangle(
                padding,
                likesRect.Bottom + 10,
                Width - (2 * padding),
                20);

            likeButtonRect = new Rectangle(
                padding,
                websiteRect.Bottom + 10,
                80,
                30);

            profileButtonRect = new Rectangle(
                likeButtonRect.Right + 10,
                websiteRect.Bottom + 10,
                80,
                30);

            dropdownRect = new Rectangle(
                Width - padding - 30,
                padding,
                30,
                30);
        }

        private void CalculateMinimalLayout()
        {
            int padding = 15;

            // Set the size for this view
            this.Size = new Size(300, 200);

            // Center the logo
            logoRect = new Rectangle(
                (Width - 60) / 2,
                padding,
                60, 60);

            nameRect = new Rectangle(
                padding,
                logoRect.Bottom + 10,
                Width - (2 * padding),
                25);

            // Center the profile button
            profileButtonRect = new Rectangle(
                (Width - 80) / 2,
                nameRect.Bottom + 20,
                80,
                30);

            // Hide other elements in minimal view
            typeRect = Rectangle.Empty;
            descRect = Rectangle.Empty;
            likesRect = Rectangle.Empty;
            websiteRect = Rectangle.Empty;
            likeButtonRect = Rectangle.Empty;
            dropdownRect = Rectangle.Empty;
        }

        private void CalculateSocialCardLayout()
        {
            int padding = 15;

            // Set the size for this view
            this.Size = new Size(300, 230);

            logoRect = new Rectangle(
                padding, padding,
                60, 60);

            nameRect = new Rectangle(
                padding,
                padding + logoRect.Height + 10,
                Width - (2 * padding),
                25);

            typeRect = new Rectangle(
                padding,
                nameRect.Bottom + 5,
                Width - (2 * padding),
                20);

            descRect = new Rectangle(
                padding,
                typeRect.Bottom + 5,
                Width - (2 * padding),
                40);

            likesRect = new Rectangle(
                padding,
                descRect.Bottom + 10,
                Width / 3 - padding,
                20);

            websiteRect = new Rectangle(
                likesRect.Right + 10,
                descRect.Bottom + 10,
                Width - likesRect.Right - padding - 10,
                20);

            // Smaller buttons for social card
            likeButtonRect = new Rectangle(
                padding,
                websiteRect.Bottom + 10,
                60,
                25);

            profileButtonRect = new Rectangle(
                likeButtonRect.Right + 10,
                websiteRect.Bottom + 10,
                60,
                25);

            dropdownRect = new Rectangle(
                Width - padding - 30,
                padding,
                30,
                30);
        }

        private void CalculateDetailedLayout()
        {
            int padding = 15;

            // Set the size for this view
            this.Size = new Size(500, 350);

            logoRect = new Rectangle(
                padding, padding,
                60, 60);

            nameRect = new Rectangle(
                padding,
                padding + logoRect.Height + 10,
                Width - (2 * padding),
                30);

            typeRect = new Rectangle(
                padding,
                nameRect.Bottom + 10,
                Width - (2 * padding),
                20);

            // Larger description area for detailed view
            descRect = new Rectangle(
                padding,
                typeRect.Bottom + 10,
                Width - (2 * padding),
                160);

            likesRect = new Rectangle(
                padding,
                descRect.Bottom + 10,
                Width / 4,
                20);

            websiteRect = new Rectangle(
                likesRect.Right + 20,
                descRect.Bottom + 10,
                Width - likesRect.Right - padding - 20,
                20);

            likeButtonRect = new Rectangle(
                padding,
                websiteRect.Bottom + 20,
                80,
                30);

            profileButtonRect = new Rectangle(
                likeButtonRect.Right + 10,
                websiteRect.Bottom + 20,
                80,
                30);

            dropdownRect = new Rectangle(
                Width - padding - 30,
                padding,
                30,
                30);
        }
        #endregion

        #region Hit Testing
        private void SetupHitTesting()
        {
            // Clear existing hit areas
            ClearHitList();

            // Setup hit areas based on view type
            switch (_viewType)
            {
                case CompanyProfileViewType.Classic:
                case CompanyProfileViewType.Compact:
                case CompanyProfileViewType.Detailed:
                case CompanyProfileViewType.SocialCard:
                    SetupStandardHitTesting();
                    break;

                case CompanyProfileViewType.Minimal:
                    SetupMinimalHitTesting();
                    break;
            }
        }
        private void SetupStandardHitTesting()
        {
            // Company logo hit area
            AddHitArea("CompanyLogo", logoRect, btnCompanyLogo, () => {
                // Handle logo click (for example, open a detailed view)
            });

            // Like button hit area
            if (!likeButtonRect.IsEmpty)
            {
                AddHitArea("LikeButton", likeButtonRect, btnLikePage, () => {
                    // Handle like action
                });
            }

            // Profile button hit area
            if (!profileButtonRect.IsEmpty)
            {
                AddHitArea("ProfileButton", profileButtonRect, btnCompanyProfile, () => {
                    // Handle profile action
                });
            }

            // Website link hit area
            if (!websiteRect.IsEmpty)
            {
                AddHitArea("WebsiteLink", websiteRect, null, () => {
                    // Open website URL
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Website.StartsWith("http") ? Website : "https://" + Website,
                            UseShellExecute = true
                        });
                    }
                    catch { /* Handle exceptions */ }
                });
            }

            // Dropdown menu hit area
            if (!dropdownRect.IsEmpty)
            {
                AddHitArea("DropdownMenu", dropdownRect, btnDropdown, () => {
                    // Show dropdown menu
                    if (dropdownMenu != null)
                    {
                        dropdownMenu.Show(this, new Point(dropdownRect.X, dropdownRect.Bottom));
                    }
                });
            }
        }

        private void SetupMinimalHitTesting()
        {
            // Company logo hit area
            AddHitArea("CompanyLogo", logoRect, btnCompanyLogo, () => {
                // Handle logo click
            });

            // Profile button hit area (only button shown in minimal view)
            if (!profileButtonRect.IsEmpty)
            {
                AddHitArea("ProfileButton", profileButtonRect, btnCompanyProfile, () => {
                    // Handle profile action
                });
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Handle click using hit testing
            HitTest(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Invalidate to update any hover effects
            Invalidate();
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Draw the company profile
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
                return;

            // Set high quality rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw background
            Color backgroundFill = _currentTheme.CompanyPopoverBackgroundColor != Color.Empty
                ? _currentTheme.CompanyPopoverBackgroundColor
                : _currentTheme.CardBackColor;

            using (SolidBrush brush = new SolidBrush(backgroundFill))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            // Draw components based on view type
            switch (_viewType)
            {
                case CompanyProfileViewType.Classic:
                case CompanyProfileViewType.Compact:
                case CompanyProfileViewType.Detailed:
                    DrawStandardView(graphics);
                    break;

                case CompanyProfileViewType.Minimal:
                    DrawMinimalView(graphics);
                    break;

                case CompanyProfileViewType.SocialCard:
                    DrawSocialCardView(graphics);
                    break;
            }
        }

        private void DrawStandardView(Graphics graphics)
        {
            // Draw company logo
            if (!logoRect.IsEmpty && btnCompanyLogo != null)
            {
                btnCompanyLogo.Draw(graphics, logoRect);
            }

            // Draw company name
            if (!nameRect.IsEmpty && lblCompanyName != null)
            {
                lblCompanyName.Draw(graphics, nameRect);
            }

            // Draw company type
            if (!typeRect.IsEmpty && lblCompanyType != null)
            {
                lblCompanyType.Draw(graphics, typeRect);
            }

            // Draw company description
            if (!descRect.IsEmpty && lblCompanyDesc != null)
            {
                lblCompanyDesc.Draw(graphics, descRect);
            }

            // Draw likes
            if (!likesRect.IsEmpty && lblLikes != null)
            {
                lblLikes.Draw(graphics, likesRect);
            }

            // Draw website link
            if (!websiteRect.IsEmpty)
            {
                // Custom drawing for LinkLabel since it doesn't inherit from BeepControl
                Font linkFont;

                // Use company-specific link font if available
                if (UseThemeFont && _currentTheme.CompanyLinkFont != null)
                {
                    linkFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyLinkFont);
                }
                // Fall back to general link style
                else if (UseThemeFont && _currentTheme.LinkStyle != null)
                {
                    linkFont = BeepThemesManager.ToFont(_currentTheme.LinkStyle);
                }
                // Default fallback
                else
                {
                    linkFont = new Font("Segoe UI", 9, FontStyle.Underline);
                }

                Color linkColor = _currentTheme.CompanyLinkColor != Color.Empty
                    ? _currentTheme.CompanyLinkColor
                    : _currentTheme.LinkColor;

                using (SolidBrush brush = new SolidBrush(linkColor))
                {
                    graphics.DrawString(Website, linkFont, brush, websiteRect);
                }
            }

            // Draw like button
            if (!likeButtonRect.IsEmpty && btnLikePage != null)
            {
                btnLikePage.Draw(graphics, likeButtonRect);
            }

            // Draw profile button
            if (!profileButtonRect.IsEmpty && btnCompanyProfile != null)
            {
                btnCompanyProfile.Draw(graphics, profileButtonRect);
            }

            // Draw dropdown button
            if (!dropdownRect.IsEmpty && btnDropdown != null)
            {
                btnDropdown.Draw(graphics, dropdownRect);
            }
        }

        private void DrawMinimalView(Graphics graphics)
        {
            // In minimal view, we only draw logo, name, and profile button

            // Draw company logo centered
            if (!logoRect.IsEmpty && btnCompanyLogo != null)
            {
                btnCompanyLogo.Draw(graphics, logoRect);
            }

            // Draw company name with centered text alignment
            if (!nameRect.IsEmpty && lblCompanyName != null)
            {
                // Override text alignment for minimal view
                lblCompanyName.TextAlign = ContentAlignment.MiddleCenter;
                lblCompanyName.Draw(graphics, nameRect);
            }

            // Draw profile button centered
            if (!profileButtonRect.IsEmpty && btnCompanyProfile != null)
            {
                btnCompanyProfile.Draw(graphics, profileButtonRect);
            }
        }

        private void DrawSocialCardView(Graphics graphics)
        {
            // Draw social card view which is similar to standard but with more compact layout
            DrawStandardView(graphics);
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null)
                return;

            // Apply theme to base control
            base.ApplyTheme();

            // Apply theme to company name label
            if (lblCompanyName != null)
            {
                lblCompanyName.Theme = Theme;

                // Set company title color - use specialized color if available
                lblCompanyName.ForeColor = _currentTheme.CompanyTitleColor != Color.Empty
                    ? _currentTheme.CompanyTitleColor
                    : _currentTheme.TitleStyle?.TextColor ?? _currentTheme.PrimaryTextColor;

                // Apply font styling - use company specific font if available
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyTitleFont != null)
                    {
                        lblCompanyName.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyTitleFont);
                    }
                    else if (_currentTheme.TitleStyle != null)
                    {
                        lblCompanyName.TextFont = BeepThemesManager.ToFont(_currentTheme.TitleStyle);
                    }
                }
            }

            // Apply theme to company type label
            if (lblCompanyType != null)
            {
                lblCompanyType.Theme = Theme;

                // Set company subtitle color - use specialized color if available
                lblCompanyType.ForeColor = _currentTheme.CompanySubtitleColor != Color.Empty
                    ? _currentTheme.CompanySubtitleColor
                    : _currentTheme.SubtitleStyle?.TextColor ?? _currentTheme.SecondaryTextColor;

                // Apply font styling - use company specific font if available
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanySubTitleFont != null)
                    {
                        lblCompanyType.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanySubTitleFont);
                    }
                    else if (_currentTheme.SubtitleStyle != null)
                    {
                        lblCompanyType.TextFont = BeepThemesManager.ToFont(_currentTheme.SubtitleStyle);
                    }
                }
            }

            // Apply theme to company description textbox
            if (lblCompanyDesc != null)
            {
                lblCompanyDesc.Theme = Theme;

                // Set company description color - use specialized color if available
                lblCompanyDesc.ForeColor = _currentTheme.CompanyDescriptionColor != Color.Empty
                    ? _currentTheme.CompanyDescriptionColor
                    : _currentTheme.TextBoxForeColor;

                // Set company background color
                lblCompanyDesc.BackColor = _currentTheme.CompanyPopoverBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyPopoverBackgroundColor
                    : _currentTheme.CardBackColor;

                // Apply font styling - use company specific font if available
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyDescriptionFont != null)
                    {
                        lblCompanyDesc.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyDescriptionFont);
                    }
                    else if (_currentTheme.BodyStyle != null)
                    {
                        lblCompanyDesc.TextFont = BeepThemesManager.ToFont(_currentTheme.BodyStyle);
                    }
                }
            }

            // Apply theme to likes label
            if (lblLikes != null)
            {
                lblLikes.Theme = Theme;
                lblLikes.ForeColor = _currentTheme.CompanyDescriptionColor != Color.Empty
                    ? _currentTheme.CompanyDescriptionColor
                    : _currentTheme.CaptionStyle?.TextColor ?? _currentTheme.SecondaryTextColor;

                // Use description font for likes as well since they're typically smaller metadata
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyDescriptionFont != null)
                    {
                        lblLikes.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyDescriptionFont);
                    }
                    else if (_currentTheme.CaptionStyle != null)
                    {
                        lblLikes.TextFont = BeepThemesManager.ToFont(_currentTheme.CaptionStyle);
                    }
                }
            }

            // Apply theme to website link
            if (linkWebsite != null)
            {
                linkWebsite.LinkColor = _currentTheme.CompanyLinkColor != Color.Empty
                    ? _currentTheme.CompanyLinkColor
                    : _currentTheme.LinkStyle?.TextColor ?? _currentTheme.LinkColor;

                // Apply font styling - use company specific font if available
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyLinkFont != null)
                    {
                        linkWebsite.Font = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyLinkFont);
                    }
                    else if (_currentTheme.LinkStyle != null)
                    {
                        linkWebsite.Font = BeepThemesManager.ToFont(_currentTheme.LinkStyle);
                    }
                }
            }

            // Apply theme to like button
            if (btnLikePage != null)
            {
                btnLikePage.Theme = Theme;
                btnLikePage.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnLikePage.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                // Apply button font styling
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                    {
                        btnLikePage.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    }
                    else if (_currentTheme.ButtonStyle != null)
                    {
                        btnLikePage.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    }
                }
            }

            // Apply theme to profile button
            if (btnCompanyProfile != null)
            {
                btnCompanyProfile.Theme = Theme;
                btnCompanyProfile.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnCompanyProfile.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                // Apply button font styling
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                    {
                        btnCompanyProfile.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    }
                    else if (_currentTheme.ButtonStyle != null)
                    {
                        btnCompanyProfile.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    }
                }
            }

            // Apply theme to dropdown menu
            if (dropdownMenu != null)
            {
                dropdownMenu.BackColor = _currentTheme.CompanyDropdownBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyDropdownBackgroundColor
                    : _currentTheme.MenuBackColor;
                dropdownMenu.ForeColor = _currentTheme.CompanyDropdownTextColor != Color.Empty
                    ? _currentTheme.CompanyDropdownTextColor
                    : _currentTheme.MenuForeColor;

                // Apply font to dropdown menu items
                if (UseThemeFont)
                {
                    Font menuFont = null;

                    // Try to use company specific button font for menu items
                    if (_currentTheme.CompanyButtonFont != null)
                    {
                        menuFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    }
                    // Fall back to general menu item font
                    else if (_currentTheme.MenuItemUnSelectedFont != null)
                    {
                        menuFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                    }

                    if (menuFont != null)
                    {
                        foreach (ToolStripItem item in dropdownMenu.Items)
                        {
                            item.Font = menuFont;
                        }
                    }
                }
            }

            // Apply theme to dropdown button
            if (btnDropdown != null)
            {
                btnDropdown.Theme = Theme;
                btnDropdown.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnDropdown.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                // Apply button font styling
                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                    {
                        btnDropdown.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    }
                    else if (_currentTheme.ButtonStyle != null)
                    {
                        btnDropdown.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    }
                }
            }

            // Apply theme to company logo button
            if (btnCompanyLogo != null)
            {
                btnCompanyLogo.Theme = Theme;
                btnCompanyLogo.BackColor = _currentTheme.CompanyLogoBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyLogoBackgroundColor
                    : _currentTheme.AccentColor; // Use accent color as fallback for logo background

                // Set the logo image path if provided
                if (!string.IsNullOrEmpty(_logoPath))
                {
                    btnCompanyLogo.ImagePath = _logoPath;
                }
            }

            // Force redraw with the new theme
            Invalidate();
        }
        #endregion

        #region Public Properties
        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between different company profile views.")]
        public CompanyProfileViewType ViewType
        {
            get => _viewType;
            set
            {
                if (_viewType != value)
                {
                    _viewType = value;
                    CalculateLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value ?? "Company Name";
                if (lblCompanyName != null)
                {
                    lblCompanyName.Text = _companyName;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyType
        {
            get => _companyType;
            set
            {
                _companyType = value ?? "Type";
                if (lblCompanyType != null)
                {
                    lblCompanyType.Text = _companyType;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Description
        {
            get => _description;
            set
            {
                _description = value ?? "Description";
                if (lblCompanyDesc != null)
                {
                    lblCompanyDesc.Text = _description;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Likes
        {
            get => _likes;
            set
            {
                _likes = value ?? "0 Likes";
                if (lblLikes != null)
                {
                    lblLikes.Text = _likes;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Website
        {
            get => _website;
            set
            {
                _website = value ?? "www.example.com";
                if (linkWebsite != null)
                {
                    linkWebsite.Text = _website;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string LogoPath
        {
            get => _logoPath;
            set
            {
                _logoPath = value;
                if (btnCompanyLogo != null)
                {
                    btnCompanyLogo.ImagePath = _logoPath;
                }
                Invalidate();
            }
        }
        #endregion
    }
}