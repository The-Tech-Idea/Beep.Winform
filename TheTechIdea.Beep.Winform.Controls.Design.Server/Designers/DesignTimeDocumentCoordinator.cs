using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class DesignTimeDocumentCoordinator
    {
        private readonly BeepDocumentHostDesigner _designer;
        private readonly BeepDocumentHost _host;
        private readonly Collection<DocumentDescriptor> _docs;

        internal DesignTimeDocumentCoordinator(
            BeepDocumentHostDesigner designer,
            BeepDocumentHost host,
            Collection<DocumentDescriptor> docs)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _docs = docs ?? throw new ArgumentNullException(nameof(docs));
        }

        internal DocumentDescriptor? AddNewDocument(
            bool activate,
            bool selectSurface,
            string? title = null)
        {
            DocumentDescriptor descriptor = _designer.InternalCreateNextDesignTimeDocumentDescriptor(_host, _docs);
            if (!string.IsNullOrWhiteSpace(title))
            {
                descriptor.Title = title.Trim();
            }

            return AddOrUpdateDocument(descriptor, activate, selectSurface) != null
                ? descriptor
                : null;
        }

        internal static DocumentDescriptor CreateDetachedDescriptor(string id, string title)
            => new DocumentDescriptor
            {
                Id = id,
                Title = title,
                InitialContent = DocumentInitialContent.Empty
            };

        internal BeepDocumentPanel? AddOrUpdateDocument(
            DocumentDescriptor descriptor,
            bool activate,
            bool selectSurface)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            BeepDocumentPanel? panel = _designer.InternalEnsureDesignTimeDocumentOpen(_host, descriptor, activate);
            if (panel == null)
            {
                return null;
            }

            DocumentDescriptor? existing = _designer.InternalFindDesignTimeDocument(_docs, descriptor.Id);
            if (existing == null)
            {
                _docs.Add(_designer.InternalCloneDescriptor(descriptor));
            }
            else
            {
                _designer.InternalCopyDescriptorState(descriptor, existing);
            }

            if (selectSurface)
            {
                _designer.InternalSyncDesignerSelection((object?)panel ?? _host);
            }

            return panel;
        }

        internal bool RemoveDocument(
            string documentId,
            bool selectSurface,
            out DocumentDescriptor? snapshot)
        {
            snapshot = null;
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return false;
            }

            DocumentDescriptor? existing = _designer.InternalFindDesignTimeDocument(_docs, documentId);
            bool hasOpenPanel = _host.GetPanel(documentId) != null;
            if (existing == null && !hasOpenPanel)
            {
                return false;
            }

            snapshot = _designer.InternalCaptureDocumentDescriptor(_host, _docs, documentId);
            if (hasOpenPanel && !_designer.InternalCloseDesignTimeDocument(_host, documentId))
            {
                return false;
            }

            if (existing != null)
            {
                _docs.Remove(existing);
            }

            if (selectSurface)
            {
                _designer.InternalSyncDesignerSelection((object?)_host.ActivePanel ?? _host);
            }

            return existing != null || hasOpenPanel;
        }

        internal void SyncDocuments(IReadOnlyList<DocumentDescriptor> sourceDocuments)
        {
            var sourceById = sourceDocuments
                .Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id))
                .ToDictionary(descriptor => descriptor.Id, StringComparer.OrdinalIgnoreCase);

            foreach (DocumentDescriptor existing in _docs.ToList())
            {
                if (string.IsNullOrWhiteSpace(existing.Id) || sourceById.ContainsKey(existing.Id))
                {
                    continue;
                }

                RemoveDocument(existing.Id, selectSurface: false, out _);
            }

            foreach (DocumentDescriptor descriptor in sourceDocuments)
            {
                if (string.IsNullOrWhiteSpace(descriptor.Id))
                {
                    continue;
                }

                AddOrUpdateDocument(descriptor, activate: false, selectSurface: false);
            }

            DocumentDescriptor? first = sourceDocuments.FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.Id));
            if (string.IsNullOrWhiteSpace(_host.ActiveDocumentId) && first != null)
            {
                _host.SetActiveDocument(first.Id);
            }
        }
    }
}