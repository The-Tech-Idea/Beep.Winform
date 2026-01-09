using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepTabs control
    /// </summary>
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection
                    {
                        new BeepTabsActionList(this)
                    };
                }
                return _actionLists;
            }
        }

        /// <summary>
        /// Helper method to get a property value from the component
        /// </summary>
        public T GetProperty<T>(string propertyName)
        {
            if (Component == null)
                return default(T);

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property != null)
            {
                var value = property.GetValue(Component);
                if (value is T typedValue)
                    return typedValue;
            }
            return default(T);
        }

        /// <summary>
        /// Helper method to set a property value on the component
        /// </summary>
        public void SetProperty(string propertyName, object value)
        {
            if (Component == null)
                return;

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property != null && !property.IsReadOnly)
            {
                var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                var oldValue = property.GetValue(Component);
                
                changeService?.OnComponentChanging(Component, property);
                property.SetValue(Component, value);
                changeService?.OnComponentChanged(Component, property, oldValue, value);
            }
        }
    }

    /// <summary>
    /// Action list for BeepTabs smart tags
    /// Provides quick access to common tab properties and style presets
    /// </summary>
    public class BeepTabsActionList : DesignerActionList
    {
        private readonly BeepTabsDesigner _designer;

        public BeepTabsActionList(BeepTabsDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepTabs? Tabs => Component as BeepTabs;

        #region Properties

        [Category("Tabs")]
        [Description("Visual style of the tabs")]
        public TabStyle TabStyle
        {
            get => _designer.GetProperty<TabStyle>("TabStyle");
            set => _designer.SetProperty("TabStyle", value);
        }

        [Category("Appearance")]
        [Description("Header height in pixels")]
        public int HeaderHeight
        {
            get => _designer.GetProperty<int>("HeaderHeight");
            set => _designer.SetProperty("HeaderHeight", value);
        }

        [Category("Appearance")]
        [Description("Position of tab headers")]
        public TabHeaderPosition HeaderPosition
        {
            get => _designer.GetProperty<TabHeaderPosition>("HeaderPosition");
            set => _designer.SetProperty("HeaderPosition", value);
        }

        [Category("Behavior")]
        [Description("Show close buttons on tabs")]
        public bool ShowCloseButtons
        {
            get => _designer.GetProperty<bool>("ShowCloseButtons");
            set => _designer.SetProperty("ShowCloseButtons", value);
        }

        [Category("Appearance")]
        [Description("Theme name")]
        public string Theme
        {
            get => _designer.GetProperty<string>("Theme");
            set => _designer.SetProperty("Theme", value);
        }

        #endregion

        #region Actions

        public void ApplyClassicStyle()
        {
            TabStyle = TabStyle.Classic;
        }

        public void ApplyUnderlineStyle()
        {
            TabStyle = TabStyle.Underline;
        }

        public void ApplyCapsuleStyle()
        {
            TabStyle = TabStyle.Capsule;
        }

        public void ApplyCardStyle()
        {
            TabStyle = TabStyle.Card;
        }

        public void ApplyMinimalStyle()
        {
            TabStyle = TabStyle.Minimal;
        }

        public void UseRecommendedHeaderHeight()
        {
            if (Tabs != null)
            {
                var style = Tabs.TabStyle;
                HeaderHeight = TabStyleHelpers.GetRecommendedHeaderHeight(style);
            }
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Tabs"));
            items.Add(new DesignerActionPropertyItem("TabStyle", "Tab Style:", "Tabs"));
            items.Add(new DesignerActionPropertyItem("ShowCloseButtons", "Show Close Buttons:", "Tabs"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("HeaderHeight", "Header Height:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("HeaderPosition", "Header Position:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("Theme", "Theme:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyClassicStyle", "Classic Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyUnderlineStyle", "Underline Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyCapsuleStyle", "Capsule Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyCardStyle", "Card Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalStyle", "Minimal Style", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, "UseRecommendedHeaderHeight", "Use Recommended Header Height", "Layout", true));

            return items;
        }
    }
}
