using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.MDI;

namespace TheTechIdea.Beep.Winform.Controls.MDI.Designers
{
    // Designer for BeepMdiManager (a Component, not a Control)
    public class BeepMdiManagerDesigner : ComponentDesigner
    {
        private DesignerActionListCollection _lists;
        public override DesignerActionListCollection ActionLists => _lists ??= new DesignerActionListCollection { new ActionList(this) };

        internal BeepMdiManager Manager => Component as BeepMdiManager;

        internal void ShowEditorDialog()
        {
            if (Manager == null) return;
            using var dlg = new BeepMdiManagerDocumentsDialog(Manager, (IServiceProvider)Component.Site);
            dlg.ShowDialog();
        }

        private class ActionList : DesignerActionList
        {
            private readonly BeepMdiManagerDesigner _designer;
            private readonly IComponentChangeService _change;
            public ActionList(BeepMdiManagerDesigner designer) : base(designer.Component)
            {
                _designer = designer;
                _change = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            }
            private BeepMdiManager M => _designer.Manager;
            private void Set(string prop, object value)
            {
                if (M == null) return;
                var p = TypeDescriptor.GetProperties(M)[prop];
                if (p == null) return; var old = p.GetValue(M); if (Equals(old, value)) return;
                _change?.OnComponentChanging(M, p);
                p.SetValue(M, value);
                _change?.OnComponentChanged(M, p, old, value);
            }
            public bool EnableTabbedMdi { get => M != null && M.EnableTabbedMdi; set { if (M!=null) Set(nameof(M.EnableTabbedMdi), value); } }
            public bool HideChildCaptions { get => M != null && M.HideChildCaptions; set { if (M!=null) Set(nameof(M.HideChildCaptions), value); } }
            public bool AllowTabReorder { get => M != null && M.AllowTabReorder; set { if (M!=null) Set(nameof(M.AllowTabReorder), value); } }
            public bool EnableMenuMerge { get => M != null && M.EnableMenuMerge; set { if (M!=null) Set(nameof(M.EnableMenuMerge), value); } }
            public bool AutoCreateDocumentsOnLoad { get => M != null && M.AutoCreateDocumentsOnLoad; set { if (M!=null) Set(nameof(M.AutoCreateDocumentsOnLoad), value); } }

            public void AddDocument()
            {
                if (M == null) return;
                var docsProp = TypeDescriptor.GetProperties(M)[nameof(M.Documents)];
                _change?.OnComponentChanging(M, docsProp);
                M.Documents.Add(new BeepMdiDocument { Name = "Doc" + (M.Documents.Count + 1), Text = "Document " + (M.Documents.Count + 1) });
                _change?.OnComponentChanged(M, docsProp, null, null);
            }

