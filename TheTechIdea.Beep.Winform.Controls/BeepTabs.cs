using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Vis;
using System.Collections;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using System.Windows.Forms;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTabs : BeepControl, IDisplayContainer
    {
        public FlowLayoutPanel _headerPanel { get; set; }
        public BeepPanel HeaderPanel { get; set; }
        private  BindingList<BeepPanel> _tabPages =new BindingList<BeepPanel>();
        private ContainerTypeEnum _containerType= ContainerTypeEnum.TabbedPanel;
        public int index { get; set; } = 0;
        protected int _buttonheight = 20;
        protected int _buttonewidth = 60;
        protected int _selectedIndex;
        protected BeepPanel _selectedPanel;
        protected bool _isTabbedPanel;
        public int Topy=30;
        public BeepTabs()
        {
            this.ShowAllBorders =true  ;
            this.ShowShadow =false ;
          
            _menuitems.ListChanged += _tabPages_ListChanged;
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 200;
            }
            ApplyThemeToChilds = false;
        }

        private void _tabPages_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Console.WriteLine("Update Tab 1");
            UpdateTabs();
            Console.WriteLine("Update Tab 2");
        }

        private SimpleItemCollection _menuitems = new SimpleItemCollection();

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection TabPages
        {
            get => _menuitems;
            set
            {
                _menuitems = value;
                UpdateTabs();
            }
        }

       

        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between Single and Tabbed mode.")]
        public ContainerTypeEnum ContainerType
        {
            get => _containerType;
            set
            {
                _containerType = value;
       //         UpdateContainerType();
            }
        }

        public IVisManager VisManager { get; set; }
        public IDMEEditor Editor { get; set; }

        // Event declarations
        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        private void UpdateTabs()
        {
            if (_tabPages == null)
            {
                _tabPages = new BindingList<BeepPanel>();

            }
            this.Controls.Clear();
            Console.WriteLine("1");
            HeaderPanel = new BeepPanel()
            {
                ShowShadow = false,
                ShowAllBorders = this.ShowAllBorders,


            };
            Controls.Add(HeaderPanel);
            Console.WriteLine("2");
            HeaderPanel.Theme = this.Theme;
            Console.WriteLine("3");
            HeaderPanel.Height = Topy;
            HeaderPanel.Location = new Point(DrawingRect.X,DrawingRect.Y);
            HeaderPanel.Width = DrawingRect.Width;
            HeaderPanel.BackColor =_currentTheme.BackgroundColor;
            Console.WriteLine("4");
            _headerPanel = CreateFlowLayoutPanel();
            HeaderPanel.Controls.Add(_headerPanel);
       
            string lastGuid = "";
            Console.WriteLine("5");
            if (_menuitems!=null)
            {
                
             //   MessageBox.Show("Header Panel Check 4");
                foreach (var item in _menuitems)
                {
                    //   MessageBox.Show("Header Panel Check 4.5");
                    Console.WriteLine("6");
                    BeepPanel panel = CreateContainerPanel(string.IsNullOrEmpty(item.Text)? "Tab ": item.Text);
                    _tabPages.Add(panel);
                    Controls.Add(panel);
                    lastGuid = panel.GuidID.ToString();
                    item.ReferenceID = lastGuid;
                    _headerPanel.Controls.Add(CreateButton(item,lastGuid));
                    AddinAdded?.Invoke(this, new ContainerEvents { TitleText = item.Text, Control = null, Guidid = lastGuid });
                }
                SelectTab(lastGuid);
            }
            
            ApplyTheme();
        }
        protected BeepPanel CreateContainerPanel(string text)
        {
            BeepPanel newPanel = new BeepPanel
            {
                TitleText = text,
                Visible = false,
                ShowTitle = false,
                ShowTitleLine = false,
                ShowAllBorders = true,
                ShowShadow = false,
                Size = new System.Drawing.Size(DrawingRect.Width, DrawingRect.Height - _headerPanel.Height - 1),
                Location = new System.Drawing.Point(DrawingRect.X, DrawingRect.Top + _headerPanel.Height + 1)
            };
            Console.WriteLine("7");
            newPanel.Theme = Theme;
            newPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            return newPanel;
        }
        protected FlowLayoutPanel CreateFlowLayoutPanel()
        { 
            var x=new FlowLayoutPanel() { };
            x.AutoSize = true;
            x.FlowDirection = FlowDirection.LeftToRight;
            x.Dock = DockStyle.Fill;
            x.BackColor = _currentTheme.BackgroundColor;
            x.ForeColor = _currentTheme.LatestForColor;
            return x;
        }
        protected BeepButton CreateButton(SimpleItem item,string panelguid)
        {
            Console.WriteLine("1");
            var tabButton = new BeepButton
            {
                Text = item.Text,

                Margin = new Padding(1, 0, 1, 0), // Add padding for spacing
                Tag = item,
                ShowAllBorders = true,
                ShowShadow = false,
                Height = _buttonheight,
                //  IsChild = true,
                OverrideFontSize = TypeStyleFontSize.Small,
                ImageAlign = ContentAlignment.MiddleRight,
                TextAlign = ContentAlignment.MiddleLeft,
                MaxImageSize = new Size(10, 10),

                SavedGuidID = panelguid,
            };
          
            Size textSize = TextRenderer.MeasureText(item.Text, tabButton.Font);
            Console.WriteLine("2");
            tabButton.Size = textSize;
            tabButton.Theme = Theme;
            Console.WriteLine("3");

            tabButton.MaxImageSize = new Size(16, 16);
               // tabButton.LogoImage = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";
            tabButton.Click += TabButton_Click;
            return tabButton;
        }
        public bool AddControl(string title, IDM_Addin control, ContainerTypeEnum containerType)
        {
            ContainerType = containerType;
            index++;
            BeepPanel newPanel = new BeepPanel
            {
                TitleText = title,
                Visible = false,
                ShowTitle = false,
                ShowTitleLine = false,
                ShowAllBorders = true,
                ShowShadow = false,
                Size = new System.Drawing.Size(DrawingRect.Width, DrawingRect.Height - _headerPanel.Height - 1),
                Location = new System.Drawing.Point(DrawingRect.X, DrawingRect.Top + _headerPanel.Height + 1)
            };
            newPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            newPanel.Controls.Add((Control)control);
            newPanel.Tag = Guid.NewGuid().ToString();
            newPanel.Theme = Theme;
            _tabPages.Add(newPanel);

            Controls.Add(newPanel);
            UpdateTabHeaders();
            newPanel.BringToFront();
            AddinAdded?.Invoke(this, new ContainerEvents { TitleText = title, Control = control, Guidid = newPanel.Tag.ToString() });
            return true;
        }

        public bool RemoveControl(string title, IDM_Addin control)
        {
            var panelToRemove = _tabPages.ToList().Find(p => p.Controls.Contains((Control)control));
            if (panelToRemove != null)
            {
                Controls.Remove(panelToRemove);
                _tabPages.Remove(panelToRemove);
                UpdateTabHeaders();
                AddinRemoved?.Invoke(this, new ContainerEvents { TitleText = title, Control = control, Guidid = panelToRemove.Tag.ToString() });
                return true;
            }
            return false;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            var panelToRemove = _tabPages.ToList().Find(p => p.Tag?.ToString() == guidid);
            if (panelToRemove != null)
            {
                Controls.Remove(panelToRemove);
                _tabPages.Remove(panelToRemove);
                UpdateTabHeaders();
                AddinRemoved?.Invoke(this, new ContainerEvents { Guidid = guidid });
                return true;
            }
            return false;
        }

        public bool RemoveControlByName(string name)
        {
            var panelToRemove = _tabPages.ToList().Find(p => p.Text == name);
            if (panelToRemove != null)
            {
                Controls.Remove(panelToRemove);
                _tabPages.Remove(panelToRemove);
                UpdateTabHeaders();
                AddinRemoved?.Invoke(this, new ContainerEvents { TitleText = name });
                return true;
            }
            return false;
        }

        public bool ShowControl(string title, IDM_Addin control)
        {
            var panelToShow = _tabPages.ToList().Find(p => p.Controls.Contains((Control)control));
            if (panelToShow != null)
            {
                SelectTab(panelToShow);
                AddinAdded?.Invoke(this, new ContainerEvents { TitleText = title, Control = control, Guidid = panelToShow.Tag.ToString() });
                return true;
            }
            return false;
        }

        public bool IsControlExit(IDM_Addin control)
        {
            foreach (BeepPanel panel in _tabPages)
            {
                if (panel.Controls.Contains((Control)control))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            _tabPages.Clear();
            Controls.Clear();
            Controls.Add(_headerPanel);
            AddinRemoved?.Invoke(this, null);
        }

        private void UpdateTabHeaders()
        {
            _headerPanel.Controls.Clear();

            _headerPanel.FlowDirection = FlowDirection.LeftToRight; // Set left alignment
            _headerPanel.WrapContents = true; // Ensure buttons are in one line
            string lastGuid = "";
            if (_menuitems != null)
            {
                foreach (var item in _menuitems)
                {  
                   lastGuid = item.ReferenceID;
                   _headerPanel.Controls.Add(CreateButton(item, lastGuid));
                    AddinAdded?.Invoke(this, new ContainerEvents { TitleText = item.Text, Control = null, Guidid = lastGuid });
                }
            }
        }

        private void CloseClicked(object? sender, BeepEventDataArgs e)
        {
            if (sender is BeepButton button && button.SavedGuidID is string panelguidid)
            {
                // should remove tab

            }
        }

        private void TabButton_Click(object sender, EventArgs e)
        {
          //  MessageBox.Show("Tab Button Clicked");
            if (sender is BeepButton button && button.SavedGuidID is string panelguidid)
                SelectTab(panelguidid);
        }
        private void SelectTab(string selectedPanelguidid)
        {
            //MessageBox.Show(selectedPanelguidid);
            foreach (BeepPanel panel in _tabPages)
            {
                panel.Visible = panel.GuidID == selectedPanelguidid;
                if(panel.Visible)
                {
                    panel.BringToFront();
                }
            }
                
        }
        private void SelectTab(BeepPanel selectedPanel)
        {
            foreach (BeepPanel panel in _tabPages)
                panel.Visible = panel == selectedPanel;
        }

        private void UpdateContainerType()
        {
            if (_containerType == ContainerTypeEnum.TabbedPanel)
                _headerPanel.Visible = true;
            else
                _headerPanel.Visible = false;
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            KeyPressed?.Invoke(this, keyCombination);
            return null;
        }
        public void Add(BeepPanel panel)
        {

           
            panel.Size = new Size(Width - (BorderRadius*2) - 10, Height - _headerPanel.Height - (BorderRadius*2) - 10);
            panel.Location = new Point(BorderRadius + 10,  _headerPanel.Height + BorderRadius + 10);
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(panel);
            panel.StaticNotMoving = true;
            UpdateTabHeaders();
            Invalidate();

        }
        public override void ApplyTheme()
        {
            if (_headerPanel == null)
            {
                return;
            }
            if (_headerPanel == null)
            {
                return;
            }
            //base.ApplyTheme();
            _headerPanel.BackColor = _currentTheme.BackgroundColor;
            _headerPanel.ForeColor = _currentTheme.ButtonForeColor;
            HeaderPanel.Theme=Theme;
            //_headerPanel.Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            try
            {
                for (int i = 0; i < _headerPanel.Controls.Count; i++)
            {
                if (_headerPanel.Controls[i] is BeepButton)
                {
                   
                        BeepButton btn = (BeepButton)_headerPanel.Controls[i];
                        btn.Theme = Theme;
                     
                        btn.Size=btn.GetSuitableSize();
                    }
                   
                   
                }
            }
            catch (Exception ex)
            {
             //   MessageBox.Show($"Error in applying theme to button {ex.Message}");
            }
            foreach (BeepPanel panel in _tabPages)
            {
                panel.Theme = Theme;
               
            }
        }
    }

   
}
