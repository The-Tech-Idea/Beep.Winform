using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TheTechIdea.Beep.Container.Services;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// A sample tool window.
    /// </summary>
    [VisualStudioContribution]
    public class BeepDataToolWindow : ToolWindow
    {
        private readonly BeepDataToolWindowContent content ;
        private readonly IBeepService _beepservice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataToolWindow" /> class.
        /// </summary>
        public BeepDataToolWindow(IBeepService beepService)
        {
            _beepservice = beepService;
            content = new BeepDataToolWindowContent(beepService);
            this.Title = "Beep Data Tool";
        }

        /// <inheritdoc />
        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            // Use this object initializer to set optional parameters for the tool window.
            Placement = ToolWindowPlacement.DocumentWell,
            Toolbar = new ToolWindowToolbar(Toolbar),
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
        [VisualStudioContribution]
        private static ToolbarConfiguration Toolbar => new("%TheTechIdea.Beep.Winform.Toolbar.DisplayName%")
        {
            Children = [ToolbarChild.Command<BeepDataToolToolBarCommand>()],
        };
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                content.Dispose();

            base.Dispose(disposing);
        }
    }
}
