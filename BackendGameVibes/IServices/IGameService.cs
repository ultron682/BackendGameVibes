﻿using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;
using BackendGameVibes.Models.Steam;

namespace BackendGameVibes.IServices {
    public interface IGameService : IDisposable {
        Task<Game?> CreateGame(int steamGameId);
        SteamApp[] FindSteamAppByName(string searchingName);
        SteamApp[] GetAllSteamIdsGames();
        Task<object?> GetGame(int id);
        Task<object[]> GetGames();
        Task<object[]> GetFilteredGames(FiltersGamesDTO filtersGamesDTO);
        Task<object[]> GetGenres();
        Task<object[]> GetLandingGames();
        Task<object[]> GetUpcomingGames();
    }
}