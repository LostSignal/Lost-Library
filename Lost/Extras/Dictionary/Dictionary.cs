//-----------------------------------------------------------------------
// <copyright file="Dictionary.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu]
    public class Dictionary : ScriptableObject
    {
        private const int LetterCount = 26;

        #pragma warning disable 0649
        [SerializeField, HideInInspector]
        private string[] words;
        #pragma warning restore 0649

        private int[] availableCharacterCounts = new int[LetterCount];
        private int[] wordCharacterCounts = new int[LetterCount];

        public int WordCount
        {
            get { return this.words != null ? this.words.Length : 0; }
        }

        public bool IsValidWord(string word)
        {
            int index = Array.BinarySearch<string>(this.words, word);
            return index >= 0 && index <= (this.words.Length - 1);
        }

        public List<string> FindValidWords(List<char> availableCharacters)
        {
            List<string> results = new List<string>();
            this.GetCharCountsFromLetterList(this.availableCharacterCounts, availableCharacters);

            for (int i = 0; i < this.words.Length; i++)
            {
                this.GetCharCountsFromWord(this.wordCharacterCounts, ref this.words[i]);
                bool isValidWord = true;

                for (int j = 0; j < LetterCount; j++)
                {
                    if (this.wordCharacterCounts[j] > this.availableCharacterCounts[j])
                    {
                        isValidWord = false;
                        break;
                    }
                }

                if (isValidWord)
                {
                    results.Add(this.words[i]);
                }
            }

            return results;
        }

        #if UNITY_EDITOR
        public void SetWords(string[] words)
        {
            this.words = words;
        }
        #endif

        private void GetCharCountsFromWord(int[] charCounts, ref string word)
        {
            this.ResetCharCounts(charCounts);

            for (int i = 0; i < word.Length; i++)
            {
                charCounts[word[i] - 'a']++;
            }
        }

        private void GetCharCountsFromLetterList(int[] charCounts, List<char> leters)
        {
            this.ResetCharCounts(charCounts);

            for (int i = 0; i < leters.Count; i++)
            {
                charCounts[leters[i] - 'a']++;
            }
        }

        private void ResetCharCounts(int[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = 0;
            }
        }
    }
}
