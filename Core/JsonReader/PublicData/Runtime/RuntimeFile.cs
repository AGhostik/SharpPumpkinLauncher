﻿namespace JsonReader.PublicData.Runtime;

public sealed class RuntimeFile
{
    public RuntimeFile(string path, string sha1, int size, string url)
    {
        Path = path;
        Sha1 = sha1;
        Size = size;
        Url = url;
    }

    public string Path { get; }
    
    public string Sha1 { get; }
    public int Size { get; }
    public string Url { get; }
}