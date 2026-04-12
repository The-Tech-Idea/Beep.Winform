using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor.UOWManager.Interfaces;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Contracts;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Contracts
{
    public interface IBeepFormsHost
    {
        string FormName { get; set; }
        string? ActiveBlockName { get; }
        IUnitofWorksManager? FormsManager { get; set; }
        BeepFormsDefinition? Definition { get; set; }
        BeepFormsViewState ViewState { get; }
        IReadOnlyList<IBeepBlockView> Blocks { get; }

        event EventHandler? ActiveBlockChanged;
        event EventHandler? FormsManagerChanged;
        event EventHandler? ViewStateChanged;

        bool RegisterBlock(IBeepBlockView blockView);
        bool UnregisterBlock(string blockName);
        bool TrySetActiveBlock(string blockName);
        void SyncFromManager();
    }
}