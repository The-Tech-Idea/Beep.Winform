#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// A collection of BeepTreeColumn objects with change notifications.
    /// </summary>
    [Editor(typeof(BeepTreeColumnCollectionEditor), typeof(UITypeEditor))]
    public class BeepTreeColumnCollection : IList<BeepTreeColumn>, ICollection<BeepTreeColumn>, IEnumerable<BeepTreeColumn>, IList
    {
        private readonly List<BeepTreeColumn> _columns = new List<BeepTreeColumn>();
        private BeepTree _owner;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event EventHandler CollectionChanged;

        /// <summary>
        /// Initializes a new instance of the BeepTreeColumnCollection class.
        /// </summary>
        public BeepTreeColumnCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance with an owner tree.
        /// </summary>
        internal BeepTreeColumnCollection(BeepTree owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets or sets the owner tree control.
        /// </summary>
        internal BeepTree Owner
        {
            get => _owner;
            set => _owner = value;
        }

        /// <summary>
        /// Gets the number of columns in the collection.
        /// </summary>
        public int Count => _columns.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the column at the specified index.
        /// </summary>
        public BeepTreeColumn this[int index]
        {
            get
            {
                if (index < 0 || index >= _columns.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _columns[index];
            }
            set
            {
                if (index < 0 || index >= _columns.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _columns[index] = value;
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        public BeepTreeColumn this[string name]
        {
            get
            {
                foreach (var col in _columns)
                {
                    if (col.Name == name)
                        return col;
                }
                return null;
            }
        }

        /// <summary>
        /// Adds a column to the collection.
        /// </summary>
        public void Add(BeepTreeColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            _columns.Add(column);
            OnCollectionChanged();
        }

        /// <summary>
        /// Adds a new column with the specified name and caption.
        /// </summary>
        public BeepTreeColumn Add(string name, string caption, int width = 100)
        {
            var column = new BeepTreeColumn
            {
                Name = name,
                Caption = caption,
                FieldName = name,
                Width = width
            };
            Add(column);
            return column;
        }

        /// <summary>
        /// Inserts a column at the specified index.
        /// </summary>
        public void Insert(int index, BeepTreeColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            _columns.Insert(index, column);
            OnCollectionChanged();
        }

        /// <summary>
        /// Removes the specified column from the collection.
        /// </summary>
        public bool Remove(BeepTreeColumn column)
        {
            if (column == null)
                return false;
            bool removed = _columns.Remove(column);
            if (removed)
                OnCollectionChanged();
            return removed;
        }

        /// <summary>
        /// Removes the column at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _columns.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _columns.RemoveAt(index);
            OnCollectionChanged();
        }

        /// <summary>
        /// Removes the column with the specified name.
        /// </summary>
        public void Remove(string name)
        {
            var column = this[name];
            if (column != null)
                Remove(column);
        }

        /// <summary>
        /// Clears all columns from the collection.
        /// </summary>
        public void Clear()
        {
            _columns.Clear();
            OnCollectionChanged();
        }

        /// <summary>
        /// Determines whether the collection contains the specified column.
        /// </summary>
        public bool Contains(BeepTreeColumn column)
        {
            return _columns.Contains(column);
        }

        /// <summary>
        /// Determines whether the collection contains a column with the specified name.
        /// </summary>
        public bool Contains(string name)
        {
            return this[name] != null;
        }

        /// <summary>
        /// Copies the columns to an array.
        /// </summary>
        public void CopyTo(BeepTreeColumn[] array, int arrayIndex)
        {
            _columns.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the index of the specified column.
        /// </summary>
        public int IndexOf(BeepTreeColumn column)
        {
            return _columns.IndexOf(column);
        }

        /// <summary>
        /// Gets all visible columns in display order.
        /// </summary>
        public IEnumerable<BeepTreeColumn> GetVisibleColumns()
        {
            foreach (var col in _columns)
            {
                if (col.Visible)
                    yield return col;
            }
        }

        /// <summary>
        /// Gets all fixed (pinned) columns.
        /// </summary>
        public IEnumerable<BeepTreeColumn> GetFixedColumns()
        {
            foreach (var col in _columns)
            {
                if (col.IsFixed && col.Visible)
                    yield return col;
            }
        }

        /// <summary>
        /// Gets the total width of all visible columns.
        /// </summary>
        public int GetTotalVisibleWidth()
        {
            int total = 0;
            foreach (var col in GetVisibleColumns())
                total += col.Width;
            return total;
        }

        /// <summary>
        /// Gets the total width of all fixed columns.
        /// </summary>
        public int GetTotalFixedWidth()
        {
            int total = 0;
            foreach (var col in GetFixedColumns())
                total += col.Width;
            return total;
        }

        /// <summary>
        /// Moves a column to a new index.
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _columns.Count)
                throw new ArgumentOutOfRangeException(nameof(oldIndex));
            if (newIndex < 0 || newIndex >= _columns.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            var column = _columns[oldIndex];
            _columns.RemoveAt(oldIndex);
            _columns.Insert(newIndex, column);
            OnCollectionChanged();
        }

        /// <summary>
        /// Raises the CollectionChanged event.
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            CollectionChanged?.Invoke(this, EventArgs.Empty);
            _owner?.Invalidate();
        }

        #region IEnumerable Implementation

        public IEnumerator<BeepTreeColumn> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        #endregion

        #region IList Implementation (for designer support)

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is BeepTreeColumn column)
                    this[index] = column;
                else
                    throw new ArgumentException("Value must be a BeepTreeColumn", nameof(value));
            }
        }

        bool IList.IsFixedSize => false;

        int IList.Add(object value)
        {
            if (value is BeepTreeColumn column)
            {
                Add(column);
                return _columns.Count - 1;
            }
            throw new ArgumentException("Value must be a BeepTreeColumn", nameof(value));
        }

        bool IList.Contains(object value)
        {
            if (value is BeepTreeColumn column)
                return Contains(column);
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is BeepTreeColumn column)
                return IndexOf(column);
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (value is BeepTreeColumn column)
                Insert(index, column);
            else
                throw new ArgumentException("Value must be a BeepTreeColumn", nameof(value));
        }

        void IList.Remove(object value)
        {
            if (value is BeepTreeColumn column)
                Remove(column);
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => ((ICollection)_columns).SyncRoot;

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_columns).CopyTo(array, index);
        }

        #endregion
    }

    /// <summary>
    /// Simple collection editor for BeepTreeColumnCollection (placeholder for full implementation).
    /// </summary>
    public class BeepTreeColumnCollectionEditor : CollectionEditor
    {
        public BeepTreeColumnCollectionEditor(Type type) : base(type)
        {
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(BeepTreeColumn);
        }
    }
}
