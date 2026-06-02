# Beep.Winform.Controls

**Modern, themeable WinForms controls for .NET 8 applications** — 80+ controls, 35+ form styles, 27 design systems, 130+ painter classes.

[![NuGet](https://img.shields.io/badge/nuget-v1.0.207-blue)](https://www.nuget.org/packages/TheTechIdea.Beep.Winform.Controls)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)

## Overview

`TheTechIdea.Beep.Winform.Controls` is a comprehensive library of modern, themeable Windows Forms controls. Every control inherits from **BaseControl**, which uses a **Painter Strategy Pattern** to separate rendering from behavior. Forms use **BeepiFormPro** — a fully custom-drawn Form with 35+ form styles (Material, Fluent, macOS, Cyberpunk, Nord, Dracula, etc.). The entire system is coordinated by **BeepThemesManager** (global theme switching) and **BeepStyling** (27 design systems for backgrounds, borders, shadows, and typography).

## Core Architecture

| Component | Description |
|-----------|-------------|
| **BaseControl** | All controls inherit from `BaseControl`. Provides automatic theming via `IBeepUIComponent`, pluggable painter system (`PainterKind`: Classic, Material, Card, Glassmorphism, Neumorphism, etc.), DPI scaling, data binding, hit testing, and tooltip support. |
| **BeepiFormPro** | Modern borderless Form with 35+ styles via `FormStyle` enum. Each style has a dedicated `IFormPainter` that handles caption bar, buttons, borders, and backdrop rendering. |
| **BeepThemesManager** | Static manager that discovers all `IBeepTheme` implementations via assembly scanning. Fires `ThemeChanged` and `FormStyleChanged` events. |
| **BeepStyling** | Central styling coordinator with 130+ painter classes across 7 folders supporting 27 design systems (Material3, Fluent2, iOS15, AntDesign, ChakraUI, etc.). |
| **Painter Strategy** | Rendering pipeline: `OnPaint → EnsurePainter() → _painter.UpdateLayout() → _painter.Paint() → DrawingRect`. Derived controls only paint their inner content into `DrawingRect`. |

## Library Stats

- **80+** Controls Available
- **35+** Form Styles (BeepiFormPro)
- **27** Design Systems (BeepStyling)
- **130+** Painter Classes

## Installation

```bash
# Package Manager Console
Install-Package TheTechIdea.Beep.Winform.Controls

# .NET CLI
dotnet add package TheTechIdea.Beep.Winform.Controls

# Package Reference
<PackageReference Include="TheTechIdea.Beep.Winform.Controls" Version="1.0.207" />
```

## Quick Start

```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

public class MainForm : BeepiFormPro
{
    public MainForm()
    {
        Text = "My App";
        FormStyle = FormStyle.Material;

        var btn = new BeepButton();
        btn.Text = "Hello Beep!";
        btn.Size = new Size(160, 45);
        btn.Location = new Point(50, 100);
        btn.ImagePath = "icons/save.svg";
        btn.Click += (s, e) => MessageBox.Show("Hello!");
        Controls.Add(btn);
    }
}

// In Program.cs
BeepThemesManager.ApplyTheme("MaterialBlue");
Application.Run(new MainForm());
```

## Documentation

Full documentation is available in the [Help](./Help) folder:
- [Home](./Help/home.html) — Overview and quick start
- [Getting Started](./Help/getting-started/installation.html) — Installation guide
- [Controls Reference](./Help/controls/) — 95+ control documentation pages
- [Guides & Examples](./Help/guides/) — Best practices, performance, accessibility
- [Architecture & Internals](./Help/architecture/) — Docking, GridX, Chart, Calendar, Theme internals
- [Design-Time Infrastructure](./Help/design-time/) — Designer classes, action lists, editors

## Solution Structure

| Project | Description |
|---------|-------------|
| `TheTechIdea.Beep.Winform.Controls` | Main controls library (NuGet: `TheTechIdea.Beep.Winform.Controls`) |
| `TheTechIdea.Beep.Winform.Controls.Design.Server` | Design-time support for Visual Studio toolbox integration |
| `TheTechIdea.Beep.Winform.Controls.Tests` | Unit tests |

## License

MIT License — see [LICENSE.txt](./LICENSE.txt)

## Requirements

- .NET 8.0 or later
- Windows (WinForms)
