# Judging Rubric — System Pulse

You are reviewing two anonymized implementations (`A/` and `B/`) of the same spec.
You do not know which is which. Be honest, be specific, be brief.

## Score each implementation 0–10 on each axis

### 1. Functionality (0–10)
Does it actually run? Are the numbers correct? Does the polling honor the ≥1Hz target?
Does it handle missing GPU? Does the historical view work?

### 2. Design quality (0–10)
Visual polish, layout, information hierarchy, density. Does it look like something
a human would willingly leave on their screen?

### 3. Code quality (0–10)
Organization, idiomatic use of the chosen stack, maintainability, dependency hygiene.
Would a senior engineer be okay inheriting this?

### 4. Decision quality (0–10)
Read `DECISIONS.md`. Were the choices defensible? Is the reasoning honest about
trade-offs, or is it post-hoc justification of arbitrary picks?

### 5. Polish (0–10)
Edge cases, error states, install ergonomics, README clarity, "feels finished."

## Required output (per implementation)

```
## Implementation [A or B]

Functionality:    X/10 — one-line reason
Design quality:   X/10 — one-line reason
Code quality:     X/10 — one-line reason
Decision quality: X/10 — one-line reason
Polish:           X/10 — one-line reason

Total: XX/50

Verdict (3 sentences max): ...

Would I ship this? Yes / No / With one fix: <what fix>
```

## Then a final comparison

```
## Comparison

Which is stronger overall, and why (3 sentences max):
Single biggest differentiator:
What does NOT differ meaningfully between the two:
```

Do not speculate about which thinking level produced which. Score the artifact in front of you.
