using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        private sealed class PendingRuntimeDocumentAdd
        {
            public bool Activate { get; init; }
        }

        private readonly List<DocumentDescriptor> _runtimeDocuments = new();
        private readonly List<PendingRuntimeDocumentAdd> _pendingRuntimeDocumentAdds = new();
        private int _runtimeDocumentReplayDepth;
        private string? _preferredActiveRuntimeDocumentId;

        private BeepDocumentPanel? AddDocumentCore(
            string title,
            string? iconPath,
            bool activate,
            bool trackForViewSwitch)
        {
            if (_view == null)
                return null;

            PendingRuntimeDocumentAdd? pending = null;
            if (trackForViewSwitch && !IsDesignTimeComponent)
            {
                pending = new PendingRuntimeDocumentAdd { Activate = activate };
                _pendingRuntimeDocumentAdds.Add(pending);
            }

            try
            {
                return _view.AddDocument(title, iconPath ?? string.Empty, activate);
            }
            finally
            {
                if (pending != null)
                    _pendingRuntimeDocumentAdds.Remove(pending);
            }
        }

        private void ApplyRuntimeDocuments()
        {
            if (_view == null || IsDesignTimeComponent || _runtimeDocuments.Count == 0)
                return;

            _runtimeDocumentReplayDepth++;
            try
            {
                _view.BeginBatchAddDocuments();
                try
                {
                    DocumentDescriptor? preferredActive = null;
                    foreach (var descriptor in _runtimeDocuments)
                    {
                        if (!string.IsNullOrEmpty(_preferredActiveRuntimeDocumentId)
                            && string.Equals(descriptor.Id, _preferredActiveRuntimeDocumentId, StringComparison.OrdinalIgnoreCase))
                        {
                            preferredActive = descriptor;
                            continue;
                        }

                        _view.AddDocument(CloneRuntimeDescriptor(descriptor));
                    }

                    if (preferredActive != null)
                        _view.AddDocument(CloneRuntimeDescriptor(preferredActive));
                }
                finally
                {
                    _view.EndBatchAddDocuments();
                }
            }
            finally
            {
                _runtimeDocumentReplayDepth--;
            }
        }

        private void TrackManagedRuntimeDocumentAdded(DocumentAddedEventArgs e)
        {
            if (_runtimeDocumentReplayDepth > 0 || IsDesignTimeComponent || e?.Descriptor == null)
                return;

            if (_pendingRuntimeDocumentAdds.Count == 0)
                return;

            UpsertRuntimeDocument(e.Descriptor);

            var pending = _pendingRuntimeDocumentAdds[_pendingRuntimeDocumentAdds.Count - 1];
            if (pending.Activate)
                _preferredActiveRuntimeDocumentId = e.Descriptor.Id;
        }

        private void TrackManagedRuntimeDocumentRemoved(string? documentId)
        {
            if (string.IsNullOrEmpty(documentId))
                return;

            if (!RemoveRuntimeDocument(documentId))
                return;

            if (string.Equals(_preferredActiveRuntimeDocumentId, documentId, StringComparison.OrdinalIgnoreCase))
                _preferredActiveRuntimeDocumentId = _runtimeDocuments.Count > 0
                    ? _runtimeDocuments[_runtimeDocuments.Count - 1].Id
                    : null;
        }

        private void TrackManagedRuntimeDocumentActivation(string? documentId)
        {
            if (string.IsNullOrEmpty(documentId))
                return;

            if (ContainsRuntimeDocument(documentId))
                _preferredActiveRuntimeDocumentId = documentId;
        }

        private void UpsertRuntimeDocument(DocumentDescriptor descriptor)
        {
            var clone = CloneRuntimeDescriptor(descriptor);
            for (int i = 0; i < _runtimeDocuments.Count; i++)
            {
                if (!string.Equals(_runtimeDocuments[i].Id, clone.Id, StringComparison.OrdinalIgnoreCase))
                    continue;

                _runtimeDocuments[i] = clone;
                return;
            }

            _runtimeDocuments.Add(clone);
        }

        private bool RemoveRuntimeDocument(string documentId)
        {
            for (int i = 0; i < _runtimeDocuments.Count; i++)
            {
                if (!string.Equals(_runtimeDocuments[i].Id, documentId, StringComparison.OrdinalIgnoreCase))
                    continue;

                _runtimeDocuments.RemoveAt(i);
                return true;
            }

            return false;
        }

        private bool ContainsRuntimeDocument(string documentId)
        {
            for (int i = 0; i < _runtimeDocuments.Count; i++)
            {
                if (string.Equals(_runtimeDocuments[i].Id, documentId, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static DocumentDescriptor CloneRuntimeDescriptor(DocumentDescriptor source)
        {
            var clone = new DocumentDescriptor
            {
                Id = source.Id,
                PersistenceKey = source.PersistenceKey,
                PreviousGroupId = source.PreviousGroupId,
                Title = source.Title,
                IconPath = source.IconPath,
                IsModified = source.IsModified,
                IsPinned = source.IsPinned,
                CanClose = source.CanClose,
                Category = source.Category,
                TooltipText = source.TooltipText,
                Tag = source.Tag,
                BadgeText = source.BadgeText,
                BadgeColor = source.BadgeColor,
                TabColor = source.TabColor,
                AccentColor = source.AccentColor,
                InitialContent = source.InitialContent
            };

            foreach (var pair in source.CustomData)
                clone.CustomData[pair.Key] = pair.Value;

            return clone;
        }
    }
}