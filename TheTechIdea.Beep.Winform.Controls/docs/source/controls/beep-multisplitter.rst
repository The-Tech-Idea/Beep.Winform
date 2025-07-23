BeepMultiSplitter
=================

.. image:: /_static/images/beep-multisplitter-demo.svg
   :alt: BeepMultiSplitter demonstration
   :align: center
   :width: 600px

*Resizable multi-panel layout with dynamic row and column management*

Overview
--------

**BeepMultiSplitter** is a flexible layout control that allows you to create 
resizable multi-panel layouts with dynamic row and column management. It provides intuitive 
user interaction for resizing panels and a robust API for programmatically managing the layout.

Built on top of the ``TableLayoutPanel`` control, BeepMultiSplitter enhances it with 
interactive resizing capabilities, context menu support for adding/removing rows and columns, 
and drag-and-drop functionality for content reorganization.

Key Features
------------

.. grid:: 2

   .. grid-item-card:: ?? Interactive Resizing
      
      Resize rows and columns by dragging the borders between panels

   .. grid-item-card:: ?? Dynamic Layout
      
      Add or remove rows and columns at runtime through code or context menu

   .. grid-item-card:: ??? Drag and Drop Support
      
      Move content between panels with intuitive drag and drop operations

   .. grid-item-card:: ?? Responsive Design
      
      Automatically adjusts to container size changes with flexible sizing options

   .. grid-item-card:: ?? Context Menu Integration
      
      Built-in context menu for common layout operations

   .. grid-item-card:: ?? Theme Support
      
      Integrates with Beep theming system for consistent application styling

Basic Usage
-----------

Simple 2x2 Layout
~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Create the BeepMultiSplitter control
   BeepMultiSplitter splitter = new BeepMultiSplitter
   {
       Dock = DockStyle.Fill,
   };

   // Configure the initial layout (2x2)
   splitter.tableLayoutPanel.ColumnCount = 2;
   splitter.tableLayoutPanel.RowCount = 2;

   // Add content to the cells
   var panel1 = new BeepPanel { Dock = DockStyle.Fill };
   var panel2 = new BeepPanel { Dock = DockStyle.Fill };
   var panel3 = new BeepPanel { Dock = DockStyle.Fill };
   var panel4 = new BeepPanel { Dock = DockStyle.Fill };

   splitter.tableLayoutPanel.Controls.Add(panel1, 0, 0);
   splitter.tableLayoutPanel.Controls.Add(panel2, 1, 0);
   splitter.tableLayoutPanel.Controls.Add(panel3, 0, 1);
   splitter.tableLayoutPanel.Controls.Add(panel4, 1, 1);

   // Add to form or parent container
   this.Controls.Add(splitter);

.. tip::
   The BeepMultiSplitter exposes its internal TableLayoutPanel as a public property, making it easy to access all standard TableLayoutPanel functionality while gaining the additional resizing capabilities.

Properties
----------

Core Properties
~~~~~~~~~~~~~~~

.. list-table::
   :header-rows: 1
   :widths: 25 25 50

   * - Property
     - Type
     - Description
   * - ``tableLayoutPanel``
     - TableLayoutPanel
     - The underlying TableLayoutPanel that manages the layout of child controls

Inherited Properties
~~~~~~~~~~~~~~~~~~~~

As a subclass of ``BeepControl``, BeepMultiSplitter inherits all the standard BeepControl properties including:

* **Theme** (``string``) - The name of the theme to apply to this control
* **BorderRadius** (``int``) - Radius for rounded corners of the control
* **IsFrameless** (``bool``) - Whether the control has a visible frame
* **IsShadowAffectedByTheme** (``bool``) - Whether the shadow appearance is affected by the theme

.. note::
   You can also access and manipulate all standard TableLayoutPanel properties through the ``tableLayoutPanel`` property:
   
   * ``splitter.tableLayoutPanel.ColumnCount``
   * ``splitter.tableLayoutPanel.RowCount``
   * ``splitter.tableLayoutPanel.ColumnStyles``
   * ``splitter.tableLayoutPanel.RowStyles``
   * ``splitter.tableLayoutPanel.CellBorderStyle``

Methods
-------

Layout Management
~~~~~~~~~~~~~~~~~

.. list-table::
   :header-rows: 1
   :widths: 40 60

   * - Method
     - Description
   * - ``AddRow()``
     - Adds a new row to the layout
   * - ``RemoveRow()``
     - Removes the last row from the layout
   * - ``AddColumn()``
     - Adds a new column to the layout
   * - ``RemoveColumn()``
     - Removes the last column from the layout
   * - ``MoveColumn(int sourceIndex, int destinationIndex)``
     - Moves a column from one index to another

Adding Rows and Columns Programmatically
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Add a new row
   splitter.AddRow();

   // Add a new column
   splitter.AddColumn();

   // Remove the last row
   splitter.RemoveRow();

   // Remove the last column
   splitter.RemoveColumn();

Examples
--------

