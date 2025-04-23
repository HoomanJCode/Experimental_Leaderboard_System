using System.Threading.Tasks;
using Repositories.Models;
using UnityEngine;

namespace Services
{
    public static class PlayersAuthentication
    {
        // Singleton instance
        public static PlayersAuthenticationService Instance => PlayersAuthenticationService.Instance;

        /// <summary>
        /// Adds a new player to the system
        /// </summary>
        public static async Task<Player> RegisterPlayer(string name, string description, Sprite avatar=null)=>
        await Instance.RegisterPlayer(name, description, avatar);

        /// <summary>
        /// Removes a player from the system
        /// </summary>
        public static async Task<bool> RemovePlayer(int playerId)=> await Instance.RemovePlayer(playerId);
        /// <summary>
        /// Updates an existing player
        /// </summary>
        public static async Task<bool> UpdatePlayer(int playerId, string name, string description) => await Instance.UpdatePlayer(playerId,name,description);
        /// <summary>
        /// Updates an existing player Avatar
        /// </summary>
        public static async Task<bool> UpdatePlayerAvatar(int playerId,Sprite avatar) => await Instance.UpdatePlayerAvatar(playerId,avatar);
        /// <summary>
        /// Retrieves a player by their ID
        /// </summary>
        public static async Task<Player> GetPlayerById(int playerId) => await Instance.GetPlayerById(playerId);
        public static async Task<bool> PlayerExist(int playerId) => await Instance.PlayerExist(playerId);
        public static async Task<Sprite> GetPlayerAvatarById(int playerId) => await Instance.GetPlayerAvatarById(playerId);


    }
}