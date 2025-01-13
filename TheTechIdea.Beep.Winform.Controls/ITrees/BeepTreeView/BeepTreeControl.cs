using System.ComponentModel;
using System.Reflection;
using System.Xml.Linq;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using static TheTechIdea.Beep.Utilities.Util;


namespace TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView
{
    [ToolboxItem(true)]
    [DisplayName("Beep Tree Control")]
    [Category("Beep Controls")]
    [Description("A control that displays hierarchical data in a tree format.")]
    public partial class BeepTreeControl : BeepTree,ITree
    {
        public BeepTreeControl()
        {
            
        }
        public BeepTreeControl(IBeepService service)
        {
            BeepService = service;
           
          
        }
        public void init(IBeepService service)
        {
            VisManager = service.vis;
            DMEEditor = service.DMEEditor;
            treeBranchHandler = new BeepTreeBranchHandler(service, this);
            DropHandler = new BeepTreeNodeDragandDropHandler(service, this);
            this.NodeRightClicked += BeepTreeControl_NodeRightClicked;
            this.NodeClicked += BeepTreeControl_NodeClicked;
            this.SelectedItemChanged += BeepTreeControl_SelectedItemChanged;
        }

      



        #region "Properties"

        public string ObjectType { get; set; }= "Beep";
        public string CategoryIcon { get; set; }= "Category.svg";
        public string SelectIcon { get; set; } = "Select.svg";
        public string TreeType { get; set; } = "Beep";
        public IBranch CurrentBranch { get; set; }
        public IBeepService BeepService { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public List<int> SelectedBranchs { get; set; } = new List<int>();
        public PassedArgs args { get; set; }
        public int SeqID { get; set; }
        public List<IBranch> Branches { get; set; }=new List<IBranch>();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Tuple<IBranch, string>> GenerBranchs { get; set; } = new List<Tuple<IBranch, string>>();
        public List<MenuList> Menus { get; set; } = new List<MenuList>();
        public IVisManager VisManager { get; set; }
        public int SelectedBranchID { get; set; }
        public string Filterstring { get; set; }
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
        #region "Handlers"
        public IErrorsInfo RunMethod(object branch, string MethodName)
        {
            try
            {
                ControlExtensions.RunMethodFromBranch(this, branch, MethodName);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
           
        }
        public ITreeBranchHandler treeBranchHandler { get; set; }
        public BeepTreeNodeDragandDropHandler DropHandler { get; set; }
        public IBranch SelectedBranch { get; private set; }
        public SimpleItem SelectedItem { get; private set; }

        private void BeepTreeControl_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (SelectedBranch == null)
            {
                Console.WriteLine("Selected Branch is null");
                return;
            }
            AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == SelectedBranch.Name ).FirstOrDefault();
           
            if (cls != null)
            {
                if (!DynamicFunctionCallingManager.IsMethodApplicabletoNode(cls, SelectedBranch)) return;

                RunMethod(SelectedBranch, e.SelectedItem.Text);

            }
        }
        private void BeepTreeControl_NodeClicked(object? sender, BeepMouseEventArgs e)
        {
            BeepTreeNode node = (BeepTreeNode)sender;
            IBranch br =Branches.FirstOrDefault(c => c.GuidID == node.GuidID);
            AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == br.Name && x.Methods.Where(y => y.DoubleClick == true || y.Click == true).Any()).FirstOrDefault();
            if (cls != null)
            {
                if (!DynamicFunctionCallingManager.IsMethodApplicabletoNode(cls, br)) return;
                
                RunMethod(br, br.BranchText);

            }
        }
        private void BeepTreeControl_NodeRightClicked(object? sender, BeepMouseEventArgs e)
        {
           var clickedNode = sender as BeepTreeNode;
            if (clickedNode == null) return;
            SelectedBranch= GetBranchByGuidID(clickedNode.SavedGuidID);
            SelectedItem=GetNodeByGuidID(clickedNode.GuidID);
            var a = DynamicMenuManager.GetMenuItemsList((SimpleItem)clickedNode.Tag);
            if (a == null) return;
            CurrentMenutems = new BindingList<SimpleItem>(a);
            if ( CurrentMenutems.Count > 0)
            {
                LastNodeMenuShown = clickedNode;
                TogglePopup();
            }
        }
        #endregion "Handlers"
        #region "Change Branch Properties"
        public void ChangeBranchIcon(int branchid, string iconname)
        {
            
        }

