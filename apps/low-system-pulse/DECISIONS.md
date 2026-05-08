# Decisions

System Pulse is implemented as a single PowerShell/WPF script because Windows 11 ships with PowerShell, WPF, CIM, and performance counters. That keeps install time close to zero and satisfies the single-command run requirement without asking the user to install Node, Python packages, .NET SDK, or a native desktop framework.

The widget polls once per second. That cadence is fast enough for live health feedback and slow enough to keep the app below the 5% CPU target on a normal desktop. Metric collection uses built-in Windows APIs: `Get-Counter` for CPU/GPU and `Get-CimInstance` for memory and disk.

CPU gets prime placement because it is the most volatile health signal. A 60-sample queue feeds a lightweight WPF polyline chart for the required historical view. RAM, disk, and GPU are shown as compact text blocks to preserve the small corner-widget footprint.

GPU is intentionally best-effort. Windows exposes GPU usage through performance counters only when the driver and counter set are available. If those counters are missing or fail, the widget displays `GPU not detected` instead of crashing or leaving an empty area.

The UI uses a restrained dark Windows-style theme, fixed small dimensions, and topmost positioning near the bottom-right work area so it feels like a pinned desktop utility rather than a full application.
