# 02 — Query: read only the requested window

Compare this lesson with 01. The composition uses `.Query(new StockQuerySource(...))` instead of `.Records(rows)`. Grid calls `StockQuerySource.QueryAsync` with Offset, Size, filters and sorts. The repository applies those conditions to the complete SQLite result and returns only the requested window plus its total count.

The view enables `Api.Projection.EnableDataVirtualization` and disables paging for scroll-to-load. Paging can use the same Query source; it changes how the user selects a window, not the source contract. UI virtualization is active in both cases. No ReadAll call is needed to supply this query session.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Run the desktop client
dotnet run --project Samples/02-DataVirtualization/DataVirtualization.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
