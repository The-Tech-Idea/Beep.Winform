using System;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Central coordinator for docking/group topology mutations.
    /// Ensures mutation execution order is consistent and post-mutation
    /// tree validation/repair hooks always run.
    /// </summary>
    internal sealed class DocumentHostTreeMutationCoordinator
    {
        private readonly BeepDocumentHost _host;

        public DocumentHostTreeMutationCoordinator(BeepDocumentHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public bool Execute(string operationName, Action mutationAction)
        {
            DocumentHostOperationNames.EnsureKnown(operationName);
            ArgumentNullException.ThrowIfNull(mutationAction);

            DocumentHostLayoutTransaction? tx = _host.EnableTransactionalDocking
                ? new DocumentHostLayoutTransaction(_host)
                : null;

            try
            {
                mutationAction();
                _host.ValidateAndRepairLayoutTree(operationName);
                tx?.Complete();

                if (tx == null)
                    _host.RecalculateLayout();

                return true;
            }
            finally
            {
                tx?.Dispose();
            }
        }
    }
}
