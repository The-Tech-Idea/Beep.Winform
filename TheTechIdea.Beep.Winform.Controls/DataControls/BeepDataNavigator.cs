using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.DataControls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum DataNavigatorStyle
    {
        Classic,
        MaterialBar,
        OutlineBar,
        SoftShadowBar,
        GlassBar,
        FlatUnderline,
        Pill
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Data Navigator")]
    [Category("Beep Controls")]
    [Description("A data navigator control for CRUD operations.")]
    public class BeepDataNavigator : BaseControl
    {
        public BeepButton btnFirst, btnPrevious, btnNext, btnLast, btnInsert, btnDelete, btnSave, btnCancel;
        public BeepButton btnQuery, btnFilter,btnPrint,btnEmail;
        public BeepButton txtPosition;
        public bool IsInQueryMode { get; private set; } = false;
        private INavigatorPainter _painter;
        private DataNavigatorStyle _style = DataNavigatorStyle.MaterialBar;
        private Color _accentColor = Color.FromArgb(0, 150, 136);

        public IUnitofWork UnitOfWork
        {
            get => _unitOfWork;
            set
            {
                if (_unitOfWork != value)
                {
                    if (_unitOfWork != null)
                    {
                        _unitOfWork.Units.CurrentChanged -= Units_CurrentChanged;
                    }

                    _unitOfWork = value;

                    if (_unitOfWork != null)
                    {
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
                if (btnEmail != null) btnEmail.Visible = value;
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
                if (btnPrint != null) btnPrint.Visible = value;
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
                ArrangeControls();
                Invalidate();
            }
        }
        public int XOffset { get; set; } = 2;
        public int YOffset { get; set; } = 1;
        int adjustedHeight;

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
            txtPosition.IsFrameless = true;
            txtPosition.MouseEnter += TxtPosition_MouseEnter;
            txtPosition.MouseHover += TxtPosition_MouseHover;
            InitializePainter();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case DataNavigatorStyle.MaterialBar:
                    _painter = new MaterialBarNavigatorPainter();
                    break;
                case DataNavigatorStyle.OutlineBar:
                    _painter = new OutlineBarNavigatorPainter();
                    break;
                case DataNavigatorStyle.SoftShadowBar:
                    _painter = new SoftShadowBarNavigatorPainter();
                    break;
                case DataNavigatorStyle.GlassBar:
                    _painter = new GlassBarNavigatorPainter();
                    break;
                case DataNavigatorStyle.FlatUnderline:
                    _painter = new FlatUnderlineNavigatorPainter();
                    break;
                case DataNavigatorStyle.Pill:
                    _painter = new PillNavigatorPainter();
                    break;
                case DataNavigatorStyle.Classic:
                default:
                    _painter = new OutlineBarNavigatorPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
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
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = ButtonHeight + (YOffset * 2);
            }
            _isinit = false;
            ArrangeControls();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GetDimensions();
            ArrangeControls();
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

            btnQuery = CreateButton("Query", btnQuery_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg");
            btnFilter = CreateButton("Filter", btnFilter_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.filter.svg");
            btnPrint = CreateButton("Print", btnPrint_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg");
            btnEmail = CreateButton("Email", btnEmail_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg");

            SetThemeEffects(btnLast);
            SetThemeEffects(btnInsert);
            SetThemeEffects(btnDelete);
            SetThemeEffects(btnSave);
            SetThemeEffects(btnCancel);
            SetThemeEffects(btnQuery);
            SetThemeEffects(btnFilter);
            SetThemeEffects(btnPrint);
            SetThemeEffects(btnEmail);

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
                IsRoundedAffectedByTheme = false,
                IsRounded = false,
                Anchor = AnchorStyles.None,
                ImageAlign= ContentAlignment.MiddleCenter,
                TextAlign= ContentAlignment.MiddleCenter,
                TextImageRelation= TextImageRelation.TextAboveImage,
                OverrideFontSize = TypeStyleFontSize.Small
            };

            Controls.AddRange(new Control[]
            {
                btnFirst, btnPrevious, btnNext, btnLast, txtPosition,
                btnInsert, btnDelete, btnSave, btnCancel, btnQuery, btnFilter, btnPrint, btnEmail
            });

            ArrangeControls();
        }
        private void SetThemeEffects(BeepButton button)
        {
            button.IsBorderAffectedByTheme = false;
            button.IsShadowAffectedByTheme = false;
            button.IsRoundedAffectedByTheme = false;
        }
        void GetDimensions()
        {
            UpdateDrawingRect();
            drawRectX = DrawingRect.X;
            drawRectY = DrawingRect.Y;
            drawRectWidth = DrawingRect.Width;
            drawRectHeight = DrawingRect.Height;
            txtPosition.SetFont();
            int totalLabelWidth = TextRenderer.MeasureText(txtPosition.Text, txtPosition.Font).Width + 10;
            txtPosition.Size = txtPosition.GetPreferredSize(new Size(totalLabelWidth, txtPosition.Height));

            if(adjustedHeight ==0)
            {
                adjustedHeight = ButtonHeight + ((BorderThickness+YOffset) * 2);
                return;
            }
            if (drawRectHeight < ButtonHeight )
            {
                adjustedHeight= drawRectHeight + ((BorderThickness + YOffset) * 2);
                _buttonHeight = adjustedHeight - ((BorderThickness + YOffset) * 2);
                foreach (var item in Controls)
                {
                    if (item is BeepButton)
                    {
                        ((BeepButton)item).Size = new Size(ButtonWidth, _buttonHeight);
                        ((BeepButton)item).MaxImageSize = new Size(ButtonWidth - 2, _buttonHeight - 2);
                    }

                }
            }
            this.Height = adjustedHeight;
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            GetDimensions();
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
        protected override void DrawContent(Graphics g)
        {
            // Let BaseControl (Material helper) draw first
            base.DrawContent(g);
            UpdateDrawingRect();

            var ctx = new NavigatorLayout
            {
                DrawingRect = DrawingRect,
                Radius = BorderRadius,
                AccentColor = _accentColor,
            };
            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;
            _painter?.DrawBackground(g, ctx);
            _painter?.DrawForeground(g, ctx);
        }
        private void ArrangeControls()
        {
            UpdateDrawingRect();

            bool professionalStyle = false;
            try
            {
                var parentGrid = this.Parent as TheTechIdea.Beep.Winform.Controls.Grid.BeepGrid;
                if (parentGrid != null)
                {
                    var style = (GridX.BeepGridStyle)parentGrid.GridStyle;
                    professionalStyle = (
                        style == GridX.BeepGridStyle.Corporate ||
                        style == GridX.BeepGridStyle.Minimal ||
                        style == GridX.BeepGridStyle.Clean ||
                        style == GridX.BeepGridStyle.Flat ||
                        style == GridX.BeepGridStyle.Material ||
                        style == GridX.BeepGridStyle.Compact ||
                        style == GridX.BeepGridStyle.Card ||
                        style == GridX.BeepGridStyle.Borderless);
                }
            }
            catch { professionalStyle = false; }

            int visibleButtons = 0;
            foreach (var control in Controls)
            {
                if (control is BeepButton button && button.Visible)
                {
                    visibleButtons++;
                }
            }

            int totalButtonWidth = (ButtonWidth + buttonSpacing) * visibleButtons;
            int totalLabelWidth = TextRenderer.MeasureText(txtPosition.Text, txtPosition.Font).Width + 10;
            int totalWidth = totalButtonWidth + totalLabelWidth + buttonSpacing;

            int centerX = DrawingRect.Left + (DrawingRect.Width - totalWidth) / 2;
            int centerY = DrawingRect.Top + (DrawingRect.Height - ButtonHeight) / 2;

            int currentX = centerX;

            if (professionalStyle)
            {
                foreach (var control in Controls)
                {
                    if (control is BeepButton button)
                    {
                        if (button == btnFirst || button == btnPrevious || button == btnNext || button == btnLast)
                        {
                            button.Visible = false;
                            continue;
                        }
                    }
                }

                txtPosition.Size = txtPosition.GetPreferredSize(new Size(totalLabelWidth, ButtonHeight));
                int labelX = DrawingRect.Left + (DrawingRect.Width - txtPosition.Width) / 2;
                txtPosition.Location = new Point(labelX, centerY);
                return;
            }

            foreach (var control in Controls)
            {
                if (control is BeepButton button && button.Visible)
                {
                    button.Size = new Size(ButtonWidth, ButtonHeight);
                    button.Location = new Point(currentX, centerY);
                    currentX += ButtonWidth + buttonSpacing;
                }
            }
            txtPosition.Size = txtPosition.GetPreferredSize(new Size(totalLabelWidth, ButtonHeight));
            txtPosition.Location = new Point(currentX, centerY);
            currentX += txtPosition.Width + buttonSpacing;

            if (ShowSendEmail && btnEmail.Visible)
            {
                btnEmail.Location = new Point(currentX, centerY);
                currentX += ButtonWidth + buttonSpacing;
            }

            if (ShowPrint && btnPrint.Visible)
            {
                btnPrint.Location = new Point(currentX, centerY);
            }
        }
        #region "Event Handlers"

        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            UpdateRecordCountDisplay();
            UpdateNavigationButtonState();
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
            NewRecordCreated?.Invoke(this, args);
            if (args.Cancel)
            {
                return;
            }
            else
            {
                UnitOfWork?.New();
                UpdateRecordCountDisplay();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Delete", null);
            DeleteCalled?.Invoke(this, args);
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
            SaveCalled?.Invoke(this, args);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var args = new BeepEventDataArgs("Rollback", null);
            if (MessageBox.Show(this.Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                RollbackCalled?.Invoke(this, args);
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
            var currentIndex = UnitOfWork.Units.CurrentIndex;
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
            if(txtPosition== null)
            {
                return;
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl is BeepControl)
                {
                    ((BeepControl)ctrl).Theme = Theme;
                }
            }
            txtPosition.Theme = Theme;
            InitializePainter();
            Invalidate();
        }

        [Category("Appearance")]
        public DataNavigatorStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }
    }
}
