using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepExtendedButton control
    /// </summary>
    public class BeepExtendedButtonDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepExtendedButton? ExtendedButton => Component as BeepExtendedButton;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Select Main Button Icon...", OnSelectMainIcon),
                        new DesignerVerb("Select Extend Button Icon...", OnSelectExtendIcon),
                        new DesignerVerb("Clear Icons", OnClearIcons)
                    };
                }
                return _verbs;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepExtendedButtonActionList(this));
            return lists;
        }

        private void OnSelectMainIcon(object? sender, EventArgs e)
        {
            SelectIcon("ImagePath");
        }

        private void OnSelectExtendIcon(object? sender, EventArgs e)
        {
            SelectIcon("ExtendButtonImagePath");
        }

        private void SelectIcon(string propertyName)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null) return;

            var currentValue = property.GetValue(Component) as string;
            
            using (var dialog = new Editors.IconPickerDialog(currentValue))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProperty(propertyName, dialog.SelectedIconPath);
                }
            }
        }

        private void OnClearIcons(object? sender, EventArgs e)
        {
            SetProperty("ImagePath", string.Empty);
            SetProperty("ExtendButtonImagePath", string.Empty);
        }
    }

    public class BeepExtendedButtonActionList : DesignerActionList
    {
        private readonly BeepExtendedButtonDesigner _designer;

        public BeepExtendedButtonActionList(BeepExtendedButtonDesigner designer)
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

        [Category("Layout")]
        public int ButtonWidth
        {
            get => _designer.GetProperty<int>("ButtonWidth");
            set => _designer.SetProperty("ButtonWidth", value);
        }

        [Category("Layout")]
        public int RightButtonSize
        {
            get => _designer.GetProperty<int>("RightButtonSize");
            set => _designer.SetProperty("RightButtonSize", value);
        }

        #endregion

        #region Actions

        public void SelectMainIcon() => _designer.Verbs[0].Invoke();
        public void SelectExtendIcon() => _designer.Verbs[1].Invoke();

        public void UseDropdownExtendIcon()
        {
            _designer.SetProperty("ExtendButtonImagePath", TheTechIdea.Beep.Icons.SvgsUI.ChevronDown);
        }

        public void UseMoreExtendIcon()
        {
            _designer.SetProperty("ExtendButtonImagePath", TheTechIdea.Beep.Icons.SvgsUI.Menu);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Text", "Button Text", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ButtonWidth", "Main Button Width", "Layout"));
            items.Add(new DesignerActionPropertyItem("RightButtonSize", "Extend Button Size", "Layout"));

            items.Add(new DesignerActionHeaderItem("Icons"));
            items.Add(new DesignerActionMethodItem(this, "SelectMainIcon", "Select Main Icon...", "Icons", true));
            items.Add(new DesignerActionMethodItem(this, "SelectExtendIcon", "Select Extend Icon...", "Icons", true));

            items.Add(new DesignerActionHeaderItem("Extend Icon Presets"));
            items.Add(new DesignerActionMethodItem(this, "UseDropdownExtendIcon", "▼ Dropdown", "Extend Icon Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseMoreExtendIcon", "⋮ More Options", "Extend Icon Presets", false));

            return items;
        }
    }
}

