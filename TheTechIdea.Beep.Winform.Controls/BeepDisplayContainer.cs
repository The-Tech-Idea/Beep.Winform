using System.ComponentModel;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDisplayContainer))]
    [DisplayName("Beep Display Container")]
    [Description("A container control that displays multiple addins as tabs.")]
    public class BeepDisplayContainer : BeepTabs, IDisplayContainer
    {
        private readonly Dictionary<string, TabPage> _controls = new Dictionary<string, TabPage>();

        public BeepDisplayContainer()
        {
            //Uncomment if you want a default tab on initialization
             if (TabPages.Count == 0)
            {
                AddControl("Default Tab", null, ContainerTypeEnum.TabbedPanel);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TabPage SelectedTab
        {
            get => base.SelectedTab;
            set
            {
                // Trigger AddinChanging event before changing the tab
                var previousTab = base.SelectedTab;
                if (previousTab != value)
                {
                    var previousAddin = previousTab?.Tag as IDM_Addin;
                    if (previousAddin != null)
                    {
                        AddinChanging?.Invoke(this, new ContainerEvents
                        {
                            TitleText = previousTab?.Text,
                            Control = previousAddin,
                            ContainerType = ContainerTypeEnum.TabbedPanel,
                            Guidid = previousAddin.GuidID
                        });
                    }

                    base.SelectedTab = value;

                    // Trigger AddinChanged event after changing the tab
                    var currentAddin = value?.Tag as IDM_Addin;
                    if (currentAddin != null)
                    {
                        AddinChanged?.Invoke(this, new ContainerEvents
                        {
                            TitleText = value?.Text,
                            Control = currentAddin,
                            ContainerType = ContainerTypeEnum.TabbedPanel,
                            Guidid = currentAddin.GuidID
                        });
                    }
                }

                PerformLayout();
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

        public bool AddControl(string TitleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
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
            catch (Exception ex)
            {
                // The addin should raise OnError internally; we can't invoke it directly
                return false;
            }

            // Create and configure a new TabPage
            var tabPage = new TabPage
            {
                Text = TitleText,
                Tag = control
            };

            // Add control to the TabPage if it’s a Windows Forms control
            if (control is Control winControl)
            {
                winControl.Dock = DockStyle.Fill;
                tabPage.Controls.Add(winControl);
            }

            // Add TabPage to dictionary and UI container
            _controls[TitleText] = tabPage;
            TabPages.Add(tabPage);

            // Trigger AddinAdded event
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
            foreach (var tabPage in _controls.Values.ToList())
            {
                var control = tabPage.Tag as IDM_Addin;
                RemoveControl(tabPage.Text, control);
            }
            _controls.Clear();
            TabPages.Clear();
        }

        public bool IsControlExit(IDM_Addin control)
        {
            return control != null && _controls.Values.Any(tab => tab.Tag == control);
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

            var tabPage = _controls[TitleText];
            if (tabPage.Tag != control)
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
                    // The addin should raise OnError internally
                }
            }

            _controls.Remove(TitleText);
            TabPages.Remove(tabPage);

            // Trigger AddinRemoved event
            AddinRemoved?.Invoke(this, new ContainerEvents
            {
                TitleText = TitleText,
                Control = control,
                ContainerType = ContainerTypeEnum.TabbedPanel,
                Guidid = control?.GuidID
            });

            return true;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            if (string.IsNullOrEmpty(guidid))
                return false;

            var entry = _controls.FirstOrDefault(c => c.Value.Tag is IDM_Addin addin && addin.GuidID == guidid);
            if (entry.Key == null)
                return false;

            return RemoveControl(entry.Key, entry.Value.Tag as IDM_Addin);
        }

        public bool RemoveControlByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _controls.ContainsKey(name) && RemoveControl(name, _controls[name].Tag as IDM_Addin);
        }

        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            if (!_controls.ContainsKey(TitleText))
                return false;

            var tabPage = _controls[TitleText];
            if (tabPage.Tag != control)
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
                    // The addin should raise OnError internally
                }
            }

            SelectedTab = tabPage;
            return true;
        }
    }

  
}