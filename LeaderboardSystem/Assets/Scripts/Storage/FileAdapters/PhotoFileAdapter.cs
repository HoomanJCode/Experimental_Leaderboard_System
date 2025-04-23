using System.IO;
using System.Threading.Tasks;
public class PhotoFileAdapter : FileSystemAdapter<byte[]>
{
    protected override Task<byte[]> ReadFromFileAsync(string filePath)
    {
        return File.ReadAllBytesAsync(filePath);
    }

    protected override Task WriteToFileAsync(string filePath, byte[] data)
    {
        return File.WriteAllBytesAsync(filePath, data);
    }
}
