using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Repositories.Models;
using Repositories;
using System.Diagnostics;

[TestFixture]
public class AvatarRepositoryTests
{
    private IAvatarRepository _repository;
    private readonly PlayerAvatar _testAvatar = new(1, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 });

    [SetUp]
    public void Setup()
    {
        _repository = new AvatarRepository("TestPath",new TestStorageAdapter());
    }

    [Test]
    [Timeout(2000)]
    public async Task AddAsync_ShouldStoreAvatar()
    {
        // Act
        await _repository.AddAsync(_testAvatar);

        // Assert
        var retrieved = await _repository.GetByIdAsync(_testAvatar.PlayerId);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(_testAvatar.PlayerId, retrieved.PlayerId);
        Assert.AreEqual(_testAvatar.ProfileImage, retrieved.ProfileImage);
    }

    [Test]
    [Timeout(2000)]
    public async Task GetByIdAsync_ShouldReturnNullForNonExistentPlayer()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    [Timeout(2000)]
    public async Task UpdateAsync_ShouldModifyExistingAvatar()
    {
        // Arrange
        await _repository.AddAsync(_testAvatar);
        var updatedAvatar = new PlayerAvatar(_testAvatar.PlayerId, new byte[] { 0x02, 0x02, 0x03, 0x05, 0x05 });

        // Act
        await _repository.UpdateAsync(updatedAvatar);

        // Assert
        var retrieved = await _repository.GetByIdAsync(_testAvatar.PlayerId);
        Assert.AreEqual(updatedAvatar.ProfileImage, retrieved.ProfileImage);
    }

    [Test]
    [Timeout(2000)]
    public void UpdateAsync_ShouldThrowForNonExistentPlayer()
    {
        // Arrange
        var nonExistentAvatar = new PlayerAvatar { PlayerId = 999 };

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(nonExistentAvatar));
    }

    [Test]
    [Timeout(2000)]
    public async Task DeleteAsync_ShouldRemoveAvatar()
    {
        // Arrange
        await _repository.AddAsync(_testAvatar);

        // Act
        await _repository.DeleteAsync(_testAvatar.PlayerId);

        // Assert
        var retrieved = await _repository.GetByIdAsync(_testAvatar.PlayerId);
        Assert.IsNull(retrieved);
    }

    [Test]
    [Timeout(2000)]
    public void DeleteAsync_ShouldThrowForNonExistentPlayer()
    {
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.DeleteAsync(999));
    }

    private class TestStorageAdapter : IStorageAdapter<byte[]>
    {
        public Dictionary<string, byte[]> Storage = new();

        public bool Exists(string path) => Storage.ContainsKey(path);

        public Task SaveAsync(string path, byte[] data)
        {
            if (Storage.ContainsKey(path)) Storage[path] = data;
            else Storage.Add(path, data);
            UnityEngine.Debug.Log($"{path} Stored.");
            return Task.CompletedTask;
        }

        public Task<byte[]> LoadAsync(string path)
        {
            UnityEngine.Debug.Log($"Requested {path}.");
            if (Storage.ContainsKey(path))
                return Task.FromResult(Storage[path]);
            return null;
        }

        public void Delete(string path)
        {
            UnityEngine.Debug.Log($"Remove {path}.");
            if (Storage.ContainsKey(path))
                Storage.Remove(path);
        }
    }
}