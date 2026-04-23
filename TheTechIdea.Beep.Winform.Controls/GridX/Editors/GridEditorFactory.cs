using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// Factory that resolves <see cref="IGridEditor"/> instances by <see cref="BeepColumnType"/>.
    /// Supports custom editor registration at runtime.
    /// </summary>
    public sealed class GridEditorFactory
    {
        private readonly Dictionary<BeepColumnType, IGridEditor> _editors = new();
        private IGridEditor _fallback;

        public GridEditorFactory()
        {
            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            _editors[BeepColumnType.Text] = new BeepGridTextEditor();
            _editors[BeepColumnType.ComboBox] = new BeepGridComboBoxEditor();
            _editors[BeepColumnType.DateTime] = new BeepGridDateDropDownEditor();
            _editors[BeepColumnType.NumericUpDown] = new BeepGridNumericEditor();
            _editors[BeepColumnType.CheckBoxBool] = new BeepGridCheckBoxEditor(BeepColumnType.CheckBoxBool);
            _editors[BeepColumnType.CheckBoxChar] = new BeepGridCheckBoxEditor(BeepColumnType.CheckBoxChar);
            _editors[BeepColumnType.CheckBoxString] = new BeepGridCheckBoxEditor(BeepColumnType.CheckBoxString);
            _editors[BeepColumnType.ListBox] = new BeepGridGenericEditor(BeepColumnType.ListBox);
            _editors[BeepColumnType.ListOfValue] = new BeepGridGenericEditor(BeepColumnType.ListOfValue);
            _editors[BeepColumnType.Image] = new BeepGridGenericEditor(BeepColumnType.Image);
            _editors[BeepColumnType.Button] = new BeepGridGenericEditor(BeepColumnType.Button);
            _editors[BeepColumnType.ProgressBar] = new BeepGridGenericEditor(BeepColumnType.ProgressBar);
            _editors[BeepColumnType.Radio] = new BeepGridGenericEditor(BeepColumnType.Radio);
            _editors[BeepColumnType.MaskedTextBox] = new BeepGridMaskedEditor();

            _fallback = new BeepGridTextEditor();
        }

        /// <summary>
        /// Registers or overrides an editor for a given column type.
        /// </summary>
        public void Register(BeepColumnType type, IGridEditor editor)
        {
            if (editor == null) throw new ArgumentNullException(nameof(editor));
            _editors[type] = editor;
        }

        /// <summary>
        /// Sets the fallback editor used when no specific registration exists.
        /// </summary>
        public void SetFallback(IGridEditor editor)
        {
            _fallback = editor ?? throw new ArgumentNullException(nameof(editor));
        }

        /// <summary>
        /// Resolves the <see cref="IGridEditor"/> for the given column type.
        /// </summary>
        public IGridEditor Resolve(BeepColumnType type)
        {
            if (_editors.TryGetValue(type, out var editor))
                return editor;
            return _fallback;
        }
    }
}
