using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Events for BeepChipListBox
    /// </summary>
    public partial class BeepChipListBox
    {
        #region Events

        /// <summary>
        /// Occurs when the selection changes in either the list or chip group
        /// </summary>
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the selection changes in either the list or chip group")]
        public event EventHandler<ChipListBoxSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Occurs when the search text changes
        /// </summary>
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the search text changes")]
        public event EventHandler<ChipListBoxSearchEventArgs> SearchTextChanged;

        /// <summary>
        /// Occurs when a chip is removed via its close button
        /// </summary>
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when a chip is removed via its close button")]
        public event EventHandler<SimpleItem> ChipRemoved;

        /// <summary>
        /// Occurs when an item is clicked in the list
        /// </summary>
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when an item is clicked in the list")]
        public event EventHandler<SimpleItem> ItemClicked;

        /// <summary>
        /// Occurs when an item's checkbox state changes
        /// </summary>
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when an item's checkbox state changes")]
        public event EventHandler<ItemCheckedEventArgs> ItemChecked;

        #endregion

        #region Event Raisers

        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        protected virtual void OnSelectionChanged(SimpleItem currentItem, IReadOnlyList<SimpleItem> selectedItems, SelectionChangeSource source)
        {
            SelectionChanged?.Invoke(this, new ChipListBoxSelectionChangedEventArgs(
                currentItem,
                selectedItems,
                source
            ));
        }

        /// <summary>
        /// Raises the SearchTextChanged event
        /// </summary>
        protected virtual void OnSearchTextChanged()
        {
            SearchTextChanged?.Invoke(this, new ChipListBoxSearchEventArgs(_searchBox?.Text ?? ""));
        }

        /// <summary>
        /// Raises the ChipRemoved event
        /// </summary>
        protected virtual void OnChipRemoved(SimpleItem item)
        {
            ChipRemoved?.Invoke(this, item);
        }

        /// <summary>
        /// Raises the ItemClicked event
        /// </summary>
        protected virtual void OnItemClicked(SimpleItem item)
        {
            ItemClicked?.Invoke(this, item);
        }

        /// <summary>
        /// Raises the ItemChecked event
        /// </summary>
        protected virtual void OnItemChecked(SimpleItem item, bool isChecked)
        {
            ItemChecked?.Invoke(this, new ItemCheckedEventArgs(item, isChecked));
        }

        #endregion

        #region Keyboard Navigation

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle keyboard shortcuts
            switch (keyData)
            {
                case Keys.Control | Keys.F:
                    // Focus search box
                    if (_showSearch && _searchBox != null)
                    {
                        _searchBox.Focus();
                        return true;
                    }
                    break;

                case Keys.Escape:
                    // Clear search if focused
                    if (_searchBox != null && _searchBox.Focused)
                    {
                        _searchBox.Text = "";
                        _listBox?.Focus();
                        return true;
                    }
                    break;

                case Keys.Control | Keys.A:
                    // Select all if multi-select is enabled
                    if (_allowMultiSelect && _listBox != null)
                    {
                        SelectAll();
                        return true;
                    }
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Focus Management

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // Focus the search box by default if visible
            if (_showSearch && _searchBox != null && _searchBox.Visible)
            {
                _searchBox.Focus();
            }
            else if (_showList && _listBox != null && _listBox.Visible)
            {
                _listBox.Focus();
            }
            else if (_showChips && _chipGroup != null && _chipGroup.Visible)
            {
                _chipGroup.Focus();
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // Forward mouse wheel to the appropriate child control
            if (_listPanel != null && _listPanel.Bounds.Contains(e.Location))
            {
                // Let the list handle it
            }
            else if (_chipPanel != null && _chipPanel.Bounds.Contains(e.Location))
            {
                // Let the chip group handle it
            }
        }

        #endregion
    }

    #region Additional Event Args

    /// <summary>
    /// Event arguments for item checked state changes
    /// </summary>
    public class ItemCheckedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public bool IsChecked { get; }

        public ItemCheckedEventArgs(SimpleItem item, bool isChecked)
        {
            Item = item;
            IsChecked = isChecked;
        }
    }

    #endregion
}

