using TheTechIdea.Beep.Editor;


namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataNavigator : BeepControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete, btnSave, btnCancel;
        public BeepButton btnQuery, btnFilter,btnPrint,btnEmail;
        public BeepButton txtPosition;
        public bool IsInQueryMode { get; private set; } = false;

        public IUnitofWork UnitOfWork
        {
            get => _unitOfWork;
            set
            {
                if (_unitOfWork != value)
                {
                    if (_unitOfWork != null)
                    {
                        // Unsubscribe from previous events
                        _unitOfWork.Units.CurrentChanged -= Units_CurrentChanged;
                    }

                    _unitOfWork = value;

                    if (_unitOfWork != null)
                    {
                        // Subscribe to new events
                        _unitOfWork.Units.CurrentChanged += Units_CurrentChanged;
                    }
                    UpdateRecordCountDisplay();
                    UpdateNavigationButtonState();
                }
            }
        }
        private IUnitofWork _unitOfWork;

        protected int _buttonWidth = 15;
        protected int _buttonHeight = 15;
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        private bool _showsendemail=false;
        private bool _showprint = false;
        public bool ShowSendEmail
        {
            get => _showsendemail;
            set
            {
                _showsendemail = value;
                btnEmail.Visible = value;
                ArrangeControls();
                Invalidate();
            }
        }
        public bool ShowPrint
        {
            get => _showprint;
            set
            {
                _showprint = value;
                btnPrint.Visible = value;
                ArrangeControls();
                Invalidate();
            }
        }

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
               // adjustedHeight = ButtonHeight + (YOffset * 2);
                ArrangeControls();
                Invalidate();
            }
        }

        public int XOffset { get; set; } = 2;
        public int YOffset { get; set; } = 1;
        int adjustedHeight;

        // Events for CRUD actions
        public event EventHandler<BeepEventDataArgs> NewRecordCreated;
        public event EventHandler<BeepEventDataArgs> SaveCalled;
        public event EventHandler<BeepEventDataArgs> DeleteCalled;
        public event EventHandler<BeepEventDataArgs> EditCalled;
        public event EventHandler<BeepEventDataArgs> RollbackCalled;
        public event EventHandler<BeepEventDataArgs> QueryCalled;
        public event EventHandler<BeepEventDataArgs> FilterCalled;
        public event EventHandler<BeepEventDataArgs> EmailCalled;
        public event EventHandler<BeepEventDataArgs> PrintCalled;

        int buttonSpacing = 5;
        private bool _isinit;

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
            UpdateDrawingRect();
            CreateNavigator();
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            ApplyThemeToChilds = false;
            IsChild = false;
            txtPosition.IsFramless = true;
            txtPosition.MouseEnter += TxtPosition_MouseEnter;
            txtPosition.MouseHover += TxtPosition_MouseHover;
        }

        private void TxtPosition_MouseEnter(object? sender, EventArgs e)
        {
            txtPosition.IsHovered = false;
        }

        private void TxtPosition_MouseHover(object? sender, EventArgs e)
        {
            txtPosition.IsHovered = false;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            _isinit = true;
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = ButtonHeight + (YOffset * 2); // Include YOffset
            }
            _isinit = false;
            ArrangeControls();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GetDimensions();
            ArrangeControls(); // Rearrange controls on resize
        }

        private void CreateNavigator()
        {
            UpdateDrawingRect();
            btnFirst = CreateButton("First", btnFirst_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-left.svg");
            btnPrevious = CreateButton("Previous", btnPrevious_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg");
            btnNext = CreateButton("Next", btnNext_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg");
            btnLast = CreateButton("Last", btnLast_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-right.svg");
            btnInsert = CreateButton("Insert", btnInsert_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.add.svg");
            btnDelete = CreateButton("Delete", btnDelete_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg");
            btnSave = CreateButton("Save", btnSave_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.floppy-disk.svg");
            btnCancel = CreateButton("Rollback", btnCancel_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.back-button.svg");

            // Set theme effects to false for all buttons
            SetThemeEffects(btnFirst);
            SetThemeEffects(btnPrevious);
            SetThemeEffects(btnNext);
            SetThemeEffects(btnLast);
            SetThemeEffects(btnInsert);
            SetThemeEffects(btnDelete);
            SetThemeEffects(btnSave);
            SetThemeEffects(btnCancel);

            txtPosition = new BeepButton
            {
                Text = "1 of 1",
                Size = new Size(ButtonWidth - 1, ButtonHeight - 1),
                MaxImageSize = new Size(ButtonWidth - 2, ButtonHeight - 2),
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                ApplyThemeOnImage = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.None,
                ImageAlign= ContentAlignment.MiddleCenter,
                TextAlign= ContentAlignment.MiddleCenter,
                TextImageRelation= TextImageRelation.TextAboveImage,
                OverrideFontSize = TypeStyleFontSize.Small
            };

           

            btnQuery = CreateButton("Query", btnQuery_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg");
            btnFilter = CreateButton("Filter", btnFilter_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.filter.svg");
            btnPrint = CreateButton("Print", btnPrint_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg");
            btnEmail = CreateButton("Email", btnEmail_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg");
            SetThemeEffects(btnQuery);
            SetThemeEffects(btnFilter);
            SetThemeEffects(btnPrint);
            SetThemeEffects(btnEmail);

            Controls.AddRange(new Control[]
            {
                btnFirst, btnPrevious, btnNext, btnLast, txtPosition,
                btnInsert, btnDelete, btnSave, btnCancel,btnQuery,btnFilter,btnPrint,btnEmail
            });




            ArrangeControls();
        }

        private void SetThemeEffects(BeepButton button)
        {
            button.IsBorderAffectedByTheme = false;
            button.IsShadowAffectedByTheme = false;
        }

        void GetDimensions()
        {
            UpdateDrawingRect();
            drawRectX = DrawingRect.X;
            drawRectY = DrawingRect.Y;
            drawRectWidth = DrawingRect.Width;
            drawRectHeight = DrawingRect.Height;
            txtPosition.SetFont();
            int totalLabelWidth = TextRenderer.MeasureText(txtPosition.Text, txtPosition.Font).Width + 10; // Add padding
            txtPosition.Size = txtPosition.GetPreferredSize(new Size(totalLabelWidth, txtPosition.Height));
         
            if(adjustedHeight ==0)
            {
                adjustedHeight = ButtonHeight + ((BorderThickness+YOffset) * 2);
                // _buttonHeight = adjustedHeight - ((BorderThickness + YOffset) * 2);
                return;
            }
            if (drawRectHeight < ButtonHeight )
            {
                adjustedHeight= drawRectHeight + ((BorderThickness + YOffset) * 2);
                _buttonHeight = adjustedHeight - ((BorderThickness + YOffset) * 2);
                // resize all button heights
                foreach (var item in Controls)
                {
                    if (item is BeepButton)
                    {
                        ((BeepButton)item).Size = new Size(ButtonWidth, _buttonHeight);
                        ((BeepButton)item).MaxImageSize = new Size(ButtonWidth - 2, _buttonHeight - 2);
                    }

                }
            }
          
         //   adjustedHeight = _buttonHeight + (YOffset * 2); // Adjusted height includes YOffset
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            GetDimensions();
            // Enforce a fixed height
           // if(txtPosition.Height > adjustedHeight)
           // {
              // adjustedHeight= txtPosition.Height+(YOffset*2);
           // }
            base.SetBoundsCore(x, y, width, adjustedHeight, specified);
        }

        private BeepButton CreateButton(string text, EventHandler onClick, string imagepath = null)
        {
            var btn = new BeepButton
            {
                Text = text,
                Size = new Size(ButtonWidth-1, ButtonHeight-1),
                MaxImageSize = new Size(ButtonWidth - 2, ButtonHeight - 2),
                Tag = text,
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                ApplyThemeOnImage = true,
                Anchor = AnchorStyles.None
            };
            if (!string.IsNullOrEmpty(imagepath))
            {
                btn.MaxImageSize = new Size(ButtonWidth - 2, ButtonHeight - 2);
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
            int noOfButtons = 12;
          
            GetDimensions();
            if(ShowSendEmail)
            {
                btnEmail.Visible = true;
             
            }
            else
            {
                noOfButtons -= 1;
                btnEmail.Visible = false;
            }
            if(ShowPrint)
            {
                btnPrint.Visible = true;
              
            }
            else
            {
                noOfButtons -= 1;
                btnPrint.Visible = false;
            }
            // Calculate total width of all controls
            int totalButtonWidth = (ButtonWidth + buttonSpacing) * noOfButtons; // Width for 8 buttons
            int totalLabelWidth = TextRenderer.MeasureText(txtPosition.Text, txtPosition.Font).Width + 10; // Add padding
            int totalWidth = totalButtonWidth + totalLabelWidth + buttonSpacing;

            // Center the controls horizontally
            int centerX = drawRectX + (drawRectWidth - totalWidth) / 2;

            // Center the controls vertically, considering YOffset
            int centerY = drawRectY + (YOffset);

            // Arrange navigation buttons
            int currentX = centerX;
            btnQuery.Location = new Point(currentX, centerY);
            currentX = btnQuery.Right + buttonSpacing;
            btnFilter.Location = new Point(currentX, centerY);
            currentX = btnFilter.Right + buttonSpacing;
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
            txtPosition.Size = txtPosition.GetPreferredSize(new Size(totalLabelWidth, txtPosition.Height));
            // Center the text vertically, considering smaller height for the label
            int textCenterY = drawRectY + (drawRectHeight - txtPosition.Height) / 2;

            txtPosition.Location = new Point(currentX, textCenterY);
            currentX = txtPosition.Right + buttonSpacing;
            if (ShowSendEmail)
            {
               btnEmail.Location = new Point(currentX, centerY);
            }
            if (ShowPrint)
            {
                if (ShowSendEmail)
                {
                    currentX = btnEmail.Right + buttonSpacing;
                }
                else
                {
                    currentX = txtPosition.Right + buttonSpacing;
                }

                btnPrint.Location = new Point(currentX, centerY);
            }
        
           
        }

        #region "Event Handlers"

        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            UpdateRecordCountDisplay();
            UpdateNavigationButtonState(); // Update buttons when the current index changes
        }


        public void UpdateRecordCountDisplay()
        {
            if (UnitOfWork != null && UnitOfWork.Units != null)
            {
                int position = UnitOfWork.Units.CurrentIndex + 1;
                int count = UnitOfWork.Units.Count;
                txtPosition.Text = $"{position} of {count}";
            }
            else
            {
                txtPosition.Text = "0 of 0";
            }
        }

        // Event handlers for navigation buttons
        private void btnQuery_Click(object sender, EventArgs e)
        {
            QueryCalled?.Invoke(this, new BeepEventDataArgs("Query", null));
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterCalled?.Invoke(this, new BeepEventDataArgs("Filter", null));

        }
        private void btnEmail_Click(object sender, EventArgs e)
        {
            EmailCalled?.Invoke(this, new BeepEventDataArgs("Email", null));
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintCalled?.Invoke(this, new BeepEventDataArgs("Print", null));
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            UnitOfWork?.MoveFirst();

            UpdateRecordCountDisplay();
            UpdateNavigationButtonState();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            UnitOfWork?.MovePrevious();
            UpdateRecordCountDisplay();
            UpdateNavigationButtonState();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            UnitOfWork?.MoveNext();
            UpdateRecordCountDisplay();
            UpdateNavigationButtonState();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            UnitOfWork?.MoveLast();
            UpdateRecordCountDisplay();
            UpdateNavigationButtonState();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Insert", null);
            NewRecordCreated?.Invoke(this, args); // Raise event for the parent control to handle
            if (args.Cancel)
            {
                return;
            }
            else
            {
                UnitOfWork?.Create();
                UpdateRecordCountDisplay();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Delete", null);
            DeleteCalled?.Invoke(this, args); // Raise event for the parent control to handle
            if(args.Cancel)
            {
                return;
            }
            else
            {
                UnitOfWork?.Delete();
                UpdateRecordCountDisplay();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Save", null);
            SaveCalled?.Invoke(this, args); // Raise event for the parent control to handle
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Rollback", null);
            if (MessageBox.Show(this.Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                RollbackCalled?.Invoke(this, args); // Raise event for the parent control to handle
                if (args.Cancel)
                {
                    return;
                }
                else
                {
                    UnitOfWork?.Rollback();
                    UpdateRecordCountDisplay();
                }
            }
        }

        #endregion "Event Handlers"
        private void NotifyUser(string message, bool isError = false)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        private void HighlightCurrentRecord()
        {
            // Assuming UnitOfWork.Units.CurrentIndex is exposed
            var currentIndex = UnitOfWork.Units.CurrentIndex;
            // Update UI styling to reflect the current record
        }

        private void UpdateNavigationButtonState()
        {
            btnFirst.Enabled = UnitOfWork != null && UnitOfWork.Units.CurrentIndex > 0;
            btnPrevious.Enabled = UnitOfWork != null && UnitOfWork.Units.CurrentIndex > 0;
            btnNext.Enabled = UnitOfWork != null && UnitOfWork.Units.CurrentIndex < UnitOfWork.Units.Count - 1;
            btnLast.Enabled = UnitOfWork != null && UnitOfWork.Units.CurrentIndex < UnitOfWork.Units.Count - 1;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
                btnPrevious_Click(null, null);
            else if (keyData == Keys.Right)
                btnNext_Click(null, null);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            BackColor = _currentTheme.ButtonBackColor;
            foreach (Control ctrl in Controls)
            {
                // apply theme to all child controls
                if (ctrl is BeepControl)
                {
                    ((BeepControl)ctrl).Theme = Theme;
                }
            }
            Invalidate();
        }
    }
}
