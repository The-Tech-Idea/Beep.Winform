using TheTechIdea.Beep.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.DataBase;
using System.Drawing.Design;
using System.Collections;
using System.Reflection;


namespace TheTechIdea.Beep.Winform.Controls.DataNavigator
{
    public enum BeepBindingSourceMode
    {
        ObservableBindingList,
        UnitofWok, ViewModel, None
    }
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    [DefaultProperty(nameof(DataSource))]
    [DefaultEvent(nameof(CurrentChanged))]
    [ComplexBindingProperties(nameof(DataSource), nameof(DataMember))]
    public class BeepBindingSource : Component
    {
        private object _dataSource;
        private string _dataMember;
        private IBindingList _list;
        private int _position = -1;
        private BeepBindingSourceMode _mode = BeepBindingSourceMode.None;

        public event EventHandler DataSourceChanged;
        public event EventHandler DataMemberChanged;
        public event EventHandler CurrentChanged;
        public event EventHandler PositionChanged;
        public event ListChangedEventHandler ListChanged;

        public BeepBindingSource()
        {
            _list = new BindingList<object>(); // Default empty list
        }

        /// <summary>
        /// Allows selecting a DataSource from the Designer
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    ResolveDataSource();
                    DataSourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Allows selecting a DataMember in the Designer
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public string DataMember
        {
            get => _dataMember;
            set
            {
                if (_dataMember != value)
                {
                    _dataMember = value;
                    ResolveDataSource();
                    DataMemberChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int Position
        {
            get => _position;
            set
            {
                if (_position != value && value >= 0 && value < _list.Count)
                {
                    _position = value;
                    PositionChanged?.Invoke(this, EventArgs.Empty);
                    CurrentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public object Current => (_position >= 0 && _position < _list.Count) ? _list[_position] : null;

        private void ResolveDataSource()
        {
            _mode = BeepBindingSourceMode.None;
            _list = new BindingList<object>();
            _position = -1;

            if (_dataSource == null)
                return;

            if (_dataSource is IBindingList bindingList)
            {
                _mode = BeepBindingSourceMode.ObservableBindingList;
                _list = bindingList;
            }
            else if (_dataSource is IEnumerable enumerable)
            {
                _mode = BeepBindingSourceMode.ObservableBindingList;
                _list = new BindingList<object>(new List<object>(enumerable.Cast<object>()));
            }
            else if (_dataSource.GetType().IsClass && !string.IsNullOrEmpty(_dataMember))
            {
                PropertyInfo prop = _dataSource.GetType().GetProperty(_dataMember);
                if (prop != null && prop.GetValue(_dataSource) is IBindingList innerList)
                {
                    _mode = BeepBindingSourceMode.ViewModel;
                    _list = innerList;
                }
            }

            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void MoveNext() => Position = Math.Min(Position + 1, _list.Count - 1);
        public void MovePrevious() => Position = Math.Max(Position - 1, 0);
        public void MoveFirst() => Position = 0;
        public void MoveLast() => Position = _list.Count - 1;

        public object AddNew()
        {
            if (_list is IBindingList bindingList && bindingList.AllowNew)
            {
                object newItem = bindingList.AddNew();
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, _list.Count - 1));
                return newItem;
            }
            throw new InvalidOperationException("AddNew is not supported on this data source.");
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _list.Count)
            {
                _list.RemoveAt(index);
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            }
        }

        public void Clear()
        {
            _list.Clear();
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }

   

}
