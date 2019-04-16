//-----------------------------------------------------------------------
// <copyright file="S3.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

////
//// Special Thanks to everyone that posted on this thread. Without it, I wouldn't have gotten this code to work.
//// https://stackoverflow.com/questions/16917768/upload-to-s3-from-httpwebresponse-getresponsestream-in-c-sharp
////

namespace Lost.Addressables
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public class S3
    {
        private const string ContentType = "application/octet-stream";
        private const string Method = "PUT";

        public static void UploadFile(Config config, string key, byte[] data, bool useUnityWebRequest)
        {
            string today = String.Format("{0:ddd,' 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' 'zz00}", DateTime.Now);

            StringBuilder stringToSignBuilder = new StringBuilder();
            stringToSignBuilder.Append("PUT");
            stringToSignBuilder.Append("\n\n");
            stringToSignBuilder.Append(ContentType);
            stringToSignBuilder.Append("\n\n");
            stringToSignBuilder.Append("x-amz-date:");
            stringToSignBuilder.Append(today);
            stringToSignBuilder.Append("\n");
            stringToSignBuilder.Append("/");
            stringToSignBuilder.Append(config.BucketName);
            stringToSignBuilder.Append("/");
            stringToSignBuilder.Append(key);

            Encoding utf8Encoder = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(utf8Encoder.GetBytes(config.SecretKeyId));
            string encodedCanonical = Convert.ToBase64String(signature.ComputeHash(utf8Encoder.GetBytes(stringToSignBuilder.ToString())));

            string authHeader = "AWS " + config.AccessKeyId + ":" + encodedCanonical;
            string uri = "http://" + config.BucketName + ".s3.amazonaws.com/" + key;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", authHeader);
            headers.Add("x-amz-date", today);

            if (useUnityWebRequest)
            {
                #if UNITY_2018_3_OR_NEWER
                SendWithUnityWebRequest(uri, headers, data);
                #else
                UnityEngine.Debug.LogError("Uploading file to S3 with UnityWebRequest not allowed on non-unity platforms");
                #endif
            }
            else
            {
                SendWithHttpWebRequest(uri, headers, data);
            }
        }

        private static void SendWithHttpWebRequest(string uri, Dictionary<string, string> headers, byte[] data)
        {
            var reqS3 = (HttpWebRequest)WebRequest.Create(uri);

            foreach (var pair in headers)
            {
                reqS3.Headers.Add(pair.Key, pair.Value);
            }

            reqS3.ContentType = ContentType;
            reqS3.ContentLength = data.Length;
            reqS3.Method = Method;

            var rquestStream = reqS3.GetRequestStream();
            rquestStream.Write(data, 0, data.Length);
            rquestStream.Close();

            reqS3.GetResponse();
        }

        #if UNITY_2018_3_OR_NEWER
        private static void SendWithUnityWebRequest(string uri, Dictionary<string, string> headers, byte[] data)
        {
            using (var www = UnityEngine.Networking.UnityWebRequest.Put(uri, data))
            {
                // setting the headers
                foreach (var pair in headers)
                {
                    www.SetRequestHeader(pair.Key, pair.Value);
                }

                #pragma warning disable
                var s = www.Send();
                #pragma warning restore

                long startTick = DateTime.Now.Ticks;
                while (!s.isDone)
                {
                    if (DateTime.Now.Ticks > startTick + 10L * 10000000L)
                    {
                        UnityEngine.Debug.LogWarning("Timeout");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(www.error))
                {
                    UnityEngine.Debug.Log("Successful!");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Error : " + www.error);
                }
            }
        }
        #endif

        public class Config
        {
            public string BucketName { get; set; }
            public string AccessKeyId { get; set; }
            public string SecretKeyId { get; set; }
        }
    }
}
