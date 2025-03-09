using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Display Container")]
    [Description("A container control for displaying addins.")]
    public partial class BeepDisplayContainer : BeepControl, IDisplayContainer
    {
        public TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum _containerType = TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel;
        private Panel ContainerPanel;
        private BeepTabs TabContainerPanel;
        private Dictionary<string, (IDM_Addin Addin, TabPage TabPage)> _controls = new Dictionary<string, (IDM_Addin, TabPage)>();
        private IDM_Addin _singlePanelAddin; // Tracks the current addin in SinglePanel mode
        private BeepButton _testButton; // Track the test button

        [Category("Behavior")]
        [Description("Determines whether the container uses a single panel or a tabbed panel.")]
        public ContainerTypeEnum ContainerType
        {
            get => _containerType;
            set
            {
                if (_containerType != value)
                {
                    _containerType = value;
                    UpdateContainerLayout();
                }
            }
        }

        public BeepDisplayContainer()
        {
            InitializeComponent();

            // Ensure the container can receive focus and events
            this.TabStop = true;
            this.Enabled = true;
            //this.Click += (s, e) => Debug.WriteLine("BeepDisplayContainer clicked!");

            // Set initial layout based on ContainerType
            UpdateContainerLayout();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Initialize the container controls
            ContainerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                TabStop = true
            };
            TabContainerPanel = new BeepTabs
            {
                Dock = DockStyle.Fill,
                Visible = true,
                TabStop = true
            };


            // Add controls to the user control
            this.Controls.Add(ContainerPanel);
            this.Controls.Add(TabContainerPanel);

            TabContainerPanel.TabRemoved += TabContainerPanel_TabRemoved;
            this.Padding= new Padding(2);
            this.Name = "BeepDisplayContainer";
            this.Size = new Size(400, 300);
            this.ResumeLayout(false);
        }
        private void TabContainerPanel_TabRemoved(object sender, TabRemovedEventArgs e)
        {
            Debug.WriteLine($"Tab removed: {e.TabText}");
            if (_controls.ContainsKey(e.TabText))
            {
                _controls.Remove(e.TabText);
                Debug.WriteLine($"Removed {e.TabText} from _controls, remaining count: {_controls.Count}");
            }
        }
        private void UpdateContainerLayout()
        {
            // Toggle visibility based on ContainerType
            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.SinglePanel)
            {
                TabContainerPanel.Visible = false;
                ContainerPanel.Visible = true;

                // Ensure the test button is added to the ContainerPanel
                if (!ContainerPanel.Controls.Contains(_testButton))
                {
                    ContainerPanel.Controls.Add(_testButton);
                }

                // Move all tabbed controls to single panel (show only the selected one)
                if (TabContainerPanel.SelectedTab != null)
                {
                    var selectedAddin = TabContainerPanel.SelectedTab.Tag as IDM_Addin;
                    if (selectedAddin != null)
                    {
                        ShowControl(TabContainerPanel.SelectedTab.Text, selectedAddin);
                    }
                }
                else if (_controls.Count > 0)
                {
                    var firstAddin = _controls.First().Value.Addin;
                    ShowControl(firstAddin.Details?.AddinName ?? "Default", firstAddin);
                }
            }
            else
            {
                ContainerPanel.Visible = false;
                TabContainerPanel.Visible = true;

                //// Ensure the test button is in a tab
                //if (!TabContainerPanel.TabPages.ContainsKey("TestTab"))
                //{
                //    var testTab = new TabPage
                //    {
                //        Text = "Test Tab",
                //        Name = "TestTab"
                //    };
                //    testTab.Controls.Add(_testButton);
                //    TabContainerPanel.TabPages.Add(testTab);
                //}

                // Move single panel control to tabs
                if (_singlePanelAddin != null)
                {
                    AddControl(_singlePanelAddin.Details?.AddinName ?? "Default", _singlePanelAddin, TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel);
                    ShowControl(_singlePanelAddin.Details?.AddinName ?? "Default", _singlePanelAddin);
                    _singlePanelAddin = null;
                }
            }
        }

        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        public bool AddControl(string TitleText, IDM_Addin control, TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum pcontainerType)
        {
            if (control == null || string.IsNullOrWhiteSpace(TitleText))
                return false;

            if (_controls.ContainsKey(TitleText))
                return false; // Avoid duplicates

            // Initialize the addin
            try
            {
                control.Initialize();
            }
            catch (Exception)
            {
                // Addin should handle OnError internally
                return false;
            }

            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.SinglePanel)
            {
                // Single panel mode: replace the existing control
                if (_singlePanelAddin != null)
                {
                    string existingTitle = _controls.FirstOrDefault(x => x.Value.Addin == _singlePanelAddin).Key ?? (_singlePanelAddin.Details?.AddinName ?? "Default");
                    RemoveControl(existingTitle, _singlePanelAddin);
                }

                _singlePanelAddin = control;
                if (control is Control winControl)
                {
                    ContainerPanel.Controls.Clear();
                    ContainerPanel.Controls.Add(winControl);
                    winControl.Left = 0;
                    winControl.Top = 0;
                    winControl.Width = ContainerPanel.Width;
                    winControl.Height = ContainerPanel.Height;
                    winControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    // Ensure the test button is still present
                    //ContainerPanel.Controls.Add(_testButton);
                }

                _controls[TitleText] = (control, null);
            }
            else
            {
                var tabPage = new TabPage { Text = TitleText, Tag = control };

                if (control is Control winControl)
                {
                    winControl.Dock = DockStyle.Fill;
                    winControl.Visible = true;
                    tabPage.Controls.Add(winControl);
                    Debug.WriteLine($"Added control to tab {TitleText}, Bounds: {winControl.Bounds}, Visible: {winControl.Visible}");
                }

                TabContainerPanel.TabPages.Add(tabPage);
                _controls[TitleText] = (control, tabPage);
                TabContainerPanel.SelectedTab = tabPage;
                TabContainerPanel.Invalidate();
                TabContainerPanel.PerformLayout();
                TabContainerPanel.Update(); // Force immediate update
                Debug.WriteLine($"DisplayRectangle for tab {TitleText}: {TabContainerPanel.DisplayRectangle}");
            }

            // Trigger AddinAdded event with the interface's ContainerTypeEnum
            AddinAdded?.Invoke(this, new ContainerEvents
            {
                TitleText = TitleText,
                Control = control,
                ContainerType = pcontainerType,
                Guidid = control.GuidID
            });

            return true;
        }

        public void Clear()
        {
            foreach (var entry in _controls.ToList())
            {
                RemoveControl(entry.Key, entry.Value.Addin);
            }
            _controls.Clear();
            TabContainerPanel.TabPages.Clear();
            ContainerPanel.Controls.Clear();
            // Re-add the test button to ContainerPanel
            ContainerPanel.Controls.Add(_testButton);
            _singlePanelAddin = null;
        }

        public bool IsControlExit(IDM_Addin control)
        {
            return control != null && _controls.Values.Any(x => x.Addin == control);
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            try
            {
                KeyPressed?.Invoke(this, keyCombination);
                return new ErrorsInfo { Flag = Errors.Ok, Message = "Key processed successfully" };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo { Flag = Errors.Failed, Message = $"Error processing key: {ex.Message}" };
            }
        }

        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            if (!_controls.ContainsKey(TitleText))
                return false;

            var entry = _controls[TitleText];
            if (entry.Addin != control)
                return false; // Ensure the control matches

            // Dispose of the addin
            if (control != null)
            {
                try
                {
                    control.Dispose();
                }
                catch (Exception)
                {
                    // Addin should handle OnError internally
                }
            }

            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.SinglePanel)
            {
                ContainerPanel.Controls.Clear();
                ContainerPanel.Controls.Add(_testButton); // Re-add the test button
                _singlePanelAddin = null;
            }
            else
            {
                TabContainerPanel.TabPages.Remove(entry.TabPage);
            }

            _controls.Remove(TitleText);

            // Trigger AddinRemoved event with the correct ContainerTypeEnum
            AddinRemoved?.Invoke(this, new ContainerEvents
            {
                TitleText = TitleText,
                Control = control,
                ContainerType = TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel,
                Guidid = control?.GuidID
            });

            return true;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            if (string.IsNullOrEmpty(guidid))
                return false;

            var entry = _controls.FirstOrDefault(c => c.Value.Addin?.GuidID == guidid);
            if (entry.Key == null)
                return false;

            return RemoveControl(entry.Key, entry.Value.Addin);
        }

        public bool RemoveControlByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _controls.ContainsKey(name) && RemoveControl(name, _controls[name].Addin);
        }

        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            if (!_controls.ContainsKey(TitleText))
                return false;

            var entry = _controls[TitleText];
            if (entry.Addin != control)
                return false;

            // Trigger PreShowItem event with detailed PassedArgs
            var args = new PassedArgs
            {
                ParameterString1 = TitleText,
                Addin = control,
                EventType = "ShowControl",
                Title = TitleText,
                Timestamp = DateTime.Now,
                AddinName = control?.Details?.AddinName
            };
            PreShowItem?.Invoke(this, args);

            // Resume the addin if it was suspended
            if (control != null)
            {
                try
                {
                    control.Resume();
                }
                catch (Exception)
                {
                    // Addin should handle OnError internally
                }
            }

            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.SinglePanel)
            {
                if (_singlePanelAddin != control)
                {
                    ContainerPanel.Controls.Clear();
                    if (control is Control winControl)
                    {
                        winControl.Dock = DockStyle.Fill;
                        ContainerPanel.Controls.Add(winControl);
                    }
                    ContainerPanel.Controls.Add(_testButton); // Re-add the test button
                    _singlePanelAddin = control;
                }
            }
            else
            {
                TabContainerPanel.SelectedTab = entry.TabPage;
            }

            return true;
        }

        public override void ApplyTheme()
        {
            Debug.WriteLine("Applying theme to BeepDisplayContainer");
            Console.WriteLine("Applying theme to BeepDisplayContainer");
            base.ApplyTheme();
            Debug.WriteLine("Applying theme to BeepDisplayContainer 1");
            Console.WriteLine("Applying theme to BeepDisplayContainer 1");

            // Apply theme to single panel addin if present
            if (_singlePanelAddin != null)
            {
                try
                {
                    _singlePanelAddin.ApplyTheme();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error applying theme to _singlePanelAddin: {ex.Message}");
                }
            }

            Debug.WriteLine("Applying theme to BeepDisplayContainer 2");
            Console.WriteLine("Applying theme to BeepDisplayContainer 2");
            // Apply theme to all addins in _controls
            foreach (var entry in _controls)
            {
                if (entry.Value.Addin != null)
                {
                    try
                    {
                        entry.Value.Addin.ApplyTheme();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error applying theme to addin {entry.Key}: {ex.Message}");
                    }
                }
            }

            Debug.WriteLine("Applying theme to BeepDisplayContainer 3");
            Console.WriteLine("Applying theme to BeepDisplayContainer 3");
            // Set background color for the container
            ContainerPanel.BackColor = _currentTheme.PanelBackColor;
            BackColor = _currentTheme.PanelBackColor;
            if (TabContainerPanel != null)
            {
             //   TabContainerPanel.BackColor = _currentTheme.PanelBackColor;
                TabContainerPanel.Theme = Theme;
            }

           
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Debug.WriteLine($"MouseDown in BeepDisplayContainer at {e.Location}");
            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel && TabContainerPanel.Visible)
            {
                TabContainerPanel.Focus();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Debug.WriteLine($"MouseUp in BeepDisplayContainer at {e.Location}");
            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel && TabContainerPanel.Visible)
            {
                TabContainerPanel.Focus();
            }
        }

       
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Debug.WriteLine($"MouseClick in BeepDisplayContainer at {e.Location}");
            if (ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel && TabContainerPanel.Visible)
            {
                TabContainerPanel.Focus();
                TabContainerPanel.ReceiveMouseClick(e.Location); // Use instance method

                // Check if the clicked tab still exists before further processing
                int tabIndex = TabContainerPanel.SelectedIndex;
                if (tabIndex >= 0 && tabIndex < TabContainerPanel.TabCount)
                {
                    string tabText = TabContainerPanel.TabPages[tabIndex].Text;
                    if (_controls.ContainsKey(tabText))
                    {
                        Debug.WriteLine($"Processing click for existing tab: {tabText}");
                        // Add any additional click processing here if needed
                    }
                    else
                    {
                        Debug.WriteLine($"Click ignored: Tab {tabText} no longer exists in _controls");
                    }
                }
                else
                {
                    Debug.WriteLine($"Click ignored: No valid tab selected or tab count is 0");
                }
            }
        }
    }

    // Helper class for IErrorsInfo (placeholder if not defined elsewhere)
 
}