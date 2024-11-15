using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class WizardState : IWizardState
    {
        public WizardState(IWizardManager pwizardManager)
        {
            wizardManager = pwizardManager;
        }
        public PassedArgs args { get; set; }
        public IWizardManager wizardManager { get; set; }

    }
}
