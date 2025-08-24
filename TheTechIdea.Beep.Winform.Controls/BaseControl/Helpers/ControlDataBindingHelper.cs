using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal class ControlDataBindingHelper
    {
        private readonly BaseControl _owner;
        private readonly Dictionary<Binding, ConvertEventHandler> _formatHandlers = new();
        private readonly Dictionary<Binding, ConvertEventHandler> _parseHandlers = new();
        private readonly List<Binding> _originalBindings = new();

        public ControlDataBindingHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Data Context
        public object DataContext { get; set; }
        #endregion

        #region Binding Management
        public void SetBinding(string controlProperty, string dataSourceProperty)
        {
            if (DataContext == null)
                throw new InvalidOperationException("DataContext is not set.");

            var existingBinding = _owner.DataBindings[controlProperty];
            if (existingBinding != null)
            {
                _owner.DataBindings.Remove(existingBinding);
            }

            var binding = new Binding(controlProperty, DataContext, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };

            _owner.DataBindings.Add(binding);
            _owner.BoundProperty = controlProperty;
            _owner.DataSourceProperty = dataSourceProperty;
        }

        public void SetBinding(string controlProperty, object dataSource, string dataSourceProperty)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            var existingBinding = _owner.DataBindings[controlProperty];
            if (existingBinding != null)
            {
                _owner.DataBindings.Remove(existingBinding);
            }

            var binding = new Binding(controlProperty, dataSource, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };

            _owner.DataBindings.Add(binding);
            _owner.BoundProperty = controlProperty;
            _owner.DataSourceProperty = dataSourceProperty;
        }

        public void AddBinding(string controlProperty, object dataSource, string dataSourceProperty, 
            ConvertEventHandler formatHandler = null, 
            ConvertEventHandler parseHandler = null)
        {
            var binding = new Binding(controlProperty, dataSource, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };

            if (formatHandler != null)
            {
                binding.Format += formatHandler;
                _formatHandlers[binding] = formatHandler;
            }

            if (parseHandler != null)
            {
                binding.Parse += parseHandler;
                _parseHandlers[binding] = parseHandler;
            }

            _owner.DataBindings.Add(binding);
            _originalBindings.Add(binding);
        }

        public void RemoveBinding(string controlProperty)
        {
            var binding = _owner.DataBindings[controlProperty];
            if (binding != null)
            {
                // Remove event handlers
                if (_formatHandlers.TryGetValue(binding, out var formatHandler))
                {
                    binding.Format -= formatHandler;
                    _formatHandlers.Remove(binding);
                }

                if (_parseHandlers.TryGetValue(binding, out var parseHandler))
                {
                    binding.Parse -= parseHandler;
                    _parseHandlers.Remove(binding);
                }

                _owner.DataBindings.Remove(binding);
                _originalBindings.Remove(binding);
            }
        }

        public void ClearBindings()
        {
            foreach (var binding in _originalBindings.ToArray())
            {
                RemoveBinding(binding.PropertyName);
            }
        }
        #endregion

        #region Value Management
        public void SetValue(object value)
        {
            if (string.IsNullOrEmpty(_owner.BoundProperty)) return;

            var controlProperty = _owner.GetType().GetProperty(_owner.BoundProperty);
            controlProperty?.SetValue(_owner, value);

            if (DataContext != null && !string.IsNullOrEmpty(_owner.DataSourceProperty))
            {
                var dataSourceProperty = DataContext.GetType().GetProperty(_owner.DataSourceProperty);
                dataSourceProperty?.SetValue(DataContext, value);
            }
        }

        public object GetValue()
        {
            if (string.IsNullOrEmpty(_owner.BoundProperty))
            {
                return null;
            }

            var controlProperty = _owner.GetType().GetProperty(_owner.BoundProperty);
            return controlProperty?.GetValue(_owner);
        }

        public void ClearValue() => SetValue(null);

        public bool HasFilterValue() => !string.IsNullOrEmpty(_owner.BoundProperty) && GetValue() != null;

        public AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = _owner.BoundProperty,
                FilterValue = GetValue()?.ToString(),
                Operator = "=",
                valueType = "string"
            };
        }
        #endregion

        #region Validation
        public bool ValidateData(out string message)
        {
            var args = new BeepComponentEventArgs(_owner, _owner.BoundProperty, _owner.LinkedProperty, GetValue());
            
            // Create a method in owner to handle validation
            _owner.InvokePropertyValidate(args);

            if (args.Cancel)
            {
                message = args.Message;
                return false;
            }
            else
            {
                message = string.Empty;
                return true;
            }
        }
        #endregion

        #region Form Layout Management
        public void SuspendFormLayout()
        {
            if (_owner.Parent != null)
            {
                _owner.SuspendLayout();
            }
        }

        public void ResumeFormLayout()
        {
            if (_owner.Parent != null)
            {
                _owner.ResumeLayout();
                _owner.PerformLayout();
            }
        }
        #endregion

        #region Event Helpers
        public void RaiseValueChanged(object newValue)
        {
            _owner.InvokeOnValueChanged(new BeepComponentEventArgs(_owner, _owner.BoundProperty, _owner.LinkedProperty, newValue));
        }

        public void RaiseLinkedValueChanged(object newValue)
        {
            _owner.InvokeOnLinkedValueChanged(new BeepComponentEventArgs(_owner, _owner.BoundProperty, _owner.LinkedProperty, newValue));
        }

        public void RaisePropertyChanged(string propertyName, object newValue)
        {
            _owner.InvokePropertyChanged(new BeepComponentEventArgs(_owner, propertyName, _owner.LinkedProperty, newValue));
        }

        public void RaiseSubmitChanges()
        {
            _owner.InvokeSubmitChanges(new BeepComponentEventArgs(_owner, _owner.BoundProperty, _owner.LinkedProperty, GetValue()));
        }
        #endregion

        #region Cleanup
        public void Dispose()
        {
            ClearBindings();
            DataContext = null;
        }
        #endregion
    }
}