using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Bridges a UnitOfWork instance to BeepGridPro without taking a generic dependency.
    /// - Reads the Units property via reflection
    /// - Binds it to the grid data and navigator
    /// - Subscribes to list change notifications to keep the grid refreshed
    /// </summary>
    internal class GridUnitOfWorkBinder
    {
        private readonly BeepGridPro _grid;
        private object _uow;
        private object _units;

        private IBindingList _asBindingList;
        private INotifyCollectionChanged _asNotifyCollectionChanged;

        // Keep event subscriptions so we can detach
        private readonly Dictionary<string, Delegate> _uowEventHandlers = new();

        public GridUnitOfWorkBinder(BeepGridPro grid) { _grid = grid; }

        public void Attach(object unitOfWork)
        {
            Detach();
            _uow = unitOfWork;
            if (_uow == null) return;
            RefreshBinding();
            SubscribeUowEvents();
        }

        public void Detach()
        {
            // Unhook Units change listeners
            if (_asBindingList != null)
            {
                _asBindingList.ListChanged -= OnListChanged;
                _asBindingList = null;
            }
            if (_asNotifyCollectionChanged != null)
            {
                _asNotifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                _asNotifyCollectionChanged = null;
            }

            // Unsubscribe UoW events
            if (_uow != null)
            {
                var t = _uow.GetType();
                foreach (var kv in _uowEventHandlers)
                {
                    var ev = t.GetEvent(kv.Key);
                    if (ev != null)
                    {
                        try { ev.RemoveEventHandler(_uow, kv.Value); } catch { }
                    }
                }
            }
            _uowEventHandlers.Clear();

            _units = null;
            _uow = null;
        }

        private void RefreshBinding()
        {
            _units = GetUnits(_uow);
            if (_units == null) return;

            // Feed grid data and navigator
            _grid.Data.Bind(_units);
            _grid.Navigator.BindTo(_units);

            // Hook change notifications to keep the view live
            _asBindingList = _units as IBindingList;
            if (_asBindingList != null)
            {
                _asBindingList.ListChanged += OnListChanged;
            }
            _asNotifyCollectionChanged = _units as INotifyCollectionChanged;
            if (_asNotifyCollectionChanged != null)
            {
                _asNotifyCollectionChanged.CollectionChanged += OnCollectionChanged;
            }

            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            // Recalc on add/remove/reset; for item changes, just refresh
            if (e.ListChangedType == ListChangedType.ItemAdded ||
                e.ListChangedType == ListChangedType.ItemDeleted ||
                e.ListChangedType == ListChangedType.Reset)
            {
                _grid.Layout.Recalculate();
            }
            _grid.SafeInvalidate();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();
        }

        private static object GetUnits(object uow)
        {
            if (uow == null) return null;
            var prop = uow.GetType().GetProperty("Units");
            return prop?.GetValue(uow);
        }

        private void SubscribeUowEvents()
        {
            // Names from IUnitofWork<T>
            string[] names = new[]
            {
                "PreDelete","PreInsert","PreCreate","PreUpdate","PreQuery",
                "PostQuery","PostInsert","PostCreate","PostUpdate","PostEdit",
                "PostDelete","PostCommit","PreCommit"
            };
            var t = _uow.GetType();
            foreach (var name in names)
            {
                var ev = t.GetEvent(name);
                if (ev == null) continue;

                var handler = BuildEventHandler(ev.EventHandlerType, name);
                if (handler != null)
                {
                    try
                    {
                        ev.AddEventHandler(_uow, handler);
                        _uowEventHandlers[name] = handler;
                    }
                    catch { }
                }
            }
        }

        private Delegate BuildEventHandler(Type eventHandlerType, string eventName)
        {
            // event signature is (object sender, UnitofWorkParams e)
            var invoke = eventHandlerType.GetMethod("Invoke");
            var parameters = invoke.GetParameters();
            if (parameters.Length != 2) return null;

            var senderParam = Expression.Parameter(parameters[0].ParameterType, "sender");
            var argsParam = Expression.Parameter(parameters[1].ParameterType, "e");

            var call = Expression.Call(
                Expression.Constant(this),
                typeof(GridUnitOfWorkBinder).GetMethod(nameof(HandleUowEvent), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic),
                Expression.Convert(senderParam, typeof(object)),
                Expression.Convert(argsParam, typeof(object)),
                Expression.Constant(eventName)
            );

            var lambda = Expression.Lambda(eventHandlerType, call, senderParam, argsParam);
            return lambda.Compile();
        }

        private void HandleUowEvent(object sender, object e, string eventName)
        {
            switch (eventName)
            {
                case "PreQuery":
                case "PreInsert":
                case "PreUpdate":
                case "PreDelete":
                case "PreCommit":
                    // no-op visual hint now; could show busy indicator
                    break;
                case "PostQuery":
                    // Units might be replaced; rebind
                    RefreshBinding();
                    break;
                case "PostInsert":
                case "PostUpdate":
                case "PostEdit":
                case "PostDelete":
                    _grid.Layout.Recalculate();
                    _grid.SafeInvalidate();
                    break;
                case "PostCommit":
                    _grid.SafeInvalidate();
                    break;
            }
        }
    }
}
