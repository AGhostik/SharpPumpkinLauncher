namespace MCLauncher.Messages;

public class StatusMessage
{
    public StatusMessage(string? status)
    {
        Status = status;
    }
    
    public string? Status { get; }
}