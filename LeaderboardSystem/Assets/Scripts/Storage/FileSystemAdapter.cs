using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public abstract class FileSystemAdapter<T> : IStorageAdapter<T>
{
    private readonly ConcurrentDictionary<int, T> _cachedData = new();
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _fileLocks = new();

    protected abstract Task<T> ReadFromFileAsync(string filePath);
    protected abstract Task WriteToFileAsync(string filePath, T data);

    public async Task<bool> Exists(string filePath)
    {
        var pathHash = filePath.GetHashCode();
        if (_cachedData.ContainsKey(pathHash))
            return await Task.FromResult(true);

        return await Task.FromResult(File.Exists(filePath));
    }

    public async Task SaveAsync(string filePath, T data)
    {
        var pathHash = filePath.GetHashCode();
        _cachedData.AddOrUpdate(pathHash, data, (hash, oldData) => data);

        var semaphore = _fileLocks.GetOrAdd(pathHash, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();

        try
        {
            await WriteToFileAsync(filePath, data);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<T> LoadAsync(string filePath)
    {
        var pathHash = filePath.GetHashCode();
        if (_cachedData.TryGetValue(pathHash, out var cachedData))
            return await Task.FromResult(cachedData);

        var data = await ReadFromFileAsync(filePath);
        _cachedData.TryAdd(pathHash, data);
        return data;
    }

    public async Task Delete(string filePath)
    {
        var pathHash = filePath.GetHashCode();
        _cachedData.TryRemove(pathHash, out _);

        if (_fileLocks.TryRemove(pathHash, out var semaphore))
        {
            await semaphore.WaitAsync();
            try
            {
                File.Delete(filePath);
            }
            finally
            {
                semaphore.Release();
            }
        }
        else
        {
            File.Delete(filePath);
        }
    }
}