namespace MCLauncher.UI.Messages
{
    public class DownloadProgressMessage
    {
        public DownloadProgressMessage(float percentage)
        {
            Percentage = percentage;
        }

        public float Percentage { get; set; }
    }
}