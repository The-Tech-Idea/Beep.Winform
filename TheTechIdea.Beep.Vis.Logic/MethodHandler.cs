using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Addin;
using System.Linq;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.ConfigUtil;
using static TheTechIdea.Beep.Utilities.Util;

namespace TheTechIdea.Beep.Vis.Logic
{
    public static class MethodHandler
    {
        public static List<IBranch> Branches { get; set; } = new List<IBranch>();
        public static List<Tuple<IBranch, string>> GenerBranchs = new List<Tuple<IBranch, string>>();
        public static IDMEEditor DMEEditor { get; set; }
        private static string uri = "";
        private static IPassedArgs passedArgs;
        
        public static AssemblyClassDefinition GetAssemblyClassDefinition(string PackageName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == PackageName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByClassName(string ClassName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == ClassName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectType(string ObjectType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClass(string ObjectType, string BranchClass)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndDataSourceType(string ObjectType, string BranchClass, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointType(string ObjectType, string BranchClass, EnumPointType PointType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointTypeAndDataSourceType(string ObjectType, string BranchClass, EnumPointType PointType, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByGuid(string Guid)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.GuidID == Guid).FirstOrDefault();
        }
        public static List<MenuList> Menus { get; set; } = new List<MenuList>();
        public static List<MenuItem> CreateMenuMethods(IBranch branch)
        {
            AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == branch.ToString()).FirstOrDefault();
            MenuList menuList = new MenuList();
            if (!IsMenuCreated(branch))
            {
                menuList = new MenuList(branch.ObjectType, branch.BranchClass, branch.BranchType);
                menuList.branchname = branch.BranchText;
                Menus.Add(menuList);
                menuList.ObjectType = branch.ObjectType;
                menuList.BranchClass = branch.BranchClass;
                
                menuList.Items = new List<MenuItem>();
            }
            else
                menuList = GetMenuList(branch);
            try
            {
                
                if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    menuList.classDefinitions.Add(cls);
                    foreach (var item in cls.Methods.Where(y => y.Hidden == false))
                    {
                        MenuItem mi = new MenuItem();
                        mi.Name = item.Caption;
                        mi.MethodName = item.Caption;
                        mi.Text = item.Caption;
                        mi.ObjectType = item.ObjectType;
                        mi.BranchClass = item.ClassType;
                        mi.PointType = item.PointType;
                        mi.ClassDefinition = cls;
                        mi.Category = item.Category;
                        mi.imagename = item.iconimage;
                        mi.MethodAttribute = item.CommandAttr;

                        menuList.Items.Add(mi);
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not add method to menu " + branch.BranchText;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return menuList.Items;
        }
        public static bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
        {
            if (cls.classProperties == null)
            {
                return true;
            }
            if (cls.classProperties.ObjectType != null)
            {
                if (!cls.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            return true;


        }
        public static IErrorsInfo CreateFunctionExtensions(ITree tree,MethodsClass item)
        {
            
            try
            {
                List<AssemblyClassDefinition> extentions = DMEEditor.ConfigEditor.GlobalFunctions.Where(o => o.classProperties != null && o.classProperties.ObjectType != null ).OrderBy(p => p.Order).ToList(); //&&  o.classProperties.menu.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                AssemblyClassDefinition cls = extentions.FirstOrDefault(x =>x.Methods.Any(y => y.GuidID == item.GuidID));

                // Ensure cls is not null before using it to avoid NullReferenceException.
                if (cls == null)
                {
                   return DMEEditor.ErrorObject;
                }

                foreach (IBranch br in tree.Branches)
                {
                    MenuList menuList = new MenuList();
                    if (!IsMenuCreated(br))
                    {
                        menuList = new MenuList(br.ObjectType, br.BranchClass, br.BranchType);
                        menuList.branchname = br.BranchText;
                        Menus.Add(menuList);
                        menuList.ObjectType = br.ObjectType;
                        menuList.BranchClass = br.BranchClass;
                    }
                    else
                        menuList = GetMenuList(br);

                    if (string.IsNullOrEmpty(item.ClassType))
                        {
                            if (item.PointType == br.BranchType)
                            {
                                MenuItem mi = new MenuItem();
                                mi.Name = item.Caption;
                                mi.MethodName = item.Caption;
                                mi.Text = item.Caption;
                                mi.ObjectType = item.ObjectType;
                                mi.BranchClass = item.ClassType;
                                mi.PointType = item.PointType;
                                mi.ClassDefinition = cls;
                                mi.MethodAttribute = item.CommandAttr;
                                mi.imagename = item.iconimage;
                                menuList.Items.Add(mi);
                            }
                        }
                        else
                        {
                            if ((item.PointType == br.BranchType) && (br.BranchClass.Equals(item.ClassType, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                MenuItem mi = new MenuItem();
                                mi.Name = item.Caption;
                                mi.MethodName = item.Name;
                                mi.Text = item.Caption;
                                mi.ObjectType = item.ObjectType;
                                mi.BranchClass = item.ClassType;
                                mi.PointType = item.PointType;
                                mi.ClassDefinition = cls;
                                mi.imagename = item.iconimage;
                                mi.MethodAttribute = item.CommandAttr;
                                menuList.Items.Add(mi);
                            }
                        }
                    
                }
            }
            catch (Exception ex)
            {
                string mes = $"Could not add method from Extension {item.Name} to menu ";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }
        public static void RunMethodFromExtension(IBranch br, AssemblyClassDefinition assemblydef, string MethodName)
        {
            if (br != null)
            {
                DMEEditor.Passedarguments.ObjectName = br.BranchText;
                DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
                DMEEditor.Passedarguments.Id = br.BranchID;
                DMEEditor.Passedarguments.ParameterInt1 = br.BranchID;

                IFunctionExtension fc = (IFunctionExtension)DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { DMEEditor, br });
                Type t = fc.GetType();
                //dynamic fc = Activator.CreateInstance(assemblydef.type, new object[] { DMEEditor, Vismanager, this });
                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
                MethodInfo method = null;
                MethodsClass methodsClass;
                if (!IsMethodApplicabletoNode(cls, br)) return;
                try
                {
                    if (br.BranchType != TheTechIdea.Beep.Vis.EnumPointType.Global)
                    {
                        methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();
                    }
                    else
                    {
                        methodsClass = cls.Methods.Where(x => x.Caption == MethodName && x.PointType == br.BranchType).FirstOrDefault();
                    }

                }
                catch (Exception)
                {

                    methodsClass = null;
                }
                if (methodsClass != null)
                {
                    method = methodsClass.Info;
                    if (method.GetParameters().Length > 0 && DMEEditor.Passedarguments.Objects.Count > 0)
                    {
                        method.Invoke(fc, new object[] { DMEEditor.Passedarguments });
                    }
                    else
                        method.Invoke(fc, null);
                }
            }


        }
        public static void FillMethods(IVisManager vis, List<IBranch> branches)
        {
            vis.PostShowItem += Vis_PostShowItem;
            MenuList m = new MenuList();
            foreach (var br in branches)
            {
                m = GetMenuList(br);

                List<MenuItem> list = new List<MenuItem>();
                list.AddRange(m.Items);
                foreach (MenuItem item in list)
                {
                    try
                    {
                        MethodsClass methodsClass;
                        try
                        {
                            methodsClass = item.ClassDefinition.Methods.Where(x => x.Caption == item.MethodName).FirstOrDefault();
                        }
                        catch (Exception)
                        {
                            methodsClass = null;
                        }
                        uri = null;
                        passedArgs = null;
                        //if (methodsClass != null)
                        //{
                        //    MethodInfo method = methodsClass.Info;
                        //    if (method.GetParameters().Length > 0)
                        //    {
                        //        if (DMEEditor.Passedarguments.Objects.Count > 0)
                        //        {
                        //            if (DMEEditor.Passedarguments.Objects[0] != null)
                        //            {
                        //                method.Invoke(br, new object[] { DMEEditor.Passedarguments.Objects[0].obj });
                        //            }
                        //        }
                              
                                    
                        //    }
                        //    else
                        //        method.Invoke(br, null);
                        //}
                        item.Uri = uri;
                        item.Parameters = passedArgs;
                        UpdateMenuItems(m, item);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }





            vis.PostShowItem -= Vis_PostShowItem;
        }
        public static List<MenuItem> GetMenuItem(IVisManager vis, IBranch br)
        {
            vis.IsinCaptureMenuMode = true;
            vis.PostShowItem += Vis_PostShowItem;
            MenuList m = new MenuList();

            m = GetMenuList(br);

            List<MenuItem> list = new List<MenuItem>();
            if (m != null)
            {
                list.AddRange(m.Items);
                foreach (MenuItem item in list)
                {
                    try
                    {
                        MethodsClass methodsClass;
                        try
                        {
                            methodsClass = item.ClassDefinition.Methods.Where(x => x.Caption == item.MethodName).FirstOrDefault();
                        }
                        catch (Exception)
                        {
                            methodsClass = null;
                        }
                        uri = null;
                        passedArgs = null;
                        if (methodsClass != null)
                        {
                            MethodInfo method = methodsClass.Info;
                            if (method.GetParameters().Length > 0)
                            {
                                if(DMEEditor.Passedarguments.Objects.Count>0)
                                {
                                    if (DMEEditor.Passedarguments.Objects[0] != null)
                                    {
                                        method.Invoke(br, new object[] { DMEEditor.Passedarguments.Objects[0].obj });
                                    }
                                }
                              
                            }
                            else
                                method.Invoke(br, null);
                        }
                        item.Uri = uri;
                        item.Parameters = passedArgs;
                        UpdateMenuItems(m, item);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }

            vis.IsinCaptureMenuMode = false;
            vis.PostShowItem -= Vis_PostShowItem;
            return list;
        }
        private static void Vis_PostShowItem(object sender, IPassedArgs e)
        {
            passedArgs = e;
            //IBranch br = (IBranch)e.Objects[0].obj;
            uri = e.AddinName;
        }
        public static IErrorsInfo RunMethodFromBranch(IBranch branch, string MethodName)
        {

            try
            {
                Type t = branch.GetType();
                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == t.Name).FirstOrDefault();
                MethodInfo method = null;
                MethodsClass methodsClass;
                try
                {
                    methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();
                }
                catch (Exception)
                {

                    methodsClass = null;
                }
                if (methodsClass != null)
                {
                    if (!IsMethodApplicabletoNode(cls, branch)) return DMEEditor.ErrorObject;
                    method = methodsClass.Info;
                    if (method.GetParameters().Length > 0 && DMEEditor.Passedarguments.Objects.Count>0)
                    {
                        method.Invoke(branch, new object[] { DMEEditor.Passedarguments.Objects[0].obj });
                    }
                    else
                        method.Invoke(branch, null);


                    //  DMEEditor.AddLogMessage("Success", "Running method", DateTime.Now, 0, null, Errors.Ok);
                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Run Method " + MethodName;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public static void Nodemenu_ItemClicked(IBranch br, AssemblyClassDefinition cls, string MethodName)
        {
            if (cls != null)
            {
                if (!IsMethodApplicabletoNode(cls, br)) return;
                if (cls.componentType == "IFunctionExtension")
                {
                    RunMethodFromExtension(br, cls, MethodName);

                }
                else
                {

                    RunMethodFromBranch(br, MethodName);
                };

            }
        }
        public static Tuple<MenuList, bool> Nodemenu_MouseClick(IBranch br, BeepMouseEventArgs e)
        {

            MenuList menuList = null;
            bool runmethod = false;
            if (br != null)
            {
                string clicks = "";
                if (e.Button == BeepMouseEventArgs.BeepMouseButtons.Right)
                {
                    if (IsMenuCreated(br))
                    {
                        menuList = GetMenuList(br);

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
                        if (!IsMethodApplicabletoNode(cls, br)) runmethod = true;
                        RunMethodFromBranch(br, clicks);

                    }
                }

            }
            return new Tuple<MenuList, bool>(menuList, runmethod);
        }
        public static bool IsMenuCreated(IBranch br)
        {
            if (br.ObjectType != null)
            {
                return Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                && p.PointType == br.BranchType).Any();
            }
            return
                false;




        }
        public static void UpdateMenuItems(MenuList menu, MenuItem menuItem)
        {
            int menuidx = Menus.FindIndex(p => p.ObjectType != null && p.BranchClass.Equals(menu.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                                      && p.ObjectType.Equals(menu.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                                                                               && p.PointType == menu.PointType);
            if (menuidx > -1)
            {
                int menuitesidx = Menus[menuidx].Items.FindIndex(p => p.Name.Equals(menuItem.Name, StringComparison.InvariantCultureIgnoreCase));
                if (menuitesidx > -1)
                {
                    Menus[menuidx].Items[menuitesidx] = menuItem;
                }

            }
        }
        public static void UpdateMenuList(MenuList menu)
        {
            int menuidx = Menus.FindIndex(p => p.ObjectType != null && p.BranchClass.Equals(menu.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                           && p.ObjectType.Equals(menu.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                                          && p.PointType == menu.PointType);

            if (menuidx > -1)
            {
                Menus[menuidx] = menu;
            }
        }
        public static MenuList GetMenuList(IBranch br)
        {

            return Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                && p.PointType == br.BranchType).FirstOrDefault();
        }
        public static List<MenuItem> GetMenuItemsList(IBranch br)
        {
            List<MenuItem> retval = new List<MenuItem>();
            var ls = Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                && p.PointType == br.BranchType).FirstOrDefault();
            if (ls != null)
            {
                retval = ls.Items;
            }

            return retval;
        }
        public static IErrorsInfo CreateGlobalMenu(IBranch br)
        {
            try
            {
                MenuList menuList = new MenuList();
                if (!IsMenuCreated(br))
                {
                    menuList = new MenuList(br.ObjectType, br.BranchClass, br.BranchType);
                    menuList.branchname = br.BranchText;
                    Menus.Add(menuList);
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
                                    MenuItem mi = new MenuItem();
                                    mi.Name = item.Caption;
                                    mi.MethodName = item.Caption;
                                    mi.Text = item.Caption;
                                    mi.ObjectType = item.ObjectType;
                                    mi.BranchClass = item.ClassType;
                                    mi.PointType = item.PointType;
                                    mi.ClassDefinition = cls;
                                    mi.MethodAttribute = item.CommandAttr;
                                    mi.imagename = item.iconimage;
                                    menuList.Items.Add(mi);
                                }
                            }
                            else
                            {
                                if ((item.PointType == br.BranchType) && (br.BranchClass.Equals(item.ClassType, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    MenuItem mi = new MenuItem();
                                    mi.Name = item.Caption;
                                    mi.MethodName = item.Name;
                                    mi.Text = item.Caption;
                                    mi.ObjectType = item.ObjectType;
                                    mi.BranchClass = item.ClassType;
                                    mi.PointType = item.PointType;
                                    mi.ClassDefinition = cls;
                                    mi.imagename = item.iconimage;
                                    mi.MethodAttribute = item.CommandAttr;
                                    menuList.Items.Add(mi);
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
        public static List<IBranch> CreateRootTree(ITree tree, bool IsinCaptureMenuMode =true)
        {
            string packagename = "";
            try
            {
               IsinCaptureMenuMode = true;

                //tree. = new TreeNodeDragandDropHandler(DMEEditor, this);
                //tree.Treebranchhandler = new TreeBranchHandler(DMEEditor, this);
                tree.Branches = new List<IBranch>();
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
                                    int id = tree.SeqID;
                                    Genrebr.Name = GenreBrAssembly.PackageName;
                                    packagename = GenreBrAssembly.PackageName;
                                    Genrebr.ID = id;
                                    Genrebr.BranchID = id;
                                    Genrebr.BranchText = GenreBrAssembly.classProperties.Caption;
                                    Genrebr.DMEEditor = DMEEditor;
                                    Genrebr.TreeEditor = tree;
                                    if (tree.TreeType != null)
                                    {
                                        if (GenreBrAssembly.classProperties.ObjectType != null)
                                        {
                                            if (GenreBrAssembly.classProperties.ObjectType.Equals(tree.TreeType, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                CreateNode(tree, id, Genrebr);
                                            }
                                        }
                                    }
                                    //else CreateNode(id, Genrebr);

                                    tree.GenerBranchs.Add(new Tuple<IBranch, string>(Genrebr, GenreBrAssembly.classProperties.menu));
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
                                int id = tree.SeqID;
                                br.Name = cls.PackageName;
                                packagename = cls.PackageName;
                                br.ID = id;
                                br.BranchID = id;
                                br.BranchText = cls.classProperties.Caption;
                                br.DMEEditor = DMEEditor;
                                br.TreeEditor = tree;
                                br.Visutil = tree.VisManager;
                                br.TreeEditor = tree;
                                if (tree.TreeType != null)
                                {
                                    if (cls.classProperties.ObjectType != null)
                                    {
                                        if (cls.classProperties.ObjectType.Equals(tree.TreeType, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            var tr = tree.GenerBranchs.FirstOrDefault(p => p.Item2.Equals(cls.classProperties.menu, StringComparison.OrdinalIgnoreCase));
                                            if (tr != null)
                                            {
                                                Genrebr = tr.Item1;
                                            }
                                            else
                                                Genrebr = null;
                                            if (Genrebr != null)
                                            {
                                                Genrebr.ChildBranchs.Add(br);
                                                if (br.ObjectType != null && br.BranchClass != null)
                                                {
                                                    // Console.WriteLine($"{CreateNode}- br.BranchText");
                                                    CreateMenuMethods(br);
                                                    CreateGlobalMenu(br);

                                                }
                                                br.CreateChildNodes();

                                            }
                                            else
                                            {
                                                br.ParentBranch = null;
                                                br.ParentBranchID = -1;
                                                br.ParentGuidID =string.Empty;
                                                CreateNode(tree, id, br);
                                                br.CreateChildNodes();
                                            }
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
            if (tree.VisManager != null)
            {
                FillMethods(tree.VisManager, tree.Branches);
            }
            
            IsinCaptureMenuMode = false;
            return tree.Branches;
        }
        public static List<IBranch> CreateRootTree(bool IsinCaptureMenuMode = true)
        {
            string packagename = "";
            try
            {
                IsinCaptureMenuMode = true;
                int SeqID = 0;
                //tree. = new TreeNodeDragandDropHandler(DMEEditor, this);
                //tree.Treebranchhandler = new TreeBranchHandler(DMEEditor, this);
                Branches = new List<IBranch>();
                GenerBranchs = new List<Tuple<IBranch, string>>();
                IBranch Genrebr = null;
                // AssemblyClassDefinition GenreBrAssembly = DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).FirstOrDefault()!;
                foreach (AssemblyClassDefinition GenreBrAssembly in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).OrderBy(x => x.Order))
                {
                    SeqID++;
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
                                    int id =SeqID;
                                    Genrebr.Name = GenreBrAssembly.PackageName;
                                    packagename = GenreBrAssembly.PackageName;
                                    Genrebr.ID = id;
                                    Genrebr.BranchID = id;
                                    Genrebr.BranchText = GenreBrAssembly.classProperties.Caption;
                                    Genrebr.DMEEditor = DMEEditor;
                                    CreateNode( id, Genrebr);
                                    //else CreateNode(id, Genrebr);
                                    Genrebr.MiscStringID = GenreBrAssembly.GuidID;
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
                    SeqID++;
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
                                br.BranchText = cls.classProperties.Caption;
                                br.DMEEditor = DMEEditor;
                                Genrebr.MiscStringID = cls.GuidID;
                                if (cls.classProperties.ObjectType != null)
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
                                                Genrebr.ChildBranchs.Add(br);
                                                if (br.ObjectType != null && br.BranchClass != null)
                                                {
                                                    // Console.WriteLine($"{CreateNode}- br.BranchText");
                                                    CreateMenuMethods(br);
                                                    CreateGlobalMenu(br);

                                                }
                                                br.CreateChildNodes();

                                            }
                                            else
                                            {
                                                br.ParentBranch = null;
                                                br.ParentBranchID = -1;
                                                br.ParentGuidID = string.Empty;
                                                CreateNode( id, br);
                                                br.CreateChildNodes();
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
       
            IsinCaptureMenuMode = false;
            return Branches;
        }
        public static IBranch CreateNode(ITree tree, int id, IBranch br)
        {
            try
            {
                br.TreeEditor = tree;
                br.Visutil = tree.VisManager;
                br.BranchID = id;
                br.ID = id;
                //n.ContextMenuStrip = 
                DMEEditor.AddLogMessage($"CreateNode -{br.BranchText}");
                //CreateMenuMethods(br);
                if (br.ObjectType != null && br.BranchClass != null)
                {
                    // Console.WriteLine($"{CreateNode}- br.BranchText");
                    CreateMenuMethods(br);
                    CreateGlobalMenu(br);

                }

                br.DMEEditor = DMEEditor;
                if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
                {
                    DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
                }
                tree.Branches.Add(br);
                return br;
            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.AddLogMessage("Error", $"Creating Branch Node {br.BranchText} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                return null;
            }


        }
        public static IBranch CreateNode( int id, IBranch br)
        {
            try
            {
               
                br.BranchID = id;
                br.ID = id;
                //n.ContextMenuStrip = 
                DMEEditor.AddLogMessage($"CreateNode -{br.BranchText}");
                //CreateMenuMethods(br);
                if (br.ObjectType != null && br.BranchClass != null)
                {
                    // Console.WriteLine($"{CreateNode}- br.BranchText");
                    CreateMenuMethods(br);
                    CreateGlobalMenu(br);

                }

                br.DMEEditor = DMEEditor;
                if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
                {
                    DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
                }
                Branches.Add(br);
                return br;
            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.AddLogMessage("Error", $"Creating Branch Node {br.BranchText} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                return null;
            }


        }
        
    }
}
