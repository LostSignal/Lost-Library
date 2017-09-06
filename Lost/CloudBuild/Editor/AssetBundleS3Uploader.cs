//-----------------------------------------------------------------------
// <copyright file="AssetBundleS3Uploader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Diagnostics;

    public class AssetBundleS3Uploader
    {
        public static void UploadDirectoryToS3WithJavaUploader(string accessKey, string secretKey, string bucket, string folder, string key)
        {
            UnityEngine.Debug.LogFormat("Environment.OSVersion: {0}", Environment.OSVersion);

            // making sure the key ends with a '/' character
            if (string.IsNullOrEmpty(key) == false)
            {
                key = key.AppendIfDoesntExist("/");
            }

            var executable = "java";
            var arguments = string.Format("-jar AssetBundleUploader.jar {0} {1} {2} {3} {4}", accessKey, secretKey, bucket, folder, key);
            var startInfo = new ProcessStartInfo(executable, arguments);

            // redirecting standard error/out
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            // starting the process and waiting
            var process = Process.Start(startInfo);

            // getting the standard out
            string output = process.StandardOutput.ReadToEnd();

            if (string.IsNullOrEmpty(output) == false)
            {
                UnityEngine.Debug.Log(output);
            }

            // getting the standard error
            var error = process.StandardError.ReadToEnd();

            if (string.IsNullOrEmpty(error) == false)
            {
                UnityEngine.Debug.LogError(error);
            }

            // waiting for exit
            process.WaitForExit();
        }

        public static void UploadDirectoryToS3WithAWSCLI(string accessKey, string screteKey, string region, string folder, string bucket, string key)
        {
            UnityEngine.Debug.LogFormat("Environment.OSVersion: {0}", Environment.OSVersion);

            // making sure the key ends with a '/' character
            if (string.IsNullOrEmpty(key) == false)
            {
                key = key.AppendIfDoesntExist("/");
            }

            var executable = "aws";
            var arguments = string.Format("s3 cp {0} s3://{1}/{2} --recursive", folder, bucket, key);
            var startInfo = new ProcessStartInfo(executable, arguments);

            // redirecting standard error/out
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            // setting up environment variables
            startInfo.EnvironmentVariables.Add("AWS_ACCESS_KEY_ID", accessKey);
            startInfo.EnvironmentVariables.Add("AWS_SECRET_ACCESS_KEY", screteKey);
            startInfo.EnvironmentVariables.Add("AWS_DEFAULT_REGION", region);

            // starting the process and waiting
            var process = Process.Start(startInfo);
            
            // getting the standard out
            string output = process.StandardOutput.ReadToEnd();
            
            if (string.IsNullOrEmpty(output) == false)
            {
                UnityEngine.Debug.Log(output);
            }
            
            // getting the standard error
            var error = process.StandardError.ReadToEnd();
            
            if (string.IsNullOrEmpty(error) == false)
            {
                UnityEngine.Debug.LogError(error);
            }
            
            // waiting for exit
            process.WaitForExit();
        }
    }
}
