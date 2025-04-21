using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using UnityEngine;
using System.Linq;
public interface ILeaderboardService
{
    Task SortScoresAsync();
    Task PushScoreAsync(PlayerScore score);
    Task<List<PlayerScore>> GetHighestScoresAsync(int count);
}