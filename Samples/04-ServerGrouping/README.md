# 04 — Grouped queries on the server

The server groups the complete matching database result and calculates summaries. The client receives the requested groups/rows rather than grouping one downloaded page. Expand a group, change sorting or a filter, and observe the HTTP requests.

Compare the ordinary query in 03 with the grouped request/response mapping here. EF Core and SQLite remain server implementation details. Grid supplies the query conditions; application code defines what its API accepts and how groups are returned.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Terminal 1: keep the server running
dotnet run --project Samples/04-ServerGrouping/Server/ServerGrouping.Server.csproj -c Release

# Terminal 2: client
dotnet run --project Samples/04-ServerGrouping/Client/ServerGrouping.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
