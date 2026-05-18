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

        private void PrepareHostedContentForViewSwitch(IBeepDocumentManagerView view)
        {
            if (view == null || IsDesignTimeComponent)
                return;

            PrepareAddinsForViewSwitch(view);
            PrepareExtendedControlsForViewSwitch(view);
        }

        private void RehostHostedContentForCurrentView()
        {
            if (_view == null || IsDesignTimeComponent)
                return;

            RehostAddinsForCurrentView();
        }

        private void PrepareAddinsForViewSwitch(IBeepDocumentManagerView view)
        {
            if (_addinEntries.Count == 0)
                return;

            foreach (var pair in _addinEntries.ToList())
            {
                var entry = pair.Value;
                if (!IsAddinHostedInView(entry, view))
                    continue;

                if (TryDetachExternalMdiAddin(view, entry, out var detachedEntry))
                {
                    _addinEntries[pair.Key] = detachedEntry;
                    continue;
                }

                if (!CanRehostAddin(entry))
                    continue;

                DetachHostedAddinSurface(entry);
                SuppressHostedAddinRemoval(entry.DocumentId);
                TryCloseHostedDocument(view, entry.DocumentId);
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
            => _view != null && IsAddinHostedInView(entry, _view);

        private bool IsAddinHostedInView(AddinEntry entry, IBeepDocumentManagerView view)
        {
            if (string.IsNullOrEmpty(entry.DocumentId))
                return false;

            if (entry.DocumentPanel != null && !entry.DocumentPanel.IsDisposed)
                return ReferenceEquals(view.GetPanel(entry.DocumentId), entry.DocumentPanel);

            if (entry.MdiChild != null && !entry.MdiChild.IsDisposed && view is BeepNativeMdiView mdi)
                return ReferenceEquals(entry.MdiChild.MdiParent, mdi.ParentForm);

            return view.GetPanel(entry.DocumentId) != null;
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
            if (_view == null || entry.Addin == null)
                return null;

            if (_view is BeepNativeMdiView mdi)
                return HostAddinInMdi(entry.Title, entry.Addin, mdi, entry);

            return HostAddinInPanel(entry.Title, entry.Addin, entry);
        }

        private bool TryDetachExternalMdiAddin(
            IBeepDocumentManagerView view,
            AddinEntry entry,
            out AddinEntry detachedEntry)
        {
            detachedEntry = entry;

            if (view is not BeepNativeMdiView mdi || string.IsNullOrEmpty(entry.DocumentId))
                return false;

            if (entry.Addin is not Form addinForm || addinForm.IsDisposed || !ReferenceEquals(entry.MdiChild, addinForm))
                return false;

            if (!mdi.DetachDocumentForm(entry.DocumentId, addinForm))
                return false;

            detachedEntry = CreateDetachedAddinEntry(entry);
            return true;
        }

        private void PrepareExtendedControlsForViewSwitch(IBeepDocumentManagerView view)
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
                TryCloseHostedDocument(view, info.HostedDocumentId);
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

        private static void TryCloseHostedDocument(IBeepDocumentManagerView view, string? documentId)
        {
            if (view == null || string.IsNullOrEmpty(documentId))
                return;

            try { view.DetachDocumentForTransfer(documentId); } catch { }
        }

        private bool IsExtendedControlHostedInCurrentView(Control control, AttachedDocInfo info)
        {
            if (_view == null || string.IsNullOrEmpty(info.HostedDocumentId) || control.IsDisposed)
                return false;

            if (_view is BeepNativeMdiView mdi)
                return IsExtendedControlHostedInNativeMdi(mdi, control);

            if (control.Parent is not BeepDocumentPanel panel || panel.IsDisposed)
                return false;

            return string.Equals(panel.DocumentId, info.HostedDocumentId, StringComparison.OrdinalIgnoreCase)
                   && ReferenceEquals(_view.GetPanel(info.HostedDocumentId), panel);
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