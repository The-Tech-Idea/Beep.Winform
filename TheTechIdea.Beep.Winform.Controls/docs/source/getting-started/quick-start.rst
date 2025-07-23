Quick Start Guide
=================

This guide will help you create your first application using Beep Controls in just a few minutes.

Prerequisites
-------------

Before you begin, make sure you have:

* Completed the :doc:`installation` process
* A Windows Forms project targeting .NET 6.0+ with Windows support
* Basic knowledge of C# and Windows Forms

Creating Your First Beep Control
---------------------------------

Let's start by creating a simple form with a few Beep controls to demonstrate the basic functionality.

Step 1: Create a New Form
~~~~~~~~~~~~~~~~~~~~~~~~~~

Create a new Windows Forms application or add a new form to your existing project:

.. code-block:: csharp

   using System;
   using System.Drawing;
   using System.Windows.Forms;
   using TheTechIdea.Beep.Winform.Controls;

   namespace BeepControlsDemo
   {
       public partial class MainForm : Form
       {
           public MainForm()
           {
               InitializeComponent();
               SetupForm();
               CreateBeepControls();
           }

           private void SetupForm()
           {
               this.Text = "Beep Controls Demo";
               this.Size = new Size(800, 600);
               this.StartPosition = FormStartPosition.CenterScreen;
           }
       }
   }

Step 2: Add Beep Controls
~~~~~~~~~~~~~~~~~~~~~~~~~~

Now let's add some Beep controls to demonstrate their capabilities:

.. code-block:: csharp

   private void CreateBeepControls()
   {
       // Create a BeepPanel as a container
       var mainPanel = new BeepPanel
       {
           Dock = DockStyle.Fill,
           ShowTitle = true,
           TitleText = "Beep Controls Demo",
           Padding = new Padding(20)
       };
       this.Controls.Add(mainPanel);

       // Add a BeepButton
       var beepButton = new BeepButton
       {
           Text = "Click Me!",
           Size = new Size(150, 40),
           Location = new Point(20, 60),
           UIVariant = ReactUIVariant.Contained,
           UIColor = ReactUIColor.Primary
       };
       beepButton.Click += BeepButton_Click;
       mainPanel.Controls.Add(beepButton);

       // Add a BeepTextBox
       var beepTextBox = new BeepTextBox
       {
           PlaceholderText = "Enter some text...",
           Size = new Size(200, 30),
           Location = new Point(20, 120)
       };
       mainPanel.Controls.Add(beepTextBox);

       // Add a BeepLabel
       var beepLabel = new BeepLabel
       {
           Text = "Welcome to Beep Controls!",
           AutoSize = true,
           Location = new Point(20, 170),
           Font = new Font("Segoe UI", 12, FontStyle.Bold)
       };
       mainPanel.Controls.Add(beepLabel);

       // Add a BeepProgressBar
       var progressBar = new BeepProgressBar
       {
           Size = new Size(300, 20),
           Location = new Point(20, 210),
           Value = 75,
           ShowPercentage = true
       };
       mainPanel.Controls.Add(progressBar);
   }

   private void BeepButton_Click(object sender, EventArgs e)
   {
       MessageBox.Show("Hello from Beep Controls!", "Button Clicked", 
                      MessageBoxButtons.OK, MessageBoxIcon.Information);
   }

Step 3: Apply Theming
~~~~~~~~~~~~~~~~~~~~~

One of the powerful features of Beep Controls is the theming system. Let's add theme switching:

.. code-block:: csharp

   private void CreateThemingControls()
   {
       // Create theme selection buttons
       var lightThemeButton = new BeepButton
       {
           Text = "Light Theme",
           Size = new Size(100, 30),
           Location = new Point(20, 250),
           UIVariant = ReactUIVariant.Outlined
       };
       lightThemeButton.Click += (s, e) => ApplyTheme("LightTheme");

       var darkThemeButton = new BeepButton
       {
           Text = "Dark Theme",
           Size = new Size(100, 30),
           Location = new Point(130, 250),
           UIVariant = ReactUIVariant.Outlined
       };
       darkThemeButton.Click += (s, e) => ApplyTheme("DarkTheme");

       // Add to the main panel
       var mainPanel = this.Controls.OfType<BeepPanel>().First();
       mainPanel.Controls.Add(lightThemeButton);
       mainPanel.Controls.Add(darkThemeButton);
   }

   private void ApplyTheme(string themeName)
   {
       // Apply theme to all Beep controls
       ApplyThemeToControls(this.Controls, themeName);
   }

   private void ApplyThemeToControls(Control.ControlCollection controls, string themeName)
   {
       foreach (Control control in controls)
       {
           if (control is BeepControl beepControl)
           {
               beepControl.Theme = themeName;
               beepControl.ApplyTheme();
           }
           
           // Recursively apply to child controls
           if (control.HasChildren)
           {
               ApplyThemeToControls(control.Controls, themeName);
           }
       }
   }

