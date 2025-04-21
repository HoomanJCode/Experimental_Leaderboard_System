using System.IO;
using System.Threading.Tasks;
// Binary file implementation (for images)
public class PhotoFileAdapter : IStorageAdapter<byte[]>
{
    public async Task<bool> Exists(string filePath) => await Task.FromResult(File.Exists(filePath));
    public async Task SaveAsync(string filePath, byte[] imageData) => await File.WriteAllBytesAsync(filePath, imageData);
    public async Task<byte[]> LoadAsync(string filePath) => await File.ReadAllBytesAsync(filePath);
    public async Task Delete(string filePath) => await Task.Run(() => File.Delete(filePath));
}
