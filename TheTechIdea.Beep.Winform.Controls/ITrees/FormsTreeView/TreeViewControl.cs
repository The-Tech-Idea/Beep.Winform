//using System.Reflection;
//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.Addin;
//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Logger;
//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.ConfigUtil;
//using static TheTechIdea.Beep.Utilities.Util;




//namespace TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView
//{
//    [Addin(Caption = "Beep", Name = "TreeControl", misc = "Control")]
//    public class TreeViewControl :  ITree
//    {
//        public event EventHandler<IPassedArgs> PreShowItem;
//        public event EventHandler<IPassedArgs> PreCallModule;
//        public event EventHandler<IBranch> RefreshBranch;
//        public event EventHandler<IBranch> RefreshChildBranchs;
//        public event EventHandler<IBranch> RefreshParentBranch;
//        public event EventHandler<IBranch> RefreshBranchIcon;
//        public event EventHandler<IBranch> RefreshBranchText;
//        public List<Tuple<IBranch, string>> GenerBranchs { get; set; } = new List<Tuple<IBranch, string>>();
//        public TreeViewControl()
//        {

//        }
//        public TreeViewControl(IDMEEditor pDMEEditor, IAppManager pVismanager)
//        {
//            DMEEditor = pDMEEditor;
//            VisManager = pVismanager;
//        }
//        #region "Addin Properties"
//        public string ParentName { get; set; }
//        public string ObjectName { get; set; }
//        public string ObjectType { get; set; }
//        public string AddinName { get; set; }
//        public string Description { get; set; }
//        public bool DefaultCreate { get; set; }
//        public string DllPath { get; set; }
//        public string DllName { get; set; }
//        public string NameSpace { get; set; }
//        public IErrorsInfo ErrorObject { get; set; }
//        public IDMLogger Logger { get; set; }
//        public IDMEEditor DMEEditor { get; set; }
//        public EntityStructure EntityStructure { get; set; }
//        public string EntityName { get; set; }
//        public IPassedArgs Passedarg { get; set; }
//        #endregion
//        public string TreeType { get; set; }

//        public IAppManager VisManager { get; set; }
//        public TreeView TreeV { get; set; }
//        public string CategoryIcon { get; set; } = "Category.png";
//        public string SelectIcon { get; set; } = "check.png";
//        public IBranch CurrentBranch { get; set; }
//        public List<ContextMenuStrip> menus { get; set; } = new List<ContextMenuStrip>(); //ContextMenuStrip
//        public PassedArgs args { get; set; }
//        static int pSeqID = 0;
//        public int SeqID
//        {
//            get
//            {
//                pSeqID += 1;
//                return pSeqID;
//            }
//        }
//        public List<IBranch> Branches { get; set; } = new List<IBranch>();
//        public int SelectedBranchID { get; set; }
//        public string TreeEvent { get; set; }
//        public string TreeOP { get; set; }
//        public TreeNode LastSelectedNode { get; set; }
//        public TreeNode SelectedNode { get; set; }
//        public int StartselectBranchID { get; set; }
//        public Color SelectBackColor { get; set; }
        
//        public List<MenuList> Menus { get; set; } = new List<MenuList>();
//        public List<int> SelectedBranchs { get; set; } = new List<int>();
//        public TreeNodeDragandDropHandler treeNodeDragandDropHandler { get; set; }
//        public ITreeBranchHandler Treebranchhandler { get; set; }
//        ErrorsInfo ErrorsandMesseges;
//        // VisHelper visHelper;
//        ImageList imageList;
//        private ImageList GetImageList()
//        {
//            // visHelper = (VisHelper)VisManager.visHelper;


//            return imageList;//visHelper.ImageList32;

//        }

//        public IErrorsInfo CreateFunctionExtensions(MethodsClass item)
//        {
//            ContextMenuStrip nodemenu = new ContextMenuStrip();
//            try
//            {
//                nodemenu.ImageList = GetImageList();
//                ToolStripItem st = nodemenu.Items.Add(item.Caption);
//                foreach (IBranch br in Branches)
//                {
//                    if (br.BranchType == item.PointType)
//                    {
//                        nodemenu.Name = br.ToString();
//                        if (item.iconimage != null)
//                        {
//                            st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
//                        }
//                        nodemenu.ItemClicked += Nodemenu_ItemClicked;
//                        nodemenu.Tag = br;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                string mes = $"Could not add method from Extension {item.Name} to menu ";
//                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
//            };
//            return DMEEditor.ErrorObject;

