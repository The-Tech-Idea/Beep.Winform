using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor
{
    /// <summary>
    /// Reuses <see cref="BeepTextBox"/>, <see cref="BeepDateTimePicker"/>,
    /// <see cref="BeepComboBox"/> and <see cref="BeepCheckBoxBool"/> instances
    /// across editor sessions so we don't allocate a new WinForms control
    /// every time a user double-clicks an event.
    ///
    /// The pool is per-<see cref="CalendarEditorHost"/> and stores free
    /// instances in a per-type stack. When a descriptor's factory needs a
    /// control, it calls <see cref="Acquire(Type)"/>; when the editor is
    /// torn down, it calls <see cref="Release(Control)"/>.
    /// </summary>
    public sealed class CalendarEditorPool
    {
        private readonly Dictionary<Type, Stack<Control>> _free = new();

        public Control Acquire(Type controlType)
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            if (!typeof(Control).IsAssignableFrom(controlType))
                throw new ArgumentException($"{controlType.Name} is not a Control.", nameof(controlType));

            if (_free.TryGetValue(controlType, out var stack) && stack.Count > 0)
            {
                return stack.Pop();
            }
            return (Control)Activator.CreateInstance(controlType);
        }

        public void Release(Control control)
        {
            if (control == null) return;
            var type = control.GetType();
            if (!_free.TryGetValue(type, out var stack))
            {
                stack = new Stack<Control>();
                _free[type] = stack;
            }
            if (stack.Count < 16)
            {
                control.Visible = false;
                stack.Push(control);
            }
            else
            {
                control.Dispose();
            }
        }

        public void DisposeAll()
        {
            foreach (var stack in _free.Values)
            {
                while (stack.Count > 0)
                {
                    try { stack.Pop().Dispose(); } catch { }
                }
            }
            _free.Clear();
        }
    }
}
