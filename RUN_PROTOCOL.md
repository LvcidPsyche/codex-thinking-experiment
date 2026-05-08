# Run Protocol

Goal: produce two artifact bundles that can be fairly compared.

## Per run (do this twice — once at low thinking, once at xhigh)

### Setup
1. Open Codex in a fresh session. Set thinking level (`low` or `xhigh`).
2. Create an empty working directory: `runs/_tmp_<level>/`.
3. Paste `PROMPT.md` verbatim. Do not paraphrase. Do not add context.
4. Start a wall-clock timer.

### During
- Do NOT answer follow-up questions if the model asks (the prompt forbids them — score the model lower if it asks anyway, but still don't engage).
- Do NOT intervene in design choices.
- Capture the full transcript to `runs/_tmp_<level>/transcript.md`.

### After model says "done"
1. Stop the wall-clock timer. Record `wall_clock_seconds` in `runs/_tmp_<level>/metrics.json`.
2. Record output token count from Codex (or estimated from transcript).
3. Follow the model's README. Time the install + first-run-to-visible-window.
4. Take the screenshot at `screenshot.png` if the model didn't.
5. Once the app is running, leave it for 60 seconds. Capture:
   - App CPU % (Task Manager, average over 30s of those 60s)
   - App RAM in MB
   - App's reported polling rate (visual eyeball is fine)
6. Run `cloc .` (or equivalent) in the dir. Record total LOC.
7. Record install size in MB (whatever the model produces — exe, folder, package).
8. Record dependency count (read package.json / Cargo.toml / requirements.txt).

### metrics.json template
```json
{
  "thinking_level": "low | xhigh",
  "wall_clock_seconds": 0,
  "output_tokens_est": 0,
  "stack": "e.g. Tauri+Svelte | Electron+React | Python+PyQt",
  "install_to_running_seconds": 0,
  "app_cpu_percent_avg": 0.0,
  "app_ram_mb": 0,
  "app_polling_hz_observed": 0,
  "loc_total": 0,
  "install_size_mb": 0,
  "dependency_count": 0,
  "asked_followup_questions": false,
  "crashed_during_60s_observation": false,
  "gpu_detection_correct": true
}
```

## Anonymization

After both runs are done:
1. Flip a coin. Heads = low becomes A, tails = low becomes B.
2. `mv runs/_tmp_low runs/<A or B>` and `mv runs/_tmp_xhigh runs/<the other>`.
3. Write the true mapping into `MAPPING.md`. **Do not open it again until all judges submit.**

## Sending to judges

Each judge gets:
- `PROMPT.md`
- `RUBRIC.md`
- The full contents of `runs/A/` and `runs/B/`

They do NOT get `metrics.json` files (those are the quantitative track — keep
them out of the qualitative judging to avoid biasing the rubric).

Save each judge's verdict to `judges/<name>.md`.
