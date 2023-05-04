namespace Launcher.PublicData;

public sealed class Versions
{
    private readonly Dictionary<string, Version> _versions;

    public static Versions Empty => new();

    private Versions()
    {
        Latest = null;
        LatestSnapshot = null;
        Release = Array.Empty<Version>();
        Snapshot = Array.Empty<Version>();
        Beta = Array.Empty<Version>();
        Alpha = Array.Empty<Version>();
        _versions = new Dictionary<string, Version>();
    }

    public Versions(string? latestId, string? latestSnapshotId, IReadOnlyList<Version> release,
        IReadOnlyList<Version> snapshot, IReadOnlyList<Version> beta, IReadOnlyList<Version> alpha)
    {
        Release = release;
        Snapshot = snapshot;
        Beta = beta;
        Alpha = alpha;

        _versions = new Dictionary<string, Version>();
        AddVersionToDictionary(Alpha);
        AddVersionToDictionary(Beta);
        AddVersionToDictionary(Snapshot);
        AddVersionToDictionary(Release);

        Latest = Release.FirstOrDefault(version => version.Id == latestId);
        LatestSnapshot = Snapshot.FirstOrDefault(version => version.Id == latestSnapshotId);
    }

    public Version? Latest { get; }
    
    public Version? LatestSnapshot { get; }

    public IReadOnlyList<Version> Release { get; }

    public IReadOnlyList<Version> Snapshot { get; }

    public IReadOnlyList<Version> Beta { get; }

    public IReadOnlyList<Version> Alpha { get; }
    
    public IReadOnlyDictionary<string, Version> AllVersions => _versions;

    public bool IsEmpty => Release.Count == 0 && Snapshot.Count == 0 && Beta.Count == 0 && Alpha.Count == 0;

    public static Versions Merge(Versions a, Versions b)
    {
        var latest = a.Latest ?? b.Latest;
        var latestSnapshot = a.LatestSnapshot ?? b.LatestSnapshot;
        
        var release = MergeTwoList(a.Release, b.Release);
        var snapshot = MergeTwoList(a.Snapshot, b.Snapshot);
        var beta = MergeTwoList(a.Beta, b.Beta);
        var alpha = MergeTwoList(a.Alpha, b.Alpha);

        return new Versions(latest?.Id, latestSnapshot?.Id, release, snapshot, beta, alpha);
        
        IReadOnlyList<Version> MergeTwoList(IReadOnlyList<Version> listA, IReadOnlyList<Version> listB)
        {
            var result = new List<Version>();
            for (var i = 0; i < listA.Count; i++)
            {
                var element = listA[i];
                for (var j = 0; j < listB.Count; j++)
                {
                    if (listA[i].Id != listB[j].Id)
                        continue;
                    
                    var mergeResult = Version.Merge(listA[i], listB[j]);
                    if (mergeResult == null)
                        continue;

                    element = mergeResult;
                    break;
                }
                result.Add(element);
            }

            return result.ToArray();
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Versions versions)
            return Equals(versions);
        return false;
    }

    private bool Equals(Versions other)
    {
        return Release.Equals(other.Release) &&
               Snapshot.Equals(other.Snapshot) &&
               Beta.Equals(other.Beta) &&
               Alpha.Equals(other.Alpha) &&
               _versions.Equals(other._versions) &&
               Equals(Latest, other.Latest) &&
               Equals(LatestSnapshot, other.LatestSnapshot);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Release, Snapshot, Beta, Alpha, _versions);
    }
    
    private void AddVersionToDictionary(IReadOnlyList<Version> versions)
    {
        for (var i = 0; i < versions.Count; i++)
            _versions.Add(versions[i].Id, versions[i]);
    }
}