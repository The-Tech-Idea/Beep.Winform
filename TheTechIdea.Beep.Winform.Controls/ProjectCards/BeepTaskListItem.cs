using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
 

 

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Task List Item")]
    [Description("A themed task list item with project, title, time, attachments, and checkbox.")]
    public class BeepTaskListItemControl : BeepControl
    {
        private BeepLabel lblProject;
        private BeepLabel lblTaskTitle;
        private BeepLabel lblStatus;
        private BeepLabel lblTime;
        private BeepButton btnAttachment;
        private BeepCheckBoxBool chkDone;

        public BeepTaskListItemControl()
        {
            Height = 100;
            Width = 500;
            BorderRadius = 3;
            Padding = new Padding(10);
            BackColor = Color.White;
            BorderColor = Color.LightGray;

            InitializeComponents();
            ApplyTheme();
        }

        private void InitializeComponents()
        {
            // Project Label
            lblProject = new BeepLabel
            {
                Text = "Project: Coin Calc",
             
                ForeColor = _currentTheme.SecondaryTextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Left // Anchor to the left
            };

            // Status Label with dot indicator
            lblStatus = new BeepLabel
            {
                Text = "● New",
              
                ForeColor = Color.Goldenrod,
                AutoSize = true,
                Anchor = AnchorStyles.Right // Anchor to the right
            };

            // Checkbox
            chkDone = new BeepCheckBoxBool
            {
                Size = new Size(20, 20),
                CurrentValue = false,
                CheckBoxSize = 15,
                Anchor = AnchorStyles.Top | AnchorStyles.Right // Anchor to top-right
            };

            // Task Title Label
            lblTaskTitle = new BeepLabel
            {
                Text = "BeepTaskList Item",
                
                ForeColor = _currentTheme.PrimaryTextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top // Anchor to top-left
            };

            // Time Label with clock icon
            lblTime = new BeepLabel
            {
                Text = "🕒 2 hrs",
             
                ForeColor = _currentTheme.SecondaryTextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Left // Anchor to the left
            };

            // Attachment Button with paperclip icon
            btnAttachment = new BeepButton
            {
                Text = "2 attachments",
                ImagePath = "paperclip.svg",
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(130, 24),
                BorderRadius = 3,
                BorderColor = Color.LightGray,
                BackColor = Color.White,
                Anchor = AnchorStyles.Left // Anchor to the left
            };

            // Add Controls
            Controls.Add(lblProject);
            Controls.Add(lblStatus);
            Controls.Add(chkDone);
            Controls.Add(lblTaskTitle);
            Controls.Add(lblTime);
            Controls.Add(btnAttachment);

            // Ensure controls are themed
            foreach (Control control in Controls)
            {
                if (control is BeepControl beepControl)
                {
                    beepControl.Theme = Theme;
                }
            }

            // Initial positioning (will be updated on resize)
            UpdateControlPositions();
        }

        #region Public Properties

        [Category("Task")]
        public string ProjectName
        {
            get => lblProject.Text.Replace("Project: ", "");
            set => lblProject.Text = $"Project: {value}";
        }

        [Category("Task")]
        public string TaskTitle
        {
            get => lblTaskTitle.Text;
            set => lblTaskTitle.Text = value;
        }

        [Category("Task")]
        public string TimeSpent
        {
            get => lblTime.Text.Replace("🕒 ", "");
            set => lblTime.Text = $"🕒 {value}";
        }

        [Category("Task")]
        public int AttachmentCount
        {
            get => int.TryParse(btnAttachment.Text.Split(' ')[0], out var count) ? count : 0;
            set => btnAttachment.Text = $"{value} attachments";
        }

        [Category("Task")]
        public bool IsDone
        {
            get => Convert.ToBoolean(chkDone.CurrentValue);
            set => chkDone.CurrentValue = value;
        }

        [Category("Task")]
        public string StatusText
        {
            get => lblStatus.Text.Replace("● ", "");
            set => lblStatus.Text = $"● {value}";
        }

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
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if(lblTime== null || lblTaskTitle == null || lblProject == null || btnAttachment == null || lblStatus == null)
                return;
            BackColor = _currentTheme.TaskCardBackColor;
            BorderColor = _currentTheme.TaskCardBorderColor;
            lblTaskTitle.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockHeaderFont());
            lblTime.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockTextFont());
            btnAttachment.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetButtonFont());
            lblProject.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockHeaderFont());
            lblTime.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockTextFont());
            lblStatus.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockTextFont());
            // Apply theme to the main control
            // Apply theme to child controls
            lblProject.ForeColor = _currentTheme.SecondaryTextColor;
            lblTaskTitle.ForeColor = _currentTheme.PrimaryTextColor;
            lblTime.ForeColor = _currentTheme.SecondaryTextColor;
            lblStatus.ForeColor = Color.Goldenrod;
            btnAttachment.BackColor = _currentTheme.BackColor;
            btnAttachment.ForeColor = _currentTheme.PrimaryTextColor;

            Invalidate();
        }

        #endregion

        #region Private Methods

        private void UpdateControlPositions()
        {
            // Calculate padding and margins
            int padding = Padding.Left;
            int verticalSpacing = 5;
            int statusRightMargin = 10;

            // Project Label (top-left)
            lblProject.Location = new Point(padding, padding);

            // Status Label and Checkbox (top-right)
            int statusX = Width - lblStatus.Width - statusRightMargin - chkDone.Width - padding;
            lblStatus.Location = new Point(statusX, padding);
            chkDone.Location = new Point(Width - chkDone.Width - padding, padding - 3); // Slight offset to align with status

            // Task Title (below project)
            lblTaskTitle.Location = new Point(padding, lblProject.Bottom + verticalSpacing);

            // Time Label (below task title)
            lblTime.Location = new Point(padding, lblTaskTitle.Bottom + verticalSpacing);

            // Attachment Button (to the right of time)
            btnAttachment.Location = new Point(lblTime.Right + verticalSpacing, lblTime.Top);
        }

        #endregion
    }
}