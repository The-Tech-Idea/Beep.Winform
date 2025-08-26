using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Examples
{
    /// <summary>
    /// Example form demonstrating BeepMaterialTextField with all features
    /// </summary>
    public partial class MaterialTextFieldExampleForm : Form
    {
        public MaterialTextFieldExampleForm()
        {
            InitializeComponent();
            SetupExamples();
        }

        private void InitializeComponent()
        {
            Text = "Material Design TextField Examples";
            Size = new Size(1000, 1200);
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;
            BackColor = Color.FromArgb(250, 250, 250);
        }

        private void SetupExamples()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            int yPosition = 20;
            const int spacing = 100;

            // Title
            var titleLabel = new Label
            {
                Text = "Material Design TextField Examples - Beep Controls",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, yPosition),
                Size = new Size(900, 40),
                ForeColor = Color.FromArgb(33, 33, 33)
            };
            panel.Controls.Add(titleLabel);
            yPosition += 60;

            // 1. Basic Outlined TextField
            AddSectionTitle(panel, "1. Basic Outlined Text Field", ref yPosition);
            var basicOutlined = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 72),
                LabelText = "Enter your name",
                Variant = MaterialTextFieldVariant.Outlined,
                HelperText = "This is a helper text"
            };
            panel.Controls.Add(basicOutlined);
            yPosition += spacing;

            // 2. Filled TextField
            AddSectionTitle(panel, "2. Filled Text Field", ref yPosition);
            var basicFilled = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 72),
                LabelText = "Email address",
                Variant = MaterialTextFieldVariant.Filled,
                HelperText = "We'll never share your email"
            };
            panel.Controls.Add(basicFilled);
            yPosition += spacing;

            // 3. Required Field
            AddSectionTitle(panel, "3. Required Field with Validation", ref yPosition);
            var requiredField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 72),
                LabelText = "Password",
                Variant = MaterialTextFieldVariant.Outlined,
                IsRequired = true,
                HelperText = "Password is required",
                UseSystemPasswordChar = true
            };
            panel.Controls.Add(requiredField);
            yPosition += spacing;

            // 4. Search Box Style with Dual Icons
            AddSectionTitle(panel, "4. Search Box Style with Dual Icons", ref yPosition);
            var searchField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(400, 48),
                PlaceholderText = "Search...",
                SearchBoxStyle = true,
                LeadingIconPath = Svgs.Search,
                TrailingIconPath = Svgs.Microphone,
                ShowClearButton = false, // Use custom trailing icon instead
                FillColor = Color.FromArgb(240, 240, 240)
            };
            
            // Handle icon clicks
            searchField.LeadingIconClicked += (s, e) => MessageBox.Show("Search icon clicked!");
            searchField.TrailingIconClicked += (s, e) => MessageBox.Show("Microphone icon clicked!");
            
            panel.Controls.Add(searchField);
            yPosition += spacing;

            // 5. Curved Border with Custom Radius
            AddSectionTitle(panel, "5. Custom Curved Border", ref yPosition);
            var curvedField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 56),
                LabelText = "Curved Border",
                CurvedBorderRadius = 28, // Half of height for pill shape
                ShowFill = true,
                FillColor = Color.FromArgb(250, 250, 250),
                LeadingIconPath = Svgs.Person,
                HelperText = "Custom curved border radius"
            };
            panel.Controls.Add(curvedField);
            yPosition += spacing;

            // 6. Dual Icons with Actions
            AddSectionTitle(panel, "6. Dual Clickable Icons", ref yPosition);
            var dualIconField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(350, 64),
                LabelText = "Message",
                LeadingIconPath = Svgs.Email,
                TrailingIconPath = Svgs.Send,
                Variant = MaterialTextFieldVariant.Outlined,
                HelperText = "Click icons to perform actions"
            };
            
            // Configure dual icons
            dualIconField.ConfigureDualIcons(Svgs.Email, Svgs.Send, true, true);
            
            // Handle icon clicks
            dualIconField.LeadingIconClicked += (s, e) => MessageBox.Show("Email icon clicked!");
            dualIconField.TrailingIconClicked += (s, e) => 
            {
                if (!string.IsNullOrEmpty(dualIconField.Text))
                {
                    MessageBox.Show($"Sending: {dualIconField.Text}");
                    dualIconField.Clear();
                }
                else
                {
                    MessageBox.Show("Enter a message first!");
                }
            };
            
            panel.Controls.Add(dualIconField);
            yPosition += spacing;

            // 7. Error State
            AddSectionTitle(panel, "7. Error State with Validation", ref yPosition);
            var errorField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 72),
                LabelText = "Email",
                Variant = MaterialTextFieldVariant.Outlined,
                ErrorText = "Please enter a valid email address",
                Text = "invalid-email",
                LeadingIconPath = Svgs.Email
            };
            panel.Controls.Add(errorField);
            yPosition += spacing;

            // 8. Read-Only Field
            AddSectionTitle(panel, "8. Read-Only Field", ref yPosition);
            var readOnlyField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(300, 72),
                LabelText = "User ID",
                Variant = MaterialTextFieldVariant.Filled,
                ReadOnly = true,
                Text = "USER-12345",
                HelperText = "This field cannot be edited",
                LeadingIconPath = Svgs.User
            };
            panel.Controls.Add(readOnlyField);
            yPosition += spacing;

            // 9. Multiline TextField
            AddSectionTitle(panel, "9. Multiline Text Field with Line Numbers", ref yPosition);
            var multilineField = new BeepMaterialTextField
            {
                Location = new Point(20, yPosition),
                Size = new Size(400, 150),
                LabelText = "Description",
                Variant = MaterialTextFieldVariant.Outlined,
                Multiline = true,
                ShowLineNumbers = true,
                HelperText = "Enter a detailed description",
                Text = "Line 1: This is a multiline text field\nLine 2: With line numbers enabled\nLine 3: Material Design compliant"
            };
            panel.Controls.Add(multilineField);
            yPosition += 170;

            // 10. Advanced Features Demo
            AddSectionTitle(panel, "10. Advanced Features Demo", ref yPosition);
            CreateAdvancedFeaturesDemo(panel, ref yPosition);

            // 11. New Material Design 3.0 Features Demo
            AddSectionTitle(panel, "11. Material Design 3.0 Enhanced Features", ref yPosition);
            CreateMaterialDesign3Demo(panel, ref yPosition);

            // 12. Complete Contact Form
            AddSectionTitle(panel, "12. Complete Contact Form Example", ref yPosition);
            CreateContactForm(panel, ref yPosition);

            // Set the panel size
            panel.Height = yPosition + 100;
            Controls.Add(panel);

            // Setup event handlers for demonstration
            SetupEventHandlers(basicOutlined, basicFilled, requiredField, searchField, curvedField, dualIconField, errorField, readOnlyField);
        }

        private void AddSectionTitle(Panel panel, string title, ref int yPosition)
        {
            var label = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, yPosition),
                Size = new Size(900, 25),
                ForeColor = Color.FromArgb(66, 66, 66)
            };
            panel.Controls.Add(label);
            yPosition += 35;
        }

        private void CreateAdvancedFeaturesDemo(Panel parent, ref int yPosition)
        {
            var demoPanel = new Panel
            {
                Location = new Point(20, yPosition),
                Size = new Size(600, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Syntax highlighting field
            var syntaxField = new BeepMaterialTextField
            {
                Location = new Point(20, 20),
                Size = new Size(550, 100),
                LabelText = "Code Editor",
                Variant = MaterialTextFieldVariant.Outlined,
                Multiline = true,
                SyntaxHighlightingEnabled = true,
                SyntaxLanguage = SyntaxLanguage.CSharp,
                ShowLineNumbers = true,
                Text = "public class Example\n{\n    public string Name { get; set; }\n}",
                HelperText = "C# syntax highlighting enabled"
            };

            // Auto-complete field
            var autoCompleteField = new BeepMaterialTextField
            {
                Location = new Point(20, 140),
                Size = new Size(250, 72),
                LabelText = "Programming Language",
                Variant = MaterialTextFieldVariant.Filled,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                LeadingIconPath = Svgs.Search,
                HelperText = "Type to see suggestions"
            };
            
            // Add some autocomplete items
            autoCompleteField.AddAutoCompleteItems(new[] { "C#", "JavaScript", "Python", "Java", "TypeScript", "Go", "Rust" });

            // Masked input field
            var maskedField = new BeepMaterialTextField
            {
                Location = new Point(290, 140),
                Size = new Size(250, 72),
                LabelText = "Phone Number",
                Variant = MaterialTextFieldVariant.Outlined,
                MaskFormat = TextBoxMaskFormat.PhoneNumber,
                HelperText = "Format: (555) 123-4567"
            };

            demoPanel.Controls.AddRange(new Control[] { syntaxField, autoCompleteField, maskedField });
            parent.Controls.Add(demoPanel);
            yPosition += 320;
        }

        private void CreateMaterialDesign3Demo(Panel parent, ref int yPosition)
        {
            var demoPanel = new Panel
            {
                Location = new Point(20, yPosition),
                Size = new Size(600, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Standard variant field
            var standardField = new BeepMaterialTextField
            {
                Location = new Point(20, 20),
                Size = new Size(250, 72),
                LabelText = "Standard Variant",
                Variant = MaterialTextFieldVariant.Standard,
                HelperText = "Standard variant with underline only"
            };

            // Dense density field
            var denseField = new BeepMaterialTextField
            {
                Location = new Point(290, 20),
                Size = new Size(250, 56),
                LabelText = "Dense Field",
                Variant = MaterialTextFieldVariant.Outlined,
                Density = MaterialTextFieldDensity.Dense,
                HelperText = "Dense spacing (56px height)"
            };

            // Field with prefix/suffix
            var prefixSuffixField = new BeepMaterialTextField
            {
                Location = new Point(20, 100),
                Size = new Size(250, 72),
                LabelText = "Amount",
                Variant = MaterialTextFieldVariant.Outlined,
                PrefixText = "$",
                SuffixText = ".00",
                Text = "123",
                HelperText = "Field with prefix and suffix"
            };

            // Field with character counter
            var counterField = new BeepMaterialTextField
            {
                Location = new Point(290, 100),
                Size = new Size(250, 72),
                LabelText = "Tweet Message",
                Variant = MaterialTextFieldVariant.Filled,
                MaxLength = 280,
                ShowCharacterCounter = true,
                Multiline = true,
                Text = "This is a sample tweet message with character counter enabled.",
                HelperText = "Max 280 characters"
            };

            // Comfortable density field
            var comfortableField = new BeepMaterialTextField
            {
                Location = new Point(20, 200),
                Size = new Size(250, 88),
                LabelText = "Comfortable Field",
                Variant = MaterialTextFieldVariant.Filled,
                Density = MaterialTextFieldDensity.Comfortable,
                HelperText = "Comfortable spacing (88px height)"
            };

            // URL field with prefix
            var urlField = new BeepMaterialTextField
            {
                Location = new Point(290, 200),
                Size = new Size(250, 72),
                LabelText = "Website URL",
                Variant = MaterialTextFieldVariant.Outlined,
                PrefixText = "https://",
                Text = "example.com",
                LeadingIconPath = Svgs.Share,
                HelperText = "Enter your website URL"
            };

            // Currency field with all features
            var currencyField = new BeepMaterialTextField
            {
                Location = new Point(20, 300),
                Size = new Size(520, 72),
                LabelText = "Product Price",
                Variant = MaterialTextFieldVariant.Outlined,
                PrefixText = "$",
                SuffixText = "USD",
                MaxLength = 10,
                ShowCharacterCounter = true,
                MaskFormat = TextBoxMaskFormat.Currency,
                IsRequired = true,
                Text = "99.99",
                HelperText = "Enter price in USD"
            };

            demoPanel.Controls.AddRange(new Control[] 
            { 
                standardField, denseField, prefixSuffixField, counterField, 
                comfortableField, urlField, currencyField 
            });

            parent.Controls.Add(demoPanel);
            yPosition += 420;
        }

        private void CreateContactForm(Panel parent, ref int yPosition)
        {
            var formPanel = new Panel
            {
                Location = new Point(20, yPosition),
                Size = new Size(600, 500),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Form title
            var formTitle = new Label
            {
                Text = "Contact Information Form",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 10),
                Size = new Size(400, 30),
                ForeColor = Color.FromArgb(33, 33, 33)
            };

            // First Name
            var firstName = new BeepMaterialTextField
            {
                Location = new Point(20, 50),
                Size = new Size(250, 72),
                LabelText = "First Name",
                Variant = MaterialTextFieldVariant.Outlined,
                IsRequired = true,
                LeadingIconPath = Svgs.User
            };

            // Last Name  
            var lastName = new BeepMaterialTextField
            {
                Location = new Point(290, 50),
                Size = new Size(250, 72),
                LabelText = "Last Name",
                Variant = MaterialTextFieldVariant.Outlined,
                IsRequired = true
            };

            // Email
            var email = new BeepMaterialTextField
            {
                Location = new Point(20, 140),
                Size = new Size(520, 72),
                LabelText = "Email Address",
                Variant = MaterialTextFieldVariant.Filled,
                LeadingIconPath = Svgs.Email,
                ShowClearButton = true,
                IsRequired = true,
                HelperText = "We'll use this to contact you"
            };

            // Phone field
            var phone = new BeepMaterialTextField
            {
                Location = new Point(20, 230),
                Size = new Size(250, 72),
                LabelText = "Phone Number",
                Variant = MaterialTextFieldVariant.Outlined,
                MaskFormat = TextBoxMaskFormat.PhoneNumber,
                PlaceholderText = "(555) 123-4567"
            };

            // Company
            var company = new BeepMaterialTextField
            {
                Location = new Point(290, 230),
                Size = new Size(250, 72),
                LabelText = "Company",
                Variant = MaterialTextFieldVariant.Outlined,
                HelperText = "Optional"
            };

            // Message
            var message = new BeepMaterialTextField
            {
                Location = new Point(20, 320),
                Size = new Size(520, 120),
                LabelText = "Message",
                Variant = MaterialTextFieldVariant.Outlined,
                Multiline = true,
                HelperText = "Tell us how we can help you",
                MaxLength = 500
            };

            // Submit button
            var submitButton = new Button
            {
                Text = "Submit Form",
                Location = new Point(20, 460),
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(98, 0, 238),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            submitButton.Click += (s, e) => ValidateAndSubmitForm(firstName, lastName, email, phone, company, message);

            formPanel.Controls.AddRange(new Control[] 
            { 
                formTitle, firstName, lastName, email, phone, company, message, submitButton 
            });

            parent.Controls.Add(formPanel);
            yPosition += 520;
        }

        private void ValidateAndSubmitForm(params BeepMaterialTextField[] fields)
        {
            bool allValid = true;
            foreach (var field in fields)
            {
                string message;
                if (!field.ValidateData(out message))
                {
                    allValid = false;
                }
            }

            if (allValid)
            {
                MessageBox.Show("Form submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please correct the errors in the form.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetupEventHandlers(params BeepMaterialTextField[] textFields)
        {
            foreach (var textField in textFields)
            {
                textField.TextChanged += (s, e) =>
                {
                    var tf = s as BeepMaterialTextField;
                    Console.WriteLine($"Text changed in {tf?.LabelText}: {tf?.Text}");
                };

                textField.LeadingIconClicked += (s, e) =>
                {
                    var tf = s as BeepMaterialTextField;
                    MessageBox.Show($"Leading icon clicked in {tf?.LabelText}", "Icon Clicked");
                };

                textField.TrailingIconClicked += (s, e) =>
                {
                    var tf = s as BeepMaterialTextField;
                    MessageBox.Show($"Trailing icon clicked in {tf?.LabelText}", "Icon Clicked");
                };

                textField.ClearButtonClicked += (s, e) =>
                {
                    var tf = s as BeepMaterialTextField;
                    MessageBox.Show($"Clear button clicked in {tf?.LabelText}", "Clear Clicked");
                };
            }
        }
    }
}