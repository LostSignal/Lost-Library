//-----------------------------------------------------------------------
// <copyright file="NetworkUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public static class NetworkUtil
    {
        public static void SetBit(ref int bytes, int bitIndex, bool value)
        {
            if (value)
            {
                bytes |= 1 << bitIndex;
            }
            else
            {
                bytes &= ~(1 << bitIndex);
            }
        }

        public static bool GetBit(int bytes, int bitIndex)
        {
            return ((1 << bitIndex) & bytes) > 0;
        }
    }
}
