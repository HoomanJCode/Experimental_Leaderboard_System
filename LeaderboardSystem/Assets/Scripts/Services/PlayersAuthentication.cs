using System.Threading.Tasks;
using Repositories.Models;
using UnityEngine;

namespace Services
{
    public static class PlayersAuthentication
    {
        // Singleton instance
        private static PlayersAuthenticationService _instance;
        public static PlayersAuthenticationService Instance => _instance ??= new PlayersAuthenticationService();

        /// <summary>
        /// Adds a new player to the system
        /// </summary>
        public static async Task<Player> RegisterPlayer(string name, string description, Texture2D avatar=null)=>
        await Instance.RegisterPlayer(name, description, avatar);

        /// <summary>
        /// Removes a player from the system
        /// </summary>
        public static async Task<bool> RemovePlayer(int playerId)=> await Instance.RemovePlayer(playerId);
        /// <summary>
        /// Updates an existing player
        /// </summary>
        public static async Task<bool> UpdatePlayer(Player player) => await Instance.UpdatePlayer(player);
        /// <summary>
        /// Updates an existing player Avatar
        /// </summary>
        public static async Task<bool> UpdatePlayerAvatar(PlayerAvatar avatar) => await Instance.UpdatePlayerAvatar(avatar);
        /// <summary>
        /// Retrieves a player by their ID
        /// </summary>
        public static async Task<Player> GetPlayerById(int playerId) => await Instance.GetPlayerById(playerId);
        public static async Task<bool> PlayerExist(int playerId) => await Instance.PlayerExist(playerId);
        public static async Task<PlayerAvatar> GetPlayerAvatarById(int playerId) => await Instance.GetPlayerAvatarById(playerId);


    }
}