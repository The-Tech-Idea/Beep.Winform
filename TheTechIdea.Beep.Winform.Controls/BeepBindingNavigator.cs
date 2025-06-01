using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepBindingNavigator))]
    [Category("Beep Controls")]
    [Description("A flexible binding navigator control for data navigation and CRUD operations using BindingSource.")]
    [DisplayName("Beep Binding Navigator")]
    public partial class BeepBindingNavigator : BeepControl
    {
        #region Fields
        private object _dataSource;
        private BindingSource _bindingsource = new BindingSource();
        private Panel MainPanel;
        private BeepLabel Recordnumberinglabel1;
        private BeepButton FindButton, NewButton, EditButton, PreviousButton, NextButton, RemoveButton, RollbackButton, SaveButton, PrinterButton, MessageButton;
        private int spacing = 5; // Spacing between buttons
        private int labelWidth = 100; // Width of the label
        private Size buttonSize = new Size(16, 16);
        private ToolTip searchtooltip, addtooltip, edittooltip, removetooltip, nexttooltip, previoustooltip, canceltooltip, savetooltip, printtooltip, sharetooltip;
        #endregion

        #region Properties
        public BeepBindingNavigator()
        {
            IsRounded = false;
        }

        public object DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                ConfigureDataSource();
            }
        }

        public BindingSource BindingSource
        {
            get => _bindingsource;
            set
            {
                if (_bindingsource != value)
                {
                    UnsubscribeBindingSourceEvents();
                    _bindingsource = value ?? new BindingSource();
                    SubscribeBindingSourceEvents();
                    datasourcechanged();
                }
            }
        }

        public IDMEEditor DMEEditor { get; set; }
        public bool VerifyDelete { get; set; } = true;
        public int ButtonBorderSize { get; set; } = 0;
        public int ControlHeight { get; set; } = 30;

        // Events
        public event EventHandler<BindingSource> CallPrinter;
        public event EventHandler<BindingSource> SendMessage;
        public event EventHandler<BindingSource> ShowSearch;
        public event EventHandler<BindingSource> NewRecordCreated;
        public event EventHandler<BindingSource> SaveCalled;
        public event EventHandler<BindingSource> DeleteCalled;
        public event EventHandler<BindingSource> EditCalled;
        // New Events for Propagating BindingSource Events
        public event EventHandler CurrentChanged;           // Propagates BindingSource.CurrentChanged
        public event EventHandler<ListChangedEventArgs> ListChanged; // Propagates BindingSource.ListChanged
        public event EventHandler PositionChanged;          // Propagates BindingSource.PositionChanged
        public event EventHandler DataSourceChanged;        // Propagates BindingSource.DataSourceChanged
        #endregion

        #region Initialization and Layout
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            initControls();
        }

        private void initControls()
        {
            Controls.Clear();
            CreateButtons();
            Controls.Add(MainPanel);
            SubscribeBindingSourceEvents();
            datasourcechanged();
        }

        private void CreateButtons()
        {
            SuspendLayout();

            MainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            UpdateDrawingRect();
            MainPanel.Bounds = DrawingRect;

            FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.search_1.svg", buttonSize, FindpictureBox_Click);
            EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.pencil.svg", buttonSize, EditpictureBox_Click);
            PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.print1.svg", buttonSize, PrinterpictureBox_Click);
            MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg", buttonSize, MessagepictureBox_Click);
            SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.check.svg", buttonSize, SavepictureBox_Click);
            PreviousButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.backwards.svg", buttonSize, PreviouspictureBox_Click);
            Recordnumberinglabel1 = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(labelWidth, buttonSize.Height),
                Text = "0",
                ShowAllBorders = true,
                IsRounded = false,
                IsChild = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsRoundedAffectedByTheme = false
            };
            NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.forward.svg", buttonSize, NextpictureBox_Click);
            NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.plus.svg", buttonSize, NewButton_Click);
            RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg", buttonSize, RemovepictureBox_Click);
            RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.go-back.svg", buttonSize, RollbackpictureBox_Click);

            var buttons = new List<Control>
            {
                FindButton, EditButton, PrinterButton, MessageButton, SaveButton,
                PreviousButton, Recordnumberinglabel1, NextButton, NewButton, RemoveButton, RollbackButton
            };
            PositionControls(buttons, spacing);

            ResumeLayout(false);
        }

        private BeepButton CreateButton(string imagePath, Size size, EventHandler clickHandler)
        {
            var button = new BeepButton
            {
                ImagePath = imagePath,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFrameless = true,
                Size = size,
                IsChild = true,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
                Padding = new Padding(0),
                MaxImageSize = new Size(size.Width - 1, size.Height - 1)
            };
            button.Click += clickHandler;
            return button;
        }

        private void PositionControls(List<Control> controls, int spacing)
        {
            if (MainPanel == null || controls == null || controls.Count == 0) return;

            int totalWidth = controls.Sum(c => c.Width) + spacing * (controls.Count - 1);
            int startX = (MainPanel.Width - totalWidth) / 2;
            int currentX = startX;
            int centerY = (MainPanel.Height - controls[0].Height) / 2;

            foreach (var control in controls)
            {
                if (!MainPanel.Controls.Contains(control))
                {
                    MainPanel.Controls.Add(control);
                }
                control.Left = currentX;
                control.Top = centerY;
                currentX += control.Width + spacing;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (MainPanel != null)
            {
                var buttons = new List<Control>
                {
                    FindButton, EditButton, PrinterButton, MessageButton, SaveButton,
                    PreviousButton, Recordnumberinglabel1, NextButton, NewButton, RemoveButton, RollbackButton
                };
                PositionControls(buttons, spacing);
            }
        }
        #endregion

        #region Data Source Configuration
        private void ConfigureDataSource()
        {
            UnsubscribeBindingSourceEvents();

            if (_dataSource == null)
            {
                _bindingsource = new BindingSource();
                SendLog("DataSource is null; defaulting to empty BindingSource.");
            }
            else if (_dataSource is BindingSource bindingSource)
            {
                _bindingsource = bindingSource;
            }
            else if (_dataSource is IBindingList bindingList)
            {
                _bindingsource = new BindingSource { DataSource = bindingList };
            }
            else if (_dataSource is IList list && list.GetType().IsGenericType)
            {
                _bindingsource = new BindingSource { DataSource = list };
            }
            else if (_dataSource is DataTable dataTable)
            {
                _bindingsource = new BindingSource { DataSource = dataTable };
            }
            else if (_dataSource is DataSet dataSet && dataSet.Tables.Count > 0)
            {
                _bindingsource = new BindingSource { DataSource = dataSet.Tables[0] };
            }
            else if (_dataSource is IEnumerable enumerable)
            {
                _bindingsource = new BindingSource { DataSource = enumerable.Cast<object>().ToList() };
            }
            else
            {
                _bindingsource = new BindingSource();
                SendLog("Unsupported DataSource type; defaulting to empty BindingSource.");
            }

            SubscribeBindingSourceEvents();
            datasourcechanged();
        }

        private void SubscribeBindingSourceEvents()
        {
            if (_bindingsource != null)
            {
                _bindingsource.CurrentChanged += BindingSource_CurrentChanged;
                _bindingsource.ListChanged += BindingSource_ListChanged;
                _bindingsource.PositionChanged += BindingSource_PositionChanged;
                _bindingsource.DataSourceChanged += BindingSource_DataSourceChanged;
            }
        }

        private void UnsubscribeBindingSourceEvents()
        {
            if (_bindingsource != null)
            {
                _bindingsource.CurrentChanged -= BindingSource_CurrentChanged;
                _bindingsource.ListChanged -= BindingSource_ListChanged;
                _bindingsource.PositionChanged -= BindingSource_PositionChanged;
                _bindingsource.DataSourceChanged -= BindingSource_DataSourceChanged;
            }
        }

        private void datasourcechanged()
        {
            if (Recordnumberinglabel1 == null || _bindingsource == null) return;
            UpdateRecordNumber();
            UpdateNavigationButtonState();
        }
        #endregion

        #region Event Handlers
        #region Event Handlers
        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            UpdateRecordNumber();
            UpdateNavigationButtonState(); // Add this to refresh button states
            CurrentChanged?.Invoke(this, e); // Propagate to external subscribers
        }

        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            datasourcechanged();
            ListChanged?.Invoke(this, e); // Propagate to external subscribers
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            UpdateRecordNumber();
            PositionChanged?.Invoke(this, e); // Propagate to external subscribers
        }

        private void BindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            datasourcechanged();
            DataSourceChanged?.Invoke(this, e); // Propagate to external subscribers
        }
        #endregion

        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                SendLog("Save Record");
                _bindingsource.EndEdit();
                SaveCalled?.Invoke(sender, _bindingsource);
                MessageBox.Show(Parent, "Record Saved", "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
                MessageBox.Show(Parent, ex.Message, "Beep", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {
                SendLog("Add New Record");
                NewRecordCreated?.Invoke(this, _bindingsource);
                _bindingsource.AddNew();
                _bindingsource.ResetBindings(false);
               
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void RemovepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                SendLog("Remove Record");
                DeleteCalled?.Invoke(sender, _bindingsource);
                if (_bindingsource.Current != null && _bindingsource.Count > 0)
                {
                    if (VerifyDelete && MessageBox.Show(Parent, "Are you sure you want to Delete Record?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        return;

                    _bindingsource.RemoveCurrent();
                    _bindingsource.ResetBindings(false);
                }
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void NextpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingsource.Count > 0 && _bindingsource.Position + 1 < _bindingsource.Count)
                {
                    _bindingsource.MoveNext();
                    SendLog("Next Record");
                }
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void PreviouspictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                SendLog("Previous Record");
                if (_bindingsource.Count > 0 && _bindingsource.Position > 0)
                {
                    _bindingsource.MovePrevious();
                 
                }
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void RollbackpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    _bindingsource.CancelEdit();
                    _bindingsource.ResetBindings(false);
                }
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void EditpictureBox_Click(object sender, EventArgs e) => EditCalled?.Invoke(sender, _bindingsource);
        private void FindpictureBox_Click(object sender, EventArgs e) => ShowSearch?.Invoke(this, _bindingsource);
        private void PrinterpictureBox_Click(object sender, EventArgs e) => CallPrinter?.Invoke(this, _bindingsource);
        private void MessagepictureBox_Click(object sender, EventArgs e) => SendMessage?.Invoke(this, _bindingsource);
        #endregion

        #region Helpers
        private void UpdateRecordNumber()
        {
            if (Recordnumberinglabel1 == null || _bindingsource == null) return;
            if (_bindingsource.Count > 0)
            {
                Recordnumberinglabel1.Text = $"{_bindingsource.Position + 1} From {_bindingsource.Count}";
            }
            else
            {
                Recordnumberinglabel1.Text = "-";
            }
        }

        private void UpdateNavigationButtonState()
        {
            PreviousButton.Enabled = _bindingsource != null && _bindingsource.Position > 0;
            NextButton.Enabled = _bindingsource != null && _bindingsource.Position < _bindingsource.Count - 1;
            RemoveButton.Enabled = _bindingsource != null && _bindingsource.Count > 0;
            SaveButton.Enabled = _bindingsource != null && _bindingsource.Count > 0;
        }

       

        public override void ApplyTheme()
        {
            if (_currentTheme == null || MainPanel == null) return;
            BackColor = _currentTheme.PanelBackColor;
            MainPanel.BackColor = _currentTheme.PanelBackColor;

            foreach (var button in new[] { FindButton, NextButton, PreviousButton, RemoveButton, SaveButton, RollbackButton, EditButton, NewButton, PrinterButton, MessageButton })
            {
                if (button != null) button.Theme = Theme;
            }

            if (Recordnumberinglabel1 != null)
            {
                Recordnumberinglabel1.Font = BeepThemesManager_v2.ToFont(_currentTheme.LabelSmall);
                Recordnumberinglabel1.BackColor = _currentTheme.LabelBackColor;
                Recordnumberinglabel1.ForeColor = _currentTheme.LabelForeColor;
            }

            // Tooltips
            searchtooltip = SetupToolTip(searchtooltip, FindButton, "Search");
            addtooltip = SetupToolTip(addtooltip, NewButton, "Add New Record");
            edittooltip = SetupToolTip(edittooltip, EditButton, "Edit Record");
            removetooltip = SetupToolTip(removetooltip, RemoveButton, "Remove Record");
            nexttooltip = SetupToolTip(nexttooltip, NextButton, "Next Record");
            previoustooltip = SetupToolTip(previoustooltip, PreviousButton, "Previous Record");
            canceltooltip = SetupToolTip(canceltooltip, RollbackButton, "Cancel Changes");
            savetooltip = SetupToolTip(savetooltip, SaveButton, "Save Record");
            printtooltip = SetupToolTip(printtooltip, PrinterButton, "Print Record");
            sharetooltip = SetupToolTip(sharetooltip, MessageButton, "Share Record");
        }

        private ToolTip SetupToolTip(ToolTip tooltip, Control control, string text)
        {
            if (tooltip == null)
            {
                tooltip = new ToolTip
                {
                    BackColor = _currentTheme?.ToolTipBackColor ?? SystemColors.Info,
                    ForeColor = _currentTheme?.ToolTipForeColor ?? SystemColors.InfoText
                };
            }
            tooltip.SetToolTip(control, text);
            return tooltip;
        }
        #endregion
        private void SendLog(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}