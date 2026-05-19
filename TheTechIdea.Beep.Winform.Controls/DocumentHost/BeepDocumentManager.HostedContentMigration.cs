using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        private readonly HashSet<string> _suppressedHostedAddinRemovals =
            new(StringComparer.OrdinalIgnoreCase);

        private void PrepareHostedContentForViewSwitch(BeepDocumentHost host)
        {
            if (host == null || IsDesignTimeComponent)
                return;

            PrepareAddinsForViewSwitch(host);
            PrepareExtendedControlsForViewSwitch(host);
        }

        private void RehostHostedContentForCurrentView()
        {
            if (_host == null || IsDesignTimeComponent)
                return;

            RehostAddinsForCurrentView();
        }

        private void PrepareAddinsForViewSwitch(BeepDocumentHost host)
        {
            if (_addinEntries.Count == 0)
                return;

            foreach (var pair in _addinEntries.ToList())
            {
                var entry = pair.Value;
                if (!IsAddinHostedInView(entry, host))
                    continue;

                if (!CanRehostAddin(entry))
                    continue;

                DetachHostedAddinSurface(entry);
                SuppressHostedAddinRemoval(entry.DocumentId);
                TryCloseHostedDocument(host, entry.DocumentId);
                _addinEntries[pair.Key] = CreateDetachedAddinEntry(entry);
            }
        }

        private void RehostAddinsForCurrentView()
        {
            foreach (var pair in _addinEntries.ToList())
            {
                var entry = pair.Value;
                if (IsAddinHostedInCurrentView(entry))
                    continue;

                if (!CanAttemptAddinRehost(entry))
                    continue;

                var rehosted = TryRehostExistingAddin(entry);
                if (rehosted == null)
                    continue;

                _addinEntries[pair.Key] = rehosted;
            }
        }

        private bool IsAddinHostedInCurrentView(AddinEntry entry)
            => _host != null && IsAddinHostedInView(entry, _host);

        private bool IsAddinHostedInView(AddinEntry entry, BeepDocumentHost host)
        {
            if (string.IsNullOrEmpty(entry.DocumentId))
                return false;

            if (entry.DocumentPanel != null && !entry.DocumentPanel.IsDisposed)
                return ReferenceEquals(host.GetPanel(entry.DocumentId), entry.DocumentPanel);

            return host.GetPanel(entry.DocumentId) != null;
        }

        private static bool CanRehostAddin(AddinEntry entry)
            => entry.Addin == null || entry.Addin is not Control control || !control.IsDisposed;

        private static bool CanAttemptAddinRehost(AddinEntry entry)
            => entry.Addin == null || entry.Addin is not Control control || !control.IsDisposed;

        private static void DetachHostedAddinSurface(AddinEntry entry)
        {
            if (entry.Addin is not Control control || control.IsDisposed)
                return;

            try
            {
                control.Parent?.Controls.Remove(control);
            }
            catch { }
        }

        private void SuppressHostedAddinRemoval(string? documentId)
        {
            if (_viewDocumentTransferDepth > 0)
                return;

            if (!string.IsNullOrEmpty(documentId))
                _suppressedHostedAddinRemovals.Add(documentId);
        }

        private static AddinEntry CreateDetachedAddinEntry(AddinEntry source)
            => new()
            {
                Title = source.Title,
                Addin = source.Addin,
                DocumentId = string.Empty,
                MdiChild = null,
                DocumentPanel = null,
                OriginalFormBorderStyle = source.OriginalFormBorderStyle
            };

        private AddinEntry? TryRehostExistingAddin(AddinEntry entry)
        {
            if (_host == null || entry.Addin == null)
                return null;

            return HostAddinInPanel(entry.Title, entry.Addin, entry);
        }

        private void PrepareExtendedControlsForViewSwitch(BeepDocumentHost host)
        {
            foreach (var control in _extendedControls)
            {
                if (control == null || control.IsDisposed)
                    continue;

                if (!_attachedInfo.TryGetValue(control, out var info))
                    continue;

                if (string.IsNullOrEmpty(info.HostedDocumentId))
                    continue;

                DetachExtendedControlSurface(control);
                TryCloseHostedDocument(host, info.HostedDocumentId);
                info.HostedDocumentId = null;
            }
        }

        private static void DetachExtendedControlSurface(Control control)
        {
            try
            {
                control.Parent?.Controls.Remove(control);
            }
            catch { }
        }

        private static void TryCloseHostedDocument(BeepDocumentHost host, string? documentId)
        {
            if (host == null || string.IsNullOrEmpty(documentId))
                return;

            try { host.CloseDocument(documentId); } catch { }
        }

        private bool IsExtendedControlHostedInCurrentView(Control control, AttachedDocInfo info)
        {
            if (_host == null || string.IsNullOrEmpty(info.HostedDocumentId) || control.IsDisposed)
                return false;

            if (control.Parent is not BeepDocumentPanel panel || panel.IsDisposed)
                return false;

            return string.Equals(panel.DocumentId, info.HostedDocumentId, StringComparison.OrdinalIgnoreCase)
                   && ReferenceEquals(_host.GetPanel(info.HostedDocumentId), panel);
        }

        private bool TryRemoveDetachedExternalAddin(AddinEntry entry)
        {
            if (entry.Addin is not Form addinForm || addinForm.IsDisposed || !addinForm.TopLevel)
                return false;

            try
            {
                addinForm.Close();
            }
            catch
            {
                return false;
            }

            return addinForm.IsDisposed;
        }
    }
}
