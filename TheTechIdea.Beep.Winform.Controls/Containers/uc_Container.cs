using TheTechIdea.Beep.Vis.Modules;
using System.Diagnostics;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.Containers
{
    public partial class uc_Container : BeepControl, IDisplayContainer
    {
        public ContainerTypeEnum ContainerType { get; set; }
        private Panel ContainerPanel=new Panel();
        private TabControl TabContainerPanel = new TabControl();

        public IVisManager VisManager { get; set; }
        public IDMEEditor Editor { get; set; }

        public event EventHandler<ContainerEvents?> AddinAdded ;
        public event EventHandler<ContainerEvents?> AddinRemoved;
        public event EventHandler<ContainerEvents?> AddinMoved;
        public event EventHandler<ContainerEvents?> AddinChanging;
        public event EventHandler<ContainerEvents?> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        public uc_Container()
        {
            InitializeComponent();

            TabContainerPanel.TabIndexChanged += TabContainerPanel_TabIndexChanged1; ;

        }

        private void TabContainerPanel_TabIndexChanged1(object? sender, EventArgs e)
        {
            if (TabContainerPanel.SelectedIndex >= 0)
            {
                TabPage p = TabContainerPanel.TabPages[TabContainerPanel.SelectedIndex];
                if (p != null)
                {
                    if (p.Controls.Count > 0)
                    {
                        IDM_Addin addin = (IDM_Addin)p.Controls[0];
                        if (VisManager != null)
                        {
                            VisManager.CurrentDisplayedAddin = addin;
                        }
                    }
                }
            }
        }

       
        public bool IsControlExit(IDM_Addin control)
        {
            if (TabContainerPanel != null)
            {
                for (int i = TabContainerPanel.TabPages.Count - 1; i >= 0; i--)
                {
                    TabPage tabPage = TabContainerPanel.TabPages[i];
                    foreach (Control ctl in tabPage.Controls)
                    {
                        IDM_Addin d = (IDM_Addin)ctl;
                        if (ctl == control)
                        {
                            return true;
                        }
                    }
                    
                }
            }
            if (ContainerPanel != null)
            {
                for (int i = ContainerPanel.Controls.Count - 1; i >= 0; i--)
                {
                    Control ctl = ContainerPanel.Controls[i];
                    IDM_Addin d = (IDM_Addin)ctl;
                    if (ctl == control)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        private void TabContainerPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Image CloseImage = global::TheTechIdea.Beep.Winform.Controls.Properties.Resources.close;
            for (var i = 0; i < this.TabContainerPanel.TabPages.Count; i++)
            {
                var tabRect = this.TabContainerPanel.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var imageRect = new Rectangle(tabRect.Right - CloseImage.Width,
                                         tabRect.Top + (tabRect.Height - CloseImage.Height) / 2,
                                         CloseImage.Width,
                                         CloseImage.Height);
                if (imageRect.Contains(e.Location))
                {
                    try
                    {
                        TabPage t = this.TabContainerPanel.TabPages[i];
                        if (t.Controls.Count > 0)
                        {
                            Panel panel = (Panel)t.Controls[0];
                            if(panel.Controls.Count > 0)
                            {
                                IDM_Addin addin = (IDM_Addin)panel.Controls[0];
                                AddinRemoved?.Invoke(this, new ContainerEvents() { Control = addin, TitleText = t.Text });
                            }
                            t.Controls.Clear();
                        }
                        this.TabContainerPanel.TabPages.RemoveAt(i);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($" Error while Removing Tab : {ex.Message}");
                    }
                   
                    break;
                }
            }
        }
        private void TabContainerPanel_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Image CloseImage = global::TheTechIdea.Beep.Winform.Controls.Properties.Resources.close;
                Image img = new Bitmap(CloseImage);
                Rectangle r = e.Bounds;
                r = this.TabContainerPanel.GetTabRect(e.Index);
                r.Offset(2, 2);
                Brush TitleBrush = new SolidBrush(Color.Black);
                Font f = this.Font;
                string title = this.TabContainerPanel.TabPages[e.Index].Text;
                SizeF titlesize=e.Graphics.MeasureString(title, f);
                //This code will render a "x" mark at the end of the Tab caption. 
                e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
                e.Graphics.DrawString(this.TabContainerPanel.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
                e.DrawFocusRectangle();
                //e.Graphics.DrawString(title, f, TitleBrush, new PointF(r.X, r.Y));
                
                //e.Graphics.DrawImage(img, new Point(r.X + (this.TabContainerPanel.GetTabRect(e.Index).Width - _imageLocation.X), _imageLocation.Y));

            }
            catch (Exception ex) { }
        }
        public bool AddControl(string TitleText, IDM_Addin pcontrol, ContainerTypeEnum pcontainerType)
        {
            Control control = (Control)pcontrol;
            ContainerType = pcontainerType;
            if (control == null ) { return false; }
            switch (pcontainerType)
            {
                case ContainerTypeEnum.SinglePanel:
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() => AddToSingle(TitleText, control)));
                    }
                    else
                    {
                        AddToSingle(TitleText, control);
                    }
                    break;
                case ContainerTypeEnum.TabbedPanel:
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() => AddToTabbed(TitleText, control)));
                    }
                    else
                    {
                        AddToTabbed(TitleText, control);
                    }
                    break;
                default:
                    break;
            }
            return control != null && control.Controls.Contains(control);
        }
        private bool AddToTabbed(string TitleText,Control control)
        {
            if(ContainerType== ContainerTypeEnum.SinglePanel)
            {
                if (Controls.Contains(ContainerPanel))
                {
                    Controls.Remove(ContainerPanel);
                }
              
            }
            if (!Controls.Contains(TabContainerPanel))
            {
                TabContainerPanel = new TabControl();
                TabContainerPanel.AllowDrop = true;
                TabContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
           | System.Windows.Forms.AnchorStyles.Left)
           | System.Windows.Forms.AnchorStyles.Right)));
                TabContainerPanel.Location = new System.Drawing.Point(0,0);
                TabContainerPanel.Name = "ControlPanel";
                TabContainerPanel.Size = new System.Drawing.Size(this.Width, this.Height);
                TabContainerPanel.TabPages.Clear();
                Controls.Add(TabContainerPanel);
            //    CloseImage = global::TheTechIdea.Beep.Winform.Controls.Properties.Resources.close;
                this.TabContainerPanel.Multiline = true;
                TabContainerPanel.DrawMode = TabDrawMode.OwnerDrawFixed;
                TabContainerPanel.DrawItem += TabContainerPanel_DrawItem;
                TabContainerPanel.MouseClick += TabContainerPanel_MouseClick;
                TabContainerPanel.Padding = new Point(30, 4);
            }
            TabContainerPanel.TabPages.Add(TitleText, TitleText);
            Panel panel1 = new Panel();
            panel1.Dock = DockStyle.Fill;
            panel1.Left = 0;
            panel1.Top = 0;
            //panel1.Width = TabContainerPanel.Width-20;
            //panel1.Height = TabContainerPanel.Height-20;
            panel1.Resize += Panel1_Resize;
            panel1.AutoScroll = true;
            TabContainerPanel.SelectedTab = TabContainerPanel.TabPages[TabContainerPanel.TabPages.Count - 1];
            // TabContainerPanel.TabPages[TabContainerPanel.TabPages.Count-1].Controls.Add(control);
            if (TabContainerPanel.SelectedTab!=null)
            {
                TabContainerPanel.SelectedTab.Controls.Add(panel1);
            }else
            {
                TabContainerPanel.TabPages[0].Controls.Add(panel1);
            }
            try
            {
                TabContainerPanel.SelectedTab.Text = TitleText;
                ////if (this.InvokeRequired)
                ////{
                //    this.Invoke((MethodInvoker)(() => panel1.Controls.Add(control)));
                ////}
                ////else
                ////{
                    panel1.Controls.Add(control);
                //}
              
            }
            catch (Exception ex)
            {
                MessageBox.Show( $"Error in Displaying Control {control.Name}-{ex.Message}","Beep", MessageBoxButtons.OK);
                Debug.WriteLine(ex.Message);
            }
           
           
            control.Location = new System.Drawing.Point(0, 0);
         
            if((control.Size.Height< panel1.Size.Height)&&((control.Size.Width < panel1.Size.Width)))
            {
                control.Dock = DockStyle.Fill;
            }else if ((control.Size.Height < panel1.Size.Height) )
            {
                control.Height = panel1.Height ;
                control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top )
         | System.Windows.Forms.AnchorStyles.Left)
         | System.Windows.Forms.AnchorStyles.Right)));
            }
            else if ((control.Size.Width < panel1.Size.Width))
            {
                control.Width = panel1.Width ;
                control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top )
         | System.Windows.Forms.AnchorStyles.Left)
         )));
            }
            //    this.TitleLabel.Text = TitleText;
            return true;
        }
        private void Panel1_Resize(object? sender, EventArgs e)
        {
            Panel? panel1 = sender as Panel;
            if (panel1 != null)
            {
                if(panel1.Controls.Count > 0)
                {
                    Control control = panel1.Controls[0];
                    if ((control.Size.Height < panel1.Size.Height) && ((control.Size.Width < panel1.Size.Width)))
                    {
                        control.Dock = DockStyle.Fill;
                    }
                    else if ((control.Size.Height < panel1.Size.Height))
                    {
                        control.Height = panel1.Height ;
                        control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
                 | System.Windows.Forms.AnchorStyles.Left)
                 | System.Windows.Forms.AnchorStyles.Right)));
                    }
                    else if ((control.Size.Width < panel1.Size.Width))
                    {
                        control.Width = panel1.Width ;
                        control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top )
                 | System.Windows.Forms.AnchorStyles.Left)
                 )));
                    }
                }
               
            }
            
        }
        private bool AddToSingle(string TitleText, Control control)
        {
            if (ContainerType == ContainerTypeEnum.TabbedPanel)
            {
                if (Controls.Contains(TabContainerPanel))
                {
                    Controls.Remove(TabContainerPanel);
                }

            }
            if (!Controls.Contains(ContainerPanel))
            {
                ContainerPanel = new Panel();
                ContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
         | System.Windows.Forms.AnchorStyles.Left)
         | System.Windows.Forms.AnchorStyles.Right)));
                ContainerPanel.Location = new System.Drawing.Point(0, 0);
                ContainerPanel.Name = "ControlPanel";
                ContainerPanel.Dock = DockStyle.None;
                ContainerPanel.Size = new System.Drawing.Size(this.Width, this.Height );
                Controls.Add(ContainerPanel);
            }
            ContainerPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
         //   this.TitleLabel.Text = TitleText;
            return true;
        }
        public static Rectangle GetRTLCoordinates(Rectangle container, Rectangle drawRectangle)
        {
            return new Rectangle(
                container.Width - drawRectangle.Width - drawRectangle.X,
                drawRectangle.Y,
                drawRectangle.Width,
                drawRectangle.Height);
        }
        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            bool retval = true;
            AddinRemoved?.Invoke(this, new ContainerEvents() { Control = control, TitleText = TitleText });
            return retval;
        }
        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            bool retval = true;
            AddinAdded?.Invoke(this, new ContainerEvents() { Control = control, TitleText = TitleText });
            return retval;

        }
        public void Clear()
        {
            if (TabContainerPanel != null)
            {
                for (int i = TabContainerPanel.TabPages.Count - 1; i >= 0; i--)
                {
                    TabPage tabPage = TabContainerPanel.TabPages[i];
                    foreach (Control ctl in tabPage.Controls)
                    {
                        ctl.Dispose();
                    }
                    TabContainerPanel.TabPages.RemoveAt(i);
                }
            }
            if (ContainerPanel != null)
            {
                for (int i = ContainerPanel.Controls.Count - 1; i >= 0; i--)
                {
                    Control ctl = ContainerPanel.Controls[i];
                    ctl.Dispose();
                    ContainerPanel.Controls.RemoveAt(i);
                }
            }

        }
        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            return null;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            throw new NotImplementedException();
        }

        public bool RemoveControlByName(string guidid)
        {
            throw new NotImplementedException();
        }
    }
   
}
