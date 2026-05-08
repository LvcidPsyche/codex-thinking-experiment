# Task — System Pulse

Build **System Pulse**, a Windows 11 desktop widget that shows live system health.

## Hard requirements
- Single command to run after install (document it in README).
- Updates the displayed metrics at least once per second.
- The app itself must stay under 5% CPU on a modern desktop while running.
- Shows live: CPU %, RAM used / total, GPU % (if available), primary disk used / total.
- Shows at least one historical view — e.g. a 60-second line chart for CPU.
- Handles missing GPU gracefully (no crash, no empty widget — say "GPU not detected").
- Small footprint UI. Should look at home pinned to a screen corner.
- README with: what it does, how to install, how to run, how to build from source.
- A `DECISIONS.md` (≤300 words) explaining your main design choices and why.

## Free choices (this is the test)
You decide everything else: language, framework, layout, theme, polling cadence,
which metric gets prime real estate, how history is visualized, error UX, packaging.

## Constraints
- No follow-up questions. Read the spec, make decisions, ship.
- One pass. Don't ask the user to clarify or confirm.
- Target machine: Windows 11, 16GB RAM, mid-range GPU. Assume nothing else.

## Deliverables
- Source code in this directory.
- `README.md` with install + run + build steps.
- `DECISIONS.md` justifying main design choices (≤300 words).
- A screenshot of the app running (any image format), checked into the repo as `screenshot.png`.

## Done = a human can clone this dir, follow the README, and have System Pulse running in under 3 minutes.
