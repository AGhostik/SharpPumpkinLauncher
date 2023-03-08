namespace MCLauncher.Messages;

public class InstallProgressMessage
{
    public InstallProgressMessage(float percentage)
    {
        Percentage = percentage;
    }
        
    public float Percentage { get; }
}