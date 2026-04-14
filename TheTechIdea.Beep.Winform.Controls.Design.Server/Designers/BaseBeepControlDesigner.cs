using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public interface IBeepDesignerActionHost
    {
        IComponent? Component { get; }
        void SetProperty(string propertyName, object value);
        T? GetProperty<T>(string propertyName);
        void ApplyTheme();
    }

    /// <summary>
    /// Base designer for all BeepControl controls
    /// Provides common design-time functionality: style selection, theme application, painter configuration
    /// </summary>
    public abstract class BaseBeepControlDesigner : Microsoft.DotNet.DesignTools.Designers.ControlDesigner, IBeepDesignerActionHost
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
}

