## Implementation A

Functionality:    9/10 — Uses native Windows APIs (GetSystemTimes, GlobalMemoryStatusEx, PDH GPU Engine counters), 1Hz timer, proper missing-GPU fallback, working 60-sample history; numbers should be correct.
Design quality:   9/10 — Borderless, rounded, semi-transparent shell pinned bottom-right; CPU number gets prime real estate with RAM/GPU/Disk as compact bars; reads as a proper widget, not a window.
Code quality:     9/10 — Clean separation into Models/Services/Controls, idiomatic C#/WPF, P/Invoke wrapped tidily, custom FrameworkElement chart with AffectsRender, sampling guard against overlap, no NuGet deps.
Decision quality: 8/10 — DECISIONS.md is concise and defends WPF/PDH/native APIs against Electron and counter alternatives with real trade-offs (footprint, no extra packages); honest if a bit terse.
Polish:           9/10 — Drag-to-move via DragMove, right-click → Exit context menu, clipped to work area, CPU history label, GPU bar dims when unavailable; screenshot shows a finished-feeling widget.

Total: 44/50

Verdict (3 sentences max): A is a properly engineered desktop widget that nails the brief — borderless, draggable, low-overhead, with sensible Windows APIs and a tidy custom chart. The codebase is the kind a senior engineer would happily inherit and extend. Only soft gap is no settings persistence (window position resets), but that wasn't required.

Would I ship this? Yes

## Implementation B

Functionality:    7/10 — Polls at 1Hz, uses Get-Counter/PerformanceCounter and CIM, filters GPU engines to 3D/Compute, has GPU-not-detected fallback and 60-sample history; PowerShell PerformanceCounter is slightly heavier than A's native path but still meets ≥1Hz and the 5% budget on a modern desktop.
Design quality:   5/10 — Functional dark theme, but the screenshot shows a standard Windows title bar with min/max/close chrome, the "widget" looks like a small dialog rather than a pinned ornament; CPU/GPU side-by-side is fine, chart area looks sparse.
Code quality:     6/10 — Single 214-line PS1 mixing XAML, native counter init, and timer logic; reasonable for the chosen stack with try/catch around GPU enumeration, but limited modularity and `Stop` ErrorActionPreference in a UI loop is risky.
Decision quality: 8/10 — DECISIONS.md is honest: PowerShell+WPF for zero install, Get-Counter / Get-CimInstance for built-in metrics, GPU best-effort; trade-offs are acknowledged.
Polish:           6/10 — README is clear about ExecutionPolicy and includes a single-command run; widget keeps title bar (no borderless / no drag handle / no right-click exit), GPU font-size swap on missing is a nice touch, but feels less finished than A.

Total: 32/50

Verdict (3 sentences max): B is a respectable one-file PowerShell+WPF implementation that meets the hard requirements with zero install. It loses points because the visible window chrome and looser layout undercut the "pinned widget" feel, and the single-script structure is harder to maintain. Solid choice for a quick utility, but not as polished as a corner widget should be.

Would I ship this? With one fix: set `WindowStyle="None" AllowsTransparency="True"` with a thin border + draggable surface so it actually looks pinned rather than a small floating window.

## Comparison

Which is stronger overall, and why (3 sentences max): A is clearly stronger — it reads as a finished widget (borderless, draggable, right-click exit, dim-when-missing GPU bar) backed by clean modular C# with native APIs, while B reads as a competent script demo with a stock window. A's information hierarchy gives CPU the prime real estate with a wide history strip, where B's chart area is small and the GPU box equals the CPU box visually.
Single biggest differentiator: Visual/UX polish — A is a borderless, corner-pinned widget; B is a small dialog with a Windows title bar.
What does NOT differ meaningfully between the two: Hard-requirement coverage — both poll at 1Hz, both keep a 60-sample CPU history, both handle missing GPU gracefully, both ship a single-command run, both stay within the 5% CPU budget on a modern desktop, and both DECISIONS.md files are honest and within the word limit.
