using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum LoginViewType
    {
        Simple,
        Compact,
        Minimal,
        Social,
        SocialView2,
        Modern,
        Avatar,
        Extended,
        Full
    }

    [Browsable(true)]
    [Category("UI")]
    [Description("A control that displays a Login.")]
    [DisplayName("Beep Login")]
    public class BeepLogin : BeepControl
    {
        private Panel loginPanel;
        private BeepLabel lblTitle;
        private BeepLabel lblSubtitle;
        private BeepTextBox txtUsername;
        private BeepTextBox txtPassword;
        private BeepButton btnLogin;
        private LinkLabel lnkForgotPassword;
        private LinkLabel lnkRegister;
        private BeepCircularButton btnLogo;
        private BeepCircularButton btnAvatar;
        private BeepButton btnGoogleLogin;
        private BeepButton btnFacebookLogin;
        private BeepButton btnTwitterLogin;
        private BeepCheckBoxBool chkRememberMe;

        private LoginViewType _viewType = LoginViewType.Simple;

        public BeepLogin()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Basic control settings with modern defaults
            this.Size = new Size(300, 200);
            this.Padding = new Padding(5);

            // Set modern gradient defaults
            UseGradientBackground = true;
            ModernGradientType = ModernGradientType.Subtle;
            IsRounded = true;
             BorderRadius = 12;
          //  ShowShadow = true;

            // Main panel
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };

            // Create all child controls with improved font sizes for better readability
            lblTitle = new BeepLabel
            {
                Font = new Font("Segoe UI", 16, FontStyle.Bold), // Increased from 14
                IsChild = true,
                IsFrameless = true,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.None,
                UseGradientBackground = false // Labels don't need gradients
            };

            lblSubtitle = new BeepLabel
            {
                Font = new Font("Segoe UI", 12, FontStyle.Regular), // Increased from 10
                IsChild = true,
                IsFrameless = true,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.None,
                UseGradientBackground = false
            };

            txtUsername = new BeepTextBox
            {
                Font = new Font("Segoe UI", 12, FontStyle.Regular), // Increased from 10
                IsChild = true,
                IsFrameless = false,
                IsRounded = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                PlaceholderText = "Username or Email",
                ModernGradientType = ModernGradientType.Subtle,
                UseGradientBackground = true,
                Anchor = AnchorStyles.None
            };

            txtPassword = new BeepTextBox
            {
                Font = new Font("Segoe UI", 12, FontStyle.Regular), // Increased from 10
                IsChild = true,
                IsFrameless = false,
                IsRounded = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                PlaceholderText = "Password",
                PasswordChar = '*',
                ModernGradientType = ModernGradientType.Subtle,
                UseGradientBackground = true,
                Anchor = AnchorStyles.None
            };

            btnLogin = new BeepButton
            {
                Text = "Login",
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // Added explicit font size
                IsRounded = true,
              
                IsBorderAffectedByTheme = false,
                ModernGradientType = ModernGradientType.Linear,
                UseGradientBackground = true,
              //  ShowShadow = true,
                Anchor = AnchorStyles.None
            };

            lnkForgotPassword = new LinkLabel
            {
                Text = "Forgot Password?",
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Added explicit font size
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lnkRegister = new LinkLabel
            {
                Text = "Sign up now",
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Added explicit font size
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnLogo = new BeepCircularButton
            {
                AutoSize = false,
                IsFrameless = true,
                IsChild = true,
             //   ShowShadow = true,
                ModernGradientType = ModernGradientType.Radial,
                UseGradientBackground = true,
                Anchor = AnchorStyles.None
            };

            btnAvatar = new BeepCircularButton
            {
                AutoSize = false,
                IsFrameless = true,
                IsChild = true,
            //    ShowShadow = true,
                ModernGradientType = ModernGradientType.Radial,
                UseGradientBackground = true,
                Anchor = AnchorStyles.None
            };

            btnGoogleLogin = new BeepButton
            {
                Text = "Google",
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Added explicit font size
                IsRounded = true,
             
                IsBorderAffectedByTheme = false,
                ModernGradientType = ModernGradientType.Linear,
                UseGradientBackground = true,
                ShowShadow = true,
                Anchor = AnchorStyles.None
            };

            btnFacebookLogin = new BeepButton
            {
                Text = "Facebook",
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Added explicit font size
                IsRounded = true,
             
                IsBorderAffectedByTheme = false,
                ModernGradientType = ModernGradientType.Linear,
                UseGradientBackground = true,
              //  ShowShadow = true,
                Anchor = AnchorStyles.None
            };

            btnTwitterLogin = new BeepButton
            {
                Text = "Twitter",
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Added explicit font size
                IsRounded = true,
             
                IsBorderAffectedByTheme = false,
                ModernGradientType = ModernGradientType.Linear,
                UseGradientBackground = true,
            //    ShowShadow = true,
                Anchor = AnchorStyles.None
            };

            chkRememberMe = new BeepCheckBoxBool
            {
                Text = "Remember Me",
                AutoSize = false,
                IsChild = true,
                PainterKind= BaseControlPainterKind.Classic,
                IsFrameless=true,
                Font = new Font("Segoe UI", 11, FontStyle.Regular), // Increased from 9
                Anchor = AnchorStyles.None
            };

            // Attach event handlers
            btnLogin.Click += (s, e) => OnLoginClick();
            btnGoogleLogin.Click += (s, e) => OnGoogleLoginClick();
            btnFacebookLogin.Click += (s, e) => OnFacebookLoginClick();
            btnTwitterLogin.Click += (s, e) => OnTwitterLoginClick();

            this.Controls.Add(loginPanel);
            ApplyViewType();
        }

        private void SetDummyData()
        {
            txtUsername.Text = "user@example.com";
            txtPassword.Text = "password123";
            LogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cool.svg";
            AvatarPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
                SetDummyData();
        }

        public void BeginInit() { }
        public void EndInit()
        {
            if (DesignMode)
                SetDummyData();
        }

        private void ApplyViewType()
        {
            // Remove all children from the panel.
            loginPanel.Controls.Clear();

            // Hide all controls by default; they will be made visible in each view.
            lblTitle.Visible = false;
            lblSubtitle.Visible = false;
            txtUsername.Visible = true;
            txtPassword.Visible = true;
            btnLogin.Visible = true;
            lnkForgotPassword.Visible = false;
            lnkRegister.Visible = false;
            btnLogo.Visible = false;
            btnAvatar.Visible = false;
            btnGoogleLogin.Visible = false;
            btnFacebookLogin.Visible = false;
            btnTwitterLogin.Visible = false;
            chkRememberMe.Visible = false;

            // Apply modern styling based on view type
            ApplyModernStylingForViewType();

            // Use manual positioning for every view type.
            switch (_viewType)
            {
                case LoginViewType.Simple:
                    SetupSimpleView();
                    break;
                case LoginViewType.Compact:
                    SetupCompactView();
                    break;
                case LoginViewType.Minimal:
                    SetupMinimalView();
                    break;
                case LoginViewType.Social:
                    SetupSocialView();
                    break;
                case LoginViewType.SocialView2:
                    SetupSocialView2();
                    break;
                case LoginViewType.Modern:
                    SetupModernView();
                    break;
                case LoginViewType.Avatar:
                    SetupAvatarView();
                    break;
                case LoginViewType.Extended:
                    SetupExtendedView();
                    break;
                case LoginViewType.Full:
                    SetupFullView();
                    break;
            }

            // Add controls to the panel (their Parent remains loginPanel)
            loginPanel.Controls.Add(lblTitle);
            loginPanel.Controls.Add(lblSubtitle);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(txtPassword);
            loginPanel.Controls.Add(btnLogin);
            loginPanel.Controls.Add(lnkForgotPassword);
            loginPanel.Controls.Add(lnkRegister);
            loginPanel.Controls.Add(btnLogo);
            loginPanel.Controls.Add(btnAvatar);
            loginPanel.Controls.Add(btnGoogleLogin);
            loginPanel.Controls.Add(btnFacebookLogin);
            loginPanel.Controls.Add(btnTwitterLogin);
            loginPanel.Controls.Add(chkRememberMe);
        }

        /// <summary>
        /// Apply modern styling based on the selected view type
        /// </summary>
        private void ApplyModernStylingForViewType()
        {
            switch (_viewType)
            {
                case LoginViewType.Simple:
                case LoginViewType.Compact:
                    // Simple, clean styling
                    ModernGradientType = ModernGradientType.Subtle;
                    BorderRadius = 8;
                    break;

                case LoginViewType.Modern:
                    // More pronounced modern styling
                    ModernGradientType = ModernGradientType.Linear;
                    BorderRadius = 16;
                    UseGlassmorphism = true;
                    GlassmorphismOpacity = 0.1f;
                    break;

                case LoginViewType.Social:
                case LoginViewType.SocialView2:
                    // Social login gets more colorful gradients
                    ModernGradientType = ModernGradientType.Mesh;
                    BorderRadius = 12;
                    break;

                case LoginViewType.Avatar:
                    // Avatar view gets radial gradients to emphasize the circular avatar
                    ModernGradientType = ModernGradientType.Radial;
                    BorderRadius = 20;
                    break;

                case LoginViewType.Extended:
                case LoginViewType.Full:
                    // Extended views get subtle conic gradients for sophistication
                    ModernGradientType = ModernGradientType.Conic;
                    BorderRadius = 10;
                    break;

                default:
                    ModernGradientType = ModernGradientType.Subtle;
                    BorderRadius = 8;
                    break;
            }

            // Apply styling to child controls based on view type
            ApplyChildControlStyling();
        }

        /// <summary>
        /// Apply modern styling to child controls
        /// </summary>
        private void ApplyChildControlStyling()
        {
            // ProgressBarStyle buttons based on their purpose
            btnLogin.ModernGradientType = ModernGradientType.Linear;
          //  btnLogin.BorderRadius = 8;
           // btnLogin.ShowShadow = true;

            // Social login buttons get different colors and gradients
            btnGoogleLogin.ModernGradientType = ModernGradientType.Linear;
         //   btnGoogleLogin.BorderRadius = 6;
            
            btnFacebookLogin.ModernGradientType = ModernGradientType.Linear;
           // btnFacebookLogin.BorderRadius = 6;
            
            btnTwitterLogin.ModernGradientType = ModernGradientType.Linear;
          //  btnTwitterLogin.BorderRadius = 6;

            // Avatar and logo get radial gradients
            btnAvatar.ModernGradientType = ModernGradientType.Radial;
            btnLogo.ModernGradientType = ModernGradientType.Radial;

            // Input fields get subtle gradients
            txtUsername.ModernGradientType = ModernGradientType.Subtle;
           // txtUsername.BorderRadius = 6;
            
            txtPassword.ModernGradientType = ModernGradientType.Subtle;
          //  txtPassword.BorderRadius = 6;
        }

        #region Manual Positioning Layouts for Each View

        // Helper: get the available width inside the loginPanel.
        private int ContainerWidth()
        {
            return loginPanel.Width - loginPanel.Padding.Left - loginPanel.Padding.Right;
        }

        // SIMPLE VIEW: vertically stacked – Title, Avatar, Username, Password, [Remember, Forgot], Login.
        private void SetupSimpleView()
        {
            // Clear previous layout and disable auto-scrolling
            loginPanel.Controls.Clear();
            loginPanel.AutoScroll = false;

            // Set overall size of the BeepLogin control
            this.Size = new Size(300, 300);

            int margin = 10;
            int currentY = margin;
            int containerWidth = ContainerWidth(); // A helper that calculates available width inside loginPanel

            // 1) Title
            lblTitle.Text = "Login";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblTitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            // 2) Avatar
            btnAvatar.Visible = true;
            btnAvatar.Size = new Size(60, 60);
            btnAvatar.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnAvatar.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnAvatar);
            currentY += btnAvatar.Height + margin;

            // 3) Username
            txtUsername.Visible = true;
            txtUsername.Size = new Size(250, 30);
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // 4) Password
            txtPassword.Visible = true;
            txtPassword.Size = new Size(250, 30);
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // 5) Remember Me (left) and Forgot Password (right) on the same line
            chkRememberMe.Visible = true;
            chkRememberMe.AutoSize = true;
            // Align it with the left edge of the textboxes
            int leftEdge = loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2;
            chkRememberMe.Location = new Point(leftEdge, currentY );  // Slight vertical offset
            loginPanel.Controls.Add(chkRememberMe);

            lnkForgotPassword.Visible = true;
            lnkForgotPassword.AutoSize = true;
            // Place near the right edge
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + containerWidth - lnkForgotPassword.Width - margin,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);

            // Increase currentY by the tallest control in that row
            currentY += Math.Max(chkRememberMe.Height, lnkForgotPassword.Height) + margin;

            // 6) Login button (centered)
            btnLogin.Visible = true;
            btnLogin.Size = new Size(100, 30);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
        }


        // COMPACT VIEW: Title on top; side-by-side Username & Password; then Login; then Forgot; then Register.
        private void SetupCompactView()
        {
            // Set the overall size of the control
            this.Size = new Size(500, 200);
            int margin = 15;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // --- Title ---
            lblTitle.Text = "SIGN IN";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblTitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            // --- Side-by-side Username and Password ---
            txtUsername.Size = new Size(200, 30);
            txtPassword.Size = new Size(200, 30);
            // Place Username on the left
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + margin,
                currentY
            );
            // Place Password on the right
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + containerWidth - txtPassword.Width - margin,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(txtPassword);
            currentY += txtUsername.Height + margin;

            // --- Login button (centered) ---
            btnLogin.Size = new Size(100, 30);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
            currentY += btnLogin.Height + margin;

            // --- Forgot Password link (centered) ---
            lnkForgotPassword.Visible = true;
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkForgotPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);
            currentY += lnkForgotPassword.Height + margin;

            // --- Register link (centered) ---
            lnkRegister.Visible = true;
            lnkRegister.AutoSize = true;
            lnkRegister.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkRegister.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkRegister);
        }

        // MINIMAL VIEW: Row with Avatar (left) & Title (to right), then Username, Password, Register.
        private void SetupMinimalView()
        {
            // Set overall control size (adjust height if needed for the extra login button)
            this.Size = new Size(300, 280);
            int margin = 10;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // --- Row 1: Avatar and Title side-by-side ---
            btnAvatar.Visible = true;
            btnAvatar.Size = new Size(60, 80);
            // Place avatar with a fixed left margin
            btnAvatar.Location = new Point(loginPanel.Padding.Left + margin, currentY);
            loginPanel.Controls.Add(btnAvatar);

            lblTitle.Text = "Member Login";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true;
            // Position title to the right of the avatar, vertically centered relative to avatar
            lblTitle.Location = new Point(
                btnAvatar.Right + margin,
                currentY + (btnAvatar.Height - lblTitle.PreferredSize.Height) / 2
            );
            loginPanel.Controls.Add(lblTitle);

            // Move currentY below the avatar row
            currentY = btnAvatar.Bottom + margin;

            // --- Row 2: Username textbox ---
            txtUsername.Size = new Size(250, 30);
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // --- Row 3: Password textbox ---
            txtPassword.Size = new Size(250, 30);
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // --- Row 4: Login button (added now) ---
            btnLogin.Visible = true;
            btnLogin.Text = "Login";
            btnLogin.Size = new Size(100, 30);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
            currentY += btnLogin.Height + margin;

            // --- Row 5: Register link (centered) ---
            lnkRegister.Text = "Don't have an account? SIGN UP";
            lnkRegister.Visible = true;
            lnkRegister.AutoSize = true;
            lnkRegister.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkRegister.PreferredSize.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkRegister);

            // Hide controls not used in Minimal view
            lblSubtitle.Visible = false;
            lnkForgotPassword.Visible = false;
            btnLogo.Visible = false;
            btnGoogleLogin.Visible = false;
            btnFacebookLogin.Visible = false;
            btnTwitterLogin.Visible = false;
            chkRememberMe.Visible = false;
        }


        // SOCIAL VIEW: Title; Username; Password; Row with Remember & Login; Subtitle; Social buttons; Register.

        private void SetupSocialView()
        {
            // Clear old controls and paint handlers
            loginPanel.Controls.Clear();


            // Turn off auto-scrolling so manual positions remain stable
            loginPanel.AutoScroll = false;

            // Overall control size
            this.Size = new Size(450, 500);

            // Basic margin and vertical spacing
            int margin = 15;
            int currentY = margin;
            // The usable width inside loginPanel
            int containerWidth = loginPanel.ClientSize.Width
                                 - loginPanel.Padding.Left
                                 - loginPanel.Padding.Right;

            //---------------------------
            // 1) Title: "Sign In"
            //---------------------------
            lblTitle.Visible = true;
            lblTitle.Text = "Sign In";
            lblTitle.AutoSize = true;
            // Place top-left
            lblTitle.Location = new Point(loginPanel.Padding.Left, currentY);
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            //---------------------------
            // 2) Subtitle (wrap text)
            //---------------------------
            lblSubtitle.Visible = true;
            lblSubtitle.Text = "Lorem ipsum dolor sit amet elit. Sapiente sit aut eos consectetur adipisicing.";
            lblSubtitle.AutoSize = true;
            // Force wrapping by limiting maximum width
            lblSubtitle.MaximumSize = new Size(containerWidth, 0);
            lblSubtitle.Location = new Point(loginPanel.Padding.Left, currentY);
            loginPanel.Controls.Add(lblSubtitle);
            currentY += lblSubtitle.Height + margin;

            //------------------------------------------------
            // 3) We define a "card" rectangle behind the textboxes
            //    We'll figure out final height after we place them
            //------------------------------------------------
            int cardWidth = 320; // narrower than container, so it’s centered
            int cardHeight = 0;  // we’ll calculate it below
            int cardX = loginPanel.Padding.Left + (containerWidth - cardWidth) / 2;
            int cardY = currentY;

            // We'll place the textboxes, "Remember me," etc. inside that card
            int cardInnerMargin = 10; // margin inside the card
            int localY = cardY + cardInnerMargin; // local vertical offset

            // Username
            txtUsername.Visible = true;
            txtUsername.PlaceholderText = "Username";
            txtUsername.Size = new Size(cardWidth - cardInnerMargin * 2, 30);
            txtUsername.Location = new Point(
                cardX + cardInnerMargin,
                localY
            );
            loginPanel.Controls.Add(txtUsername);
            localY += txtUsername.Height + cardInnerMargin;

            // Password
            txtPassword.Visible = true;
            txtPassword.PlaceholderText = "Password";
            txtPassword.Size = new Size(cardWidth - cardInnerMargin * 2, 30);
            txtPassword.Location = new Point(
                cardX + cardInnerMargin,
                localY
            );
            loginPanel.Controls.Add(txtPassword);
            localY += txtPassword.Height + cardInnerMargin;

            // "Remember me" on left, "Forgot Password" on right
            chkRememberMe.Visible = true;
            chkRememberMe.Text = "Remember me";
            chkRememberMe.Location = new Point(
                cardX + cardInnerMargin,
                localY-5
            );
            loginPanel.Controls.Add(chkRememberMe);

            lnkForgotPassword.Visible = true;
            lnkForgotPassword.Text = "Forgot Password";
            lnkForgotPassword.AutoSize = true;
            // place near the right edge of the card
            int forgotX = cardX + cardWidth - lnkForgotPassword.PreferredWidth - cardInnerMargin;
            lnkForgotPassword.Location = new Point(forgotX, localY);
            loginPanel.Controls.Add(lnkForgotPassword);

            localY += Math.Max(chkRememberMe.Height, lnkForgotPassword.Height) + cardInnerMargin;

            // Now we know how tall the card must be
            cardHeight = localY - cardY;

            // Move main currentY below the card
            currentY += cardHeight + margin;

            //---------------------------------
            // 4) "Log In" button (centered)
            //---------------------------------
            btnLogin.Visible = true;
            btnLogin.Text = "Log In";
            btnLogin.Size = new Size(120, 35);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
            currentY += btnLogin.Height + margin;

            //---------------------------------
            // 5) "— or —" separator
            //---------------------------------
            Label orLabel = new Label
            {
                Text = "— or —",
                AutoSize = true,
                Anchor = AnchorStyles.None
            };
            orLabel.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - orLabel.PreferredWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(orLabel);
            currentY += orLabel.Height + margin;

            //---------------------------------
            // 6) Social Buttons: Facebook, Twitter, Google
            //---------------------------------
            int socialWidth = cardWidth;
            int socialHeight = 40;

            // Facebook
            btnFacebookLogin.Visible = true;
            btnFacebookLogin.Text = " Login with Facebook";
            btnFacebookLogin.Size = new Size(socialWidth, socialHeight);
            btnFacebookLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - socialWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnFacebookLogin);
            currentY += socialHeight + margin;

            // Twitter
            btnTwitterLogin.Visible = true;
            btnTwitterLogin.Text = " Login with Twitter";
            btnTwitterLogin.Size = new Size(socialWidth, socialHeight);
            btnTwitterLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - socialWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnTwitterLogin);
            currentY += socialHeight + margin;

            // Google
            btnGoogleLogin.Visible = true;
            btnGoogleLogin.Text = " Login with Google";
            btnGoogleLogin.Size = new Size(socialWidth, socialHeight);
            btnGoogleLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - socialWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnGoogleLogin);
            currentY += socialHeight + margin;

            // Hide anything else not used
            btnAvatar.Visible = false;
            btnLogo.Visible = false;

            

            // Optionally adjust final control height
            this.Size = new Size(this.Width, currentY + loginPanel.Padding.Bottom + margin);
        }

        // SOCIAL VIEW 2: Title; Username; Password; Forgot; Login; Subtitle; Social buttons; Register.
        private void SetupSocialView2()
        {
            // Set overall control size
            this.Size = new Size(350, 450);
            int margin = 15;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // --- Title ---
            lblTitle.Text = "Sign In";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblTitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            // --- Username ---
            txtUsername.Size = new Size(250, 30);
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // --- Password ---
            txtPassword.Size = new Size(250, 30);
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // --- Forgot Password ---
            lnkForgotPassword.Visible = true;
            lnkForgotPassword.AutoSize = false;
            lnkForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            // Let the link span almost the full width of loginPanel
            lnkForgotPassword.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + containerWidth - lnkForgotPassword.Width - margin,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);
            currentY += lnkForgotPassword.Height + margin;

            // --- Login button ---
            btnLogin.Text = "SIGN IN";
            btnLogin.Size = new Size(150, 40);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
            currentY += btnLogin.Height + margin;

            // --- Subtitle for social login ---
            lblSubtitle.Text = "Or login with";
            lblSubtitle.Visible = true;
            lblSubtitle.AutoSize = false;
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            lblSubtitle.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lblSubtitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblSubtitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblSubtitle);
            currentY += lblSubtitle.Height + margin;

            // --- Social buttons: Facebook and Google ---
            btnFacebookLogin.Visible = true;
            btnGoogleLogin.Visible = true;
            btnFacebookLogin.Size = new Size(150, 40);
            btnGoogleLogin.Size = new Size(150, 40);
            int totalSocialWidth = btnFacebookLogin.Width + btnGoogleLogin.Width + 10;
            btnFacebookLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - totalSocialWidth) / 2,
                currentY
            );
            btnGoogleLogin.Location = new Point(btnFacebookLogin.Right + 10, currentY);
            loginPanel.Controls.Add(btnFacebookLogin);
            loginPanel.Controls.Add(btnGoogleLogin);
            currentY += btnFacebookLogin.Height + margin;

            // --- Register link ---
            lnkRegister.Text = "Sign Up";
            lnkRegister.Visible = true;
            lnkRegister.AutoSize = false;
            lnkRegister.TextAlign = ContentAlignment.MiddleCenter;
            lnkRegister.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lnkRegister.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkRegister.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkRegister);

            // Hide unused controls for this view
            btnTwitterLogin.Visible = false;
            chkRememberMe.Visible = false;
            btnLogo.Visible = false;
            btnAvatar.Visible = false;
        }


        // MODERN VIEW: Title; Subtitle; Username; Password; Row with Remember & Forgot; Login.
        private void SetupModernView()
        {
            // Set overall control size
            this.Size = new Size(400, 350);
            int margin = 15;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // Clear any existing controls on loginPanel
            loginPanel.Controls.Clear();

            // --- Title ---
            lblTitle.Text = "Login to Colorlib";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true; // Ensure width is computed
            lblTitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblTitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            // --- Subtitle ---
            lblSubtitle.Text = "Lorem ipsum dolor sit amet elit. Sapiente sit aut eos consectetur adipisicing.";
            lblSubtitle.Visible = true;
            // Set a fixed height and limit maximum width so text wraps if necessary
            lblSubtitle.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // Center the subtitle horizontally
            lblSubtitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblSubtitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblSubtitle);
            currentY += lblSubtitle.Height + margin;

            // --- Username ---
            txtUsername.Size = new Size(250, 30);
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // --- Password ---
            txtPassword.Size = new Size(250, 30);
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // --- "Remember Me" Checkbox ---
            chkRememberMe.Visible = true;
            // For Modern view, we want a single row spanning nearly the full container width
            chkRememberMe.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            // Align to the left with a small margin
            chkRememberMe.Location = new Point(loginPanel.Padding.Left + margin, currentY);
            loginPanel.Controls.Add(chkRememberMe);
            currentY += chkRememberMe.Height + margin;

            // --- "Forgot Password" Link ---
            lnkForgotPassword.Visible = true;
            lnkForgotPassword.AutoSize = false;
            lnkForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            lnkForgotPassword.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkForgotPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);
            currentY += lnkForgotPassword.Height + margin;

            // --- Login Button ---
            btnLogin.Size = new Size(100, 30);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
        }


        // AVATAR VIEW: Avatar; Title; Username; Password; Forgot; Register; Login.
        private void SetupAvatarView()
        {
            // Set overall control size
            this.Size = new Size(300, 330);
            int margin = 10;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // --- Avatar (centered) ---
            btnAvatar.Visible = true;
            btnAvatar.Size = new Size(60, 60);
            btnAvatar.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnAvatar.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnAvatar);
            currentY += btnAvatar.Height + margin;

            // --- Title (centered in a fixed height area) ---
            lblTitle.Text = "John Doe";
            lblTitle.Visible = true;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // Use a fixed height so the label occupies 40 pixels in height
            lblTitle.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lblTitle.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lblTitle.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);
            currentY += lblTitle.Height + margin;

            // --- Username (centered) ---
            txtUsername.Size = new Size(250, 30);
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // --- Password (centered) ---
            txtPassword.Size = new Size(250, 30);
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // --- Forgot Password (centered) ---
            lnkForgotPassword.Visible = true;
            lnkForgotPassword.AutoSize = false;
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkForgotPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);
            currentY += lnkForgotPassword.PreferredHeight + margin;

            // --- Register Link (centered) ---
            lnkRegister.Visible = true;
            lnkRegister.AutoSize = true;
            lnkRegister.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkRegister.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkRegister);
            currentY += lnkRegister.PreferredHeight + margin;

            // --- Login Button (centered) ---
            btnLogin.Size = new Size(100, 30);
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - btnLogin.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(btnLogin);
        }


        // EXTENDED VIEW: Title; Subtitle; Username; Password; Row with Login, Google, Facebook.
        private void SetupExtendedView()
        {
            // Clear any previous controls and disable auto-scroll
            loginPanel.Controls.Clear();
            loginPanel.AutoScroll = false;

            // Set overall control size
            this.Size = new Size(500, 250);

            int margin = 10;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            // --- Row 1: Top Row – Title (left) and Social Icons (right) ---
            // Title on left
            lblTitle.Text = "Sign In";
            lblTitle.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(
                loginPanel.Padding.Left,
                currentY
            );
            loginPanel.Controls.Add(lblTitle);

            // Social icons on the right (for example, two icons)
            int iconSize = 30;
            btnFacebookLogin.Visible = true;
            btnFacebookLogin.Text = "f"; // Use actual icon/text as desired
            btnFacebookLogin.Size = new Size(iconSize, iconSize);
            btnFacebookLogin.Location = new Point(
                loginPanel.Padding.Left + containerWidth - (iconSize * 2) - 5,
                currentY
            );
            loginPanel.Controls.Add(btnFacebookLogin);

            btnTwitterLogin.Visible = true;
            btnTwitterLogin.Text = "t";
            btnTwitterLogin.Size = new Size(iconSize, iconSize);
            btnTwitterLogin.Location = new Point(
                btnFacebookLogin.Right + 5,
                currentY
            );
            loginPanel.Controls.Add(btnTwitterLogin);

            // Advance currentY by the tallest element in row 1
            currentY += Math.Max(lblTitle.Height, iconSize) + margin;

            // --- Row 2: Subtitle ---
            lblSubtitle.Text = "Please enter your credentials below:";
            lblSubtitle.Visible = true;
            lblSubtitle.AutoSize = true;
            // Limit max width so text wraps if necessary
            lblSubtitle.MaximumSize = new Size(containerWidth, 0);
            lblSubtitle.Location = new Point(
                loginPanel.Padding.Left,
                currentY
            );
            loginPanel.Controls.Add(lblSubtitle);
            currentY += lblSubtitle.Height + margin;

            // --- Row 3: Username Textbox (centered) ---
            txtUsername.Size = new Size(300, 30);
            txtUsername.Visible = true;
            txtUsername.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtUsername);
            currentY += txtUsername.Height + margin;

            // --- Row 4: Password Textbox (centered) ---
            txtPassword.Size = new Size(300, 30);
            txtPassword.Visible = true;
            txtPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2,
                currentY
            );
            loginPanel.Controls.Add(txtPassword);
            currentY += txtPassword.Height + margin;

            // --- Row 5: "Save Password" Checkbox (left) and Login Button (right) ---
            chkRememberMe.Visible = true;
            chkRememberMe.Text = "Save Password";
            chkRememberMe.AutoSize = true;
            chkRememberMe.Location = new Point(
                loginPanel.Padding.Left,
                currentY + 2   // slight vertical adjustment
            );
            loginPanel.Controls.Add(chkRememberMe);

            btnLogin.Visible = true;
            btnLogin.Text = "Login";
            btnLogin.Size = new Size(80, 30);
            // Position the login button near the right edge of the available container area
            btnLogin.Location = new Point(
                loginPanel.Padding.Left + containerWidth - btnLogin.Width,
                currentY - 5    // adjust as needed for vertical alignment
            );
            loginPanel.Controls.Add(btnLogin);
            currentY += Math.Max(chkRememberMe.Height, btnLogin.Height) + margin;

            // --- Row 6: Bottom Text Lines ---
            // "Don't have an account? Sign Up"
            Label signUpLabel = new Label
            {
                Text = "Don't have an account? Sign Up",
                AutoSize = true
            };
            signUpLabel.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - signUpLabel.PreferredWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(signUpLabel);
            currentY += signUpLabel.Height + margin;

            // "Forgot Password" link (centered)
            lnkForgotPassword.Visible = true;
            lnkForgotPassword.Text = "Forgot Password";
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Location = new Point(
                loginPanel.Padding.Left + (containerWidth - lnkForgotPassword.PreferredWidth) / 2,
                currentY
            );
            loginPanel.Controls.Add(lnkForgotPassword);
            currentY += lnkForgotPassword.Height + margin;

            // Hide unused controls
            btnLogo.Visible = false;
            btnGoogleLogin.Visible = false;
            btnAvatar.Visible = false;

            // Optionally adjust overall control height to fit all rows
           // this.Size = new Size(this.Width, currentY + loginPanel.Padding.Bottom + margin);
        }


        // FULL VIEW: Logo; Row with Avatar & Title; Subtitle; Username; Password; Row with Login, Forgot, Register.
        private void SetupFullView()
        {
            this.Size = new Size(500, 360);
            int margin = 10;
            int currentY = margin;
            int containerWidth = ContainerWidth();

            btnLogo.Visible = true;
            btnLogo.Size = new Size(60, 60);
            btnLogo.Location = new Point(loginPanel.Padding.Left + (containerWidth - btnLogo.Width) / 2, currentY);
            currentY += btnLogo.Height + margin;

            // Avatar & Title row
            btnAvatar.Visible = true;
            btnAvatar.Size = new Size(60, 60);
            btnAvatar.Location = new Point(loginPanel.Padding.Left + (containerWidth - btnAvatar.Width) / 2, currentY);
            currentY += btnAvatar.Height + margin;

            lblTitle.Text = "Full Login";
            lblTitle.Visible = true;
            lblTitle.AutoSize = false;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lblTitle.Location = new Point(loginPanel.Padding.Left + margin, currentY);
            currentY += lblTitle.PreferredSize.Height + margin;

            lblSubtitle.Text = "Welcome! Please sign in below.";
            lblSubtitle.Visible = true;
            lblSubtitle.AutoSize = false;
            lblSubtitle.Size = new Size(containerWidth - (loginPanel.Padding.Left * 2), 40);
            lblSubtitle.Location = new Point(loginPanel.Padding.Left + margin, currentY);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            currentY += lblSubtitle.PreferredSize.Height + margin;

            txtUsername.Size = new Size(300, 30);
            txtUsername.Location = new Point(loginPanel.Padding.Left + (containerWidth - txtUsername.Width) / 2, currentY);
            currentY += txtUsername.Height + margin;

            txtPassword.Size = new Size(300, 30);
            txtPassword.Location = new Point(loginPanel.Padding.Left + (containerWidth - txtPassword.Width) / 2, currentY);
            currentY += txtPassword.Height + margin;

            // Row with Login, Forgot, Register
            btnLogin.Size = new Size(100, 30);
            lnkForgotPassword.Visible = true;
            lnkRegister.Visible = true;
            int totalRowWidth = btnLogin.Width + lnkForgotPassword.Width + lnkRegister.Width + 20;
            int startX = loginPanel.Padding.Left + (containerWidth - totalRowWidth) / 2;
            btnLogin.Location = new Point(startX, currentY);
            lnkForgotPassword.Location = new Point(btnLogin.Right + 10, currentY + (btnLogin.Height - lnkForgotPassword.Height) / 2);
            lnkRegister.Location = new Point(lnkForgotPassword.Right + 10, currentY + (btnLogin.Height - lnkRegister.Height) / 2);
        }

        #endregion

        protected virtual void OnLoginClick()
        {
            LoginClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGoogleLoginClick()
        {
            GoogleLoginClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFacebookLoginClick()
        {
            FacebookLoginClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTwitterLoginClick()
        {
            TwitterLoginClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnRegisterClick()
        {
            RegisterClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnForgotPasswordClick()
        {
            ForgotPasswordClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnAvatarClick()
        {
            AvatarClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnLogoClick()
        {
            LogoClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnViewTypeChanged()
        {
            ViewTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LoginClick;
        public event EventHandler GoogleLoginClick;
        public event EventHandler FacebookLoginClick;
        public event EventHandler TwitterLoginClick;
        public event EventHandler RegisterClick;
        public event EventHandler ForgotPasswordClick;
        public event EventHandler AvatarClick;
        public event EventHandler LogoClick;
        public event EventHandler ViewTypeChanged;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between different login views.")]
        public LoginViewType ViewType
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
        public string Username
        {
            get => txtUsername.Text;
            set => txtUsername.Text = value ?? "";
        }

        [Browsable(true)]
        [Category("Data")]
        public string Password
        {
            get => txtPassword.Text;
            set => txtPassword.Text = value ?? "";
        }

        [Browsable(true)]
        [Category("Data")]
        public string LogoPath
        {
            get => btnLogo.ImagePath;
            set => btnLogo.ImagePath = value;
        }

        [Browsable(true)]
        [Category("Data")]
        public string AvatarPath
        {
            get => btnAvatar.ImagePath;
            set => btnAvatar.ImagePath = value;
        }

        [Browsable(true)]
        [Category("Data")]
        public bool RememberMe
        {
            get => chkRememberMe.CurrentValue;
            set => chkRememberMe.CurrentValue = value;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (loginPanel == null) return;
            
            // Apply modern theme styling to the login panel first
            loginPanel.BackColor = _currentTheme.LoginPopoverBackgroundColor;

            // Apply enhanced theming with modern gradients
            if (_currentTheme != null)
            {
                // Set gradient colors based on theme
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;

                // Apply theme colors to child controls
                lblTitle.ForeColor = _currentTheme.LoginTitleColor;
                lblSubtitle.ForeColor = _currentTheme.LoginSubtitleColor;
                
                // Text boxes get theme colors and modern styling
                txtUsername.ForeColor = _currentTheme.LoginTitleColor;
                txtUsername.BackColor = _currentTheme.TextBoxBackColor;
                txtUsername.BorderColor = _currentTheme.TextBoxBorderColor;
                
                txtPassword.ForeColor = _currentTheme.LoginTitleColor;
                txtPassword.BackColor = _currentTheme.TextBoxBackColor;
                txtPassword.BorderColor = _currentTheme.TextBoxBorderColor;

                // Links get theme colors
                lnkForgotPassword.LinkColor = _currentTheme.LoginLinkColor;
                lnkRegister.LinkColor = _currentTheme.LoginLinkColor;

                // Buttons get enhanced styling
                ApplyButtonTheming(btnLogin, _currentTheme.LoginButtonBackgroundColor, _currentTheme.LoginButtonTextColor);
                ApplyButtonTheming(btnGoogleLogin, Color.FromArgb(219, 68, 55), Color.White); // Google brand colors
                ApplyButtonTheming(btnFacebookLogin, Color.FromArgb(66, 103, 178), Color.White); // Facebook brand colors
                ApplyButtonTheming(btnTwitterLogin, Color.FromArgb(29, 161, 242), Color.White); // Twitter brand colors

                // Avatar and logo get theme-aware backgrounds
                btnLogo.BackColor = _currentTheme.LoginLogoBackgroundColor;
                btnAvatar.BackColor = _currentTheme.LoginLogoBackgroundColor;

                // Apply glassmorphism for modern themes
                if (ModernGradientType == ModernGradientType.Linear || 
                    ModernGradientType == ModernGradientType.Mesh ||
                    _viewType == LoginViewType.Modern)
                {
                    UseGlassmorphism = true;
                    GlassmorphismOpacity = 0.08f;
                }
            }

            // Apply base theme functionality
            base.ApplyTheme();
        }

        /// <summary>
        /// Apply modern theming to buttons with gradient and styling
        /// </summary>
        private void ApplyButtonTheming(BeepButton button, Color backgroundColor, Color textColor)
        {
            if (button == null) return;

            button.BackColor = backgroundColor;
            button.ForeColor = textColor;
            
            // Set gradient colors based on the background color
            button.GradientStartColor = backgroundColor;
            button.GradientEndColor = DarkenColor(backgroundColor, 0.8f);
            
            // Apply modern styling
            button.UseGradientBackground = true;
            button.IsRounded = true;
          //  button.ShowShadow = true;
           // button.ShadowOpacity = 0.3f;
            
            // Set hover colors
            button.HoverBackColor = LightenColor(backgroundColor, 1.1f);
            button.HoverForeColor = textColor;
        }

        /// <summary>
        /// Utility method to darken a color
        /// </summary>
        private Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor)
            );
        }

        /// <summary>
        /// Utility method to lighten a color
        /// </summary>
        private Color LightenColor(Color color, float factor) 
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor))
            );
        }

        public void ClearPassword()
        {
            throw new NotImplementedException();
        }

        public void FocusPassword()
        {
            throw new NotImplementedException();
        }
    }
}
