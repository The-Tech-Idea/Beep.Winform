using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Winform.Controls.Containers
{
    public partial class uc_Container : BeepControl, IDisplayContainer
    {
        private static readonly Image CloseImage = global::TheTechIdea.Beep.Winform.Controls.Properties.Resources.close;
        public ContainerTypeEnum ContainerType { get; set; }
        private Panel ContainerPanel = new Panel();
        private TabControl TabContainerPanel = new TabControl();

        public Vis.Modules.IAppManager VisManager { get; set; }
        public IDMEEditor Editor { get; set; }

        public event EventHandler<ContainerEvents?> AddinAdded;
        public event EventHandler<ContainerEvents?> AddinRemoved;
        public event EventHandler<ContainerEvents?> AddinMoved;
        public event EventHandler<ContainerEvents?> AddinChanging;
        public event EventHandler<ContainerEvents?> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        /// <summary>
        /// Initializes a new instance of the <see cref="uc_Container"/> class.
        /// </summary>
        public uc_Container()
        {
            InitializeComponent();
            TabContainerPanel.TabIndexChanged += TabContainerPanel_TabIndexChanged;
        }

        private void TabContainerPanel_TabIndexChanged(object? sender, EventArgs e)
        {
            if (TabContainerPanel.SelectedIndex >= 0)
            {
                TabPage p = TabContainerPanel.TabPages[TabContainerPanel.SelectedIndex];
                if (p?.Controls.Count > 0 && p.Controls[0] is IDM_Addin addin && VisManager != null)
                {
                    VisManager.CurrentDisplayedAddin = addin;
                }
            }
        }

        /// <summary>
        /// Checks if a control exists within the container.
        /// </summary>
        public bool IsControlExit(IDM_Addin control) =>
            TabContainerPanel?.TabPages.Cast<TabPage>()
                .SelectMany(tab => tab.Controls.Cast<Control>())
                .Any(c => c == control) ??
            ContainerPanel?.Controls.Cast<Control>().Any(c => c == control) ?? false;

        /// <summary>
        /// Adds a control to the container based on the specified type.
        /// </summary>
        public bool AddControl(string TitleText, IDM_Addin pcontrol, ContainerTypeEnum pcontainerType)
        {
            if (this.InvokeRequired)
            {
                return (bool)this.Invoke(new Func<string, IDM_Addin, ContainerTypeEnum, bool>(AddControl), TitleText, pcontrol, pcontainerType);
            }

            Control control = (Control)pcontrol;
            if (control == null) return false;

            ContainerType = pcontainerType;
            bool success = false;
            switch (pcontainerType)
            {
                case ContainerTypeEnum.SinglePanel:
                    success = AddToSingle(TitleText, control);
                    break;
                case ContainerTypeEnum.TabbedPanel:
                    success = AddToTabbed(TitleText, control);
                    break;
            }

            if (success)
            {
                AddinAdded?.Invoke(this, new ContainerEvents { Control = pcontrol, TitleText = TitleText });
            }
            return success;
        }

        private bool AddToTabbed(string TitleText, Control control)
        {
            try
            {
                EnsureTabContainerSetup();
                TabPage newTab = new TabPage(TitleText);
                Panel panel = CreateAndSetupPanel(newTab);

                TabContainerPanel.TabPages.Add(newTab);
                TabContainerPanel.SelectedTab = newTab;

                AddControlToPanel(panel, control);
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to add control to tabbed panel: {ex.Message}");
                return false;
            }
        }

        private void EnsureTabContainerSetup()
        {
            if (ContainerType == ContainerTypeEnum.SinglePanel && Controls.Contains(ContainerPanel))
            {
                Controls.Remove(ContainerPanel);
            }

            if (!Controls.Contains(TabContainerPanel))
            {
                SetupTabContainer();
            }
        }

        private void SetupTabContainer()
        {
            TabContainerPanel = new TabControl
            {
                AllowDrop = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new System.Drawing.Point(0, 0),
                Name = "ControlPanel",
                Size = new System.Drawing.Size(Width, Height),
                Multiline = true,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                Padding = new Point(30, 4)
            };
            TabContainerPanel.DrawItem += TabContainerPanel_DrawItem;
            TabContainerPanel.MouseClick += TabContainerPanel_MouseClick;
            Controls.Add(TabContainerPanel);
        }

        private Panel CreateAndSetupPanel(TabPage tab)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            panel.Resize += Panel1_Resize;
            tab.Controls.Add(panel);
            return panel;
        }

        private void AddControlToPanel(Panel panel, Control control)
        {
            panel.Controls.Add(control);
            AdjustControlSize(control, panel);
        }

        private void AdjustControlSize(Control control, Panel panel)
        {
            control.Location = new System.Drawing.Point(0, 0);
            if (control.Size.Height < panel.Size.Height && control.Size.Width < panel.Size.Width)
            {
                control.Dock = DockStyle.Fill;
            }
            else if (control.Size.Height < panel.Size.Height)
            {
                control.Height = panel.Height;
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            else if (control.Size.Width < panel.Size.Width)
            {
                control.Width = panel.Width;
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }
        }

        private void Panel1_Resize(object? sender, EventArgs e)
        {
            if (sender is Panel panel && panel.Controls.Count > 0)
            {
                AdjustControlSize(panel.Controls[0], panel);
            }
        }

        private bool AddToSingle(string TitleText, Control control)
        {
            try
            {
                if (ContainerType == ContainerTypeEnum.TabbedPanel && Controls.Contains(TabContainerPanel))
                {
                    Controls.Remove(TabContainerPanel);
                }

                EnsureSinglePanelSetup();
                ContainerPanel.Controls.Add(control);
                control.Dock = DockStyle.Fill;
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to add control to single panel: {ex.Message}");
                return false;
            }
        }

        private void EnsureSinglePanelSetup()
        {
            if (!Controls.Contains(ContainerPanel))
            {
                ContainerPanel = new Panel
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Location = new System.Drawing.Point(0, 0),
                    Name = "ControlPanel",
                    Size = new System.Drawing.Size(Width, Height)
                };
                Controls.Add(ContainerPanel);
            }
        }

        private void TabContainerPanel_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < TabContainerPanel.TabPages.Count; i++)
            {
                var tabRect = TabContainerPanel.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var imageRect = new Rectangle(tabRect.Right - CloseImage.Width,
                    tabRect.Top + (tabRect.Height - CloseImage.Height) / 2,
                    CloseImage.Width, CloseImage.Height);

                if (imageRect.Contains(e.Location))
                {
                    try
                    {
                        TabPage tab = TabContainerPanel.TabPages[i];
                        if (tab.Controls.Count > 0 && tab.Controls[0] is IDM_Addin addin)
                        {
                            AddinRemoved?.Invoke(this, new ContainerEvents { Control = addin, TitleText = tab.Text });
                            tab.Controls.Clear();
                            TabContainerPanel.TabPages.RemoveAt(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error while removing tab: {ex.Message}");
                    }
                    break;
                }
            }
        }

        private void TabContainerPanel_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    var tabRect = TabContainerPanel.GetTabRect(e.Index);
                    var text = TabContainerPanel.TabPages[e.Index].Text;
                    var textSize = e.Graphics.MeasureString(text, Font);
                    var textPoint = new PointF(tabRect.X + 12, tabRect.Y + (tabRect.Height - textSize.Height) / 2);
                    var closePoint = new Point(tabRect.Right - CloseImage.Width - 2, tabRect.Top + (tabRect.Height - CloseImage.Height) / 2);

                    e.Graphics.DrawString(text, Font, brush, textPoint);
                    e.Graphics.DrawImage(CloseImage, closePoint);

                    if (TabContainerPanel.SelectedIndex == e.Index)
                    {
                        e.Graphics.DrawRectangle(Pens.Blue, e.Bounds);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error drawing tab item: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a control from the container by title and control reference.
        /// </summary>
        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            try
            {
                if (IsControlExit(control))
                {
                    if (ContainerType == ContainerTypeEnum.TabbedPanel)
                    {
                        foreach (TabPage tab in TabContainerPanel.TabPages)
                        {
                            if (tab.Text == TitleText && tab.Controls.Contains((Control)control))
                            {
                                tab.Controls.Remove((Control)control);
                                TabContainerPanel.TabPages.Remove(tab);
                                break;
                            }
                        }
                    }
                    else
                    {
                        ContainerPanel.Controls.Remove((Control)control);
                    }
                    AddinRemoved?.Invoke(this, new ContainerEvents { Control = control, TitleText = TitleText });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError($"Error removing control: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Shows a control within the container.
        /// </summary>
        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            if (AddControl(TitleText, control, ContainerType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all controls from the container.
        /// </summary>
        public void Clear()
        {
            try
            {
                if (TabContainerPanel != null)
                {
                    TabContainerPanel.SuspendLayout();
                    foreach (TabPage tab in TabContainerPanel.TabPages.Cast<TabPage>().ToList())
                    {
                        foreach (Control ctl in tab.Controls)
                        {
                            ctl.Dispose();
                        }
                        TabContainerPanel.TabPages.Remove(tab);
                    }
                    TabContainerPanel.ResumeLayout();
                }

                if (ContainerPanel != null)
                {
                    ContainerPanel.SuspendLayout();
                    foreach (Control ctl in ContainerPanel.Controls.Cast<Control>().ToList())
                    {
                        ctl.Dispose();
                    }
                    ContainerPanel.Controls.Clear();
                    ContainerPanel.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                LogError($"Error clearing container: {ex.Message}");
            }
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            KeyPressed?.Invoke(this, keyCombination);
            return null; // Implement if there's specific logic required
        }

        /// <summary>
        /// Removes a control by its GUID tag.
        /// </summary>
        public bool RemoveControlByGuidTag(string guidid)
        {
            try
            {
                var tabsToRemove = TabContainerPanel?.TabPages.Cast<TabPage>()
                    .Where(tab => tab.Controls.Cast<Control>().Any(c => c.Tag?.ToString() == guidid)).ToList();
                if (tabsToRemove != null)
                {
                    foreach (var tab in tabsToRemove)
                    {
                        if (tab.Controls[0] is IDM_Addin addin)
                        {
                            AddinRemoved?.Invoke(this, new ContainerEvents { Control = addin, TitleText = tab.Text });
                        }
                        TabContainerPanel.TabPages.Remove(tab);
                    }
                    return tabsToRemove.Any();
                }

                var controlToRemove = ContainerPanel?.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Tag?.ToString() == guidid);
                if (controlToRemove != null)
                {
                    if (controlToRemove is IDM_Addin addin)
                    {
                        AddinRemoved?.Invoke(this, new ContainerEvents { Control = addin, TitleText = controlToRemove.Text });
                    }
                    ContainerPanel.Controls.Remove(controlToRemove);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError($"Error removing control by GUID: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes a control by its name.
        /// </summary>
        public bool RemoveControlByName(string name)
        {
            try
            {
                var tabsToRemove = TabContainerPanel?.TabPages.Cast<TabPage>()
                    .Where(tab => tab.Controls.Cast<Control>().Any(c => c.Name == name)).ToList();
                if (tabsToRemove != null)
                {
                    foreach (var tab in tabsToRemove)
                    {
                        if (tab.Controls[0] is IDM_Addin addin)
                        {
                            AddinRemoved?.Invoke(this, new ContainerEvents { Control = addin, TitleText = tab.Text });
                        }
                        TabContainerPanel.TabPages.Remove(tab);
                    }
                    return tabsToRemove.Any();
                }

                var controlToRemove = ContainerPanel?.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == name);
                if (controlToRemove != null)
                {
                    if (controlToRemove is IDM_Addin addin)
                    {
                        AddinRemoved?.Invoke(this, new ContainerEvents { Control = addin, TitleText = controlToRemove.Text });
                    }
                    ContainerPanel.Controls.Remove(controlToRemove);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError($"Error removing control by name: {ex.Message}");
                return false;
            }
        }

        private void LogError(string message)
        {
            //Debug.WriteLine(message);
            // Consider integrating with a proper logging framework like Serilog or NLog
           // Editor?.ErrorObject?.FlagError(ErrorTypes.ERROR, message); // Assuming Editor has an error logging mechanism
        }
    }
}