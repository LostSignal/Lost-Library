//-----------------------------------------------------------------------
// <copyright file="Obfuscator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public static class Obfuscator
    {
        private const int byteObfuscatorLength = 4 * 1024; // 4 kilobytes
        private const int byteSeed = 830378379;
        private static List<byte> byteObfuscatorList;

        static Obfuscator()
        {
            var random = new System.Random(byteSeed);
            int byteCount = byteObfuscatorLength;
            byteObfuscatorList = new List<byte>(byteCount);

            for (int i = 0; i < byteCount; i++)
            {
                byteObfuscatorList.Add((byte)random.Next(0, 255));
            }
        }

        public static string ObfuscateString(string sourceString)
        {
            byte[] sourceStringBytes = System.Text.Encoding.UTF8.GetBytes(sourceString);

            for (int i = 0; i < sourceStringBytes.Length; i++)
            {
                sourceStringBytes[i] ^= byteObfuscatorList[i % byteObfuscatorLength];
            }

            return Convert.ToBase64String(sourceStringBytes);
        }

        public static string DeobfuscateString(string obfuscatedString)
        {
            byte[] obfuscatedStringBytes = Convert.FromBase64String(obfuscatedString);

            for (int i = 0; i < obfuscatedStringBytes.Length; i++)
            {
                obfuscatedStringBytes[i] ^= byteObfuscatorList[i % byteObfuscatorLength];
            }

            return System.Text.Encoding.UTF8.GetString(obfuscatedStringBytes);
        }
    }
}
