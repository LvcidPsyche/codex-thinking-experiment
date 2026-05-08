# Apps

The public repo includes both runnable versions of System Pulse.

## xHigh System Pulse

Path: `apps/xhigh-system-pulse`

Stack: .NET 8 WPF

Run:

```powershell
cd apps\xhigh-system-pulse
dotnet run --project .\src\SystemPulse\SystemPulse.csproj -c Release
```

## Low System Pulse

Path: `apps/low-system-pulse`

Stack: PowerShell + WPF

Run:

```powershell
cd apps\low-system-pulse
powershell -ExecutionPolicy Bypass -File .\SystemPulse.ps1
```
