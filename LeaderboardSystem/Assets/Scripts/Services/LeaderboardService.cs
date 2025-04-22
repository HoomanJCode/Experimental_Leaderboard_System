using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using System.Linq;
using Services;
using System;
using System.Diagnostics;

public class LeaderboardService : LeaderboardRepository, ILeaderboardService
{
    public LeaderboardService(string LeaderboardKey) : base(LeaderboardKey)
    {
    }

    public async Task SortScoresAsync()
    {
        
        Scores.Sort((a, b) => a.Score.CompareTo(b.Score));
        //todo: save changes in period
        await SaveChangesAsync();
    }

    public async Task PushScoreAsync(PlayerScore score)
    {
        if (!await PlayersAuthentication.PlayerExist(score.PlayerId))
            throw new InvalidOperationException("Player Not Exist!");
        Scores.Add(score);
        await SortScoresAsync();
    }

    public async Task<List<PlayerScore>> GetHighestScoresAsync(int count)
    {
        await SortScoresAsync();
        return Scores.Take(count).ToList();
    }
}
