using System.Diagnostics;
using System.Text;

namespace SimpleLogger;

public static class Logger
{
    private static readonly string _fileName;
    private static object _locker = new();

    static Logger()
    {
        _fileName = $"launcher_logs_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.txt";
    }
    
    public static void Log(object? value)
    {
        Log(value as string);
    }
    
    public static void Log(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        
        Debug.WriteLine(text);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("[Log] [");
        stringBuilder.Append(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        stringBuilder.Append("] ");
        stringBuilder.AppendLine(text);

        lock (_locker)
        {
            File.AppendAllText(_fileName, stringBuilder.ToString());
        }
    }

    public static void Log(Exception exception)
    {
        Debug.WriteLine(exception);
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("[Exception] [");
        stringBuilder.Append(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        stringBuilder.AppendLine("] ");
        
        stringBuilder.AppendLine("\t= Message =");
        stringBuilder.Append('\t');
        stringBuilder.AppendLine(exception.Message);
        
        stringBuilder.AppendLine("\t= StackTrace =");
        stringBuilder.Append('\t');
        stringBuilder.AppendLine(exception.StackTrace);
        
        stringBuilder.AppendLine("\t= Source =");
        stringBuilder.Append('\t');
        stringBuilder.AppendLine(exception.Source);

        if (exception.TargetSite != null)
        {
            stringBuilder.AppendLine("\t= TargetSite =");
            stringBuilder.Append('\t');
            stringBuilder.AppendLine(exception.TargetSite.ToString());
        }

        stringBuilder.AppendLine("\t= end of exception log =");

        lock (_locker)
        {
            File.AppendAllText(_fileName, stringBuilder.ToString());
        }
    }
}