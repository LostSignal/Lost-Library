//-----------------------------------------------------------------------
// <copyright file="DictionaryTest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using NUnit.Framework;
    using UnityEditor;

    public class DictionaryTest
    {
        [Test]
        public void Validate()
        {
            string[] dictionaryAssetGuids = AssetDatabase.FindAssets("DefaultDictionary");
            Assert.True(dictionaryAssetGuids.Length == 1, "Couldn't find default dictionary to test with.");

            string dictionaryAssetPath = AssetDatabase.GUIDToAssetPath(dictionaryAssetGuids[0]);
            var dictionary = AssetDatabase.LoadAssetAtPath(dictionaryAssetPath, typeof(Dictionary)) as Dictionary;

            Assert.NotNull(dictionary);

            Assert.True(dictionary.IsValidWord("aa"));
            Assert.True(dictionary.IsValidWord("zzzs"));
            Assert.True(dictionary.IsValidWord("commendableness"));
            Assert.True(dictionary.IsValidWord("redistributions"));
            Assert.True(dictionary.IsValidWord("unbracketed"));

            Assert.False(dictionary.IsValidWord(string.Empty));
            Assert.False(dictionary.IsValidWord(null));
            Assert.False(dictionary.IsValidWord("unbowings"));
            Assert.False(dictionary.IsValidWord("brian"));
        }
    }
}
