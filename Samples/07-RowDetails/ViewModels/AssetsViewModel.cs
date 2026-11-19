using PhialeGrid.Contracts;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.Models;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Configuration;
namespace PhialeTech.Grid.Samples.Avalonia.RowDetails.ViewModels;

public sealed class AssetsViewModel(IGridContext<AssetRow> grid) : IDisposable
{
    public IGridContext<AssetRow> Grid { get; } = grid;
    public IReadOnlyList<IGridColumnDescriptor> Columns => AssetColumns.All;
    public void Dispose() => Grid.Dispose();
}
