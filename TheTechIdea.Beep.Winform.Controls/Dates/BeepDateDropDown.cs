using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public class BeepDateDropDown :BaseControl
    {
        // All implementation is in partial classes:
        // - BeepDateDropDown.Core.cs: Core fields, events, initialization, and constructor
        // - BeepDateDropDown.Properties.cs: All properties (TreeStyle, Nodes, Selection, etc.)
        // - BeepDateDropDown.Events.cs: Event handlers (mouse, keyboard, context menu)
        // - BeepDateDropDown.Methods.cs: Public methods (node operations, search, etc.)
        // - BeepDateDropDown.Drawing.cs: DrawContent override and painting delegation
        // - BeepDateDropDown.Layout.cs: Layout calculations and visible node rebuilding
        // - BeepDateDropDown.Scrolling.cs: Scrollbar management and scrolling logic
        // - BeepDateDropDown.DateHandling.cs: Date selection and calendar logic
        // Component initialization is handled in BeepDateDropDown.Core.cs constructor
        // This control provides a dropdown calendar for date selection integrated with BaseControl features
        // It supports various date formats, min/max date constraints, and custom styling
        // it well use BaseControl features like theming, resizing, and event handling and HitTest
        // The dropdown can be triggered by clicking an icon or focusing the input area
        // Selected dates are displayed in a formatted text area
        // The control raises events for date changes and dropdown open/close actions
        // It is designed for easy integration into forms and applications requiring date input
        // It can be themed and customized to match application styles
        // it well supports keyboard navigation and accessibility features
        // it well use BeepDateTimePicker for date selection UI in Popup or DropDown
        // Different date formats can be set via properties
        // Different calendar views (month, year) can be toggled  via properties
        // Min and Max date constraints can be enforced via properties
        // The control can be resized and will adjust its layout accordingly
        // It well use Painter for Different styles like BeepTree and BeepGrid and image i will provide

    }
}