Creating a Code Editor with Preview Layout
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Create splitter with code editor and preview panels
   private void CreateCodeEditorLayout()
   {
       // Create the splitter control
       BeepMultiSplitter editorSplitter = new BeepMultiSplitter
       {
           Dock = DockStyle.Fill
       };

       // Configure as a 1x2 layout (side by side)
       editorSplitter.tableLayoutPanel.ColumnCount = 2;
       editorSplitter.tableLayoutPanel.RowCount = 1;
       
       // Set column widths (60% for editor, 40% for preview)
       editorSplitter.tableLayoutPanel.ColumnStyles.Clear();
       editorSplitter.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
       editorSplitter.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

       // Create the editor panel
       BeepPanel editorPanel = new BeepPanel
       {
           Dock = DockStyle.Fill,
           ShowTitle = true,
           TitleText = "Code Editor"
       };
       TextBox codeTextBox = new TextBox
       {
           Multiline = true,
           Dock = DockStyle.Fill,
           ScrollBars = ScrollBars.Both,
           Font = new Font("Consolas", 10)
       };
       editorPanel.Controls.Add(codeTextBox);

       // Create the preview panel
       BeepPanel previewPanel = new BeepPanel
       {
           Dock = DockStyle.Fill,
           ShowTitle = true,
           TitleText = "Preview"
       };
       
       // Add panels to the splitter
       editorSplitter.tableLayoutPanel.Controls.Add(editorPanel, 0, 0);
       editorSplitter.tableLayoutPanel.Controls.Add(previewPanel, 1, 0);

       // Add to form
       this.Controls.Add(editorSplitter);
   }

Creating a Dashboard Layout
~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Create a dashboard layout with multiple panels
   private void CreateDashboardLayout()
   {
       // Create the splitter control
       BeepMultiSplitter dashboardSplitter = new BeepMultiSplitter
       {
           Dock = DockStyle.Fill
       };

       // Configure as a 2x2 layout
       dashboardSplitter.tableLayoutPanel.ColumnCount = 2;
       dashboardSplitter.tableLayoutPanel.RowCount = 2;
       
       // Create metric panels
       BeepStatCard revenueCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Total Revenue",
           ValueText = "$12,456.78",
           PercentageText = "+8.5%",
           IsTrendingUp = true
       };
       
       BeepStatCard visitsCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Visitors",
           ValueText = "45,239",
           PercentageText = "+12.7%",
           IsTrendingUp = true
       };
       
       BeepStatCard conversionCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Conversion Rate",
           ValueText = "3.2%",
           PercentageText = "-0.8%",
           IsTrendingUp = false
       };
       
       BeepChart chart = new BeepChart
       {
           Dock = DockStyle.Fill
       };
       
       // Add panels to the splitter
       dashboardSplitter.tableLayoutPanel.Controls.Add(revenueCard, 0, 0);
       dashboardSplitter.tableLayoutPanel.Controls.Add(visitsCard, 1, 0);
       dashboardSplitter.tableLayoutPanel.Controls.Add(conversionCard, 0, 1);
       dashboardSplitter.tableLayoutPanel.Controls.Add(chart, 1, 1);

       // Add to form
       this.Controls.Add(dashboardSplitter);
   }

Theming
-------

The BeepMultiSplitter integrates with the Beep theming system to maintain a consistent look and feel across your application.

Applying Themes
~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Apply a theme to the splitter
   splitter.Theme = "DarkTheme";
   splitter.ApplyTheme();

   // Theme properties that affect BeepMultiSplitter:
   // - PanelBackColor: Background color of the splitter
   // - PanelForeColor: Text color of any labels within the splitter
   // - BorderColor: Color of the cell borders

.. tip::
   **Cell Border Styling**
   
   You can customize the appearance of cell borders using the standard TableLayoutPanel properties:
   
   .. code-block:: csharp
   
      splitter.tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
      // Other options: None, Inset, InsetDouble, Outset, OutsetDouble, OutsetPartial

Best Practices
--------------

Layout Design
~~~~~~~~~~~~~

* **Initial sizing:** Set appropriate initial sizes for rows and columns using RowStyles and ColumnStyles collections
* **Minimum sizes:** Consider setting minimum sizes for panels to prevent users from resizing them too small
* **Dock property:** Use Dock = DockStyle.Fill for child controls to ensure they fill their cells

Performance
~~~~~~~~~~~

* **Limit nested splitters:** While you can nest BeepMultiSplitter controls, limit the depth to avoid performance issues
* **SuspendLayout/ResumeLayout:** When adding multiple controls, use SuspendLayout() and ResumeLayout() to improve performance

User Experience
~~~~~~~~~~~~~~~

* **Consistent resizing:** Maintain consistent resize behavior across your application
* **Visual feedback:** Consider adding visual cues when the user is near resizable borders
* **Restore state:** Save and restore the splitter state between sessions for a better user experience

Suspending Layout During Mass Updates
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Improve performance during batch operations
   splitter.tableLayoutPanel.SuspendLayout();

   // Add multiple controls or make multiple changes
   for (int i = 0; i < 10; i++)
   {
       for (int j = 0; j < 5; j++)
       {
           var control = new Label { Text = $"Cell {i},{j}", Dock = DockStyle.Fill };
           splitter.tableLayoutPanel.Controls.Add(control, j, i);
       }
   }

   // Resume layout and refresh
   splitter.tableLayoutPanel.ResumeLayout(true);

.. warning::
   Be careful when removing rows or columns that contain important controls. Always ensure you're properly disposing of controls or relocating them before removing their containing rows or columns.

See Also
--------

* :doc:`beep-panel` - Basic panel control
* :doc:`beep-statcard` - Statistical data display
* :doc:`../api/beep-control-base` - Base control class