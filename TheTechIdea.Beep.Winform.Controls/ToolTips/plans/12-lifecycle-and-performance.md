# 12 — Lifecycle & Performance

**Priority P2.**

## Current behaviour

### A window per show

`ToolTipInstance.ShowAsync` constructs a fresh `CustomToolTip` (`new CustomToolTip()`) each time.
`CustomToolTip` is a `Form`-derived top-level window, so every hover creates and destroys a native
window with its own handle, message queue and GDI resources.

Sweeping a toolbar creates one window per button. WinForms window creation is not free, and it also
causes the visible flash some users report when tooltips appear.

Every mature implementation keeps **one** reusable window per manager (or a small pool) and just
re-contents and re-positions it — which is also the prerequisite for the move-transition described
in [06](06-delay-groups.md).

### Defensive `catch (ObjectDisposedException)` scattered through the lifecycle

`ToolTipInstance` has at least six `catch (ObjectDisposedException)` blocks guarding
`_tooltip`/`_tooltip.IsDisposed` access. That density is a symptom: the object's lifetime is not
actually owned anywhere, so every call site defends itself. Reusing one window and giving it a
single owner removes most of these.

### Sweep timer

The manager runs a `System.Threading.Timer` every 5 seconds to clean up expired tooltips. This is a
*threadpool* timer touching UI objects — verify every path marshals to the UI thread. Expiry should
be driven by each tooltip's own timer, with the sweep kept only as a backstop for leaks.

### Handler and filter leaks

- `AttachControlHandlers` / `DetachControlHandlers` correctly store named delegates so they can be
  unsubscribed — a comment records that the previous anonymous-lambda version leaked on every
  reassignment. Good; keep it.
- `_controlTooltips` and `_attachedHandlers` are keyed by `Control`. If a control is disposed
  without `RemoveTooltip`, both dictionaries keep it alive. There is no `Disposed` subscription.
- `OutsideClickMessageFilter` is process-wide; see [05](05-dismissal-and-focus.md).

### `Control.GetHashCode()` as an identity key

```csharp
config.Key = $"control_{control.GetHashCode()}_{DateTime.Now.Ticks}";
```

`GetHashCode` is not an identity and can collide. The ticks suffix makes collision unlikely but the
key is then not stable for the same control, so lookups by control must go through
`_controlTooltips` anyway. Use a `ConditionalWeakTable` or the control reference directly.

## Work

1. **Reuse one tooltip window per manager**, re-contented and repositioned per show; pool only if a
   scenario needs simultaneous tooltips (pinned ones — see [10](10-pinning.md)).
2. **Single ownership of the window's lifetime**, removing most `ObjectDisposedException` guards.
3. **`ConditionalWeakTable`** (or subscribe to `Control.Disposed`) so a disposed control cannot be
   kept alive by the manager's dictionaries.
4. **Per-tooltip expiry timers** on the UI thread; the 5-second sweep becomes a diagnostic backstop
   that logs when it finds something, rather than being the primary mechanism.
5. **Audit thread affinity** — `ShowTooltipAsync` is async and the sweep is on a threadpool thread;
   every touch of a `Control` must be marshalled.
6. **Cache painter resources per style**, not per paint, and confirm the cached brushes are
   invalidated on theme change (see [11](11-theming-and-styles.md)).

## Verification

- Show and hide 1,000 tooltips; assert the process's USER/GDI handle counts return to baseline and
  window count stays at one.
- Attach a tooltip, dispose the control without calling `RemoveTooltip`, force GC; assert the
  control is collected and the manager's dictionaries do not retain it.
- Hover a 20-button toolbar quickly; measure window creations (expect 0 after the first) and total
  time.
- Assert no `Control` member is touched from a non-UI thread, under a debug thread-affinity check.
