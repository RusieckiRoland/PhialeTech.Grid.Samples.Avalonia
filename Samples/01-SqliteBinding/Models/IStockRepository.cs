namespace PhialeTech.Grid.Samples.Avalonia.SqliteBinding.Models;

public interface IStockRepository
{
    Task<IReadOnlyList<StockRow>> ReadAllAsync(CancellationToken cancellationToken, int limit = 100000);
}
