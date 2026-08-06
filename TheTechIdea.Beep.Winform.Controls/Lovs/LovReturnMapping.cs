using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Lovs
{
    /// <summary>
    /// A field of <see cref="SimpleItem"/> a LOV can return.
    /// </summary>
    /// <remarks>
    /// <see cref="SimpleItem"/> is the item type every list control in this library uses, and it already
    /// carries the columns a LOV needs to hand back. A developer maps their query's columns onto these
    /// fields when they build the list; the LOV reads them straight back out.
    /// </remarks>
    public enum LovField
    {
        /// <summary>The key — <c>SimpleItem.Value</c>.</summary>
        Value,

        /// <summary>The display text — <c>SimpleItem.Text</c>.</summary>
        Text,

        Name,
        Description,
        SubText,
        SubText2,
        SubText3,
        ImagePath,
        GuidId,
        ID,
    }

    /// <summary>
    /// One field the LOV returns into another control when a row is chosen.
    /// </summary>
    /// <remarks>
    /// Oracle Forms' <i>return items</i>. Picking a department returns its number into the LOV's own
    /// field and its location into a second one; without this the control could only ever fill itself,
    /// and a caller had to hand-wire the selection event and dig the values out.
    /// </remarks>
    public sealed class LovReturnMapping
    {
        public LovReturnMapping() { }

        public LovReturnMapping(LovField field, Control target)
        {
            Field = field;
            Target = target;
        }

        public LovReturnMapping(LovField field, Action<object?> assign)
        {
            Field = field;
            Assign = assign;
        }

        /// <summary>Which field of the chosen item to return.</summary>
        [Category("LOV")]
        [Description("Which SimpleItem field to return.")]
        [DefaultValue(LovField.Value)]
        public LovField Field { get; set; } = LovField.Value;

        /// <summary>
        /// Control that receives the value. A Beep control gets it through <c>SetValue</c>; anything
        /// else has its <c>Text</c> set.
        /// </summary>
        [Category("LOV")]
        [Description("Control that receives the value.")]
        public Control? Target { get; set; }

        /// <summary>
        /// Called with the value instead of writing to a control — for a view model, a variable, or a
        /// follow-on query.
        /// </summary>
        [Browsable(false)]
        public Action<object?>? Assign { get; set; }

        /// <summary>Whether this mapping has somewhere to put a value.</summary>
        public bool IsUsable => Target != null || Assign != null;

        /// <summary>Reads this mapping's field from <paramref name="item"/>.</summary>
        public object? Read(SimpleItem item) => item == null ? null : Field switch
        {
            LovField.Value       => item.Value,
            LovField.Text        => item.Text,
            LovField.Name        => item.Name,
            LovField.Description => item.Description,
            LovField.SubText     => item.SubText,
            LovField.SubText2    => item.SubText2,
            LovField.SubText3    => item.SubText3,
            LovField.ImagePath   => item.ImagePath,
            LovField.GuidId      => item.GuidId,
            LovField.ID          => item.ID,
            _                    => null,
        };

        public override string ToString() =>
            $"{Field} -> {(Assign != null ? "delegate" : Target?.Name ?? "(nothing)")}";
    }
}
