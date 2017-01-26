//-----------------------------------------------------------------------
// <copyright file="IdGeneratorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    
    public static class IdGeneratorUtil
    {
        private static Random rand = new Random((int)System.DateTime.Now.ToFileTimeUtc());
        
        public static ushort GenerateUniqueUshort(HashSet<ushort> taken)
        {
            var valid = new List<ushort>(256);

            for (ushort i = 0; i < 256; i++)
            {
                if (taken == null || taken.Contains(i) == false)
                {
                    valid.Add(i);
                }
            }

            return valid[(int)(valid.Count * rand.NextDouble())];
        }
    }
}
