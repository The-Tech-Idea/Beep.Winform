using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dual Percentage Widget")]
    [Description("A themed control to display two labeled percentages with an icon on the left section.")]
    public class BeepDualPercentageControl : BeepControl
    {
        private BeepLabel lblLeftIcon;
        private BeepLabel lblLeftLabel;
        private BeepLabel lblLeftPercentage;
        private BeepLabel lblRightLabel;
        private BeepLabel lblRightPercentage;
        private BeepLabel lblDivider;

        public BeepDualPercentageControl()
        {
            Height = 60; // Initial height to match the image
            Width = 300; // Initial width to match the image
            BorderRadius = 10;
            Padding = new Padding(10);
            BackColor = Color.White;
            BorderColor = Color.LightGray;

            InitializeComponents();
            ApplyTheme();
        }

        private void InitializeComponents()
        {
            // Left Section Icon
            lblLeftIcon = new BeepLabel
            {
                Text = "🌙", // Placeholder; replace with an actual icon path if available
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // Left Section Label
            lblLeftLabel = new BeepLabel
            {
                Text = "Category 1",
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // Left Section Percentage
            lblLeftPercentage = new BeepLabel
            {
                Text = "34%",
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // Right Section Label
            lblRightLabel = new BeepLabel
            {
                Text = "Category 2",
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = _currentTheme.PrimaryTextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };

            // Right Section Percentage
            lblRightPercentage = new BeepLabel
            {
                Text = "66%",
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = _currentTheme.PrimaryTextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };

            // Divider
            lblDivider = new BeepLabel
            {
                Text = "|",
                Font = _currentTheme.GetBlockTextFont(),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Anchor = AnchorStyles.None // Will be positioned dynamically
            };

            // Add Controls
            Controls.Add(lblLeftIcon);
            Controls.Add(lblLeftLabel);
            Controls.Add(lblLeftPercentage);
            Controls.Add(lblRightLabel);
            Controls.Add(lblRightPercentage);
            Controls.Add(lblDivider);

            // Ensure controls are themed
            foreach (Control control in Controls)
            {
                if (control is BeepControl beepControl)
                {
                    beepControl.Theme = Theme;
                }
            }

            // Initial positioning
            UpdateControlPositions();
        }

        #region Public Properties

        [Category("Data")]
        public string LeftLabelText
        {
            get => lblLeftLabel.Text;
            set => lblLeftLabel.Text = value;
        }

        [Category("Data")]
        public string LeftPercentage
        {
            get => lblLeftPercentage.Text.Replace("%", "");
            set => lblLeftPercentage.Text = $"{value}%";
        }

        [Category("Data")]
        public string RightLabelText
        {
            get => lblRightLabel.Text;
            set => lblRightLabel.Text = value;
        }

        [Category("Data")]
        public string RightPercentage
        {
            get => lblRightPercentage.Text.Replace("%", "");
            set => lblRightPercentage.Text = $"{value}%";
        }

        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) for the left section icon.")]
        public string LeftIconPath
        {
            get => lblLeftIcon.ImagePath;
            set => lblLeftIcon.ImagePath = value;
        }

        [Category("Appearance")]
        public Color LeftSectionColor { get; set; } = Color.FromArgb(200, 144, 238, 144); // Light green as in the image

        #endregion

        #region Overrides

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            UpdateControlPositions(); // Reposition controls on resize
            Invalidate(); // Redraw the control
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateDrawingRect();

            // Draw a light border around the control
            using (Pen borderPen = new Pen(BorderColor, 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            // Draw the colored background for the left section
            using (SolidBrush sectionBrush = new SolidBrush(LeftSectionColor))
            {
                int leftSectionWidth = Width / 2; // Split the control into two halves
                using (GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, leftSectionWidth, Height), BorderRadius))
                {
                    e.Graphics.FillPath(sectionBrush, path);
                }
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.BackColor;
            BorderColor = _currentTheme.BorderColor;

            // Apply theme to child controls
            lblLeftIcon.ForeColor = Color.White;
            lblLeftLabel.ForeColor = Color.White;
            lblLeftPercentage.ForeColor = Color.White;
            lblRightLabel.ForeColor = _currentTheme.PrimaryTextColor;
            lblRightPercentage.ForeColor = _currentTheme.PrimaryTextColor;
            lblDivider.ForeColor = Color.LightGray;

            Invalidate();
        }

        #endregion

        #region Private Methods

        private void UpdateControlPositions()
        {
            int padding = Padding.Left;
            int verticalSpacing = 5;
            int iconSize = 24; // Approximate size for the icon

            // Calculate the width of the left section (half of the control)
            int leftSectionWidth = Width / 2;

            // Left Section: Icon, Label, and Percentage
            // Icon (top-left of the left section)
            lblLeftIcon.Location = new Point(padding, padding);
            lblLeftIcon.Size = new Size(iconSize, iconSize); // Ensure the icon has a fixed size

            // Label (below the icon in the left section)
            lblLeftLabel.Location = new Point(padding, lblLeftIcon.Bottom + verticalSpacing);

            // Percentage (to the right of the icon in the left section)
            lblLeftPercentage.Location = new Point(padding + iconSize + verticalSpacing, padding);

            // Right Section: Label and Percentage
            // Label (top-right of the right section)
            lblRightLabel.Location = new Point(Width - lblRightLabel.Width - padding, padding);

            // Percentage (below the label in the right section)
            lblRightPercentage.Location = new Point(Width - lblRightPercentage.Width - padding, lblRightLabel.Bottom + verticalSpacing);

            // Divider (center between the two sections)
            lblDivider.Location = new Point(leftSectionWidth - (lblDivider.Width / 2), (Height - lblDivider.Height) / 2);
        }

        #endregion
    }
}