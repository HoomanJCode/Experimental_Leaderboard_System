using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Repositories;
using UnityEngine;

[TestFixture]
public class AvatarRepositoryTests
{
    private IAvatarRepository _repository;
    private TestStorageAdapter _testStorage;
    private Sprite _testSprite;
    private Texture2D _testTexture;

    [SetUp]
    public void Setup()
    {
        _testStorage = new TestStorageAdapter();
        _repository = new AvatarRepository("TestPath", _testStorage);

        // Create a test sprite
        _testTexture = new Texture2D(2, 2);
        _testTexture.SetPixels(new Color[] { Color.red, Color.green, Color.blue, Color.white });
        _testTexture.Apply();
        _testSprite = Sprite.Create(_testTexture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
    }

    [TearDown]
    public void Teardown()
    {
        if (_testTexture != null)
            UnityEngine.Object.DestroyImmediate(_testTexture);
    }

    [Test]
    public async Task AddOrUpdateAsync_ShouldStoreAvatar()
    {
        // Act
        await _repository.AddOrUpdateAsync(1, _testSprite);

        // Assert
        var retrieved = await _repository.GetByIdAsync(1);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(_testSprite.texture.width, retrieved.texture.width);
        Assert.AreEqual(_testSprite.texture.height, retrieved.texture.height);
    }

    [Test]
    public async Task AddOrUpdateAsync_ShouldUpdateCache()
    {
        // Arrange
        await _repository.AddOrUpdateAsync(1, _testSprite);

        // Act
        var newSprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
        await _repository.AddOrUpdateAsync(1, newSprite);

        // Assert
        var cachedSprite = await _repository.GetByIdAsync(1);
        Assert.AreSame(newSprite.texture, cachedSprite.texture);

        // Cleanup
        UnityEngine.Object.DestroyImmediate(newSprite.texture);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCachedAvatar()
    {
        // Arrange
        await _repository.AddOrUpdateAsync(1, _testSprite);
        _testStorage.Storage.Clear(); // Clear storage to prove we're using cache

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(_testSprite.texture.width, result.texture.width);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNullForNonExistentPlayer()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public async Task GetByIdAsync_ShouldLoadFromStorageWhenNotCached()
    {
        // Arrange
        var bytes = SpriteUtilities.SpriteToByte(_testSprite);
        // Store using the same path format the repository uses
        var expectedPath = Path.Combine("TestPath", "1");
        await _testStorage.SaveAsync(expectedPath, bytes);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result, "Should load sprite from storage when not in cache");
        Assert.AreEqual(_testSprite.texture.width, result.texture.width, "Loaded sprite should match original dimensions");
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveAvatarFromCacheAndStorage()
    {
        // Arrange
        await _repository.AddOrUpdateAsync(1, _testSprite);

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        Assert.IsFalse(_testStorage.Storage.ContainsKey("TestPath/1"));
        Assert.IsNull(await _repository.GetByIdAsync(1));
    }

    [Test]
    public void DeleteAsync_ShouldThrowForNonExistentPlayer()
    {
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _repository.DeleteAsync(999));
    }

    [Test]
    public async Task HasAvatarAsync_ShouldReturnTrueForCachedAvatar()
    {
        // Arrange
        await _repository.AddOrUpdateAsync(1, _testSprite);
        _testStorage.Storage.Clear(); // Clear storage to prove we're using cache

        // Act
        var result = await _repository.HasAvatarAsync(1);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task HasAvatarAsync_ShouldCheckStorageWhenNotCached()
    {
        // Arrange
        var bytes = SpriteUtilities.SpriteToByte(_testSprite);
        // Store using the same path format the repository uses
        var expectedPath = Path.Combine("TestPath", "1");
        await _testStorage.SaveAsync(expectedPath, bytes);

        // Act
        var result = await _repository.HasAvatarAsync(1);

        // Assert
        Assert.IsTrue(result, "Should find avatar in storage when not in cache");
    }

    [Test]
    public async Task HasAvatarAsync_ShouldReturnFalseForNonExistentPlayer()
    {
        // Act
        var result = await _repository.HasAvatarAsync(999);

        // Assert
        Assert.IsFalse(result);
    }

    private class TestStorageAdapter : IStorageAdapter<byte[]>
    {
        public Dictionary<string, byte[]> Storage = new();

        public Task<bool> Exists(string filePath)
        {
            return Task.FromResult(Storage.ContainsKey(filePath));
        }

        public Task SaveAsync(string path, byte[] data)
        {
            Storage[path] = data;
            return Task.CompletedTask;
        }

        public Task<byte[]> LoadAsync(string path)
        {
            if (Storage.TryGetValue(path, out var data))
            {
                return Task.FromResult(data);
            }
            return Task.FromResult<byte[]>(null);
        }

        public Task Delete(string path)
        {
            Storage.Remove(path);
            return Task.CompletedTask;
        }
    }
}