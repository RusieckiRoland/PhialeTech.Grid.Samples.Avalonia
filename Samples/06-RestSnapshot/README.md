# 06 — Download a price list, review locally, commit remotely

A selection form appears before the Grid. Choose a supplier and download its price-review snapshot. The operator can edit proposed sale prices, effective dates, decisions and comments locally. Navigation, filters and grouping operate on the downloaded set without sending another query for each action.

Only Commit sends pending changes to the server. The EF/SQLite server checks record versions, business validation and conflicts. A rejected save must not be presented as a successful write. The second-operator scenario demonstrates a concurrent modification. Read the response and keep/reconcile rejected work rather than silently overwriting another operator.

Follow the selection ViewModel, snapshot composition, commit port and server transaction in that order. Reading and committing are separate operations; local edits are not automatically persisted merely because they appear in the Grid.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Terminal 1: keep the server running
dotnet run --project Samples/06-RestSnapshot/Server/PriceReview.Server.csproj -c Release

# Terminal 2: client
dotnet run --project Samples/06-RestSnapshot/RestSnapshot.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
