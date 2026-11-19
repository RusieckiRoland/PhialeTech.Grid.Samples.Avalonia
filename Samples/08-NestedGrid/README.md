# 08 — Editable master and detail, one or two command bars

The master contains location ID, contact, phone and notes. `.Records(repository.ReadLocations())` supplies the master rows; `LocationCommitPort` saves master edits. Expanding a location uses its stable ID to request the matching assets and create a separate detail session with its own commit port.

`_details` is a dictionary of data sessions keyed by location ID, not a dictionary of visual tables. A child view borrows its session. Closing/collapsing that view does not discard pending detail edits. This small example retains sessions for six locations until composition is disposed; a large master would need an explicit retention/eviction policy that protects unsaved work. UI virtualization alone does not free that dictionary or virtualize its data.

`UseSharedDetailCommandBar` selects one master toolbar routed to the active Grid. In shared mode the detail toolbar is unavailable. Turn the option off to give each Grid its own toolbar. Master and detail keep independent edit buffers and commit ports: saving the active child must not silently save the master. Never retain a child view API across a master projection rebuild; the data session outlives a particular view lease.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Run the desktop client
dotnet run --project Samples/08-NestedGrid/NestedGrid.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
