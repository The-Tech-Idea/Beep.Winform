using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        private int _viewDocumentTransferDepth;

        private void PrepareViewForManagerTransition(IBeepDocumentManagerView view)
        {
            if (view == null || IsDesignTimeComponent)
                return;

            _viewDocumentTransferDepth++;
            try
            {
                PrepareHostedContentForViewSwitch(view);
                CaptureManagerOwnedDocumentContentForTransfer(view);
                RemoveManagerOwnedDocumentsFromView(view);
            }
            finally
            {
                _viewDocumentTransferDepth--;
            }
        }

        private void RemoveManagerOwnedDocumentsFromView(IBeepDocumentManagerView view)
        {
            foreach (var documentId in EnumerateManagerOwnedDocumentIds())
            {
                try { view.DetachDocumentForTransfer(documentId); } catch { }
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