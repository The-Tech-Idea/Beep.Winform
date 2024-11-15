﻿
using TheTechIdea.Beep.Winform.Controls.DataNavigator;

using TheTechIdea.Beep.Vis.Modules;
using System.Security.Authentication.ExtendedProtection;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataNavigator : BeepControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete,  btnSave, btnCancel;
        public BeepLabel txtPosition;

        public BeepBindingSource BindingSource { get; set; } = new BeepBindingSource();
        public int ButtonWidth { get; set; } = 15;
        public int ButtonHeight { get; set; } = 15;
        public int XOffset { get; set; } = 5;
        public int YOffset { get; set; } = 5;
        // Events for CRUD actions
        public event EventHandler<BeepEventDataArgs> NewRecordCreated;
        public event EventHandler<BeepEventDataArgs> SaveCalled;
        public event EventHandler<BeepEventDataArgs> DeleteCalled;
        public event EventHandler<BeepEventDataArgs> EditCalled;
        int buttonSpacing = 5;
        public int ButtonSpacing
        {
            get => buttonSpacing;
            set
            {
                buttonSpacing = value;
               ArrangeControls();
                Invalidate();
            }
        }
        public BeepDataNavigator()
        {
           
           // UpdateMinimumSize();
            UpdateDrawingRect();
            CreateNavigator();
            InitializeBindingSourceEvents();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            ArrangeControls();
        }
        private void CreateNavigator()
        {
            UpdateDrawingRect();
            btnFirst = CreateButton("First", btnFirst_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnPrevious = CreateButton("Previous", btnPrevious_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnNext = CreateButton("Next", btnNext_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnLast = CreateButton("Last", btnLast_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnInsert = CreateButton("Insert", btnInsert_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnDelete = CreateButton("Delete", btnDelete_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnSave = CreateButton("Save", btnSave_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");
            btnCancel = CreateButton("Cancel", btnCancel_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.FunctionButton.favorite.svg");

          
            txtPosition = new BeepLabel { Text = "1 of 1",
                Size = new Size(60, ButtonHeight),
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Anchor = AnchorStyles.None
            };
          
          
  
            Controls.AddRange(new Control[]
            {
                btnFirst, btnPrevious, btnNext, btnLast,  txtPosition,  
                btnInsert, btnDelete,  btnSave, btnCancel
            });

            ArrangeControls();
           
        }

        private BeepButton CreateButton(string text, EventHandler onClick,string imagepath=null)
        {
            var btn = new BeepButton
            {
                Text = text,
                Size = new Size(ButtonWidth, ButtonHeight),
                MaxImageSize = new Size(ButtonWidth-1, ButtonHeight-1),
                Tag = text,
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                Anchor = AnchorStyles.None
            };
            if (!string.IsNullOrEmpty(imagepath))
            {
                btn.MaxImageSize = new Size(ButtonWidth -1, ButtonHeight - 1);
                btn.ImagePath = imagepath;
                btn.ImageAlign = ContentAlignment.MiddleCenter;
                btn.TextImageRelation = TextImageRelation.ImageAboveText;
                btn.Text = "";
                btn.ToolTipText = text;
            }
            btn.Click += onClick;
            return btn;
        }

        private void ArrangeControls()
        {
            // Get the dimensions of DrawingRect
            int drawRectX = DrawingRect.X;
            int drawRectY = DrawingRect.Y;
            int drawRectWidth = DrawingRect.Width;
            int drawRectHeight = DrawingRect.Height;

            // Calculate the total width of all buttons and spacing in between
            int totalButtonWidth = (ButtonWidth + 5) * 8; // Adjust this based on the actual number of buttons
            int totalLabelWidth = txtPosition.Width;
            int totalWidth = totalButtonWidth + totalLabelWidth;

            // Calculate starting X position to center align the buttons and label horizontally within DrawingRect
            int x = drawRectX + (drawRectWidth - totalWidth) / 2; // Center horizontally within DrawingRect
            int y = drawRectY + (drawRectHeight - ButtonHeight) / 2; // Center vertically within DrawingRect

            // Arrange navigation buttons (first, previous, next, last)
            btnFirst.Location = new Point(x, y);
            btnPrevious.Location = new Point(btnFirst.Right + buttonSpacing, y);
            btnNext.Location = new Point(btnPrevious.Right + buttonSpacing, y);
            btnLast.Location = new Point(btnNext.Right + buttonSpacing, y);

            // Arrange CRUD buttons (insert, delete, save, cancel)
            btnInsert.Location = new Point(btnLast.Right + buttonSpacing, y);
            btnDelete.Location = new Point(btnInsert.Right + buttonSpacing, y);
            btnSave.Location = new Point(btnDelete.Right + buttonSpacing, y);
            btnCancel.Location = new Point(btnSave.Right + buttonSpacing, y);

            // Position the label for current position (txtPosition) after the cancel button
            txtPosition.Size = new Size(60, ButtonHeight);
            txtPosition.Location = new Point(btnCancel.Right + buttonSpacing, y);
            this.MinimumSize = new Size((ButtonWidth * 9)+(buttonSpacing*11) , ButtonHeight + 4); // Set based on layout needs
          //  this.Size = new Size(400, ButtonHeight + 4); // Default start size
        }


        private void InitializeBindingSourceEvents()
        {
            BindingSource.CurrentChanged += (s, e) => UpdateRecordCountDisplay();
        }

        private void UpdateRecordCountDisplay()
        {
            txtPosition.Text = $"{BindingSource.Position + 1} of {BindingSource.Count}";
           
        }

        // Event handlers for CRUD buttons (similar to your initial code)
        // Event handlers for CRUD buttons (fully implemented)
        private void btnFirst_Click(object sender, EventArgs e)
        {
            BindingSource.MoveFirst();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (BindingSource.Position > 0)
                BindingSource.MovePrevious();
            else
                MessageBox.Show("Already at the first record.");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (BindingSource.Position < BindingSource.Count - 1)
                BindingSource.MoveNext();
            else
                MessageBox.Show("Already at the last record.");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            BindingSource.MoveLast();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Insert", null);
            NewRecordCreated?.Invoke(this, args); // Invoke event to notify listeners
            if (!args.Cancel)
                BindingSource.AddNew();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (BindingSource.Current != null)
            {
                var confirmDelete = MessageBox.Show("Are you sure you want to delete this record?", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (confirmDelete == System.Windows.Forms.DialogResult.Yes)
                {
                    var args = new BeepEventDataArgs("Delete", null);
                    DeleteCalled?.Invoke(this, args); // Invoke event to notify listeners
                    if (!args.Cancel)
                        BindingSource.RemoveCurrent();
                }
            }
            else
            {
                MessageBox.Show("No record selected to delete.");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Save", null);
            SaveCalled?.Invoke(this, args); // Invoke event to notify listeners
                                            // Add custom save logic if necessary, e.g., BindingSource.EndEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BindingSource.CancelEdit();
            MessageBox.Show("Changes have been canceled.");
        }
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            foreach (Control ctrl in Controls)
            {
                if (ctrl is BeepButton)
                {
                    ((BeepButton)ctrl).Theme = Theme;
                    ((BeepButton)ctrl).ApplyThemeToSvg();
                }
                else if (ctrl is BeepLabel)
                {
                    ((BeepLabel)ctrl).Theme = Theme;
                }
            }
        }
    }
}
