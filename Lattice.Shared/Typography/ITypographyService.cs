namespace Lattice.Shared.Typography;

public interface ITypographyService
{
    TypographyDefinition Current { get; }

    Task LoadAsync(string preset);

    TextStyleDefinition GetStyle(string name);

    FontDefinition GetFont(string family);
}