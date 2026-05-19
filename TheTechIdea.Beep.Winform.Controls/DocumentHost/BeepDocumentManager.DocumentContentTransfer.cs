using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        private readonly Dictionary<string, List<Control>> _transferredDocumentContent =
            new(StringComparer.OrdinalIgnoreCase);

        private void CaptureManagerOwnedDocumentContentForTransfer(BeepDocumentHost host)
        {
            if (host == null || IsDesignTimeComponent)
                return;

            foreach (var documentId in EnumerateManagerOwnedDocumentIds())
            {
                if (string.IsNullOrWhiteSpace(documentId))
                    continue;

                var panel = host.GetPanel(documentId);
                if (panel == null || panel.IsDisposed)
                    continue;

                CaptureDocumentContent(documentId, panel);
            }
        }

        private void CaptureDocumentContent(string documentId, Control container)
        {
            if (string.IsNullOrWhiteSpace(documentId) || container == null || container.IsDisposed)
                return;

            var captured = container.Controls.Cast<Control>()
                .Where(control => control != null && !control.IsDisposed)
                .ToList();

            if (captured.Count == 0)
                return;

            foreach (var control in captured)
            {
                try { container.Controls.Remove(control); } catch { }
            }

            if (_transferredDocumentContent.TryGetValue(documentId, out var existing))
                existing.AddRange(captured);
            else
                _transferredDocumentContent[documentId] = captured;
        }

        private void RehostTransferredDocumentContentForCurrentView()
        {
            if (_host == null || IsDesignTimeComponent || _transferredDocumentContent.Count == 0)
                return;

            foreach (var pair in _transferredDocumentContent.ToList())
            {
                if (TryRehostTransferredDocumentContent(pair.Key, pair.Value))
                    _transferredDocumentContent.Remove(pair.Key);
            }
        }

        private bool TryRehostTransferredDocumentContent(string documentId, List<Control> controls)
        {
            if (_host == null || string.IsNullOrWhiteSpace(documentId) || controls == null || controls.Count == 0)
                return false;

            Control? target = _host.GetPanel(documentId);

            if (target == null || target.IsDisposed)
                return false;

            if (target.Controls.Count > 0)
                target.Controls.Clear();

            foreach (var control in controls.ToList())
            {
                if (control == null || control.IsDisposed)
                    continue;

                if (!target.Controls.Contains(control))
                    target.Controls.Add(control);

                control.Visible = true;
            }

            return true;
        }
    }
}