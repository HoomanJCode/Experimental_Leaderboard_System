using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using System.Linq;
using Services;
using System;

public class LeaderboardService : LeaderboardRepository, ILeaderboardService
{
    public LeaderboardService(string LeaderboardKey) : base(LeaderboardKey)
    {
    }

    public Task<List<PlayerScore>> GetSortedScores()
    {
        var list=Scores.Select(x => new PlayerScore(x.Key,x.Value)).ToList();
        list.Sort((a, b) => b.Score.CompareTo(a.Score));
        return Task.FromResult(list);
    }

    public async Task PushScoreAsync(int playerId,int score)
    {
        if (!await PlayersAuthentication.PlayerExist(playerId))
            throw new InvalidOperationException("Player Not Exist!");
        Scores.AddOrUpdate(playerId,score,(a,b)=>score);
    }
    public async Task DeleteScoreAsync(int playerId)
    {
        Scores.TryRemove(playerId, out _);
        await Task.CompletedTask;
    }
    public async Task<List<PlayerScore>> GetHighestScoresAsync(int count)
    {
        var sortedList=await GetSortedScores();
        return sortedList.Take(count).ToList();
    }

}
