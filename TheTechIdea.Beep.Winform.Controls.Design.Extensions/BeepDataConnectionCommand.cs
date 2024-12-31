﻿using Microsoft;
using Microsoft.ServiceHub.Framework;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query;
using System.Diagnostics;
using TheTechIdea.Beep.Container.Services;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// BeepDataConnectionCommand handler.
    /// </summary>
    [VisualStudioContribution]
    internal class BeepDataConnectionCommand : Command
    {
        private readonly IBeepService _beepservices;
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataConnectionCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public BeepDataConnectionCommand(TraceSource traceSource,IBeepService beepService)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            _beepservices= beepService;
            this.logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%TheTechIdea.Beep.Winform.Controls.Design.Extensions.BeepDataConnectionCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
            Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
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
            // Use ExecuteCommandAsync to perform the command's action.
            var aciveproject = context.GetActiveProjectAsync(cancellationToken);
            IServiceBroker serviceBroker = context.Extensibility.ServiceBroker;
            
            ProjectQueryableSpace workspace = new ProjectQueryableSpace(serviceBroker: serviceBroker, joinableTaskContext: null);
            
            await this.Extensibility.Shell().ShowPromptAsync("Hello from an extension!", PromptOptions.OK, cancellationToken);
        }
    }
}
