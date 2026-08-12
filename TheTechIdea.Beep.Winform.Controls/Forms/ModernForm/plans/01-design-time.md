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

## Outcome — ☑ done

Reproduced, fixed, and both fixes proven able to fail.

### Measured before the fix

```
FAIL  runtime:     captionRight 584 -> 584,  clientWidth 584 -> 898   << stale by 314px
FAIL  design-mode: captionRight   0 ->   0,  clientWidth 572 -> 886   << layout never computed
```

The design-mode number is the sharper finding: `CaptionRect.Right == 0` means the caption was
painted from a layout that had **never been calculated**, not merely a stale one.

### Three defects, not one

1. **`ShouldRefreshChromeAfterWmSize` returned false for exactly this case**
   (`Win32.cs:116-127`). It refreshed only on a window-*state* change or a size change while
   **not** Normal. A Normal-state resize — which is what the designer and `form.Size = …` both
   do — matched neither. Fixed by adding `programmaticNormalResize`, gated on a new
   `_inInteractiveSizeMove` flag set from `WM_ENTERSIZEMOVE` / cleared on `WM_EXITSIZEMOVE`, so
   the original deferral during a real drag is preserved — that deferral is what fixed the missing
   right/bottom border, and undoing it would have traded one bug for another.
2. **The design-mode paint branch never computed the layout** (`Events.cs:29-64`). The runtime
   branch calls `EnsureLayoutCalculated()`; the design-mode branch went straight to
   `PaintCaption(…, CurrentLayout.CaptionRect)` with whatever that held. Added the call.
3. **`OnResize` was a no-op, so nothing recomputed *or* repainted.** It now recomputes directly
   rather than invalidating and hoping a paint arrives — the designer surface does not necessarily
   deliver one. Interactive drags return early, preserving the deferral.

### Break-it-first — run, not assumed

| break | result |
|---|---|
| `OnResize` back to a no-op | **design-mode red**, runtime still green |
| `programmaticNormalResize` removed from `ShouldRefreshChromeAfterWmSize` | **runtime red** as well (both red) |
| both restored | both green |

Worth recording: the two fixes defend **different paths** — runtime is carried by the `WM_SIZE`
change, design mode by `OnResize`. A single break would have suggested one fix was redundant.

### A capture artefact that nearly became a false finding

The first renders were **pure white**. Nothing was wrong with the paint: `DrawToBitmap` sends
`WM_PRINTCLIENT`, which does not invoke this form's custom `OnPaintBackground` — where all the
chrome is drawn. Read carelessly, that is "the caption is missing". `FormProbe.Shot` now drives
`OnPaintBackground` over a magenta-prefilled bitmap, so what is painted is visible and anything
left magenta is genuinely unpainted. Renders eyeballed: caption, title and the three buttons sit at
the new right edge after the resize.

### Verified

- Runtime programmatic resize: `captionRight 584 → 898` against `clientWidth 584 → 898`.
- Design-mode (Site with `DesignMode = true`) resize: `572 → 886` against `572 → 886`.
- Renders eyeballed before/after in both modes.
- Full library builds clean.

### NOT done — deliberately, with reasons

- **The `DocumentDesigner` rebase (F2).** The reported bug is fixed without it, and the change
  cannot be verified by any probe — it only manifests inside the real VS designer host, where a
  mistake takes the whole design surface down. It stays open in the tracker rather than being made
  blind. `BeepiFormProDesigner` still hooks no `Resize`/`SizeChanged`; nothing now depends on it
  doing so, because the form no longer relies on the designer to prompt a refresh.
- **Removing the ctor's `AutoScaleDimensions = (96,96)` (F5).** Untouched: it needs an audit of
  every derived form's serialized value, and getting it wrong silently rescales existing forms.
  Belongs with stage 04, which owns the end-to-end DPI story.
- **Manual VS check.** A probe cannot drive the real designer. Drag-resizing a derived form in
  Visual Studio has **not** been done and should be, by a human, before this is called finished in
  the product sense.

