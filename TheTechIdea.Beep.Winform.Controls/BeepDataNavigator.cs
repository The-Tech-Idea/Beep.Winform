
using TheTechIdea.Beep.Winform.Controls.DataNavigator;
using TheTechIdea.Beep.Editor;
using static System.Net.Mime.MediaTypeNames;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataNavigator : BeepControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete,  btnSave, btnCancel;
        public BeepButton txtPosition;

        public BeepBindingSource BindingSource { get; set; } = new BeepBindingSource();
        protected int _buttonWidth = 15;
        protected int _buttonHeight = 15;
        int drawRectX;//= DrawingRect.X + 1;
        int drawRectY;//= DrawingRect.Y + 1;
        int drawRectWidth;
        int drawRectHeight;
        public int ButtonWidth
        {
            get => _buttonWidth;
            set
            {
                _buttonWidth = value;
              
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
                 adjustedHeight = ButtonHeight + (YOffset * 2);
               
                ArrangeControls();
                Invalidate();
            }
        }
        public int XOffset { get; set; } = 5;
        public int YOffset { get; set; } = 5;
        int adjustedHeight;
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
            ApplyThemeToChilds = false  ;
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = ButtonHeight + (YOffset * 2); // Include YOffset
            }
        }



        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GetDimensions();
           // Height = adjustedHeight + (YOffset * 2); // Adjust height to include YOffset
            ArrangeControls(); // Rearrange controls on resize
        }



        //protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        //{
        //    // Limit the height to fit the buttons and padding
        //     adjustedHeight = ButtonHeight + (YOffset * 2);
        //    base.SetBoundsCore(x, y, width, adjustedHeight, specified);
        //}

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

          

            txtPosition = new BeepButton
            { Text = "1 of 1",
                Size = new Size(ButtonWidth, ButtonHeight),
                MaxImageSize = new Size(ButtonWidth - 2, ButtonHeight - 2),
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
        void GetDimensions()
        {
            UpdateDrawingRect();
            drawRectX = DrawingRect.X ;
            drawRectY = DrawingRect.Y ; // Consider YOffset
            drawRectWidth = DrawingRect.Width ;
            drawRectHeight = DrawingRect.Height ;
            adjustedHeight = ButtonHeight + (YOffset * 2); // Adjusted height includes YOffset
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            GetDimensions();
            // Enforce a fixed height
            height = ButtonHeight + (YOffset * 2);
            base.SetBoundsCore(x, y, width, height+2, specified);
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
            GetDimensions();

            // Calculate total width of all controls
            int totalButtonWidth = (ButtonWidth + buttonSpacing) * 8; // Width for 8 buttons
            int totalLabelWidth = TextRenderer.MeasureText(txtPosition.Text, txtPosition.Font).Width + 10; // Add padding
            int totalWidth = totalButtonWidth + totalLabelWidth + buttonSpacing;

            // Center the controls horizontally
            int centerX = drawRectX + (drawRectWidth - totalWidth) / 2;

            // Center the controls vertically, considering YOffset
            int centerY = drawRectY + YOffset;

            // Arrange navigation buttons
            int currentX = centerX;
            btnFirst.Location = new Point(currentX, centerY);
            currentX = btnFirst.Right + buttonSpacing;

            btnPrevious.Location = new Point(currentX, centerY);
            currentX = btnPrevious.Right + buttonSpacing;

            btnNext.Location = new Point(currentX, centerY);
            currentX = btnNext.Right + buttonSpacing;

            btnLast.Location = new Point(currentX, centerY);
            currentX = btnLast.Right + buttonSpacing;

            // Arrange CRUD buttons
            btnInsert.Location = new Point(currentX, centerY);
            currentX = btnInsert.Right + buttonSpacing;

            btnDelete.Location = new Point(currentX, centerY);
            currentX = btnDelete.Right + buttonSpacing;

            btnSave.Location = new Point(currentX, centerY);
            currentX = btnSave.Right + buttonSpacing;

            btnCancel.Location = new Point(currentX, centerY);
            currentX = btnCancel.Right + buttonSpacing;

            // Arrange txtPosition
            txtPosition.Location = new Point(currentX, centerY);
            txtPosition.Size = new Size(totalLabelWidth, ButtonHeight);
        }





        #region "Event Handlers"
        private void InitializeBindingSourceEvents()
        {
            BindingSource.CurrentChanged += (s, e) => UpdateRecordCountDisplay();
        }

        public void UpdateRecordCountDisplay()
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
        #endregion "Event Handlers"

        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            txtPosition.Theme = Theme;
          //  txtPosition.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            foreach (Control ctrl in Controls)
            {
                // apply theme to all child controls
                if (ctrl is BeepControl)
                {
                    ((BeepControl)ctrl).Theme = Theme;
                    if (ctrl is BeepButton)
                    {
                        ((BeepButton)ctrl).ApplyThemeOnImage = true;
                    }
                   
                
                    
                    // ((BeepControl)ctrl).ApplyTheme();
                    
                }
            }
        }
    }
}
