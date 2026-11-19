using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Composition;
using PhialeTech.Grid.Samples.Avalonia.DataVirtualization.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Views;
using PhialeTech.Grid.Samples.Avalonia.DataVirtualization.Infrastructure;
namespace PhialeTech.Grid.Samples.Avalonia.DataVirtualization;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override Task<Window> CreateWindowAsync()
    {
        var repository = new StockRepository(Path.Combine(AppContext.BaseDirectory, "Data", "warehouse-1000000.sqlite"));
        var source = new StockGridComposition(repository);
        Composition = source;
        var api = GridApiFactory.CreateRuntime();
        return Task.FromResult<Window>(new MainWindow(api, new StocksViewModel(source.CreateBinding, api)));
    }
}
