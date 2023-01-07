using OceanBattle.Client.Core.Abstractions;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Ships;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace OcenBattle.Client.Core.Services
{
    public class GameApiClient : ApiClientBase, IGameApiClient
    {       
        public GameApiClient(HttpClient httpClient)
            : base(httpClient)
        {           
        }

        public async Task<IEnumerable<UserDto>?> GetActivePlayers()
        {
            HttpResponseMessage response = 
                await _httpClient.GetAsync($"game/active");

            response.EnsureSuccessStatusCode();

            IEnumerable<UserDto>? players =
                await response.Content.ReadAsAsync<IEnumerable<UserDto>>(_formatters);

            return players;
        }

        public async Task<IEnumerable<Level>?> GetLevels()
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync($"game/levels");

            response.EnsureSuccessStatusCode();

            IEnumerable<LevelDto>? levelsDtos =
                await response.Content.ReadAsAsync<IEnumerable<LevelDto>>(_formatters);

            if (levelsDtos is null)
                return null;

            IEnumerable<Level>? levels = levelsDtos
                .Select(l => new Level
                {
                    BattlefieldSize = l.BattlefieldSize,
                    AvailableTypes = l.AvailableTypes is null ? null :
                    l.AvailableTypes.ToDictionary(kvp => _typesReversed[kvp.Key], kvp => kvp.Value)
                });

            return levels;
        }

        private readonly Dictionary<string, Type> _typesReversed = new Dictionary<string, Type>
        {
            { nameof(Corvette), typeof(Corvette) },
            { nameof(Frigate), typeof(Frigate) },
            { nameof(Destroyer), typeof(Destroyer) },
            { nameof(Cruiser), typeof(Cruiser) },
            { nameof(Battleship), typeof(Battleship) }
        };
    }
}
