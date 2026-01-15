using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.DataBlocks.Navigation
{
    /// <summary>
    /// Navigation toolbar for BeepDataBlock - Oracle Forms-like navigation
    /// Auto-generated when block is created
    /// </summary>
    [ToolboxItem(false)] // Not shown in toolbox - auto-added to blocks
    [Category("Beep Controls")]
    [DisplayName("Beep Block Navigation Bar")]
    [Description("Navigation toolbar for BeepDataBlock with First, Previous, Next, Last, Insert, Update, Delete, Save, Cancel buttons")]
    public partial class BeepBlockNavigationBar : BaseControl
    {
        #region Fields

        private BeepDataBlock _parentBlock;
        private BeepButton _btnFirst;
        private BeepButton _btnPrevious;
        private BeepButton _btnNext;
        private BeepButton _btnLast;
        private BeepButton _btnInsert;
        private BeepButton _btnUpdate;
        private BeepButton _btnDelete;
        private BeepButton _btnSave;
        private BeepButton _btnCancel;
        private BeepButton _btnQuery;
        private Label _lblRecordInfo;

        #endregion

        #region Properties

        [Browsable(false)]
        public BeepDataBlock ParentBlock
        {
            get => _parentBlock;
            set
            {
                if (_parentBlock != value)
                {
                    UnsubscribeFromBlock();
                    _parentBlock = value;
                    SubscribeToBlock();
                    UpdateButtonStates();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show navigation buttons (First, Previous, Next, Last)")]
        public bool ShowNavigationButtons { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show CRUD buttons (Insert, Update, Delete)")]
        public bool ShowCRUDButtons { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show query mode toggle button")]
        public bool ShowQueryButton { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show record information label")]
        public bool ShowRecordInfo { get; set; } = true;

        #endregion

        #region Constructor

        public BeepBlockNavigationBar()
        {
            InitializeComponent();
            InitializeButtons();
            Dock = DockStyle.Top;
            Height = 40;
        }

        public BeepBlockNavigationBar(BeepDataBlock parentBlock) : this()
        {
            ParentBlock = parentBlock;
        }

        #endregion

        #region Initialization

        private void InitializeComponent()
        {
            SuspendLayout();
            
            BackColor = Color.FromArgb(240, 240, 240);
            Padding = new Padding(5);
            
            ResumeLayout(false);
        }

        private void InitializeButtons()
        {
            // Navigation Buttons
            _btnFirst = CreateButton("⏮", "First", "Navigate to first record");
            _btnPrevious = CreateButton("⏪", "Previous", "Navigate to previous record");
            _btnNext = CreateButton("⏩", "Next", "Navigate to next record");
            _btnLast = CreateButton("⏭", "Last", "Navigate to last record");

            // CRUD Buttons
            _btnInsert = CreateButton("➕", "Insert", "Insert new record");
            _btnUpdate = CreateButton("✏️", "Update", "Update current record");
            _btnDelete = CreateButton("🗑️", "Delete", "Delete current record");

            // Action Buttons
            _btnSave = CreateButton("💾", "Save", "Save changes");
            _btnCancel = CreateButton("❌", "Cancel", "Cancel changes");

            // Query Button
            _btnQuery = CreateButton("🔍", "Query", "Enter query mode");

            // Record Info Label
            _lblRecordInfo = new Label
            {
                AutoSize = true,
                Text = "No records",
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(10, 0, 10, 0),
                ForeColor = Color.Black
            };

            // Wire up events
            _btnFirst.Click += BtnFirst_Click;
            _btnPrevious.Click += BtnPrevious_Click;
            _btnNext.Click += BtnNext_Click;
            _btnLast.Click += BtnLast_Click;
            _btnInsert.Click += BtnInsert_Click;
            _btnUpdate.Click += BtnUpdate_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += BtnCancel_Click;
            _btnQuery.Click += BtnQuery_Click;

            // Add to controls
            Controls.Add(_btnFirst);
            Controls.Add(_btnPrevious);
            Controls.Add(_btnNext);
            Controls.Add(_btnLast);
            Controls.Add(_btnInsert);
            Controls.Add(_btnUpdate);
            Controls.Add(_btnDelete);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
            Controls.Add(_btnQuery);
            Controls.Add(_lblRecordInfo);
        }

        private BeepButton CreateButton(string text, string name, string toolTip)
        {
            var btn = new BeepButton
            {
                Text = text,
                Name = name,
                ToolTipText = toolTip,
                Size = new Size(35, 30),
                Margin = new Padding(2),
                AutoSize = false
            };
            return btn;
        }

        #endregion

        #region Event Handlers

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            NavigateFirst();
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            NavigatePrevious();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            NavigateNext();
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            NavigateLast();
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            InsertRecord();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateRecord();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CancelChanges();
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            ToggleQueryMode();
        }

        #endregion

        #region Navigation Methods

        private void NavigateFirst()
        {
            try
            {
                if (_parentBlock?.Data?.Units == null) return;
                
                if (_parentBlock.Data.Units.Count > 0)
                {
                    _parentBlock.Data.Units.MoveFirst();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to first: {ex.Message}");
            }
        }

        private void NavigatePrevious()
        {
            try
            {
                if (_parentBlock?.Data?.Units == null) return;
                
                if (_parentBlock.Data.Units.CurrentIndex > 0)
                {
                    _parentBlock.Data.Units.MovePrevious();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating previous: {ex.Message}");
            }
        }

        private void NavigateNext()
        {
            try
            {
                if (_parentBlock?.Data?.Units == null) return;
                
                if (_parentBlock.Data.Units.CurrentIndex < _parentBlock.Data.Units.Count - 1)
                {
                    _parentBlock.Data.Units.MoveNext();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating next: {ex.Message}");
            }
        }

        private void NavigateLast()
        {
            try
            {
                if (_parentBlock?.Data?.Units == null) return;
                
                if (_parentBlock.Data.Units.Count > 0)
                {
                    _parentBlock.Data.Units.MoveLast();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to last: {ex.Message}");
            }
        }

        #endregion

        #region CRUD Methods

        private void InsertRecord()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                // Check if block allows insert based on BlockMode
                if (_parentBlock.BlockMode == DataBlockMode.Query ||
                    _parentBlock.BlockMode == DataBlockMode.ReadOnly)
                {
                    MessageBox.Show("Insert operation not allowed in this block mode.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _parentBlock.Data.New();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateRecord()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                if (_parentBlock.Data.Units?.Current == null)
                {
                    MessageBox.Show("No record selected for update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                await _parentBlock.Data.UpdateAsync(_parentBlock.Data.Units.Current);
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteRecord()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                if (_parentBlock.Data.Units?.Current == null)
                {
                    MessageBox.Show("No record selected for deletion.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    await _parentBlock.Data.DeleteAsync(_parentBlock.Data.Units.Current);
                    UpdateButtonStates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SaveChanges()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                await _parentBlock.Data.Commit();
                UpdateButtonStates();
                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelChanges()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                _parentBlock.Data.Rollback();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error canceling changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleQueryMode()
        {
            try
            {
                if (_parentBlock == null) return;
                
                if (_parentBlock.IsInQueryMode)
                {
                    // Execute query
                    ExecuteQuery();
                }
                else
                {
                    // Enter query mode
                    EnterQueryMode();
                }
                
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling query mode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnterQueryMode()
        {
            if (_parentBlock?.Data == null) return;
            
            // Note: IsInQueryMode property might need to be made public settable
            // For now, use reflection or add a public setter
            _parentBlock.GetType().GetProperty("IsInQueryMode")?.SetValue(_parentBlock, true);
            _btnQuery.Text = "▶️"; // Execute query icon
            _btnQuery.ToolTipText = "Execute Query";
        }

        private async void ExecuteQuery()
        {
            try
            {
                if (_parentBlock?.Data == null) return;
                
                // Execute query based on block's query mode
                await _parentBlock.ExecuteQueryByExampleAsync();
                _parentBlock.GetType().GetProperty("IsInQueryMode")?.SetValue(_parentBlock, false);
                _btnQuery.Text = "🔍"; // Query icon
                _btnQuery.ToolTipText = "Enter Query";
                
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing query: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Block Event Subscription

        private void SubscribeToBlock()
        {
            if (_parentBlock == null) return;

            // Subscribe to block events
            if (_parentBlock.Data?.Units != null)
            {
                _parentBlock.Data.Units.CurrentChanged += Units_CurrentChanged;
            }
        }

        private void UnsubscribeFromBlock()
        {
            if (_parentBlock?.Data?.Units != null)
            {
                _parentBlock.Data.Units.CurrentChanged -= Units_CurrentChanged;
            }
        }

        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            UpdateRecordInfo();
        }

        #endregion

        #region Button State Management

        private void UpdateButtonStates()
        {
            if (_parentBlock?.Data?.Units == null)
            {
                SetAllButtonsEnabled(false);
                return;
            }

            var units = _parentBlock.Data.Units;
            int count = units.Count;
            int currentIndex = units.CurrentIndex;

            // Navigation buttons
            _btnFirst.Enabled = ShowNavigationButtons && count > 0 && currentIndex > 0;
            _btnPrevious.Enabled = ShowNavigationButtons && count > 0 && currentIndex > 0;
            _btnNext.Enabled = ShowNavigationButtons && count > 0 && currentIndex < count - 1;
            _btnLast.Enabled = ShowNavigationButtons && count > 0 && currentIndex < count - 1;

            // CRUD buttons - based on BlockMode
            bool allowModify = _parentBlock.BlockMode == DataBlockMode.CRUD || 
                               _parentBlock.BlockMode == DataBlockMode.Insert;

            bool isInQueryMode = (bool)(_parentBlock.GetType().GetProperty("IsInQueryMode")?.GetValue(_parentBlock) ?? false);

            _btnInsert.Enabled = ShowCRUDButtons && allowModify && !isInQueryMode;
            _btnUpdate.Enabled = ShowCRUDButtons && allowModify && count > 0 && !isInQueryMode;
            _btnDelete.Enabled = ShowCRUDButtons && allowModify && count > 0 && !isInQueryMode;

            // Action buttons
            _btnSave.Enabled = _parentBlock.Data?.IsDirty ?? false;
            _btnCancel.Enabled = _parentBlock.Data?.IsDirty ?? false;

            // Query button
            _btnQuery.Visible = ShowQueryButton;
            _btnQuery.Enabled = _parentBlock.BlockMode != DataBlockMode.ReadOnly;

            UpdateRecordInfo();
        }

        private void SetAllButtonsEnabled(bool enabled)
        {
            _btnFirst.Enabled = enabled;
            _btnPrevious.Enabled = enabled;
            _btnNext.Enabled = enabled;
            _btnLast.Enabled = enabled;
            _btnInsert.Enabled = enabled;
            _btnUpdate.Enabled = enabled;
            _btnDelete.Enabled = enabled;
            _btnSave.Enabled = enabled;
            _btnCancel.Enabled = enabled;
        }

        private void UpdateRecordInfo()
        {
            if (!ShowRecordInfo || _parentBlock?.Data?.Units == null)
            {
                _lblRecordInfo.Text = "";
                return;
            }

            var units = _parentBlock.Data.Units;
            int count = units.Count;
            int currentIndex = units.CurrentIndex + 1;

            bool isInQueryMode = (bool)(_parentBlock.GetType().GetProperty("IsInQueryMode")?.GetValue(_parentBlock) ?? false);

            if (isInQueryMode)
            {
                _lblRecordInfo.Text = "Query Mode";
            }
            else if (count == 0)
            {
                _lblRecordInfo.Text = "No records";
            }
            else
            {
                _lblRecordInfo.Text = $"Record {currentIndex} of {count}";
            }
        }

        #endregion

        #region Layout Management

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            // Arrange buttons horizontally
            int x = Padding.Left;
            int buttonWidth = 35;
            int buttonSpacing = 2;
            int y = Padding.Top;
            int buttonHeight = Height - Padding.Vertical;

            if (ShowNavigationButtons)
            {
                ArrangeButton(_btnFirst, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                ArrangeButton(_btnPrevious, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                ArrangeButton(_btnNext, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                ArrangeButton(_btnLast, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                x += 10; // Separator
            }

            if (ShowCRUDButtons)
            {
                ArrangeButton(_btnInsert, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                ArrangeButton(_btnUpdate, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                ArrangeButton(_btnDelete, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
                x += 10; // Separator
            }

            ArrangeButton(_btnSave, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
            ArrangeButton(_btnCancel, ref x, y, buttonWidth, buttonHeight, buttonSpacing);

            if (ShowQueryButton)
            {
                x += 10; // Separator
                ArrangeButton(_btnQuery, ref x, y, buttonWidth, buttonHeight, buttonSpacing);
            }

            // Position record info label
            _lblRecordInfo.Location = new Point(Width - _lblRecordInfo.Width - Padding.Right, y);
            _lblRecordInfo.Height = buttonHeight;
        }

        private void ArrangeButton(Control button, ref int x, int y, int width, int height, int spacing)
        {
            if (button.Visible)
            {
                button.Location = new Point(x, y);
                button.Size = new Size(width, height);
                x += width + spacing;
            }
        }

        #endregion

        #region Cleanup

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            // Auto-detect parent block
            if (Parent is BeepDataBlock block)
            {
                ParentBlock = block;
            }
            else if (Parent != null)
            {
                // Walk up parent chain to find BeepDataBlock
                var parent = Parent.Parent;
                while (parent != null)
                {
                    if (parent is BeepDataBlock parentBlock)
                    {
                        ParentBlock = parentBlock;
                        break;
                    }
                    parent = parent.Parent;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromBlock();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
