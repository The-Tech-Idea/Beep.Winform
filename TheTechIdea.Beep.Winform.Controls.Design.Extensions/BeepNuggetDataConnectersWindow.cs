using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// A sample tool window.
    /// </summary>
    [VisualStudioContribution]
    public class BeepNuggetDataConnectersWindow : ToolWindow
    {
        private readonly BeepNuggetDataConnectersWindowContent content ;
        private readonly IBeepService _beepservice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepNuggetDataConnectersWindow" /> class.
        /// </summary>
        public BeepNuggetDataConnectersWindow(IBeepService beepService)
        {
            _beepservice= beepService;
            content = new BeepNuggetDataConnectersWindowContent(_beepservice);
            this.Title = "Beep Data Connecters Nugget Management";
        }

        /// <inheritdoc />
        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            // Use this object initializer to set optional parameters for the tool window.
            Placement = ToolWindowPlacement.Floating,
        };

        /// <inheritdoc />
        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IRemoteUserControl>(content);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                content.Dispose();

            base.Dispose(disposing);
        }
    }
}
