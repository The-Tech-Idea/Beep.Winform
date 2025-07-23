Migration Guide
===============

This guide helps you migrate from standard WinForms controls to Beep Controls, or upgrade from previous versions of Beep Controls.

Migrating from Standard WinForms Controls
------------------------------------------

Overview
~~~~~~~~

Beep Controls are designed to be drop-in replacements for standard WinForms controls with enhanced functionality. The migration process is straightforward but requires some adjustments for optimal results.

Control Mapping
~~~~~~~~~~~~~~~

.. list-table::
   :header-rows: 1
   :widths: 30 30 40

   * - Standard Control
     - Beep Control
     - Key Differences
   * - ``Button``
     - ``BeepButton``
     - React-style variants, theming, SVG icons
   * - ``TextBox``
     - ``BeepTextBox``
     - Placeholder text, validation, floating labels
   * - ``Label``
     - ``BeepLabel``
     - Theming, enhanced typography
   * - ``Panel``
     - ``BeepPanel``
     - Title bars, theming, enhanced borders
   * - ``ProgressBar``
     - ``BeepProgressBar``
     - Percentage display, theming, custom colors
   * - ``ComboBox``
     - ``BeepComboBox``
     - Enhanced styling, theming, React variants

Step-by-Step Migration
~~~~~~~~~~~~~~~~~~~~~~

**Step 1: Replace Using Statements**

.. code-block:: csharp

   // Before
   using System.Windows.Forms;

   // After
   using System.Windows.Forms;
   using TheTechIdea.Beep.Winform.Controls;

**Step 2: Replace Control Declarations**

.. code-block:: csharp

   // Before
   private Button myButton;
   private TextBox myTextBox;
   private Panel myPanel;

   // After
   private BeepButton myButton;
   private BeepTextBox myTextBox;
   private BeepPanel myPanel;

**Step 3: Update Control Initialization**

.. code-block:: csharp

   // Before
   myButton = new Button
   {
       Text = "Click Me",
       Size = new Size(100, 30),
       BackColor = Color.Blue,
       ForeColor = Color.White
   };

   // After
   myButton = new BeepButton
   {
       Text = "Click Me",
       Size = new Size(100, 30),
       UIVariant = ReactUIVariant.Contained,
       UIColor = ReactUIColor.Primary,
       Theme = "DefaultTheme"
   };

**Step 4: Apply Themes**

.. code-block:: csharp

   // Apply theme to all Beep controls
   private void ApplyThemeToForm()
   {
       ApplyThemeToControls(this.Controls, "DefaultTheme");
   }

   private void ApplyThemeToControls(Control.ControlCollection controls, string theme)
   {
       foreach (Control control in controls)
       {
           if (control is BeepControl beepControl)
           {
               beepControl.Theme = theme;
               beepControl.ApplyTheme();
           }
           
           if (control.HasChildren)
           {
               ApplyThemeToControls(control.Controls, theme);
           }
       }
   }

Common Migration Scenarios
~~~~~~~~~~~~~~~~~~~~~~~~~~

**Migrating a Login Form**

.. code-block:: csharp

   // Before - Standard WinForms
   public partial class LoginForm : Form
   {
       private TextBox usernameTextBox;
       private TextBox passwordTextBox;
       private Button loginButton;
       private Label titleLabel;

       private void InitializeControls()
       {
           usernameTextBox = new TextBox
           {
               Location = new Point(50, 50),
               Size = new Size(200, 25)
           };

           passwordTextBox = new TextBox
           {
               Location = new Point(50, 90),
               Size = new Size(200, 25),
               UseSystemPasswordChar = true
           };

           loginButton = new Button
           {
               Text = "Login",
               Location = new Point(50, 130),
               Size = new Size(200, 35),
               BackColor = Color.DodgerBlue,
               ForeColor = Color.White
           };

           titleLabel = new Label
           {
               Text = "User Login",
               Location = new Point(50, 20),
               Font = new Font("Arial", 12, FontStyle.Bold)
           };
       }
   }

   // After - Beep Controls
   public partial class LoginForm : Form
   {
       private BeepTextBox usernameTextBox;
       private BeepTextBox passwordTextBox;
       private BeepButton loginButton;
       private BeepLabel titleLabel;

       private void InitializeControls()
       {
           usernameTextBox = new BeepTextBox
           {
               Location = new Point(50, 50),
               Size = new Size(200, 25),
               PlaceholderText = "Enter username",
               Theme = "DefaultTheme"
           };

           passwordTextBox = new BeepTextBox
           {
               Location = new Point(50, 90),
               Size = new Size(200, 25),
               UseSystemPasswordChar = true,
               PlaceholderText = "Enter password",
               Theme = "DefaultTheme"
           };

           loginButton = new BeepButton
           {
               Text = "Login",
               Location = new Point(50, 130),
               Size = new Size(200, 35),
               UIVariant = ReactUIVariant.Contained,
               UIColor = ReactUIColor.Primary,
               Theme = "DefaultTheme"
           };

           titleLabel = new BeepLabel
           {
               Text = "User Login",
               Location = new Point(50, 20),
               Font = new Font("Segoe UI", 12, FontStyle.Bold),
               Theme = "DefaultTheme"
           };

           // Apply themes
           ApplyThemeToForm();
       }
   }

