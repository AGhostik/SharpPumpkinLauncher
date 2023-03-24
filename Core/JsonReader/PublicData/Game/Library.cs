namespace JsonReader.PublicData.Game;

public sealed class Library
{
    public Library(LibraryFile? file, LibraryFile? nativesWindowsFile, LibraryFile? nativesLinuxFile,
        LibraryFile? nativesOsxFile, string? nativesWindows, string? nativesLinux, string? nativesOsx, Rule[] rules)
    {
        File = file;
        NativesWindowsFile = nativesWindowsFile;
        NativesLinuxFile = nativesLinuxFile;
        NativesOsxFile = nativesOsxFile;
        NativesWindows = nativesWindows;
        NativesLinux = nativesLinux;
        NativesOsx = nativesOsx;
        Rules = rules;
    }

    public LibraryFile? File { get; }
    public LibraryFile? NativesWindowsFile { get; }
    public LibraryFile? NativesLinuxFile { get; }
    public LibraryFile? NativesOsxFile { get; }
    public string? NativesWindows { get; }
    public string? NativesLinux { get; }
    public string? NativesOsx { get; }
    public Rule[] Rules { get; }
}