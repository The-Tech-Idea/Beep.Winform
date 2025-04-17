using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules.Wizards;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IWizardManager
    {
        string Title { get; set; }
        string Description { get; set; }
        int Count { get; }
        IWizardNode entryform { get; }
        LinkedList<IWizardNode> Nodes { get; set; }
        int SelectedIndex { get; }
        IWizardNode lastform { get; }
             int StartLeft { get; set; }
         int StartTop { get; set; } 
        bool Isloaded { get; set; }
        bool IsSaved { get; set; }  
        bool IsEdited { get; set; }
        bool IsEditing { get; set; }
        bool IsHidden { get; set; }
        bool IsVisible { get; set; }

         int CurrentIdx { get;  }
        void MoveNext();
        void MovePrevious();
        void InitWizardForm();
        void Show();
        void Hide();
        void Show(IWizardNode node);
        WizardState State { get; set; }



    }
}