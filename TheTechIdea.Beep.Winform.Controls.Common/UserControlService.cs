using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Desktop.Common
{

    public interface IUserControlService
    {
        void Register<T>(string key, T control) where T : UserControl;
        T GetControl<T>(string key) where T : UserControl;
        bool TryGetControl<T>(string key, out T control) where T : UserControl;
        void RemoveControl<T>(string key) where T : UserControl;
        void DisposeAll();
    }

    public class UserControlService : IUserControlService, IDisposable
    {
        private readonly Dictionary<(Type, string), UserControl> _controls = new();

        public void Register<T>(string key, T control) where T : UserControl
        {
            var controlKey = (typeof(T), key);

            // Dispose old control if different instance
            if (_controls.TryGetValue(controlKey, out var oldControl))
            {
                if (!ReferenceEquals(oldControl, control))
                {
                    oldControl.Dispose();
                }
            }

            _controls[controlKey] = control;
        }

        public T GetControl<T>(string key) where T : UserControl
        {
            var controlKey = (typeof(T), key);

            if (_controls.TryGetValue(controlKey, out var control))
            {
                return (T)control;
            }

            // Auto-create using default constructor
            var newControl = Activator.CreateInstance<T>();
            _controls[controlKey] = newControl;
            return newControl;
        }

        public bool TryGetControl<T>(string key, out T control) where T : UserControl
        {
            var controlKey = (typeof(T), key);

            if (_controls.TryGetValue(controlKey, out var existing))
            {
                control = (T)existing;
                return true;
            }

            control = null;
            return false;
        }

        public void RemoveControl<T>(string key) where T : UserControl
        {
            var controlKey = (typeof(T), key);

            if (_controls.TryGetValue(controlKey, out var control))
            {
                control.Dispose();
                _controls.Remove(controlKey);
            }
        }

        public void DisposeAll()
        {
            foreach (var control in _controls.Values)
            {
                control.Dispose();
            }
            _controls.Clear();
        }

        public void Dispose() => DisposeAll();
    }


}
