## Implementation A

Functionality:    9/10 - Meets the core live metrics, GPU fallback, 1Hz polling, and CPU history requirements with a real desktop app.
Design quality:   8/10 - Strong corner-widget look, though the disk row is cramped in the screenshot.
Code quality:     9/10 - Clean WPF structure, no external dependencies, separated metrics/service/model/UI concerns.
Decision quality: 9/10 - The WPF/native API choices are well justified and match the task constraints.
Polish:           8/10 - README, build/run docs, screenshot, drag, and exit path are solid; minor UI spacing issue remains.

Total: 43/50

Verdict (3 sentences max): This is a complete, maintainable implementation that feels close to shippable. It makes sensible platform choices and handles the tricky GPU path defensively. The main weakness is small visual polish around dense metric text.

Would I ship this? With one fix: adjust the right-side metric layout so long disk values cannot crowd the label.

## Implementation B

Functionality:    7/10 - Covers the required metrics and CPU history, but the script-based launcher leaves more runtime/UI rough edges.
Design quality:   6/10 - The widget is readable, but the default title bar and visible PowerShell host make it feel less like a pinned desktop utility.
Code quality:     6/10 - Single-file PowerShell is pragmatic, but global state, embedded XAML, and limited cleanup make it less maintainable.
Decision quality: 7/10 - The zero-install PowerShell choice is defensible, though it undercuts the polished widget experience.
Polish:           5/10 - Docs are clear, but the run path visibly exposes the host console and the screenshot is a full desktop capture.

Total: 31/50

Verdict (3 sentences max): This is a functional prototype with a smart low-install story. It satisfies much of the spec, but it feels more like a script demo than a finished desktop widget. The visible console/title chrome are the biggest product-quality problems.

Would I ship this? With one fix: make the documented run path hide the PowerShell host and present only the widget window.

## Comparison

Which is stronger overall, and why (3 sentences max):
Implementation A is stronger overall because it delivers a cleaner native app with better structure, better polish, and a more widget-like user experience. B’s zero-install approach is useful, but it creates visible product roughness that the spec specifically pressures against.

Single biggest differentiator:
A feels like a small Windows desktop app; B still feels like a PowerShell-hosted utility.

What does NOT differ meaningfully between the two:
Both attempt the required CPU/RAM/GPU/disk metrics, 1-second updates, GPU fallback text, CPU history, README, DECISIONS, and screenshot deliverables.