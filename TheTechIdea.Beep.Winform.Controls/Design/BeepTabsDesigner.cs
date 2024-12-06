using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    internal class BeepTabsDesigner : ParentControlDesigner
    {
        #region Instance Members

        private DesignerVerbCollection _verbs;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;
        private BeepTabs_old _beepTabs;

        #endregion

        #region Properties

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Add Tab", OnAddTab),
                        new DesignerVerb("Remove Tab", OnRemoveTab)
                    };

                    UpdateRemoveTabVerbState();
                }

                return _verbs;
            }
        }

        private void UpdateRemoveTabVerbState()
        {
            if (_verbs == null || _beepTabs == null) return;

            // Enable or disable the "Remove Tab" verb based on the number of tabs
            _verbs[1].Enabled = _beepTabs.TabPanels.Count > 0;
        }

        #endregion

        #region Initialization

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            _beepTabs = component as BeepTabs_old;
            _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (_changeService != null)
            {
                _changeService.ComponentChanged += OnComponentChanged;
            }

            // Enable drag-and-drop support
            EnableDragDrop(true);
        }

        #endregion

        #region Event Handlers

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (_beepTabs == null || e.Component != _beepTabs || e.Member.Name != "TabPanels")
                return;

            UpdateRemoveTabVerbState();
        }

        private void OnAddTab(object sender, EventArgs e)
        {
            if (_beepTabs == null || _designerHost == null) return;

            using (var transaction = _designerHost.CreateTransaction("Add Tab"))
            {
                try
                {
                    var panel = CreateNewTabPanel($"Tab {_beepTabs.TabPanels.Count + 1}");
                    if (panel == null) return;

                    _changeService.OnComponentChanging(_beepTabs.TabPanels, null);
                    _beepTabs.TabPanels.Add(panel);
                    _changeService.OnComponentChanged(_beepTabs.TabPanels, null, null, null);

                    UpdateRemoveTabVerbState();
                }
                finally
                {
                    transaction.Commit();
                }
            }
        }

        private void OnRemoveTab(object sender, EventArgs e)
        {
            if (_beepTabs == null || _designerHost == null || _beepTabs.TabPanels.Count == 0) return;

            using (var transaction = _designerHost.CreateTransaction("Remove Tab"))
            {
                try
                {
                    var lastPanel = _beepTabs.TabPanels.Last();

                    _changeService.OnComponentChanging(_beepTabs.TabPanels, null);
                    _beepTabs.TabPanels.Remove(lastPanel);
                    _designerHost.DestroyComponent(lastPanel);
                    _changeService.OnComponentChanged(_beepTabs.TabPanels, null, null, null);

                    UpdateRemoveTabVerbState();
                }
                finally
                {
                    transaction.Commit();
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.Effect = e.Data.GetDataPresent(typeof(ToolboxItem)) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effect = e.Data.GetDataPresent(typeof(ToolboxItem)) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (_beepTabs == null)
                return;

            var selectedTab = _beepTabs.SelectedTab;
            if (selectedTab == null)
            {
                MessageBox.Show("No tab selected to drop the control.");
                return;
            }

            if (e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                var toolboxItem = (ToolboxItem)e.Data.GetData(typeof(ToolboxItem));
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));

                if (host != null)
                {
                    using (var transaction = host.CreateTransaction("Add Control to Tab"))
                    {
                        try
                        {
                            // Create the control
                            var control = (Control)toolboxItem.CreateComponents(host)[0];

                            // Add the control to the selected tab
                            AddControlToPanel(control, selectedTab, selectedTab.PointToClient(new Point(e.X, e.Y)));
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private BeepPanel CreateNewTabPanel(string title)
        {
            if (_designerHost == null) return null;

            var panel = (BeepPanel)_designerHost.CreateComponent(typeof(BeepPanel));
            panel.TitleText = title;
            panel.Dock = DockStyle.Fill;
            panel.Visible = false;
            panel.ShowTitle = false;
            panel.ShowTitleLine = false;
            panel.ShowAllBorders = true;
            panel.ShowShadow = false;
            panel.AllowDrop = true;

            return panel;
        }

        private void AddControlToPanel(Control control, BeepPanel panel, Point dropPoint)
        {
            if (panel == null || control == null) return;

            try
            {
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));
                var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                if (host != null && changeService != null)
                {
                    using (var transaction = host.CreateTransaction("Add Control to BeepPanel"))
                    {
                        try
                        {
                            // Notify Designer of changes
                            changeService.OnComponentChanging(panel.Controls, null);

                            // Create the control using DesignerHost
                            var createdControl = (Control)host.CreateComponent(control.GetType());
                            if (createdControl != null)
                            {
                                // Set control properties
                                createdControl.Location = panel.PointToClient(dropPoint);
                                createdControl.Name = GetUniqueControlName(panel, createdControl.GetType());

                                // Add the control to the BeepPanel
                                panel.Controls.Add(createdControl);

                                // Notify Designer of changes
                                changeService.OnComponentChanged(panel.Controls, null, null, null);

                                Console.WriteLine($"Control added to {panel.TitleText} at {createdControl.Location}.");
                            }
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding control: {ex.Message}");
            }
        }

        private string GetUniqueControlName(Control parent, Type controlType)
        {
            string baseName = controlType.Name.ToLower();
            int index = 1;

            while (parent.Controls.Find($"{baseName}{index}", true).Length > 0)
            {
                index++;
            }

            return $"{baseName}{index}";
        }

        #endregion
    }
}
