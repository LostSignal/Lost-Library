//-----------------------------------------------------------------------
// <copyright file="MatchMakingHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.Types;

    public static class MatchMakingHelper
    {
        // matchmaking configuration
        private static NetworkMatch matchMaker = null;
        private static string matchHost = "mm.unet.unity3d.com";
        private static int matchPort = 443;
        
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

        public static UnityTask<bool> DropConnection(MatchInfo matchInfo)
        {
            return UnityTask<bool>.Run(DropConnectionCoroutine(matchInfo));
        }

        private static IEnumerator<MatchInfo> CreateMatchCoroutine(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, int eloScore)
        {
            InitializeMatchMaker();

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
            InitializeMatchMaker();

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

        private static IEnumerator<MatchInfo> JoinMatchCoroutine(NetworkID networkId, string matchPassword, int eloScoreForClient)
        {
            InitializeMatchMaker();

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
            InitializeMatchMaker();

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
            InitializeMatchMaker();

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

        private static void InitializeMatchMaker()
        {
            if (matchMaker != null)
            {
                return;
            }

            var networkMatchObject = SingletonUtil.GetOrCreateSingletonChildObject("NetworkMatch").gameObject;
            matchMaker = networkMatchObject.AddComponent<NetworkMatch>();

            SetMatchHost(matchHost, matchPort, matchPort == 443);
        }

        private static void SetMatchHost(string newHost, int port, bool isHttps)
        {
            if (newHost == "127.0.0.1")
            {
                newHost = "localhost";
            }
            
            if (newHost.StartsWith("http://"))
            {
                newHost = newHost.Replace("http://", "");
            }

            if (newHost.StartsWith("https://"))
            {
                newHost = newHost.Replace("https://", "");
            }

            string prefix = isHttps ? "https://" : "http://";
            matchHost = newHost;
            matchPort = port;

            matchMaker.baseUri = new Uri(prefix + matchHost + ":" + matchPort);
        }
    }
}
