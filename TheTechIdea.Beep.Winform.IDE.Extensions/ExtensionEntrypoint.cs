using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using TheTechIdea.Beep.Desktop.Common;
namespace TheTechIdea.Beep.Winform.IDE.Extensions
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        /// <inheritdoc/>
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "TheTechIdea.Beep.Winform.IDE.Extensions.07e039f1-55fd-425d-90d0-fe25b37967b3",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Publisher name",
                    displayName: "TheTechIdea.Beep.Winform.IDE.Extensions",
                    description: "Extension description"),
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);

            BeepDesktopServices.RegisterDesktopServices(serviceCollection, new Action<DesktopServiceOptions>(Action =>
            {
                // Configure desktop services options here if needed
                Action.BeepDirectory = "BeepWinformIDE";
                Action.EnableViewDiscovery = true;
                Action.ConfigType = Utilities.BeepConfigType.Application;
                Action.ContainerName="BeepWinformIDEContainer";
                Action.EnableAssemblyLoading = true;
                Action.EnableViewDiscovery = true;
                
            }));

            // You can configure dependency injection here by adding services to the serviceCollection.
        }
    }
}
