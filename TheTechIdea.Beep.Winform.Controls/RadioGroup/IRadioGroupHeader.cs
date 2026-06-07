using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    /// <summary>
    /// Marker interface. Implement on a <see cref="SimpleItem"/>-derived class
    /// (or assign an instance of this interface to <see cref="SimpleItem.Tag"/>)
    /// to mark the item as a non-interactive section header. Renderers draw
    /// headers as section labels and the hit-tester skips them.
    /// </summary>
    public interface IRadioGroupHeader
    {
    }

    /// <summary>
    /// Convenience adapter: any <see cref="SimpleItem"/> carrying an
    /// <see cref="IRadioGroupHeader"/>-implementing object in <see cref="SimpleItem.Tag"/>
    /// is treated as a section header.
    /// </summary>
    public static class RadioGroupHeaderExtensions
    {
        public static bool IsHeader(this SimpleItem item)
        {
            if (item == null) return false;
            if (item.Tag is IRadioGroupHeader) return true;
            return false;
        }
    }
}
