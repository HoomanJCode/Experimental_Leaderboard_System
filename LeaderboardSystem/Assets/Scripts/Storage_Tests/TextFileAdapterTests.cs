using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TextFileAdapterTests
{

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator Exists_ReturnsFalseForNonExistentFile()
    {
        var adapter = new TextFileAdapter();
        var checkTask = adapter.Exists("nonexistent_file.dat");
        yield return new WaitUntil(() => checkTask.IsCompleted);
        Assert.IsFalse(checkTask.Result);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator SaveAsync_CreatesFile()
    {
        var adapter = new TextFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(TextFileAdapterTests) + "." + nameof(SaveAsync_CreatesFile) + ".dat");
        var testTextData = "This is a test string with some content.";

        yield return adapter.SaveAsync(testFilePath, testTextData);
        Assert.IsTrue(File.Exists(testFilePath));
        File.Delete(testFilePath);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator LoadAsync_ReturnsSavedData()
    {
        var adapter = new TextFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(TextFileAdapterTests) + "." + nameof(LoadAsync_ReturnsSavedData) + ".dat");
        var testTextData = "This is a test string with some content.";

        yield return adapter.SaveAsync(testFilePath, testTextData);
        var loadTask = adapter.LoadAsync(testFilePath);
        yield return new WaitUntil(() => loadTask.IsCompleted);

        Assert.AreEqual(testTextData, loadTask.Result);
        File.Delete(testFilePath);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator Delete_RemovesFile()
    {
        var adapter = new TextFileAdapter();
        var testFilePath = Path.Combine(Application.persistentDataPath, nameof(TextFileAdapterTests) + "." + nameof(Delete_RemovesFile) + ".dat");
        var testTextData = "This is a test string with some content.";

        File.WriteAllText(testFilePath, testTextData);
        yield return adapter.Delete(testFilePath);
        Assert.IsFalse(File.Exists(testFilePath));
        File.Delete(testFilePath);
    }

    [UnityTest]
    [Timeout(2000)]
    public IEnumerator LoadAsync_ThrowsForNonExistentFile()
    {
        var adapter = new TextFileAdapter();

        var loadTask = adapter.LoadAsync("nonexistent_file.txt");
        yield return new WaitUntil(() => loadTask.IsCompleted);

        Assert.IsTrue(loadTask.IsFaulted);
        Assert.IsNotNull(loadTask.Exception);
        Assert.IsInstanceOf<FileNotFoundException>(loadTask.Exception.InnerException);
    }
}