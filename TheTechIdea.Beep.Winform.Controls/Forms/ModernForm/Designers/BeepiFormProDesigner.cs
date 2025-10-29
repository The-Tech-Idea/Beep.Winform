using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Designers
{
 // Simple designer to force design-time repaints when components change
 public class BeepiFormProDesigner : ParentControlDesigner
 {
 private IComponentChangeService _changeSvc;
 private Control _control;

 public override void Initialize(System.ComponentModel.IComponent component)
 {
 base.Initialize(component);

 _control = component as Control;

 _changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
 if (_changeSvc != null)
 {
 _changeSvc.ComponentAdded += OnComponentAdded;
 _changeSvc.ComponentRemoved += OnComponentRemoved;
 _changeSvc.ComponentChanged += OnComponentChanged;
 }

 // Hook selection service to repaint when selection changes
 var sel = (ISelectionService)GetService(typeof(ISelectionService));
 if (sel != null)
 {
 sel.SelectionChanged += (s, e) => RepaintDesignerControl();
 }

 // Hook child control events for design-time repaint
 if (_control != null)
 {
 _control.ControlAdded += Control_ControlAdded;
 _control.ControlRemoved += Control_ControlRemoved;

 // Hook existing child controls
 foreach (Control child in _control.Controls)
 {
 HookChildEvents(child);
 }
 }
 }

 private void Control_ControlAdded(object sender, ControlEventArgs e)
 {
 HookChildEvents(e.Control);
 RepaintDesignerControl();
 }

 private void Control_ControlRemoved(object sender, ControlEventArgs e)
 {
 UnhookChildEvents(e.Control);
 RepaintDesignerControl();
 }

 private void HookChildEvents(Control ctrl)
 {
 if (ctrl == null) return;
 try
 {
 ctrl.Move += ChildControlChanged;
 ctrl.Resize += ChildControlChanged;
 ctrl.VisibleChanged += ChildControlChanged;
 }
 catch { }
 }

 private void UnhookChildEvents(Control ctrl)
 {
 if (ctrl == null) return;
 try
 {
 ctrl.Move -= ChildControlChanged;
 ctrl.Resize -= ChildControlChanged;
 ctrl.VisibleChanged -= ChildControlChanged;
 }
 catch { }
 }

 private void ChildControlChanged(object sender, EventArgs e)
 {
 RepaintDesignerControl();
 }

 private void OnComponentAdded(object sender, ComponentEventArgs e)
 {
 RepaintDesignerControl();
 }

 private void OnComponentRemoved(object sender, ComponentEventArgs e)
 {
 RepaintDesignerControl();
 }

 private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
 {
 RepaintDesignerControl();
 }

 private void RepaintDesignerControl()
 {
 if (Component is Control c && c.IsHandleCreated)
 {
 // Invalidate + Update to ensure immediate repaint in designer
 try
 {
 c.Invalidate();
 c.Update();
 c.Refresh();
 }
 catch { }
 }
 }

 public override DesignerActionListCollection ActionLists
 {
 get
 {
 var lists = new DesignerActionListCollection();
 lists.Add(new BeepiFormProActionList(Component));
 return lists;
 }
 }

 protected override void Dispose(bool disposing)
 {
 if (_changeSvc != null)
 {
 _changeSvc.ComponentAdded -= OnComponentAdded;
 _changeSvc.ComponentRemoved -= OnComponentRemoved;
 _changeSvc.ComponentChanged -= OnComponentChanged;
 }

 if (_control != null)
 {
 try
 {
 _control.ControlAdded -= Control_ControlAdded;
 _control.ControlRemoved -= Control_ControlRemoved;

 foreach (Control child in _control.Controls)
 {
 UnhookChildEvents(child);
 }
 }
 catch { }
 }

 base.Dispose(disposing);
 }
 }
}
