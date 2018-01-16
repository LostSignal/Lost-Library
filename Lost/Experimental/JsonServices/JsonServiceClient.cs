//-----------------------------------------------------------------------
// <copyright file="JsonServiceClient.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    //// NOTE [bgish]: I have absolutely no clue if this even works anymore
    ////
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

        protected string Server
        {
            get { return this.server; }
        }

        protected IEnumerator<U> SendPostRequest<T, U>(string url, T request)
        {
            string requestJson = JsonUtility.ToJson(request);
            byte[] requestJsonBytes = System.Text.Encoding.UTF8.GetBytes(requestJson);

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            //// headers.Add("securityKey", this.GetSecurityKey(requestJson));

            var www = new WWW(this.Server + url, requestJsonBytes, headers);

            while (www.isDone == false)
            {
                yield return default(U);
            }

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogErrorFormat("WWW Error: {0}", www.error);
                throw new System.Exception(www.error);
            }

            // TODO [bgish]: Throw exception if we time out / bad connection
            // TODO [bgish]: Throw exception if got a bad response
            // TODO [bgish]: Throw exception if couldn't correctly deserialize response
            yield return JsonUtility.FromJson<U>(www.text);
        }

        // public UnityTask<T> SendGetRequest<T>(string url)
        // {
        //     return UnityTask<T>.Run(this.SendGetRequestInternal<T>(url));
        // }
        // 
        // public UnityTask<int> SendPostRequest<T>(string url, T request)
        // {
        //     return UnityTask<int>.Run(this.SendPostRequestInternal<T>(url, request));
        // }
        // 
        // private IEnumerator<T> SendGetRequestInternal<T>(string url)
        // {
        //     var www = new WWW(Path.Combine(this.server, url));
        // 
        //     while (www.isDone == false)
        //     {
        //         yield return default(T);
        //     }
        // 
        //     if (string.IsNullOrEmpty(www.error) == false)
        //     {
        //         Debug.LogErrorFormat("WWW Error: {0}", www.error);
        //         throw new System.Exception(www.error);
        //     }
        // 
        //     // TODO [bgish]: Throw exception if we time out / bad connection
        //     // TODO [bgish]: Throw exception if got a bad response
        //     // TODO [bgish]: Throw exception if couldn't correctly deserialize response
        //     yield return JsonUtility.FromJson<T>(www.text);
        // }
        // 
        // private IEnumerator<int> SendPostRequestInternal<T>(string url, T request)
        // {
        //     string requestJson = JsonUtility.ToJson(request);
        //     byte[] requestJsonBytes = System.Text.Encoding.UTF8.GetBytes(requestJson);
        // 
        //     var headers = new Dictionary<string, string>();
        //     headers.Add("Content-Type", "application/json");
        //     
        //     //// headers.Add("securityKey", this.GetSecurityKey(requestJson));
        // 
        //     var www = new WWW(this.server + url, requestJsonBytes, headers);
        // 
        //     while (www.isDone == false)
        //     {
        //         yield return default(int);
        //     }
        // 
        //     if (string.IsNullOrEmpty(www.error) == false)
        //     {
        //         Debug.LogErrorFormat("WWW Error: {0}", www.error);
        //         throw new System.Exception(www.error);
        //     }
        // 
        //     // TODO [bgish]: Throw exception if we time out / bad connection
        //     // TODO [bgish]: Throw exception if got a bad response
        //     // TODO [bgish]: Throw exception if couldn't correctly deserialize response
        //     yield return JsonUtility.FromJson<int>(www.text);
        // } 
        // 
        // private string GetSecurityKey(string json)
        // {
        //     return "key";
        // }
    }
}
