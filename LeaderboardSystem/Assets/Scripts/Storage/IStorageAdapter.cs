using System.Threading.Tasks;

// Generic interface for storage operations
public interface IStorageAdapter<T>
{
    Task<bool> Exists(string filePath);                 // Check if file exists
    Task SaveAsync(string filePath, T data);      // Save data to path
    Task<T> LoadAsync(string filePath);           // Load data from path
    Task Delete(string filePath);                 // Delete file at path
}
