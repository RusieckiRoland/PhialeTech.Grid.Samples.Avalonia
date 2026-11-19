# 03 — Query through a REST API

The client uses the library HTTP adapter. `StocksEndpoint` maps Grid requests to the API request and maps the API response back to records plus TotalCount. The normal ASP.NET Core server applies filters, sorting and paging through EF Core to the warehouse SQLite database. The server has no dependency on Grid.

Follow composition → HTTP options/mapping → server endpoint → EF query → response. Grouping is deliberately outside this small lesson; lesson 04 adds it. Bearer authentication belongs on the application's HttpClient or a token handler; do not place credentials in source code.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Terminal 1: keep the server running
dotnet run --project Samples/03-HttpRest/Server/HttpRest.Server.csproj -c Release

# Terminal 2: client
dotnet run --project Samples/03-HttpRest/Client/HttpRest.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