//        }
//        private bool IsMenuCreated(IBranch br)
//        {
//            if (br.ObjectType != null)
//            {
//                return Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
//                && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
//                && p.PointType == br.BranchType).Any();
//            }
//            return
//                false;
//        }
//        private MenuList GetMenuList(IBranch br)
//        {
//            return Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
//                && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
//                && p.PointType == br.BranchType).FirstOrDefault();
//        }
//        public T CreateInstance<T>(params object[] paramArray)
//        {
//            return (T)Activator.CreateInstance(typeof(T), args: paramArray);
//        }
//        public TreeNode CurrentNode { get; set; }
//        public TreeNode ParentNode { get; set; }
//        public IErrorsInfo CreateRootTree()
//        {
//            string packagename = "";
//            try
//            {
//                //bool HasConstructor=false;
//                SetupTreeView();
//                treeNodeDragandDropHandler = new TreeNodeDragandDropHandler(DMEEditor, this);
//                Treebranchhandler = new TreeBranchHandler(DMEEditor, this, this);
//                Branches = new List<IBranch>();
//                IBranch Genrebr = null;
//                // AssemblyClassDefinition GenreBrAssembly = DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).FirstOrDefault()!;
//                foreach (AssemblyClassDefinition GenreBrAssembly in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).OrderBy(x => x.Order))
//                {
//                    if (GenreBrAssembly != null)
//                    {

//                        Type adc = DMEEditor.assemblyHandler.GetType(GenreBrAssembly.PackageName);
//                        ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
//                        if (ctor != null)
//                        {
//                            ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
//                            try
//                            {
//                                Genrebr = createdActivator();
//                                if (Genrebr.BranchType == EnumPointType.Genre)
//                                {
//                                    if (GenreBrAssembly.PackageName != null)
//                                    {
//                                        PassedArgs x = new PassedArgs { ObjectType = GenreBrAssembly.PackageName };
//                                        x.Cancel = false;
//                                        x.AddinName = GenreBrAssembly.PackageName;
//                                        x.AddinType = GenreBrAssembly.componentType;
//                                        x.CurrentEntity = GenreBrAssembly.PackageName;
//                                        x.ObjectName = GenreBrAssembly.className;
//                                        x.ObjectType = GenreBrAssembly.classProperties.ClassType;
//                                        PreShowItem?.Invoke(this, x);
//                                        //   VisManager.Addins.Where(p => p.ObjectName == GenreBrAssembly.className).FirstOrDefault().Run();
//                                        if (x.Cancel)
//                                        {
//                                            Genrebr.Visible = false;
//                                        }
//                                    }
//                                    int id = SeqID;
//                                    Genrebr.Name = GenreBrAssembly.PackageName;
//                                    packagename = GenreBrAssembly.PackageName;
//                                    Genrebr.ID = id;
//                                    Genrebr.BranchID = id;
//                                    if (TreeType != null)
//                                    {
//                                        if (GenreBrAssembly.classProperties.ObjectType != null)
//                                        {
//                                            if (GenreBrAssembly.classProperties.ObjectType.Equals(TreeType, StringComparison.InvariantCultureIgnoreCase))
//                                            {
//                                                if (Genrebr.Visible)
//                                                {
//                                                    CreateNode(id, Genrebr, TreeV);
//                                                }

//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (Genrebr.Visible)
//                                        {
//                                            CreateNode(id, Genrebr, TreeV);
//                                        }
//                                    }

