//-----------------------------------------------------------------------
// <copyright file="BetterStringBuilder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;

    public enum IntFormat
    {
        Plain,
        ThousandsSeperated,
    }

    public struct BetterStringBuilder
    {
        private static readonly char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly char[] charBuffer = new char[512];
        private static int currentLength;

        public static BetterStringBuilder New()
        {
            currentLength = 0;
            return new BetterStringBuilder();
        }

        public BetterStringBuilder Append(string value)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    charBuffer[currentLength++] = value[i];
                }
            }

            return this;
        }

        public BetterStringBuilder Append(char value)
        {
            charBuffer[currentLength++] = value;
            return this;
        }

        public BetterStringBuilder Append(int value, IntFormat format = IntFormat.Plain)
        {
            return this.Append((long)value, format);
        }

        public BetterStringBuilder Append(long value, IntFormat format = IntFormat.Plain)
        {
            switch (format)
            {
                case IntFormat.Plain:
                    return this.AppendLong(value, false);

                case IntFormat.ThousandsSeperated:
                    return this.AppendLong(value, true);

                default:
                    Debug.LogErrorFormat("Found Unknown IntFormat {0}", format);
                    return this.AppendLong(value, false);
            }
        }

        public void Set(TMP_Text text)
        {
            text.SetCharArray(charBuffer, 0, currentLength);
        }

        public override string ToString()
        {
            return new string(charBuffer, 0, currentLength);
        }

        private BetterStringBuilder AppendLong(long value, bool showThousandsSeperator)
        {
            string thousandsSeperator = showThousandsSeperator ? Localization.GetThousandsSeperator() : string.Empty;

            if (value < 0)
            {
                charBuffer[currentLength++] = '-';
                value *= -1;
            }

            int digitCount = 1;
            int divisor = 10;

            while (value / divisor != 0)
            {
                divisor *= 10;
                digitCount++;
            }

            for (int i = 0; i < digitCount; i++)
            {
                divisor /= 10;
                charBuffer[currentLength++] = digits[(value / divisor) % 10];

                if (divisor == 1000 || divisor == 1000000 || divisor == 1000000000)
                {
                    this.Append(thousandsSeperator);
                }
            }

            return this;
        }
    }
}
