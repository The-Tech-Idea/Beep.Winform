using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DataNavigator;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataNavigator : BeepControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete, btnSave, btnCancel;
        public BeepButton txtPosition;

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
                }
            }
        }
        private IUnitofWork _unitOfWork;

        protected int _buttonWidth = 25;
        protected int _buttonHeight = 25;
        int drawRectX;
        int drawRectY;
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
            UpdateDrawingRect();
            CreateNavigator();
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            ApplyThemeToChilds = false;
            IsChild = false;
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
            btnCancel = CreateButton("Cancel", btnCancel_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.back-button.svg");

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
                Size = new Size(ButtonWidth, ButtonHeight),
                MaxImageSize = new Size(ButtonWidth - 2, ButtonHeight - 2),
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                ApplyThemeOnImage = true,
                Anchor = AnchorStyles.None
            };

            SetThemeEffects(txtPosition);

            Controls.AddRange(new Control[]
            {
                btnFirst, btnPrevious, btnNext, btnLast, txtPosition,
                btnInsert, btnDelete, btnSave, btnCancel
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
            adjustedHeight = ButtonHeight + (YOffset * 2); // Adjusted height includes YOffset
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            GetDimensions();
            // Enforce a fixed height
            height = ButtonHeight + (YOffset * 2);
            base.SetBoundsCore(x, y, width, height + 2, specified);
        }

        private BeepButton CreateButton(string text, EventHandler onClick, string imagepath = null)
        {
            var btn = new BeepButton
            {
                Text = text,
                Size = new Size(ButtonWidth, ButtonHeight),
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

        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            UpdateRecordCountDisplay();
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
        private void btnFirst_Click(object sender, EventArgs e)
        {
            UnitOfWork?.Units.MoveFirst();
            UpdateRecordCountDisplay();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            UnitOfWork?.Units.MovePrevious();
            UpdateRecordCountDisplay();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            UnitOfWork?.Units.MoveNext();
            UpdateRecordCountDisplay();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            UnitOfWork?.Units.MoveLast();
            UpdateRecordCountDisplay();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Insert", null);
            NewRecordCreated?.Invoke(this, args); // Raise event for the parent control to handle
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Delete", null);
            DeleteCalled?.Invoke(this, args); // Raise event for the parent control to handle
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Save", null);
            SaveCalled?.Invoke(this, args); // Raise event for the parent control to handle
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Cancel", null);
            // You may implement cancel logic here if needed
        }

        #endregion "Event Handlers"

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
