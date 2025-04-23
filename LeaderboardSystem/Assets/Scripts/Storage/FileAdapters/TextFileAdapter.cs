using System.IO;
using System.Threading.Tasks;
// Text file implementation (for JSON/XML)
public class TextFileAdapter : FileSystemAdapter<string>
{
    protected override Task<string> ReadFromFileAsync(string filePath)
    {
        return File.ReadAllTextAsync(filePath);
    }

    protected override Task WriteToFileAsync(string filePath, string data)
    {
        return File.WriteAllTextAsync(filePath, data);
    }
}