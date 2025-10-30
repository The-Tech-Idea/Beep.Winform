# planfixtextpainters.md

Goal: Convert TextPainters to use `PaintersFactory` for cached brushes/pens where appropriate and standardize `using` of `Styling` namespace.

Plan (trackable):
1. Enumerate files in `Styling/TextPainters` and list them here.
2. Process files in batches of10. For each file:
 - Add `using TheTechIdea.Beep.Winform.Controls.Styling;` if missing.
 - Replace `new SolidBrush`, `new Pen`, and `new LinearGradientBrush` with `PaintersFactory.GetSolidBrush`, `PaintersFactory.GetPen`, and `PaintersFactory.GetLinearGradientBrush` respectively where safe.
 - Do NOT dispose brushes/pens returned by `PaintersFactory`.
 - Prefer delegating any complex rendering to shared helpers if applicable.
 - Update status in this plan file.

Files in folder:
- `AccessibilityTextPainter.cs` - DONE
- `AppleDesignTextPainter.cs` - DONE
- `CorporateTextPainter.cs` - DONE
- `DesignSystemTextPainter.cs` - DONE
- `EffectTextPainter.cs` - DONE
- `GamingTextPainter.cs` - DONE
- `GlassTextPainter.cs` - DONE
- `LinuxDesktopTextPainter.cs` - DONE
- `MaterialDesignTextPainter.cs` - DONE
- `MicrosoftDesignTextPainter.cs` - DONE
- `MonospaceDesignTextPainter.cs` - DONE
- `RetroTextPainter.cs` - DONE
- `StandardDesignTextPainter.cs` - DONE
- `TerminalTextPainter.cs` - DONE
- `ValueTextPainter.cs` - DONE
- `WebFrameworkTextPainter.cs` - DONE

Batch progress:
- All text painter files processed and updated to use PaintersFactory cached brushes/pens where applicable.

Notes:
- I did not change complex uses of `PathGradientBrush` or embedded font logic.
- Remember not to dispose factory brushes/pens.

You can ask me to run a build to validate the changes (`build`) or to review any specific files. Reply `build` or `done`.