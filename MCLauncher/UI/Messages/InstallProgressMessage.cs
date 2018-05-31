namespace MCLauncher.UI.Messages
{
    public class InstallProgressMessage
    {
        public InstallProgressMessage(float percentage)
        {
            Percentage = percentage;
        }
        
        public float Percentage { get; set; }
    }
}