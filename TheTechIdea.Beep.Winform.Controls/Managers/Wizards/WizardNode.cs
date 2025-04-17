using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Vis.Modules.Wizards
{
    /// <summary>
    /// Default implementation of <see cref="IWizardNode"/>.
    /// </summary>
    public class WizardNode : IWizardNode
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public IDM_Addin Page { get; set; }
        public IBeepUIComponent Indicator { get; set; }
        public IList<IBeepUIComponent> ActionButtons { get; } = new List<IBeepUIComponent>();

        public bool CanMoveNext { get; set; }
        public bool CanMovePrevious { get; set; }
        public bool CanFinish { get; set; }
        public bool CanCancel { get; set; }

        /// <summary>
        /// Creates a new wizard node.
        /// </summary>
        /// <param name="page">The add-in or control to host as the content.</param>
        /// <param name="name">The display name for the step.</param>
        /// <param name="indicator">The themed component used as step indicator.</param>
        public WizardNode(IDM_Addin page, string name, IBeepUIComponent indicator)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Indicator = indicator ?? throw new ArgumentNullException(nameof(indicator));

            // default navigation
            CanMovePrevious = true;
            CanMoveNext = true;
            CanFinish = false;
            CanCancel = true;
        }

        /// <summary>
        /// Adds an action button to this step.
        /// </summary>
        public void AddActionButton(IBeepUIComponent button)
        {
            if (button == null) throw new ArgumentNullException(nameof(button));
            ActionButtons.Add(button);
        }
    }
}
