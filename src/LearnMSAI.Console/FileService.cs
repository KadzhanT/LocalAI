using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace LearnMSAI.Console;
public static class FileService
{
    private const long MaxFileSize = 1024 * 500; // 500 KB
    public static string GetFileContent(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        FileInfo info = new FileInfo(path);
        if (info.Length > MaxFileSize)
            throw new Exception("File size exceeds the maximum limit of 500 KB.");

        string extension = Path.GetExtension(path).ToLower();

        if (extension == ".pdf")
        {
            return ReadPdf(path);
        }

        return File.ReadAllText(path);
    }

    private static string ReadPdf(string path)
    {
        StringBuilder text = new StringBuilder();
        try
        {
            using (PdfDocument document = PdfDocument.Open(path))
            {
                foreach (Page page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при чтении PDF: {ex.Message}");
        }
        return text.ToString();
    }


}