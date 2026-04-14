using System;
using System.ComponentModel;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Shared action list for container controls
    /// Provides the shared container actions that are actually implemented across designers.
    /// </summary>
    public class ContainerControlActionList : DesignerActionList
    {
        private readonly IBeepDesignerActionHost _designer;

        public ContainerControlActionList(IBeepDesignerActionHost designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BaseControl? BeepControl => Component as BaseControl;

        #region Actions

        /// <summary>
        /// Clear all child controls
        /// </summary>
        public void ClearAllChildren()
        {
            var control = BeepControl;
            if (control != null)
            {
                control.Controls.Clear();
            }
        }

        /// <summary>
        /// Arrange child controls using default layout
        /// </summary>
        public void ArrangeChildren()
        {
            var control = BeepControl;
            if (control != null)
            {
                control.PerformLayout();
            }
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Container Actions"));
            items.Add(new DesignerActionMethodItem(this, "ArrangeChildren", "Arrange Children", "Container Actions", true));
            items.Add(new DesignerActionMethodItem(this, "ClearAllChildren", "Clear All Children", "Container Actions", true));

            return items;
        }
    }
}
