namespace Launcher.PublicData;

public sealed class Versions
{
    private readonly List<Version> _release;
    private readonly List<Version> _snapshot;
    private readonly List<Version> _beta;
    private readonly List<Version> _alpha;
    private readonly Dictionary<string, Version> _versions;

    public static Versions Empty => new();

    private Versions()
    {
        Latest = null;
        LatestSnapshot = null;
        _release = new List<Version>();
        _snapshot = new List<Version>();
        _beta = new List<Version>();
        _alpha = new List<Version>();
        _versions = new Dictionary<string, Version>();
    }

    public Versions(string? latestId, string? latestSnapshotId, 
        List<Version> release, List<Version> snapshot, List<Version> beta, List<Version> alpha)
    {
        _release = release;
        _snapshot = snapshot;
        _beta = beta;
        _alpha = alpha;

        _versions = new Dictionary<string, Version>();
        AddVersionToDictionary(_alpha);
        AddVersionToDictionary(_beta);
        AddVersionToDictionary(_snapshot);
        AddVersionToDictionary(_release);

        Latest = _release.Find(version => version.Id == latestId);
        LatestSnapshot = _snapshot.Find(version => version.Id == latestSnapshotId);

        void AddVersionToDictionary(IReadOnlyList<Version> versions)
        {
            for (var i = 0; i < versions.Count; i++)
                _versions.Add(versions[i].Id, versions[i]);
        }
    }
    
    public Version? Latest { get; }
    
    public Version? LatestSnapshot { get; }

    public IReadOnlyList<Version> Release => _release;

    public IReadOnlyList<Version> Snapshot => _snapshot;

    public IReadOnlyList<Version> Beta => _beta;

    public IReadOnlyList<Version> Alpha => _alpha;

    public IReadOnlyDictionary<string, Version> AllVersions => _versions;

    public override bool Equals(object? obj)
    {
        if (obj is Versions versions)
            return Equals(versions);
        return false;
    }

    private bool Equals(Versions other)
    {
        return _release.Equals(other._release) && _snapshot.Equals(other._snapshot) && _beta.Equals(other._beta) &&
               _alpha.Equals(other._alpha) && _versions.Equals(other._versions) && Equals(Latest, other.Latest) &&
               Equals(LatestSnapshot, other.LatestSnapshot);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_release, _snapshot, _beta, _alpha, _versions, Latest, LatestSnapshot);
    }
}