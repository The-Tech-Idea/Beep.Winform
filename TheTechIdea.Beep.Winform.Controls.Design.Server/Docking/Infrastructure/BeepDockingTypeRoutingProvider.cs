using System.Collections.Generic;
using System.Composition;
using Microsoft.DotNet.DesignTools.TypeRouting;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure
{
    /// <summary>
    /// Registers docking designer types with the Microsoft.DotNet.DesignTools host.
    /// Follows the same pattern as BeepMdiTypeRoutingProvider.
    /// </summary>
    [Shared]
    [ExportTypeRoutingDefinitionProvider]
    internal sealed class BeepDockingTypeRoutingProvider : TypeRoutingDefinitionProvider
    {
        public override IEnumerable<TypeRoutingDefinition> GetDefinitions()
        {
            return new[]
            {
                new TypeRoutingDefinition(TypeRoutingKinds.Designer, nameof(BeepDockingManagerDesigner), typeof(BeepDockingManagerDesigner)),
                new TypeRoutingDefinition(TypeRoutingKinds.Designer, nameof(BeepDockspaceDesigner),      typeof(BeepDockspaceDesigner)),
                new TypeRoutingDefinition(TypeRoutingKinds.Designer, nameof(DockPanelDesigner),          typeof(DockPanelDesigner))
            };
        }
    }
}
