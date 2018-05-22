//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str)
        {
            if (string.IsNullOrEmpty(str) == false)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsWhiteSpace(str[i]))
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        public static string AppendIfDoesntExist(this string str, string value)
        {
            if (str.EndsWith(value) == false)
            {
                str = str + value;
            }

            return str;
        }

        public static byte[] GetUTF8Bytes(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        //// #if !UNITY_METRO && !UNITY_WP8
        //// private static SHA256 sha256 = SHA256Managed.Create();
        //// public static byte[] GetSHA256Hash(this string str)
        //// {
        ////     return sha256.ComputeHash(str.GetUTF8Bytes());
        //// }
        //// #else
        //// public static byte[] GetSHA256Hash(this string str)
        //// {
        ////     var hasher = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm("SHA256");
        ////
        ////     return null;
        ////     //IBuffer input = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
        ////     //IBuffer hashed = hasher.HashData(input);
        ////     //this.textBase64.Text = CryptographicBuffer.EncodeToBase64String(hashed);
        ////     //this.textHex.Text = CryptographicBuffer.EncodeToHexString(hashed);
        //// }
        //// public static string GetSHA256Key(string hashKey, string stringToSign)
        //// {
        ////     HashAlgorithmProvider hap = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
        ////     CryptographicHash ch = hap.CreateHash();
        ////
        ////     // read in bytes from file then append with ch.Append(data)
        ////
        ////     Windows.Storage.Streams.IBuffer b_hash = ch.GetValueAndReset();       // hash it
        ////     string hash_string = CryptographicBuffer.EncodeToHexString(b_hash);   // encode it to a hex string for easy reading
        //// }
        //// #endif
    }
}
