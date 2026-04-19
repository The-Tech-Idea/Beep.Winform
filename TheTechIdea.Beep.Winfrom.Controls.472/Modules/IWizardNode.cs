using System;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.Collections.Generic;


namespace TheTechIdea.Beep.Vis.Modules.Wizards
{
    /// <summary>
    /// Represents a single step in the wizard: the indicator, the page, and any per-step actions.
    /// </summary>
    public interface IWizardNode
    {
        /// <summary>Zero-based step index.</summary>
        int Index { get; set; }
        /// <summary>Logical name displayed on the indicator.</summary>
        string Name { get; set; }
        /// <summary>The UserControl (or any Control) to host for this step.</summary>
        IDM_Addin Page { get; set; }
        /// <summary>The circular/tab-style button in the step bar.</summary>
        IBeepUIComponent Indicator { get; set; } // IBeepUIComponent
        /// <summary>Optional extra buttons shown for this step (e.g. Help, Reset).</summary>
        IList<IBeepUIComponent> ActionButtons { get; } // use IBeepUIComponent 

        /// <summary>Can navigate forward from this step?</summary>
        bool CanMoveNext { get; }
        /// <summary>Can navigate backward from this step?</summary>
        bool CanMovePrevious { get; }
        /// <summary>Can finish the wizard on this step?</summary>
        bool CanFinish { get; }
        /// <summary>Can cancel the wizard on this step?</summary>
        bool CanCancel { get; }
    }
}