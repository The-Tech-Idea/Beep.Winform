using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IHasBeepViewModelClass
    {
        /// <summary>
        /// A string containing the full type name (e.g. "MyNamespace.MyViewModel")
        /// of a class that implements IBeepViewModel.
        /// </summary>
        string BeepViewModelClass { get; }
      
    }
}
