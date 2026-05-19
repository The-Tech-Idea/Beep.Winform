// BeepMdiDocumentHostDesigner.cs
// Design-time designer for BeepDocumentHost (BeepMDI).
// Provides tab clicking, document management, and smart-tag actions.
// ---------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Behaviors;
using TheTechIdea.Beep.Winform.Controls.BeepMDI;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMdiDocumentHostDesigner : ParentControlDesigner
    {
        private BeepDocumentHost? _host;
        private DesignerVerbCollection? _verbs;
        private DesignerActionListCollection? _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _host = component as BeepDocumentHost;

            if (_host != null)
            {
                _host.HandleCreated += OnHostHandleCreated;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _host != null)
            {
                try { _host.HandleCreated -= OnHostHandleCreated; } catch { }
            }
            base.Dispose(disposing);
        }

        private void OnHostHandleCreated(object? sender, EventArgs e)
        {
            // Sync selection to active document when handle is created
            if (_host?.ActiveDocumentId != null)
            {
                var panel = _host.GetPanel(_host.ActiveDocumentId);
                if (panel != null)
                {
                    var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                    selSvc?.SetSelectedComponents(new object[] { panel });
                }
            }
        }

        // ── Designer Verbs ──

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Add Document", OnAddDocument),
                        new DesignerVerb("Clear All Documents", OnClearDocuments),
                    };
                }
                return _verbs;
            }
        }

        private void OnAddDocument(object? sender, EventArgs e)
        {
            if (_host == null) return;

            var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            BeepDocumentPanel? panel = null;
            try
            {
                txn = designerHost?.CreateTransaction("Add Document");
                changeSvc?.OnComponentChanging(_host, null);

                // Create panel through designer host so it serializes to designer.cs
                if (designerHost != null)
                {
                    panel = (BeepDocumentPanel)designerHost.CreateComponent(typeof(BeepDocumentPanel));
                    if (panel != null)
                    {
                        panel.DocumentTitle = $"Document {_host.DocumentCount + 1}";
                        // Add to host's Controls so designer shows it
                        if (!_host.Controls.Contains(panel))
                        {
                            _host.Controls.Add(panel);
                        }
                        // Add to collection - RegisterDocumentPanel will be called automatically
                        _host.DocumentPanels.Add(panel);
                    }
                }
                else
                {
                    // Fallback for runtime
                    panel = _host.AddDocument($"Document {_host.DocumentCount + 1}");
                }
                
                changeSvc?.OnComponentChanged(_host, null, null, null);
                txn?.Commit();

                // Select the new panel
                if (panel != null)
                {
                    var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                    selSvc?.SetSelectedComponents(new object[] { panel });
                }
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        private void OnClearDocuments(object? sender, EventArgs e)
        {
            if (_host == null) return;

            var result = MessageBox.Show(
                "Remove all documents?",
                "Clear All Documents",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            try
            {
                txn = host?.CreateTransaction("Clear All Documents");
                changeSvc?.OnComponentChanging(_host, null);

                _host.CloseAllDocuments();

                changeSvc?.OnComponentChanged(_host, null, null, null);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        // ── Smart Tag Action List ──

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection
                    {
                        new BeepMdiDocumentHostActionList(this)
                    };
                }
                return _actionLists;
            }
        }

        // ── Mouse Handling for Tab Clicks ──

        protected override void OnMouseDragBegin(int x, int y, Keys modifierKeys)
        {
            if (_host == null)
            {
                base.OnMouseDragBegin(x, y, modifierKeys);
                return;
            }

            // Check if clicking on a tab
            var clientPoint = _host.PointToClient(new Point(x, y));
            
            // If click is in tab strip area, handle tab selection
            if (clientPoint.Y < 32 && _host.DocumentCount > 0)
            {
                // Let the host handle the click (it will activate the document)
                // Then select the active panel
                var activeId = _host.ActiveDocumentId;
                if (activeId != null)
                {
                    var panel = _host.GetPanel(activeId);
                    if (panel != null)
                    {
                        var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                        selSvc?.SetSelectedComponents(new object[] { panel });
                        return; // Don't start drag
                    }
                }
            }

            base.OnMouseDragBegin(x, y, modifierKeys);
        }

        // ── Toolbox Drop Support ──

        protected override Control GetParentForComponent(IComponent component)
        {
            if (_host == null) return base.GetParentForComponent(component);

            // If host has documents, drop into the active one
            if (_host.DocumentCount > 0 && _host.ActiveDocumentId != null)
            {
                var panel = _host.GetPanel(_host.ActiveDocumentId);
                if (panel != null) return panel;
            }

            // If empty, create a document first
            if (_host.DocumentCount == 0)
            {
                var panel = _host.AddDocument("Document 1");
                if (panel != null) return panel;
            }

            return base.GetParentForComponent(component);
        }

        public override bool CanParent(Control control)
        {
            // Allow all controls - they will be routed to the active document panel
            // via GetParentForComponent
            return true;
        }

        // ── Adornments ──

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            if (_host == null || _host.DocumentCount > 0) return;

            // Draw hint when empty
            var g = pe.Graphics;
            var rc = _host.ClientRectangle;

            using var dashPen = new Pen(Color.Gray, 1f);
            dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(dashPen, rc.X + 4, rc.Y + 4, rc.Width - 8, rc.Height - 8);

            using var font = new Font("Segoe UI", 10f);
            using var brush = new SolidBrush(Color.Gray);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("BeepDocumentHost\nDouble-click to add a document", font, brush, rc, sf);
        }

        public override void DoDefaultAction()
        {
            OnAddDocument(null, EventArgs.Empty);
        }

        // ── Property Filtering ──

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            string[] hidden = {
                "AutoScroll", "AutoScrollMargin", "AutoScrollMinSize",
                "AutoScrollPosition", "HorizontalScroll", "VerticalScroll",
                "BorderStyle", "AutoSize", "AutoSizeMode", "Padding", "Margin",
                "MinimumSize", "MaximumSize", "BackColor", "BackgroundImage",
                "BackgroundImageLayout", "ForeColor", "Font", "Cursor",
                "RightToLeft", "ImeMode", "CausesValidation", "UseWaitCursor",
                "AllowDrop", "Text", "Tag", "AccessibleDescription",
                "AccessibleName", "AccessibleRole"
            };

            foreach (var name in hidden)
            {
                if (properties[name] is PropertyDescriptor pd)
                    properties[name] = TypeDescriptor.CreateProperty(pd.ComponentType, pd, new BrowsableAttribute(false));
            }
        }
    }

    // ── Action List ──

    internal class BeepMdiDocumentHostActionList : DesignerActionList
    {
        private readonly BeepMdiDocumentHostDesigner _designer;

        public BeepMdiDocumentHostActionList(BeepMdiDocumentHostDesigner designer) : base(designer.Component)
        {
            _designer = designer;
            AutoShow = true;
        }

        private BeepDocumentHost? Host => _designer.Component as BeepDocumentHost;

        public void AddDocument() => _designer.Verbs[0].Invoke();
        public void ClearAllDocuments() => _designer.Verbs[1].Invoke();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Documents"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddDocument), "Add Document", "Documents", "Add a new document", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllDocuments), "Clear All Documents", "Documents", "Remove all documents", true));
            
            if (Host != null)
            {
                items.Add(new DesignerActionTextItem($"Documents: {Host.DocumentCount}", "Documents"));
            }
            
            return items;
        }
    }
}
