using System.Collections.Generic;
using System.Composition;
using Microsoft.DotNet.DesignTools.TypeRouting;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Wizards
{
    /// <summary>
    /// Registers WizardPageDesigner with the Microsoft.DotNet.DesignTools host.
    /// Without this MEF export, the string-based [Designer] attribute on WizardPage
    /// would not resolve in the out-of-process designer host.
    /// </summary>
    [Shared]
    [ExportTypeRoutingDefinitionProvider]
    internal sealed class WizardTypeRoutingProvider : TypeRoutingDefinitionProvider
    {
        public override IEnumerable<TypeRoutingDefinition> GetDefinitions()
        {
            return new[]
            {
                new TypeRoutingDefinition(
                    TypeRoutingKinds.Designer,
                    nameof(WizardPageDesigner),
                    typeof(WizardPageDesigner))
            };
        }
    }
}
