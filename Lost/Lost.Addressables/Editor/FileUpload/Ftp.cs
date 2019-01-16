//-----------------------------------------------------------------------
// <copyright file="Ftp.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Addressables
{
    using System;
    using System.IO;
    using System.Net;
    using UnityEngine;

    public class Ftp
    {
        public static void UploadDirectory(string assetBundleRelativeDirectory, string ftpUrl, string username, string password)
        {
            var assetBundleDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), assetBundleRelativeDirectory);
            var assetBundleDirectory = new DirectoryInfo(assetBundleDirectoryPath);

            // making sure the given folder exists
            Debug.LogFormat("Creating FTP Directory: {0}", ftpUrl);
            CreateFolderOnFTP(ftpUrl, username, password);

            // figuring out which directories to create
            foreach (var directory in assetBundleDirectory.GetDirectories("*", SearchOption.AllDirectories))
            {
                var newFtpDirectory = Path.Combine(ftpUrl, directory.FullName.Substring(assetBundleDirectoryPath.Length + 1)).Replace("\\", "/");
                Debug.LogFormat("Creating FTP Directory: {0}", newFtpDirectory);

                CreateFolderOnFTP(newFtpDirectory, username, password);
            }

            foreach (var file in assetBundleDirectory.GetFiles("*", SearchOption.AllDirectories))
            {
                var newFtpFile = Path.Combine(ftpUrl, file.FullName.Substring(assetBundleDirectoryPath.Length + 1)).Replace("\\", "/");
                Debug.LogFormat("Creating New FTP File: {0}", newFtpFile);

                UploadToFTP(newFtpFile, file.FullName, username, password);
            }
        }

        private static FtpStatusCode CreateFolderOnFTP(String fullFtpDirectoryPath, String inUsername, String inPassword)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(fullFtpDirectoryPath);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(inUsername, inPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            try
            {
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    return resp.StatusCode;
                }
            }
            catch (WebException webException)
            {
                if (webException.Message.ToLower().Contains("file exists"))
                {
                    // ignore it since it already exists
                    return FtpStatusCode.CommandOK;
                }
                else
                {
                    throw webException;
                }
            }
        }

        private static void UploadToFTP(String fullFtpFilePath, String fullLocalFilePath, String inUsername, String inPassword)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(fullFtpFilePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(inUsername, inPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            byte[] buffer = null;

            // Create a stream from the file
            using (FileStream stream = File.OpenRead(fullLocalFilePath))
            {
                buffer = new byte[stream.Length];

                // Read the file into the a local stream
                stream.Read(buffer, 0, buffer.Length);
            }

            // making sure the buffer was created
            if (buffer == null)
            {
                return;
            }

            // create a stream to the FTP server
            using (Stream reqStream = request.GetRequestStream())
            {
                // Write the local stream to the FTP stream, 2 kb at a time
                int offset = 0;
                int chunk = (buffer.Length > 2048) ? 2048 : buffer.Length;
                while (offset < buffer.Length)
                {
                    reqStream.Write(buffer, offset, chunk);
                    offset += chunk;
                    chunk = (buffer.Length - offset < chunk) ? (buffer.Length - offset) : chunk;
                }
            }
        }
    }
}
