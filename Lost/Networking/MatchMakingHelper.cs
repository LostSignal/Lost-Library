//-----------------------------------------------------------------------
// <copyright file="MatchMakingHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.Types;

    public static class MatchMakingHelper
    {
        // TODO [bgish]:  Might be worth it to make this localizable someday
        public static readonly string ValidMatchNameCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static readonly string[] AvailableMatchHosts = new string[] { "us1-mm.unet.unity3d.com",
                                                                             "eu1-mm.unet.unity3d.com",
                                                                             "ap1-mm.unet.unity3d.com" };

        private static readonly System.Random random;

        // matchmaking configuration
        private static NetworkMatch matchMaker = null;
        private static int closestMatchHostIndex = -1;
        private static int currentMatchHostIndex = -1;
        private static bool isInitializationRunning;
        private static bool isInitialized;

        public static int RequestDomain { get; set; }

        public static int CurrentMatchHostIndex
        {
            get { return currentMatchHostIndex; }
        }

        public static int ClosestMatchHostIndex
        {
            get { return closestMatchHostIndex; }
        }

        static MatchMakingHelper()
        {
            // generating a new good random number
            byte[] guidByteArray = Guid.NewGuid().ToByteArray();
            int seed = guidByteArray[0] << 24 | guidByteArray[1] << 16 | guidByteArray[2] << 8 | guidByteArray[3];
            random = new System.Random(seed);
        }

        public static void SetMatchHostIndex(int index)
        {
            currentMatchHostIndex = index;

            if (matchMaker != null)
            {
                matchMaker.baseUri = new Uri("https://" + AvailableMatchHosts[currentMatchHostIndex] + ":443");
            }
        }

        public static string GenerateRandomRoomName(int size)
        {
            StringBuilder roomName = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                int randomIndex = random.Next(0, AvailableMatchHosts.Length);
                roomName.Append(AvailableMatchHosts[randomIndex]);
            }

            return roomName.ToString();
        }

        public static UnityTask<MatchInfo> CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, int eloScore)
        {
            return UnityTask<MatchInfo>.Run(CreateMatchCoroutine(matchName, matchSize, matchAdvertise, matchPassword, eloScore));
        }

        public static UnityTask<List<MatchInfoSnapshot>> ListMatches(string matchName, string matchPassword, int eloScore, bool filterOutPrivateMatches)
        {
            return UnityTask<List<MatchInfoSnapshot>>.Run(ListMatchesCoroutine(matchName, matchPassword, eloScore, filterOutPrivateMatches));
        }

        public static UnityTask<bool> DoesMatchExist(string matchName)
        {
            return UnityTask<bool>.Run(DoesMatchExistCoroutine(matchName));
        }

        public static UnityTask<MatchInfo> JoinMatch(NetworkID networkId, string matchPassword, int eloScoreForClient)
        {
            return UnityTask<MatchInfo>.Run(JoinMatchCoroutine(networkId, matchPassword, eloScoreForClient));
        }

        public static UnityTask<bool> DestroyMatch(MatchInfo matchInfo)
        {
            return UnityTask<bool>.Run(DestroyMatchCoroutine(matchInfo));
        }

        public static UnityTask<bool> DropConnection(MatchInfo matchInfo)
        {
            return UnityTask<bool>.Run(DropConnectionCoroutine(matchInfo));
        }

        public static UnityTask<bool> SetMatchAttributes(MatchInfo matchInfo, bool isListed)
        {
            return UnityTask<bool>.Run(SetMatchAttributesCoroutine(matchInfo, isListed));
        }

        private static IEnumerator<MatchInfo> CreateMatchCoroutine(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, int eloScore)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(MatchInfo);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            MatchInfo matchInfoResult = null;

            matchMaker.CreateMatch(
                matchName,
                matchSize,
                matchAdvertise,
                matchPassword,
                string.Empty,   // publicClientAddress
                string.Empty,   // privateClientAddress
                eloScore,
                RequestDomain,
                (bool success, string extendedInfo, MatchInfo matchInfo) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    matchInfoResult = matchInfo;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return null;
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return matchInfoResult;
            }
        }

        private static IEnumerator<List<MatchInfoSnapshot>> ListMatchesCoroutine(string matchName, string matchPassword, int eloScore, bool filterOutPrivateMatches)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(List<MatchInfoSnapshot>);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            List<MatchInfoSnapshot> matchesResult = null;

            matchMaker.ListMatches(
                0,  // startPageNumber
                10, // resultPageSize
                matchName,
                filterOutPrivateMatches,
                eloScore,
                RequestDomain,
                (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    matchesResult = matches;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return null;
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return matchesResult;
            }
        }

        private static IEnumerator<bool> DoesMatchExistCoroutine(string matchName)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(bool);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            List<MatchInfoSnapshot> matchesResult = null;

            matchMaker.ListMatches(
                0,         // startPageNumber
                10,        // resultPageSize
                matchName,
                false,     // filterOutPrivateMatches
                0,         // eloScore
                RequestDomain,
                (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    matchesResult = matches;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return default(bool);
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return matchesResult.Count > 0;
            }
        }

        private static IEnumerator<MatchInfo> JoinMatchCoroutine(NetworkID networkId, string matchPassword, int eloScoreForClient)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(MatchInfo);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            MatchInfo matchInfoResult = null;

            matchMaker.JoinMatch(
                networkId,
                matchPassword,
                string.Empty,    // publicClientAddress
                string.Empty,    // privateClientAddress
                eloScoreForClient,
                RequestDomain,
                (bool success, string extendedInfo, MatchInfo matchInfo) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    matchInfoResult = matchInfo;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return null;
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return matchInfoResult;
            }
        }

        private static IEnumerator<bool> DestroyMatchCoroutine(MatchInfo matchInfo)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(bool);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;

            matchMaker.DestroyMatch(
                matchInfo.networkId,
                matchInfo.domain,
                (bool success, string extendedInfo) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return default(bool);
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return true;
            }
        }

        private static IEnumerator<bool> DropConnectionCoroutine(MatchInfo matchInfo)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(bool);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;

            matchMaker.DropConnection(
                matchInfo.networkId,
                matchInfo.nodeId,
                matchInfo.domain,
                (bool success, string extendedInfo) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return default(bool);
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return true;
            }
        }

        private static IEnumerator<bool> SetMatchAttributesCoroutine(MatchInfo matchInfo, bool isListed)
        {
            if (isInitialized == false)
            {
                var initialize = InitializeMatchMaker();

                while (initialize.MoveNext())
                {
                    yield return default(bool);
                }
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;

            matchMaker.SetMatchAttributes(
                matchInfo.networkId,
                isListed,
                matchInfo.domain,
                (bool success, string extendedInfo) =>
                {
                    successResult = success;
                    extendedInfoResult = extendedInfo;
                    isDone = true;
                });

            while (isDone == false)
            {
                yield return default(bool);
            }

            if (successResult == false)
            {
                throw new MatchMakingException(extendedInfoResult);
            }
            else
            {
                yield return true;
            }
        }

        private static IEnumerator InitializeMatchMaker()
        {
            if (isInitializationRunning == true)
            {
                yield return null;
            }

            if (isInitialized)
            {
                yield break;
            }

            isInitializationRunning = true;

            // creating match maker
            if (matchMaker == null)
            {
                var networkMatchObject = SingletonUtil.GetOrCreateSingletonChildObject("NetworkMatch").gameObject;
                matchMaker = networkMatchObject.AddComponent<NetworkMatch>();
            }

            // finding closest match making host
            if (closestMatchHostIndex == -1)
            {
                int minMatchHostIndex = -1;
                int minPing = int.MaxValue;

                for (int i = 0; i < AvailableMatchHosts.Length; i++)
                {
                    Ping ping = new Ping(AvailableMatchHosts[i]);

                    while (ping.isDone == false)
                    {
                        yield return null;
                    }

                    if (ping.time < minPing)
                    {
                        minMatchHostIndex = i;
                        minPing = ping.time;
                    }
                }

                closestMatchHostIndex = minMatchHostIndex;
            }

            if (currentMatchHostIndex == -1)
            {
                currentMatchHostIndex = closestMatchHostIndex;
            }

            SetMatchHostIndex(currentMatchHostIndex);

            isInitialized = matchMaker != null && closestMatchHostIndex != -1;
            isInitializationRunning = false;
        }
    }
}
