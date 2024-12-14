﻿using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDisplayContainer : BeepTabs, IDisplayContainer
    {
        private Dictionary<string, TabPage> _controls = new Dictionary<string, TabPage>();

        public BeepDisplayContainer()
        {
            
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
            if (control == null || string.IsNullOrWhiteSpace(TitleText)) return false;

            if (_controls.ContainsKey(TitleText)) return false; // Avoid duplicates

            // Create and configure a new TabPage
            var tabPage = new TabPage
            {
                Text = TitleText,
                Tag = control,
                BackColor = _currentTheme.PanelBackColor
            };

            // Add control to the TabPage
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
                ContainerType = pcontainerType
            });

            return true;
        }

        public void Clear()
        {
            _controls.Clear();
            TabPages.Clear();
        }
        public bool IsControlExit(IDM_Addin control)
        {
            return _controls.Values.Any(tab => tab.Tag == control);
        }
        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            KeyPressed?.Invoke(this, keyCombination);
            return null; // Replace with appropriate error handling if needed
        }
        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            if (!_controls.ContainsKey(TitleText)) return false;

            var tabPage = _controls[TitleText];
            _controls.Remove(TitleText);
            TabPages.Remove(tabPage);

            AddinRemoved?.Invoke(this, new ContainerEvents
            {
                TitleText = TitleText,
                Control = control
            });

            return true;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            var entry = _controls.FirstOrDefault(c => ((IDM_Addin)c.Value.Tag).GuidID == guidid);
            return entry.Key != null && RemoveControl(entry.Key, (IDM_Addin)entry.Value.Tag);
        }

        public bool RemoveControlByName(string name)
        {
            return _controls.ContainsKey(name) && RemoveControl(name, (IDM_Addin)_controls[name].Tag);
        }
        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            if (!_controls.ContainsKey(TitleText)) return false;

            SelectedTab = _controls[TitleText];
            return true;
        }

    }
}