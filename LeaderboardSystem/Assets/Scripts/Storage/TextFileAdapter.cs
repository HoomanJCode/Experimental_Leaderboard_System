using System.IO;
using System.Threading.Tasks;
// Text file implementation (for JSON/XML)
public class TextFileAdapter : IStorageAdapter<string>
{
    public bool Exists(string filePath) => File.Exists(filePath);
    public async Task SaveAsync(string filePath, string textData) => await File.WriteAllTextAsync(filePath, textData);
    public async Task<string> LoadAsync(string filePath) => await File.ReadAllTextAsync(filePath);
    public void Delete(string filePath) => File.Delete(filePath);
}