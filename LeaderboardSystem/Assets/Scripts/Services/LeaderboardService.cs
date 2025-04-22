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

        await Task.Run(() => Scores.Sort((a, b) => b.Score.CompareTo(a.Score)));
        //todo: save changes in period
        //await SaveChangesAsync();
    }

    public async Task PushScoreAsync(PlayerScore score)
    {
        if (!await PlayersAuthentication.PlayerExist(score.PlayerId))
            throw new InvalidOperationException("Player Not Exist!");
        var lastScore = Scores.Find(x=>x.PlayerId==score.PlayerId);
        if (lastScore == null) Scores.Add(score);
        else lastScore.Score = score.Score;
        await SortScoresAsync();
    }
    public async Task DeleteScoreAsync(int playerId)
    {
        await Task.Run(()=> Scores.RemoveAll(x => x.PlayerId == playerId));
    }
    public async Task<List<PlayerScore>> GetHighestScoresAsync(int count)
    {
        await SortScoresAsync();
        return Scores.Take(count).ToList();
    }
}
