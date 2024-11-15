using System;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules.Wizards
{
    public interface IWizardNode
    {
        int Index { get; set; }
        object Data { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
        bool IsInit { get; set; }
        bool IsHidden { get; set; }
        bool IsVisible { get; set; }
        bool IsChecked { get; set; }
        bool IsDisabled { get; set; }

        string Name { get; set; }
        IDM_Addin Addin { get; set; }
        string Type { get; set; }
        IWizardButton Wizardbutton { get; set; }
        event EventHandler<IWizardNode> WizardNodeClickEvent;
        event EventHandler<IWizardNode> WizardNodeEnterEvent;
        event EventHandler<IWizardNode> WizardNodeLeaveEvent;


        void LeaveButton();
        void OverButton();
    }
}