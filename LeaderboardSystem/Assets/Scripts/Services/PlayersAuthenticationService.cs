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
        public async Task RegisterPlayer(string name,string description,Texture2D avatar)
        {
            var addedPlayerId = await _playerRepository.AddPlayerAsync(new SavePlayerDto(name,description));
            if (avatar != null)
                await _avatarRepository.AddAsync(new PlayerAvatar(addedPlayerId,avatar.GetRawTextureData()));

            await _playerRepository.SaveChangesAsync();
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
        public async Task<bool> UpdatePlayer(Player player)
        {
            if (player == null)
            throw new ArgumentNullException(nameof(player));
            if (!await _playerRepository.Exist(player.Id)) return false;
            await _playerRepository.UpdatePlayerAsync(player);
            await _playerRepository.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Updates an existing player Avatar
        /// </summary>
        public async Task<bool> UpdatePlayerAvatar(PlayerAvatar avatar)
        {
            if (avatar == null)
            throw new ArgumentNullException(nameof(avatar));
            if (!await _playerRepository.Exist(avatar.PlayerId)) return false;
            if (await _avatarRepository.HasAvatarAsync(avatar.PlayerId))
                await _avatarRepository.UpdateAsync(avatar);
            return true;
        }

        /// <summary>
        /// Retrieves a player by their ID
        /// </summary>
        public async Task<Player> GetPlayerById(int playerId) => await _playerRepository.GetByIdAsync(playerId);
        public async Task<bool> PlayerExist(int playerId) => await _playerRepository.Exist(playerId);
        public async Task<PlayerAvatar> GetPlayerAvatarById(int playerId) => await _avatarRepository.GetByIdAsync(playerId);
    }
}