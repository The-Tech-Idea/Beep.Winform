using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Contracts;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Contracts
{
    public interface IBeepBlockView
    {
        string BlockName { get; set; }
        string ManagerBlockName { get; }
        bool IsBound { get; }
        IBeepFormsHost? FormsHost { get; }
        BeepBlockDefinition? Definition { get; set; }
        BeepBlockViewState ViewState { get; }

        void Bind(IBeepFormsHost formsHost);
        void Unbind();
        void ApplyDefinition(BeepBlockDefinition definition);
        void SyncFromManager();
    }
}