using NUnit.Framework;
using Repositories;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class PlayerRepositoryTests
{

    [Test]
    [Timeout(2000)]
    public async Task AddPlayerAsync_ShouldReturnPlayerId()
    {
        var _repository = new PlayerRepository("TestPath",new TestStorageAdapter());
        var result = await _repository.AddPlayerAsync("New Player", "Desc");
        Assert.AreEqual(1, result); // First player should get id 1
    }

    [Test]
    [Timeout(2000)]
    public async Task AddPlayerAsync_ShouldIncrementIds()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        await _repository.AddPlayerAsync("New Player1", "Desc");
        var result = await _repository.AddPlayerAsync("New Player2", "Desc");
        Assert.AreEqual(2, result); // Second player should get id 2
    }

    [Test]
    [Timeout(2000)]
    public async Task GetByIdAsync_ShouldReturnNullForEmptyRepository()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        var result = await _repository.GetByIdAsync(1);
        Assert.IsNull(result);
    }

    [Test]
    [Timeout(2000)]
    public async Task GetByIdAsync_ShouldReturnCorrectPlayer()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        await _repository.AddPlayerAsync("New Player1", "Desc");
        var playerId = await _repository.AddPlayerAsync("New Player2", "Desc");
        await _repository.AddPlayerAsync("New Player3", "Desc");
        var result = await _repository.GetByIdAsync(playerId);
        Assert.IsNotNull(result);
        Assert.AreEqual("New Player2", result.Name);
        Assert.AreEqual("Desc", result.Description);
        Assert.AreEqual(playerId, result.Id);
    }

    [Test]
    [Timeout(2000)]
    public async Task UpdatePlayerAsync_ShouldModifyExistingPlayer()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        var playerId = await _repository.AddPlayerAsync("New Player1", "Desc");
        var updatedPlayer = new Player(playerId, "Updated Name","Desc2");

        // Act
        await _repository.UpdatePlayerAsync(new Player(playerId, updatedPlayer.Name, updatedPlayer.Description));
        var result = await _repository.GetByIdAsync(playerId);

        // Assert
        Assert.AreEqual(updatedPlayer.Name, result.Name);
        Assert.AreEqual(updatedPlayer.Description, result.Description);
    }

    [Test]
    [Timeout(2000)]
    public void UpdatePlayerAsync_ShouldThrowForNonExistentPlayer()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        // Arrange

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.UpdatePlayerAsync(new Player(99, "Non Existent", "Desc2")));
    }

    [Test]
    [Timeout(2000)]
    public async Task SaveChangesAsync_ShouldCompleteWithoutErrors()
    {
        var _repository = new PlayerRepository("TestPath", new TestStorageAdapter());
        // Act & Assert
        await _repository.SaveChangesAsync();
        Assert.Pass("Save completed without exceptions");
    }
    private class TestStorageAdapter : IStorageAdapter<string>
    {
        public Dictionary<string, string> Storage = new();

        public Task<bool> Exists(string filePath) => Task.FromResult(Storage.ContainsKey(filePath));

        public Task SaveAsync(string path, string data)
        {
            if (Storage.ContainsKey(path)) Storage[path] = data;
            else Storage.Add(path, data);
            UnityEngine.Debug.Log($"{path} Stored.");
            return Task.CompletedTask;
        }

        public Task<string> LoadAsync(string path)
        {
            UnityEngine.Debug.Log($"Requested {path}.");
            if (Storage.ContainsKey(path))
                return Task.FromResult(Storage[path]);
            return null;
        }

        public Task Delete(string path)
        {
            UnityEngine.Debug.Log($"Remove {path}.");
            return Task.Run(() =>
            {
                if (Storage.ContainsKey(path))
                    Storage.Remove(path);
            });
        }

    }
}


