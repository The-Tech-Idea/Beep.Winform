using System;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Lightweight transaction scope for layout and docking mutations.
    /// Suspends layout while mutating and restores it deterministically.
    /// </summary>
    internal sealed class DocumentHostLayoutTransaction : IDisposable
    {
        private readonly BeepDocumentHost _host;
        private readonly bool _wasSuspended;
        private bool _completed;

        public DocumentHostLayoutTransaction(BeepDocumentHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _wasSuspended = host.GetLayoutSuspended();
            host.SetLayoutSuspended(true);
        }

        public void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            _host.SetLayoutSuspended(_wasSuspended);
            if (_completed)
            {
                _host.RecalculateLayout();
            }
        }
    }
}
