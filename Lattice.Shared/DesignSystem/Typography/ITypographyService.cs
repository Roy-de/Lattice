namespace Lattice.Shared.DesignSystem.Typography;

public interface ITypographyService
{
    TypographyDefinition Current { get; }
    
    IReadOnlyList<TypographyDefinition> Definitions { get; }

    Task LoadAsync(string preset);

    TextStyleDefinition GetStyle(string name);

    FontDefinition GetFont(string family);
}