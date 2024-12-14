using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Tree;
using TheTechIdea.Beep.Winform.Controls.Common;
using static TheTechIdea.Beep.Utilities.Util;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepBranchITree : BeepTree, ITree
    {
        private IBeepService services;

        public BeepBranchITree( IBeepService beepService, ITreeBranchHandler branchHandler,IVisManager vis)
        {
            services= beepService;
            treeBranchHandler = branchHandler;
            VisManager = vis;
            DMEEditor=services.DMEEditor; 
        }
        #region "Properties"
        public string CategoryIcon { get; set; }= "folder";
        public string SelectIcon { get; set; } = "select";
        public string TreeType { get; set; }
        public IBranch CurrentBranch { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public List<int> SelectedBranchs { get; set; } = new List<int>();
        public PassedArgs args { get; set; }

        private int _SeqID=0;
        public int SeqID => _SeqID++;

        public List<IBranch> Branches { get; set; } = new List<IBranch>();
        public List<Tuple<IBranch, string>> GenerBranchs { get; set; }=new List<Tuple<IBranch, string>>();
        public IVisManager VisManager { get; set; }
        public int SelectedBranchID { get; set; }
        public ITreeBranchHandler treeBranchHandler { get; set; }
        public string Filterstring { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; private set; }
        #endregion "Properties"
        #region "Events"   
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<IBranch> RefreshBranch;
        public event EventHandler<IBranch> RefreshChildBranchs;
        public event EventHandler<IBranch> RefreshParentBranch;
        public event EventHandler<IBranch> RefreshBranchIcon;
        public event EventHandler<IBranch> RefreshBranchText;
        #endregion "Events"
        #region "Change Branch Methods"
        public IErrorsInfo TurnonOffCheckBox(IPassedArgs Passedarguments)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }
        public void ChangeBranchIcon(int branchid, string iconname)
        {

        }

        public void ChangeBranchIcon(string branchname, string iconname)
        {

        }

        public void ChangeBranchIcon(IBranch branch, string iconname)
        {

        }

        public void ChangeBranchText(int branchid, string text)
        {

        }

        public void ChangeBranchText(string branchname, string text)
        {

        }

        public void ChangeBranchText(IBranch branch, string text)
        {

        }
        #endregion "Change Branch Methods"
        #region "Create Branch Methods"
        public IErrorsInfo CreateFunctionExtensions(MethodsClass item)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        //public IErrorsInfo CreateRootTree()
        //{
        //    IErrorsInfo retval = new ErrorsInfo();
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        retval.Message = ex.Message;
        //        retval.Flag = Errors.Failed;

        //    }
        //    return retval;
        //}
        public IErrorsInfo CreateRootTree()
        {
            string packagename = "";
            try
            {
                //bool HasConstructor=false;
              //  SetupTreeView();
         //       treeNodeDragandDropHandler = new TreeNodeDragandDropHandler(DMEEditor, this);
              //  treeBranchHandler = new TreeBranchHandler(DMEEditor, this, this);
                Branches = new List<IBranch>();
                IBranch Genrebr = null;
                // AssemblyClassDefinition GenreBrAssembly = DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).FirstOrDefault()!;
                foreach (AssemblyClassDefinition GenreBrAssembly in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).OrderBy(x => x.Order))
                {
                    if (GenreBrAssembly != null)
                    {

                        Type adc = DMEEditor.assemblyHandler.GetType(GenreBrAssembly.PackageName);
                        ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
                        if (ctor != null)
                        {
                            ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                            try
                            {
                                Genrebr = createdActivator();
                                if (Genrebr.BranchType == EnumPointType.Genre)
                                {
                                    if (GenreBrAssembly.PackageName != null)
                                    {
                                        PassedArgs x = new PassedArgs { ObjectType = GenreBrAssembly.PackageName };
                                        x.Cancel = false;
                                        x.AddinName = GenreBrAssembly.PackageName;
                                        x.AddinType = GenreBrAssembly.componentType;
                                        x.CurrentEntity = GenreBrAssembly.PackageName;
                                        x.ObjectName = GenreBrAssembly.className;
                                        x.ObjectType = GenreBrAssembly.classProperties.ClassType;
                                        PreShowItem?.Invoke(this, x);
                                        //   VisManager.Addins.Where(p => p.ObjectName == GenreBrAssembly.className).FirstOrDefault().Run();
                                        if (x.Cancel)
                                        {
                                            Genrebr.Visible = false;
                                        }
                                    }
                                    int id = SeqID;
                                    Genrebr.Name = GenreBrAssembly.PackageName;
                                    packagename = GenreBrAssembly.PackageName;
                                    Genrebr.ID = id;
                                    Genrebr.BranchID = id;
                                    if (TreeType != null)
                                    {
                                        if (GenreBrAssembly.classProperties.ObjectType != null)
                                        {
                                            if (GenreBrAssembly.classProperties.ObjectType.Equals(TreeType, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                if (Genrebr.Visible)
                                                {
                                                    CreateNode(id, Genrebr);
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Genrebr.Visible)
                                        {
                                            CreateNode(id, Genrebr);
                                        }
                                    }

                                    GenerBranchs.Add(new Tuple<IBranch, string>(Genrebr, GenreBrAssembly.classProperties.menu));
                                }
                            }
                            catch (Exception ex)
                            {
                                DMEEditor.AddLogMessage("Error", $"Creating Tree Root Node {GenreBrAssembly.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                            }
                        }
                    }
                }

                foreach (AssemblyClassDefinition cls in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null).OrderBy(x => x.Order))
                {
                    Type adc = DMEEditor.assemblyHandler.GetType(cls.PackageName);
                    ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;

                    if (ctor != null)
                    {
                        ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                        try
                        {
                            IBranch br = createdActivator();
                            if (br.BranchType == EnumPointType.Root)
                            {
                                int id = SeqID;
                                br.Name = cls.PackageName;
                                packagename = cls.PackageName;
                                br.ID = id;
                                br.BranchID = id;
                                if (cls.PackageName != null)
                                {
                                    PassedArgs x = new PassedArgs { ObjectType = cls.PackageName };
                                    x.Cancel = false;
                                    x.AddinName = cls.PackageName;
                                    x.AddinType = cls.componentType;
                                    x.CurrentEntity = cls.PackageName;
                                    x.ObjectName = cls.className;
                                    x.ObjectType = cls.classProperties.ClassType;
                                    PreShowItem?.Invoke(this, x);
                                    //   VisManager.Addins.Where(p => p.ObjectName == GenreBrAssembly.className).FirstOrDefault().Run();
                                    if (x.Cancel)
                                    {
                                        br.Visible = false;
                                    }
                                }
                                if (TreeType != null)
                                {
                                    if (cls.classProperties.ObjectType != null)
                                    {
                                        if (cls.classProperties.ObjectType.Equals(TreeType, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            var tr = GenerBranchs.FirstOrDefault(p => p.Item2.Equals(cls.classProperties.menu, StringComparison.OrdinalIgnoreCase));
                                            if (tr != null)
                                            {
                                                Genrebr = tr.Item1;
                                            }
                                            else
                                                Genrebr = null;

                                            if (Genrebr != null)
                                            {
                                                if (Genrebr.Visible)
                                                {
                                                    treeBranchHandler.AddBranch(Genrebr, br);
                                                    br!.CreateChildNodes();
                                                }

                                            }
                                            else
                                            {
                                                if (br.Visible)
                                                {
                                                    CreateNode(id, br);
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (cls.classProperties.Category != DatasourceCategory.NONE)
                                    {
                                        treeBranchHandler.AddBranch(Genrebr, br);
                                        br!.CreateChildNodes();
                                    }
                                    else
                                    {
                                        if (br.Visible)
                                        {
                                            CreateNode(id, br);
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DMEEditor.AddLogMessage("Error", $"Creating Tree Root Node {cls.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.AddLogMessage("Error", $"Creating Tree Root Node {packagename} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);

            };
            return DMEEditor.ErrorObject;
        }
 
        private void CreateNode(int id, IBranch br)
        {
            BeepTreeNode n = CreateTreeNode(br.BranchText,br.IconImageName,br.GuidID,"IBRANCH", null);
            n.Tag = br;
            br.TreeEditor = this;
            br.Visutil = VisManager;
            br.BranchID = id;
            br.ID = id;
            n.Name = br.ID.ToString();
          
            //br.ParentBranch = n;
            int imgidx = VisManager.visHelper.GetImageIndex(br.IconImageName);
            if (imgidx == -1)
            {
                imgidx = VisManager.visHelper.GetImageIndexFromConnectioName(br.BranchText);
            }
            if (imgidx == -1)
            {
               n.ImagePath = CategoryIcon;
            }
            //n.ContextMenuStrip = 
            Console.WriteLine(br.BranchText);
            CreateMenuMethods(br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                CreateGlobalMenu(br);
            }

            br.DMEEditor = DMEEditor;
            if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
            {
                DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
            }
            try
            {
                br.SetConfig(this, DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
            }
            catch (Exception ex)
            {

            }

            Branches.Add(br);
            AddNode(n);
            br.CreateChildNodes();
        }
        #endregion "Create Branch Methods"
        #region "Get Branch Methods"
        public IBranch GetBranchByEntityGuidID(string guidid)
        {
            IBranch retval =null;
            try
            {

            }
            catch (Exception ex)
            {
              
            }
            return retval;
        }

        public IBranch GetBranchByGuidID(string guidid)
        {
            IBranch retval = null;
            try
            {

            }
            catch (Exception ex)
            {

            }
            return retval;
        }

        public IBranch GetBranchByMiscGuidID(string guidid)
        {
            IBranch retval = null;
            try
            {

            }
            catch (Exception ex)
            {

            }
            return retval;
        }

        public object GetTreeNodeByID(int id)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }
        #endregion "Get Branch Methods"
        #region "Refresh Branch Methods"

        public void RefreshImageList()
        {

        }

        public void RefreshTree()
        {

        }

        public void RefreshTree(IBranch branch)
        {

        }

        public void RefreshTree(int branchid)
        {

        }

        public void RefreshTree(string branchname)
        {

        }
        #endregion "Refresh Branch Methods"
        #region "Run Method"
        public void RunFunction(object sender, EventArgs e)
        {
            IBranch br = null;
            AssemblyClassDefinition assemblydef = new AssemblyClassDefinition();
            MethodInfo method = null;
            MethodsClass methodsClass;
            string MethodName = "";
            if (sender == null) { return; }
            if (sender.GetType() == typeof(ToolStripButton))
            {
                ToolStripButton item = (ToolStripButton)sender;
                assemblydef = (AssemblyClassDefinition)item.Tag;
                MethodName = item.Text;
            }
            if (sender.GetType() == typeof(ToolStripMenuItem))
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                assemblydef = (AssemblyClassDefinition)item.Tag;
                MethodName = item.Text;
            }
            dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { DMEEditor, VisManager, this });
            //  dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.type.ToString(), new object[] { DMEEditor, Vismanager, this });
            if (fc == null)
            {
                return;
            }

            Type t = ((IFunctionExtension)fc).GetType();
            AssemblyClassDefinition cls = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
            if (cls != null)
            {
                if (SelectedNode != null)
                {
                    BeepTreeNode n = SelectedNode;
                    br = (IBranch)n.Tag;
                }

            }
            methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();

            if (DMEEditor.Passedarguments == null)
            {
                DMEEditor.Passedarguments = new PassedArgs();
            }
            if (br != null)
            {
                DMEEditor.Passedarguments.ObjectName = br.BranchText;
                DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
                DMEEditor.Passedarguments.Id = br.BranchID;
                DMEEditor.Passedarguments.ParameterInt1 = br.BranchID;
                if (!IsMethodApplicabletoNode(cls, br)) return;

            }

            if (methodsClass != null)
            {
                PassedArgs args = new PassedArgs();
                ErrorsandMesseges = new ErrorsInfo();
                args.Cancel = false;
                PreCallModule?.Invoke(this, args);

                if (args.Cancel)
                {
                    DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Function Access Denied";
                    return;
                }
                method = methodsClass.Info;
                if (method.GetParameters().Length > 0)
                {
                    method.Invoke(fc, new object[] { DMEEditor.Passedarguments });
                }
                else
                    method.Invoke(fc, null);
            }
        }
        public void RunFunction(IBranch br, ToolStripItem item)
        {
            if (SelectedNode != null)
            {
                BeepTreeNode n = SelectedNode;
                br = (IBranch)n.Tag;
            }
            if (br != null)
            {
                if (DMEEditor.Passedarguments == null)
                {
                    DMEEditor.Passedarguments = new PassedArgs();
                }

                AssemblyClassDefinition assemblydef = (AssemblyClassDefinition)item.Tag;
                dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { DMEEditor, VisManager, this });
                Type t = ((IFunctionExtension)fc).GetType();
                //dynamic fc = Activator.CreateInstance(assemblydef.type, new object[] { DMEEditor, Vismanager, this });
                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
                MethodInfo method = null;
                MethodsClass methodsClass;
                if (!IsMethodApplicabletoNode(cls, br)) return;

                try
                {
                    if (br.BranchType != EnumPointType.Global)
                    {
                        methodsClass = cls.Methods.Where(x => x.Caption == item.Text).FirstOrDefault();
                    }
                    else
                    {
                        methodsClass = cls.Methods.Where(x => x.Caption == item.Text && x.PointType == br.BranchType).FirstOrDefault();
                    }

                }
                catch (Exception)
                {

                    methodsClass = null;
                }
                if (methodsClass != null)
                {
                    PassedArgs args = new PassedArgs();
                    ErrorsandMesseges = new ErrorsInfo();
                    args.ObjectName = br.BranchText;
                    args.DatasourceName = br.DataSourceName;
                    args.Id = br.BranchID;
                    args.ObjectType = methodsClass.ObjectType;
                    args.ParameterInt1 = br.BranchID;
                    args.Cancel = false;
                    PreCallModule?.Invoke(this, args);
                    if (args.Cancel)
                    {
                        DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {br.BranchText}", DateTime.Now, 0, br.BranchText, Errors.Failed);
                        ErrorsandMesseges.Flag = Errors.Failed;
                        ErrorsandMesseges.Message = $"Function Access Denied";
                        return;
                    }
                    method = methodsClass.Info;
                    if (method.GetParameters().Length > 0)
                    {
                        method.Invoke(fc, new object[] { DMEEditor.Passedarguments });
                    }
                    else
                        method.Invoke(fc, null);
                }
            }


        }
        public IErrorsInfo RunMethod(object branch, string MethodName)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }
        #endregion "Run Method"
        #region "Menu Methods"
        #region "Menu Generation"
        public IErrorsInfo CreateGlobalMenu(IBranch br)
        {
            try
            {
                MenuList menuList = new MenuList();
                if (!IsMenuCreated(br))
                {
                    menuList = new MenuList(br.ObjectType, br.BranchClass, br.BranchType);
                    menuList.branchname = br.BranchText;
                    Menus.Add(menuList);
                    ContextMenuStrip nodemenu = new ContextMenuStrip();
                    nodemenu.ImageList = GetImageList();
                    //nodemenu.ItemClicked -= Nodemenu_ItemClicked;
                    //nodemenu.ItemClicked += Nodemenu_ItemClicked;
                    nodemenu.Items.Clear();
                    menuList.Menu = nodemenu;
                    menuList.ObjectType = br.ObjectType;
                    menuList.BranchClass = br.BranchClass;
                }
                else
                    menuList = GetMenuList(br);
                List<AssemblyClassDefinition> extentions = DMEEditor.ConfigEditor.GlobalFunctions.Where(o => o.classProperties != null && o.classProperties.ObjectType != null && o.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Order).ToList(); //&&  o.classProperties.menu.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                foreach (AssemblyClassDefinition cls in extentions)
                {
                    if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        menuList.classDefinitions.Add(cls);
                        foreach (var item in cls.Methods)
                        {
                            if (string.IsNullOrEmpty(item.ClassType))
                            {
                                if (item.PointType == br.BranchType)
                                {
                                    ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
                                    ToolStripItem st = ls.Items.Add(item.Caption);
                                    ls.Name = br.ToString();
                                    if (item.iconimage != null)
                                    {
                                        st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
                                    }
                                    st.Tag = cls;
                                    ls.Tag = cls;
                                    ls.ItemClicked -= Nodemenu_ItemClicked;
                                    ls.ItemClicked += Nodemenu_ItemClicked;
                                }
                            }
                            else
                            {
                                if ((item.PointType == br.BranchType) && (br.BranchClass.Equals(item.ClassType, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
                                    ToolStripItem st = ls.Items.Add(item.Caption);
                                    ls.Name = br.ToString();
                                    if (item.iconimage != null)
                                    {
                                        st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
                                    }
                                    st.Tag = cls;
                                    ls.Tag = cls;
                                    ls.ItemClicked -= Nodemenu_ItemClicked;
                                    ls.ItemClicked += Nodemenu_ItemClicked;
                                }
                            }
                        }
                    }
                }
                return DMEEditor.ErrorObject;
            }
            catch (Exception ex)
            {
                return DMEEditor.ErrorObject;
            }
        }
        public ContextMenuStrip CreateMenuMethods(IBranch branch)
        {
            ContextMenuStrip ls = null;
            MenuList menuList = new MenuList();
            if (!IsMenuCreated(branch))
            {
                menuList = new MenuList(branch.ObjectType, branch.BranchClass, branch.BranchType);
                menuList.branchname = branch.BranchText;
                Menus.Add(menuList);
                ContextMenuStrip nodemenu = new ContextMenuStrip();
                nodemenu.Items.Clear();
                nodemenu.ImageList = GetImageList();
                //nodemenu.ItemClicked -= Nodemenu_ItemClicked;
                //nodemenu.ItemClicked += Nodemenu_ItemClicked;
                menuList.Menu = nodemenu;
                menuList.ObjectType = branch.ObjectType;
                menuList.BranchClass = branch.BranchClass;
            }
            else
                menuList = GetMenuList(branch);
            try
            {
                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == branch.ToString()).FirstOrDefault();
                if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    menuList.classDefinitions.Add(cls);
                    foreach (var item in cls.Methods.Where(y => y.Hidden == false))
                    {
                        ls = (ContextMenuStrip)menuList.Menu;
                        ToolStripItem st = ls.Items.Add(item.Caption);
                        ls.Name = branch.ToString();
                        ls.ItemClicked -= Nodemenu_ItemClicked;
                        ls.ItemClicked += Nodemenu_ItemClicked;
                        if (item.iconimage != null)
                        {
                            try
                            {
                                st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
                            }
                            catch (Exception)
                            {
                                string mes = $"Could not get image {item.iconimage} for menthod on branch {branch.BranchText}";
                                DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                            }

                        }
                        st.Tag = cls;
                        ls.Tag = cls;
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not add method to menu " + branch.BranchText;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return ls;
        }
        public void Nodemenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            ToolStripItem item = e.ClickedItem;
            BeepTreeNode n = SelectedNode;
            menu.Hide();
            IBranch br = (IBranch)n.Tag;
            AssemblyClassDefinition cls = (AssemblyClassDefinition)item.Tag;

            if (cls != null)
            {
                if (!IsMethodApplicabletoNode(cls, br)) return;
                if (cls.componentType == "IFunctionExtension")
                {
                    RunFunction(br, item);

                }
                else
                {

                    RunMethod(br, item.Text);
                };

            }
        }
        public void Nodemenu_MouseClick(TreeNodeMouseClickEventArgs e)
        {
            // ContextMenuStrip n = (ContextMenuStrip)e.;
            BeepTreeNode n = SelectedNode;

            IBranch br = (IBranch)n.Tag;

            if (br != null)
            {
                SelectedBranchID = br.ID;
                DMEEditor.Passedarguments.CurrentEntity = br.BranchText;
                if (br.BranchType == EnumPointType.DataPoint)
                {
                    if (!string.IsNullOrEmpty(br.DataSourceName))
                    {
                        DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
                    }

                }

                VisManager.Helpers.GetValues(DMEEditor.Passedarguments);
                VisManager.Helpers.pbr = br;

                string clicks = "";
                if (e.Button == MouseButtons.Right)
                {

                    if (IsMenuCreated(br))
                    {
                        MenuList menuList = GetMenuList(br);
                        ContextMenuStrip ls = (ContextMenuStrip)menuList.Menu;
                        ls.Show(Cursor.Position);
                    }

                }
                else
                {
                    switch (e.Clicks)
                    {
                        case 1:
                            clicks = "SingleClick";
                            break;
                        case 2:
                            clicks = "DoubleClick";
                            break;

                        default:
                            break;
                    }
                    AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == br.Name && x.Methods.Where(y => y.DoubleClick == true || y.Click == true).Any()).FirstOrDefault();
                    if (cls != null)
                    {
                        if (!IsMethodApplicabletoNode(cls, br)) return;
                        RunMethod(br, clicks);

                    }

                }

            }

        }

        #endregion "Menu Generation"
        #endregion "Menu Methods"
    }
}
