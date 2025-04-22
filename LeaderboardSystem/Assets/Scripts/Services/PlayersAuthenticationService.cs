using System;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using UnityEngine;

namespace Services
{
    public class PlayersAuthenticationService
    {
        // Dependencies
        private readonly IPlayerRepository _playerRepository;
        private readonly IAvatarRepository _avatarRepository;

        // Private constructor for singleton
        public PlayersAuthenticationService()
        {
            // Initialize repositories (could be injected via DI in a real scenario)
            _playerRepository = new PlayerRepository();
            _avatarRepository = new AvatarRepository();
        }


        /// <summary>
        /// Adds a new player to the system
        /// </summary>
        public async Task<Player> RegisterPlayer(string name,string description,Sprite avatar=null)
        {
            var addedPlayerId = await _playerRepository.AddPlayerAsync(new SavePlayerDto(name,description));
            if (avatar != null)
                await _avatarRepository.AddOrUpdateAsync(addedPlayerId, avatar);

            //await _playerRepository.SaveChangesAsync();
            return new Player(addedPlayerId, name, description);
        }

        /// <summary>
        /// Removes a player from the system
        /// </summary>
        public async Task<bool> RemovePlayer(int playerId)
        {
            if (!await _playerRepository.Exist(playerId)) return false;
            if (await _avatarRepository.HasAvatarAsync(playerId))
                await _avatarRepository.DeleteAsync(playerId);
            await _playerRepository.DeleteAsync(playerId);
            await _playerRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates an existing player
        /// </summary>
        public async Task<bool> UpdatePlayer(int playerId,string name,string description)
        {
            var player = new Player(playerId,name,description);
            if (!await _playerRepository.Exist(player.Id)) return false;
            await _playerRepository.UpdatePlayerAsync(player);
            await _playerRepository.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Updates an existing player Avatar
        /// </summary>
        public async Task<bool> UpdatePlayerAvatar(int playerid,Sprite avatar)
        {
            if (avatar == null)
            throw new ArgumentNullException(nameof(avatar));
            if (!await _playerRepository.Exist(playerid)) return false;
            if (await _avatarRepository.HasAvatarAsync(playerid))
                await _avatarRepository.AddOrUpdateAsync(playerid, avatar);
            return true;
        }

        /// <summary>
        /// Retrieves a player by their ID
        /// </summary>
        public async Task<Player> GetPlayerById(int playerId) => await _playerRepository.GetByIdAsync(playerId);
        public async Task<bool> PlayerExist(int playerId) => await _playerRepository.Exist(playerId);
        public async Task<Sprite> GetPlayerAvatarById(int playerId) => await _avatarRepository.GetByIdAsync(playerId);
    }
}