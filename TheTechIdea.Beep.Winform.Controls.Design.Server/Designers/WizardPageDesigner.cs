using System;
using System.ComponentModel;
using Microsoft.DotNet.DesignTools.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Designer for WizardPage — enables child control drag-drop at design time.
    /// Does NOT use BaseBeepControlDesigner because WizardPage is a container
    /// and uses its own built-in double-buffering.
    /// </summary>
    internal sealed class WizardPageDesigner : ParentControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

        public override void InitializeNewComponent(System.Collections.IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
        }
    }
}
