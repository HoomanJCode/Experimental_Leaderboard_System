using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using System.Linq;
using Services;
using System;
using UnityEngine;
using System.Collections;

public class LeaderboardService : LeaderboardRepository, ILeaderboardService, IServiceSetup
{
    public LeaderboardService(string LeaderboardKey) : base(LeaderboardKey)
    {
        CoroutineRunner.Singletone.StartCoroutine(MemChangesEnumerator());
    }

    private bool scoresLoaded = false;
    private float saveTimer = -1;
    private void SaveChanges()
    {
        saveTimer = 4;
    }
    private IEnumerator MemChangesEnumerator()
    {
        yield return PlayersAuthenticationService.Instance.WaitCheckForSetup().UntileComplete();
        yield return LoadScoresAsync().UntileComplete();
        scoresLoaded = true;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (saveTimer < 0) continue;
            if (saveTimer == 0) yield return SaveChangesAsync().UntileComplete();
            saveTimer--;
        }
    }
    public Task<List<PlayerScore>> GetSortedScores()
    {
        var list=Scores.Select(x => new PlayerScore(x.Key,x.Value)).ToList();
        list.Sort((a, b) => b.Score.CompareTo(a.Score));
        return Task.FromResult(list);
    }

    public async Task PushScoreAsync(int playerId,int score)
    {
        if (!await PlayersAuthenticationService.Instance.PlayerExist(playerId))
            throw new InvalidOperationException("Player Not Exist!");
        Scores.AddOrUpdate(playerId,score,(a,b)=>score);
        SaveChanges();
    }
    public async Task DeleteScoreAsync(int playerId)
    {
        Scores.TryRemove(playerId, out _);
        SaveChanges();
        await Task.CompletedTask;
    }
    public async Task<List<PlayerScore>> GetHighestScoresAsync(int count)
    {
        var sortedList=await GetSortedScores();
        return sortedList.Take(count).ToList();
    }
    public async Task WaitCheckForSetup()
    {
        while (!scoresLoaded)
            await Task.Delay(100);
    }

}
