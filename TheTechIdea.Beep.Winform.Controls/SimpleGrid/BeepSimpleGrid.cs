using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSimpleGrid - A comprehensive WinForms data grid control with advanced features
    /// 
    /// This is a modular control split across multiple partial classes for better maintainability:
    /// - BeepSimpleGrid.Properties.cs - All fields and properties
    /// - BeepSimpleGrid.DataManagement.cs - Data source and BindingSource management
    /// - BeepSimpleGrid.Columns.cs - Column operations and configuration
    /// - BeepSimpleGrid.Rows.cs - Row management and navigation
    /// - BeepSimpleGrid.Rendering.Core.cs - Core rendering infrastructure
    /// - BeepSimpleGrid.Rendering.Panels.cs - Panel rendering (headers, footer, etc.)
    /// - BeepSimpleGrid.Rendering.Cells.cs - Cell and row content rendering
    /// - BeepSimpleGrid.Editing.cs - Cell editing functionality
    /// - BeepSimpleGrid.Navigation.cs - Navigation controls and paging
    /// - BeepSimpleGrid.ChangeTracking.cs - Change tracking and state management
    /// - BeepSimpleGrid.FilterSort.cs - Filtering and sorting operations
    /// - BeepSimpleGrid.MouseEvents.cs - Mouse interaction and events
    /// - BeepSimpleGrid.Theme.cs - Theme application
    /// - BeepSimpleGrid.Scrolling.cs - Scrollbar management and smooth scrolling
    /// - BeepSimpleGrid.Helpers.cs - Helper methods and utilities
    /// - BeepSimpleGrid.Events.cs - Event declarations
    /// - BeepSimpleGrid.Initialization.cs - Constructor and initialization
    /// </summary>
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data in a simple table format with scrollbars, filtering, sorting, and advanced editing capabilities.")]
    [DisplayName("Beep Simple Grid")]
    public partial class BeepSimpleGrid : BaseControl
    {
        // All implementation is in the partial class files listed above
    }
}
