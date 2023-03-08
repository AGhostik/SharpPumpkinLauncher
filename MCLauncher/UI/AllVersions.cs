using System.Collections.Generic;

namespace MCLauncher.UI;

public class AllVersions
{
    public string? Latest { get; set; }
    public string? LatestSnapshot { get; set; }
    public List<string> Custom { get; init; } = new();
    public List<string> Release { get; init; } = new();
    public List<string> Snapshot { get; init; } = new();
    public List<string> Beta { get; init; } = new();
    public List<string> Alpha { get; init; } = new();
}