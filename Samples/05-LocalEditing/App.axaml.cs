using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.Composition;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.Views;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.Infrastructure;
namespace PhialeTech.Grid.Samples.Avalonia.LocalEditing;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override async Task<Window> CreateWindowAsync()
    {
        var source = await AssetGridComposition.CreateAsync(DemoDatabase.OpenWorkingCopy());
        Composition = source;
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<Models.AssetRow>(api, source.Binding);
        context.SetValidators(new Configuration.AssetBusinessRules(), new AssetBudgetValidator(new AssetRepository(source.DatabasePath)));
        return new MainWindow(api, new AssetsViewModel(context, source.DatabasePath));
    }
}
