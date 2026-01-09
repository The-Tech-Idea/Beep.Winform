using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Shared action list for container controls
    /// Provides common actions for managing child controls
    /// </summary>
    public class ContainerControlActionList : DesignerActionList
    {
        private readonly BaseBeepControlDesigner _designer;

        public ContainerControlActionList(BaseBeepControlDesigner designer)
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

        /// <summary>
        /// Apply dock layout to children
        /// </summary>
        public void ApplyDockLayout()
        {
            // In a full implementation, this would set Dock property on children
            // For now, just a placeholder
        }

        /// <summary>
        /// Apply flow layout to children
        /// </summary>
        public void ApplyFlowLayout()
        {
            // In a full implementation, this would arrange children in a flow
            // For now, just a placeholder
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Container Actions"));
            items.Add(new DesignerActionMethodItem(this, "ArrangeChildren", "Arrange Children", "Container Actions", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyDockLayout", "Apply Dock Layout", "Container Actions", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyFlowLayout", "Apply Flow Layout", "Container Actions", true));
            items.Add(new DesignerActionMethodItem(this, "ClearAllChildren", "Clear All Children", "Container Actions", true));

            return items;
        }
    }
}
