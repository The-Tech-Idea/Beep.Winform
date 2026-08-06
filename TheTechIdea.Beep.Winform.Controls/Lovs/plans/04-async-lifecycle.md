# Stage 04 — The spinner outlives the popup

**Kind:** bug · **Files:** `BeepLovPopup.cs` · **Status: done.**

`LoadItemsAsync` started the spinner timer through `BeginLoading`, then on cancellation did:

```csharp
catch (OperationCanceledException)
{
    return;   // popup was closed / new load started — silently discard
}
```

`_spinnerTimer` was never stopped. Cancelling a load — which `Cancel()` and `OnDeactivate` both do,
so simply pressing Escape while a query was in flight — left a 80 ms timer ticking on a hidden form
**for the life of the process**, invalidating a label nobody could see.

## The fix

A `StopSpinner` helper, called on every exit path: cancellation, failure, and the "was the popup closed
while we were loading" check that also returned bare.

The load failure path reports through `BeepLog` rather than only writing the message into the overlay
label, and the cancellation path reports at `Info` — expected flow rather than a failure, but a LOV
that opens empty is a support question and "the load was cancelled" is the answer.

`_searchCts` and the search debounce timer, added in [07](07-scale.md), are disposed alongside.

## What is still worth measuring

`OnDeactivate` hides the popup whenever it loses activation. Whether a modal dialog opened from a LOV
selection can cause the popup to hide mid-selection is **unverified** — plausible, not measured, and
should not be repeated as a finding until it is.
