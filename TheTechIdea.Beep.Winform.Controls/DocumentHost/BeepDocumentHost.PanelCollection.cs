// BeepDocumentHost.PanelCollection.cs
// Typed collection of BeepDocumentPanel instances owned by BeepDocumentHost.
//
// Purpose:
//   Provides a serializable vehicle for document panels so the WinForms
//   CodeDom serializer generates Designer.cs entries of the form:
//
//     this.documentPanel1 = new BeepDocumentPanel();
//     this.documentPanel1.DocumentId    = "my-doc-id";
//     this.documentPanel1.DocumentTitle = "My Document";
//     this.beepDocumentHost1.DocumentPanels.Add(this.documentPanel1);
//
//   On form re-open the serialized Add() call re-registers each panel with the
//   host, eliminating the previous "orphan-panel + duplicate" bug that occurred
//   when DesignTimeDocuments descriptors were serialized and ApplyDesignTimeDocuments()
//   tried to create a second panel for the same document id.
//
// Thread safety: design-time only (single-threaded UI thread).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// A <see cref="Collection{T}"/> of <see cref="BeepDocumentPanel"/> instances
    /// that automatically registers each panel with its owning
    /// <see cref="BeepDocumentHost"/> when added, and closes it when removed.
    /// </summary>
    public sealed class BeepDocumentPanelCollection : Collection<BeepDocumentPanel>
    {
        private readonly BeepDocumentHost _host;
        private bool _syncingFromHost;

        internal BeepDocumentPanelCollection(BeepDocumentHost host)
        {
            _host = host;
        }

        /// <summary>
        /// Adds the panel to the collection. If the host does not yet know about
        /// this panel (checked by DocumentId), <see cref="BeepDocumentHost.RegisterDocumentPanel"/>
        /// is called to wire it into the tab strip and content area.
        /// </summary>
        protected override void InsertItem(int index, BeepDocumentPanel panel)
        {
            BeepDocumentPanel? existingPanel = ValidatePanelForInsert(panel);

            base.InsertItem(index, panel);

            // ContainsOpenDocument guards against double-registration when the
            // designer creates a panel via CreateComponent() then calls Add().
            if (existingPanel == null)
            {
                _host.RegisterDocumentPanel(panel, activate: false);
            }
        }

        /// <summary>
        /// Removes the panel from the collection and closes the corresponding
        /// document inside the host.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            var panel = this[index];
            base.RemoveItem(index);
            if (!_syncingFromHost)
                _host.CloseDocument(panel.DocumentId);
        }

        protected override void SetItem(int index, BeepDocumentPanel panel)
        {
            var oldPanel = this[index];
            BeepDocumentPanel? existingPanel = ValidatePanelForInsert(panel, index);
            base.SetItem(index, panel);

            if (_syncingFromHost)
            {
                return;
            }

            if (!ReferenceEquals(oldPanel, panel))
            {
                _host.CloseDocument(oldPanel.DocumentId);
            }

            if (existingPanel == null)
            {
                _host.RegisterDocumentPanel(panel, activate: false);
            }
        }

        protected override void ClearItems()
        {
            var documentIds = this.Select(panel => panel.DocumentId).ToList();
            base.ClearItems();

            if (_syncingFromHost)
            {
                return;
            }

            foreach (string documentId in documentIds)
            {
                _host.CloseDocument(documentId);
            }
        }

        internal void RemoveFromHost(string documentId)
        {
            for (int index = 0; index < Count; index++)
            {
                if (!string.Equals(this[index].DocumentId, documentId, StringComparison.Ordinal))
                {
                    continue;
                }

                _syncingFromHost = true;
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    _syncingFromHost = false;
                }

                break;
            }
        }

        private BeepDocumentPanel? ValidatePanelForInsert(BeepDocumentPanel panel, int ignoredIndex = -1)
        {
            ArgumentNullException.ThrowIfNull(panel);

            if (string.IsNullOrWhiteSpace(panel.DocumentId))
            {
                throw new InvalidOperationException("Document panels must have a DocumentId before being added.");
            }

            for (int index = 0; index < Count; index++)
            {
                if (index == ignoredIndex)
                {
                    continue;
                }

                BeepDocumentPanel existingCollectionPanel = this[index];
                if (ReferenceEquals(existingCollectionPanel, panel))
                {
                    throw new InvalidOperationException("This document panel already belongs to the host.");
                }

                if (string.Equals(existingCollectionPanel.DocumentId, panel.DocumentId, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"Document '{panel.DocumentId}' already belongs to the host.");
                }
            }

            BeepDocumentPanel? existingHostPanel = _host.GetPanel(panel.DocumentId);
            if (existingHostPanel != null && !ReferenceEquals(existingHostPanel, panel))
            {
                throw new InvalidOperationException($"Document '{panel.DocumentId}' already exists.");
            }

            return existingHostPanel;
        }
    }
}
