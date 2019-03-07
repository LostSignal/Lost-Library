//-----------------------------------------------------------------------
// <copyright file="PlayFabLeaderboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab.ClientModels;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class PlayFabLeaderboard<T> : VirtualizedScrollRect<T> where T : MonoBehaviour
    {
        private static readonly int ProgressHash = Animator.StringToHash("Progress");

        #pragma warning disable 0649
        [Tooltip("This is how many entires that will be held in memory.")]
        [SerializeField] private int maxEntriesBeforeTrimming = 200;

        [Header("Loading More Entries Animations")]
        [SerializeField] private float dragToRefreshDistance = 200.0f;
        [SerializeField] private Animator topLoadingAnimator;
        [SerializeField] private Animator bottomLoadingAnimator;

        [Header("PlayFab Settings")]
        [SerializeField] private string leaderboardName;
        [SerializeField] private bool isCenteredAroundPlayer;
        [SerializeField] private bool shouldShowAvatarUrl;

        [Range(10, 100)]
        [SerializeField] private int maxResultsCount = 10;

        [Header("Friend Properties")]
        [SerializeField] private bool isFriendLeaderboard;
        [SerializeField] private bool includeFacebookFriends;
        [SerializeField] private bool includeSteamFriends;
        #pragma warning restore 0649

        private PlayerProfileViewConstraints profileContraints = new PlayerProfileViewConstraints();
        private List<PlayerLeaderboardEntry> entries = new List<PlayerLeaderboardEntry>();
        private float lastTopDistance = 0.0f;
        private float lastBottomDistance = 0.0f;
        private bool moreTopEntiresExist = true;
        private bool moreBottomEntriesExist = true;
        private bool coroutineRunning = false;

        public bool IsCenteredAroundPlayer
        {
            get { return this.isCenteredAroundPlayer; }
            set { this.isCenteredAroundPlayer = value; }
        }

        public bool IsFriendLeaderboard
        {
            get { return this.isFriendLeaderboard; }
            set { this.isFriendLeaderboard = value; }
        }

        public bool IncludeFacebookFriends
        {
            get { return this.includeFacebookFriends; }
            set { this.includeFacebookFriends = value; }
        }

        public bool IncludeSteamFriends
        {
            get { return this.includeSteamFriends; }
            set { this.includeSteamFriends = value; }
        }

        public void RefreshLeaderboard()
        {
            this.entries.Clear();
            this.SetCount(0);
            this.ScrollRect.verticalNormalizedPosition = 0.0f;  // Reset the ScrollRect back to the top

            this.StartCoroutine(this.RefreshLeaderboardCoroutine(null));
        }

        public void AddMoreToTop()
        {
            this.StartCoroutine(this.AddToTopCoroutine());
        }

        public void AddMoreToBottom()
        {
            this.StartCoroutine(this.AddToBottomCoroutine());
        }

        protected abstract void ShowLeaderboardEntry(T item, PlayerLeaderboardEntry entry, bool isCurrentPlayer);

        protected override void Awake()
        {
            base.Awake();

            this.OnShowItem += this.ShowItem;
        }

        protected virtual void OnEnable()
        {
            this.DisableLoadingIndicators();
        }

        protected override void Update()
        {
            base.Update();

            if (this.coroutineRunning)
            {
                return;
            }

            if (this.moreTopEntiresExist && this.TopDistance != this.lastTopDistance)
            {
                bool animatorEnabled = this.TopDistance > 0.0f;
                this.topLoadingAnimator.gameObject.SafeSetActive(animatorEnabled);

                if (animatorEnabled)
                {
                    this.topLoadingAnimator.SetFloat(ProgressHash, this.TopDistance / this.dragToRefreshDistance);
                }

                this.lastTopDistance = this.TopDistance;

                if (this.TopDistance > this.dragToRefreshDistance)
                {
                    this.AddMoreToTop();
                }
            }

            if (this.moreBottomEntriesExist && this.BottomDistance != this.lastBottomDistance)
            {
                bool animatorEnabled = this.BottomDistance > 0.0f;
                this.bottomLoadingAnimator.gameObject.SafeSetActive(animatorEnabled);

                if (animatorEnabled)
                {
                    this.bottomLoadingAnimator.SetFloat(ProgressHash, this.BottomDistance / this.dragToRefreshDistance);
                }

                this.lastBottomDistance = this.BottomDistance;

                if (this.BottomDistance > this.dragToRefreshDistance)
                {
                    this.AddMoreToBottom();
                }
            }
        }

        private void DisableLoadingIndicators()
        {
            this.topLoadingAnimator.gameObject.SafeSetActive(false);
            this.bottomLoadingAnimator.gameObject.SafeSetActive(false);
        }

        private void ShowItem(T item, int index)
        {
            PlayerLeaderboardEntry entry = entries[index];
            ShowLeaderboardEntry(item, entry, entry.PlayFabId == PF.User.PlayFabId);
        }

        private IEnumerator RefreshLeaderboardCoroutine(int? startPosition)
        {
            // Setting any profile constraints
            this.profileContraints.ShowDisplayName = true;
            this.profileContraints.ShowAvatarUrl = this.shouldShowAvatarUrl;

            this.moreBottomEntriesExist = true;
            this.moreTopEntiresExist = true;

            if (this.isFriendLeaderboard)
            {
                if (this.isCenteredAroundPlayer && startPosition.HasValue == false)
                {
                    var leaderboard = PF.Do(new GetFriendLeaderboardAroundPlayerRequest
                    {
                        IncludeFacebookFriends = this.includeFacebookFriends,
                        IncludeSteamFriends = this.includeSteamFriends,
                        MaxResultsCount = this.maxResultsCount,
                        StatisticName = this.leaderboardName,
                        ProfileConstraints = this.profileContraints,
                    });

                    yield return leaderboard;

                    this.AppendLeaderboardResults(leaderboard, leaderboard.HasError == false ? leaderboard.Value.Leaderboard : null);
                }
                else
                {
                    var leaderboard = PF.Do(new GetFriendLeaderboardRequest
                    {
                        IncludeFacebookFriends = this.includeFacebookFriends,
                        IncludeSteamFriends = this.includeSteamFriends,
                        MaxResultsCount = this.maxResultsCount,
                        StatisticName = this.leaderboardName,
                        StartPosition = startPosition.HasValue ? startPosition.Value : 0,
                        ProfileConstraints = this.profileContraints,
                    });

                    yield return leaderboard;

                    this.AppendLeaderboardResults(leaderboard, leaderboard.HasError == false ? leaderboard.Value.Leaderboard : null);
                }
            }
            else
            {
                if (this.isCenteredAroundPlayer && startPosition.HasValue == false)
                {
                    var leaderboard = PF.Do(new GetLeaderboardAroundPlayerRequest
                    {
                        MaxResultsCount = this.maxResultsCount,
                        StatisticName = this.leaderboardName,
                        ProfileConstraints = this.profileContraints,
                    });

                    yield return leaderboard;

                    this.AppendLeaderboardResults(leaderboard, leaderboard.HasError == false ? leaderboard.Value.Leaderboard : null);
                }
                else
                {
                    var leaderboard = PF.Do(new GetLeaderboardRequest
                    {
                        MaxResultsCount = this.maxResultsCount,
                        StatisticName = this.leaderboardName,
                        StartPosition = startPosition.HasValue ? startPosition.Value : 0,
                        ProfileConstraints = this.profileContraints,
                    });

                    yield return leaderboard;

                    this.AppendLeaderboardResults(leaderboard, leaderboard.HasError == false ? leaderboard.Value.Leaderboard : null);
                }
            }

            // Testing is we should put the current player in the center of the leaderboard
            if (startPosition.HasValue == false && this.isCenteredAroundPlayer)
            {
                for (int i = 0; i < this.entries.Count; i++)
                {
                    if (this.entries[i].PlayFabId == PF.User.PlayFabId)
                    {
                        this.CenterOnIndex(i);
                    }
                }
            }
        }

        private IEnumerator AddToTopCoroutine()
        {
            if (this.coroutineRunning)
            {
                yield break;
            }

            this.coroutineRunning = true;
            this.ScrollRect.enabled = false;

            if (this.entries.Count > 0)
            {
                PlayerLeaderboardEntry firstEntry = this.entries[0];

                // Making sure we're not already at the top
                if (firstEntry.Position != 0)
                {
                    int position = Mathf.Max(firstEntry.Position - this.maxResultsCount - 1, 0);

                    yield return this.RefreshLeaderboardCoroutine(position);
                }
            }

            this.coroutineRunning = false;
            this.ScrollRect.enabled = true;
            this.DisableLoadingIndicators();
        }

        private IEnumerator AddToBottomCoroutine()
        {
            if (this.coroutineRunning)
            {
                yield break;
            }

            this.coroutineRunning = true;
            this.ScrollRect.enabled = false;

            if (this.entries.Count > 0)
            {
                int oldLastPostion = this.entries[this.entries.Count - 1].Position;

                PlayerLeaderboardEntry lastEntry = this.entries[this.entries.Count - 1];
                yield return this.RefreshLeaderboardCoroutine(lastEntry.Position + 1);

                int newLastPostion = this.entries[this.entries.Count - 1].Position;

                // TODO [bgish]: Need to be able to detect if an error occored, if one did, it would make this.moreBottomEntriesExist false when it shouldn't

                // If we asked for more, but didn't get a full list back, then there aren't any more to get
                this.moreBottomEntriesExist = (newLastPostion - oldLastPostion) == this.maxResultsCount;
            }

            this.coroutineRunning = false;
            this.ScrollRect.enabled = true;
            this.DisableLoadingIndicators();
        }

        private void AppendLeaderboardResults(IUnityTask unityTask, List<PlayerLeaderboardEntry> leaderboardEntries)
        {
            if (unityTask.HasError || leaderboardEntries == null)
            {
                // TODO [bgish]: Do some error handling
                return;
            }

            if (leaderboardEntries.Count == 0)
            {
                return;
            }

            bool addToBeginningOfList = this.entries.Count == 0 ? false : leaderboardEntries[0].Position < this.entries[0].Position;

            if (addToBeginningOfList)
            {
                // removing dups from the results
                for (int i = leaderboardEntries.Count - 1; i >= 0; i--)
                {
                    if (leaderboardEntries[i].Position >= this.entries[0].Position)
                    {
                        leaderboardEntries.RemoveAt(i);
                    }
                }

                // Shifting results so don't get a pop
                this.ShiftContent(-leaderboardEntries.Count);

                leaderboardEntries.AddRange(this.entries);
                this.entries = leaderboardEntries;
            }
            else
            {
                this.entries.AddRange(leaderboardEntries);
            }

            // Trimming elements that are over the max entries size
            if (this.entries.Count > this.maxEntriesBeforeTrimming)
            {
                int itemsToTrimCount = this.entries.Count - this.maxEntriesBeforeTrimming;

                if (addToBeginningOfList)
                {
                    this.entries.RemoveRange(this.entries.Count - itemsToTrimCount, itemsToTrimCount);
                }
                else
                {
                    this.entries.RemoveRange(0, itemsToTrimCount);
                    this.ShiftContent(itemsToTrimCount);
                }
            }

            // Making sure more top entries exist
            this.moreTopEntiresExist = this.entries[0].Position != 0;

            this.SetCount(this.entries.Count);
        }
    }
}

#endif
