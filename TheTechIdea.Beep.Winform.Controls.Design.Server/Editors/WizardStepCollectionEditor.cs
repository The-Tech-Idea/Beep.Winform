using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Collection editor for WizardStep collection
    /// Provides UI for adding, removing, and reordering wizard steps
    /// </summary>
    /// <remarks>
    /// Note: CollectionEditor is not available in .NET 8+ for design-time assemblies loaded by the designer.
    /// This would need to be implemented as a UITypeEditor instead, or moved to the main controls project.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WizardStepCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            // This would require a custom dialog for editing the collection
            // For now, return the value unchanged
            // TODO: Implement custom collection editing dialog
            return value;
        }
    }
}
