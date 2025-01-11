using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class DynamicFunctionCallingManager
    {
        public static IDMEEditor DMEEditor { get; set; }
        public static void RunFunctionFromExtensions(this ITree tree, SimpleItem item, string MethodName)
        {
            AssemblyClassDefinitionManager.DMEEditor = tree.DMEEditor;
            IBranch br = null;
            AssemblyClassDefinition assemblydef = new AssemblyClassDefinition();
            MethodInfo method = null;
            MethodsClass methodsClass;
            assemblydef = AssemblyClassDefinitionManager.GetAssemblyClassDefinitionByGuid(item.AssemblyClassDefinitionID);
            dynamic fc = tree.DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { tree.DMEEditor, tree.VisManager, tree });
            //  dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.type.ToString(), new object[] { DMEEditor, Vismanager, this });
            if (fc == null)
            {
                return;
            }

            Type t = ((IFunctionExtension)fc).GetType();
            //   AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();

            methodsClass = assemblydef.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();

            if (tree.DMEEditor.Passedarguments == null)
            {
                tree.DMEEditor.Passedarguments = new PassedArgs();
            }
            if (br != null)
            {
                tree.DMEEditor.Passedarguments.ObjectName = br.BranchText;
                tree.DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
                tree.DMEEditor.Passedarguments.Id = br.BranchID;
                tree.DMEEditor.Passedarguments.ParameterInt1 = br.BranchID;
                if (!IsMethodApplicabletoNode(assemblydef, br)) return;

            }

            //if (methodsClass != null)
            //{
            //    PassedArgs args = new PassedArgs();
            //    ErrorsInfo ErrorsandMesseges = new ErrorsInfo();
            //    args.Cancel = false;
            //    tree.PreCallModule?.Invoke(tree, args);

            //    if (args.Cancel)
            //    {
            //        tree.DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
            //        ErrorsandMesseges.Flag = Errors.Failed;
            //        ErrorsandMesseges.Message = $"Function Access Denied";
            //        return;
            //    }

            //}
            method = methodsClass.Info;
            if (method.GetParameters().Length > 0)
            {
                method.Invoke(fc, new object[] { tree.DMEEditor.Passedarguments });
            }
            else
                method.Invoke(fc, null);
        }
        private static bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
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
        public static IErrorsInfo RunMethodFromObject(this ITree tree, object branch, string MethodName)
        {
            try
            {
                Type t = branch.GetType();
                AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == t.Name).FirstOrDefault();
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
                    if (!IsMethodApplicabletoNode(cls, (IBranch)branch)) return tree.DMEEditor.ErrorObject;
                    //PassedArgs args = new PassedArgs();
                    //args.ObjectName = MethodName;
                    //args.ObjectType = methodsClass.ObjectType;
                    //args.Cancel = false;
                    //PreCallModule?.Invoke(this, args);
                    //if (args.Cancel)
                    //{
                    //    DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
                    //    ErrorsandMesseges.Flag = Errors.Failed;
                    //    ErrorsandMesseges.Message = $"Function Access Denied";
                    //    return ErrorsandMesseges;
                    //}

                    method = methodsClass.Info;
                    if (method.GetParameters().Length > 0)
                    {
                        method.Invoke(branch, new object[] { tree.DMEEditor.Passedarguments.Objects[0].obj });
                    }
                    else
                        method.Invoke(branch, null);


                    //  DMEEditor.AddLogMessage("Success", "Running method", DateTime.Now, 0, null, Errors.Ok);
                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Run Method " + MethodName;
                tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return tree.DMEEditor.ErrorObject;
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


    }
}
