namespace Lattice.Shared.Services;

public interface IApplicationIcons
{
    string GetIcon(string name, int? width = null, int? height = null);

    bool Exists(string name);

    IReadOnlyCollection<string> GetIconNames();
}