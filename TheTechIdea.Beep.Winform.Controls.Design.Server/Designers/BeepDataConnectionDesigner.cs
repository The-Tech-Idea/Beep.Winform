using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Component designer for <see cref="BeepDataConnection"/>.
    /// Attaches a design-time <see cref="TheTechIdea.Beep.Container.Services.IBeepService"/> lease
    /// (sourced from <see cref="DesignTimeBeepServiceManager"/>) so the tray component
    /// populates its <c>DataConnections</c> list from the live BeepService configuration
    /// while the form is open in Visual Studio.
    /// </summary>
    public sealed class BeepDataConnectionDesigner : ComponentDesigner
    {
        private IDesignTimeServiceLease? _serviceLease;
        private BeepDataConnection? _dataConnection;
        private IComponentChangeService? _changeService;
        private DesignerActionListCollection? _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            _dataConnection = component as BeepDataConnection;
            if (_dataConnection == null)
            {
                return;
            }

            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            try
            {
                _serviceLease = DesignTimeBeepServiceManager.Acquire(_dataConnection);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[BeepDataConnectionDesigner] Lease acquisition failed: {ex.GetType().Name}: {ex.Message}");
                _serviceLease?.Dispose();
                _serviceLease = null;
            }

            if (_serviceLease != null)
            {
                try
                {
                    _dataConnection.AttachSharedBeepService(_serviceLease.BeepService, reloadConnections: true);

                    PropertyDescriptor? connectionsProperty = TypeDescriptor.GetProperties(_dataConnection)["DataConnections"];
                    if (connectionsProperty != null)
                    {
                        _changeService?.OnComponentChanged(_dataConnection, connectionsProperty, null, _dataConnection.DataConnections);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[BeepDataConnectionDesigner] Shared service attach failed: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _actionLists = null;
                _serviceLease?.Dispose();
                _serviceLease = null;
                _dataConnection = null;
                _changeService = null;
            }

            base.Dispose(disposing);
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepDataConnectionActionList(this)
            };

        internal IDMEEditor? GetEditor()
            => _serviceLease?.BeepService?.DMEEditor;

        internal T? GetProperty<T>(string propertyName)
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

        internal void SetProperty(string propertyName, object value)
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

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, currentValue, value);
        }
    }
}
