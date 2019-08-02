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
        Abbreviated = 100,
    }

    public struct BetterStringBuilder
    {
        private const long Billion = 1000000000;
        private const long Million = 1000000;
        private const long Thousand = 1000;

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

        public BetterStringBuilder AppendLine()
        {
            return this.Append("\n");
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

        public BetterStringBuilder AppendTwoDigitNumber(int value)
        {
            if (value >= 100)
            {
                Debug.LogWarning("AppendTwoDigitNumber was given a number greater than 2 digits.");
            }

            if (value >= 10)
            {
                return this.Append((long)value, IntFormat.Plain);
            }
            else
            {
                return this.Append(0).Append((long)value, IntFormat.Plain);
            }
        }

        public BetterStringBuilder Append(long value, IntFormat format = IntFormat.Plain)
        {
            switch (format)
            {
                case IntFormat.Plain:
                    return this.AppendLong(value, false);

                case IntFormat.ThousandsSeperated:
                    return this.AppendLong(value, true);

                case IntFormat.Abbreviated:
                    return this.AppendAbbreviated(value);

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
            string thousandsSeperator = showThousandsSeperator ? Localization.Localization.GetThousandsSeperator() : string.Empty;

            if (value < 0)
            {
                charBuffer[currentLength++] = '-';
                value *= -1;
            }

            int digitCount = 1;
            long divisor = 10;

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

        private BetterStringBuilder AppendAbbreviated(long value)
        {
            long number = value;
            long remainder = 0;
            string postfix = string.Empty;

            if (value >= Billion)
            {
                number = value / Billion;
                remainder = (value - (number * Billion)) / (Billion / 10);
                postfix = "B";
            }
            else if (value >= Million)
            {
                number = value / Million;
                remainder = (value - (number * Million)) / (Million / 10);
                postfix = "M";
            }
            else if (value >= Thousand)
            {
                number = value / Thousand;
                remainder = (value - (number * Thousand)) / (Thousand / 10);
                postfix = "K";
            }

            if (remainder != 0)
            {
                return BetterStringBuilder.New()
                    .Append(number)
                    .Append(Localization.Localization.GetDecimalPointSeperator())
                    .Append(remainder)
                    .Append(postfix);
            }
            else
            {
                return BetterStringBuilder.New().Append(number).Append(postfix);
            }
        }
    }
}