Advanced Example: Dashboard Layout
----------------------------------

Let's create a more advanced example that demonstrates multiple controls working together:

.. code-block:: csharp

   private void CreateDashboardExample()
   {
       // Create a splitter for layout
       var splitter = new BeepMultiSplitter
       {
           Dock = DockStyle.Fill
       };
       
       // Configure 2x2 layout
       splitter.tableLayoutPanel.ColumnCount = 2;
       splitter.tableLayoutPanel.RowCount = 2;
       
       // Create stat cards
       var revenueCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Revenue",
           ValueText = "$12,345",
           PercentageText = "+8.3%",
           IsTrendingUp = true,
           Margin = new Padding(5)
       };
       
       var usersCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Active Users",
           ValueText = "1,234",
           PercentageText = "+12.1%",
           IsTrendingUp = true,
           Margin = new Padding(5)
       };
       
       var ordersCard = new BeepStatCard
       {
           Dock = DockStyle.Fill,
           HeaderText = "Orders",
           ValueText = "987",
           PercentageText = "-2.4%",
           IsTrendingUp = false,
           Margin = new Padding(5)
       };
       
       // Create a panel for controls
       var controlsPanel = new BeepPanel
       {
           Dock = DockStyle.Fill,
           ShowTitle = true,
           TitleText = "Controls",
           Margin = new Padding(5)
       };
       
       // Add some interactive controls to the panel
       var toggleSwitch = new BeepSwitch
       {
           Text = "Enable Notifications",
           Location = new Point(20, 50),
           Checked = true
       };
       
       var slider = new BeepProgressBar
       {
           Size = new Size(200, 20),
           Location = new Point(20, 90),
           Value = 60,
           ShowPercentage = true
       };
       
       controlsPanel.Controls.Add(toggleSwitch);
       controlsPanel.Controls.Add(slider);
       
       // Add all panels to the splitter
       splitter.tableLayoutPanel.Controls.Add(revenueCard, 0, 0);
       splitter.tableLayoutPanel.Controls.Add(usersCard, 1, 0);
       splitter.tableLayoutPanel.Controls.Add(ordersCard, 0, 1);
       splitter.tableLayoutPanel.Controls.Add(controlsPanel, 1, 1);
       
       this.Controls.Add(splitter);
   }

Best Practices
--------------

1. **Use Consistent Theming**
   
   Apply themes consistently across your application:
   
   .. code-block:: csharp
   
      // Set a default theme for your application
      BeepThemesManager.SetApplicationTheme("DefaultTheme");

2. **Leverage React-Style Properties**
   
   Use the UIVariant, UISize, and UIColor properties for consistent styling:
   
   .. code-block:: csharp
   
      var primaryButton = new BeepButton
      {
          Text = "Primary Action",
          UIVariant = ReactUIVariant.Contained,
          UIColor = ReactUIColor.Primary,
          UISize = ReactUISize.Large
      };

3. **Handle Events Properly**
   
   Always properly handle events and dispose of resources:
   
   .. code-block:: csharp
   
      protected override void Dispose(bool disposing)
      {
          if (disposing)
          {
              // Dispose of Beep controls and unsubscribe from events
          }
          base.Dispose(disposing);
      }

4. **Use Layout Controls Effectively**
   
   Take advantage of layout controls like BeepPanel and BeepMultiSplitter for responsive designs.

Next Steps
----------

Now that you have a basic understanding of Beep Controls, explore these topics:

* :doc:`theming` - Learn about the comprehensive theming system
* :doc:`../controls/beep-button` - Detailed documentation for specific controls
* :doc:`../examples/basic-examples` - More working examples
* :doc:`../guides/best-practices` - Advanced tips and best practices

Troubleshooting
---------------

If you encounter issues:

* Ensure all using statements are correct
* Check that your project targets the correct .NET framework
* Verify that the Beep Controls package is properly installed
* Make sure you're calling ``ApplyTheme()`` after setting theme properties

.. tip::
   **Quick Debug Tip**
   
   If controls aren't appearing correctly, try setting explicit sizes and locations first, then gradually move to responsive layouts with Dock and Anchor properties.