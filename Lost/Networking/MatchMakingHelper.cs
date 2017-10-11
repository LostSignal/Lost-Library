//-----------------------------------------------------------------------
// <copyright file="MatchMakingHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.Types;

    public static class MatchMakingHelper
    {
        private static bool hasStartedMatchMaker = false;

        public static int RequestDomain { get; set; }  

        public static UnityTask<MatchInfo> CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, int eloScore)
        {
            return UnityTask<MatchInfo>.Run(CreateMatchCoroutine(matchName, matchSize, matchAdvertise, matchPassword, eloScore));
        }
        
        public static UnityTask<List<MatchInfoSnapshot>> ListMatches(string matchName, string matchPassword, int eloScore, bool filterOutPrivateMatches)
        {
            return UnityTask<List<MatchInfoSnapshot>>.Run(ListMatchesCoroutine(matchName, matchPassword, eloScore, filterOutPrivateMatches));
        }

        public static UnityTask<MatchInfo> JoinMatch(NetworkID networkId, string matchPassword, int eloScoreForClient)
        {
            return UnityTask<MatchInfo>.Run(JoinMatchCoroutine(networkId, matchPassword, eloScoreForClient));
        }

        public static UnityTask<bool> DestroyMatch(MatchInfo matchInfo)
        {
            return UnityTask<bool>.Run(DestroyMatchCoroutine(matchInfo));
        }        

        private static IEnumerator<MatchInfo> CreateMatchCoroutine(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, int eloScore)
        {
            if (hasStartedMatchMaker == false)
            {
                NetworkManager.singleton.StartMatchMaker();
                hasStartedMatchMaker = true;
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            MatchInfo matchInfoResult = null;
                        
            NetworkManager.singleton.matchMaker.CreateMatch(
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
            if (hasStartedMatchMaker == false)
            {
                NetworkManager.singleton.StartMatchMaker();
                hasStartedMatchMaker = true;
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            List<MatchInfoSnapshot> matchesResult = null;

            NetworkManager.singleton.matchMaker.ListMatches(
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

        private static IEnumerator<MatchInfo> JoinMatchCoroutine(NetworkID networkId, string matchPassword, int eloScoreForClient)
        {
            if (hasStartedMatchMaker == false)
            {
                NetworkManager.singleton.StartMatchMaker();
                hasStartedMatchMaker = true;
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;
            MatchInfo matchInfoResult = null;

            NetworkManager.singleton.matchMaker.JoinMatch(
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
            if (hasStartedMatchMaker == false)
            {
                NetworkManager.singleton.StartMatchMaker();
                hasStartedMatchMaker = true;
            }

            bool isDone = false;
            bool successResult = false;
            string extendedInfoResult = null;

            NetworkManager.singleton.matchMaker.DestroyMatch(
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
    }
}
