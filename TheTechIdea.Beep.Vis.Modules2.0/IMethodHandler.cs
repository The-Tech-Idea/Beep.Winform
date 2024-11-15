
using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IMethodHandler
    {
        IDMEEditor DMEEditor { get; }
        List<MenuList> Menus { get; set; }
        MenuList GetMenuList(IBranch br);
        bool IsMenuCreated(IBranch br);
        bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br);
        void Nodemenu_ItemClicked(IBranch br, AssemblyClassDefinition cls, string MethodName);
        Tuple<MenuList, bool> Nodemenu_MouseClick(IBranch br, BeepMouseEventArgs e);
        IErrorsInfo RunMethodFromBranch(IBranch branch, string MethodName);
        void RunMethodFromExtension(IBranch br, AssemblyClassDefinition assemblydef, string MethodName);
    }
}