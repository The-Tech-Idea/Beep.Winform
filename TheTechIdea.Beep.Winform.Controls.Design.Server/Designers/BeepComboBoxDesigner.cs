using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepComboBox control
    /// </summary>
    public class BeepComboBoxDesigner : BaseBeepControlDesigner
    {
        public BeepComboBox? ComboBox => Component as BeepComboBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepComboBoxActionList(this));
            return lists;
        }
    }

    public class BeepComboBoxActionList : DesignerActionList
    {
        private readonly BeepComboBoxDesigner _designer;

        public BeepComboBoxActionList(BeepComboBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public bool IsEditable
        {
            get => _designer.GetProperty<bool>("IsEditable");
            set => _designer.SetProperty("IsEditable", value);
        }

        [Category("Behavior")]
        public bool MultiSelect
        {
            get => _designer.GetProperty<bool>("AllowMultipleSelection");
            set => _designer.SetProperty("AllowMultipleSelection", value);
        }

        [Category("Appearance")]
        [Description("The visual combo-box shell and popup composition.")]
        public ComboBoxType ComboBoxType
        {
            get => _designer.GetProperty<ComboBoxType>("ComboBoxType");
            set => _designer.SetProperty("ComboBoxType", value);
        }

        [Category("Appearance")]
        [Description("Placeholder text shown when no item is selected.")]
        public string PlaceholderText
        {
            get => _designer.GetProperty<string>("PlaceholderText") ?? string.Empty;
            set => _designer.SetProperty("PlaceholderText", value);
        }

        [Category("Behavior")]
        [Description("Show a search box inside the dropdown popup.")]
        public bool ShowSearchInDropdown
        {
            get => _designer.GetProperty<bool>("ShowSearchInDropdown");
            set => _designer.SetProperty("ShowSearchInDropdown", value);
        }

        [Category("Behavior")]
        [Description("Enable auto-complete while typing.")]
        public bool AutoComplete
        {
            get => _designer.GetProperty<bool>("AutoComplete");
            set => _designer.SetProperty("AutoComplete", value);
        }

        [Category("Behavior")]
        [Description("Auto-complete matching strategy.")]
        public BeepAutoCompleteMode AutoCompleteMode
        {
            get => _designer.GetProperty<BeepAutoCompleteMode>("AutoCompleteMode");
            set => _designer.SetProperty("AutoCompleteMode", value);
        }

        [Category("Behavior")]
        [Description("Minimum typed characters before auto-complete runs.")]
        public int AutoCompleteMinLength
        {
            get => _designer.GetProperty<int>("AutoCompleteMinLength");
            set => _designer.SetProperty("AutoCompleteMinLength", value);
        }

        [Category("Behavior")]
        [Description("Maximum number of suggested matches.")]
        public int MaxSuggestions
        {
            get => _designer.GetProperty<int>("MaxSuggestions");
            set => _designer.SetProperty("MaxSuggestions", value);
        }

        [Category("Behavior")]
        [Description("Allow unmatched free-text tokens when editing is enabled.")]
        public bool AllowFreeText
        {
            get => _designer.GetProperty<bool>("AllowFreeText");
            set => _designer.SetProperty("AllowFreeText", value);
        }

        [Category("Behavior")]
        [Description("Show loading state instead of allowing selection interaction.")]
        public bool IsLoading
        {
            get => _designer.GetProperty<bool>("IsLoading");
            set => _designer.SetProperty("IsLoading", value);
        }

        #endregion

        #region Presets

        public void ConfigureAsSearchablePicker()
        {
            ComboBoxType = ComboBoxType.OutlineSearchable;
            ShowSearchInDropdown = true;
            AutoComplete = true;
            AutoCompleteMode = BeepAutoCompleteMode.Prefix;
            AutoCompleteMinLength = 1;
            MaxSuggestions = 10;
            IsEditable = true;
        }

        public void ConfigureAsChipMultiSelect()
        {
            ComboBoxType = ComboBoxType.MultiChipSearch;
            MultiSelect = true;
            ShowSearchInDropdown = true;
            IsEditable = true;
            AllowFreeText = false;
        }

        public void ConfigureAsCommandMenu()
        {
            ComboBoxType = ComboBoxType.CommandMenu;
            ShowSearchInDropdown = true;
            AutoComplete = true;
            AutoCompleteMode = BeepAutoCompleteMode.Fuzzy;
            PlaceholderText = "Run command...";
            IsEditable = true;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsSearchablePicker), "Searchable Picker", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsChipMultiSelect), "Chip Multi-Select", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsCommandMenu), "Command Menu", "Presets", true));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(ComboBoxType), "Combo Box Type", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(PlaceholderText), "Placeholder Text", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("IsEditable", "Editable", "Behavior"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowSearchInDropdown), "Show Search In Dropdown", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoComplete), "Auto Complete", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoCompleteMode), "Auto Complete Mode", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoCompleteMinLength), "Auto Complete Min Length", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(MaxSuggestions), "Max Suggestions", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowFreeText), "Allow Free Text", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(IsLoading), "Is Loading", "Behavior"));

            return items;
        }
    }
}

