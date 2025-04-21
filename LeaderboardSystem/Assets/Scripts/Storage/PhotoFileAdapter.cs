using System.IO;
using System.Threading.Tasks;
// Binary file implementation (for images)
public class PhotoFileAdapter : IStorageAdapter<byte[]>
{
    public Task<bool> Exists(string filePath) => Task.Run(() => File.Exists(filePath));
    public async Task SaveAsync(string filePath, byte[] imageData) => await File.WriteAllBytesAsync(filePath, imageData);
    public async Task<byte[]> LoadAsync(string filePath) => await File.ReadAllBytesAsync(filePath);
    public Task Delete(string filePath) => Task.Run(() => File.Delete(filePath));
}
