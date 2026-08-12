# Stage 01 — The designer: resize refresh, and a real root designer

**The reported bug:** enlarging the form on the design surface does not refresh its content — the
caption bar, borders and painted chrome stay at the old size until something else forces a repaint.

## Mechanism (evidence)

Three defects compound, and any one of them alone would degrade the design surface:

1. **Geometry sync never runs at design time.** `OnResize` is a documented no-op
   (`BeepiFormPro.Events.cs:139-149`): all geometry work — `UpdateWindowRegion`,
   `UpdateFormRegion`, border-shape rebuild — is deferred to `OnResizeEnd` /
   `WM_EXITSIZEMOVE` (`Win32.cs:347-369`). Those fire at the end of an *interactive* user resize.
   The designer resizes the form by writing `Size`/calling `SetBoundsCore` — no modal size loop,
   no `WM_EXITSIZEMOVE`, no `OnResizeEnd`. The deferred work simply never happens.
   `EnsureLayoutCalculated` *does* key on `_lastLayoutSize != ClientSize` (`Core.cs:44`), so the
   layout would self-heal on the next paint — **if** a full repaint arrives, which is the next
   defect's job to guarantee and it does not.

2. **The trailing repaint can be dropped.** `DebouncedInvalidate` (`BeepiFormPro.cs:54-63`)
   is leading-edge-only: any call within 16 ms of the previous one returns without painting *and
   without scheduling anything*. The final invalidate of a resize burst — the one that would show
   the finished size — is exactly the call most likely to be swallowed. (Fixed properly in stage
   02; stage 01 needs at least the design-time symptom gone.)

3. **The wrong designer class.** `BeepiFormProDesigner` derives from `ParentControlDesigner`
   (`Designers/BeepiFormProDesigner.cs:10`) but is attached to a *Form* — the root-document
   designer in WinForms is `DocumentDesigner`, which owns frame behaviour, snaplines, and
   resize-driven invalidation of the design surface. The custom designer also hooks
   `ComponentAdded/Removed/Changed` and selection, but never the control's own
   `Resize`/`SizeChanged` — so its repaint plumbing misses the one event this bug is about.

Also in scope, because it is designer-facing and adjacent: the ctor writes
`AutoScaleDimensions = new SizeF(96f, 96f)` directly beneath a comment saying to remove it
(`BeepiFormPro.cs:70-73`). Serialized designer files of derived forms carry their own
`AutoScaleDimensions`; the two fight. Decide one owner (the designer file), delete the ctor write.

## The work

1. **Reproduce first, in a harness, not by hand** — a design-mode simulation: instantiate under
   `LicenseManager.UsageMode == Designtime` semantics (set `Site` with `DesignMode = true`),
   resize by property write, render, and measure whether chrome tracked the new bounds. This is
   the failing check; it must be red before any fix.
2. **Make geometry follow non-interactive resizes.** In `OnResize` (or `SetBoundsCore`), when not
   inside an interactive size loop (`WM_ENTERSIZEMOVE`..`WM_EXITSIZEMOVE` flag already exists in
   Win32.cs), run the same sync `OnResizeEnd` runs — guarded so the interactive path keeps its
   deferral. This fixes programmatic resizes at *runtime* too (`form.Size = …` has the same bug).
3. **Rebase the designer on `DocumentDesigner`**, keep the action-list and component hooks, add a
   `SizeChanged` hook, and delete repaint plumbing that `DocumentDesigner` already provides.
4. **One AutoScale owner** — remove the ctor's `AutoScaleDimensions` write; verify a derived form
   opens in the designer at the size its designer file says.

## Verification (stage 07's probe carries these)

- Design-mode simulation: create at 600×400, render, set `Size = (900, 650)`, render again —
  caption bar right edge, button cluster, and border must all sit at the new width. Print the
  measured caption-right vs client-right; failing run prints the stale pair.
- Runtime programmatic resize: same check without the design-mode site.
- Break-it-first: re-instate the `OnResize` no-op and confirm both checks go red.
- Eyeball: open one derived form in VS, drag-resize, confirm live chrome. (Manual — record that it
  was done and by whom; a probe cannot drive the real VS designer.)

## Exit criteria

Both checks green with the break-test recorded; the library builds; `CardsWizardForm` (a
`BeepiFormPro` derivative) opens in the designer and drag-resize refreshes; decisions and any
public-member changes recorded here.
