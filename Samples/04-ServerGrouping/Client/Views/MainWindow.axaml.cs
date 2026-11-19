using Avalonia.Threading;
using PhialeGrid.Core.Interaction;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Contracts;
using PhialeGrid.Core.Regions;
using PhialeGrid.Core.Query;
using PhialeGrid.Core.Summaries;
using PhialeTech.Grid.Samples.Avalonia.ServerGrouping.ViewModels;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.ServerGrouping.Views;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _lifetime = new();
    private bool _closeRequested;
    public MainWindow(IGridApi api, StocksViewModel model)
    {
        AvaloniaXamlLoader.Load(this);
        StocksGrid = this.FindControl<GridControl>("StocksGrid")!;
        // Avalonia uses the same neutral projection API instead of WPF dependency properties.
        StocksGrid.BindRuntime(api);
        StocksGrid.ConfigureColumns(model.Columns);
        api.Projection.IsPagingEnabled = true;
        api.Projection.PageSize = 100;
        api.Projection.EnableDataVirtualization = true;
        api.Configuration.FilterRowHeight = 32;
        api.Projection.Groups = [new("WarehouseName"), new("Category")];
        api.Projection.Summaries = [new("ProductCode", GridSummaryType.Count), new("OnHand", GridSummaryType.Sum),
            new("Reserved", GridSummaryType.Sum), new("Available", GridSummaryType.Sum)];
        api.Projection.ShowGroupSummaries = true;
        api.Workspace.Execute(new(GridRegionCommandKind.Close, GridRegionKind.TopCommandRegion));
        api.Workspace.Execute(new(GridRegionCommandKind.Close, GridRegionKind.SummaryBottomRegion));

        api.Workspace.SetCapabilityPolicy(new Configuration.GroupingExamplePolicy());
        DataContext = model;
        Opened += async (_, _) => { try { await StocksGrid.LoadDataAsync(0, 100, _lifetime.Token); } catch (OperationCanceledException) { } catch (PhialeGrid.Core.Data.GridDataReadException) { /* Core exposes the read error and retry state. */ } };
        // A viewport read may still be completing when the user closes the window.
        // Keep the view alive until Core releases its operation scope; never block the UI thread.
        Closing += (_, args) =>
        {
            if (StocksGrid.CanCloseRuntime)
                return;
            args.Cancel = true;
            if (_closeRequested)
                return;
            _closeRequested = true;
            _lifetime.Cancel();
            if (model.Grid?.CancelCommand.CanExecute(null) == true)
                model.Grid.CancelCommand.Execute(null);
            Dispatcher.UIThread.Post(async () =>
            {
                while (!StocksGrid.CanCloseRuntime)
                    await Task.Delay(25);
                Close();
            });
        };
        Closed += (_, _) =>
        {
            _lifetime.Cancel();
            model.Grid.Dispose();
            StocksGrid.CloseRuntime();
            _lifetime.Dispose();
        };
    }
}
