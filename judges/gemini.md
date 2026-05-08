## Implementation A

Functionality:    10/10 — Uses native Windows APIs to reliably fetch all metrics at 1Hz without locking the UI.
Design quality:   9/10 — Borderless, transparent, dark-themed UI with clear hierarchy looks exactly like a modern Windows widget.
Code quality:     9/10 — Idiomatic, well-structured C# with proper async polling, safe native interop, and clean XAML separation.
Decision quality: 8/10 — WPF is perfect for the 5% CPU budget, but requiring the user to install the .NET SDK to run it is a slightly heavy trade-off.
Polish:           9/10 — Drag-to-move, context menu exit, and robust error handling make it feel like a highly finished product.

Total: 45/50

Verdict (3 sentences max): A highly professional implementation that nails the visual and performance requirements. The architecture ensures the UI remains responsive, and the visual styling perfectly matches the "pinned corner widget" spec. The only minor downside is the dependency on the .NET 8 SDK for the initial run, though it perfectly follows the instructions.

Would I ship this? Yes

## Implementation B

Functionality:    9/10 — Accurately collects all required metrics and renders the chart, though synchronous polling may cause micro-stutters.
Design quality:   7/10 — Clean dark theme, but the standard window borders make it look slightly more like a dialog box than a sleek widget.
Code quality:     7/10 — Highly creative use of PowerShell and WPF, but polling performance counters on the UI thread is an architectural flaw.
Decision quality: 9/10 — Brilliantly optimizes for distribution by requiring zero dependencies or compilation on Windows 11.
Polish:           7/10 — Functional and well-documented, but lacks the borderless UI and drag-to-move mechanics expected of a desktop widget.

Total: 39/50

Verdict (3 sentences max): An ingenious, dependency-free script that perfectly solves the distribution problem on Windows 11. However, it sacrifices some UI responsiveness and visual polish by running everything synchronously in a single PowerShell script. While an incredible prototype, it feels slightly less like a native widget due to the title bar.

Would I ship this? With one fix: make the window borderless and add a drag-to-move event handler.

## Comparison

Which is stronger overall, and why (3 sentences max): Implementation A is stronger overall because it delivers a significantly better user experience that truly feels like a pinned desktop widget. While B's zero-dependency installation is clever, A's background polling, borderless design, and robust code structure make it a much more viable long-term utility. A is an application you would actually leave running on your screen, whereas B feels more like a brilliant hack.

Single biggest differentiator: A's use of a compiled language with asynchronous background polling ensures smooth UI performance, whereas B risks stuttering by polling synchronously on the UI thread.

What does NOT differ meaningfully between the two: Both correctly identified WPF as the optimal framework to build a lightweight, low-CPU Windows desktop UI without relying on heavy third-party libraries.

