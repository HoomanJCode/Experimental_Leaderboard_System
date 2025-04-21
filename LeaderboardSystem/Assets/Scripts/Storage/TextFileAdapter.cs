using System.IO;
using System.Threading.Tasks;
// Text file implementation (for JSON/XML)
public class TextFileAdapter : IStorageAdapter<string>
{
    public Task<bool> Exists(string filePath) => Task.Run(() => File.Exists(filePath));
    public async Task SaveAsync(string filePath, string textData) => await File.WriteAllTextAsync(filePath, textData);
    public async Task<string> LoadAsync(string filePath) => await File.ReadAllTextAsync(filePath);
    public Task Delete(string filePath) => Task.Run(() => File.Delete(filePath));
}