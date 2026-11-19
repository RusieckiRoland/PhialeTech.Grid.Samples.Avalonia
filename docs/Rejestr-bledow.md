# Internal issue register

Do not export this register into the public website or package README automatically.

## AVA-SAMPLES-0001 — new context has no initial query

- Reported by: AI during the Avalonia port, 2026-11-19.
- Symptom: refresh cannot load a new context before its first confirmed query; an initial child Grid could remain empty.
- Area: example composition/view lifecycle, not a released component fix.
- Correction: issue an initial Query or LoadDataAsync, including newly attached native detail views. Reuse the retained detail session after collapse.
- Status/version: corrected locally in the new examples; no new NuGet version assigned.
- Evidence: native cases 01–08 passed on Windows and Linux; all desktop smoke startups passed.

## AVA-SAMPLES-0002 — test retains an expired native detail API

- Reported by: AI during the native scenario 08 test, 2026-11-19.
- Symptom: the test kept a view-scoped API after editing the master rebuilt its projection, causing ObjectDisposedException for the expired view lease.
- Correction: the test retrieves the current child view/API after projection changes. The application composition retains data sessions, not visual leases.
- Status/version: local test correction, no new NuGet version assigned.
- Evidence: case 08 independently verifies child persistence, retained master edits and shared/separate toolbar availability on both platforms.

## Component limitation observed during combined test setup

The previously recorded Grid issue GRID-TYPE-CACHE-20261117 also applies to preview.12: artificial co-hosting of separate example assemblies with identical linked model type names can reuse the wrong cached reflection metadata. Separate process tests model the actual independent applications; they do not fix that component issue. The canonical open register is Phiale.Grid/docs/Rejestr-bledow.md. No fix or release for that issue is claimed here.
