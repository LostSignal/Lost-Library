//-----------------------------------------------------------------------
// <copyright file="JsonServiceClient.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    //// TODO how should I handle errors that SendGetRequest and SendPostRequest return.  Is returning error codes the right way to go?  
    ////
    //// TODO most definitely need to turn these into Coroutines that run in the background
    //// https://github.com/TwistedOakStudios/TOUnityUtilities/blob/master/Assets/TOUtilities/SpecialCoroutines.cs
    //// https://www.youtube.com/watch?v=ciDD6Wl-Evk
    
    public abstract class JsonServiceClient
    {
        private string server;

        protected JsonServiceClient(string server)
        {
            this.server = server;
        }

        protected T SendGetRequest<T>(string url)
        {
            var www = new WWW(this.server + url);

            while (www.isDone == false)
            {
                ThreadUtil.SleepInMillis(1);
            }

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogErrorFormat("WWW Error: {0}", www.error);
            }

            // throw exception if we time out / bad connection
            // throw exception if got a bad response
            // throw exception if couldn't correctly deserialize response
            return JsonUtility.FromJson<T>(www.text);
        }

        protected int SendPostRequest<T>(string url, T request)
        {
            string requestJson = JsonUtility.ToJson(request);
            byte[] requestJsonBytes = System.Text.Encoding.UTF8.GetBytes(requestJson);

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            
            //// headers.Add("securityKey", this.GetSecurityKey(requestJson));

            var www = new WWW(this.server + url, requestJsonBytes, headers);

            while (www.isDone == false)
            {
                ThreadUtil.SleepInMillis(1);
            }

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogErrorFormat("WWW Error: {0}", www.error);
            }

            // throw exception if we time out / bad connection
            // throw exception if got a bad response
            // throw exception if couldn't correctly deserialize response
            return JsonUtility.FromJson<int>(www.text);
        } 

        private string GetSecurityKey(string json)
        {
            return "key";
        }
    }
}
