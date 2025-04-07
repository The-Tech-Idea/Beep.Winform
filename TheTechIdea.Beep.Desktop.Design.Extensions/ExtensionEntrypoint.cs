using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Extensibility;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Container;

namespace TheTechIdea.Beep.Desktop.Design.Extensions
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
                    id: "TheTechIdea.Beep.Desktop.Design.Extensions.c7597d06-22b1-4cc6-b977-ceedc7b9df9a",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "The Tech Idea",
                    displayName: "TheTechIdea.Beep.Desktop.Design.Extensions",
                    description: "Visual Studio Extension for Beep FrameWork"),
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterBeep(AppContext.BaseDirectory, null, BeepConfigType.Application, true);
            serviceCollection.RegisterRouter();
            serviceCollection.RegisterKeyHandler();
            serviceCollection.RegisterViewModels();
            serviceCollection.RegisterViews();
            serviceCollection.RegisterAppManager();
            serviceCollection.RegisterControlManager();
            base.InitializeServices(serviceCollection);
            // Retreiving Services and Configuring them
          
            // You can configure dependency injection here by adding services to the serviceCollection.
        }
    }
}
