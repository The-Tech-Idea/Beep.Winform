using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Toggle;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepToggle control
    /// Similar to BeepSwitch but with toggle-specific features
    /// </summary>
    public class BeepToggleDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepToggle? Toggle => Component as BeepToggle;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Select Icon...", OnSelectIcon),
                        new DesignerVerb("Clear Icon", OnClearIcon),
                        new DesignerVerb("Toggle State", OnToggle)
                    };
                }
                return _verbs;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepToggleActionList(this));
            return lists;
        }

        private void OnSelectIcon(object? sender, EventArgs e)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)["IconName"];
            if (property == null) return;

            var currentValue = property.GetValue(Component) as string;
            
            using (var dialog = new Editors.IconPickerDialog(currentValue))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProperty("IconName", dialog.SelectedIconPath);
                }
            }
        }

        private void OnClearIcon(object? sender, EventArgs e)
        {
            SetProperty("IconName", string.Empty);
        }

        private void OnToggle(object? sender, EventArgs e)
        {
            if (Toggle != null)
            {
                var currentValue = GetProperty<object>("Value");
                // Toggle implementation depends on value type
            }
        }
    }

    /// <summary>
    /// Action list for BeepToggle smart tags
    /// </summary>
    public class BeepToggleActionList : DesignerActionList
    {
        private readonly BeepToggleDesigner _designer;

        public BeepToggleActionList(BeepToggleDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Appearance")]
        public string LabelText
        {
            get => _designer.GetProperty<string>("LabelText") ?? "";
            set => _designer.SetProperty("LabelText", value);
        }

        #endregion

        #region Actions

        public void SelectIcon()
        {
            _designer.Verbs[0].Invoke();
        }

        public void UseCheckmarkIcon()
        {
            _designer.SetProperty("IconName", TheTechIdea.Beep.Icons.SvgsUI.Check);
        }

        public void UseStarIcon()
        {
            _designer.SetProperty("IconName", TheTechIdea.Beep.Icons.SvgsUI.Star);
        }

        public void UseHeartIcon()
        {
            _designer.SetProperty("IconName", TheTechIdea.Beep.Icons.SvgsUI.Heart);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("LabelText", "Label Text", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Icon"));
            items.Add(new DesignerActionMethodItem(this, "SelectIcon", "Select Icon...", "Icon", true));
            items.Add(new DesignerActionMethodItem(this, "UseCheckmarkIcon", "✓ Checkmark", "Icon", false));
            items.Add(new DesignerActionMethodItem(this, "UseStarIcon", "⭐ Star", "Icon", false));
            items.Add(new DesignerActionMethodItem(this, "UseHeartIcon", "❤️ Heart", "Icon", false));

            return items;
        }
    }
}