            public void EditDocuments() => _designer.ShowEditorDialog();

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                return new DesignerActionItemCollection
                {
                    new DesignerActionHeaderItem("Documents"),
                    new DesignerActionMethodItem(this,nameof(AddDocument),"Add Document"),
                    new DesignerActionMethodItem(this,nameof(EditDocuments),"Edit Documents…"),
                    new DesignerActionHeaderItem("Options"),
                    new DesignerActionPropertyItem(nameof(EnableTabbedMdi),"Enable Tabbed MDI"),
                    new DesignerActionPropertyItem(nameof(HideChildCaptions),"Hide Child Captions"),
                    new DesignerActionPropertyItem(nameof(AllowTabReorder),"Allow Tab Reorder"),
                    new DesignerActionPropertyItem(nameof(EnableMenuMerge),"Enable Menu Merge"),
                    new DesignerActionPropertyItem(nameof(AutoCreateDocumentsOnLoad),"Auto-create On Load")
                };
            }
        }
    }

    internal sealed class BeepMdiManagerDocumentsDialog : Form
    {
        private readonly BeepMdiManager _manager;
        private readonly IComponentChangeService _change;
        private readonly ListBox _list;
        private readonly PropertyGrid _pg;
        private readonly ComboBox _cmbMenus;
        private readonly CheckBox _chkEnableMerge;
        public BeepMdiManagerDocumentsDialog(BeepMdiManager manager, IServiceProvider sp)
        {
            _manager = manager;
            _change = sp?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            Text = "Beep MDI Documents"; Width=760; Height=420; FormBorderStyle=FormBorderStyle.FixedDialog; MaximizeBox=false; MinimizeBox=false; StartPosition=FormStartPosition.CenterParent;
            var leftPanel = new Panel{Dock=DockStyle.Left,Width=220,Padding=new Padding(4)};
            var btnAdd = new Button{Text="Add",Width=60}; var btnDel=new Button{Text="Delete",Width=60}; var btnUp=new Button{Text="Up",Width=50}; var btnDown=new Button{Text="Down",Width=50};
            var flow=new FlowLayoutPanel{Dock=DockStyle.Top,Height=34}; flow.Controls.AddRange(new Control[]{btnAdd,btnDel,btnUp,btnDown});
            _list=new ListBox{Dock=DockStyle.Fill}; leftPanel.Controls.Add(_list); leftPanel.Controls.Add(flow); Controls.Add(leftPanel);
            _pg=new PropertyGrid{Dock=DockStyle.Fill}; Controls.Add(_pg);
            var right=new Panel{Dock=DockStyle.Right,Width=220,Padding=new Padding(6)}; Controls.Add(right);
            _chkEnableMerge=new CheckBox{Text="Enable Menu Merge",AutoSize=true}; right.Controls.Add(_chkEnableMerge);
            right.Controls.Add(new Label{Text="Target MenuStrip:",AutoSize=true,Top=40});
            _cmbMenus=new ComboBox{Left=0,Top=60,Width=200,DropDownStyle=ComboBoxStyle.DropDownList}; right.Controls.Add(_cmbMenus);
            var btnOK=new Button{Text="OK",DialogResult=DialogResult.OK,Top=300,Left=50,Width=70}; var btnCancel=new Button{Text="Cancel",DialogResult=DialogResult.Cancel,Top=300,Left=130,Width=70}; right.Controls.Add(btnOK); right.Controls.Add(btnCancel);
            AcceptButton=btnOK; CancelButton=btnCancel;

            // events
            _list.SelectedIndexChanged += (s,e)=> _pg.SelectedObject=_list.SelectedItem;
            btnAdd.Click += (s,e)=> AddDoc();
            btnDel.Click += (s,e)=> DeleteDoc();
            btnUp.Click += (s,e)=> MoveDoc(-1);
            btnDown.Click += (s,e)=> MoveDoc(1);
            btnOK.Click += (s,e)=> ApplyChanges();

            LoadInitial();
        }
        private void LoadInitial()
        {
            _list.Items.Clear();
            foreach(var d in _manager.Documents) _list.Items.Add(d);
            if(_list.Items.Count>0) _list.SelectedIndex=0;
            _chkEnableMerge.Checked=_manager.EnableMenuMerge;
            if(_manager.HostForm!=null)
            {
                foreach(var ms in _manager.HostForm.Controls.OfType<MenuStrip>())
                {
                    _cmbMenus.Items.Add(ms.Name);
                    if(_manager.MergeTargetMenuStrip==ms) _cmbMenus.SelectedItem=ms.Name;
                }
            }
        }
        private void AddDoc()
        {
            var docsProp=TypeDescriptor.GetProperties(_manager)[nameof(_manager.Documents)];
            _change?.OnComponentChanging(_manager,docsProp);
            var doc=new BeepMdiDocument{ Name="Doc"+(_manager.Documents.Count+1), Text="Document "+(_manager.Documents.Count+1)};
            _manager.Documents.Add(doc);
            _change?.OnComponentChanged(_manager,docsProp,null,null);
            _list.Items.Add(doc); _list.SelectedItem=doc;
        }
        private void DeleteDoc()
        {
            if(_list.SelectedItem is BeepMdiDocument doc){
                var docsProp=TypeDescriptor.GetProperties(_manager)[nameof(_manager.Documents)];
                _change?.OnComponentChanging(_manager,docsProp);
                _manager.Documents.Remove(doc);
                _change?.OnComponentChanged(_manager,docsProp,null,null);
                int idx=_list.SelectedIndex; _list.Items.RemoveAt(idx); if(_list.Items.Count>0) _list.SelectedIndex=Math.Min(idx,_list.Items.Count-1);
            }
        }
        private void MoveDoc(int delta)
        {
            int idx=_list.SelectedIndex; if(idx<0) return; int ni=idx+delta; if(ni<0||ni>=_list.Items.Count) return;
            var docsProp=TypeDescriptor.GetProperties(_manager)[nameof(_manager.Documents)];
            _change?.OnComponentChanging(_manager,docsProp);
            var item=_manager.Documents[idx]; _manager.Documents.RemoveAt(idx); _manager.Documents.Insert(ni,item);
            _change?.OnComponentChanged(_manager,docsProp,null,null);
            _list.Items.Clear(); foreach(var d in _manager.Documents) _list.Items.Add(d); _list.SelectedIndex=ni;
        }
        private void ApplyChanges()
        {
            var mgrProps=TypeDescriptor.GetProperties(_manager);
            void Set(string n,object v){var p=mgrProps[n]; if(p==null) return; var old=p.GetValue(_manager); if(Equals(old,v)) return; _change?.OnComponentChanging(_manager,p); p.SetValue(_manager,v); _change?.OnComponentChanged(_manager,p,old,v);}            
            Set(nameof(_manager.EnableMenuMerge), _chkEnableMerge.Checked);
            if(_cmbMenus.SelectedItem!=null && _manager.HostForm!=null){
                var ms=_manager.HostForm.Controls.OfType<MenuStrip>().FirstOrDefault(m=>m.Name==_cmbMenus.SelectedItem.ToString());
                Set(nameof(_manager.MergeTargetMenuStrip), ms);
            }
        }
    }
}
