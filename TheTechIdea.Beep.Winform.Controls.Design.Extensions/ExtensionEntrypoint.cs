using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using TheTechIdea.Beep.Container;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        /// <inheritdoc/>
        public override ExtensionConfiguration ExtensionConfiguration
        {
            get
            {
                return new()
                {
                    Metadata = new(
                    id: "TheTechIdea.Beep.Winform.Controls.Design.Extensions.4600cee6-258d-4394-90a1-1cb6696aef0a",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Publisher name",
                    displayName: "Beep Winform Extensions",
                    description: "Extension description"),
                };
            }
        }

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
          
            base.InitializeServices(serviceCollection);
            serviceCollection.RegisterBeep(AppContext.BaseDirectory, null, BeepConfigType.DataConnector, true);
            // You can configure dependency injection here by adding services to the serviceCollection.
        }
    }
}
