using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Examples
{
    /// <summary>
    /// Example UserControl for login dialog
    /// Demonstrates data binding, validation, and result extraction
    /// </summary>
    public class LoginDialogControl : UserControl
    {
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRememberMe;
        private Label lblError;

        #region Properties

        /// <summary>
        /// Username entered by user
        /// </summary>
        public string Username => txtUsername.Text;

        /// <summary>
        /// Password entered by user
        /// </summary>
        public string Password => txtPassword.Text;

        /// <summary>
        /// Whether user wants to be remembered
        /// </summary>
        public bool RememberMe => chkRememberMe.Checked;

        #endregion

        public LoginDialogControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Username Label
            lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(10, 10),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            // Username TextBox
            txtUsername = new TextBox
            {
                Location = new Point(10, 35),
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 10F),
                TabIndex = 0
            };

            // Password Label
            lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(10, 70),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            // Password TextBox
            txtPassword = new TextBox
            {
                Location = new Point(10, 95),
                Size = new Size(280, 24),
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10F),
                TabIndex = 1
            };

            // Remember Me Checkbox
            chkRememberMe = new CheckBox
            {
                Text = "Remember me",
                Location = new Point(10, 130),
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 9F),
                TabIndex = 2
            };

            // Error Label
            lblError = new Label
            {
                Text = "",
                Location = new Point(10, 160),
                Size = new Size(280, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69),
                Visible = false
            };

            // Add controls
            Controls.AddRange(new Control[]
            {
                lblUsername,
                txtUsername,
                lblPassword,
                txtPassword,
                chkRememberMe,
                lblError
            });

            // Set UserControl size
            Size = new Size(300, 200);
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// Validate the login form
        /// </summary>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if valid</returns>
        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorMessage = "Username is required.";
                ShowError(errorMessage);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorMessage = "Password is required.";
                ShowError(errorMessage);
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                errorMessage = "Password must be at least 6 characters.";
                ShowError(errorMessage);
                return false;
            }

            errorMessage = string.Empty;
            HideError();
            return true;
        }

        /// <summary>
        /// Show error message
        /// </summary>
        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        /// <summary>
        /// Hide error message
        /// </summary>
        private void HideError()
        {
            lblError.Visible = false;
        }

        /// <summary>
        /// Clear the form
        /// </summary>
        public void Clear()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            chkRememberMe.Checked = false;
            HideError();
        }
    }
}
