extern alias one;
extern alias two;
extern alias three;
extern alias four;
extern alias five;
extern alias six;
extern alias seven;
extern alias eight;
extern alias restserver;
extern alias groupserver;
extern alias priceserver;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PhialeGrid.Contracts;
using PhialeGrid.Core;
using PhialeGrid.Core.Interaction;
using PhialeGrid.Core.Regions;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeSamples.NativeTests;

[NonParallelizable]
public class ScenarioTests
{
    private static string Seed => Path.Combine(AppContext.BaseDirectory, "Data", "local-editing.sqlite");
    private static string TemporaryDatabase()
    {
        var file = Path.Combine(Path.GetTempPath(), "PhialeAva-" + Guid.NewGuid().ToString("N") + ".sqlite");
        File.Copy(Seed, file);
        return file;
    }
    private static void Pump(Window window)
    {
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick(2);
    }
    private static async Task Wait(Window window, Func<bool> ready)
    {
        var end = DateTime.UtcNow.AddSeconds(40);
        while (!ready())
        {
            if (DateTime.UtcNow > end)
                throw new TimeoutException("View did not reach expected state: " + window.DataContext?.GetType().GetProperty("Status")?.GetValue(window.DataContext) + "; " + string.Join(";", window.GetVisualDescendants().OfType<GridControl>().Select(g => g.Name + ":" + g.Api.Data.Current?.HasConfirmedQuery + ":" + g.Api.Data.Current?.LoadError)));
            await Task.Delay(20);
            Pump(window);
        }
        Pump(window);
    }
    private static GridControl Grid(Window window, string name) => window.FindControl<GridControl>(name)!;
    private static bool Loaded(GridControl grid) => grid.Api.Data.Current?.HasConfirmedQuery == true && !grid.Api.Data.Current.IsBusy;
    private static Button Button(GridControl grid, string name) => grid.GetVisualDescendants().OfType<Button>().First(item => item.Name == name);
    private static void Click(Window window, Button button)
    {
        Pump(window);
        Assert.That(button.IsEnabled && button.IsEffectivelyVisible, Is.True, button.Name);
        button.RaiseEvent(new RoutedEventArgs(global::Avalonia.Controls.Button.ClickEvent));
        Pump(window);
    }
    private static void Capture(Window window, string name)
    {
        var dir = Environment.GetEnvironmentVariable("PHIALE_DEMO_SCREENSHOTS");
        if (string.IsNullOrEmpty(dir))
            return;
        Directory.CreateDirectory(dir);
        Pump(window);
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)window.Bounds.Width, (int)window.Bounds.Height));
        bitmap.Render(window);
        bitmap.Save(Path.Combine(dir, name + ".png"));
    }
    [AvaloniaTest]
    public async Task Example01LoadsRecordsAndPagesLocally()
    {
        var repository = new one::PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Infrastructure.SqliteStockRepository(restserver::PhialeTech.Grid.Samples.HttpRest.Server.StockServer.FindDatabase());
        using var source = new one::PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Composition.StockGridComposition();
        var api = GridApiFactory.CreateRuntime();
        var model = new one::PhialeTech.Grid.Samples.Avalonia.SqliteBinding.ViewModels.StocksViewModel(repository, source.CreateBinding, api) { SelectedSize = 10000 };
        var window = new one::PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Views.MainWindow(api, model);
        window.Show();
        try
        {
            await Wait(window, () => model.IsIdle && model.Grid != null && Loaded(Grid(window, "StockGrid")));
            Assert.That(model.Rows.Count, Is.EqualTo(10000));
            Assert.That(api.Projection.IsPagingEnabled, Is.True);
            Capture(window, "01-records");
        }
        finally { window.Close(); await Wait(window, () => !window.IsVisible); }
    }
    [AvaloniaTest]
    public async Task Example02LoadsOnlyRequestedWindows()
    {
        var repository = new two::PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Infrastructure.StockRepository(restserver::PhialeTech.Grid.Samples.HttpRest.Server.StockServer.FindDatabase());
        using var source = new two::PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Composition.StockGridComposition(repository);
        var api = GridApiFactory.CreateRuntime();
        var model = new two::PhialeTech.Grid.Samples.Avalonia.DataVirtualization.ViewModels.StocksViewModel(source.CreateBinding, api) { SelectedSize = 100000 };
        var window = new two::PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Views.MainWindow(api, model);
        window.Show();
        try
        {
            await Wait(window, () => model.Grid != null && Loaded(Grid(window, "StockGrid")));
            Assert.That(api.Data.Current.TotalCount, Is.EqualTo(100000));
            Assert.That(api.Projection.IsPagingEnabled, Is.False);
            Assert.That(api.Projection.EnableDataVirtualization, Is.True);
            await Grid(window, "StockGrid").LoadDataAsync(90000, 100);
            Assert.That(api.Data.Current.VisibleRecords.Count, Is.LessThanOrEqualTo(100));
            Capture(window, "02-query");
        }
        finally { window.Close(); await Wait(window, () => !window.IsVisible); }
    }
    [AvaloniaTest]
    public async Task Example03UsesTheRealRestServer()
    {
        await using var server = restserver::PhialeTech.Grid.Samples.HttpRest.Server.StockServer.Create("http://127.0.0.1:0");
        await server.StartAsync();
        var address = server.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var source = new three::PhialeTech.Grid.Samples.Avalonia.HttpRest.Composition.StockGridComposition(address);
        var api = GridApiFactory.CreateRuntime();
        var model = new three::PhialeTech.Grid.Samples.Avalonia.HttpRest.ViewModels.StocksViewModel(source.CreateBinding, api);
        var window = new three::PhialeTech.Grid.Samples.Avalonia.HttpRest.Views.MainWindow(api, model);
        window.Show();
        try
        {
            await Wait(window, () => Loaded(Grid(window, "StocksGrid")));
            Assert.That(api.Data.Current.TotalCount, Is.EqualTo(1000000));
            Assert.That(api.Data.Current.VisibleRecords.Count, Is.EqualTo(100));
            Capture(window, "03-rest");
        }
        finally { window.Close(); await server.StopAsync(); }
    }
    [AvaloniaTest]
    public async Task Example04GroupsOnTheServer()
    {
        await using var server = groupserver::PhialeTech.Grid.Samples.HttpRest.Server.GroupingServer.Create("http://127.0.0.1:0");
        await server.StartAsync();
        var address = server.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var source = new four::PhialeTech.Grid.Samples.Avalonia.ServerGrouping.Composition.StockGridComposition(address);
        var api = GridApiFactory.CreateRuntime();
        var model = new four::PhialeTech.Grid.Samples.Avalonia.ServerGrouping.ViewModels.StocksViewModel(source.CreateBinding, api);
        var window = new four::PhialeTech.Grid.Samples.Avalonia.ServerGrouping.Views.MainWindow(api, model);
        window.Show();
        try
        {
            await Wait(window, () => Loaded(Grid(window, "StocksGrid")));
            Assert.That(api.Projection.Groups.Count, Is.EqualTo(2));
            Assert.That(api.Data.Current.TotalCount, Is.GreaterThan(0));
            Capture(window, "04-server-groups");
        }
        finally { window.Close(); await server.StopAsync(); }
    }
    [AvaloniaTest]
    public async Task Example05BuffersPersistsAndSwitchesToImmediate()
    {
        var database = TemporaryDatabase();
        using var source = await five::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Composition.AssetGridComposition.CreateAsync(database);
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<five::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Models.AssetRow>(api, source.Binding);
        var model = new five::PhialeTech.Grid.Samples.Avalonia.LocalEditing.ViewModels.AssetsViewModel(context, database);
        var window = new five::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Views.MainWindow(api, model);
        window.Show();
        try
        {
            var grid = Grid(window, "AssetsGrid");
            await Wait(window, () => Loaded(grid));
            var repository = new five::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Infrastructure.AssetRepository(database);
            var original = repository.ReadAll().Single(row => row.Id == "EQ-001").Name;
            Assert.That(context.SetValue("EQ-001", "Name", "Zmiana w buforze"), Is.True);
            Pump(window);
            Assert.That(repository.ReadAll().Single(row => row.Id == "EQ-001").Name, Is.EqualTo(original));
            Assert.That(context.PendingRecordCount, Is.EqualTo(1));
            Capture(window, "05-buffered");
            Assert.That((await context.SaveAsync()).Succeeded, Is.True);
            Assert.That(repository.ReadAll().Single(row => row.Id == "EQ-001").Name, Is.EqualTo("Zmiana w buforze"));
            model.IsImmediate = true;
            Pump(window);
            Assert.That(Button(grid, "GridCommit").IsVisible, Is.False);
            Assert.That(Button(grid, "GridCancelChanges").IsVisible, Is.False);
            api.Editing.BeginRecordEdit("EQ-001");
            Assert.That(context.SetValue("EQ-001", "Name", "Zapis natychmiastowy"), Is.True);
            Assert.That(repository.ReadAll().Single(row => row.Id == "EQ-001").Name, Is.EqualTo("Zmiana w buforze"));
            api.Editing.CompleteRecordEdit("EQ-001", true);
            await Wait(window, () => !context.HasPendingChanges && !context.IsBusy);
            Assert.That(repository.ReadAll().Single(row => row.Id == "EQ-001").Name, Is.EqualTo("Zapis natychmiastowy"));
            Capture(window, "05-immediate");
        }
        finally { context.DiscardChanges(); window.Close(); }
    }
    [AvaloniaTest]
    public async Task Example06DownloadsSnapshotAndEditsWithoutAnotherRead()
    {
        var folder = Path.Combine(Path.GetTempPath(), "PhialeAvaPrices-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        await using var server = priceserver::PhialeTech.Grid.Samples.PriceReview.Server.PriceServer.Create("http://127.0.0.1:0", Path.Combine(folder, "prices.sqlite"));
        await priceserver::PhialeTech.Grid.Samples.PriceReview.Server.PriceServer.InitializeAsync(server);
        await server.StartAsync();
        var address = server.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var source = new six::PhialeTech.Grid.Samples.Avalonia.RestSnapshot.Composition.SnapshotComposition();
        var api = GridApiFactory.CreateRuntime();
        var model = new six::PhialeTech.Grid.Samples.Avalonia.RestSnapshot.ViewModels.PriceReviewViewModel(api, source) { Address = address };
        var window = new six::PhialeTech.Grid.Samples.Avalonia.RestSnapshot.Views.MainWindow(api, model);
        window.Show();
        try
        {
            Pump(window);
            Assert.That(model.HasNoGrid, Is.True);
            await model.LoadAsync();
            await Wait(window, () => model.Grid != null && Loaded(Grid(window, "PricesGrid")));
            Assert.That(model.Grid!.TotalCount, Is.EqualTo(10000));
            var requests = source.RequestCount;
            var row = (six::PhialeTech.Grid.Samples.PriceReview.Contracts.PriceRow)api.Data.Current.VisibleRecords[0];
            Assert.That(model.Grid.SetValue(row.Id, "Comment", "Przegląd lokalny"), Is.True);
            Assert.That(source.RequestCount, Is.EqualTo(requests));
            Capture(window, "06-snapshot");
            // Commit really persists through REST; a fresh download must retain the edit.
            await model.SaveAsync();
            Assert.That(model.Grid.HasPendingChanges, Is.False, model.SaveReport);
            Assert.That(source.RequestCount, Is.GreaterThan(requests));
            await model.LoadAsync();
            await Wait(window, () => Loaded(Grid(window, "PricesGrid")));
            var saved = api.Data.Current.VisibleRecords
                .Cast<six::PhialeTech.Grid.Samples.PriceReview.Contracts.PriceRow>()
                .Single(record => record.Id == row.Id);
            Assert.That(saved.Comment, Is.EqualTo("Przegląd lokalny"));
            model.Grid.DiscardChanges();
        }
        finally { window.Close(); await server.StopAsync(); }
    }
    [AvaloniaTest]
    public async Task Example07EmbedsTheApplicationUserControl()
    {
        using var source = new seven::PhialeTech.Grid.Samples.Avalonia.RowDetails.Composition.AssetGridComposition(seven::PhialeTech.Grid.Samples.Avalonia.RowDetails.Infrastructure.AssetCatalog.Read());
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<seven::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Models.AssetRow>(api, source.Binding);
        var model = new seven::PhialeTech.Grid.Samples.Avalonia.RowDetails.ViewModels.AssetsViewModel(context);
        var window = new seven::PhialeTech.Grid.Samples.Avalonia.RowDetails.Views.MainWindow(model);
        window.Show();
        try
        {
            await Wait(window, () => Loaded(Grid(window, "AssetsGrid")));
            api.Details.Expand("EQ-001");
            Grid(window, "AssetsGrid").RefreshProjection();
            Pump(window);
            Assert.That(window.GetVisualDescendants().OfType<seven::PhialeTech.Grid.Samples.Avalonia.RowDetails.Views.AssetDetailsView>().Count(), Is.EqualTo(1));
            Capture(window, "07-user-control");
        }
        finally { window.Close(); await Wait(window, () => !window.IsVisible); }
    }
    [AvaloniaTest]
    public async Task Example08PreservesIndependentEditsAndRoutesTheSharedBar()
    {
        var database = TemporaryDatabase();
        var repository = new eight::PhialeTech.Grid.Samples.Avalonia.NestedGrid.Infrastructure.LocationRepository(database);
        using var source = new eight::PhialeTech.Grid.Samples.Avalonia.NestedGrid.Composition.LocationGridComposition(repository);
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<eight::PhialeTech.Grid.Samples.Avalonia.NestedGrid.Models.LocationRow>(api, source.Binding);
        var model = new eight::PhialeTech.Grid.Samples.Avalonia.NestedGrid.ViewModels.LocationsViewModel(context, database);
        var window = new eight::PhialeTech.Grid.Samples.Avalonia.NestedGrid.Views.MainWindow(model, source);
        window.Show();
        try
        {
            var root = Grid(window, "LocationsGrid");
            await Wait(window, () => Loaded(root));
            Assert.That(model.Columns.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(api.Details.UseSharedCommandBar, Is.True);
            var parent = repository.ReadLocations()[0];
            api.Details.Expand(parent.Id);
            root.RefreshProjection();
            Pump(window);
            GridControl Detail() => window.GetVisualDescendants().OfType<GridControl>().Single(grid => grid != root);
            await Wait(window, () => Loaded(Detail()));
            Assert.That(context.SetValue(parent.Id, "Contact", "Kontakt po edycji"), Is.True);
            Pump(window);
            var asset = (eight::PhialeTech.Grid.Samples.Avalonia.LocalEditing.Models.AssetRow)Detail().Api.Data.Current.VisibleRecords[0];
            Assert.That(Detail().Api.Editing.TrySetFieldValue(asset.Id, "Name", "Wyposażenie po edycji"), Is.True);
            Detail().RefreshProjection();
            Detail().Api.Details.ActivateCommandBarTarget();
            Pump(window);
            Click(window, Button(root, "GridCommit"));
            await Wait(window, () => !Detail().Api.Editing.IsCommitting && !Detail().Api.Editing.HasPendingEdits);
            Assert.That(context.HasPendingChanges, Is.True);
            Assert.That(repository.Assets.ReadAll().Single(row => row.Id == asset.Id).Name, Is.EqualTo("Wyposażenie po edycji"));
            Assert.That(repository.ReadLocations().Single(row => row.Id == parent.Id).Contact, Is.EqualTo(parent.Contact));
            Assert.That((await context.SaveAsync()).Succeeded, Is.True);
            Assert.That(repository.ReadLocations().Single(row => row.Id == parent.Id).Contact, Is.EqualTo("Kontakt po edycji"));
            Capture(window, "08-shared-master-detail");
            root.UseSharedDetailCommandBar = false;
            Pump(window);
            Assert.That(Detail().Api.Workspace.ResolveCurrent().Single(region => region.RegionKind == GridRegionKind.TopCommandRegion).IsAvailable, Is.True);
            Capture(window, "08-separate-bars");
        }
        finally { context.DiscardChanges(); window.Close(); }
    }
}
