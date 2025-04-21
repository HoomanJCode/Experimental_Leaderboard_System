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

    public async Task SortScoresAsync()
    {
        Scores.Sort((a, b) => b.Score.CompareTo(a.Score));
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
