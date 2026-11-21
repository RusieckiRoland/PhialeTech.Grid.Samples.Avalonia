# Phiale.Grid — Avalonia examples

Eight scenarios corresponding to the WPF examples, with the same numbering and business behavior.
Every lesson owns its columns, data composition and ViewModel. Shared contains only desktop startup,
Design styles and application dialogs. Grid and Design are consumed as published NuGet packages;
there are no references to private component source projects or the WPF samples checkout.

| Example | Data strategy | What to learn |
| --- | --- | --- |
| 01 — SQLite | Records, whole local set | Load records once, then work in memory. |
| 02 — Data virtualization | Query, local on-demand reads | Read only requested windows; filters/sorts/groups apply at the source. |
| 03 — HTTP REST | Remote Query | Adapt Grid requests to an EF/SQLite REST API. |
| 04 — Server grouping | Remote grouped Query | Group and summarize the full result on the server. |
| 05 — Local editing | Local records and commit port | Typed editors, validation, Buffered/Immediate, review/discard/save. |
| 06 — REST snapshot | Snapshot and remote commit | Download a selected price list, edit locally, validate versions on commit. |
| 07 — Custom detail | Local records | Embed an application UserControl beneath a row. |
| 08 — Nested Grid | Local master/detail sessions | Edit master and detail independently, optionally with one shared command bar. |

Open PhialeTech.Grid.Samples.Avalonia.sln. Start the first lesson from this directory:

```powershell
dotnet run --project Samples/01-SqliteBinding/SqliteBinding.csproj -c Release
```
For 03/04/06 first start its Server project in a separate terminal, then its client.
The application restores Grid 2.0.0-preview.13 and Design.Avalonia 0.1.0-preview.52 from NuGet.org.
The public source database uses Git LFS. After a clone run git lfs install and git lfs pull.

Example code and synthetic data are MIT; component packages retain their own Phiale license.
The author requested equivalent scenarios after the WPF release and clean Windows VM acceptance.
AI adapted the author's existing example code to Avalonia; no third-party repository implementation was used.

## Verification

Build Debug or Release with `dotnet build PhialeTech.Grid.Samples.Avalonia.sln -c Release`.
Run each of the five data runners with `dotnet run --project Tests/<name>/<name>.csproj -c Release`:
SqliteBinding.Checks, DataVirtualization.Checks, HttpRest.Checks, ServerGrouping.Checks and LocalEditing.Checks.
Run `dotnet test Tests/Native.Checks/Native.Checks.csproj -c Release` for all eight native UI scenarios.
Each native case runs in an independent process, matching the separate applications. These are real Avalonia controls rendered headlessly; they do not establish GPU/driver performance.
Screenshots and test logs are written under ignored `artifacts/`.
For a short actual desktop startup check: `dotnet run --project <client.csproj> -c Release -- --smoke`.

Accepted on 2026-11-19: Debug/Release builds with zero warnings/errors, five data runners and eight native scenario checks on Windows and Linux (headless UI), plus all eight actual Windows desktop startup checks. See [verification scope](docs/Verification.md) for evidence and limits.

Use the repository NuGet.Config to restore the published packages from NuGet.org.
