using System.Xml;
using System.Xml.Serialization;

namespace UserSettings;

public class LauncherSettings
{
    private const string FileName = "userSettings.xml";

    public static LauncherSettings Instance { get; } = new();

    public SettingsData Data { get; set; } = new();
    
    public static void Save()
    {
        var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
        var xmlSerializer = new XmlSerializer(typeof(SettingsData));
        using var xmlWriter = XmlWriter.Create(FileName, xmlWriterSettings);
        xmlSerializer.Serialize(xmlWriter, Instance.Data);
    }

    public static bool Load()
    {
        if (!File.Exists(FileName))
            return false;
        
        using var stream = new FileStream(FileName, FileMode.OpenOrCreate);
        var xmlSerializer = new XmlSerializer(typeof(SettingsData));

        try
        {
            var data = xmlSerializer.Deserialize(stream);
            if (data is SettingsData userSettingsData)
                Instance.Data = userSettingsData;

            return true;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}