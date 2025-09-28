using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepFormWidget with all form styles
    /// </summary>
    public static class BeepFormWidgetSamples
    {
        /// <summary>
        /// Creates a field group form widget
        /// Uses FieldGroupPainter.cs
        /// </summary>
        public static BeepFormWidget CreateFieldGroupWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.FieldGroup,
                Title = "User Registration",
                Subtitle = "Personal Information",
                Description = "Enter your personal details to create an account",
                ShowValidation = true,
                ShowRequired = true,
                Layout = FormLayout.Vertical,
                Size = new Size(350, 280),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a validation panel form widget
        /// Uses ValidationPanelPainter.cs
        /// </summary>
        public static BeepFormWidget CreateValidationPanelWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.ValidationPanel,
                Title = "Form Validation",
                Subtitle = "Field Verification",
                Description = "Review and fix validation errors before submission",
                ShowValidation = true,
                ValidationMode = FormValidationMode.Realtime,
                Size = new Size(380, 250),
                AccentColor = Color.FromArgb(244, 67, 54)
            };
        }

        /// <summary>
        /// Creates a form section widget
        /// Uses FormSectionPainter.cs
        /// </summary>
        public static BeepFormWidget CreateFormSectionWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.FormSection,
                Title = "Contact Information",
                Subtitle = "Communication Details",
                Description = "Provide your contact details for communication",
                ShowRequired = true,
                Layout = FormLayout.Vertical,
                Size = new Size(320, 200),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates an input card form widget
        /// Uses InputCardPainter.cs
        /// </summary>
        public static BeepFormWidget CreateInputCardWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.InputCard,
                Title = "Email Address",
                Subtitle = "Primary Contact",
                Description = "Enter your primary email address for account verification",
                ShowValidation = true,
                ValidationMode = FormValidationMode.OnChange,
                Size = new Size(300, 180),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a form step widget
        /// Uses FormStepPainter.cs
        /// </summary>
        public static BeepFormWidget CreateFormStepWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.FormStep,
                Title = "Account Setup",
                Subtitle = "Step-by-Step Registration",
                Description = "Complete each step to set up your account",
                ShowProgress = true,
                CurrentStep = 2,
                TotalSteps = 4,
                Size = new Size(400, 300),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a fieldset form widget
        /// Uses FieldSetPainter.cs
        /// </summary>
        public static BeepFormWidget CreateFieldSetWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.FieldSet,
                Title = "Billing Address",
                Subtitle = "Payment Information",
                Description = "Enter billing address for payment processing",
                ShowRequired = true,
                Layout = FormLayout.Grid,
                Size = new Size(350, 220),
                AccentColor = Color.FromArgb(255, 152, 0)
            };
        }

        /// <summary>
        /// Creates an inline form widget
        /// Uses InlineFormPainter.cs
        /// </summary>
        public static BeepFormWidget CreateInlineFormWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.InlineForm,
                Title = "Quick Search",
                Subtitle = "Find Records",
                Description = "Enter search criteria to find records",
                Layout = FormLayout.Horizontal,
                Size = new Size(400, 120),
                AccentColor = Color.FromArgb(103, 58, 183)
            };
        }

        /// <summary>
        /// Creates a compact form widget
        /// Uses CompactFormPainter.cs
        /// </summary>
        public static BeepFormWidget CreateCompactFormWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.CompactForm,
                Title = "Login",
                Subtitle = "Account Access",
                Description = "Enter credentials to access your account",
                Layout = FormLayout.Stacked,
                Size = new Size(280, 160),
                AccentColor = Color.FromArgb(0, 150, 136)
            };
        }

        /// <summary>
        /// Creates a validated input widget
        /// Uses ValidatedInputPainter.cs
        /// </summary>
        public static BeepFormWidget CreateValidatedInputWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.ValidatedInput,
                Title = "Password",
                Subtitle = "Security Verification",
                Description = "Enter a strong password for account security",
                ShowValidation = true,
                ValidationMode = FormValidationMode.Realtime,
                Size = new Size(320, 140),
                AccentColor = Color.FromArgb(63, 81, 181)
            };
        }

        /// <summary>
        /// Creates a form summary widget
        /// Uses FormSummaryPainter.cs
        /// </summary>
        public static BeepFormWidget CreateFormSummaryWidget()
        {
            return new BeepFormWidget
            {
                Style = FormWidgetStyle.FormSummary,
                Title = "Registration Summary",
                Subtitle = "Review Information",
                Description = "Review your information before final submission",
                IsReadOnly = true,
                ShowValidation = false,
                Size = new Size(360, 240),
                AccentColor = Color.FromArgb(233, 30, 99)
            };
        }

        /// <summary>
        /// Gets all form widget samples
        /// </summary>
        public static BeepFormWidget[] GetAllSamples()
        {
            return new BeepFormWidget[]
            {
                CreateFieldGroupWidget(),
                CreateValidationPanelWidget(),
                CreateFormSectionWidget(),
                CreateInputCardWidget(),
                CreateFormStepWidget(),
                CreateFieldSetWidget(),
                CreateInlineFormWidget(),
                CreateCompactFormWidget(),
                CreateValidatedInputWidget(),
                CreateFormSummaryWidget()
            };
        }
    }
}