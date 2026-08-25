using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Company.Models;
using TheTechIdea.Beep.Winform.Controls.Company.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Company
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
    public partial class BeepCompanyProfile : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.CompanyProfile;
        #region Private Fields
        private BeepLabel lblCompanyName = null!;
        private BeepLabel lblCompanyType = null!;
        private BeepTextBox lblCompanyDesc = null!;
        private BeepLabel lblLikes = null!;
        private BeepButton btnLikePage = null!;
        private BeepButton btnCompanyProfile = null!;
        private ContextMenuStrip dropdownMenu = null!;
        private BeepButton btnDropdown = null!;
        private BeepCircularButton btnCompanyLogo = null!;

        private CompanyProfileViewType _viewType = CompanyProfileViewType.Classic;
        private string _companyName = "Company Name";
        private string _companyType = "Type";
        private string _description = "Description";
        private string _likes = "0 Likes";
        private string _website = "www.example.com";
        private string _logoPath = "";

        private ICompanyProfilePainter _profilePainter = null!;
        private CompanyProfileLayout _layout = null!;
        #endregion

        #region Constructor
        public BeepCompanyProfile()
        {
            Padding = new Padding(2);

            _profilePainter = CreatePainter(_viewType);
            Size = _profilePainter.PreferredSize;

            InitializeComponents();
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

            // Calculate the initial layout for the default view.
            RecalculateLayout();
        }

        private void SetDummyData()
        {
            CompanyName = "Sample Inc.";
            CompanyType = "Software Development";
            Description = "An innovative company delivering cutting-edge software solutions.";
            Likes = "5,678 Likes";
            Website = "www.sampleinc.com";
            LogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
            Theme = "DefaultType";
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

        #region Layout
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Template controls do not exist yet during construction; skip relayout until then.
            if (lblCompanyName != null)
                RecalculateLayout();
        }

        private void RecalculateLayout()
        {
            _layout = _profilePainter.Layout(ClientSize);
            SetupHitTesting(_layout);
            Invalidate();
        }

        private ICompanyProfilePainter CreatePainter(CompanyProfileViewType viewType) => viewType switch
        {
            CompanyProfileViewType.Compact => new CompactCompanyProfilePainter(this),
            CompanyProfileViewType.Minimal => new MinimalCompanyProfilePainter(this),
            CompanyProfileViewType.SocialCard => new SocialCardCompanyProfilePainter(this),
            CompanyProfileViewType.Detailed => new DetailedCompanyProfilePainter(this),
            _ => new ClassicCompanyProfilePainter(this)
        };
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
                    _profilePainter = CreatePainter(value);
                    Size = _profilePainter.PreferredSize;
                    RecalculateLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public new string CompanyName
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

        #region Internal Accessors
        internal BeepLabel CompanyNameLabel => lblCompanyName;
        internal BeepLabel CompanyTypeLabel => lblCompanyType;
        internal BeepTextBox DescriptionBox => lblCompanyDesc;
        internal BeepLabel LikesLabel => lblLikes;
        internal BeepButton LikeButton => btnLikePage;
        internal BeepButton ProfileButton => btnCompanyProfile;
        internal BeepButton DropdownButton => btnDropdown;
        internal BeepCircularButton LogoButton => btnCompanyLogo;
        internal IBeepTheme CurrentTheme => _currentTheme;
        #endregion
    }
}