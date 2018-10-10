//-----------------------------------------------------------------------
// <copyright file="Leaderboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using PlayFab.ClientModels;
    using System.Collections.Generic;
    using UnityEngine;

    // https://api.playfab.com/documentation/client/method/GetLeaderboardAroundPlayer

    public abstract class PlayFabLeaderboard<T> : VirtualizedScrollRect<T> where T : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private GameObject loadingObject;
        [SerializeField] private string leaderboardName;
        [SerializeField] private bool isCenteredAroundPlayer;
        [SerializeField] private bool isFriendLeaderboard;
        #pragma warning restore 0649

        // GetFriendLeaderboardAroundPlayer
        // GetFriendLeaderboard
        // GetLeaderboard
        // GetLeaderboardAroundPlayer

        private List<PlayerLeaderboardEntry> entries = new List<PlayerLeaderboardEntry>()
        {
            new PlayerLeaderboardEntry { DisplayName = null,                Position = 0, StatValue = 51516511, PlayFabId = "5F3FB9882E45DB59" },
            new PlayerLeaderboardEntry { DisplayName = "Tea-Note",          Position = 1, StatValue = 5051485,  PlayFabId = "494F1D2872AC346B" },
            new PlayerLeaderboardEntry { DisplayName = "brgihsy",           Position = 2, StatValue = 5001821,  PlayFabId = "62458EBB44252C80" },
            new PlayerLeaderboardEntry { DisplayName = "brgishy (android)", Position = 3, StatValue = 5000611,  PlayFabId = "C4DDF08937E39CBB" },
            new PlayerLeaderboardEntry { DisplayName = "brgishy (editor)",  Position = 4, StatValue = 400011,   PlayFabId = "18663527F862664A" },
            new PlayerLeaderboardEntry { DisplayName = "Tea-iPad",          Position = 5, StatValue = 35001,    PlayFabId = "B54A13C76EB39B7F" },
        };
        
        protected abstract void ShowLeaderboardEntry(T item, PlayerLeaderboardEntry entry, bool isCurrentPlayer);

        protected override void Awake()
        {
            base.Awake();

            this.OnShowItem += this.ShowItem;
            this.SetCount(this.entries.Count);
        }

        private void ShowItem(T item, int index)
        {
            PlayerLeaderboardEntry entry = entries[index];
            ShowLeaderboardEntry(item, entry, entry.PlayFabId == PF.PlayFabId);
        }
    }
}
