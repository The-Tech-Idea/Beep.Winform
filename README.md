Beep.Winform Solution

Overview

This workspace contains several related .NET projects that build the Beep WinForms UI components, visualization modules, and data management libraries used across Beep applications. Projects target .NET 8/.NET 9 and several are packaged as NuGet packages on build.

Projects

- TheTechIdea.Beep.Vis.Modules
  - Path: TheTechIdea.Beep.Vis.Modules2.0/TheTechIdea.Beep.Vis.Modules.csproj
  - Targets: net8.0, net9.0
  - Version: 2.0.23
  - Description: Visualization interfaces used to standardize GUI/UI for Beep apps. Produces a NuGet package on build.
  - Notes: Copies package to LocalNugetFiles and DLLs to outputDLL after build/pack.

- WinformSampleApp
  - Path: WinformSampleApp/WinformSampleApp.csproj
  - Targets: net9.0-windows
  - Description: Sample WinForms application demonstrating usage of the Winform controls.
  - Notes: References TheTechIdea.Beep.Winform.Controls project. Run from Visual Studio or dotnet run (Windows only).

- TheTechIdea.Beep.Winform.Controls
  - Path: TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj
  - Targets: net8.0-windows
  - Version: 1.0.168
  - Description: Core WinForms controls, resources (fonts, SVG icons) and components used by Beep WinForms apps. Produces a NuGet package on build.
  - Notes: Many embedded resources and fonts are included. Post-build copies DLLs to outputDLL and NuGet to LocalNugetFiles.

- TheTechIdea.Beep.Winform.Default.Views
  - Path: TheTechIdea.Beep.Winform.Default.Views/TheTechIdea.Beep.Winform.Default.Views.csproj
  - Targets: net8.0-windows
  - Version: 1.0.1
  - Description: Default view implementations and UI compositions for Beep WinForms apps. Depends on several Beep libraries and the Winform controls projects.
  - Notes: Contains docs and sample folders (CRUD, ETL, DDL) in the project structure.

- TheTechIdea.Beep.Winform.Controls.Integrated
  - Path: TheTechIdea.Beep.Winform.Controls.Integrated/TheTechIdea.Beep.Winform.Controls.Integrated.csproj
  - Targets: net8.0-windows
  - Version: 1.0.1
  - Description: Integrated extensions and components for the Winform controls project. Packaged as NuGet on build.
  - Notes: Contains a docs folder and marks some components as design-time components.

- TheTechIdea.Beep.Vis.Loader
  - Path: ..\Beep.Branchs\TheTechIdea.Beep.Vis.Loader\TheTechIdea.Beep.Vis.Loader.csproj
  - Targets: net8.0, net9.0
  - Version: 1.0.6
  - Description: Loader project that depends on TheTechIdea.Beep.Vis.Modules and packages as NuGet.

- TheTechIdea.Beep.TreeNodes
  - Path: ..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\TheTechIdea.Beep.TreeNodes.csproj
  - Targets: net8.0, net9.0
  - Version: 1.0.15
  - Description: Tree node implementations used by Beep UI components. Packaged as NuGet on build.

- TheTechIdea.Beep.DataManagementModels
  - Path: ..\BeepDM\DataManagementModelsStandard\DataManagementModels.csproj
  - Targets: net8.0, net9.0
  - Version: 2.0.79
  - Description: Data Management library models used across the system. Produces package and docs.xml.

- TheTechIdea.Beep.DataManagementEngine
  - Path: ..\BeepDM\DataManagementEngineStandard\DataManagementEngine.csproj
  - Targets: net8.0, net9.0
  - Version: 2.0.37
  - Description: Core data management engine. Depends on DataManagementModels and Roslyn (Microsoft.CodeAnalysis.CSharp) for dynamic code features. Produces package and docs.

- TheTechIdea.Beep.AssemblyLoader
  - Path: ..\BeepDM\Assembly_helpersStandard\Assembly_helpers.csproj
  - Targets: net8.0, net9.0
  - Version: 2.0.10
  - Description: Assembly loading helpers and DI integration used by the engine and other components.

- Beep.Nugget.Engine
  - Path: ..\Beep.Nugget\Beep.Nugget.Engine\Beep.Nugget.Engine.csproj
  - Targets: net8.0
  - Description: Utility engine for NuGet-related operations. Depends on NuGet libraries and Beep assembly loader.

Build and packaging notes

- Common build command (from repository root):
  - dotnet build -c Release
  - dotnet pack -c Release (for projects that produce NuGet packages)
- Many projects use PostBuild/Pack targets to copy generated NuGet packages to a LocalNugetFiles folder and copy DLLs to an outputDLL folder organized by project and target framework.
- Windows-only projects (UseWindowsForms) target net8.0-windows or net9.0-windows and must be built on Windows with the desktop workload installed.

How to run the sample app

- Open the solution containing WinformSampleApp in Visual Studio (ensure .NET 9 SDK and Windows Forms workload installed).
- Set WinformSampleApp as startup project and run.
- Alternatively build the solution and run the generated executable from bin/(Configuration)/net9.0-windows.

Notes

- Versions shown are taken from project files and may differ across branches. When packaging is required, check the project's PackageId and Version in the csproj.
- Several projects include many embedded resources (SVG icons and fonts). Ensure these resources remain in the project tree if packaging.

If you want, I can also:
- Generate a table of all projects with full paths and versions in CSV or Markdown format.
- Add contributor and license sections to this README.
