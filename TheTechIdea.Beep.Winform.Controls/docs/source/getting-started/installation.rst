Installation Guide
==================

System Requirements
-------------------

Before installing Beep Controls, ensure your development environment meets the following requirements:

.. list-table::
   :header-rows: 1
   :widths: 30 70

   * - Requirement
     - Specification
   * - .NET Framework
     - .NET 6.0, 7.0, 8.0, or 9.0
   * - Target Framework
     - net6.0-windows, net7.0-windows, net8.0-windows, or net9.0-windows
   * - Development Environment
     - Visual Studio 2022 (recommended) or Visual Studio Code with C# extension
   * - Windows Version
     - Windows 10 version 1809 or later, Windows 11

Installation Methods
--------------------

Package Manager Console
~~~~~~~~~~~~~~~~~~~~~~~

1. Open your project in Visual Studio
2. Go to **Tools** ? **NuGet Package Manager** ? **Package Manager Console**
3. Run the following command:

.. code-block:: powershell

   Install-Package TheTechIdea.Beep.Winform.Controls

.NET CLI
~~~~~~~~

If you're using the .NET CLI, navigate to your project directory and run:

.. code-block:: bash

   dotnet add package TheTechIdea.Beep.Winform.Controls

PackageReference
~~~~~~~~~~~~~~~~

Add the following to your project file (``.csproj``):

.. code-block:: xml

   <PackageReference Include="TheTechIdea.Beep.Winform.Controls" Version="1.0.164" />

Then restore packages:

.. code-block:: bash

   dotnet restore

Project Configuration
---------------------

Target Framework
~~~~~~~~~~~~~~~~

Ensure your project targets a Windows-specific framework. Update your ``.csproj`` file:

.. code-block:: xml

   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>WinExe</OutputType>
       <TargetFramework>net8.0-windows</TargetFramework>
       <UseWindowsForms>true</UseWindowsForms>
       <Nullable>enable</Nullable>
     </PropertyGroup>
   </Project>

Enable Windows Forms
~~~~~~~~~~~~~~~~~~~~~

Make sure Windows Forms is enabled in your project:

.. code-block:: xml

   <UseWindowsForms>true</UseWindowsForms>

Verification
------------

To verify the installation was successful, try creating a simple form with a Beep control:

.. code-block:: csharp

   using TheTechIdea.Beep.Winform.Controls;

   public partial class Form1 : Form
   {
       public Form1()
       {
           InitializeComponent();
           
           // Create a simple Beep button to test installation
           var beepButton = new BeepButton
           {
               Text = "Hello Beep!",
               Size = new Size(120, 40),
               Location = new Point(50, 50)
           };
           
           this.Controls.Add(beepButton);
       }
   }

If the code compiles and runs without errors, the installation was successful!

Troubleshooting
---------------

Common Issues
~~~~~~~~~~~~~

**Package not found**
   - Ensure you have the correct package name: ``TheTechIdea.Beep.Winform.Controls``
   - Check your NuGet package sources

**Build errors**
   - Verify your project targets a Windows-specific framework (e.g., ``net8.0-windows``)
   - Ensure ``UseWindowsForms`` is set to ``true``

**Runtime errors**
   - Make sure you have the correct .NET runtime installed
   - Check that all dependencies are properly restored

**Design-time issues**
   - Rebuild your solution
   - Close and reopen Visual Studio
   - Clear NuGet caches: ``dotnet nuget locals all --clear``

Getting Help
~~~~~~~~~~~~

If you encounter issues during installation:

1. Check the project's GitHub repository for known issues
2. Ensure you're using a supported .NET version
3. Try creating a new test project to isolate the issue
4. Contact support with detailed error information

Next Steps
----------

Once installation is complete, proceed to:

* :doc:`quick-start` - Create your first Beep Controls application
* :doc:`theming` - Learn about the theming system
* :doc:`../examples/basic-examples` - View working examples