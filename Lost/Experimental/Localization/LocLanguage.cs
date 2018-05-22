//-----------------------------------------------------------------------
// <copyright file="LocLanguage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class LocLanguage
    {
        public static readonly LocLanguage English = CreateLocLanguage(1, "English", "Eng");
        public static readonly LocLanguage French = CreateLocLanguage(2, "French", "Fre");
        public static readonly LocLanguage Italian = CreateLocLanguage(3, "Italian", "Ita");
        public static readonly LocLanguage German = CreateLocLanguage(4, "German", "Ger");
        public static readonly LocLanguage Spanish = CreateLocLanguage(5, "Spanish", "Spa");
        public static readonly LocLanguage ChineseManderin = CreateLocLanguage(6, "Mandarin", "Man");
        public static readonly LocLanguage Japanese = CreateLocLanguage(7, "Japanese", "Jpn");
        public static readonly LocLanguage Korean = CreateLocLanguage(8, "Korean", "Kor");
        public static readonly LocLanguage Russian = CreateLocLanguage(9, "Russian", "Rus");
        public static readonly LocLanguage Portuguese = CreateLocLanguage(10, "Portuguese", "Por");

        private static readonly List<LocLanguage> AllLanguages = new List<LocLanguage>();

        private LocLanguage()
        {
        }

        public static IEnumerable<LocLanguage> Languages
        {
            get { return AllLanguages; }
        }

        public ushort Id { get; private set; }

        public string Name { get; private set; }

        public string ShortName { get; private set; }

        public static LocLanguage GetCurrentOSLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English: return English;
                case SystemLanguage.French: return French;
                case SystemLanguage.Italian: return Italian;
                case SystemLanguage.German: return German;
                case SystemLanguage.Spanish: return Spanish;
                case SystemLanguage.Chinese: return ChineseManderin;
                case SystemLanguage.Japanese: return Japanese;
                case SystemLanguage.Korean: return Korean;
                case SystemLanguage.Russian: return Russian;
                case SystemLanguage.Portuguese: return Portuguese;
                default: return English;
            }
        }

        private static LocLanguage CreateLocLanguage(ushort id, string name, string shortName)
        {
            LocLanguage newLan = new LocLanguage
            {
                Id = id,
                Name = name,
                ShortName = shortName
            };

            AllLanguages.Add(newLan);

            return newLan;
        }
    }
}
