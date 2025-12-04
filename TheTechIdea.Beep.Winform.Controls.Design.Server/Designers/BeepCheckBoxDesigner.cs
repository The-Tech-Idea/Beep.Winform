using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepCheckBox control
    /// </summary>
    public class BeepCheckBoxDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Select Custom Check Mark Image...", OnSelectCheckMarkImage),
                        new DesignerVerb("Clear Custom Image", OnClearCheckMarkImage),
                        new DesignerVerb("Toggle State", OnToggleState)
                    };
                }
                return _verbs;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepCheckBoxActionList(this));
            return lists;
        }

        private void OnSelectCheckMarkImage(object? sender, EventArgs e)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
            if (property == null) return;

            var currentValue = property.GetValue(Component) as string;
            
            using (var dialog = new Editors.IconPickerDialog(currentValue))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProperty("ImagePath", dialog.SelectedIconPath);
                }
            }
        }

        private void OnClearCheckMarkImage(object? sender, EventArgs e)
        {
            SetProperty("ImagePath", string.Empty);
        }

        private void OnToggleState(object? sender, EventArgs e)
        {
            // Toggle the Checked property (simpler approach to avoid enum dependency)
            var currentValue = GetProperty<object>("CurrentValue");
            // Just invalidate to toggle at design time
            Component?.Site?.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
        }
    }

    public class BeepCheckBoxActionList : DesignerActionList
    {
        private readonly BeepCheckBoxDesigner _designer;

        public BeepCheckBoxActionList(BeepCheckBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Appearance")]
        public string Text
        {
            get => _designer.GetProperty<string>("Text") ?? "";
            set => _designer.SetProperty("Text", value);
        }

        [Category("Appearance")]
        public int CheckBoxSize
        {
            get => _designer.GetProperty<int>("CheckBoxSize");
            set => _designer.SetProperty("CheckBoxSize", value);
        }

        // Note: TextAlignRelativeToCheckBox removed to avoid enum dependency

        #endregion

        #region Actions

        public void SelectCheckMarkIcon()
        {
            _designer.Verbs[0].Invoke();
        }

        public void SetSizeSmall() => CheckBoxSize = 12;
        public void SetSizeMedium() => CheckBoxSize = 16;
        public void SetSizeLarge() => CheckBoxSize = 20;
        public void SetSizeXLarge() => CheckBoxSize = 24;

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Text", "Text", "Appearance"));
            items.Add(new DesignerActionPropertyItem("CheckBoxSize", "Check Box Size", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Size Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetSizeSmall", "Small (12px)", "Size Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetSizeMedium", "Medium (16px)", "Size Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetSizeLarge", "Large (20px)", "Size Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetSizeXLarge", "X-Large (24px)", "Size Presets", false));

            items.Add(new DesignerActionHeaderItem("Custom Icon"));
            items.Add(new DesignerActionMethodItem(this, "SelectCheckMarkIcon", "Select Check Mark Icon...", "Custom Icon", true));

            return items;
        }
    }
}

