using UnityEngine;

public abstract class LeaderboardJunction : MonoBehaviour
{
    public LeaderboardService Service { get; set; }
    protected void Setup(string leaderboardName)
    {
        Service = new LeaderboardService(leaderboardName);
    }
}