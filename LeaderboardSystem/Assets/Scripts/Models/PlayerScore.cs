namespace Repositories.Models
{
    public class PlayerScore
    {
        public int PlayerId { get; set; }
        public int Score { get; set; }

        public PlayerScore() { }

        public PlayerScore(int playerId, int score)
        {
            PlayerId = playerId;
            Score = score;
        }
    }
}