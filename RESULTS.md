# Results — Codex 5.5 Low vs xHigh

Mapping revealed after all three judge verdicts were collected:

- Implementation A = xHigh
- Implementation B = Low

## Quantitative

| Metric                          | Low              | xHigh           | Δ |
|---------------------------------|------------------|-----------------|---|
| Wall-clock seconds              | 499.502          | 793.877         | xHigh +294.375s / 1.59x |
| Output tokens                   | 10,839           | 27,032          | xHigh +16,193 / 2.49x |
| Reasoning output tokens         | 2,743            | 11,335          | xHigh +8,592 / 4.13x |
| Stack chosen                    | PowerShell + WPF | .NET 8 WPF      | n/a |
| Install → running (s)           | 0.449            | 7.289           | xHigh +6.840s |
| App CPU % (avg, 30s)            | 0.150            | 0.088           | xHigh -0.062 |
| App RAM (MB)                    | 158.1            | 128.9           | xHigh -29.2 |
| Observed polling Hz             | 1.0              | 1.0             | same |
| Total LOC                       | 226              | 695             | xHigh +469 / 3.08x |
| Install size (MB)               | 0.229            | 0.188           | xHigh -0.041 |
| Dependency count                | 0                | 0               | same |
| Asked follow-ups (forbidden)    | false            | false           | same |
| Crashed in 60s                  | false            | false           | same |
| GPU detection correct           | true             | true            | same |

Notes: CPU is normalized by logical processor count. LOC excludes transcripts, metrics, screenshots, generated build folders, and orchestration-only files. xHigh install size is the framework-dependent `dotnet publish` output; Low install size is the script artifact folder excluding metrics/transcript.

## Qualitative — average judge scores

| Axis             | Low avg | xHigh avg | Δ |
|------------------|---------|-----------|---|
| Functionality    | 7.67    | 9.33      | xHigh +1.67 |
| Design quality   | 6.00    | 8.67      | xHigh +2.67 |
| Code quality     | 6.33    | 9.00      | xHigh +2.67 |
| Decision quality | 8.00    | 8.33      | xHigh +0.33 |
| Polish           | 6.00    | 8.67      | xHigh +2.67 |
| **Total /50**    | **34.00** | **44.00** | **xHigh +10.00** |

## Per-judge scores

| Judge   | Low total | xHigh total | Picked stronger |
|---------|-----------|-------------|------------------|
| Claude  | 32        | 44          | xHigh / A |
| Codex   | 31        | 43          | xHigh / A |
| Gemini  | 39        | 45          | xHigh / A |

## Inter-judge agreement

- All three picked the same winner: yes
- Disagreement axis: none material; all judges favored xHigh overall.
- Largest single-axis disagreement: 2 points, across several Low-axis scores such as functionality/design/decision quality.

## Findings

1. What surprised me most: Low produced a genuinely usable zero-install widget, not just a broken script.
2. What I expected and was confirmed: xHigh spent far more tokens and wall-clock time.
3. What I expected and was wrong about: xHigh did not just add complexity; judges viewed most of the added structure as real maintainability and polish.
4. The single biggest functional difference: xHigh used a compiled WPF app with async/background metric sampling and native Windows APIs; Low used a single PowerShell/WPF script with more UI-thread risk.
5. The single biggest aesthetic difference: xHigh looked like a borderless pinned widget; Low looked like a small PowerShell-hosted window with standard chrome.
6. Cost-per-quality-point (output tokens / average total score):
   - Low: 319 output tokens per judge-score point
   - xHigh: 614 output tokens per judge-score point
   - Marginal lift: 1,619 extra output tokens per additional judge-score point
7. Bottom line: use Low when the goal is a quick local utility or first-pass prototype; use xHigh when maintainability, UI polish, and "ship it tomorrow" quality matter.
