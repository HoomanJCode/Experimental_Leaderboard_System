using UnityEngine;

public abstract class LeaderboardJunction : MonoBehaviour
{
    public LeaderboardService Service { get; set; }
    protected virtual void Awake()
    {
        Service = new LeaderboardService(nameof(MainLeaderboardJunction));
    }
}