using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules.Wizards;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    /// <summary>
    /// Manages the collection of wizard steps and navigation logic.
    /// </summary>
    public interface IWizardManager
    {
        /// <summary>Title displayed at top of wizard (optional).</summary>
        string Title { get; set; }
        /// <summary>Optional description shown below the title.</summary>
        string Description { get; set; }

        /// <summary>All steps in order.</summary>
        LinkedList<IWizardNode> Nodes { get; set; }
        /// <summary>Number of steps.</summary>
        int Count { get; }
        /// <summary>First (entry) step.</summary>
        IWizardNode EntryForm { get; }
        /// <summary>Last (final) step.</summary>
        IWizardNode LastForm { get; }
        /// <summary>Zero-based index of the currently displayed step.</summary>
        int CurrentIdx { get; }

        /// <summary>Initializes the UI (step bar, buttons) after Nodes collection is set.</summary>
        void InitWizardForm();
        /// <summary>Show the wizard starting at the first step.</summary>
        void Show();
        /// <summary>Show a specific step.</summary>
        void Show(IWizardNode node);
        /// <summary>Hide the wizard entirely.</summary>
        void Hide();

        /// <summary>Go to the next step (if allowed).</summary>
        void MoveNext();
        /// <summary>Go to the previous step (if allowed).</summary>
        void MovePrevious();
    }
}