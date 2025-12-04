using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Base designer for all BeepControl controls
    /// Provides common design-time functionality: style selection, theme application, painter configuration
    /// </summary>
    public abstract class BaseBeepControlDesigner : Microsoft.DotNet.DesignTools.Designers.ControlDesigner
    {
        protected IComponentChangeService? ChangeService;
        private DesignerActionListCollection? _actionLists;

        /// <summary>
        /// Gets the BeepControl being designed
        /// </summary>
        protected BaseControl? BeepControl => Component as BaseControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            ChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        /// <summary>
        /// Override to provide control-specific action lists
        /// </summary>
        protected abstract DesignerActionListCollection GetControlSpecificActionLists();

        /// <summary>
        /// Combines common actions with control-specific actions
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    
                    // Add common actions for all Beep controls
                    _actionLists.Add(new CommonBeepControlActionList(this));
                    
                    // Add control-specific actions
                    var specific = GetControlSpecificActionLists();
                    if (specific != null)
                    {
                        _actionLists.AddRange(specific);
                    }
                }
                return _actionLists;
            }
        }

        #region Common Helper Methods

        /// <summary>
        /// Set a property value with change notification
        /// </summary>
        public void SetProperty(string propertyName, object value)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null) return;

            var currentValue = property.GetValue(Component);
            if (Equals(currentValue, value)) return;

            ChangeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            ChangeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        /// <summary>
        /// Get a property value
        /// </summary>
        public T? GetProperty<T>(string propertyName)
        {
            if (Component == null) return default;

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null) return default;

            var value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        /// <summary>
        /// Apply theme to control
        /// </summary>
        public void ApplyTheme()
        {
            BeepControl?.ApplyTheme();
        }

        /// <summary>
        /// Set control style
        /// </summary>
        public void SetStyle(BeepControlStyle style)
        {
            SetProperty("ControlStyle", style);
        }

        #endregion
    }

    /// <summary>
    /// Common action list for all BeepControl controls
    /// Provides Style, Theme, and Painter actions
    /// </summary>
    public class CommonBeepControlActionList : DesignerActionList
    {
        private readonly BaseBeepControlDesigner _designer;

        public CommonBeepControlActionList(BaseBeepControlDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BaseControl? BeepControl => Component as BaseControl;

        #region Common Properties (for smart tags)

        [Category("Style")]
        [Description("The visual style of the control")]
        public BeepControlStyle ControlStyle
        {
            get => _designer.GetProperty<BeepControlStyle>("ControlStyle");
            set => _designer.SetProperty("ControlStyle", value);
        }

        [Category("Theme")]
        [Description("Use theme colors instead of style colors")]
        public bool UseThemeColors
        {
            get => _designer.GetProperty<bool>("UseThemeColors");
            set => _designer.SetProperty("UseThemeColors", value);
        }

        [Category("Painting")]
        [Description("Use the BeepStyling system for painting")]
        public bool UseFormStylePaint
        {
            get => _designer.GetProperty<bool>("UseFormStylePaint");
            set => _designer.SetProperty("UseFormStylePaint", value);
        }

        #endregion

        #region Common Actions

        public void ApplyTheme()
        {
            _designer.ApplyTheme();
        }

        public void SelectStyle()
        {
            // Open style selector dialog (to be implemented)
            var dialog = new StyleSelectorDialog(ControlStyle);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ControlStyle = dialog.SelectedStyle;
            }
        }

        // Quick style presets
        public void SetStyleToMaterial3() => ControlStyle = BeepControlStyle.Material3;
        public void SetStyleToiOS15() => ControlStyle = BeepControlStyle.iOS15;
        public void SetStyleToFluent2() => ControlStyle = BeepControlStyle.Fluent2;
        public void SetStyleToMinimal() => ControlStyle = BeepControlStyle.Minimal;
        public void SetStyleToBrutalist() => ControlStyle = BeepControlStyle.Brutalist;
        public void SetStyleToNeumorphism() => ControlStyle = BeepControlStyle.Neumorphism;

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Style selection
            items.Add(new DesignerActionHeaderItem("Visual Style"));
            items.Add(new DesignerActionMethodItem(this, "SelectStyle", "Select Style...", "Visual Style", true));
            
            // Quick style presets (most popular)
            items.Add(new DesignerActionMethodItem(this, "SetStyleToMaterial3", "Material 3", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToiOS15", "iOS 15", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToFluent2", "Fluent 2", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToMinimal", "Minimal", "Visual Style", false));

            // Theme
            items.Add(new DesignerActionHeaderItem("Theme"));
            items.Add(new DesignerActionMethodItem(this, "ApplyTheme", "Apply Current Theme", "Theme", true));
            items.Add(new DesignerActionPropertyItem("UseThemeColors", "Use Theme Colors", "Theme"));

            // Painting
            items.Add(new DesignerActionHeaderItem("Painting"));
            items.Add(new DesignerActionPropertyItem("UseFormStylePaint", "Use Style Painting", "Painting"));

            return items;
        }
    }
}

