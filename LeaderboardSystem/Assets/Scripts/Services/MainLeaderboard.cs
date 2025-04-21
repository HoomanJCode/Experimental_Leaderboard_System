using System.Threading.Tasks;
using UnityEngine;

public class MainLeaderboard : MonoBehaviour
{
    LeaderboardService leaderboardService;
    private void Awake()
    {
        leaderboardService = new LeaderboardService(nameof(MainLeaderboard));
    }
}