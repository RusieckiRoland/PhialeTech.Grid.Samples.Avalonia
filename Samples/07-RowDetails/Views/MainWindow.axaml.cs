using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Details;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.RowDetails.Views;

public partial class MainWindow : Window
{
    public MainWindow(AssetsViewModel model)
    {
        AvaloniaXamlLoader.Load(this);
        AssetsGrid = this.FindControl<GridControl>("AssetsGrid")!;
        AssetsGrid.BindRuntime(model.Grid.Runtime);
        AssetsGrid.ConfigureColumns(model.Columns);
        AssetsGrid.Api.Workspace.SetCapabilityPolicy(new HttpRest.Configuration.RestExamplePolicy());
        AssetsGrid.Api.Projection.IsPagingEnabled = false;
        AssetsGrid.Api.Projection.EnableDataVirtualization = false;
        AssetsGrid.SetDetailProvider(new AssetDetailsProvider());
        AssetsGrid.RowDetailContentFactory = AssetDetailsFactory.Create;
        DataContext = model;
        Opened += async (_, _) => await AssetsGrid.LoadDataAsync(0, 100);
        Closed += (_, _) =>
        {
            model.Dispose();
            AssetsGrid.CloseRuntime();
        };
    }
}
