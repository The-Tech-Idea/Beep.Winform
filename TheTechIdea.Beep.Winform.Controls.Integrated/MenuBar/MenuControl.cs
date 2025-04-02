
//using System.Reflection;
//using TheTechIdea.Beep.Winform.Controls.Helpers;
//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.Addin;
//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Logger;
//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Winform.Controls.KeyManagement;
//using TheTechIdea.Beep.Editor;

//using TheTechIdea.Beep.ConfigUtil;

//namespace TheTechIdea.Beep.Winform.Controls.MenuBar
//{
//    //[AddinAttribute(Caption = "Beep", Name = "MenuControl", misc = "Control")]
//    public class MenuControl 
//    {
//        public MenuControl()
//        {
                
//        }
//        public MenuControl(IDMEEditor pDMEEditor, TreeViewControl ptreeControl)
//        {
//            SetMenuControl(pDMEEditor, ptreeControl);

//        }
//        public void  SetMenuControl(IDMEEditor pDMEEditor, TreeViewControl ptreeControl)
//        {
//            Editor = pDMEEditor;
//            Treecontrol = ptreeControl;
//            vismanager =Treecontrol.VisManager;
//            TreeV = Treecontrol.TreeV;
//           // GetMethods();

//        }
        
//        public TreeView TreeV { get; set; }
//        public MenuStrip MenuStrip { get; set; }
//        private TreeViewControl Treecontrol { get; set; }
      
//        public string ParentName { get ; set ; }
//        public string ObjectName { get ; set ; }
//        public string ObjectType { get ; set ; }
//        public string AddinName { get ; set ; }
//        public string Description { get ; set ; }
//        public bool DefaultCreate { get ; set ; }
//        public string DllPath { get ; set ; }
//        public string DllName { get ; set ; }
//        public string NameSpace { get ; set ; }
//        public IErrorsInfo ErrorObject { get ; set ; }
//        public IDMLogger Logger { get ; set ; }
//        public IDMEEditor Editor { get ; set ; }
//        public EntityStructure EntityStructure { get ; set ; }
      
//        public string EntityName { get ; set ; }
//        public IPassedArgs Passedarg { get ; set ; }
//        ImageList imageList;
//        public  IAppManager vismanager { get; set; }
//    //    public List<ToolStripMenuItem> menuitems { get; set; } = new List<ToolStripMenuItem>();
//        VisHelper visHelper { get { return (VisHelper)vismanager.visHelper; }  }

//        public string GuidID { get  ; set  ; }

//        public void Run(IPassedArgs pPassedarg)
//        {
          
//        }
//        private ImageList GetImageList()
//        {
//            VisHelper visHelper=(VisHelper)vismanager.visHelper;
           
           
//                    return imageList;
          
//        }
//        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
//        {
//            Editor = pbl;
//        }
//        public ToolStripMenuItem GetToolStripMenuItem(string Name)
//        {
//            ToolStripMenuItem item = new ToolStripMenuItem();
//            foreach (ToolStripMenuItem item1 in MenuStrip.Items)
//            {
//                if (item1.Text == Name)
//                {
//                    item = item1;
//                    break;
//                }
//            }
//            return item;
//        }
//        public ToolStripMenuItem GetToolStripMenuItem(AddinAttribute attrib)
//        {
//           ToolStripMenuItem item=GetToolStripMenuItem(attrib.Caption);
//            if (string.IsNullOrEmpty(item.Name))
//            {
//                item = new ToolStripMenuItem();
//                item.Name = attrib.Name;
//                item.Value = new System.Drawing.Value(20, 20);
//                item.Text = attrib.Caption;
//                item.Tag = attrib.TypeId;
//                item.ForeColor = Color.Black;
//                item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
//                item.ImageIndex = vismanager.visHelper.GetImageIndex(attrib.iconimage);
//                MenuStrip.Items.Add(item);
//            }
//            return item;
//        }
//       public bool MenuitemExist(ToolStripMenuItem Extensionsmenu,string Name)
//        {
//            bool retval = false;
//            foreach (ToolStripMenuItem item1 in Extensionsmenu.DropDownItems)
//            {
//                if (item1.Text == Name)
//                {
//                    retval = true;
//                    break;
//                }
//            }
//            return retval;
//        }
//        public IErrorsInfo CreateGlobalMenu()
//        {
//            try
//            {
//                MenuStrip.ImageScalingSize = new Value(20, 20);


//                List<AssemblyClassDefinition> extentions = Editor.ConfigEditor.GlobalFunctions.Where(o=>o.classProperties!=null && o.classProperties.ObjectType!=null && (o.classProperties.Showin== ShowinType.Menu || o.classProperties.Showin == ShowinType.Both) && o.classProperties.ObjectType.Equals(ObjectType,StringComparison.CurrentCultureIgnoreCase)).OrderBy(p => p.Order).ToList();
//                foreach (AssemblyClassDefinition cls in extentions)
//                {
//                    Type type = cls.type;
//                    AddinAttribute attrib = (AddinAttribute)type.GetCustomAttribute(typeof(AddinAttribute), true);
//                    if (attrib != null)
//                    {
//                        MenuStrip.ImageList = GetImageList();
//                        ToolStripMenuItem Extensionsmenu = GetToolStripMenuItem(attrib);

