namespace Lattice.Shared.Typography;

public sealed class TypographyInitializer(
    ITypographyService typography)
{
    public async Task InitializeAsync()
    {
        await typography.LoadAsync("ledger");
    }
}