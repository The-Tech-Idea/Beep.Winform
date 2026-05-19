using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        private int _viewDocumentTransferDepth;

        private void PrepareViewForManagerTransition(BeepDocumentHost? host)
        {
            if (host == null || IsDesignTimeComponent)
                return;

            _viewDocumentTransferDepth++;
            try
            {
                PrepareHostedContentForViewSwitch(host);
                CaptureManagerOwnedDocumentContentForTransfer(host);
                RemoveManagerOwnedDocumentsFromHost(host);
            }
            finally
            {
                _viewDocumentTransferDepth--;
            }
        }

        private void RemoveManagerOwnedDocumentsFromHost(BeepDocumentHost host)
        {
            foreach (var documentId in EnumerateManagerOwnedDocumentIds())
            {
                try { host.CloseDocument(documentId); } catch { }
            }
        }

        private IEnumerable<string> EnumerateManagerOwnedDocumentIds()
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var descriptor in _runtimeDocuments)
            {
                if (!string.IsNullOrWhiteSpace(descriptor.Id))
                    ids.Add(descriptor.Id);
            }

            foreach (var descriptor in _designTimeDocuments)
            {
                if (!string.IsNullOrWhiteSpace(descriptor.Id))
                    ids.Add(descriptor.Id);
            }

            return ids;
        }
    }
}