# planfixshadowpainters.md

Goal: Convert ShadowPainters to use `PaintersFactory` and `ShadowPainterHelpers` consistently to avoid per-paint GDI allocations and standardize shadow behavior.

Plan (trackable, updated as each file is processed):

1. Update `ShadowPainterHelpers.cs` to use `PaintersFactory` for brushes and pens where appropriate.
 - Status: DONE
2. Process shadow painters in batches of10. For each file:
 - Add `using TheTechIdea.Beep.Winform.Controls.Styling;` if missing.
 - Replace `new SolidBrush`, `new Pen`, and `new LinearGradientBrush` with `PaintersFactory.GetSolidBrush`, `PaintersFactory.GetPen`, and `PaintersFactory.GetLinearGradientBrush` respectively.
 - Prefer delegating to `ShadowPainterHelpers` for complex shadow behaviors (soft, neon, material, etc.).
 - Update this plan file with file status.

Batching policy:
- Work in batches of10 files at a time, update plan after each batch, do not run builds unless asked.

Progress

Batch1 - Completed
- `AntDesignShadowPainter.cs` — DONE
- `AppleShadowPainter.cs` — DONE
- `ArcLinuxShadowPainter.cs` — DONE
- `BootstrapShadowPainter.cs` — DONE
- `BrutalistShadowPainter.cs` — DONE
- `CartoonShadowPainter.cs` — DONE
- `ChakraUIShadowPainter.cs` — DONE
- `ChatBubbleShadowPainter.cs` — DONE
- `CinnamonShadowPainter.cs` — DONE
- `CyberpunkShadowPainter.cs` — DONE

Batch2 - Completed
- `DarkGlowShadowPainter.cs` — DONE
- `DiscordStyleShadowPainter.cs` — DONE
- `DraculaShadowPainter.cs` — DONE
- `EffectShadowPainter.cs` — DONE
- `ElementaryShadowPainter.cs` — DONE
- `FigmaCardShadowPainter.cs` — DONE
- `Fluent2ShadowPainter.cs` — DONE
- `FluentShadowPainter.cs` — DONE
- `GamingShadowPainter.cs` — DONE
- `GlassAcrylicShadowPainter.cs` — DONE

Batch3 - Completed
- `GlassmorphismShadowPainter.cs` — DONE
- `GnomeShadowPainter.cs` — DONE
- `GradientModernShadowPainter.cs` — DONE
- `GruvBoxShadowPainter.cs` — DONE
- `HighContrastShadowPainter.cs` — DONE
- `HolographicShadowPainter.cs` — DONE
- `iOS15ShadowPainter.cs` — DONE
- `KdeShadowPainter.cs` — DONE
- `MacOSBigSurShadowPainter.cs` — DONE
- `Material3ShadowPainter.cs` — DONE

Batch4 - Completed
- `MaterialShadowPainter.cs` — DONE
- `MaterialYouShadowPainter.cs` — DONE
- `Metro2ShadowPainter.cs` — DONE
- `MetroShadowPainter.cs` — DONE
- `MinimalShadowPainter.cs` — DONE
- `ModernShadowPainter.cs` — DONE
- `NeoBrutalistShadowPainter.cs` — DONE
- `NeonShadowPainter.cs` — DONE
- `NeumorphismShadowPainter.cs` — DONE
- `NordicShadowPainter.cs` — DONE

Batch5 - Completed
- `NordShadowPainter.cs` — DONE
- `NotionMinimalShadowPainter.cs` — DONE
- `OfficeShadowPainter.cs` — DONE
- `OneDarkShadowPainter.cs` — DONE
- `PaperShadowPainter.cs` — DONE
- `PillRailShadowPainter.cs` — DONE
- `RetroShadowPainter.cs` — DONE
- `SolarizedShadowPainter.cs` — DONE
- `StandardShadowPainter.cs` — DONE
- `StripeDashboardShadowPainter.cs` — DONE
- `TailwindCardShadowPainter.cs` — DONE
- `TerminalShadowPainter.cs` — NOOP
- `TokyoShadowPainter.cs` — DONE
- `UbuntuShadowPainter.cs` — DONE

Batch6 - Completed
- `VercelCleanShadowPainter.cs` — DONE
- `WebFrameworkShadowPainter.cs` — DONE
- `Windows11MicaShadowPainter.cs` — DONE

Final note:
- Most shadow painters now use `ShadowPainterHelpers` and `PaintersFactory` per the plan. If you want I can run a workspace build to confirm no errors, or continue to review and optimize helpers.

Reply `build` to run a build and validate, `continue` to run further checks, or `done` to finish the task.
