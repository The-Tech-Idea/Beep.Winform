using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Shared base designer for Beep parent/container controls that still need the common
    /// style and theme smart-tag surface.
    /// </summary>
    public abstract class BaseBeepParentControlDesigner : ParentControlDesigner, IBeepDesignerActionHost
    {
        protected IComponentChangeService? ChangeService;
        private DesignerActionListCollection? _actionLists;

        protected BaseControl? BeepControl => Component as BaseControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            ChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        protected abstract DesignerActionListCollection GetControlSpecificActionLists();

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection
                    {
                        new CommonBeepControlActionList(this)
                    };

                    DesignerActionListCollection specific = GetControlSpecificActionLists();
                    if (specific != null)
                    {
                        _actionLists.AddRange(specific);
                    }
                }

                return _actionLists;
            }
        }

        public void SetProperty(string propertyName, object value)
        {
            if (Component == null)
            {
                return;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            object? currentValue = property.GetValue(Component);
            if (Equals(currentValue, value))
            {
                return;
            }

            ChangeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            ChangeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        public T? GetProperty<T>(string propertyName)
        {
            if (Component == null)
            {
                return default;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return default;
            }

            object? value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        public void ApplyTheme()
        {
            BeepControl?.ApplyTheme();
        }
    }
}