//                                    GenerBranchs.Add(new Tuple<IBranch, string>(Genrebr, GenreBrAssembly.classProperties.menu));
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {GenreBrAssembly.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
//                            }
//                        }
//                    }
//                }

//                foreach (AssemblyClassDefinition cls in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null).OrderBy(x => x.Order))
//                {
//                    Type adc = DMEEditor.assemblyHandler.GetType(cls.PackageName);
//                    ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;

//                    if (ctor != null)
//                    {
//                        ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
//                        try
//                        {
//                            IBranch br = createdActivator();
//                            if (br.BranchType == EnumPointType.Root)
//                            {
//                                int id = SeqID;
//                                br.Name = cls.PackageName;
//                                packagename = cls.PackageName;
//                                br.ID = id;
//                                br.BranchID = id;
//                                if (cls.PackageName != null)
//                                {
//                                    PassedArgs x = new PassedArgs { ObjectType = cls.PackageName };
//                                    x.Cancel = false;
//                                    x.AddinName = cls.PackageName;
//                                    x.AddinType = cls.componentType;
//                                    x.CurrentEntity = cls.PackageName;
//                                    x.ObjectName = cls.className;
//                                    x.ObjectType = cls.classProperties.ClassType;
//                                    PreShowItem?.Invoke(this, x);
//                                    //   VisManager.Addins.Where(p => p.ObjectName == GenreBrAssembly.className).FirstOrDefault().Run();
//                                    if (x.Cancel)
//                                    {
//                                        br.Visible = false;
//                                    }
//                                }
//                                if (TreeType != null)
//                                {
//                                    if (cls.classProperties.ObjectType != null)
//                                    {
//                                        if (cls.classProperties.ObjectType.Equals(TreeType, StringComparison.InvariantCultureIgnoreCase))
//                                        {
//                                            var tr = GenerBranchs.FirstOrDefault(p => p.Item2.Equals(cls.classProperties.menu, StringComparison.OrdinalIgnoreCase));
//                                            if (tr != null)
//                                            {
//                                                Genrebr = tr.Item1;
//                                            }
//                                            else
//                                                Genrebr = null;

//                                            if (Genrebr != null)
//                                            {
//                                                if (Genrebr.Visible)
//                                                {
//                                                    Treebranchhandler.AddBranch(Genrebr, br);
//                                                    br!.CreateChildNodes();
//                                                }

//                                            }
//                                            else
//                                            {
//                                                if (br.Visible)
//                                                {
//                                                    CreateNode(id, br, TreeV);
//                                                }

//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    if (cls.classProperties.Category != DatasourceCategory.NONE)
//                                    {
//                                        Treebranchhandler.AddBranch(Genrebr, br);
//                                        br!.CreateChildNodes();
//                                    }
//                                    else
//                                    {
//                                        if (br.Visible)
//                                        {
//                                            CreateNode(id, br, TreeV);
//                                        }

//                                    }
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {cls.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                DMEEditor.ErrorObject.Ex = ex;
//                DMEEditor.ErrorObject.Flag = Errors.Failed;
//                DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {packagename} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);

//            };
//            return DMEEditor.ErrorObject;
//        }
//        private void CreateNode(int id, IBranch br, TreeView tree)
//        {
//            TreeNode n = TreeV.Nodes.Add(br.BranchText);
//            n.Tag = br;
//            br.TreeEditor = this;
//            br.Visutil = VisManager;
//            br.BranchID = id;
//            br.ID = id;
//            n.Name = br.ID.ToString();
//            ParentNode = null;
//            CurrentNode = n;
//            //br.ParentBranch = n;
//            int imgidx = VisManager.visHelper.GetImageIndex(br.IconImageName);
//            if (imgidx == -1)
//            {
//                imgidx = VisManager.visHelper.GetImageIndexFromConnectioName(br.BranchText);
//            }
//            if (imgidx == -1)
//            {
//                n.ImageKey = br.IconImageName;
//                n.SelectedImageKey = br.IconImageName;

//            }
//            else
//            {
//                n.ImageIndex = imgidx;
//                n.SelectedImageIndex = imgidx;

//            }
//            //n.ContextMenuStrip = 
//            Console.WriteLine(br.BranchText);
//            CreateMenuMethods(br);
//            if (br.ObjectType != null && br.BranchClass != null)
//            {
//                CreateGlobalMenu(br);
//            }

//            br.DMEEditor = DMEEditor;
//            if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
//            {
//                DMEEditor.ConfigEditor.objectTypes.Add(new Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
//            }
//            try
//            {
//                br.SetConfig(this, DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
//            }
//            catch (Exception ex)
//            {

//            }

//            Branches.Add(br);
//            br.CreateChildNodes();
//        }
//        public IErrorsInfo CreateGlobalMenu(IBranch br)
//        {
//            try
//            {
//                MenuList menuList = new MenuList();
//                if (!IsMenuCreated(br))
//                {
//                    menuList = new MenuList(br.ObjectType, br.BranchClass, br.BranchType);
//                    menuList.branchname = br.BranchText;
//                    Menus.Add(menuList);
//                    ContextMenuStrip nodemenu = new ContextMenuStrip();
//                    nodemenu.ImageList = GetImageList();
//                    //nodemenu.ItemClicked -= Nodemenu_ItemClicked;
//                    //nodemenu.ItemClicked += Nodemenu_ItemClicked;
//                    nodemenu.Items.Clear();
//                    menuList.Menu = nodemenu;
//                    menuList.ObjectType = br.ObjectType;
//                    menuList.BranchClass = br.BranchClass;
//                }
//                else
//                    menuList = GetMenuList(br);
//                List<AssemblyClassDefinition> extentions = DMEEditor.ConfigEditor.GlobalFunctions.Where(o => o.classProperties != null && o.classProperties.ObjectType != null && o.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Order).ToList(); //&&  o.classProperties.menu.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
//                foreach (AssemblyClassDefinition cls in extentions)
//                {
//                    if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.CurrentCultureIgnoreCase)))
//                    {
//                        menuList.classDefinitions.Add(cls);
//                        foreach (var item in cls.Methods)
//                        {
//                            if (string.IsNullOrEmpty(item.ClassType))
//                            {
//                                if (item.PointType == br.BranchType)
//                                {
//                                    ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
//                                    ToolStripItem st = ls.Items.Add(item.Caption);
//                                    ls.Name = br.ToString();
//                                    if (item.iconimage != null)
//                                    {
//                                        st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
//                                    }
//                                    st.Tag = cls;
//                                    ls.Tag = cls;
//                                    ls.ItemClicked -= Nodemenu_ItemClicked;
//                                    ls.ItemClicked += Nodemenu_ItemClicked;
//                                }
//                            }
//                            else
//                            {
//                                if (item.PointType == br.BranchType && br.BranchClass.Equals(item.ClassType, StringComparison.InvariantCultureIgnoreCase))
//                                {
//                                    ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
//                                    ToolStripItem st = ls.Items.Add(item.Caption);
//                                    ls.Name = br.ToString();
//                                    if (item.iconimage != null)
//                                    {
//                                        st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
//                                    }
//                                    st.Tag = cls;
//                                    ls.Tag = cls;
//                                    ls.ItemClicked -= Nodemenu_ItemClicked;
//                                    ls.ItemClicked += Nodemenu_ItemClicked;
//                                }
//                            }
//                        }
//                    }
//                }
//                return DMEEditor.ErrorObject;
//            }
//            catch (Exception ex)
//            {
//                return DMEEditor.ErrorObject;
//            }
//        }
//        public ContextMenuStrip CreateMenuMethods(IBranch branch)
//        {
//            ContextMenuStrip ls = null;
//            MenuList menuList = new MenuList();
//            if (!IsMenuCreated(branch))
//            {
//                menuList = new MenuList(branch.ObjectType, branch.BranchClass, branch.BranchType);
//                menuList.branchname = branch.BranchText;
//                Menus.Add(menuList);
//                ContextMenuStrip nodemenu = new ContextMenuStrip();
//                nodemenu.Items.Clear();
//                nodemenu.ImageList = GetImageList();
//                //nodemenu.ItemClicked -= Nodemenu_ItemClicked;
//                //nodemenu.ItemClicked += Nodemenu_ItemClicked;
//                menuList.Menu = nodemenu;
//                menuList.ObjectType = branch.ObjectType;
//                menuList.BranchClass = branch.BranchClass;
//            }
//            else
//                menuList = GetMenuList(branch);
//            try
//            {
//                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == branch.ToString()).FirstOrDefault();
//                if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.InvariantCultureIgnoreCase)))
//                {
//                    menuList.classDefinitions.Add(cls);
//                    foreach (var item in cls.Methods.Where(y => y.Hidden == false))
//                    {
//                        ls = (ContextMenuStrip)menuList.Menu;
//                        ToolStripItem st = ls.Items.Add(item.Caption);
//                        ls.Name = branch.ToString();
//                        ls.ItemClicked -= Nodemenu_ItemClicked;
//                        ls.ItemClicked += Nodemenu_ItemClicked;
//                        if (item.iconimage != null)
//                        {
//                            try
//                            {
//                                st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
//                            }
//                            catch (Exception)
//                            {
//                                string mes = $"Could not get image {item.iconimage} for menthod on branch {branch.BranchText}";
//                                DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
//                            }

//                        }
//                        st.Tag = cls;
//                        ls.Tag = cls;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                string mes = "Could not add method to menu " + branch.BranchText;
//                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
//            };
//            return ls;
//        }
//        public void RunFunction(object sender, EventArgs e)
//        {
//            IBranch br = null;
//            AssemblyClassDefinition assemblydef = new AssemblyClassDefinition();
//            MethodInfo method = null;
//            MethodsClass methodsClass;
//            string MethodName = "";
//            if (sender == null) { return; }
//            if (sender.GetType() == typeof(ToolStripButton))
//            {
//                ToolStripButton item = (ToolStripButton)sender;
//                assemblydef = (AssemblyClassDefinition)item.Tag;
//                MethodName = item.Text;
//            }
//            if (sender.GetType() == typeof(ToolStripMenuItem))
//            {
//                ToolStripMenuItem item = (ToolStripMenuItem)sender;
//                assemblydef = (AssemblyClassDefinition)item.Tag;
//                MethodName = item.Text;
//            }
//            dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { DMEEditor, VisManager, this });
//            //  dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.type.ToString(), new object[] { DMEEditor, Vismanager, this });
//            if (fc == null)
//            {
//                return;
//            }

//            Type t = ((IFunctionExtension)fc).GetType();
//            AssemblyClassDefinition cls = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
//            if (cls != null)
//            {
//                if (TreeV.SelectedNode != null)
//                {
//                    TreeNode n = TreeV.SelectedNode;
//                    br = (IBranch)n.Tag;
//                }

//            }
//            methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();

//            if (DMEEditor.Passedarguments == null)
//            {
//                DMEEditor.Passedarguments = new PassedArgs();
//            }
//            if (br != null)
//            {
//                DMEEditor.Passedarguments.ObjectName = br.BranchText;
//                DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
//                DMEEditor.Passedarguments.Id = br.BranchID;
//                DMEEditor.Passedarguments.ParameterInt1 = br.BranchID;
//                if (!IsMethodApplicabletoNode(cls, br)) return;

//            }

//            if (methodsClass != null)
//            {
//                PassedArgs args = new PassedArgs();
//                ErrorsandMesseges = new ErrorsInfo();
//                args.Cancel = false;
//                PreCallModule?.Invoke(this, args);

//                if (args.Cancel)
//                {
//                    DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
//                    ErrorsandMesseges.Flag = Errors.Failed;
//                    ErrorsandMesseges.Message = $"Function Access Denied";
//                    return;
//                }
//                method = methodsClass.Info;
//                if (method.GetParameters().Length > 0)
//                {
//                    method.Invoke(fc, new object[] { DMEEditor.Passedarguments });
//                }
//                else
//                    method.Invoke(fc, null);
//            }
//        }
//        public void RunFunction(IBranch br, ToolStripItem item)
//        {
//            if (TreeV.SelectedNode != null)
//            {
//                TreeNode n = TreeV.SelectedNode;
//                br = (IBranch)n.Tag;
//            }
//            if (br != null)
//            {
//                if (DMEEditor.Passedarguments == null)
//                {
//                    DMEEditor.Passedarguments = new PassedArgs();
//                }

//                AssemblyClassDefinition assemblydef = (AssemblyClassDefinition)item.Tag;
//                dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { DMEEditor, VisManager, this });
//                Type t = ((IFunctionExtension)fc).GetType();
//                //dynamic fc = Activator.CreateInstance(assemblydef.type, new object[] { DMEEditor, Vismanager, this });
//                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
//                MethodInfo method = null;
//                MethodsClass methodsClass;
//                if (!IsMethodApplicabletoNode(cls, br)) return;

//                try
//                {
//                    if (br.BranchType != EnumPointType.Global)
//                    {
//                        methodsClass = cls.Methods.Where(x => x.Caption == item.Text).FirstOrDefault();
//                    }
//                    else
//                    {
//                        methodsClass = cls.Methods.Where(x => x.Caption == item.Text && x.PointType == br.BranchType).FirstOrDefault();
//                    }

//                }
//                catch (Exception)
//                {

//                    methodsClass = null;
//                }
//                if (methodsClass != null)
//                {
//                    PassedArgs args = new PassedArgs();
//                    ErrorsandMesseges = new ErrorsInfo();
//                    args.ObjectName = br.BranchText;
//                    args.DatasourceName = br.DataSourceName;
//                    args.Id = br.BranchID;
//                    args.ObjectType = methodsClass.ObjectType;
//                    args.ParameterInt1 = br.BranchID;
//                    args.Cancel = false;
//                    PreCallModule?.Invoke(this, args);
//                    if (args.Cancel)
//                    {
//                        DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {br.BranchText}", DateTime.Now, 0, br.BranchText, Errors.Failed);
//                        ErrorsandMesseges.Flag = Errors.Failed;
//                        ErrorsandMesseges.Message = $"Function Access Denied";
//                        return;
//                    }
//                    method = methodsClass.Info;
//                    if (method.GetParameters().Length > 0)
//                    {
//                        method.Invoke(fc, new object[] { DMEEditor.Passedarguments });
//                    }
//                    else
//                        method.Invoke(fc, null);
//                }
//            }


//        }
//        public IErrorsInfo RunMethod(object branch, string MethodName)
//        {

//            try
//            {
//                Type t = branch.GetType();
//                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == t.Name).FirstOrDefault();
//                MethodInfo method = null;
//                MethodsClass methodsClass;
//                try
//                {
//                    methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();
//                }
//                catch (Exception)
//                {

//                    methodsClass = null;
//                }
//                if (methodsClass != null)
//                {

//                    if (!IsMethodApplicabletoNode(cls, (IBranch)branch)) return DMEEditor.ErrorObject;
//                    PassedArgs args = new PassedArgs();
//                    args.ObjectName = MethodName;
//                    args.ObjectType = methodsClass.ObjectType;
//                    args.Cancel = false;
//                    PreCallModule?.Invoke(this, args);
//                    if (args.Cancel)
//                    {
//                        DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
//                        ErrorsandMesseges.Flag = Errors.Failed;
//                        ErrorsandMesseges.Message = $"Function Access Denied";
//                        return ErrorsandMesseges;
//                    }

//                    method = methodsClass.Info;
//                    if (method.GetParameters().Length > 0)
//                    {
//                        method.Invoke(branch, new object[] { DMEEditor.Passedarguments.Objects[0].obj });
//                    }
//                    else
//                        method.Invoke(branch, null);


//                    //  DMEEditor.AddLogMessage("Success", "Running method", DateTime.Now, 0, null, Errors.Ok);
//                }

//            }
//            catch (Exception ex)
//            {
//                string mes = "Could not Run Method " + MethodName;
//                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
//            };
//            return DMEEditor.ErrorObject;
//        }
//        public void Run(IPassedArgs pPassedarg)
//        {

//        }

//        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
//        {
//            return;
//        }
//        private void SetupTreeView()
//        {
//            TreeV.CheckBoxes = false;
//            RefreshImageList();



//            // TreeV.SelectedImageIndex = VisManager.visHelper.GetImageIndex(SelectIcon);

//        }
//        public IErrorsInfo TurnonOffCheckBox(IPassedArgs Passedarguments)
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            try
//            {

//                //TreeView trv = (TreeView)TreeEditor.TreeStrucure;
//                TreeV.CheckBoxes = !TreeV.CheckBoxes;
//                SelectedBranchs.Clear();

//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Fail", $"Could not select entities {ex.Message}", DateTime.Now, 0, Passedarguments.DatasourceName, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;

//        }
//        #region "TreeNode Handling"
//        public TreeNode GetTreeNodeByID(int id, TreeNodeCollection p_Nodes)
//        {
//            try
//            {
//                foreach (TreeNode node in p_Nodes)
//                {
//                    IBranch br = (IBranch)node.Tag;
//                    if (br.ID == id)
//                    {
//                        return node;
//                    }

//                    if (node.Nodes.Count > 0)
//                    {
//                        var result = GetTreeNodeByID(id, node.Nodes);
//                        if (result != null)
//                        {
//                            return result;
//                        }
//                    }
//                }
//                //     return p_Nodes.Find(id.ToString(), true).FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                return null;


//            }


//            return null;// TreeV.NodesControls.Cast<TreeNode>().Where(n => n.Tag.ToString() == tag).FirstOrDefault();
//        }
//        public TreeNode GetTreeNodeByCaption(string Caption, TreeNodeCollection p_Nodes)
//        {
//            foreach (TreeNode node in p_Nodes)
//            {
//                if (node.Text == Caption)
//                {
//                    return node;
//                }

//                if (node.Nodes.Count > 0)
//                {
//                    var result = GetTreeNodeByCaption(Caption, node.Nodes);
//                    if (result != null)
//                    {
//                        return result;
//                    }
//                }
//            }
//            return null;
//        }
//        #endregion
//        #region "Method Calling"
//        private bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
//        {
//            if (cls.classProperties == null)
//            {
//                return true;
//            }
//            if (cls.classProperties.ObjectType != null)
//            {
//                //if (!cls.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase))
//                //{
//                //    return false ;
//                //}
//            }
//            return true;


//        }
//        public void Nodemenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
//        {
//            ContextMenuStrip menu = (ContextMenuStrip)sender;
//            ToolStripItem item = e.ClickedItem;
//            TreeNode n = TreeV.SelectedNode;
//            menu.Hide();
//            IBranch br = (IBranch)n.Tag;
//            AssemblyClassDefinition cls = (AssemblyClassDefinition)item.Tag;

//            if (cls != null)
//            {
//                if (!IsMethodApplicabletoNode(cls, br)) return;
//                if (cls.componentType == "IFunctionExtension")
//                {
//                    RunFunction(br, item);

//                }
//                else
//                {

//                    RunMethod(br, item.Text);
//                };

//            }
//        }
//        public void Nodemenu_MouseClick(TreeNodeMouseClickEventArgs e)
//        {
//            // ContextMenuStrip n = (ContextMenuStrip)e.;
//            TreeNode n = TreeV.SelectedNode;

//            IBranch br = (IBranch)n.Tag;

//            if (br != null)
//            {
//                SelectedBranchID = br.ID;
//                DMEEditor.Passedarguments.CurrentEntity = br.BranchText;
//                if (br.BranchType == EnumPointType.DataPoint)
//                {
//                    if (!string.IsNullOrEmpty(br.DataSourceName))
//                    {
//                        DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
//                    }

//                }

                
//                string clicks = "";
//                if (e.Button == MouseButtons.Right)
//                {

//                    if (IsMenuCreated(br))
//                    {
//                        MenuList menuList = GetMenuList(br);
//                        ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
//                        ls.Show(Cursor.Position);
//                    }

//                }
//                else
//                {
//                    switch (e.Clicks)
//                    {
//                        case 1:
//                            clicks = "SingleClick";
//                            break;
//                        case 2:
//                            clicks = "DoubleClick";
//                            break;

//                        default:
//                            break;
//                    }
//                    AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == br.Name && x.Methods.Where(y => y.DoubleClick == true || y.Click == true).Any()).FirstOrDefault();
//                    if (cls != null)
//                    {
//                        if (!IsMethodApplicabletoNode(cls, br)) return;
//                        RunMethod(br, clicks);

//                    }

//                }

//            }

//        }

//        #endregion
//        #region "Filter Nodes"
//        private string _filterstring = string.Empty;
//        public string Filterstring {
//            get { return _filterstring; }
//            set { _filterstring = value; FilterString_TextChanged(value); } 
//        }

//        public string GuidID { get; set; }
//        int ITree.SeqID { get ; set ; }

//        private TreeView TreeCache = new TreeView();
//        private bool IsFiltering = false;
//        public void FilterString_TextChanged(string value)
//        {
//            if (TreeV != null)
//            {
//                if (TreeV.Nodes.Count > 0)
//                {
//                    if (IsFiltering == false)
//                    {
//                        TreeCache.Nodes.Clear();
//                        foreach (TreeNode _node in TreeV.Nodes)
//                        {
//                            TreeCache.Nodes.Add((TreeNode)_node.Clone());
//                        }
//                        IsFiltering = true;
//                    }

//                    //blocks repainting tree till all objects loaded

//                    TreeV.BeginUpdate();
//                    TreeV.Nodes.Clear();
//                    if (value != string.Empty)
//                    {
//                        foreach (TreeNode _parentNode in TreeCache.Nodes)
//                        {
//                            ScanNodes(_parentNode, value);
//                        }
//                    }
//                    else
//                    {
//                        foreach (TreeNode _node in TreeCache.Nodes)
//                        {
//                            TreeV.Nodes.Add((TreeNode)_node.Clone());
//                        }
//                        IsFiltering = false;
//                    }
//                    //enables redrawing tree after all objects have been added
//                    TreeV.EndUpdate();
//                }
//            }

//        }
//        private void ScanNodes(TreeNode _parentNode, string value)
//        {
//            foreach (TreeNode _childNode in _parentNode.Nodes)
//            {
//                if (_childNode.Text.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
//                {
//                    TreeV.Nodes.Add((TreeNode)_childNode.Clone());
//                }
//                if (_childNode.Nodes.Count > 0)
//                {
//                    ScanNodes(_childNode, value);
//                }
//            }

//        }



//        public object GetTreeNodeByID(int id)
//        {
//            return GetTreeNodeByID(id, TreeV.Nodes);
//        }

//        public void RemoveNode(int id)
//        {
//            TreeNode tr = (TreeNode)GetTreeNodeByID(id, TreeV.Nodes);
//            if (tr != null)
//            {
//                TreeV.Nodes.Remove(tr);
//            }

//        }
//        #endregion
//        #region "Refresh StandardTree"
//        public void RefreshTree()
//        {
//            TreeV.Nodes.Clear();
//            CreateRootTree();
//        }

//        public void RefreshTree(IBranch branch)
//        {
//            branch.CreateChildNodes();
//        }
//        public void RefreshTree(int branchid)
//        {
//            IBranch branch = Branches.Where(p => p.BranchID == branchid).FirstOrDefault();
//            if (branch != null)
//            {
//                RefreshTree(branch);
//            }
//        }
//        public void RefreshTree(string branchname)
//        {
//            IBranch branch = Branches.Where(p => p.BranchText == branchname).FirstOrDefault();
//            if (branch != null)
//            {
//                RefreshTree(branch);
//            }
//        }
//        public void RefreshImageList()
//        {
//            // visHelper = (VisHelper)VisManager.visHelper;
//            TreeV.ImageList = new ImageList();
//            //  TreeV.StateImageList = new ImageList();
//            TreeV.ImageList = GetImageList();

//            //   TreeV.StateImageList = visHelper.ImageList32;
//        }
//        #endregion
//        #region "new methods"
//        public void ChangeBranchIcon(int branchid, string iconname)
//        {
//            IBranch br = Branches.Where(p => p.ID == branchid).FirstOrDefault();
//            if (br != null)
//            {
//                ChangeBranchIcon(br, iconname);
//            }
//        }
//        public void ChangeBranchIcon(string branchname, string iconname)
//        {
//            IBranch br = Branches.Where(p => p.BranchText == branchname).FirstOrDefault();
//            if (br != null)
//            {
//                ChangeBranchIcon(br, iconname);
//            }
//        }
//        public void ChangeBranchIcon(IBranch branch, string iconname)
//        {
//            TreeNode n = GetTreeNodeByID(branch.ID, TreeV.Nodes);
//            if (n != null)
//            {
//                branch.IconImageName = iconname;

//                refreshBranch(n, branch);
//            }
//        }
//        public void ChangeBranchText(int branchid, string text)
//        {
//            IBranch br = Branches.Where(p => p.ID == branchid).FirstOrDefault();
//            if (br != null)
//            {
//                ChangeBranchText(br, text);
//            }
//        }
//        public void ChangeBranchText(string branchname, string text)
//        {
//            IBranch br = Branches.Where(p => p.BranchText == branchname).FirstOrDefault();
//            if (br != null)
//            {
//                ChangeBranchText(br, text);
//            }
//        }
//        public void ChangeBranchText(IBranch branch, string text)
//        {
//            TreeNode n = GetTreeNodeByID(branch.ID, TreeV.Nodes);
//            if (n != null)
//            {
//                branch.BranchText = text;
//                n.Text = text;
//                refreshBranch(n, branch);
//            }
//        }

//        public IBranch GetBranchByGuidID(string guidid)
//        {
//            if (Branches != null)
//            {
//                if (Branches.Count > 0)
//                {
//                    return Branches.Where(p => p.GuidID == guidid).FirstOrDefault();
//                }
//            }
//            return null;
//        }
//        public IBranch GetBranchByMiscGuidID(string guidid)
//        {
//            if (Branches != null)
//            {
//                if (Branches.Count > 0)
//                {
//                    return Branches.Where(p => p.MiscStringID == guidid).FirstOrDefault();
//                }
//            }
//            return null;
//        }
//        public IBranch GetBranchByEntityGuidID(string guidid)
//        {
//            if (Branches != null)
//            {
//                if (Branches.Count > 0)
//                {
//                    return Branches.Where(p => p.EntityGuidID == guidid).FirstOrDefault();
//                }
//            }
//            return null;
//        }
//        private void refreshBranch(TreeNode n, IBranch br)
//        {
//            if (br != null)
//            {
//                int imgidx = VisManager.visHelper.GetImageIndex(br.IconImageName);
//                if (imgidx == -1)
//                {
//                    imgidx = VisManager.visHelper.GetImageIndexFromConnectioName(br.BranchText);
//                }
//                if (imgidx == -1)
//                {
//                    n.ImageKey = br.IconImageName;
//                    n.SelectedImageKey = br.IconImageName;

//                }
//                else
//                {
//                    n.ImageIndex = imgidx;
//                    n.SelectedImageIndex = imgidx;

//                }
//                n.Text = br.BranchText;
//            }
//        }

//        public void Run(params object[] args)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
