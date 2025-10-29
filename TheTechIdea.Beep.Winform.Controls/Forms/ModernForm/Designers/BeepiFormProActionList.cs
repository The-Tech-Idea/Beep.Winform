using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Designers
{
 public class BeepiFormProActionList : DesignerActionList
 {
 private readonly IComponent _component;
 private readonly DesignerActionUIService _uiService;

 public BeepiFormProActionList(IComponent component) : base(component)
 {
 _component = component;
 _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
 }

 private BeepiFormPro Target => _component as BeepiFormPro;

 public bool ShowCaptionBar
 {
 get => Target?.ShowCaptionBar ?? false;
 set {
 if (Target != null) {
 var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
 changeSvc?.OnComponentChanged(_component, null, null, null);
 Target.ShowCaptionBar = value;
 Target.Invalidate();
 }
 }
 }

 public bool EnableAnimations
 {
 get => Target?.EnableAnimations ?? false;
 set {
 if (Target != null) {
 var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
 changeSvc?.OnComponentChanged(_component, null, null, null);
 Target.EnableAnimations = value;
 Target.Invalidate();
 }
 }
 }

 public override DesignerActionItemCollection GetSortedActionItems()
 {
 var items = new DesignerActionItemCollection();
 items.Add(new DesignerActionHeaderItem("Appearance"));
 items.Add(new DesignerActionPropertyItem("ShowCaptionBar", "Show Caption Bar", "Appearance"));
 items.Add(new DesignerActionPropertyItem("EnableAnimations", "Enable Animations", "Appearance"));
 return items;
 }
 }
}
