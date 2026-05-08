# System Pulse

System Pulse is a small Windows 11 desktop widget for live system health. It stays pinned near the bottom-right screen corner and updates once per second.

It shows:

- CPU usage percentage with a 60-second history chart
- RAM used / total
- GPU usage when Windows exposes GPU engine counters
- `GPU not detected` when GPU counters are unavailable
- Primary disk used / total

## Install

Clone or copy this directory onto a Windows 11 machine. No package install is required.

If PowerShell script execution is restricted, run this once from the project directory:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

## Run

Single command after install:

```powershell
powershell -ExecutionPolicy Bypass -File .\SystemPulse.ps1
```

## Build From Source

There is no compile step. The app is a dependency-free PowerShell/WPF desktop widget using built-in Windows counters and CIM APIs.

To validate from source, run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\SystemPulse.ps1
```

Close the widget window to stop it.

## Notes

Metrics are sampled once per second. On a modern Windows 11 desktop, this keeps the widget lightweight while still feeling live. GPU availability depends on the Windows `GPU Engine` performance counters being present.
