# 07 — An application UserControl inside a row

Expand an asset row. A detail descriptor supplies identity, height and a detail ViewModel. `AssetDetailsFactory` creates an Avalonia UserControl for that descriptor. The detail is application UI, not a second Grid, and can show any business-specific content.

Follow the master record ID → detail descriptor → AssetDetailsViewModel → AssetDetailsView.axaml. The data/session belongs to composition; the view factory only creates native presentation.

## Run

.NET 8 SDK and a supported Avalonia desktop environment are required. Restore uses public NuGet packages. Run from the repository root.

```powershell
# Run the desktop client
dotnet run --project Samples/07-RowDetails/RowDetails.csproj -c Release
```

## Native Avalonia connection

The ViewModel exposes a Grid context. `MainWindow.axaml` binds `Context`; the view configures native columns with `ConfigureColumns`. Avalonia projection options are set through `Api.Projection`. The first read uses `LoadDataAsync`; later refreshes use the existing query. Views do not implement persistence.

Open the `.axaml` view, then its code-behind, ViewModel and composition. Common startup/styles are in `Shared`; business decisions remain in this lesson.
