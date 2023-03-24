namespace JsonReader.PublicData.Game;

public sealed class Library
{
    public Library(LibraryFile? file, LibraryFile? nativesWindowsFile, LibraryFile? nativesLinuxFile,
        LibraryFile? nativesOsxFile, string? nativesWindows, string? nativesLinux, string? nativesOsx,
        IReadOnlyList<Rule> rules, IReadOnlyList<string> delete)
    {
        File = file;
        NativesWindowsFile = nativesWindowsFile;
        NativesLinuxFile = nativesLinuxFile;
        NativesOsxFile = nativesOsxFile;
        NativesWindows = nativesWindows;
        NativesLinux = nativesLinux;
        NativesOsx = nativesOsx;
        Rules = rules;
        Delete = delete;
    }

    public LibraryFile? File { get; }
    public LibraryFile? NativesWindowsFile { get; }
    public LibraryFile? NativesLinuxFile { get; }
    public LibraryFile? NativesOsxFile { get; }
    public string? NativesWindows { get; }
    public string? NativesLinux { get; }
    public string? NativesOsx { get; }
    public IReadOnlyList<Rule> Rules { get; }
    public IReadOnlyList<string> Delete { get; }
}