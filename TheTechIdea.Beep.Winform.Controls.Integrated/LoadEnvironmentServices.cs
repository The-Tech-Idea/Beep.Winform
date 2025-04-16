using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Integrated
{
    public static partial class LoadEnvironmentServices
    {
        public static void LoadServices(this IBeepService service)
        {
            // Load the services you need here
            // For example:
            AssemblyClassDefinitionManager.TreeStructures=service.Config_editor.AddinTreeStructure;
            AssemblyClassDefinitionManager.BranchesClasses = service.Config_editor.BranchesClasses;
            AssemblyClassDefinitionManager.GlobalFunctions.AddRange(service.Config_editor.GlobalFunctions);

        }
    }
}
