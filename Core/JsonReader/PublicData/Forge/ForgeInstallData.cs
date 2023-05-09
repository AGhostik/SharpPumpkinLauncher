namespace JsonReader.PublicData.Forge;

public sealed class ForgeInstallData
{
    public ForgeInstallData(ForgeInstallDataItem mappings, ForgeInstallDataItem mojmaps,
        ForgeInstallDataItem mergedMappings, ForgeInstallDataItem binpatch, ForgeInstallDataItem mcUnpacked,
        ForgeInstallDataItem mcSlim, ForgeInstallDataItem mcSlimSha, ForgeInstallDataItem mcExtra,
        ForgeInstallDataItem mcExtraSha, ForgeInstallDataItem mcSrg, ForgeInstallDataItem patched,
        ForgeInstallDataItem patchedSha, ForgeInstallDataItem mpcVersion, ForgeInstallDataItem side)
    {
        Mappings = mappings;
        Mojmaps = mojmaps;
        MergedMappings = mergedMappings;
        Binpatch = binpatch;
        McUnpacked = mcUnpacked;
        McSlim = mcSlim;
        McSlimSha = mcSlimSha;
        McExtra = mcExtra;
        McExtraSha = mcExtraSha;
        McSrg = mcSrg;
        Patched = patched;
        PatchedSha = patchedSha;
        MpcVersion = mpcVersion;
        Side = side;
    }

    public ForgeInstallDataItem Mappings { get; }
    
    public ForgeInstallDataItem Mojmaps { get; }
    
    public ForgeInstallDataItem MergedMappings { get; }
    
    public ForgeInstallDataItem Binpatch { get; }
    
    public ForgeInstallDataItem McUnpacked { get; }
    
    public ForgeInstallDataItem McSlim { get; }
    
    public ForgeInstallDataItem McSlimSha { get; }
    
    public ForgeInstallDataItem McExtra { get; }
    
    public ForgeInstallDataItem McExtraSha { get; }
    
    public ForgeInstallDataItem McSrg { get; }
    
    public ForgeInstallDataItem Patched { get; }
    
    public ForgeInstallDataItem PatchedSha { get; }
    
    public ForgeInstallDataItem MpcVersion { get; }
    
    public ForgeInstallDataItem Side { get; }
}