**Migrating Data Entry Forms**

.. code-block:: csharp

   // Enhanced with Beep Controls
   private void CreateDataEntryForm()
   {
       var mainPanel = new BeepPanel
       {
           Dock = DockStyle.Fill,
           ShowTitle = true,
           TitleText = "Customer Information",
           Theme = "DefaultTheme"
       };

       var nameTextBox = new BeepTextBox
       {
           PlaceholderText = "Customer Name",
           Size = new Size(250, 30),
           Location = new Point(20, 60),
           Theme = "DefaultTheme"
       };

       var emailTextBox = new BeepTextBox
       {
           PlaceholderText = "Email Address",
           Size = new Size(250, 30),
           Location = new Point(20, 100),
           Theme = "DefaultTheme"
       };

       var saveButton = new BeepButton
       {
           Text = "Save Customer",
           Size = new Size(120, 35),
           Location = new Point(20, 150),
           UIVariant = ReactUIVariant.Contained,
           UIColor = ReactUIColor.Success,
           Theme = "DefaultTheme"
       };

       mainPanel.Controls.AddRange(new Control[] { nameTextBox, emailTextBox, saveButton });
       this.Controls.Add(mainPanel);
   }

Upgrading from Previous Versions
---------------------------------

Version 1.0.x to 1.0.164
~~~~~~~~~~~~~~~~~~~~~~~~~

**Breaking Changes:**

* Theme property names have been standardized
* Some event handlers have been updated for consistency
* SVG support has been enhanced with new properties

**Migration Steps:**

1. **Update Package Reference**

   .. code-block:: xml

      <!-- Update your .csproj file -->
      <PackageReference Include="TheTechIdea.Beep.Winform.Controls" Version="1.0.164" />

2. **Update Theme Names**

   .. code-block:: csharp

      // Before
      control.Theme = "Dark";
      
      // After
      control.Theme = "DarkTheme";

3. **Update Event Handling**

   .. code-block:: csharp

      // Before
      button.Click += Button_Click;
      
      // After (unchanged, but verify event signatures)
      button.Click += Button_Click;

**New Features in 1.0.164:**

* Enhanced SVG support with theme-aware coloring
* Improved React-style property system
* Better performance with layout controls
* Additional built-in themes

Performance Considerations
--------------------------

Theme Application
~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Efficient theme application
   this.SuspendLayout();
   
   foreach (Control control in this.Controls)
   {
       if (control is BeepControl beepControl)
       {
           beepControl.Theme = themeName;
           beepControl.ApplyTheme();
       }
   }
   
   this.ResumeLayout(true);

Memory Management
~~~~~~~~~~~~~~~~~

.. code-block:: csharp

   // Proper disposal of Beep controls
   protected override void Dispose(bool disposing)
   {
       if (disposing)
       {
           // Beep controls handle their own disposal
           // Just ensure you're not holding references
       }
       base.Dispose(disposing);
   }

Testing Your Migration
-----------------------

Checklist
~~~~~~~~~

- [ ] All controls display correctly
- [ ] Themes apply properly
- [ ] Event handling works as expected
- [ ] Performance is acceptable
- [ ] No memory leaks
- [ ] Responsive behavior is maintained

Common Issues and Solutions
---------------------------

**Issue: Controls not appearing correctly**

*Solution:* Ensure you're calling ``ApplyTheme()`` after setting theme properties.

**Issue: Poor performance with many controls**

*Solution:* Use ``SuspendLayout()`` and ``ResumeLayout()`` when adding multiple controls.

**Issue: Theme not applying to nested controls**

*Solution:* Implement recursive theme application as shown in the examples.

**Issue: Designer issues in Visual Studio**

*Solution:* Build the project first, then use the designer. Some properties may need to be set in code.

Support and Resources
---------------------

If you encounter issues during migration:

* Check the :doc:`../examples/basic-examples` for working code samples
* Review the :doc:`../guides/best-practices` guide
* Consult individual control documentation in the :doc:`../controls/index` section
* Report issues on the project's GitHub repository

Next Steps
----------

After successful migration:

* Explore the :doc:`theming` system for customization
* Learn about :doc:`../guides/best-practices` for optimal usage
* Try :doc:`../examples/advanced-examples` for inspiration