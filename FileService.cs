public static class FileService
{
    private const long MaxFileSize = 1024 * 100; // 100 KB
    public static string GetFileContent(string path)
    {
    if (!File.Exists(path))
        throw new FileNotFoundException("File not found.", path);
        FileInfo info = new FileInfo(path);
        if (info.Length > MaxFileSize)
            throw new Exception("File size exceeds the maximum limit of 100 KB.");  
        
        return File.ReadAllText(path);
    }
}