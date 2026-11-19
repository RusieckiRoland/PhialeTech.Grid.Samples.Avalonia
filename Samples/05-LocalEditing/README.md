# 05 — Local editing: buffer or immediate record save

The repository reads 24 synthetic assets into memory. `.Records(rows)` supplies them to Grid; `.CommitUsing(new SqliteCommitPort(...))` defines the only path that persists edits. The application uses a writable working copy, leaving the source example database unchanged.

`.Columns(AssetColumns.All)` supplies editors and validation, not records. `.Key(row => row.Id)` is stable identity, not row position. `.Clone(row => row with { })` creates a shallow working copy: AssetRow is a record with scalar/string fields. A normal class needs an explicit copy function; nested mutable objects need independent copies.

Set `Grid.SaveMode` on the ViewModel context after creating it and before editing. In Buffered mode Enter/Apply posts the record to the buffer; Save commits all pending changes and Discard removes them. The Changes panel shows before/after values. In Immediate mode persistence runs after the entire record is accepted; batch Save/Discard and the pending counter are hidden. Immediate mode does not mean saving every keystroke. Resolve active edits and pending changes before switching modes.

Try editing Quantity, cancelling a cell, restoring a buffered record, saving, and reopening the database. Then switch to Immediate and accept a complete record. The native checks verify both persistence paths.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Run the desktop client
dotnet run --project Samples/05-LocalEditing/LocalEditing.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
