# System Pulse

System Pulse is a small Windows 11 desktop widget for live system health. It pins itself to the bottom-right corner, stays on top, and refreshes metrics once per second.

It shows:

- CPU usage with a 60-second history chart
- RAM used / total
- GPU usage when Windows exposes GPU performance counters
- Primary system disk used / total
- A clear `GPU not detected` state when GPU counters are unavailable

## Install

Install the .NET 8 SDK on Windows 11:

```powershell
winget install Microsoft.DotNet.SDK.8
```

Then clone or copy this directory and open PowerShell in the repo root.

## Run

Single command after install:

```powershell
dotnet run --project .\src\SystemPulse\SystemPulse.csproj -c Release
```

The widget opens in the bottom-right corner. Drag the widget to move it. Right-click it and choose `Exit` to close it.

## Build From Source

```powershell
dotnet build .\src\SystemPulse\SystemPulse.csproj -c Release
```

To publish a runnable folder:

```powershell
dotnet publish .\src\SystemPulse\SystemPulse.csproj -c Release -r win-x64 --self-contained false -o .\dist
```

Run the published app:

```powershell
.\dist\SystemPulse.exe
```

## Notes

System Pulse uses native Windows APIs for CPU and memory and Windows PDH GPU counters. If the GPU counter is missing or unavailable, the widget keeps running and displays `GPU not detected`.
