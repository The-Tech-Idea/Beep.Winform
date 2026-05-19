// BeepMdiDocumentManagerDesigner.cs
// Design-time designer for BeepDocumentManager (BeepMDI).
// Provides smart-tag actions for managing views and documents.
// ---------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.BeepMDI;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMdiDocumentManagerDesigner : ComponentDesigner
    {
        private BeepDocumentManager? _manager;
        private DesignerVerbCollection? _verbs;
        private DesignerActionListCollection? _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _manager = component as BeepDocumentManager;

            // Listen for component removals to clean up our views list
            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeSvc)
            {
                changeSvc.ComponentRemoving += OnComponentRemoving;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeSvc)
                {
                    changeSvc.ComponentRemoving -= OnComponentRemoving;
                }
                _manager = null;
            }
            base.Dispose(disposing);
        }

        private void OnComponentRemoving(object? sender, ComponentEventArgs e)
        {
            // If a BeepDocumentHost is being removed from the form, also remove it from the manager
            if (e.Component is BeepDocumentHost host && _manager?.Views.Contains(host) == true)
            {
                _manager.RemoveView(host);
            }
        }

        // ── Verbs ──

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Add View", OnAddView),
                        new DesignerVerb("Remove Selected View", OnRemoveSelectedView),
                        new DesignerVerb("Add Document", OnAddDocument),
                        new DesignerVerb("Clear All Views", OnClearViews),
                    };
                }
                return _verbs;
            }
        }

        private void OnAddView(object? sender, EventArgs e)
        {
            if (_manager == null) return;

            var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            try
            {
                txn = designerHost?.CreateTransaction("Add View");
                changeSvc?.OnComponentChanging(_manager, null);

                BeepDocumentHost? view = null;
                if (designerHost != null)
                {
                    view = (BeepDocumentHost)designerHost.CreateComponent(typeof(BeepDocumentHost));
                }
                else
                {
                    view = new BeepDocumentHost();
                }

                if (view != null)
                {
                    _manager.Views.Add(view);
                    // Add to form controls if not already there
                    if (designerHost?.RootComponent is Control root && !root.Controls.Contains(view))
                    {
                        view.Dock = DockStyle.Fill;
                        root.Controls.Add(view);
                    }
                }

                changeSvc?.OnComponentChanged(_manager, null, null, null);
                txn?.Commit();

                if (view != null)
                {
                    var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                    selSvc?.SetSelectedComponents(new object[] { view });
                }
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        private void OnRemoveSelectedView(object? sender, EventArgs e)
        {
            if (_manager == null) return;

            // Get currently selected component
            var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
            var selected = selSvc?.PrimarySelection;

            // If a BeepDocumentHost is selected, remove it
            if (selected is BeepDocumentHost host && _manager.Views.Contains(host))
            {
                RemoveView(host);
                return;
            }

            // If the manager itself is selected, show a picker
            if (_manager.Views.Count == 0)
            {
                MessageBox.Show("No views to remove.", "Remove View", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show simple input dialog to pick which view to remove
            var viewNames = _manager.Views.Select((v, i) => $"{i + 1}. {v.Name}").ToArray();
            var result = InputBox.Show("Select view to remove:", "Remove View", viewNames);
            if (result >= 0 && result < _manager.Views.Count)
            {
                RemoveView(_manager.Views[result]);
            }
        }

        private void RemoveView(BeepDocumentHost view)
        {
            if (_manager == null) return;

            var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            try
            {
                txn = designerHost?.CreateTransaction("Remove View");
                changeSvc?.OnComponentChanging(_manager, null);

                // Remove from form controls
                if (view.Parent != null)
                {
                    view.Parent.Controls.Remove(view);
                }

                // Remove from manager
                _manager.RemoveView(view);

                // Destroy the component via designer host
                if (designerHost != null)
                {
                    designerHost.DestroyComponent(view);
                }
                else
                {
                    view.Dispose();
                }

                changeSvc?.OnComponentChanged(_manager, null, null, null);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        private void OnAddDocument(object? sender, EventArgs e)
        {
            if (_manager?.Views.Count == 0)
            {
                MessageBox.Show("Please add a View (BeepDocumentHost) first.", "No Views", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Add to first view
            var view = _manager.Views.FirstOrDefault();
            if (view != null)
            {
                var panel = view.AddDocument($"Document {view.DocumentCount + 1}");
                if (panel != null)
                {
                    var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                    selSvc?.SetSelectedComponents(new object[] { view });
                }
            }
        }

        private void OnClearViews(object? sender, EventArgs e)
        {
            if (_manager == null) return;

            var result = MessageBox.Show(
                "Remove all views and their documents?",
                "Clear All Views",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                
                // Remove all views from form and destroy them
                foreach (var view in _manager.Views.ToList())
                {
                    if (view.Parent != null)
                    {
                        view.Parent.Controls.Remove(view);
                    }
                    designerHost?.DestroyComponent(view);
                }
                
                _manager.ClearViews();
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
                        new BeepMdiDocumentManagerActionList(this)
                    };
                }
                return _actionLists;
            }
        }

        // ── Internal helpers for ActionList ──

        internal IDesignerHost? GetDesignerHost()
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

        internal IComponentChangeService? GetChangeService()
            => GetService(typeof(IComponentChangeService)) as IComponentChangeService;

        internal DesignerActionUIService? GetActionUIService()
            => GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;

        internal void SetPropertyWithTransaction(string propertyName, object? value, string transactionName)
        {
            if (_manager == null) return;
            var prop = TypeDescriptor.GetProperties(_manager)[propertyName];
            if (prop == null || prop.IsReadOnly) return;

            var changeSvc = GetChangeService();
            var oldValue = prop.GetValue(_manager);

            DesignerTransaction? txn = null;
            try
            {
                txn = GetDesignerHost()?.CreateTransaction(transactionName);
                changeSvc?.OnComponentChanging(_manager, prop);
                prop.SetValue(_manager, value);
                changeSvc?.OnComponentChanged(_manager, prop, oldValue, value);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
            finally
            {
                RefreshPanel();
            }
        }

        internal void RefreshPanel()
        {
            if (Component != null)
            {
                GetActionUIService()?.Refresh(Component);
            }
        }
    }

    // ── Action List ──

    internal class BeepMdiDocumentManagerActionList : DesignerActionList
    {
        private readonly BeepMdiDocumentManagerDesigner _designer;

        public BeepMdiDocumentManagerActionList(BeepMdiDocumentManagerDesigner designer) : base(designer.Component)
        {
            _designer = designer;
            AutoShow = true;
        }

        private BeepDocumentManager? Manager => _designer.Component as BeepDocumentManager;

        [Description("Beep theme name.")]
        public string ThemeName
        {
            get => Manager?.ThemeName ?? string.Empty;
            set => _designer.SetPropertyWithTransaction(nameof(BeepDocumentManager.ThemeName), value, "Set Theme");
        }

        public void AddView() => _designer.Verbs[0].Invoke();
        public void RemoveSelectedView() => _designer.Verbs[1].Invoke();
        public void AddDocument() => _designer.Verbs[2].Invoke();
        public void ClearAllViews() => _designer.Verbs[3].Invoke();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            
            items.Add(new DesignerActionHeaderItem("Views"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddView), "Add View", "Views", "Add a new BeepDocumentHost", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveSelectedView), "Remove Selected View", "Views", "Remove the selected view", true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddDocument), "Add Document", "Views", "Add a document to the first view", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllViews), "Clear All Views", "Views", "Remove all views", true));
            
            if (Manager != null)
            {
                items.Add(new DesignerActionTextItem($"Views: {Manager.Views.Count}", "Views"));
                items.Add(new DesignerActionTextItem($"Total Documents: {Manager.TotalDocumentCount}", "Views"));
            }
            
            items.Add(new DesignerActionHeaderItem("Settings"));
            items.Add(new DesignerActionPropertyItem(nameof(ThemeName), "Theme Name", "Settings", "Beep theme name"));
            
            return items;
        }
    }

    // Simple input box for selecting which view to remove
    internal static class InputBox
    {
        public static int Show(string prompt, string title, string[] options)
        {
            using var form = new Form();
            form.Text = title;
            form.Width = 350;
            form.Height = 150 + (options.Length * 25);
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;

            var label = new Label { Left = 10, Top = 10, Width = 320, Text = prompt };
            form.Controls.Add(label);

            var listBox = new ListBox { Left = 10, Top = 35, Width = 320, Height = options.Length * 25 };
            listBox.Items.AddRange(options);
            form.Controls.Add(listBox);

            var btnOk = new Button { Text = "OK", Left = 170, Top = form.Height - 70, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 255, Top = form.Height - 70, DialogResult = DialogResult.Cancel };
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0)
            {
                return listBox.SelectedIndex;
            }
            return -1;
        }
    }
}
