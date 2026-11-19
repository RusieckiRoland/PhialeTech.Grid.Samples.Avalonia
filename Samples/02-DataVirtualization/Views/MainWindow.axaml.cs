using Avalonia.Threading;
using PhialeGrid.Core.Interaction;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Contracts;
using PhialeGrid.Core.Regions;
using PhialeGrid.Core.Query;
using PhialeGrid.Core.Summaries;
using PhialeTech.Grid.Samples.Avalonia.DataVirtualization.ViewModels;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Views;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _lifetime = new();
    private bool _closeRequested;
    public MainWindow(IGridApi api, StocksViewModel model)
    {
        AvaloniaXamlLoader.Load(this);
        StockGrid = this.FindControl<GridControl>("StockGrid")!;
        // Avalonia uses the same neutral projection API instead of WPF dependency properties.
        StockGrid.BindRuntime(api);
        StockGrid.ConfigureColumns(model.Columns);
        api.Projection.IsPagingEnabled = false;
        api.Projection.PageSize = 100;
        api.Projection.EnableDataVirtualization = true;
        api.Configuration.FilterRowHeight = 32;

        api.Workspace.Execute(new(GridRegionCommandKind.Close, GridRegionKind.TopCommandRegion));
        api.Workspace.Execute(new(GridRegionCommandKind.Close, GridRegionKind.SummaryBottomRegion));
        api.Workspace.Execute(new(GridRegionCommandKind.Close, GridRegionKind.GroupingRegion));

        api.Workspace.SetCapabilityPolicy(model.Capabilities);
        DataContext = model;
        Opened += async (_, _) =>
        {
            try
            {
                await model.LoadAsync(_lifetime.Token);
            }
            catch (OperationCanceledException) { }
            catch (PhialeGrid.Core.Data.GridDataReadException)
            {
                // Core exposes the read error and retry state.
            }
        };
        // A viewport read may still be completing when the user closes the window.
        // Keep the view alive until Core releases its operation scope; never block the UI thread.
        Closing += (_, args) =>
        {
            if (StockGrid.CanCloseRuntime)
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
                while (!StockGrid.CanCloseRuntime)
                    await Task.Delay(25);
                Close();
            });
        };
        Closed += (_, _) =>
        {
            _lifetime.Cancel();
            model.Dispose();
            StockGrid.CloseRuntime();
            _lifetime.Dispose();
        };
    }
}
