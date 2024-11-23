
using TheTechIdea.Beep.Winform.Controls.DataNavigator;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataNavigator : BeepControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete,  btnSave, btnCancel;
        public BeepLabel txtPosition;

        public BeepBindingSource BindingSource { get; set; } = new BeepBindingSource();
        protected int _buttonWidth = 15;
        protected int _buttonHeight = 15;
        public int ButtonWidth
        {
            get => _buttonWidth;
            set
            {
                _buttonWidth = value;
                UpdateDrawingRect();
                ArrangeControls();
                Invalidate();
            }
        }
        public int ButtonHeight
            {
            get => _buttonHeight;
            set
            {
                _buttonHeight = value;
                UpdateDrawingRect();
                ArrangeControls();
                Invalidate();
            }
        }
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
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 30;
            }
        }
        protected override void OnResize(EventArgs e)
        {
          //  base.OnResize(e);
          //  UpdateDrawingRect();
            ArrangeControls();
        }
        private void CreateNavigator()
        {
            UpdateDrawingRect();
            btnFirst = CreateButton("First", btnFirst_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-left.svg");
            btnPrevious = CreateButton("Previous", btnPrevious_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg");
            btnNext = CreateButton("Next", btnNext_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg");
            btnLast = CreateButton("Last", btnLast_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-right.svg");
            btnInsert = CreateButton("Insert", btnInsert_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.add.svg");//
            btnDelete = CreateButton("Delete", btnDelete_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg");// "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg"
            btnSave = CreateButton("Save", btnSave_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.floppy-disk.svg");//"TheTechIdea.Beep.Winform.Controls.GFX.SVG.yes.svg"
            btnCancel = CreateButton("Cancel", btnCancel_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.back-button.svg"); //

            // set isborderaffectedbytheme  and isshadowaffected to false to all controls
            btnFirst.IsBorderAffectedByTheme = false;
            btnFirst.IsShadowAffectedByTheme = false;
            btnPrevious.IsBorderAffectedByTheme = false;
            btnPrevious.IsShadowAffectedByTheme = false;
            btnNext.IsBorderAffectedByTheme = false;
            btnNext.IsShadowAffectedByTheme = false;
            btnLast.IsBorderAffectedByTheme = false;
            btnLast.IsShadowAffectedByTheme = false;
            btnInsert.IsBorderAffectedByTheme = false;
            btnInsert.IsShadowAffectedByTheme = false;
            btnDelete.IsBorderAffectedByTheme = false;
            btnDelete.IsShadowAffectedByTheme = false;
            btnSave.IsBorderAffectedByTheme = false;
            btnSave.IsShadowAffectedByTheme = false;
            btnCancel.IsBorderAffectedByTheme = false;
            btnCancel.IsShadowAffectedByTheme = false;

          

            txtPosition = new BeepLabel { Text = "1 of 1",
                Size = new Size(60, ButtonHeight),
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                Anchor = AnchorStyles.None
            };
          
            txtPosition.IsShadowAffectedByTheme = false;
            txtPosition.IsBorderAffectedByTheme = false;

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
                MaxImageSize = new Size(ButtonWidth-2, ButtonHeight-2),
                Tag = text,
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                Anchor = AnchorStyles.None
            };
            if (!string.IsNullOrEmpty(imagepath))
            {
                btn.MaxImageSize = new Size(ButtonWidth -2, ButtonHeight - 2);
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
            txtPosition.Theme = Theme;
            foreach (Control ctrl in Controls)
            {
                // apply theme to all child controls
                if (ctrl is BeepControl)
                {
                   // ((BeepButton)ctrl).ApplyThemeOnImage = true;
                    ((BeepControl)ctrl).Theme = Theme;
                    // ((BeepControl)ctrl).ApplyTheme();
                    
                }
            }
        }
    }
}
