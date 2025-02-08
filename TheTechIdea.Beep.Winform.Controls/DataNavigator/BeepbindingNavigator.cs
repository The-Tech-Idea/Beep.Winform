
using System.ComponentModel;
using System.Data;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;



namespace TheTechIdea.Beep.Winform.Controls.BindingNavigator
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepBindingNavigator))] // Optional//"Resources.BeepButtonIcon.bmp"
    [Category("Beep Controls")]
    [Description("Beep Binding Navigator")]
    [DisplayName("Beep Binding Navigator")]
    public partial class BeepBindingNavigator : BeepControl
    {
        public BeepBindingNavigator()
        {


        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            initControls();

        }
        private Panel MainPanel;
        TableLayoutPanel layoutPanel;
        private BeepLabel Recordnumberinglabel1;
        private BeepButton FindButton;
        private BeepButton NewButton;
        private BeepButton EditButton;
        private BeepButton PrevoiusButton;
        private BeepButton NextButton;
        private BeepButton RemoveButton;
        private BeepButton RollbackButton;
        private BeepButton SaveButton;
        private BeepButton PrinterButton;
        private BeepButton MessageButton;
        int spacing = 5; // Spacing between buttons
        int labelWidth = 100; // Width of the label
        int totalControls = 11; // Number of controls
     

        public IUnitofWork<Entity> UnitofWork { get; set; }
        public event EventHandler<BindingSource> CallPrinter;
        public event EventHandler<BindingSource> SendMessage;
        public event EventHandler<BindingSource> ShowSearch;
        public event EventHandler<BindingSource> NewRecordCreated;
        public event EventHandler<BindingSource> SaveCalled;
        public event EventHandler<BindingSource> DeleteCalled;
        public event EventHandler<BindingSource> EditCalled;

        // VisManager AppManager;
        // ImportDataManager importDataManager;
        public IDMEEditor DMEEditor { get; set; }
        private BindingSource _bindingsource;
        public BindingSource BindingSource
        {
            get { return _bindingsource; }
            set
            {
                _bindingsource = value;
                if (_bindingsource != null)
                {
                    datasourcechanged();
                }
            }
        }
        public bool VerifyDelete { get; set; } = true;
        public int ButtonBorderSize { get; set; } = 0;
        public int ControlHeight { get; set; } = 30;
        private Color _HightlightColor;
        private Color _SelectedColor;

        private ToolTip searchtooltip;
        private ToolTip addtooltip;
        private ToolTip edittooltip;
        private ToolTip removetooltip;
        private ToolTip nexttooltip;
        private ToolTip previoustooltip;
        private ToolTip canceltooltip;
        private ToolTip savetooltip;
        private ToolTip printtooltip;
        private ToolTip sharetooltip;
        private Size buttonSize = new Size(14, 14);
 
        private void initControls()
        {
            Controls.Clear();
            // InitPanels();
           // Console.WriteLine("InitLayout");
            _bindingsource = new BindingSource();
          //  Console.WriteLine("InitLayout 1");
           
          //  Console.WriteLine("InitLayout 2");
            CreateButtons();
           
            Controls.Add(MainPanel);
            this.BindingSource.DataSourceChanged += BindingSource_DataSourceChanged;
            this.BindingSource.ListChanged += BindingSource_ListChanged;
            this.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
           // Console.WriteLine("InitLayout 3");
            this.PrevoiusButton.Click -= PreviouspictureBox_Click;
            this.NextButton.Click -= NextpictureBox_Click;
            this.RemoveButton.Click -= RemovepictureBox_Click;
            this.SaveButton.Click -= SavepictureBox_Click;
            this.RollbackButton.Click -= RollbackpictureBox_Click;
            this.EditButton.Click -= EditpictureBox_Click;
            this.FindButton.Click -= FindpictureBox_Click;
            this.PrinterButton.Click -= PrinterpictureBox_Click;
            this.MessageButton.Click -= MessegepictureBox_Click;
            this.NewButton.Click -= NewButton_Click;

            this.NewButton.Click += NewButton_Click;
            this.PrevoiusButton.Click += PreviouspictureBox_Click;
            this.NextButton.Click += NextpictureBox_Click;
            this.RemoveButton.Click += RemovepictureBox_Click;
            this.SaveButton.Click += SavepictureBox_Click;
            this.RollbackButton.Click += RollbackpictureBox_Click;
            this.EditButton.Click += EditpictureBox_Click;
            this.FindButton.Click += FindpictureBox_Click;
            this.PrinterButton.Click += PrinterpictureBox_Click;
            this.MessageButton.Click += MessegepictureBox_Click;

           // Console.WriteLine("InitLayout 4");
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (MainPanel == null) return;
            BackColor = _currentTheme.PanelBackColor;
            MainPanel.BackColor = _currentTheme.PanelBackColor;
            Recordnumberinglabel1.Theme = Theme;
          //  Recordnumberinglabel1.BackColor = ColorUtils.GetForColor(_currentTheme.LabelBackColor, _currentTheme.LabelForeColor);
        }
        #region "Click Methods for all Buttons"

        private void MessegepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //  {
                // BindingSource.CancelEdit();
                //FocusPicture(sender);
                SendMessage?.Invoke(this, BindingSource);
                // };

            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }
        private void PrinterpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //  {
                // BindingSource.CancelEdit();
                //      FocusPicture(sender);
                CallPrinter?.Invoke(this, BindingSource);
                // };

            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }
        private void FindpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //  {
                // BindingSource.CancelEdit();
                //FocusPicture(sender);
                ShowSearch?.Invoke(this, BindingSource);
                // };

            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }
        private void EditpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //{
                //  BindingSource.CancelEdit();
                //      FocusPicture(sender);
                // };
                EditCalled?.Invoke(sender, BindingSource);
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
                if (MessageBox.Show(this.Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    BindingSource.CancelEdit();
                    BindingSource.ResetBindings(false);
                    //      FocusPicture(sender);
                };

            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }
        }
        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                BindingSource.EndEdit();
                SaveCalled?.Invoke(sender, BindingSource);
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
                DeleteCalled?.Invoke(sender, BindingSource);
                if (BindingSource.Current != null)
                {
                    if (BindingSource.Count > 0)
                    {
                        if (VerifyDelete)
                        {
                            if (MessageBox.Show(this.Parent, "Are you sure you want to Delete Record?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                BindingSource.RemoveCurrent();
                                BindingSource.ResetBindings(false);
                            };
                        }
                        else
                        {
                            BindingSource.RemoveCurrent();
                            BindingSource.ResetBindings(false);
                        }
                    }
                }


                //FocusPicture(sender);
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
                if (BindingSource.Count > 0)
                    if (BindingSource[BindingSource.Count - 1] != null)
                        if (BindingSource.Position + 1 < BindingSource.Count)
                            BindingSource.MoveNext();
                ;

                // FocusPicture(sender);
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
                if (BindingSource.Count > 0)
                    if (BindingSource[BindingSource.Count - 1] != null)
                        if (BindingSource.Position > 0)
                            BindingSource.MovePrevious();
                ;
                //  FocusPicture(sender);
            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator {ex.Message}");
            }

        }
        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {

                BindingSource.AddNew();
                BindingSource.ResetBindings(false);
                NewRecordCreated?.Invoke(this, BindingSource);

            }
            catch (Exception ex)
            {
                SendLog($"Binding Navigator { ex.Message}");
            }
        }

        #endregion
        #region "BindingSource Events"
        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            datasourcechanged();

        }

        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            datasourcechanged();
        }
        private void BindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            if (Recordnumberinglabel1 == null) return;
            datasourcechanged();


        }
        private void datasourcechanged()
        {
            if (Recordnumberinglabel1 == null) return;
            if (this.BindingSource != null)
            {
              
                if (this.BindingSource.List != null)
                {
                    this.Recordnumberinglabel1.Text = (Convert.ToInt32(BindingSource.Position + 1)).ToString() + " From " + BindingSource.List.Count.ToString();
                }
                else
                    this.Recordnumberinglabel1.Text = "-";
            }
        }

        #endregion
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Call PositionControls again to adjust positions
            List<Control> buttons = new List<Control>
    {
        FindButton, EditButton, PrinterButton, MessageButton, SaveButton,
        PrevoiusButton, Recordnumberinglabel1, NextButton, NewButton, RemoveButton, RollbackButton
    };

            PositionControls(buttons, 5); // Adjust layout on resize
        }
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        #region "Resource Loaders"


        #endregion
        private void SendLog(string message)
        {
            if(DMEEditor!=null) { DMEEditor.AddLogMessage("Binding Navigator", message, DateTime.Now, BindingSource.Position, null, Errors.Failed); 
            }else
            MessageBox.Show(message, "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CreateButtons()
        {
            SuspendLayout();

            // Ensure MainPanel is created
            if (MainPanel == null)
            {
                MainPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
            }

            UpdateDrawingRect();

            // Set MainPanel size and position
            MainPanel.Left = DrawingRect.Left;
            MainPanel.Top = DrawingRect.Top;
            MainPanel.Width = DrawingRect.Width;
            MainPanel.Height = DrawingRect.Height;

            Controls.Clear();
            Controls.Add(MainPanel);

            // Define Button Size
          
           

            // Create Buttons
            FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg", buttonSize);
            EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.edit.svg", buttonSize);
            PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg", buttonSize);
            MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.share.svg", buttonSize);
            SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.save.svg", buttonSize);
            PrevoiusButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.previous.svg", buttonSize);
            Recordnumberinglabel1 = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(labelWidth, buttonSize.Height),
                Text = "0",
                ShowAllBorders = true,
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsRoundedAffectedByTheme = false
            };
            NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.next.svg", buttonSize);
            NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.add.svg", buttonSize);
            RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.remove.svg", buttonSize);
            RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.rollback.svg", buttonSize);

            // Add all controls to a list
            List<Control> buttons = new List<Control>
    {
        FindButton, EditButton, PrinterButton, MessageButton, SaveButton,
        PrevoiusButton, Recordnumberinglabel1, NextButton, NewButton, RemoveButton, RollbackButton
    };

            // Call function to position controls dynamically
            PositionControls(buttons, spacing);

            ResumeLayout(false);
        }
        private void PositionControls(List<Control> controls, int spacing)
        {
            if (MainPanel == null || controls == null || controls.Count == 0)
                return;

            int totalWidth = controls.Sum(c => c.Width) + (spacing * (controls.Count - 1));
            int startX = (MainPanel.Width - totalWidth) / 2;
            int currentX = startX;
            int centerY = (MainPanel.Height - controls[0].Height) / 2;

            foreach (var control in controls)
            {
                if (!MainPanel.Controls.Contains(control))
                {
                    MainPanel.Controls.Add(control); // Add control ONLY if it is not already added
                }

                // Update position dynamically
                control.Left = currentX;
                control.Top = centerY;

                currentX += control.Width + spacing;
            }
        }

        // Helper Method to Create Buttons
        private BeepButton CreateButton(string imagePath, Size buttonSize)
        {
            return new BeepButton()
            {
                ImagePath = imagePath,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                Size = buttonSize,
                IsChild = true,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
                Padding = new Padding(0),
                MaxImageSize = new Size(buttonSize.Width - 1, buttonSize.Height - 1),
                
            };
        }

    }
}
