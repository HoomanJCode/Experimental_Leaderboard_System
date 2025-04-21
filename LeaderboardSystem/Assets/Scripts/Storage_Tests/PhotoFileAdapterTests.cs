using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

public class PhotoFileAdapterTests
{
    [UnityTest]
    [Timeout(2000)]
    public IEnumerator Exists_ReturnsFalseForNonExistentFile()
    {
        var adapter = new PhotoFileAdapter();
        var checkTask = adapter.Exists("nonexistent_file.dat");
        yield return new WaitUntil(() => checkTask.IsCompleted);
        Assert.IsFalse(checkTask.Result);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator SaveAsync_CreatesFile()
    {
        var adapter = new PhotoFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(PhotoFileAdapterTests) + "." + nameof(SaveAsync_CreatesFile)+ ".dat");
        var testImageData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        yield return adapter.SaveAsync(testFilePath, testImageData);
        Assert.IsTrue(File.Exists(testFilePath));
        File.Delete(testFilePath);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator LoadAsync_ReturnsSavedData()
    {
        var adapter = new PhotoFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(PhotoFileAdapterTests) + "." + nameof(LoadAsync_ReturnsSavedData)+ ".dat");
        var testImageData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        yield return adapter.SaveAsync(testFilePath, testImageData);
        var loadTask = adapter.LoadAsync(testFilePath);
        yield return new WaitUntil(() => loadTask.IsCompleted);
        //yield return loadTask;

        Assert.AreEqual(testImageData, loadTask.Result);
        File.Delete(testFilePath);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator Delete_RemovesFile()
    {
        var adapter = new PhotoFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(PhotoFileAdapterTests) + "." + nameof(Delete_RemovesFile)+ ".dat");
        var testImageData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        File.WriteAllBytes(testFilePath, testImageData);
        yield return adapter.Delete(testFilePath);
        Assert.IsFalse(File.Exists(testFilePath));
        File.Delete(testFilePath);
    }
    [UnityTest]
    [Timeout(2000)]
    public IEnumerator LoadAsync_ThrowsForNonExistentFile()
    {
        var adapter = new PhotoFileAdapter();
        var loadTask = adapter.LoadAsync("nonexistent_file.dat");
        yield return new WaitUntil(() => loadTask.IsCompleted);

        Assert.IsTrue(loadTask.IsFaulted);
        Assert.IsNotNull(loadTask.Exception);
        Assert.IsInstanceOf<FileNotFoundException>(loadTask.Exception.InnerException);
    }
}