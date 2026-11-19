# 01 — Records: read once, query in memory

The repository reads the selected warehouse set from SQLite. `StockGridComposition` passes those rows to `.Records(rows)`. Grid filters, sorts, groups and pages that in-memory set. UI virtualization remains active; this lesson does not virtualize data reads. Changing the visible page does not read SQLite again.

Follow `StocksViewModel.LoadAsync` → repository read → composition → `Grid.Context`. `.Columns` defines fields, `.Key` identifies records, and `.Clone` supplies a working-copy function. StockRow is a record containing scalar values, so `row with { }` is sufficient here.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Run the desktop client
dotnet run --project Samples/01-SqliteBinding/SqliteBinding.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
