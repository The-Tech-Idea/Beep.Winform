using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
 

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTestimonial : BeepControl
    {
        private Panel cardPanel;
        private BeepCircularButton image;
        private BeepCircularButton companyLogo;
        private BeepLabel lblTestimonial;
        private BeepLabel lblName;
        private BeepLabel lblUsername;
        private BeepLabel lblPosition;
        private BeepButton btnClose;
        private FlowLayoutPanel ratingStars;

        private TestimonialViewType _viewType = TestimonialViewType.Classic;

        public BeepTestimonial()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(350, 200);
            this.Padding = new Padding(10);

            cardPanel = new Panel
            {
                Size = this.Size,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.None,
                BackColor = Color.LightGray // Debugging visibility
            };

            image = new BeepCircularButton
            {
                Size = new Size(50, 50),
                IsFrameless = true,
                IsChild = true
            };

            companyLogo = new BeepCircularButton
            {
                Size = new Size(60, 30),
                IsFrameless = true,
                IsChild = true
            };

            lblTestimonial = new BeepLabel
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                IsFrameless = true,
                IsChild = true,
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.White
            };

            lblName = new BeepLabel
            {
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                IsFrameless = true,
                IsChild = true,
                AutoSize = true,
                BackColor = Color.White
            };

            lblUsername = new BeepLabel
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                IsFrameless = true,
                IsChild = true,
                AutoSize = true,
                BackColor = Color.White
            };

            lblPosition = new BeepLabel
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                IsFrameless = true,
                IsChild = true,
                AutoSize = true,
                BackColor = Color.White
            };

            ratingStars = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            btnClose = new BeepButton
            {
                Text = "✕",
                Size = new Size(24, 24),
                IsFrameless = true,
                IsChild = true
            };
            btnClose.Click += (s, e) => this.Visible = false;

            this.Controls.Add(cardPanel);
            ApplyViewType();

            // Initial text for debugging
            lblTestimonial.Text = "This is a testimonial.";
            lblName.Text = "John Doe";
            lblUsername.Text = "@johndoe";
            lblPosition.Text = "Developer";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                SetDummyData();
            }
            ApplyTheme(); // Apply theme after initialization
        }

        public void BeginInit() { }
        public void EndInit()
        {
            if (DesignMode)
            {
                SetDummyData();
            }
            ApplyTheme();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (cardPanel == null) return;
                this.BackColor = _currentTheme.TestimonialBackColor;
                cardPanel.BackColor = _currentTheme.TestimonialBackColor;

                lblTestimonial.BackColor = _currentTheme.TestimonialBackColor;
                lblTestimonial.ForeColor = _currentTheme.TestimonialTextColor;

                lblName.BackColor = _currentTheme.TestimonialBackColor;
                lblName.ForeColor = _currentTheme.TestimonialNameColor;

                lblUsername.BackColor = _currentTheme.TestimonialBackColor;
                lblUsername.ForeColor = _currentTheme.TestimonialDetailsColor;

                lblPosition.BackColor = _currentTheme.TestimonialBackColor;
                lblPosition.ForeColor = _currentTheme.TestimonialDetailsColor;

                btnClose.BackColor = _currentTheme.TestimonialBackColor;
                btnClose.ForeColor = _currentTheme.TestimonialDetailsColor;
            
                UpdateRatingStars(); // Ensure stars use theme colors
          
        }

        private void SetDummyData()
        {
            ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
            CompanyLogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.kitty.svg";
            Testimonial = "This product is amazing!";
            Name = "Nick Parsons";
            Username = "@nickparsons";
            Position = "Director of Marketing";
            Rating = "5";
        }

        private void UpdateRatingStars()
        {
            ratingStars.Controls.Clear();
            int starCount = int.TryParse(Rating, out int count) ? count : 0;
            for (int i = 0; i < 5; i++)
            {
                Label star = new Label
                {
                    Text = i < starCount ? "★" : "☆",
                    ForeColor = _currentTheme.TestimonialRatingColor,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    AutoSize = true
                };
                ratingStars.Controls.Add(star);
            }
        }

        private void ApplyViewType()
        {
            cardPanel.Controls.Clear();
            switch (_viewType)
            {
                case TestimonialViewType.Classic:
                    SetupClassicView();
                    break;
                case TestimonialViewType.Minimal:
                    SetupMinimalView();
                    break;
                case TestimonialViewType.Compact:
                    SetupCompactView();
                    break;
                case TestimonialViewType.SocialCard:
                    SetupSocialCardView();
                    break;
            }
            ApplyTheme(); // Reapply theme after layout change
        }

        private void SetupClassicView()
        {
            this.Size = new Size(350, 200);
            cardPanel.Size = this.Size;

            ratingStars.Location = new Point(15, 15);
            lblTestimonial.Size = new Size(310, 60);
            lblTestimonial.Location = new Point(15, 40);
            image.Location = new Point(15, 110);
            lblName.Location = new Point(75, 110);
            lblPosition.Location = new Point(75, 130);
            lblUsername.Location = new Point(75, 150);

            cardPanel.Controls.AddRange(new Control[] { ratingStars, lblTestimonial, image, lblName, lblPosition, lblUsername });
        }

        private void SetupMinimalView()
        {
            this.Size = new Size(350, 300);
            cardPanel.Size = this.Size;

            // Center the company logo at the top
            companyLogo.Location = new Point((this.Width - companyLogo.Width) / 2, 15);

            // Put the testimonial label near the top; you already set its size to 310×80
            lblTestimonial.Location = new Point((this.Width - lblTestimonial.Width) / 2, 50);

           

            // After the text is set, you can center them:
            lblName.Location = new Point((this.Width - lblName.PreferredSize.Width) / 2, 140);
            lblPosition.Location = new Point((this.Width - lblPosition.PreferredSize.Width) / 2, 160);
            lblUsername.Location = new Point((this.Width - lblUsername.PreferredSize.Width) / 2, 180);
            // Finally, place the image below the username
            image.Location = new Point(
                (cardPanel.Width - image.Width) / 2,
                lblUsername.Bottom + 15
            );
            // Finally, add them all to the panel
            cardPanel.Controls.AddRange(new Control[] {
        companyLogo, lblTestimonial, image, lblName, lblPosition, lblUsername
    });
        }

        private void SetupCompactView()
        {
            this.Size = new Size(300, 150);
            cardPanel.Size = this.Size;

            lblTestimonial.Size = new Size(270, 40);
            lblTestimonial.Location = new Point(15, 15);
            image.Size = new Size(40, 40);
            image.Location = new Point(15, 70);
            lblName.Location = new Point(65, 70);
            lblPosition.Location = new Point(65, 90);
            lblUsername.Location = new Point(65, 110);

            cardPanel.Controls.AddRange(new Control[] { lblTestimonial, image, lblName, lblPosition, lblUsername });
        }

        private void SetupSocialCardView()
        {
            this.Size = new Size(320, 160);
            cardPanel.Size = this.Size;

            image.Size = new Size(40, 40);
            image.Location = new Point(15, 15);
            lblName.Location = new Point(65, 15);
            lblUsername.Location = new Point(65, 35);
            lblPosition.Location = new Point(65, 55);
            lblTestimonial.Size = new Size(290, 60);
            lblTestimonial.Location = new Point(15, 75);
            btnClose.Location = new Point(280, 15);

            cardPanel.Controls.AddRange(new Control[] { image, lblName, lblUsername, lblPosition, lblTestimonial, btnClose });
        }

        // Properties
        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between different testimonial views.")]
        public TestimonialViewType ViewType
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
        public string ImagePath
        {
            get => image.ImagePath;
            set => image.ImagePath = value;
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyLogoPath
        {
            get => companyLogo.ImagePath;
            set => companyLogo.ImagePath = value;
        }

        [Browsable(true)]
        [Category("Data")]
        public string Testimonial
        {
            get => lblTestimonial.Text;
            set
            {
                lblTestimonial.Text = value ?? "Default Testimonial";
                ApplyViewType();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Name
        {
            get => lblName.Text;
            set
            {
                lblName.Text = value ?? "Anonymous";
                ApplyViewType();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Username
        {
            get => lblUsername.Text;
            set
            {
                lblUsername.Text = value ?? "@username";
                ApplyViewType();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Position
        {
            get => lblPosition.Text;
            set
            {
                lblPosition.Text = value ?? "Unknown Position";
                ApplyViewType();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Rating
        {
            get => lblTestimonial?.Tag?.ToString() ?? "5";
            set
            {
                lblTestimonial.Tag = value;
                UpdateRatingStars();
            }
        }
    }
}