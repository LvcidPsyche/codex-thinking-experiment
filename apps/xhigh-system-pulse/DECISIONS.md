# Decisions

System Pulse is a WPF app targeting `net8.0-windows`. WPF keeps the runtime footprint small, ships with the Windows Desktop .NET runtime, and avoids Electron or browser-process overhead. That is the simplest way to hit the one-command run requirement while staying comfortably below the 5% CPU budget on a modern desktop.

Metrics refresh every second. CPU and RAM use direct Windows APIs instead of shelling out, which keeps polling cheap and avoids visible stutter. GPU usage uses the Windows PDH `GPU Engine(*)\Utilization Percentage` counter because it is available on modern Windows systems without extra drivers or packages. If that counter is missing, the UI explicitly says `GPU not detected`.

The widget is borderless, always-on-top, and pinned to the bottom-right working area at launch. CPU gets the largest visual treatment because it changes fastest and benefits most from history. RAM, GPU, and disk are compact bars to keep the widget useful without becoming a dashboard.

The CPU chart keeps only the last 60 samples in memory, so the historical view is fixed-cost. The app has no external NuGet dependencies; clone, run, and build all go through the .NET SDK.
