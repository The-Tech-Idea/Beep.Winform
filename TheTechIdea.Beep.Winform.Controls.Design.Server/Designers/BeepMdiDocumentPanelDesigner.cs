// BeepMdiDocumentPanelDesigner.cs
// Design-time designer for BeepDocumentPanel (BeepMDI).
// Filters properties and allows child control editing.
// ---------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Behaviors;
using TheTechIdea.Beep.Winform.Controls.BeepMDI;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMdiDocumentPanelDesigner : ParentControlDesigner
    {
        private static readonly HashSet<string> _visibleProperties = new(StringComparer.Ordinal)
        {
            nameof(Control.Name),
            nameof(BeepDocumentPanel.DocumentId),
            nameof(BeepDocumentPanel.DocumentTitle),
            nameof(BeepDocumentPanel.IconPath),
            nameof(BeepDocumentPanel.CanClose),
            nameof(BeepDocumentPanel.IsPinned),
            nameof(BeepDocumentPanel.IsModified),
        };

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (component is BeepDocumentPanel panel)
            {
                panel.ControlAdded += Panel_ControlAdded;
                panel.ControlRemoved += Panel_ControlRemoved;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Component is BeepDocumentPanel panel)
            {
                panel.ControlAdded -= Panel_ControlAdded;
                panel.ControlRemoved -= Panel_ControlRemoved;
            }
            base.Dispose(disposing);
        }

        private void Panel_ControlAdded(object? sender, ControlEventArgs e)
        {
            // Allow child controls to be designed
        }

        private void Panel_ControlRemoved(object? sender, ControlEventArgs e)
        {
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            var toRemove = new List<string>();
            foreach (DictionaryEntry entry in properties)
            {
                if (entry.Key is string key && !_visibleProperties.Contains(key))
                    toRemove.Add(key);
            }

            foreach (var key in toRemove)
                properties.Remove(key);
        }

        public override SelectionRules SelectionRules
            => SelectionRules.Visible | SelectionRules.Locked;

        public override bool CanBeParentedTo(IDesigner parentDesigner)
            => parentDesigner?.Component is BeepDocumentHost;

        public override IList<SnapLine> SnapLines
            => new List<SnapLine>();
    }
}