        public void ChangeBranchIcon(string branchname, string iconname)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchIcon(IBranch branch, string iconname)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(int branchid, string text)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(string branchname, string text)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(IBranch branch, string text)
        {
            throw new NotImplementedException();
        }
        public void RemoveNode(int id)
        {
            throw new NotImplementedException();
        }
        public IErrorsInfo TurnonOffCheckBox(IPassedArgs Passedarguments)
        {
            throw new NotImplementedException();
        }
        #endregion "Change Branch Properties"
        #region "Create Branches"
        public IErrorsInfo CreateFunctionExtensions(MethodsClass item)
        {
            try
            {
                ControlExtensions.CreateFunctionExtensions(this,item);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
           
        }
        public IErrorsInfo CreateRootTree()
        {
            string packagename = "";
            try
            {
               Branches = new List<IBranch>();
                IBranch Genrebr = null;
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
            SimpleItem item=ControlExtensions.CreateNode(this,id, br);
            //n.ContextMenuStrip = 
            Console.WriteLine(br.BranchText);
            DynamicMenuManager.CreateMenuMethods(DMEEditor, br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(DMEEditor,br);
            }

            br.DMEEditor = DMEEditor;
            if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
            {
                DMEEditor.ConfigEditor.objectTypes.Add(new Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
            }
            try
            {
                br.SetConfig(this, DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
            }
            catch (Exception ex)
            {

            }
            br.DMEEditor = DMEEditor;
            br.Visutil = VisManager;
            br.TreeEditor = this;
            Nodes.Add(item);
            Branches.Add(br);
            br.CreateChildNodes();
        }
        //public IErrorsInfo CreateRootTree()
        //{
        //    IBranch Genrebr;
        //    try
        //    {
        //        var items = ControlExtensions.CreateTreeTuple(this, DMEEditor);
        //        Branches = items.Item1;
        //        GenerBranchs = items.Item2;
        //        Nodes = ControlExtensions.GetBranchs(this, items);
        //        Branches.AddRange(GenerBranchs.Select(c => c.Item1).ToList());
        //        foreach (var item in Branches)
        //        {
        //            PostScanBranches(item);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
        //    }
        //    return DMEEditor.ErrorObject;
        //}
        //private void PostScanBranches(IBranch branch)
        //{
        //    try
        //    {
        //        SimpleItem node = GetNodeByGuidID(branch.GuidID);
        //        if (node == null)
        //        {

        //        }
        //        Console.WriteLine(node.ToString());
        //        ControlExtensions.GetSimpleItemsFromExecuteCreateChildsMethods(this, branch);
        //        if (branch.ChildBranchs.Count > 0)
        //        {
        //            foreach (var item in branch.ChildBranchs)
        //            {
        //                PostScanBranches(item);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
        //    }
        //}
        #endregion "Create Branches"
        #region "Get Branches"
        #region "Get Branches"

        public IBranch GetBranchByEntityGuidID(string guidid)
        {
            return FindBranchRecursive(Branches, branch => branch.EntityGuidID == guidid);
        }

        public IBranch GetBranchByGuidID(string guidid)
        {
            return FindBranchRecursive(Branches, branch => branch.GuidID == guidid);
        }

        public IBranch GetBranchByMiscGuidID(string guidid)
        {
            return FindBranchRecursive(Branches, branch => branch.MiscStringID == guidid);
        }

        public object GetTreeNodeByID(int id)
        {
            return FindBranchRecursive(Branches, branch => branch.ID == id);
        }

        private IBranch FindBranchRecursive(IEnumerable<IBranch> branches, Func<IBranch, bool> predicate)
        {
            foreach (var branch in branches)
            {
                if (predicate(branch))
                {
                    return branch;
                }

                var childResult = FindBranchRecursive(branch.ChildBranchs, predicate);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }

        #endregion "Get Branches"

        #endregion "Get Branches"
        #region "Refresh Tree"
        public void RefreshImageList()
        {
            throw new NotImplementedException();
        }

        public void RefreshTree()
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(IBranch branch)
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(int branchid)
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(string branchname)
        {
            throw new NotImplementedException();
        }
        #endregion "Refresh Tree"
        #region "Select Branch "
        #endregion "Select Branch "
        #region "Mouse Events"
      
        #endregion "Mouse Events"


    }
}
