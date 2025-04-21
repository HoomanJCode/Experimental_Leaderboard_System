using System.IO;
using System.Threading.Tasks;
// Text file implementation (for JSON/XML)
public class TextFileAdapter : IStorageAdapter<string>
{
    public async Task<bool> Exists(string filePath) => await Task.FromResult(File.Exists(filePath));
    public async Task SaveAsync(string filePath, string textData) => await File.WriteAllTextAsync(filePath, textData);
    public async Task<string> LoadAsync(string filePath) => await File.ReadAllTextAsync(filePath);
    public async Task Delete(string filePath) => await Task.Run(() => File.Delete(filePath));
}