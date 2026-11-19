using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Composition;
using PhialeTech.Grid.Samples.Avalonia.SqliteBinding.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Views;
using PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Infrastructure;
namespace PhialeTech.Grid.Samples.Avalonia.SqliteBinding;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override Task<Window> CreateWindowAsync()
    {
        var source = new StockGridComposition();
        Composition = source;
        var repository = new SqliteStockRepository(Path.Combine(AppContext.BaseDirectory, "Data", "warehouse-1000000.sqlite"));
        var api = GridApiFactory.CreateRuntime();
        return Task.FromResult<Window>(new MainWindow(api, new StocksViewModel(repository, source.CreateBinding, api)));
    }
}
