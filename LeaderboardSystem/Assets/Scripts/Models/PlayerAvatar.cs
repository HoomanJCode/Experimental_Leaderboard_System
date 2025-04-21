namespace Repositories.Models
{
    public class PlayerAvatar
    {
        public int PlayerId { get; set; }
        public byte[] ProfileImage { get; set; }

        public PlayerAvatar() { }

        public PlayerAvatar(int playerId, byte[] profileImage)
        {
            PlayerId = playerId;
            ProfileImage = profileImage;
        }
    }
}
