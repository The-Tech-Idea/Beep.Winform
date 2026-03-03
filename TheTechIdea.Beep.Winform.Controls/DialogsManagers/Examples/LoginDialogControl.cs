using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Examples
{
    /// <summary>
    /// Example UserControl for login dialog
    /// Demonstrates data binding, validation, and result extraction
    /// </summary>
    public class LoginDialogControl : UserControl
    {
        private BeepLabel lblUsername;
        private BeepLabel lblPassword;
        private BeepTextBox txtUsername;
        private BeepTextBox txtPassword;
        private BeepCheckBox<Boolean> chkRememberMe;
        private BeepLabel lblError;

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
        public bool RememberMe => chkRememberMe.CheckedValue;

        #endregion

        public LoginDialogControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Username Label
            lblUsername = new BeepLabel
            {
                Text = "Username:",
                Location = new Point(10, 10),
                Size = new Size(280, 20),
                UseThemeColors = true
            };

            // Username TextBox
            txtUsername = new BeepTextBox
            {
                Location = new Point(10, 35),
                Size = new Size(280, 24),
                TabIndex = 0,
                UseThemeColors = true
            };

            // Password Label
            lblPassword = new BeepLabel
            {
                Text = "Password:",
                Location = new Point(10, 70),
                Size = new Size(280, 20),
                UseThemeColors = true
            };

            // Password TextBox
            txtPassword = new BeepTextBox
            {
                Location = new Point(10, 95),
                Size = new Size(280, 24),
                UseSystemPasswordChar = true,
                TabIndex = 1,
                UseThemeColors = true
            };

            // Remember Me Checkbox
            chkRememberMe = new BeepCheckBox<Boolean>
            {
                Text = "Remember me",
                Location = new Point(10, 130),
                Size = new Size(280, 24),
                TabIndex = 2,
                UseThemeColors = true
            };

            // Error Label
            lblError = new BeepLabel
            {
                Text = "",
                Location = new Point(10, 160),
                Size = new Size(280, 40),
                ForeColor = Color.FromArgb(220, 53, 69),
                Visible = false,
                UseThemeColors = true
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
            chkRememberMe.CheckedValue = false;
            HideError();
        }
    }
}
