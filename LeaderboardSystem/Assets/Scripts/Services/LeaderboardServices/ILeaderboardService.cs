using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;
public interface ILeaderboardService
{
    Task<List<PlayerScore>> GetSortedScores();
    Task PushScoreAsync(int playerId, int score);
    Task<List<PlayerScore>> GetHighestScoresAsync(int count);
}