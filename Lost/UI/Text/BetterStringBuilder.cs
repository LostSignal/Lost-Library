//-----------------------------------------------------------------------
// <copyright file="BetterStringBuilder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;

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
            for (int i = 0; i < value.Length; i++)
            {
                charBuffer[currentLength++] = value[i];
            }

            return this;
        }

        public BetterStringBuilder Append(int value)
        {
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
            }

            return this;
        }

        public void Set(TMP_Text text)
        {
            text.SetCharArray(charBuffer, 0, currentLength);
        }
    }
}
