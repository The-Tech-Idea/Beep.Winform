using System;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Abstraction for the content panel hosted inside a popup form.
    /// Each <see cref="ComboBoxType"/> variant creates a content panel
    /// with its own layout, row rendering, and interaction behavior.
    /// Implementations must also be a <see cref="System.Windows.Forms.Control"/>.
    /// </summary>
    internal interface IPopupContentPanel
    {
        void ApplyProfile(ComboBoxPopupHostProfile profile);
        void ApplyThemeTokens(ComboBoxThemeTokens tokens);
        void UpdateModel(ComboBoxPopupModel model);
        void UpdateSearchText(string text);
        void SetKeyboardFocusIndex(int index);
        void FocusSearchBox();
        void FocusItem(SimpleItem item);

        event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        event EventHandler ApplyClicked;
        event EventHandler CancelClicked;
    }
}
