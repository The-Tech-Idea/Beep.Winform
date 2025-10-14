using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateTimePicker :BaseControl
    {
        // All BeepDateTimePicker is in partial classes:
        // - BeepDateTimePicker.Core.cs: Core fields, events, initialization, and constructor
        // - BeepDateTimePicker.Properties.cs: All properties (TreeStyle, Nodes, Selection, etc.)
        // - BeepDateTimePicker.Events.cs: Event handlers (mouse, keyboard, context menu)
        // - BeepDateTimePicker.Methods.cs: Public methods (node operations, search, etc.)
        // - BeepDateTimePicker.Drawing.cs: DrawContent override and painting delegation
        // - BeepDateTimePicker.Layout.cs: Layout calculations and visible node rebuilding
        // - BeepDateTimePicker.Scrolling.cs: Scrollbar management and scrolling logic
        // - BeepDateTimePicker.DateHandling.cs: Date selection and calendar logic
        // - BeepDateTimePicker.TimeHandling.cs: Time selection and time picker logic
        // Component initialization is handled in BeepDateTimePicker.Core.cs constructor
        // It supports various date formats, min/max date constraints, and custom styling
        // it well use BaseControl features like theming, resizing, and event handling and HitTest
        // Selected dates are displayed in a formatted text area
        // The control raises events for date changes and dropdown open/close actions
        // It is designed for easy integration into forms and applications requiring date input
        // It can be themed and customized to match application styles
        // it well supports keyboard navigation and accessibility feature
        // Different date formats can be set via properties
        // Different calendar views (month, year) can be toggled  via properties
        // Min and Max date constraints can be enforced via properties
        // The control can be resized and will adjust its layout accordingly
        // It well use Painter for Different styles like BeepTree and BeepGrid and image i will provide
    }
}
