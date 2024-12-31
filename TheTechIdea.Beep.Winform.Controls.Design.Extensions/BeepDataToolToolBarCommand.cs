using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// BeepDataToolToolBarCommand handler.
    /// </summary>
    [VisualStudioContribution]
    internal class BeepDataToolToolBarCommand : Command
    {
        private readonly IBeepService _beepservice;
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataToolToolBarCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public BeepDataToolToolBarCommand(TraceSource traceSource,IBeepService beepService)
        {
            _beepservice = beepService;
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            this.logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new(displayName: "%TheTechIdea.Beep.Winform.Controls.Design.Extensions.BeepDataToolToolBarCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. To localize the displayName, add an entry in .vsextension\string-resources.json
            // and reference it here by passing "%TheTechIdea.Beep.Winform.Controls.Design.Extensions.BeepDataToolToolBarCommand.DisplayName%" as a constructor parameter.
            Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        };

        /// <inheritdoc />
        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.
            return base.InitializeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            await this.Extensibility.Shell().ShowPromptAsync("Hello from an extension!", PromptOptions.OK, cancellationToken);
        }
       
    }
}
