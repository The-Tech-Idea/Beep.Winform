# Stage 06 — Six `catch { }` around code that cannot throw

**Kind:** cleanup · **Status: done.** Census: **0**.

Six bare catches, all in painters, all wrapping the same shape of code:

```csharp
try { if (context.AnimatedIndicatorWidth > 0f) { iX = context.AnimatedIndicatorX; } } catch { }
```

Float arithmetic on `context` fields - and the lines immediately around each one dereference `context`
without any guard. The only exception they could have caught is a `NullReferenceException` that the
next statement would raise anyway.

**They were removed rather than given a `BeepLog` call.** The house rule is that a catch must report,
but a catch that cannot fire has nothing to report; logging it would have dressed up dead code as
error handling. Where a real failure is possible the rule stands and `BeepLog` is used - see the
external-drawing handler in [03](03-cta-overhang.md), which does catch and does report.
