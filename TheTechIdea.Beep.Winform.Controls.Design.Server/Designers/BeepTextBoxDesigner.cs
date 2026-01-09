using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepTextBox control
    /// Provides smart tags for common configurations: search box, password field, autocomplete
    /// </summary>
    public class BeepTextBoxDesigner : BaseBeepControlDesigner
    {
        public BeepTextBox? TextBox => Component as BeepTextBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepTextBoxActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepTextBox smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepTextBoxActionList : DesignerActionList
    {
        private readonly BeepTextBoxDesigner _designer;

        public BeepTextBoxActionList(BeepTextBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepTextBox? TextBox => Component as BeepTextBox;

        #region Properties (for smart tags)

        [Category("Input")]
        [Description("The text content of the text box")]
        public string Text
        {
            get => _designer.GetProperty<string>("Text") ?? string.Empty;
            set => _designer.SetProperty("Text", value);
        }

        [Category("Input")]
        [Description("Placeholder text shown when textbox is empty")]
        public string PlaceholderText
        {
            get => _designer.GetProperty<string>("PlaceholderText") ?? string.Empty;
            set => _designer.SetProperty("PlaceholderText", value);
        }

        [Category("Input")]
        [Description("Maximum number of characters allowed")]
        public int MaxLength
        {
            get => _designer.GetProperty<int>("MaxLength");
            set => _designer.SetProperty("MaxLength", value);
        }

        [Category("Input")]
        [Description("Whether the text box is read-only")]
        public bool ReadOnly
        {
            get => _designer.GetProperty<bool>("ReadOnly");
            set => _designer.SetProperty("ReadOnly", value);
        }

        [Category("Password")]
        [Description("Character used to mask password input")]
        public char PasswordChar
        {
            get => _designer.GetProperty<char>("PasswordChar");
            set => _designer.SetProperty("PasswordChar", value);
        }

        [Category("Password")]
        [Description("Use system password character")]
        public bool UseSystemPasswordChar
        {
            get => _designer.GetProperty<bool>("UseSystemPasswordChar");
            set => _designer.SetProperty("UseSystemPasswordChar", value);
        }

        [Category("Autocomplete")]
        [Description("Autocomplete mode")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _designer.GetProperty<AutoCompleteMode>("AutoCompleteMode");
            set => _designer.SetProperty("AutoCompleteMode", value);
        }

        [Category("Search")]
        [Description("Enable search match highlighting")]
        public bool SearchHighlightEnabled
        {
            get => _designer.GetProperty<bool>("SearchHighlightEnabled");
            set => _designer.SetProperty("SearchHighlightEnabled", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure text box as a search box with search icon and placeholder
        /// </summary>
        public void ConfigureAsSearchBox()
        {
            PlaceholderText = "Search...";
            SearchHighlightEnabled = true;
            _designer.SetProperty("ControlStyle", BeepControlStyle.Modern);
        }

        /// <summary>
        /// Configure text box as a password field
        /// </summary>
        public void ConfigureAsPasswordField()
        {
            PasswordChar = '*';
            UseSystemPasswordChar = true;
            PlaceholderText = "Enter password...";
        }

        /// <summary>
        /// Enable autocomplete with suggest mode
        /// </summary>
        public void EnableAutocomplete()
        {
            AutoCompleteMode = AutoCompleteMode.Suggest;
            _designer.SetProperty("AutoCompleteSource", AutoCompleteSource.CustomSource);
        }

        /// <summary>
        /// Configure as email input field
        /// </summary>
        public void ConfigureAsEmailField()
        {
            PlaceholderText = "email@example.com";
            AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _designer.SetProperty("AutoCompleteSource", AutoCompleteSource.CustomSource);
        }

        /// <summary>
        /// Configure as number input field
        /// </summary>
        public void ConfigureAsNumberField()
        {
            PlaceholderText = "Enter number...";
            MaxLength = 20;
        }

        /// <summary>
        /// Configure as multiline text area
        /// </summary>
        public void ConfigureAsTextArea()
        {
            _designer.SetProperty("Multiline", true);
            _designer.SetProperty("AcceptsReturn", true);
            _designer.SetProperty("AcceptsTab", true);
            PlaceholderText = "Enter text...";
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick configuration presets
            items.Add(new DesignerActionHeaderItem("Quick Configuration"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSearchBox", "Configure as Search Box", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPasswordField", "Configure as Password Field", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "EnableAutocomplete", "Enable Autocomplete", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsEmailField", "Configure as Email Field", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsNumberField", "Configure as Number Field", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTextArea", "Configure as Text Area", "Quick Configuration", true));

            // Input properties
            items.Add(new DesignerActionHeaderItem("Input"));
            items.Add(new DesignerActionPropertyItem("Text", "Text", "Input"));
            items.Add(new DesignerActionPropertyItem("PlaceholderText", "Placeholder Text", "Input"));
            items.Add(new DesignerActionPropertyItem("MaxLength", "Max Length", "Input"));
            items.Add(new DesignerActionPropertyItem("ReadOnly", "Read Only", "Input"));

            // Password properties
            items.Add(new DesignerActionHeaderItem("Password"));
            items.Add(new DesignerActionPropertyItem("PasswordChar", "Password Character", "Password"));
            items.Add(new DesignerActionPropertyItem("UseSystemPasswordChar", "Use System Password Char", "Password"));

            // Autocomplete properties
            items.Add(new DesignerActionHeaderItem("Autocomplete"));
            items.Add(new DesignerActionPropertyItem("AutoCompleteMode", "Autocomplete Mode", "Autocomplete"));

            // Search properties
            items.Add(new DesignerActionHeaderItem("Search"));
            items.Add(new DesignerActionPropertyItem("SearchHighlightEnabled", "Enable Search Highlighting", "Search"));

            return items;
        }
    }
}