//                        //Extensionsmenu.Name = attrib.Name;
//                        //Extensionsmenu.Value = new System.Drawing.Value(20, 20);
//                        //Extensionsmenu.Text = attrib.Caption;
//                        //Extensionsmenu.Tag = attrib.TypeId;
//                        //Extensionsmenu.ForeColor = Color.Black;
//                        //Extensionsmenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
//                        //Extensionsmenu.ImageIndex = vismanager.visHelper.GetImageIndex(attrib.iconimage);
//                        // Buttons.Add(Extensionsmenu);
                     
//                        foreach (MethodsClass item in cls.Methods)
//                        {
//                            if(MenuitemExist(Extensionsmenu,item.Caption)) { continue; }
//                            ToolStripMenuItem menuItem = new ToolStripMenuItem();
//                            menuItem.Name = item.Name;
//                            menuItem.Value = new System.Drawing.Value(32, 32);
//                            menuItem.Text = item.Caption;
//                            menuItem.Tag = cls;
//                            if (item.CommandAttr.Key != BeepKeys.None)
//                            {
//                                // Convert your custom key enum (BeepKeys) to System.Windows.Forms.Keys
//                                Keys shortcutKeys = KeyManager.ConvertBeepKeysToSystemKeys(item.CommandAttr.Key);

//                                // Check for control modifier
//                                if (item.CommandAttr.Ctrl)
//                                {
//                                    shortcutKeys |= Keys.Control; // Add the Control modifier
//                                }

//                                // Check for shift modifier
//                                if (item.CommandAttr.Shift)
//                                {
//                                    shortcutKeys |= Keys.Shift; // Add the Shift modifier
//                                }

//                                // Check for alt modifier
//                                if (item.CommandAttr.Alt)
//                                {
//                                    shortcutKeys |= Keys.Alt; // Add the Alt modifier
//                                }

//                                // Assign the combined shortcut keys to the ToolStripMenuItem
//                                menuItem.ShortcutKeys = shortcutKeys;
//                            }

//                            menuItem.Click += RunToolStripFunction;
//                            menuItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;
//                            menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
//                            menuItem.Image = (Image)visHelper.GetImage(item.iconimage);
//                            Extensionsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem });
                           
                           
//                        }
                        
//                    }

//                }
//                if (ObjectType == "Beep")
//                {
//                    ToolStripMenuItem Configmenu = new ToolStripMenuItem();
//                    Configmenu.Name = "Config";
//                    // Configmenu.Value = new System.Drawing.Value(37, 20);
//                    Configmenu.Text = "Configuration";
//                    Configmenu.Tag = "Config";
//                    Configmenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
//                    Configmenu.ImageIndex = vismanager.visHelper.GetImageIndex("configuration.ico"  );
//                    foreach (AddinTreeStructure item in Editor.ConfigEditor.AddinTreeStructure)
//                    {
//                        ToolStripMenuItem menuItem = new ToolStripMenuItem();
//                        menuItem.Name = item.NodeName;
//                        //  menuItem.Value = new System.Drawing.Value(37, 20);
//                        menuItem.Text = item.NodeName;
//                        menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
//                        menuItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;
//                        menuItem.Tag = item.className;
//                        menuItem.Click += RunConfigFunction;
//                        menuItem.Image = (Image)visHelper.GetImage(item.Imagename);
//                        // menuItem.ImageIndex = vismanager.visHelper.GetImageIndex(item.Imagename);
//                        Configmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem });
//                    }
//                    MenuStrip.Items.Add(Configmenu);
//                }
             
//                ////-------------------------------------------------------------------------------------------
                
                
//                return Editor.ErrorObject;
//            }
//            catch (Exception ex)
//            {

//                return Editor.ErrorObject;
//            }
//        }
//        private void RunToolStripFunction(object sender, EventArgs e)
//        {
            
//            Treecontrol.RunFunction(sender, e);
//        }
//        private void RunConfigFunction(object sender, EventArgs e)
//        {
//          //  Treecontrol.RunFunctionFromExtensions(sender, e);
//            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
//            PassedArgs Passedarguments = new PassedArgs
//            {  // Obj= obj,
//                Addin = null,
//                AddinName = null,
//                AddinType = null,
//                DMView = null,
//                CurrentEntity = menuItem.Name,
//                ObjectName = menuItem.Name,

//                ObjectType = menuItem.Tag.ToString(),

//                EventType = "Run"

//            };
//            vismanager.ShowUserControlPopUp(menuItem.Tag.ToString(), Editor, null, Passedarguments);
//        }

//        public void Run(params object[] args)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
