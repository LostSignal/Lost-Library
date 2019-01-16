//-----------------------------------------------------------------------
// <copyright file="AzureStorage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

////
//// Special thanks to robinsh for the github depot that helped me figure out how to get this working
//// https://github.com/Azure-Samples/storage-dotnet-rest-api-with-auth
////

namespace Lost.Addressables
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public class AzureStorage
    {
        private const string ContentType = "application/octet-stream";
        private const string GetMethod = "GET";
        private const string PutMethod = "PUT";

        public static string UploadFile(Config config, string blob, byte[] data)
        {
            // Making sure to escape the blob name
            blob = Uri.EscapeUriString(blob);

            var uri = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", config.StorageAccountName, config.ContainerName, blob);

            var headers = new Dictionary<string, string>
            {
                { "x-ms-blob-type", "BlockBlob" },
            };

            StringBuilder resource = new StringBuilder();
            resource.Append("/");
            resource.Append(config.StorageAccountName);
            resource.Append("/");
            resource.Append(config.ContainerName);
            resource.Append("/");
            resource.Append(blob);

            AddDefaultAndAuthorizationHeaders(config, headers, resource.ToString(), data);

            return SendWithHttpWebRequest(uri, headers, data);
        }

        public static List<string> ListFiles(Config config, string prefix)
        {
            // Making sure to escape the prefix name
            prefix = Uri.EscapeUriString(prefix);

            var uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container&comp=list&prefix={2}", config.StorageAccountName, config.ContainerName, prefix);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            StringBuilder resource = new StringBuilder();
            resource.Append("/");
            resource.Append(config.StorageAccountName);
            resource.Append("/");
            resource.Append(config.ContainerName);
            resource.Append("\n");
            resource.Append("comp:list");
            resource.Append("\n");
            resource.Append("prefix:" + prefix);
            resource.Append("\n");
            resource.Append("restype:container");

            AddDefaultAndAuthorizationHeaders(config, headers, resource.ToString(), null);

            string xml = SendWithHttpWebRequest(uri, headers, null);

            List<string> fileNames = new List<string>();
            string startNameXml = "<Blob><Name>";
            string endNameXml = "</Name><Properties>";

            int currentIndex = 0;

            while (true)
            {
                currentIndex = xml.IndexOf(startNameXml, currentIndex);

                if (currentIndex == -1)
                {
                    break;
                }

                int startIndex = currentIndex + startNameXml.Length;
                int endIndex = xml.IndexOf(endNameXml, startIndex);
                string name = xml.Substring(startIndex, endIndex - startIndex);
                fileNames.Add(name);

                currentIndex = endIndex;
            }

            return fileNames;
        }

        private static void AddDefaultAndAuthorizationHeaders(Config config, Dictionary<string, string> headers, string resource, byte[] content)
        {
            DateTime now = DateTime.UtcNow;

            // Adding default headers
            headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
            headers.Add("x-ms-version", "2017-04-17");

            // Generating Message Signature
            StringBuilder messageSignatureBuilder = new StringBuilder();
            messageSignatureBuilder.Append(content == null ? GetMethod : PutMethod);
            messageSignatureBuilder.Append("\n\n\n");
            messageSignatureBuilder.Append(content == null ? string.Empty : content.Length.ToString());
            messageSignatureBuilder.Append("\n\n");
            messageSignatureBuilder.Append(content == null ? string.Empty : ContentType);
            messageSignatureBuilder.Append("\n\n\n\n\n\n\n");

            foreach (var headerPair in headers)
            {
                messageSignatureBuilder.Append(headerPair.Key);
                messageSignatureBuilder.Append(":");
                messageSignatureBuilder.Append(headerPair.Value);
                messageSignatureBuilder.Append("\n");
            }

            messageSignatureBuilder.Append(resource);

            // Calculating messageSignature SHA256
            string messageSignature = messageSignatureBuilder.ToString();
            byte[] signatureBytes = Encoding.UTF8.GetBytes(messageSignature);
            HMACSHA256 SHA256 = new HMACSHA256(Convert.FromBase64String(config.StorageAccountKey));

            // Adding the final Authorization header
            headers.Add("Authorization", "SharedKey " + config.StorageAccountName + ":" + Convert.ToBase64String(SHA256.ComputeHash(signatureBytes)));
        }

        private static string SendWithHttpWebRequest(string uri, Dictionary<string, string> headers, byte[] data)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);

            foreach (var pair in headers)
            {
                webRequest.Headers.Add(pair.Key, pair.Value);
            }

            webRequest.Method = data == null ? GetMethod : PutMethod;
            webRequest.ContentLength = data == null ? 0 : data.Length;

            // If data exits, set ContentType and write all the data into the RequestStream
            if (data != null)
            {
                webRequest.ContentType = ContentType;

                var rquestStream = webRequest.GetRequestStream();
                rquestStream.Write(data, 0, data.Length);
                rquestStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            using (Stream responseStream = webResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public class Config
        {
            public string StorageAccountName { get; set; }
            public string StorageAccountKey { get; set; }
            public string ContainerName { get; set; }
        }
    }
}
