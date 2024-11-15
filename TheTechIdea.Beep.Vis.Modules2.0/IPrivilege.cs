using System;
using System.Collections.Generic;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepPrivilege : IBeepPrivilege
    {
        public string Name { get; set; }

        public string ComponentName { get  ; set  ; }
        public bool IsVisible { get  ; set  ; }
        public bool IsLocked { get  ; set  ; }
        public bool IsEnabled { get  ; set  ; }
        public bool IsDisabled { get  ; set  ; }
        public bool CanSelect { get  ; set  ; }
        public bool CanDelete { get  ; set  ; }
        public bool CanEdit { get  ; set  ; }
        public bool CanInsert { get  ; set  ; }
    }
    public interface IBeepPrivilege
    {
        string Name { get; set; }
        string ComponentName { get; set; }
        bool IsVisible { get; set; }
        bool IsLocked { get; set; }
        bool IsEnabled { get; set; }
        bool IsDisabled { get; set; }
        bool CanSelect { get; set; }
        bool CanDelete { get; set; }
        bool CanEdit { get; set; }
        bool CanInsert { get; set; }


    }
}
