using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// Represents a grid cell editor that encapsulates creation, setup,
    /// value access, and event handling for a specific editor type.
    /// </summary>
    public interface IGridEditor
    {
        /// <summary>
        /// Creates a new control instance for this editor type.
        /// </summary>
        Control CreateControl();

        /// <summary>
        /// Configures the control for editing a specific cell.
        /// </summary>
        /// <param name="control">The control returned by <see cref="CreateControl"/>.</param>
        /// <param name="column">The column configuration.</param>
        /// <param name="cell">The cell being edited.</param>
        /// <param name="theme">The current theme.</param>
        void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme);

        /// <summary>
        /// Sets the editor's initial value.
        /// </summary>
        void SetValue(Control control, object value);

        /// <summary>
        /// Gets the current value from the editor.
        /// </summary>
        object GetValue(Control control);

        /// <summary>
        /// Attaches editor-specific events (popups, etc.) to the control.
        /// </summary>
        void AttachEvents(Control control, IGridEditorEvents events);

        /// <summary>
        /// Detaches editor-specific events from the control.
        /// </summary>
        void DetachEvents(Control control, IGridEditorEvents events);

        /// <summary>
        /// Called after the editor has been focused. Use this to open popups, etc.
        /// </summary>
        void OnBeginEdit(Control control);

        /// <summary>
        /// Returns true if the editor currently has an open popup that should
        /// suppress the normal LostFocus => EndEdit behavior.
        /// </summary>
        bool IsPopupOpen(Control control);
    }

    /// <summary>
    /// Event callbacks that editors can invoke during their lifecycle.
    /// </summary>
    public interface IGridEditorEvents
    {
        void RequestEndEdit(bool commit);
        void RequestCancelEdit();
    }
}
