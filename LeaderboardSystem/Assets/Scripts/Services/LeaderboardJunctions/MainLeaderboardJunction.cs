using UnityEngine;
public class MainLeaderboardJunction : LeaderboardJunction
{
    [SerializeField]
    private string leaderboardName = nameof(MainLeaderboardJunction);
    private void Awake()
    {
        Setup(leaderboardName);
    }
}
