using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

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
        private Panel popoverPanel;
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

        public BeepCompanyProfile()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(400, 250);
            this.Padding = new Padding(2);

            popoverPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.None
            };

            lblCompanyName = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 12, FontStyle.Bold),
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
            };

            lblCompanyType = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 10, FontStyle.Regular),
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                AutoSize = true
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
                AutoSize = false
            };

            lblLikes = new BeepLabel
            {
                TextFont = new Font("Segoe UI", 9, FontStyle.Regular),
                IsChild = true,
                IsFrameless = true,
                AutoSize = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };

            linkWebsite = new LinkLabel
            {
                AutoSize = true
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
            btnDropdown.Click += (s, e) => dropdownMenu.Show(btnDropdown, new Point(0, btnDropdown.Height));

            btnCompanyLogo = new BeepCircularButton
            {
                Size = new Size(60, 60),
                IsFrameless = true,
                IsChild = true
            };

            this.Controls.Add(popoverPanel);
            ApplyViewType();
            ApplyTheme(EnumBeepThemes.DefaultTheme);
        }

        private void SetDummyData()
        {
            CompanyName = "Sample Inc.";
            CompanyType = "Software Development";
            Description = "A innovative company delivering cutting-edge software solutions.";
            Likes = "5,678 Likes";
            Website = "www.sampleinc.com";
            LogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
            Theme = EnumBeepThemes.DefaultTheme;
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

        private void ApplyViewType()
        {
            popoverPanel.Controls.Clear();
            lblCompanyName.Visible = true;
            lblCompanyType.Visible = true;
            lblCompanyDesc.Visible = true;
            lblLikes.Visible = true;
            linkWebsite.Visible = true;
            btnLikePage.Visible = true;
            btnCompanyProfile.Visible = true;
            btnDropdown.Visible = true;
            btnCompanyLogo.Visible = true;
            lblCompanyName.TextAlign = ContentAlignment.MiddleLeft;
            switch (_viewType)
            {
                case CompanyProfileViewType.Classic:
                    SetupClassicView();
                    break;
                case CompanyProfileViewType.Compact:
                    SetupCompactView();
                    break;
                case CompanyProfileViewType.Minimal:
                    SetupMinimalView();
                    break;
                case CompanyProfileViewType.SocialCard:
                    SetupSocialCardView();
                    break;
                case CompanyProfileViewType.Detailed:
                    SetupDetailedView();
                    break;
            }
            popoverPanel.Controls.AddRange(new Control[] { lblCompanyName, lblCompanyType, lblCompanyDesc, lblLikes, linkWebsite, btnLikePage, btnCompanyProfile, btnDropdown, btnCompanyLogo });
           
        }

        private void SetupClassicView()
        {
            this.Size = new Size(400, 350); // Adjusted height for better spacing
            int padding = popoverPanel.Padding.Left;

            btnCompanyLogo.Location = new Point(padding, padding); // 15, 15
            lblCompanyName.Location = new Point(padding, padding + btnCompanyLogo.Height + 10); // 15, 85
            lblCompanyType.Location = new Point(padding, padding + btnCompanyLogo.Height + 35); // 15, 110
            lblCompanyDesc.Size = new Size(this.Width - (2 * padding), 80); // 370, 80
            lblCompanyDesc.Location = new Point(padding, padding + btnCompanyLogo.Height + 60); // 15, 135
            lblLikes.Location = new Point(padding, padding + btnCompanyLogo.Height + 150); // 15, 225
            linkWebsite.Location = new Point(padding, padding + btnCompanyLogo.Height + 175); // 15, 250
            btnLikePage.Location = new Point(padding, padding + btnCompanyLogo.Height + 210); // 15, 285
            btnCompanyProfile.Location = new Point(padding + btnLikePage.Width + 10, padding + btnCompanyLogo.Height + 210); // 105, 285
            btnDropdown.Location = new Point(this.Width - padding - btnDropdown.Width - 10, padding); // 345, 15
        }

        private void SetupCompactView()
        {
            this.Size = new Size(250, 300); // Adjusted height for better spacing
            int padding = popoverPanel.Padding.Left;

            btnCompanyLogo.Location = new Point(padding, padding); // 15, 15
            lblCompanyName.Location = new Point(padding, padding + btnCompanyLogo.Height + 10); // 15, 85
            lblCompanyType.Location = new Point(padding, padding + btnCompanyLogo.Height + 35); // 15, 110
            lblCompanyDesc.Size = new Size(this.Width - (2 * padding), 50); // 220, 50
            lblCompanyDesc.Location = new Point(padding, padding + btnCompanyLogo.Height + 60); // 15, 135
            lblLikes.Location = new Point(padding, padding + btnCompanyLogo.Height + 120); // 15, 195
            linkWebsite.Location = new Point(padding, padding + btnCompanyLogo.Height + 145); // 15, 220
            btnLikePage.Location = new Point(padding, padding + btnCompanyLogo.Height + 175); // 15, 250
            btnCompanyProfile.Location = new Point(padding + btnLikePage.Width + 10, padding + btnCompanyLogo.Height + 175); // 105, 250
            btnDropdown.Location = new Point(this.Width - padding - btnDropdown.Width - 10, padding); // 195, 15
        }

        private void SetupMinimalView()
        {
            this.Size = new Size(300, 200);
            int padding = popoverPanel.Padding.Left;

            btnCompanyLogo.Location = new Point((this.Width - btnCompanyLogo.Width) / 2, padding); // Centered, 15
            lblCompanyName.Location = new Point(padding, padding + btnCompanyLogo.Height + 10); // 15, 85
            lblCompanyName.Size = new Size(this.Width - 2 * padding, 20); // 270, 20
           lblCompanyName.TextAlign = ContentAlignment.MiddleCenter;
            btnCompanyProfile.Location = new Point((this.Width - btnCompanyProfile.Width) / 2, padding + btnCompanyLogo.Height + 50); // Centered, 125

            lblCompanyType.Visible = false;
            lblCompanyDesc.Visible = false;
            lblLikes.Visible = false;
            linkWebsite.Visible = false;
            btnLikePage.Visible = false;
            btnDropdown.Visible = false;
        }

        private void SetupSocialCardView()
        {
            this.Size = new Size(300, 230);
            int padding = popoverPanel.Padding.Left;

            btnCompanyLogo.Location = new Point(padding, padding); // 15, 15
            lblCompanyName.Location = new Point(padding, padding + btnCompanyLogo.Height + 10); // 15, 85
            lblCompanyType.Location = new Point(padding, padding + btnCompanyLogo.Height + 35); // 15, 110
            lblCompanyDesc.Size = new Size(this.Width - (2 * padding), 40); // 270, 40
            lblCompanyDesc.Location = new Point(padding, padding + btnCompanyLogo.Height + 60); // 15, 135
            lblLikes.Location = new Point(padding, padding + btnCompanyLogo.Height + 110); // 15, 185
            linkWebsite.Location = new Point(padding + 100, padding + btnCompanyLogo.Height + 110); // 115, 185
            btnLikePage.Size = new Size(60, 25);
            btnLikePage.Location = new Point(padding, padding + btnCompanyLogo.Height + 140); // 15, 215
            btnCompanyProfile.Size = new Size(60, 25);
            btnCompanyProfile.Location = new Point(padding + btnLikePage.Width + 10, padding + btnCompanyLogo.Height + 140); // 85, 215
            btnDropdown.Location = new Point(this.Width - padding - btnDropdown.Width - 10, padding); // 245, 15
        }

        private void SetupDetailedView()
        {
            this.Size = new Size(500,350);
            int padding = popoverPanel.Padding.Left;

            btnCompanyLogo.Location = new Point(padding, padding); // 15, 15
            lblCompanyName.Location = new Point(padding, padding + btnCompanyLogo.Height + 10); // 15, 85
            lblCompanyType.Location = new Point(padding, padding + btnCompanyLogo.Height + 35); // 15, 110
            lblCompanyDesc.Size = new Size(this.Width - (2 * padding), 160); // 470, 160
            lblCompanyDesc.Location = new Point(padding, padding + btnCompanyLogo.Height + 60); // 15, 135
            lblLikes.Location = new Point(padding, padding + btnCompanyLogo.Height + 230); // 15, 305
            linkWebsite.Location = new Point(padding + 70, padding + btnCompanyLogo.Height + 230); // 85, 305
            btnLikePage.Location = new Point(padding, padding + btnCompanyLogo.Height + 260); // 15, 335
            btnCompanyProfile.Location = new Point(padding + btnLikePage.Width + 10, padding + btnCompanyLogo.Height + 260); // 105, 335
            btnDropdown.Location = new Point(this.Width - padding - btnDropdown.Width - 10, padding); // 445, 15
        }

        public override void ApplyTheme()
        {
            if (BeepThemesManager.ThemeCompanyProfileColors.TryGetValue(Theme, out var colors))
            {
                popoverPanel.BackColor = colors.CompanyPopoverBackgroundColor;
                lblCompanyName.ForeColor = colors.CompanyTitleColor;
                lblCompanyType.ForeColor = colors.CompanySubtitleColor;
                lblCompanyDesc.ForeColor = colors.CompanyDescriptionColor;
                lblLikes.ForeColor = colors.CompanyDescriptionColor;
                linkWebsite.LinkColor = colors.CompanyLinkColor;
                btnLikePage.BackColor = colors.CompanyButtonBackgroundColor;
                btnLikePage.ForeColor = colors.CompanyButtonTextColor;
                btnCompanyProfile.BackColor = colors.CompanyButtonBackgroundColor;
                btnCompanyProfile.ForeColor = colors.CompanyButtonTextColor;
                dropdownMenu.BackColor = colors.CompanyDropdownBackgroundColor;
                dropdownMenu.ForeColor = colors.CompanyDropdownTextColor;
                btnDropdown.BackColor = colors.CompanyButtonBackgroundColor;
                btnDropdown.ForeColor = colors.CompanyButtonTextColor;
                btnCompanyLogo.BackColor = colors.CompanyLogoBackgroundColor;
            }
            else
            {
                popoverPanel.BackColor = Color.White;
                lblCompanyName.ForeColor = Color.Black;
                lblCompanyType.ForeColor = Color.DarkBlue;
                lblCompanyDesc.ForeColor = Color.Gray;
                lblLikes.ForeColor = Color.Gray;
                linkWebsite.LinkColor = Color.Blue;
                btnLikePage.BackColor = Color.Blue;
                btnLikePage.ForeColor = Color.White;
                btnCompanyProfile.BackColor = Color.Blue;
                btnCompanyProfile.ForeColor = Color.White;
                dropdownMenu.BackColor = Color.White;
                dropdownMenu.ForeColor = Color.Black;
                btnDropdown.BackColor = Color.Blue;
                btnDropdown.ForeColor = Color.White;
                btnCompanyLogo.BackColor = Color.Gray;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between different company profile views.")]
        public CompanyProfileViewType ViewType
        {
            get => _viewType;
            set
            {
                _viewType = value;
                ApplyViewType();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyName
        {
            get => lblCompanyName.Text;
            set => lblCompanyName.Text = value ?? "Company Name";
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyType
        {
            get => lblCompanyType.Text;
            set => lblCompanyType.Text = value ?? "Type";
        }

        [Browsable(true)]
        [Category("Data")]
        public string Description
        {
            get => lblCompanyDesc.Text;
            set => lblCompanyDesc.Text = value ?? "Description";
        }

        [Browsable(true)]
        [Category("Data")]
        public string Likes
        {
            get => lblLikes.Text;
            set => lblLikes.Text = value ?? "0 Likes";
        }

        [Browsable(true)]
        [Category("Data")]
        public string Website
        {
            get => linkWebsite.Text;
            set => linkWebsite.Text = value ?? "www.example.com";
        }

        [Browsable(true)]
        [Category("Data")]
        public string LogoPath
        {
            get => btnCompanyLogo.ImagePath;
            set => btnCompanyLogo.ImagePath = value;
        }
    }
}