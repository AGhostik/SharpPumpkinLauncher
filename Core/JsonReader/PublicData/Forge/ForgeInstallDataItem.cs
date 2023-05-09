namespace JsonReader.PublicData.Forge;

public sealed class ForgeInstallDataItem
{
    public ForgeInstallDataItem(string client, string server)
    {
        Client = client;
        Server = server;
    }

    public string Client { get; }
    public string Server { get; }
}