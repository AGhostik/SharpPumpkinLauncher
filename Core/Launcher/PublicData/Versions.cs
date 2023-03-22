namespace Launcher.PublicData;

public sealed class Versions
{
    private readonly List<Version> _release;
    private readonly List<Version> _snapshot;
    private readonly List<Version> _beta;
    private readonly List<Version> _alpha;

    public Versions(string? latestId, string? latestSnapshotId, 
        List<Version> release, List<Version> snapshot, List<Version> beta, List<Version> alpha)
    {
        _release = release;
        _snapshot = snapshot;
        _beta = beta;
        _alpha = alpha;

        Latest = _release.Find(version => version.Id == latestId);
        LatestSnapshot = _snapshot.Find(version => version.Id == latestSnapshotId);
    }
    
    public Version? Latest { get; }
    
    public Version? LatestSnapshot { get; }

    public IReadOnlyList<Version> Release => _release;

    public IReadOnlyList<Version> Snapshot => _snapshot;

    public IReadOnlyList<Version> Beta => _beta;

    public IReadOnlyList<Version> Alpha => _alpha;
}